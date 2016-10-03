using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace EndPointControllerWrapper
{
    public static class InvokeEndPointController
    {
        //https://github.com/marcjoha/AudioSwitcher
        public static void SelectDevice(string id)
        {
            var process = new Process
            {
                StartInfo =
                {
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true,
                    FileName = @"C:\temp\EndPointController.exe",
                    StandardOutputEncoding = Encoding.UTF8,
                    Arguments = id
                }
            };
            process.Start();
            process.WaitForExit();
        }

        public static IEnumerable<Tuple<int, string, bool>> GetDevices()
        {
            var p = new Process
            {
                StartInfo =
                {
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true,
                    FileName = @"C:\temp\EndPointController.exe",
                    Arguments = "-f \"%d|%ws|%d|%d\""
                }
            };
            p.Start();
            p.WaitForExit();
            var stdout = p.StandardOutput.ReadToEnd().Trim();

            var devices = new List<Tuple<int, string, bool>>();

            foreach (var line in stdout.Split('\n'))
            {
                var elems = line.Trim().Split('|');
                var deviceInfo = new Tuple<int, string, bool>(int.Parse(elems[0]), elems[1], elems[3].Equals("1"));
                devices.Add(deviceInfo);
            }

            return devices;
        }
    }
}