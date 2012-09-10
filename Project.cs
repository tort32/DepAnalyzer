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

            ClearDepGraph();
        }

        public string Name { get; set; }
        public FileInfo File { get; private set; }
        public string GUID { get; private set; }
        public bool IsExternal { get; private set; }

        private List<string> DepGUIDs { get; set; }

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
