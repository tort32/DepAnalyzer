using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace DepAnalyzer
{
    internal static class RtfUtils
    {
        private static RichTextBox mRtb = new RichTextBox();

        public static string GetLogFormatted(string logText)
        {
            mRtb.Text = logText;
            foreach (string line in mRtb.Lines)
            {
                if (line.StartsWith("====="))
                {
                    int startindex = mRtb.Find(line);
                    mRtb.Select(startindex, line.Length);
                    mRtb.SelectionFont = new Font(mRtb.SelectionFont, FontStyle.Bold);
                    //rtb.SelectionColor = Color.Red;
                }
            }
            return mRtb.Rtf;
        }

        public static void AppendRtf(this RichTextBox rtb, string rtfText)
        {
            const string searchText = "\\par\r\n}";
            const string removeText = "\\par";
            rtb.Select(rtb.TextLength, 0);
            rtb.SelectedRtf = rtfText;
            int removeIndex = rtb.Rtf.LastIndexOf(searchText, StringComparison.InvariantCulture);
            if (removeIndex != -1)
                rtb.Rtf = rtb.Rtf.Remove(removeIndex, removeText.Length);
        }

        public static string MergeRtf(string rtf1, string rtf2)
        {
            mRtb.Rtf = rtf1;
            mRtb.AppendRtf(rtf2);
            return mRtb.Rtf;
        }
    }

    internal class Builder
    {
        public Builder(ListView orderListView, ComboBox logComboBox, RichTextBox logTextBox)
        {
            orderList = orderListView;
            logCombo = logComboBox;
            log = logTextBox;
            devEnvPath = null;
            solutionFile = null;
            solutionConfig = null;
            IsBuilding = false;
            buildProject.AppendLog("Press \"Start Build\" button to check building of projects");
        }

        private bool TryGetDevEnvPath(string VSVerCode)
        {
            if (!String.IsNullOrEmpty(devEnvPath))
                return true;

            string devToolPath = Environment.GetEnvironmentVariable(VSVerCode + "COMNTOOLS");

            if(String.IsNullOrEmpty(devToolPath))
                return false;

            devEnvPath = Path.Combine(devToolPath, "..\\IDE\\devenv.com");
            return true;
        }

        public void GenerateOrderListForRoots(string[] rootNames)
        {
            if (projList != null)
            {
                foreach (var proj in projList)
                {
                    proj.StatusChanged -= OnProjectUpdate;
                }
            }
            projList = new List<Project>();

            List<Project> graphProjs = new List<Project>();
            foreach (string rootName in rootNames)
            {
                Project proj = SolutionParser.ProjTable[rootName];

                foreach (Project depProj in proj.ProjectTree)
                {
                    graphProjs.AddUnique(depProj);
                }
            }

            while (graphProjs.Count != 0)
            {
                var downLevelList = graphProjs.FindAll(p => !p.DepProjects.Any(graphProjs.Has)).ToList();
                projList.AddRange(downLevelList);
                graphProjs.RemoveAll(downLevelList.Has);
            }

            orderList.Items.Clear();
            foreach (var proj in projList)
            {
                var item = new ListViewItem(proj.Name);
                item.StateImageIndex = (int)proj.Status;
                orderList.Items.Add(item);
                proj.StatusChanged += OnProjectUpdate;
            }

            UpdateLogCombo();
        }

        public void StartBuild(string VSVersion, string solutionSource, string solutionConfiguration)
        {
            if (!TryGetDevEnvPath(SolutionParser.VSVerCode(VSVersion)))
            {
                string msg = String.Format("Visual Studio {0} installation required for building this solution.", VSVersion);
                MessageBox.Show(msg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (String.IsNullOrEmpty(solutionSource))
                return;

            solutionFile = new FileInfo(solutionSource);
            if (!solutionFile.Exists)
                return;

            solutionConfig = solutionConfiguration;

            workDir = solutionFile.DirectoryName;

            // build project list
            projBuilds = new List<Project>();
            projBuilds.AddRange(projList);

            Worker = new Thread(new ThreadStart(BuildAll));
            Worker.Start();
        }

        public void StopBuild()
        {
            if (Worker != null)
            {
                Worker.Abort();
                Worker = null;

                if (buildHelper != null)
                {
                    buildHelper.AbortProcess();
                    buildHelper = null;
                }

                foreach (var proj in projBuilds)
                {
                    if (proj.Status == Project.BuildStatus.Progress)
                    {
                        proj.Status = Project.BuildStatus.Wait;
                    }
                }
                OnBuildComplete(null);
            }
        }

        private void BuildAll()
        {
            IsBuilding = true;
            buildHelper = new BuildHelper(this);

            buildProject.ClearLog();
            AppendLog("Build started");

            int retCode = 0;
            bool successed = true;
            foreach (var proj in projBuilds)
            {
                if (proj.IsExternal)
                {
                    AppendLog(String.Format("Project \"{0}\" skipped as external.", proj.Name));
                    continue;
                }

                if (proj.Status == Project.BuildStatus.Success)
                {
                    AppendLog(String.Format("Project \"{0}\" skipped as successful.", proj.Name));
                    continue;
                }

                if(proj.ProjectTree.Any(p => p.Status == Project.BuildStatus.Failed))
                {
                    AppendLog(String.Format("Project \"{0}\" skipped as unavailable to build due to fails in dependency projects.", proj.Name));
                    continue;
                }

                proj.Status = Project.BuildStatus.Progress;
                proj.ClearLog();

                retCode = buildHelper.RunProcess(proj, devEnvPath, String.Format("\"{0}\" /clean \"{1}\"", solutionFile.FullName, solutionConfig));
                if (retCode != 0)
                {
                    proj.Status = Project.BuildStatus.Failed;
                    AppendLog(String.Format("Solution clear failed."));
                    successed = false;
                    continue;
                }

                retCode = buildHelper.RunProcess(proj, devEnvPath, String.Format("\"{0}\" /build \"{1}\" /project \"{2}\"", solutionFile.FullName, solutionConfig, proj.Name));
                if (retCode != 0)
                {
                    proj.Status = Project.BuildStatus.Failed;
                    AppendLog(String.Format("Project \"{0}\" build failed.", proj.Name));
                    successed = false;
                    continue;
                }

                proj.Status = Project.BuildStatus.Success;
                AppendLog(String.Format("Project \"{0}\" build successful.", proj.Name));
            }

            buildHelper = null;
            OnBuildComplete(successed);
        }

        delegate void UpdateUICallback(string data);

        private void OnProjectUpdate(object sender, EventArgs e)
        {
            Project proj = (Project)sender;
            UpdateBuildItem(proj.Name);
        }

        private void UpdateBuildItem(string projName)
        {
            if (orderList.InvokeRequired)
            {
                orderList.Invoke(new MethodInvoker(() => UpdateBuildItem(projName)));
                return;
            }

            for(int i =0;i<orderList.Items.Count;i++)
            {
                if (orderList.Items[i].Text == projName)
                {
                    Project proj = SolutionParser.ProjTable[projName];
                    orderList.Items[i].StateImageIndex = (int)proj.Status;
                    break;
                }
            }
        }

        public void UpdateLogCombo()
        {
            if (logCombo.InvokeRequired)
            {
                logCombo.Invoke(new MethodInvoker(UpdateLogCombo));
                return;
            }

            logCombo.Items.Clear();
            var logProjects = projList.ToList();
            logProjects.Insert(0, buildProject);
            foreach(Project proj in logProjects)
            {
                logCombo.Items.Add(proj);
            }
            logCombo.SelectedIndex = 0;
        }

        private void UpdateLog(object sender, EventArgs e)
        {
            if (log.IsDisposed)
                return;

            if (log.InvokeRequired)
            {
                log.Invoke(new MethodInvoker(() => UpdateLog(sender, e)));
                return;
            }

            // Update current log
            var proj = (Project)sender;
            var selectedProj = (Project)logCombo.SelectedItem;
            if(proj == selectedProj)
            {
                string appendText = String.Empty;
                string fullText = proj.GetLogFormatted(out appendText);
                if(e == null) // Full text update
                {
                    log.Rtf = fullText;

                    log.SelectionStart = 0;
                }
                else // Append text update
                {
                    log.AppendRtf(appendText);
                    //log.Rtf = fullText;

                    //log.Select(log.TextLength == 0 ? 0 : log.TextLength - 1, 0);
                    //log.SelectedRtf = appendText;

                    log.SelectionStart = log.TextLength;
                }
                log.ScrollToCaret();
            }
        }

        public void PreSelectLogCombo()
        {
            var proj = (Project)logCombo.SelectedItem;
            if (proj != null)
            {
                proj.LogChanged -= UpdateLog;
            }
        }
        public void SelectLogCombo()
        {
            var proj = (Project)logCombo.SelectedItem;
            if (proj != null)
            {
                proj.LogChanged += UpdateLog;
                UpdateLog(proj, null);
            }
        }

        public void AppendLog(string text)
        {
            buildProject.AppendLog(text);
            //Program.form.UpdateBuildLog(text);
        }

        // Calls at build finish or abortion
        private void OnBuildComplete(bool? IsCompleted)
        {
            if (!IsCompleted.HasValue)
                AppendLog("Build aborted");
            else if(IsCompleted.Value)
                AppendLog("Build finished");
            else
                AppendLog("Build failed");

            var d = new UpdateUICallback(Program.form.UpdateBuildButton);
            Program.form.Invoke(d, !IsCompleted.HasValue ? "Abort" : IsCompleted.Value ? "Success" : "Fail");

            IsBuilding = false;
        }

        // devenv SolutionName {/build|/clean|/rebuild|/deploy} SolnConfigName [/project ProjName]
        // devenv Client.sln /clean [ /project "ProjName"]
        // devenv Client.sln /build "DebugRelease" /project "ProjName"

        private List<Project> projList; // listed projects
        private List<Project> projBuilds; // building projects
        private ListView orderList;
        private ComboBox logCombo;
        private RichTextBox log;
        private FileInfo solutionFile;
        private string solutionConfig;
        private string workDir;
        private string devEnvPath;
        private Thread Worker;
        private BuildHelper buildHelper;

        private Project buildProject = new Project("<Build Log>", String.Empty, String.Empty, true);

        public bool IsBuilding { get; private set; }
    }
}
