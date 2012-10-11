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
        public BuildHelper(Builder bulderComponent)
        {
            builder = bulderComponent;
            proc = null;
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
                    CreateNoWindow = true,
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
        private Process proc;
    }
}
