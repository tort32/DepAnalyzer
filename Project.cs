using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace DepAnalyzer
{
    internal static class ProjectExtentions
    {
        public static bool AddUnique(this List<Project> projects, Project proj)
        {
            if (!projects.Has(proj))
            {
                projects.Add(proj);
                return true;
            }
            return false;
        }
        public static bool Has(this List<Project> projects, Project proj)
        {
            return projects.Exists(p => p.Name == proj.Name);
        }
    }
    internal class Project
    {
        public enum BuildStatus
        {
            Wait = 0,
            Progress = 1,
            Success = 2,
            Failed = 3
        }

        public Project(string name, string path, string guid, bool isExternal)
        {
            Name = name;
            GUID = guid;
            File = String.IsNullOrEmpty(path) ? null : new FileInfo(path);
            IsExternal = isExternal;
            Status = BuildStatus.Wait;
            Log = String.Empty;

            mPreviousLog = String.Empty;
            mPreviousLogFormatted = String.Empty;

            ClearDepGraph();
        }

        public string Name { get; set; }
        public FileInfo File { get; private set; }
        public string GUID { get; private set; }
        public bool IsExternal { get; private set; }

        private List<string> DepGUIDs { get; set; }
        private string mPreviousLog;
        private string mPreviousLogFormatted;

        public Project Parent
        {
            get; private set;
        }
        public List<Project> DepProjects
        {
            get; private set;
        }

        public bool HasChildren
        {
            get { return DepProjects.Count != 0; }
        }

        public bool HasParent
        {
            get { return Parent != null; }
        }

        public List<Project> ProjectTree
        {
            // Return all dependency projects from tree including current root node (this)
            get
            {
                List<Project> projs = new List<Project>();
                projs.Add(this);
                foreach (Project depProj in DepProjects)
                {
                    foreach (Project depDepProj in depProj.ProjectTree)
                    {
                        projs.AddUnique(depDepProj);
                    }
                }
                return projs;
            }
        }

        public List<Project> Parents
        {
            // Return all dependency parents excluding current node (this)
            get
            {
                List<Project> projs = new List<Project>();
                if (HasParent)
                {
                    projs.Add(Parent);
                    projs.AddRange(Parent.Parents);
                }
                return projs;
            }
        }

        public event EventHandler StatusChanged;

        private BuildStatus mStatus;
        public BuildStatus Status
        {
            get
            {
                return mStatus;
            }
            set
            {
                if (mStatus != value)
                {
                    mStatus = value;
                    if (StatusChanged != null)
                        StatusChanged(this, EventArgs.Empty);
                }
            }
        }

        public bool IsBuilt
        {
            get { return Status == BuildStatus.Failed || Status == BuildStatus.Success; }
        }

        public event EventHandler LogChanged;

        public string Log { get; private set; }

        public string GetLogFormatted(out string appending)
        {
            // If not found previous log in current log then reset cache
            if (!Log.StartsWith(mPreviousLog))
            {
                mPreviousLog = String.Empty;
                mPreviousLogFormatted = String.Empty;
            }

            // Find appending text (diff)
            string logAdd = Log.Substring(mPreviousLog.Length, Log.Length - mPreviousLog.Length);

            // Format appending text
            string logAddFormatted = RtfUtils.GetLogFormatted(logAdd);

            // Append formatted appending text to previous formatted text and cache results
            mPreviousLog = Log;
            mPreviousLogFormatted = RtfUtils.MergeRtf(mPreviousLogFormatted, logAddFormatted);
            appending = logAddFormatted;

            return mPreviousLogFormatted;
        }

        public bool HasLog
        {
            get { return !String.IsNullOrEmpty(Log); }
        }

        public override string ToString()
        {
            return Name;
        }

        public void ClearDepGraph()
        {
            Parent = null;
            DepProjects = null;
            DepGUIDs = new List<string>();
        }

        public void AddDepGUID(string depGUID)
        {
            if (!DepGUIDs.Contains(depGUID))
            {
                DepGUIDs.Add(depGUID);
            }
        }

        public void ResolveDeps(Dictionary<string, Project> guidTable)
        {
            DepProjects = new List<Project>();
            foreach (string guid in DepGUIDs)
            {
                Project depProj;
                if (guidTable.TryGetValue(guid, out depProj))
                {
                    depProj.Parent = this;
                    DepProjects.Add(depProj);
                }
            }
        }

        public void ClearLog()
        {
            Log = String.Empty;
        }

        public void AppendLog(string text)
        {
            Log += text + "\r\n";
            if (LogChanged != null)
                LogChanged(this, EventArgs.Empty);
        }
    }
}
