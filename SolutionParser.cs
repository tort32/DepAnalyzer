using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DepAnalyzer
{
    class SolutionParser
    {
        public static Dictionary<string, Project> ProjTable
        {
            get;
            private set;
        }

        public static string VSVersion { get; private set; }

        public static string VSVerCode(string version) {
            switch (version)
            {
                case "2005": return "VS80";
                case "2008": return "VS90";
                case "2010": return "VS100";
            }
            return null;
        }

        public static void ParseSolution(string[] solutionLines, bool isUpdate = false)
        {
            if (ProjTable == null || !isUpdate)
                ProjTable = new Dictionary<string, Project>();

            // parse deps
            Dictionary<string, Project> guidTable = new Dictionary<string, Project>();
            Project currentProj = null;
            bool projectDepencenciesSection = false;
            foreach (string rawLine in solutionLines)
            {
                string line = rawLine.Trim();
                if (line.StartsWith("# Visual Studio"))
                {
                    // # Visual Studio 2010
                    string[] s1 = line.Split(' ');
                    VSVersion = s1[3];
                }
                if (line.StartsWith("Project("))
                {
                    // Project("{8BC9CEB8-8B4A-11D0-8D11-00A0C91BC942}") = "EtlViewRealTimeTest", "..\Tools\EtlViewRealTimeTest\EtlViewRealTimeTest.vcxproj", "{3DC6B51F-56DB-4207-8AB6-EAAA78DBBBF3}"
                    string[] s1 = line.Split('=');
                    string RootGUID = s1[0].Split('\"')[1];
                    string[] s2 = s1[1].Split(',');
                    char[] trim_chars = new char[] { '\"', ' ' };
                    string projectName = s2[0].Trim(trim_chars);
                    string projectPath = s2[1].Trim(trim_chars);
                    string projectGUID = s2[2].Trim(trim_chars);
                    Project proj = null;
                    if (isUpdate)
                        proj = ProjTable.Values.FirstOrDefault(p => p.mGUID == projectGUID);
                    if(proj == null)
                        proj = new Project(projectName, projectPath, projectGUID);
                    guidTable[projectGUID] = proj;
                    currentProj = proj;
                    continue;
                }
                if (line == "EndProject")
                {
                    currentProj = null;
                    continue;
                }
                if (currentProj != null)
                {
                    if (line.StartsWith("ProjectSection("))
                    {
                        // ProjectSection(ProjectDependencies) = postProject
                        string[] s1 = line.Split('=');
                        char[] split_chars = new char[] { '(', ')' };
                        string sectionName = s1[0].Split(split_chars)[1];
                        string sectionStage = s1[1].Trim();
                        if (sectionName == "ProjectDependencies")
                            projectDepencenciesSection = true;
                        continue;
                    }
                    if (line == "EndProjectSection")
                    {
                        projectDepencenciesSection = false;
                        continue;
                    }
                    if (projectDepencenciesSection)
                    {
                        // {2901AF0B-7C78-4256-AE90-83C078387FD1} = {2901AF0B-7C78-4256-AE90-83C078387FD1}
                        string[] s1 = line.Split('=');
                        string projectGUID = s1[0].Trim();
                        string projectGUID2 = s1[1].Trim();
                        currentProj.AddDepGUID(projectGUID);
                        continue;
                    }
                }
            }

            // Resolve deps
            foreach (Project proj in guidTable.Values)
            {
                if (!isUpdate)
                {
                    string originalName = proj.mName;
                    int index = 0;
                    while (ProjTable.ContainsKey(proj.mName))
                    {
                        proj.mName = String.Format("{0}[{1}]", originalName, ++index);
                    }
                    ProjTable.Add(proj.mName, proj);
                }
                proj.ResolveDeps(guidTable);
            }
        }

        public static string[] GetRootNames(bool showSingleNodes)
        {
            List<string> projNames = new List<string>();
            foreach (Project proj in ProjTable.Values)
            {
                if (!proj.HasParent && (showSingleNodes || proj.HasChildren)) // exclude single nodes
                {
                    projNames.Add(proj.mName);
                }
            }
            return projNames.ToArray();
        }

        public static string[] GetDepNames(string projName)
        {
            List<string> projNames = new List<string>();
            Project proj = ProjTable[projName];
            foreach (Project depProj in proj.ProjectTree)
            {
                if (depProj != proj && depProj.HasChildren)
                {
                    projNames.Add(depProj.mName);
                }
            }
            return projNames.ToArray();
        }
    }
}
