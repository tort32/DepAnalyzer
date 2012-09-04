using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace DepAnalyzer
{
    internal class GraphGenerator
    {
        private string[] roots = null;
        private List<Project> graphProjs = null;
        private PictureBox picture;

        public GraphGenerator(PictureBox pictureBox)
        {
            picture = pictureBox;
            roots = null;
            graphProjs = null;
        }

        public void GenerateGraphImageForRoots(string[] rootNames)
        {
            if (picture.InvokeRequired)
            {
                picture.Invoke(new MethodInvoker(() => { GenerateGraphImageForRoots(rootNames); }));
                return;
            }

            string dotCode = GenerateGraphForRoots(rootNames);
            Image img = GenerateDotGraphImage(dotCode);
            picture.Image = img;
            picture.Size = img.Size;
        }

        private void OnProjectUpdate(object sender, EventArgs e)
        {
            GenerateGraphImageForRoots(roots);
        }

        private string ProjectStatusColor(Project proj)
        {
            if (proj.Status == Project.BuildStatus.Wait)
                return "gray";
            if (proj.Status == Project.BuildStatus.Progress)
                return "skyblue";
            if (proj.Status == Project.BuildStatus.Success)
                return "palegreen";
            if (proj.Status == Project.BuildStatus.Failed)
                return "tomato";
            return "white";
        }

        private string GenerateGraphForRoots(string[] rootNames)
        {
            string codeStr;
            List<string> codeLines = new List<string>();
            List<string> topRoots = new List<string>();

            // Build graph from roots
            bool signEvent = false;
            if(roots != rootNames)
            {
                signEvent = true;
                roots = rootNames;
                if(graphProjs != null)
                {
                    foreach (Project proj in graphProjs)
                    {
                        proj.StatusChanged -= OnProjectUpdate;
                    }
                }
            }
            graphProjs = new List<Project>();
            foreach (string rootName in rootNames)
            {
                Project proj = SolutionParser.ProjTable[rootName];

                if (!proj.HasParent) // top root node
                {
                    codeStr = String.Format("\"{0}\";", rootName);
                    topRoots.Add(codeStr);
                }

                codeStr = String.Format("\"{0}\" [shape=box,style=filled,color={1}];", rootName, ProjectStatusColor(proj));
                codeLines.Add(codeStr);

                foreach (Project depProj in proj.ProjectTree)
                {
                    graphProjs.AddUnique(depProj);
                }
            }
            codeStr = "{ rank = same;" + String.Join(" ", topRoots.ToArray()) + "}";
            codeLines.Add(codeStr);

            // Add graph edges
            foreach (Project proj in graphProjs)
            {
                if (signEvent)
                    proj.StatusChanged += OnProjectUpdate;

                string projName = proj.mName;
                foreach (Project depProj in proj.DepProjects)
                {
                    string depName = depProj.mName;
                    codeStr = String.Format("node[shape=ellipse,style=filled,color={0}];", ProjectStatusColor(depProj));
                    codeLines.Add(codeStr);
                    codeStr = String.Format("\"{0}\" -> \"{1}\";", projName, depName);
                    codeLines.Add(codeStr);
                }
            }

            // Generate graph image
            string dotCode = "digraph G {\n" + String.Join("\n", codeLines.ToArray()) + "}";
            return dotCode;
        }

        private static Image GenerateDotGraphImage(string dotCode)
        {
            // Generate dot graph
            WINGRAPHVIZLib.DOTClass dot = new WINGRAPHVIZLib.DOTClass();
            WINGRAPHVIZLib.BinaryImage img = dot.ToPNG(dotCode);

            // Convert Base64 String to byte[]
            byte[] imageBytes = Convert.FromBase64String(img.ToBase64String());
            MemoryStream ms = new MemoryStream(imageBytes, 0, imageBytes.Length);

            // Convert byte[] to Stream
            ms.Write(imageBytes, 0, imageBytes.Length);

            // Convert Stream to Image
            Image image = Image.FromStream(ms, true);
            return image;
        }
    }
}
