using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

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
            return projects.Exists(p => p.mName == proj.mName);
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

        public static string sSolutionPath = String.Empty;

        public string mName;
        public FileInfo mFile;
        public string mGUID;

        private List<string> mDepGUIDs;

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
            // Return all dependency projects from tree including the root (this)
            get
            {
                List<Project> projs = new List<Project>();
                projs.Add(this);
                foreach (Project depProj in DepProjects)
                {
                    foreach (Project depDepPeoj in depProj.ProjectTree)
                    {
                        projs.AddUnique(depDepPeoj);
                    }
                }
                return projs;
            }
        }

        public event EventHandler StatusChanged;

        private BuildStatus status;
        public BuildStatus Status
        {
            get
            {
                return status;
            }
            set
            {
                if (status != value)
                {
                    status = value;
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

        public bool HasLog
        {
            get { return !String.IsNullOrEmpty(Log); }
        }

        public override string ToString()
        {
            return mName;
        }

        public Project(string name, string path, string guid)
        {
            mName = name;
            mGUID = guid;
            mFile = String.IsNullOrEmpty(path) ? null : new FileInfo(Path.Combine(sSolutionPath, path));
            mDepGUIDs = new List<string>();
            Parent = null;
            DepProjects = null;
            Status = BuildStatus.Wait;
            Log = String.Empty;
        }

        public void AddDepGUID(string depGUID)
        {
            if (!mDepGUIDs.Contains(depGUID))
            {
                mDepGUIDs.Add(depGUID);
            }
        }

        public void ResolveDeps(Dictionary<string, Project> guidTable)
        {
            DepProjects = new List<Project>();
            foreach (string guid in mDepGUIDs)
            {
                Project depProj = guidTable[guid];
                depProj.Parent = this;
                DepProjects.Add(depProj);
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
