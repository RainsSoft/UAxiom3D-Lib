#region LGPL License

/*
Axiom Graphics Engine Library
Copyright © 2003-2011 Axiom Project Team

The overall design, and a majority of the core engine and rendering code
contained within this library is a derivative of the open source Object Oriented
Graphics Engine OGRE, which can be found at http://ogre.sourceforge.net.
Many thanks to the OGRE team for maintaining such a high quality project.

This library is free software; you can redistribute it and/or
modify it under the terms of the GNU Lesser General Public
License as published by the Free Software Foundation; either
version 2.1 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public
License along with this library; if not, write to the Free Software
Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA
*/

#endregion LGPL License

#region SVN Version Information

// <file>
//     <license see="http://axiom3d.net/wiki/index.php/license.txt"/>
//     <id value="$Id: StringConverter.cs 2940 2012-01-05 12:25:58Z borrillis $"/>
// </file>

#endregion SVN Version Information

#region Namespace Declarations

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text;

//using Axiom.Math;
using UnityEngine;
using ColorEx = UnityEngine.Color;

#endregion Namespace Declarations

namespace Axiom.Core {
    internal class CaseInsensitiveStringComparer : IEqualityComparer<string> {
        #region IEqualityComparer<string> Members

        public bool Equals(string x, string y) {
            return x.ToLower() == y.ToLower();
        }

        public int GetHashCode(string obj) {
            return obj.ToLower().GetHashCode();
        }

        #endregion IEqualityComparer<string> Members
    }

    /// <summary>
    ///     Helper class for going back and forth between strings and various types.
    /// </summary>
    public sealed class StringConverter {
        #region Fields

        /// <summary>
        ///		Culture info to use for parsing numeric data.
        /// </summary>
        private static CultureInfo englishCulture = new CultureInfo("en-US");

        #endregion Fields

        #region Constructor

        /// <summary>
        ///     Private constructor so no instances can be created.
        /// </summary>
        private StringConverter() { }

        #endregion Constructor

        #region Static Methods

        #region String.Split() replacements
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
#if UNITY_STANDALONE
        //!( XBOX || XBOX360 )
		public static string[] Split( string s, char[] separators )
		{
			return s.Split( separators, 0, StringSplitOptions.None );
		}

		public static string[] Split( string s, char[] separators, int count )
		{
			return s.Split( separators, count, StringSplitOptions.None );
		}
#else

        public static string[] Split(string s, char[] separators, int count) {
            return Split(s, separators, count, StringSplitOptions.None);
        }

        public static string[] Split(string s, char[] separators) {
            return s.Split(separators);
        }

        /// <summary>
        ///     Specifies whether applicable Overload:System.String.Split method overloads
        ///     include or omit empty substrings from the return value.
        /// </summary>
        [Flags]
        public enum StringSplitOptions {
            /// <summary>
            ///     The return value includes array elements that contain an empty string
            /// </summary>
            None = 0,

            /// <summary>
            ///     The return value does not include array elements that contain an empty string
            /// </summary>
            RemoveEmptyEntries = 1,
        }

        /// <summary>
        /// Splits a string into an Array
        /// </summary>
        /// <param name="s">The String to split</param>
        /// <param name="separators">Array of seperators to break the string at</param>
        /// <param name="count">number of elements to return in the array</param>
        /// <returns>An array containing the split strings</returns>
        /// <remarks> Adapted from code supplied by andris11
        /// <para>
        /// If the number of seperators is greater than the count parameter
        /// then the last element will contain the remainder of the string.
        /// </para>
        /// </remarks>
        public static string[] Split(string s, char[] separators, int count, StringSplitOptions options) {
            List<string> results;
            string[] _strings;
            bool removeEmptyEntries;
            bool separatorFound = false;

            //special cases
            UnityEngine.Debug.Assert(s != null, "String instance not set.");

            if (count == 0) {
                _strings = new string[] { };
                return _strings;
            }

            removeEmptyEntries = (options & StringSplitOptions.RemoveEmptyEntries) == StringSplitOptions.RemoveEmptyEntries;
            if (s == String.Empty) {
                _strings = removeEmptyEntries ? new string[] { } : new string[1] { s }; //keep same instance
                return _strings;
            }

            //init
            StringBuilder str = new StringBuilder(s.Length);
            results = new List<string>(s.Length > 10 ? 10 : s.Length);

            if (separators == null || separators.Length == 0)
                separators = new char[] { ' ' };

            //parse
            //TODO: how to handle \n chars? see MSDN examples of String.Split()

            for (int i = 0; i < s.Length; ++i) {
                bool isSeparator = false;

                foreach (char sep in separators) //using foreach with arrays is optimised (.NET2.0)
				{
                    if (s[i] == sep) {
                        isSeparator = true;
                        break;
                    }
                }

                if (isSeparator) {
                    separatorFound = true; //so at least one separator was found

                    if (!(removeEmptyEntries && str.Length == 0)) {
                        results.Add(str.ToString());
                        str.Length = 0;
                    }

                }
                else {
                    str.Append(s[i]);
                }

                if (count > 0 && results.Count == count - 1) {
                    str.Append(s.Substring(i + 1));
                    break; //limit reached
                }

            }

            if (!(count > 0 && results.Count == count)) {
                if (!(removeEmptyEntries && str.Length == 0)) {
                    results.Add(str.ToString());
                }
            }

            //result
            if (!separatorFound) {
                //no separator found, return just the same string
                return new string[1] { s }; //keep same instance, see MSDN
            }
            else {
                return results.ToArray();
            }
        }
#endif

        #endregion String.Split() replacements

        /// <summary>
        ///		Parses a boolean type value
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static bool ParseBool(string val) {
            switch (val.ToLower()) {
                case "true":
                case "on":
                    return true;
                case "false":
                case "off":
                    return false;
            }

            // make the compiler happy
            return false;
        }

        /// <summary>
        ///		Parses an array of params and returns a color from it.
        /// </summary>
        /// <param name="values">[RGBA]</param>
        public static ColorEx ParseColor(string[] values) {
            ColorEx color;
            color.r = ParseFloat(values[0]);
            color.g = ParseFloat(values[1]);
            color.b = ParseFloat(values[2]);
            color.a = (values.Length > 3) ? ParseFloat(values[3]) : 1.0f;

            return color;
        }

        /// <summary>
        ///		Parses an array of params and returns a color from it.
        ///		1,0,0,1 [RGBA]
        ///		1,0,0   [RGB]
        /// </summary>       
        /// <param name="val"></param>
        /// <param name="splitChar">split char default our use ',' </param>
        public static ColorEx ParseColor(string val, char splitChar) {
            ColorEx color;
            string[] vals = val.Split(splitChar);

            color.r = ParseFloat(vals[0]);
            color.g = ParseFloat(vals[1]);
            color.b = ParseFloat(vals[2]);
            color.a = (vals.Length == 4) ? ParseFloat(vals[3]) : 1.0f;

            return color;
        }
        public static Color32 ParseColor32(string val, char splitChar) {
            
            int r, g, b, a;
            string[] vals = val.Split(splitChar);
            ParseInt(vals[0], out r); 
            ParseInt(vals[1], out g);
            ParseInt(vals[2], out b);
            a = 255;
            if (vals.Length >= 4) ParseInt(vals[3], out a);
            Color32 color= new Color32((byte)r, (byte)g, (byte)b, (byte)a);
            return color;
        }
        /// <summary>
        ///		Parses an array of params and returns a color from it.
        /// </summary>
        /// <param name="val">1,0,0   [XYZ]</param>
        public static Vector3 ParseVector3(string[] values) {
            Vector3 vec = new Vector3();
            vec.x = ParseFloat(values[0]);
            vec.y = ParseFloat(values[1]);
            vec.z = ParseFloat(values[2]);

            return vec;
        }

        /// <summary>
        ///		Parses an array of params and returns a color from it.
        /// </summary>
        /// <param name="val">1,0,0   [X,Y,Z]</param>
        public static Vector3 ParseVector3(string val, char splitChar) {
            string[] values = val.Split(splitChar);

            Vector3 vec = new Vector3();
            vec.x = ParseFloat(values[0]);
            vec.y = ParseFloat(values[1]);
            vec.z = ParseFloat(values[2]);

            return vec;
        }

        /// <summary>
        ///		Parses an array of params and returns a color from it.
        /// </summary>
        /// <param name="val"></param>
        public static Vector4 ParseVector4(string[] values) {
            Vector4 vec = new Vector4();
            vec.x = ParseFloat(values[0]);
            vec.y = ParseFloat(values[1]);
            vec.z = ParseFloat(values[2]);
            vec.w = ParseFloat(values[3]);

            return vec;
        }
        public static Vector4 ParseVector4(string val, char splitChar) {
            string[] values = val.Split(splitChar);

            Vector4 vec = new Vector4();
            vec.x = ParseFloat(values[0]);
            vec.y = ParseFloat(values[1]);
            vec.z = ParseFloat(values[2]);
            vec.w = ParseFloat(values[3]);
            return vec;
        }
        /// <summary>
        ///		Parse a float value from a string.
        /// </summary>
        /// <remarks>
        ///		Since our file formats assume the 'en-US' style format for numbers, we need to
        ///		let the framework know that where numbers are being parsed.
        /// </remarks>
        /// <param name="val">String value holding the float.</param>
        /// <returns>A float representation of the string value.</returns>
        public static float ParseFloat(string val) {
            if (val == float.NaN.ToString()) {
                return float.NaN;
            }
            return float.Parse(val, englishCulture);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static string ToString(ColorEx color) {
            return string.Format(englishCulture, "{0},{1},{2},{3}", color.r, color.g, color.b, color.a);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="vec"></param>
        /// <returns></returns>
        public static string ToString(Vector4 vec) {
            return string.Format(englishCulture, "{0},{1},{2},{3}", vec.x, vec.y, vec.z, vec.w);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="vec"></param>
        /// <returns></returns>
        public static string ToString(Vector3 vec) {
            return string.Format(englishCulture, "{0},{1},{2}", vec.x, vec.y, vec.z);
        }

        /// <summary>
        ///     Converts a
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static string ToString(float val) {
            return val.ToString(englishCulture);
        }

        #endregion Static Methods

        public static Quaternion ParseQuaternion(string p) {
            return Quaternion.identity;
            //return Quaternion.Identity;
        }

        public static bool ParseInt(string value, out int num) {
            bool retVal = true;
            try {
                num = System.Convert.ToInt32(value);// Int32.Parse(value);
            }
            catch (Exception e) {
                num = 0;
                retVal = false;
            }
            return retVal;
        }
    }
}
