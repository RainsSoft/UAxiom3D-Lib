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
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <id value="$Id: FileSystemArchive.cs 1537 2009-03-30 19:25:01Z borrillis $"/>
// </file>

#endregion SVN Version Information

#region Namespace Declarations

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Axiom.Core;

#endregion Namespace Declarations

namespace Axiom.FileSystem
{
	/// <summary>
	/// Specialization of the Archive class to allow reading of files from filesystem folders / directories.
	/// </summary>
	/// <ogre name="FileSystemArchive">
	///     <file name="OgreFileSystem.h"   revision="1.6.2.1" lastUpdated="5/18/2006" lastUpdatedBy="Borrillis" />
	///     <file name="OgreFileSystem.cpp" revision="1.8" lastUpdated="5/18/2006" lastUpdatedBy="Borrillis" />
	/// </ogre>
	public class FileSystemArchive : Archive
	{
		#region Fields and Properties

		/// <summary>Base path; actually the same as Name, but for clarity </summary>
		protected string _basePath;
        protected virtual string CurrentDirectory { get; set; }
		/// <summary>Directory stack of previous directories </summary>
        private readonly Stack<string> _directoryStack = new Stack<string>();

		/// <summary>
		/// Is this archive capable of being monitored for additions, changes and deletions
		/// </summary>
		public override bool IsMonitorable { get { return true; } }

		#endregion Fields and Properties

		#region Utility Methods

		protected delegate void Action();
        protected virtual bool DirectoryExists(string directory) {
            return Directory.Exists(directory);
        }
		protected void SafeDirectoryChange( string directory, Action action )
		{
            if (DirectoryExists(directory))
			{
				// Check we can change to it
				pushDirectory( directory );

				try
				{
					action();
				}
				catch( Exception ex )
				{
					LogManager.Instance.Write( LogManager.BuildExceptionString( ex ) );
				}
				finally
				{
					// return to previous
					popDirectory();
				}
			}
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
        //UNITY_WINRT_8_1 Equivalent to UNITY_WP_8_1 | UNITY_WSA_8_1. It¡¯s also defined when compiling against Universal SDK 8.1. 
        //UNITY_WEBGL Platform define for WebGL. 
		/// <overloads>
		/// <summary>
		/// Utility method to retrieve all files in a directory matching pattern.
		/// </summary>
		/// <param name="pattern">File pattern</param>
		/// <param name="recursive">Whether to cascade down directories</param>
		/// <param name="simpleList">Populated if retrieving a simple list</param>
		/// <param name="detailList">Populated if retrieving a detailed list</param>
		/// </overloads>
		protected void findFiles( string pattern, bool recursive, List<string> simpleList, FileInfoList detailList )
		{
			findFiles( pattern, recursive, simpleList, detailList, "" );
		}

		/// <param name="currentDir">The current directory relative to the base of the archive, for file naming</param>
		protected void findFiles( string pattern, bool recursive, List<string> simpleList, FileInfoList detailList, string currentDir )
		{
			if( pattern == "" )
			{
				pattern = "*";
			}
			if( currentDir == "" )
			{
				currentDir = _basePath;
			}

            string[] files = getFiles(currentDir, pattern, recursive);

//#if UNITY_STANDALONE
//            //!( XBOX || XBOX360 )
//            files = Directory.GetFiles( currentDir, pattern, recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly );
//#else
//            files = recursive ? this.getFilesRecursively(currentDir, pattern) : Directory.GetFiles(currentDir, pattern);
//#endif
			foreach( string file in files )
			{
				System.IO.FileInfo fi = new System.IO.FileInfo( file );
				if( simpleList != null )
				{
					simpleList.Add( fi.Name );
				}
				if( detailList != null )
				{
					FileInfo fileInfo;
					fileInfo.Archive = this;
					fileInfo.Filename = fi.FullName;
					fileInfo.Basename = fi.FullName.Substring( Path.GetFullPath( currentDir ).Length );
					fileInfo.Path = currentDir;
					fileInfo.CompressedSize = fi.Length;
					fileInfo.UncompressedSize = fi.Length;
					fileInfo.ModifiedTime = fi.LastWriteTime;
					detailList.Add( fileInfo );
				}
			}
        }
        /// <summary>
        /// Returns the names of all files in the specified directory that match the specified search pattern, performing a recursive search
        /// </summary>
        /// <param name="dir">The directory to search.</param>
        /// <param name="pattern">The search string to match against the names of files in path.</param>
        protected virtual string[] getFiles(string dir, string pattern, bool recurse) {
            string[] files;
#if UNITY_STANDALONE
            files = Directory.GetFiles(dir, pattern, recurse ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
            
#elif SILVERLIGHT
			files = Directory.EnumerateFiles( dir, pattern, recurse ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly ).ToArray<string>();
#else
            //( XBOX || XBOX360 || ANDROID || WINDOWS_PHONE )
            if (!recurse) {
                files = Directory.GetFiles(dir, pattern);
            }
            else {
                files = getFilesRecursively(dir, pattern);
            }
#endif
            return files;
        }
//#if ( XBOX || XBOX360 || UNITY_XBOX360 || UNITY_XBOXONE)
	/// <summary>
	/// Returns the names of all files in the specified directory that match the specified search pattern, performing a recursive search
	/// </summary>
	/// <param name="dir">The directory to search.</param>
	/// <param name="pattern">The search string to match against the names of files in path.</param>
        protected virtual string[] getFilesRecursively(string dir, string pattern)
		{
			List<string> searchResults = new List<string>();
			string[] folders = Directory.GetDirectories( dir );
			string[] files = Directory.GetFiles( dir );

			foreach ( string folder in folders )
			{
				searchResults.AddRange( this.getFilesRecursively( dir + Path.GetFileName( folder ) + "\\", pattern ) );
			}

			foreach ( string file in files )
			{
				string ext = Path.GetExtension( file );

				if ( pattern == "*" || pattern.Contains( ext ) )
					searchResults.Add( file );
			}

			return searchResults.ToArray();
		}
//#endif

        /// <summary>Utility method to change the current directory </summary>
		protected void changeDirectory( string dir )
		{
            //Directory.SetCurrentDirectory( dir );
#if UNITY_STANDALONE
            Directory.SetCurrentDirectory(dir);
#else
            CurrentDirectory = dir;
#endif
		}

		/// <summary>Utility method to change directory and push the current directory onto a stack </summary>
		private void pushDirectory( string dir )
		{
            // get current directory and push it onto the stack
#if UNITY_STANDALONE
            //!( XBOX || XBOX360 )
			string cwd = Directory.GetCurrentDirectory();
			_directoryStack.Push( cwd );
#else
            _directoryStack.Push( CurrentDirectory );
#endif
            changeDirectory( dir );
		}

		/// <summary>Utility method to pop a previous directory off the stack and change to it </summary>
		private void popDirectory()
		{
			if( _directoryStack.Count == 0 ) {
#if  UNITY_STANDALONE
                //!( XBOX || XBOX360 )
				throw new AxiomException( "No directories left in the stack." );
#else
                return;
#endif
			}
			string cwd = _directoryStack.Pop();
			changeDirectory( cwd );
		}

		#endregion Utility Methods

		#region Constructors and Destructors

		public FileSystemArchive( string name, string archType )
			: base( name, archType ) {}

		~FileSystemArchive()
		{
			Unload();
		}

		#endregion Constructors and Destructors

		#region Archive Implementation

		public override bool IsCaseSensitive { get { return !PlatformManager.IsWindowsOS; } }

		public override void Load()
		{
			_basePath = Path.GetFullPath( Name ) + Path.DirectorySeparatorChar;
			IsReadOnly = false;

			SafeDirectoryChange( _basePath, () =>
			                                {
			                                	try
			                                	{
#if UNITY_STANDALONE
                                                    //!( XBOX || XBOX360 )
			                                		File.Create( _basePath + @"__testWrite.Axiom", 1, FileOptions.DeleteOnClose );
#else
													File.Create(_basePath + @"__testWrite.Axiom", 1 );
                                                    File.Delete(_basePath + @"__testWrite.Axiom");
#endif
			                                	}
			                                	catch( Exception ex )
			                                	{
			                                		IsReadOnly = true;
			                                	}
			                                } );
		}

		public override Stream Create( string filename, bool overwrite )
		{
			if( IsReadOnly )
			{
				throw new AxiomException( "Cannot create a file in a read-only archive." );
			}

			Stream stream = null;
			string fullPath = _basePath + Path.DirectorySeparatorChar + filename;
			bool exists = File.Exists( fullPath );
			if( !exists || overwrite )
			{
				try
				{
#if UNITY_STANDALONE
                    //!( XBOX || XBOX360 )
					stream = File.Create( fullPath, 1, FileOptions.RandomAccess );
#else
					stream = File.Create( fullPath, 1 );
#endif
				}
				catch( Exception ex )
				{
					throw new AxiomException( "Failed to open file : " + filename, ex );
				}
			}
			else
			{
				stream = Open( fullPath, false );
			}

			return stream;
		}

		public override void Unload()
		{
			// Nothing to do here.
		}

		public override System.IO.Stream Open( string filename, bool readOnly )
		{
			Stream strm = null;

			SafeDirectoryChange( _basePath, () =>
			                                {
			                                	if( File.Exists( _basePath + filename ) )
			                                	{
			                                		System.IO.FileInfo fi = new System.IO.FileInfo( _basePath + filename );
			                                		strm = (Stream)fi.Open( FileMode.Open, readOnly ? FileAccess.Read : FileAccess.ReadWrite );
			                                	}
			                                } );

			return strm;
		}

		public override List<string> List( bool recursive )
		{
			return Find( "*", recursive );
		}

		public override FileInfoList ListFileInfo( bool recursive )
		{
			return FindFileInfo( "*", recursive );
		}

		public override List<string> Find( string pattern, bool recursive )
		{
			List<string> ret = new List<string>();

			SafeDirectoryChange( _basePath, () => findFiles( pattern, recursive, ret, null ) );

			return ret;
		}

		public override FileInfoList FindFileInfo( string pattern, bool recursive )
		{
			FileInfoList ret = new FileInfoList();

			SafeDirectoryChange( _basePath, () => findFiles( pattern, recursive, null, ret ) );

			return ret;
		}

		public override bool Exists( string fileName )
		{
			return File.Exists( _basePath + fileName );
		}

		#endregion Archive Implementation
	}

	/// <summary>
	/// Specialization of IArchiveFactory for FileSystem files.
	/// </summary>
	/// <ogre name="FileSystemArchiveFactory">
	///     <file name="OgreFileSystem.h"   revision="1.6.2.1" lastUpdated="5/18/2006" lastUpdatedBy="Borrillis" />
	///     <file name="OgreFileSystem.cpp" revision="1.8" lastUpdated="5/18/2006" lastUpdatedBy="Borrillis" />
	/// </ogre>
	public class FileSystemArchiveFactory : ArchiveFactory
	{
		private const string _type = "Folder";

		#region ArchiveFactory Implementation

		public override string Type { get { return _type; } }

		public override Archive CreateInstance( string name )
		{
			return new FileSystemArchive( name, _type );
		}

		public override void DestroyInstance( ref Archive obj )
		{
			obj.Dispose();
		}

		#endregion ArchiveFactory Implementation
	};
}
