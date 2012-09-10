using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace DepAnalyzer
{
    class SolutionParser
    {
        private const string CPP_PROJECT_TYPE_GUID = "{8BC9CEB8-8B4A-11D0-8D11-00A0C91BC942}";
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

        public static void ParseSolution(string solutionSource, bool isUpdate)
        {
            string solutionDir = new FileInfo(solutionSource).Directory.FullName;

            if (ProjTable == null || !isUpdate)
            {
                ProjTable = new Dictionary<string, Project>();
            }
            else
            {
                foreach (Project proj in ProjTable.Values)
                {
                    proj.ClearDepGraph();
                }
            }

            // Parse deps from solution
            Dictionary<string, Project> guidTable = new Dictionary<string, Project>();
            Project currentProj = null;
            bool projectDepencenciesSection = false;
            foreach (string rawLine in File.ReadAllLines(solutionSource))
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
                    string ProjectTypeGUID = s1[0].Split('\"')[1].ToUpper();
                    string[] s2 = s1[1].Split(',');
                    char[] trim_chars = new char[] { '\"', ' ' };
                    string projectName = s2[0].Trim(trim_chars);
                    string projectPath = s2[1].Trim(trim_chars);
                    string projectGUID = s2[2].Trim(trim_chars).ToUpper();
                    Project proj = null;
                    if (ProjectTypeGUID == CPP_PROJECT_TYPE_GUID)
                    {
                        if (isUpdate)
                            proj = ProjTable.Values.FirstOrDefault(p => p.GUID == projectGUID);
                        if (proj == null)
                            proj = new Project(projectName, Path.Combine(solutionDir, projectPath), projectGUID, false);
                        guidTable[projectGUID] = proj;
                    }
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
                        string projectGUID = s1[0].Trim().ToUpper();
                        string projectGUID2 = s1[1].Trim().ToUpper();
                        currentProj.AddDepGUID(projectGUID);
                        continue;
                    }
                }
            }

            // Parse deps from project files
            // NOTE: External dependencies are only for graph presentation and are excluded from check builds
            foreach (Project proj in guidTable.Values.ToArray())
            {
                ParseProject(proj, guidTable);
            }

            // Resolve deps
            foreach (Project proj in guidTable.Values)
            {
                if (!isUpdate)
                {
                    // Unify project name on collision
                    string originalName = proj.Name;
                    int index = 0;
                    while (ProjTable.ContainsKey(proj.Name))
                    {
                        proj.Name = String.Format("{0}[{1}]", originalName, ++index);
                    }
                    ProjTable.Add(proj.Name, proj);
                }
                proj.ResolveDeps(guidTable);
            }
        }

        private static void ParseProject(Project proj, Dictionary<string, Project> guidTable)
        {
            try
            {
                string fileContent = File.ReadAllText(proj.File.FullName);
                string projectDir = proj.File.Directory.FullName;
                using (XmlReader reader = XmlReader.Create(new StringReader(fileContent)))
                {
                    while (reader.ReadToFollowing("ProjectReference"))
                    {
                        reader.MoveToAttribute("Include");
                        string projectPath = reader.Value;
                        if (reader.ReadToFollowing("Project"))
                        {
                            string projectGUID = reader.ReadElementContentAsString().ToUpper();
                            Project depProj;
                            if (!guidTable.TryGetValue(projectGUID, out depProj))
                            {
                                // Create external project
                                FileInfo projectFile = new FileInfo(Path.Combine(projectDir, projectPath));
                                string projectName = Path.GetFileNameWithoutExtension(projectFile.Name);
                                depProj = new Project(projectName, projectFile.FullName, projectGUID, true);
                                guidTable[projectGUID] = depProj;
                            }
                            proj.AddDepGUID(projectGUID);
                        }
                    }
                }
            }
            catch(Exception e)
            {
                MessageBox.Show(e.ToString(), "Parse Project", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        public static string[] GetRootNames(bool showSingleNodes)
        {
            List<string> projNames = new List<string>();
            foreach (Project proj in ProjTable.Values)
            {
                if (!proj.HasParent && (showSingleNodes || proj.HasChildren)) // exclude single nodes
                {
                    projNames.Add(proj.Name);
                }
            }
            projNames.Sort();
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
                    projNames.Add(depProj.Name);
                }
            }
            return projNames.ToArray();
        }
    }
}
