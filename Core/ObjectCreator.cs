#region LGPL License

/*
Axiom Graphics Engine Library
Copyright ?2003-2011 Axiom Project Team

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
//     <id value="$Id: ObjectCreator.cs 2940 2012-01-05 12:25:58Z borrillis $"/>
// </file>

#endregion SVN Version Information

#region Namespace Declarations

using System;
using System.Reflection;
using System.IO;
using System.Collections.Generic;

#endregion Namespace Declarations

namespace Axiom.Core {
    /// <summary>
    /// Used by configuration classes to store assembly/class names and instantiate
    /// objects from them.
    /// </summary>
    public class ObjectCreator {
        private Assembly _assembly;
        private Type _type;

        public ObjectCreator(Type type) {
            this._type = type;
        }
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
        public ObjectCreator( string assemblyName, string className )
		{
			string assemblyFile = Path.Combine( System.IO.Directory.GetCurrentDirectory(), assemblyName );
			try
			{
				_assembly = Assembly.LoadFrom( assemblyFile );
			}
			catch( Exception ex )
			{
				_assembly = Assembly.GetExecutingAssembly();
			}

			_type = _assembly.GetType( className );
		}
#endif
        /// <summary>
        /// 根据当前程序集中的 类名初始化
        /// </summary>
        /// <param name="className">当前程序集中的 类名(全路径)</param>
        public ObjectCreator(string className) {
            _assembly = Assembly.GetExecutingAssembly();
            _type = _assembly.GetType(className);
        }

        public ObjectCreator(Assembly assembly, Type type) {
            _assembly = assembly;
            _type = type;
        }

        public string GetAssemblyTitle() {
            Attribute title = Attribute.GetCustomAttribute(_assembly, typeof(AssemblyTitleAttribute));
            if (title == null) {
                return _assembly.GetName().Name;
            }
            return ((AssemblyTitleAttribute)title).Title;
        }
        /// <summary>
        /// 查找指定类型(接口)后再创建对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        [Obsolete("please not using in unity3d")]
        public T CreateInstance<T>() where T : class {
            Type type = _type;
            Assembly assembly = _assembly;
#if UNITY_STANDALONE 
            //!( XBOX || XBOX360 || SILVERLIGHT )
			// Check interfaces or Base type for casting purposes
			if( type.GetInterface( typeof( T ).Name ) != null
			    || type.BaseType.Name == typeof( T ).Name )
			{
#else
            bool typeFound = false;
            for (int i = 0; i < type.GetInterfaces().GetLength(0); i++) {
                if (type.GetInterfaces()[i] == typeof(T)) {
                    typeFound = true;
                    break;
                }
            }

            if (typeFound) {
#endif
                try {
                    return (T)Activator.CreateInstance(type);
                }
                catch (Exception e) {
                    LogManager.Instance.Write("Failed to create instance of {0} of type {0} from assembly {1}", typeof(T).Name, type, assembly == null ? " current assembly" : assembly.FullName);
                    LogManager.Instance.Write(e.Message);
                }
            }
            return null;
        }

        /// <summary>
        /// 根据类名创建 class对象,不进行查找操作
        /// </summary>
        /// <param name="className"></param>
        /// <returns></returns>
        public static object CreateInstance(string className) {
            try {
                return Activator.CreateInstance(Type.GetType(className));
            }
            catch (Exception e) {
                LogManager.Instance.Write("Failed to create instance of {0} of type {0} from assembly {1}", className, className, "current assembly");
                LogManager.Instance.Write(e.Message);
            }
            return null;
        }
        public static object CreateInstanceWithNoSearch(Type mType) {
            try { 
                return Activator.CreateInstance(mType);
            }
            catch (Exception e) {
                LogManager.Instance.Write("Failed to create instance of {0} of type {0} from assembly {1}", mType.Name, mType.Name, "current assembly");
                LogManager.Instance.Write(e.Message);
            }
            return null;
        }

        /// <summary>
        /// 根据初始化 创建对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T CreateInstanceWithNoSearch<T>() where T : class {
            try {
                return (T)Activator.CreateInstance(_type);
            }
            catch (Exception e) {
                LogManager.Instance.Write("Failed to create instance of {0} of type {0} from assembly {1}", typeof(T).Name, _type, "current assembly");
                LogManager.Instance.Write(e.Message);
            }
            return null;
        }

    }

    internal class DynamicLoader {
        #region Fields and Properties

        private string _assemblyFilename;
        private Assembly _assembly;

        #endregion Fields and Properties

        #region Construction and Destruction

        /// <summary>
        /// Creates a loader instance for the current executing assembly
        /// </summary>
        public DynamicLoader() { }
#if UNITY_STANDALONE
		/// <summary>
		/// Creates a loader instance for the specified assembly
		/// </summary>
		public DynamicLoader( string assemblyFilename )
			: this()
		{
			_assemblyFilename = assemblyFilename;
		}
#endif
        #endregion Construction and Destruction

        #region Methods

        public Assembly GetAssembly() {
            if (_assembly == null) {
                lock (this) {
                    if (String.IsNullOrEmpty(_assemblyFilename)) {
                        _assembly = Assembly.GetExecutingAssembly();
                    }
                    else {
#if UNITY_STANDALONE
						_assembly = Assembly.LoadFrom( _assemblyFilename );
#else
                        throw new Exception("Not support Assembly.LoadFrom( _assemblyFilename )");
#endif
                    }
                }
            }
            return _assembly;
        }

        public IList<ObjectCreator> Find(Type baseType) {
            List<ObjectCreator> types = new List<ObjectCreator>();
            Assembly assembly;
            Type[] assemblyTypes = null;

            try {
                assembly = GetAssembly();
                assemblyTypes = assembly.GetTypes();

                foreach (Type type in assemblyTypes) {
#if UNITY_STANDALONE
                    //!(XBOX || XBOX360 || SILVERLIGHT)
					if( ( baseType.IsInterface && type.GetInterface( baseType.FullName ) != null ) ||
					    ( !baseType.IsInterface && type.BaseType == baseType ) )
					{
						types.Add( new ObjectCreator( assembly, type ) );
					}
#else
                    for (int i = 0; i < type.GetInterfaces().GetLength(0); i++) {
                        if (type.GetInterfaces()[i] == baseType) {
                            types.Add(new ObjectCreator(assembly, type));
                            break;
                        }
                    }
#endif
                }
            }

#if UNITY_STANDALONE
                //!(XBOX || XBOX360 || SILVERLIGHT)
			catch( ReflectionTypeLoadException ex )
			{
				LogManager.Instance.Write( LogManager.BuildExceptionString( ex ) );
				LogManager.Instance.Write( "Loader Exceptions:" );
				foreach( Exception lex in ex.LoaderExceptions )
				{
					LogManager.Instance.Write( LogManager.BuildExceptionString( lex ) );
				}
			}
			catch( BadImageFormatException ex )
			{
				LogManager.Instance.Write( LogMessageLevel.Trivial, true, ex.Message );
			}

#else
            catch (Exception ex) {
                LogManager.Instance.Write(LogManager.BuildExceptionString(ex));
                LogManager.Instance.Write("Loader Exceptions:");
            }
#endif

            return types;
        }

        #endregion Methods
    }
}
