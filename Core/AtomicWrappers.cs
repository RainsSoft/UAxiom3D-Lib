#region MIT/X11 License

//Copyright © 2003-2012 Axiom 3D Rendering Engine Project
//
//Permission is hereby granted, free of charge, to any person obtaining a copy
//of this software and associated documentation files (the "Software"), to deal
//in the Software without restriction, including without limitation the rights
//to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//copies of the Software, and to permit persons to whom the Software is
//furnished to do so, subject to the following conditions:
//
//The above copyright notice and this permission notice shall be included in
//all copies or substantial portions of the Software.
//
//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//THE SOFTWARE.

#endregion License

#region SVN Version Information

// <file>
//     <license see="http://axiom3d.net/wiki/index.php/license.txt"/>
//     <id value="$Id$"/>
// </file>

#endregion SVN Version Information

#region Namespace Declarations

using System;

#endregion Namespace Declarations

namespace Axiom.Core {
    public class AtomicScalar<T> {
#if UNITY_EDITOR
//UNITY_EDITOR_WIN Platform define for editor code on Windows. 
//UNITY_EDITOR_OSX Platform define for editor code on Mac OSX. 
//UNITY_STANDALONE_OSX Platform define for compiling/executing code specifically for Mac OS (This includes Universal, PPC and Intel architectures). 

#endif
        //(Mac, Windows or Linux).
#if UNITY_STANDALONE  
//UNITY_STANDALONE_OSX Platform define for compiling/executing code specifically for Mac OS (This includes Universal, PPC and Intel architectures). 
//UNITY_STANDALONE_WIN Use this when you want to compile/execute code for Windows stand alone applications. 
//UNITY_STANDALONE_LINUX Use this when you want to compile/execute code for Linux stand alone applications. 

#endif
#if UNITY_WEBPLAYER
// the web play
#endif
        //UNITY_WII Platform define for compiling/executing code for the Wii console. 
        //UNITY_IOS Platform define for compiling/executing code for the iOS platform. 
        //UNITY_IPHONE Deprecated. Use UNITY_IOS instead. 
        //UNITY_ANDROID Platform define for the Android platform. 
        //UNITY_PS3 Platform define for running PlayStation 3 code. 
        //UNITY_PS4 Platform define for running PlayStation 4 code. 
        //UNITY_XBOX360 Platform define for executing Xbox 360 code. 
        //UNITY_XBOXONE Platform define for executing Xbox One code. 
        //UNITY_BLACKBERRY Platform define for a Blackberry10 device. 
        //UNITY_TIZEN Platform define for the Tizen platform. 
        //UNITY_WP8 Platform define for Windows Phone 8. 
        //UNITY_WP8_1 Platform define for Windows Phone 8.1. 
        //UNITY_WSA Platform define for Windows Store Apps (additionally NETFX_CORE is defined when compiling C# files against .NET Core). 
        //UNITY_WSA_8_0 Platform define for Windows Store Apps when targeting SDK 8.0. 
        //UNITY_WSA_8_1 Platform define for Windows Store Apps when targeting SDK 8.1. 
        //UNITY_WINRT Equivalent to UNITY_WP8 | UNITY_WSA. 
        //UNITY_WINRT_8_0 Equivalent to UNITY_WP8 | UNITY_WSA_8_0. 
        //UNITY_WINRT_8_1 Equivalent to UNITY_WP_8_1 | UNITY_WSA_8_1. It’s also defined when compiling against Universal SDK 8.1. 
        //UNITY_WEBGL Platform define for WebGL. 
        #region Fields

        private readonly int _size;
        private const string ERROR_MESSAGE = "Only 16, 32, and 64 bit scalars supported in win32.";

#if !UNITY_STANDALONE
        //WINDOWS_PHONE
        private static readonly object _mutex = new object();
#endif

        #endregion Fields

        #region Properties

        public T Value { get; set; }

        #endregion Properties

        #region Constructors

        public AtomicScalar() {
            var type = typeof(T);
            this._size = type.IsEnum ? 4 : Memory.SizeOf(type);
        }

        public AtomicScalar(T initial)
            : this() {
            Value = initial;
        }

        public AtomicScalar(AtomicScalar<T> cousin)
            : this() {
            Value = cousin.Value;
        }

        #endregion Constructors

        #region Methods

        public bool Cas(T old, T nu) {
            if (this._size == 2 || this._size == 4 || this._size == 8) {
                var f = Convert.ToInt64(Value);
                var o = Convert.ToInt64(old);
                var n = Convert.ToInt64(nu);

#if UNITY_STANDALONE
                //!WINDOWS_PHONE
                var result = System.Threading.Interlocked.CompareExchange(ref f, o, n).Equals(o);
#else
                bool result = false;
                lock ( _mutex )
                {
                    var oldValue = f;
                    if ( f == n )
                        f = o;

                    result = oldValue.Equals( o );
                }
#endif
                Value = _changeType(f);

                return result;
            }
            else {
                throw new AxiomException(ERROR_MESSAGE);
            }
        }

        //[AxiomHelper(0, 9)]
        private static T _changeType(object value) {
            var type = typeof(T);

            if (!type.IsEnum) {
                return (T)Convert.ChangeType(value, type, null);
            }
            else {
                var fields = type.GetFields();
                var idx = ((int)Convert.ChangeType(value, typeof(int), null)) + 1;
                if (fields.Length > 0 && idx < fields.Length) {
                    try {
                        var s = fields[idx].Name;
                        return (T)Enum.Parse(type, s, false);
                    }
                    catch {
                        return default(T);
                    }
                }
                else {
                    return default(T);
                }
            }
        }

        #endregion Methods

        #region Operator overloads

        public static AtomicScalar<T> operator ++(AtomicScalar<T> value) {
            if (value._size == 2 || value._size == 4 || value._size == 8) {
                var v = Convert.ToInt64(value.Value);
#if UNITY_STANDALONE
                //!WINDOWS_PHONE
                System.Threading.Interlocked.Increment(ref v);
#else
                lock ( _mutex )
                {
                    v++;
                }
#endif
                return new AtomicScalar<T>(_changeType(v));
            }
            else {
                throw new AxiomException(ERROR_MESSAGE);
            }
        }

        public static AtomicScalar<T> operator --(AtomicScalar<T> value) {
            if (value._size == 2 || value._size == 4 || value._size == 8) {
                var v = Convert.ToInt64(value.Value);
#if UNITY_STANDALONE
                //!WINDOWS_PHONE
                System.Threading.Interlocked.Decrement(ref v);
#else
                lock ( _mutex )
                {
                    v--;
                }
#endif
                return new AtomicScalar<T>(_changeType(v));
            }
            else {
                throw new AxiomException(ERROR_MESSAGE);
            }
        }

        #endregion Operator overloads
    };
}