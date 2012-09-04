using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace DepAnalyzer
{
    internal class Builder
    {
        public Builder(ListView orderListView, ComboBox logComboBox, TextBox logTextBox)
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
                var item = new ListViewItem(proj.mName);
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

            /*retCode = buildHelper.RunProcess(Environment.GetEnvironmentVariable("VS100COMNTOOLS") + "vsvars32.bat");
            if (retCode != 0)
                Log(String.Format("Error code: {0}", retCode));*/
            /*foreach (var proj in projBuilds)
            {
                proj.Status = Project.BuildStatus.Wait;
                UpdateItem(proj);
            }*/
            bool successed = true;
            foreach (var proj in projBuilds)
            {
                if (proj.Status == Project.BuildStatus.Success)
                {
                    AppendLog(String.Format("Project \"{0}\" skipped as successful.", proj.mName));
                    continue;
                }

                if(proj.ProjectTree.Any(p => p.Status == Project.BuildStatus.Failed))
                {
                    AppendLog(String.Format("Project \"{0}\" skipped as unavailable to build due to fails in dependency projects.", proj.mName));
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

                retCode = buildHelper.RunProcess(proj, devEnvPath, String.Format("\"{0}\" /build \"{1}\" /project \"{2}\"", solutionFile.FullName, solutionConfig, proj.mName));
                if (retCode != 0)
                {
                    proj.Status = Project.BuildStatus.Failed;
                    AppendLog(String.Format("Project \"{0}\" build failed.", proj.mName));
                    successed = false;
                    continue;
                }

                proj.Status = Project.BuildStatus.Success;
                AppendLog(String.Format("Project \"{0}\" build successful.", proj.mName));
            }

            /*
            buildHelper.AddStep("@echo off");
            string devEnvPath = Environment.GetEnvironmentVariable("VS100COMNTOOLS") + "..\\IDE\\devenv.exe";
            //buildHelper.AddStep("call \"%VS100COMNTOOLS%vsvars32\"");
            buildHelper.AddStep("@echo on");
            buildHelper.AddStep("echo 1");
            buildHelper.AddStep("ping localhost");
            //buildHelper.AddStep("ping 8.8.8.8");
            */

            //buildHelper.Run();

            buildHelper = null;

            OnBuildComplete(successed);
        }

        delegate void UpdateUICallback(string data);

        private void OnProjectUpdate(object sender, EventArgs e)
        {
            Project proj = (Project)sender;
            UpdateBuildItem(proj.mName);
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
            if (log.InvokeRequired)
            {
                log.Invoke(new MethodInvoker(() => UpdateLog(sender, e)));
                return;
            }

            var proj = (Project)sender;
            var selectedProj = (Project)logCombo.SelectedItem;
            if(proj == selectedProj)
            {
                log.Text = proj.Log;
                log.SelectionStart = log.Text.Length;
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
        private TextBox log;
        private FileInfo solutionFile;
        private string solutionConfig;
        private string workDir;
        private string devEnvPath;
        private Thread Worker;
        private BuildHelper buildHelper;

        private Project buildProject = new Project("<Build Log>", String.Empty);

        public bool IsBuilding { get; private set; }
    }
}
