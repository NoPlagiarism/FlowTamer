using Flow.Launcher.Plugin.SharedCommands;
using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Flow.Launcher.Plugin.FlowTamer
{
    internal class BTApi
    {
        static public readonly string BTExecutableFile = "bt.exe";
        public string BTPath;

        public BTApi(string path) {
            BTPath = path;
        }

        static public string FindBTUsingCustomProtocol()
        {
            // Find bt.exe with x-bt
            var regXBT = Registry.ClassesRoot.OpenSubKey(@"x-bt");
            if (regXBT is null) return null;
            string cmdValue;
            try { cmdValue = (string)regXBT.OpenSubKey(@"shell\open\command").GetValue(""); }
            catch (NullReferenceException) { return null; }
            return cmdValue.Substring(1, cmdValue.IndexOf("\"", 1) - 1);
        }

        static public string FindBTUsingHTMProtocol()
        {
            // Find bt.exe with BrowserTamerHTM
            var regBT = Registry.ClassesRoot.OpenSubKey(@"BrowserTamerHTM");
            if (regBT is null) return null;
            string cmdValue;
            try { cmdValue = (string)regBT.OpenSubKey(@"shell\open\command").GetValue(""); }
            catch (NullReferenceException) { return null; }
            return cmdValue.Substring(1, cmdValue.IndexOf("\"", 1) - 1);
        }

        static public string FindBTUsingStartMenuInternet()
        {
            // Find bt.exe with StartMenuInternet, where browsers are registered
            // src: https://github.com/MintPlayer/MintPlayer.BrowserDialog
            var HKLM = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\WOW6432Node\Clients\StartMenuInternet") ?? Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Clients\StartMenuInternet");
            var HKCU = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\WOW6432Node\Clients\StartMenuInternet") ?? Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Clients\StartMenuInternet");

            foreach (var SMIKey in new[] { HKLM, HKCU }.Where(key => key != null).Cast<RegistryKey>())
            {
                foreach (string browserKeyName in SMIKey.GetSubKeyNames())
                {
                    if (browserKeyName == "Browser Tamer")
                    {
                        string cmdValue;
                        try { cmdValue = (string)SMIKey.OpenSubKey(@"Browser Tamer\shell\open\command").GetValue("").ToString(); return cmdValue.Substring(1, cmdValue.Length - 2); }
                        catch (NullReferenceException) { return null; }

                    }
                }
            }
            return null;
        }

        static public bool CheckBTPath(string path)
        {
            try { return Path.GetFileName(path) == BTApi.BTExecutableFile && Path.Exists(path); }
            catch (Exception) { return false; }
        }

        static public string FindBT()
        {
            string path;
            foreach (Func<string> func in new Func<string>[] { FindBTUsingCustomProtocol, FindBTUsingHTMProtocol, FindBTUsingStartMenuInternet })
            {
                path = func();
                if (BTApi.CheckBTPath(path)) return path;
            }
            return null;
        }

        public void Open(string url, bool withPicker = true)
        {
            //ProcessStartInfo psi = new ProcessStartInfo(this.BTPath, (withPicker ? "pick " : "") + url);
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = this.BTPath,
                Arguments = ((withPicker ? " pick " : "") + url)
            };

            ShellCommand.Execute(psi);
        }
    }
}
