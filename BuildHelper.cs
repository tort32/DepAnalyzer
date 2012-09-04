using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading;

namespace DepAnalyzer
{
    class BuildHelper
    {
        [DllImport("user32.dll")]
        internal static extern bool ShowWindow(IntPtr hWnd, WindowShowStyle nCmdShow);

        internal enum WindowShowStyle : uint
        {
            Minimize = 6,
            Restore = 9,
        }

        public BuildHelper(Builder bulderComponent)
        {
            builder = bulderComponent;
            batchLines = new List<string>();
            proc = null;
        }

        public void AddStep(string line)
        {
            batchLines.Add(line);
        }

        public void Run()
        {
            var batchFileInfo2 = new FileInfo(Path.Combine(Environment.CurrentDirectory, "tmp2.bat"));
            FileStream fs2 = File.Open(batchFileInfo2.FullName, FileMode.Create, FileAccess.Write);
            TextWriter tw2 = new StreamWriter(fs2);
            //tw2.WriteLine("call \"%VS100COMNTOOLS%vsvars32\"");
            tw2.WriteLine("ECHO \"%VS100COMNTOOLS%vsvars32\"");
            tw2.WriteLine("exit 0");
            tw2.Flush();
            fs2.Close();

            var batchFileInfo = new FileInfo(Path.Combine(Environment.CurrentDirectory, "tmp.bat"));
            FileStream fs = File.Open(batchFileInfo.FullName, FileMode.Create, FileAccess.Write);
            TextWriter tw = new StreamWriter(fs);
            tw.WriteLine("\"" + batchFileInfo2.FullName + "\"");
            foreach (var batchLine in batchLines)
            {
                tw.WriteLine(batchLine);
            }
            tw.Flush();
            fs.Close();

            RunProcess(null, batchFileInfo.FullName);

            //File.Delete(batchFileInfo.FullName);
        }
        public int RunProcess(Project proj, string app)
        {
            return RunProcess(proj, app, null, null, true);
        }
        public int RunProcess(Project proj, string app, string args)
        {
            return RunProcess(proj, app, args, null, true);
        }
        public int RunProcess(Project proj, string app, string args, string workDir)
        {
            return RunProcess(proj, app, args, workDir, true);
        }
        public int RunProcess(Project proj, string app, string args, string workDir, bool redirectOutput)
        {
            proc = new Process()
            {
                EnableRaisingEvents = true,
                StartInfo = new ProcessStartInfo(app, args)
                {
                    WorkingDirectory = workDir,
                    RedirectStandardOutput = redirectOutput,
                    RedirectStandardError = redirectOutput,
                    UseShellExecute = !redirectOutput,
                    WindowStyle = ProcessWindowStyle.Hidden,
                }
            };
            string debugString = (workDir ?? String.Empty) + ">" + app + " " + (args ?? String.Empty);
            builder.AppendLog(debugString);
            proc.EnableRaisingEvents = true;
            if (redirectOutput)
            {
                proc.OutputDataReceived += delegate(object sender, DataReceivedEventArgs e)
                {
                    if (proj != null)
                        proj.AppendLog(e.Data);
                    else
                        builder.AppendLog(e.Data);
                };
            }
            proc.Start();

            Thread.Sleep(100);
            ShowWindow(proc.MainWindowHandle, WindowShowStyle.Minimize);

            if (redirectOutput)
                proc.BeginOutputReadLine();

            proc.WaitForExit();
            int retCode = proc.ExitCode;
            if (redirectOutput && retCode != 0)
            {
                string error = proc.StandardError.ReadToEnd();
                if (!String.IsNullOrEmpty(error))
                    builder.AppendLog("Error: " + error);
            }
            proc.Dispose();
            proc = null;
            return retCode;
        }

        public void AbortProcess()
        {
            if (proc != null)
            {
                if (!proc.HasExited)
                    proc.Kill();
                proc.Dispose();
                proc = null;
            }
        }

        private Builder builder;
        private List<string> batchLines;
        private Process proc;
    }
}
