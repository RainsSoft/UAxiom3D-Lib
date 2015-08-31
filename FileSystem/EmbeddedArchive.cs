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
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <id value="$Id$"/>
// </file>

#endregion SVN Version Information

#region Namespace Declarations

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
//using System.Windows;
using Axiom.Core;

#endregion Namespace Declarations

namespace Axiom.FileSystem {
    /// <summary>
    /// 嵌入式(程序集)资源
    /// </summary>
    public class EmbeddedArchive : FileSystemArchive {
        #region Fields and Properties

        private readonly Assembly assembly;
        private readonly List<string> resources;

        public override bool IsMonitorable {
            get {
                return false;
            }
        }

        #endregion Fields and Properties

        #region Utility Methods

        protected  bool DirectoryExists(string directory) {
            return (from res in this.resources
                    where res.StartsWith(directory)
                    select res).Any();
        }

        protected  void findFiles(string pattern, bool recursive, List<string> simpleList, FileInfoList detailList,
                                           string currentDir) {
            if (pattern == "") {
                pattern = "*";
            }
            if (currentDir == "") {
                currentDir = _basePath;
            }

            var files = getFilesRecursively(currentDir, pattern);

            foreach (var file in files) {
                if (simpleList != null) {
                    simpleList.Add(file);
                }

                if (detailList != null) {
                    detailList.Add(new FileInfo {
                        Archive = this,
                        Filename = file,
                        Basename = file.Substring(currentDir.Length),
                        Path = currentDir,
                        CompressedSize = 0,
                        UncompressedSize = 0,
                        ModifiedTime = DateTime.Now
                    });
                }
            }
        }

        protected  string[] getFiles(string dir, string pattern, bool recurse) {
            var files = !pattern.Contains("*") && Exists(dir + pattern)
                            ? new[]
			            	  {
			            	  	pattern
			            	  }
                            : from res in this.resources
                              where res.StartsWith(dir)
                              select res;

            if (pattern == "*") {
                return files.ToArray<string>();
            }

            pattern = pattern.Substring(pattern.LastIndexOf('*') + 1);

            return (from file in files
                    where file.EndsWith(pattern)
                    select file).ToArray<string>();
        }

        protected  string[] getFilesRecursively(string dir, string pattern) {
            return getFiles(dir, pattern, true);
        }

        #endregion Utility Methods

        #region Constructors and Destructors
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
        public EmbeddedArchive(string name, string archType)
            : base(name.Split('/')[0], archType) {
            var named = Name + ",";
            
            //this.assembly = (from a in AssemblyEx.Neighbors()
            //                 where a.FullName.StartsWith(named)
            //                 select a).First();
#if UNITY_STANDALONE
            this.assembly = Assembly.LoadFrom(name);
#else
            throw new Exception("Not support Assembly.LoadFrom(...)");
#endif
            Name = name.Replace('/', '.');
            this.resources = (from resource in this.assembly.GetManifestResourceNames()
                              //where resource.StartsWith(Name)
                              select resource).ToList();
            this.resources.Sort();
        }

        #endregion Constructors and Destructors

        #region Archive Implementation

        public override bool IsCaseSensitive {
            get {
                return true;
            }
        }

        public override void Load() {
            _basePath = Name + ".";
            IsReadOnly = true;
        }

        public override Stream Create(string filename, bool overwrite) {
            throw new AxiomException("Cannot create a file in a read-only archive.");
        }

        public override Stream Open(string filename, bool readOnly) {
            if (!readOnly) {
                throw new AxiomException("Cannot create a file in a read-only archive.");
            }
            return this.assembly.GetManifestResourceStream(this.resources[this.resources.BinarySearch(_basePath + filename)]);
        }

        public override bool Exists(string fileName) {
            return this.resources.BinarySearch(_basePath + fileName) >= 0;
        }

        #endregion Archive Implementation
    }

    /// <summary>
    /// Specialization of IArchiveFactory for Embedded files.
    /// </summary>
    public class EmbeddedArchiveFactory : ArchiveFactory {
        private const string _type = "Embedded";

        #region ArchiveFactory Implementation

        public override string Type {
            get {
                return _type;
            }
        }

        public override Archive CreateInstance(string name) {
            return new EmbeddedArchive(name, _type);
        }

        public override void DestroyInstance(ref Archive obj) {
            obj.Dispose();
        }

        #endregion ArchiveFactory Implementation
    };
}

