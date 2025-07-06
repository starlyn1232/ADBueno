using System;
using System.Diagnostics;
using System.Text;

namespace ADBueno.Utilities
{
    public static class CMDUtils
    {
        // Create Process object
        private static Process InitProcess(string exe,
            string args = "",
            bool redirectSTD = true,
            bool redirectERR = true,
            bool redirectINPUT = false)
        {
            Process newProcess = new Process();

            newProcess.StartInfo.FileName = exe;
            newProcess.StartInfo.Arguments = args;
            newProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            newProcess.StartInfo.CreateNoWindow = true;
            newProcess.StartInfo.UseShellExecute = false;

            newProcess.StartInfo.RedirectStandardOutput = redirectSTD;
            newProcess.StartInfo.RedirectStandardError = redirectERR;
            newProcess.StartInfo.RedirectStandardInput = redirectINPUT;

            return newProcess;
        }

        // Run process
        public static string CMDRun(
            string exe,
            string args = "",
            Process pro = null,
            bool waitToExit = true,
            int timer = 0,
            bool debug = false)
        {
            if (pro == null)
            {
                pro = InitProcess(exe, args);
            }

            var stdOutput = new StringBuilder();
            var errOutput = new StringBuilder();

            pro.OutputDataReceived += (sender, e) =>
            {
                if (e.Data != null)
                {
                    stdOutput.AppendLine(e.Data);
                    if (debug)
                        Console.WriteLine(e.Data);
                }
            };

            pro.ErrorDataReceived += (sender, e) =>
            {
                if (e.Data != null)
                {
                    errOutput.AppendLine(e.Data);
                    if (debug)
                        Console.WriteLine(e.Data);
                }
            };

            pro.Start();

            pro.BeginOutputReadLine();
            pro.BeginErrorReadLine();

            if (timer > 0)
            {
                if (!pro.WaitForExit(timer))
                {
                    try
                    {
                        pro.Kill();
                    }
                    catch (InvalidOperationException) { /* Process may have already exited */ }
                }
            }
            else if (waitToExit)
            {
                pro.WaitForExit();
            }

            // Ensure async streams are flushed
            pro.WaitForExit();

            var std = stdOutput.ToString();
            var err = errOutput.ToString();

            return std.Length > err.Length ? std : err;
        }
    }
}
