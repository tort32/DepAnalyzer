using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using DepAnalyzer.Properties;

namespace DepAnalyzer
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            SolutionTextBox.Text = Settings.Default.SolutionFile;
            ConfigTextBox.Text = Settings.Default.SolutionConfig;
            ShowSingleCheckBox.Checked = Settings.Default.ShowSingleNodes;
            ShowChildrenCheckBox.Checked = Settings.Default.ShowChildNodes;
            BuildButton.Text = "Start";

            builder = new Builder(BuildList, LogCombo, LogTextBoxRich);

            SolutionTextBox.TextChanged += textBox1_TextChanged;
            ShowSingleCheckBox.CheckedChanged += checkBox1_CheckedChanged;
            ShowChildrenCheckBox.CheckedChanged += checkBox2_CheckedChanged;

            TryParse(false);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog diag = new OpenFileDialog();
            diag.CheckFileExists = true;
            diag.Filter = "Solution files (*.sln)|*.sln|All files (*.*)|*.*";

            string solutionSource = SolutionTextBox.Text;
            if (!String.IsNullOrEmpty(solutionSource))
            {
                FileInfo snlFileInfo = new FileInfo(solutionSource);
                if (snlFileInfo.Exists && snlFileInfo.DirectoryName != null)
                {
                    diag.InitialDirectory = snlFileInfo.DirectoryName;
                }
            }

            if(diag.ShowDialog() == DialogResult.OK)
            {
                SolutionTextBox.Text = diag.FileName;
                TryParse(true);
            }
        }

        public static bool IsSolutionValid(string path)
        {
            if (String.IsNullOrEmpty(path))
                return false;

            FileInfo snlFileInfo = new FileInfo(path);
            if (!snlFileInfo.Exists)
                return false;

            return true;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            ParseButton.Enabled = IsSolutionValid(SolutionTextBox.Text);
        }

        private bool TryParse(bool isUpdate)
        {
            if (!IsSolutionValid(SolutionTextBox.Text))
                return false;

            Program.form.Icon = Resources.tree;
            string solutionSource = SolutionTextBox.Text;
            bool update = isUpdate && (Settings.Default.SolutionFile == solutionSource);
            SolutionParser.ParseSolution(solutionSource, update);
            UpdateNodeList();

            // Update settings
            Settings.Default.SolutionFile = SolutionTextBox.Text;
            Settings.Default.Save();

            return true;
        }

        private void UpdateNodeList()
        {
            bool showSingleNodes = Settings.Default.ShowSingleNodes;
            bool showChildNodes = Settings.Default.ShowChildNodes;
            ProjectList.Items.Clear();
            foreach (string rootName in SolutionParser.GetRootNames(showSingleNodes))
            {
                ProjectList.Items.Add(rootName);
                if (showChildNodes)
                {
                    foreach (string projName in SolutionParser.GetDepNames(rootName))
                    {
                        string item = "  " + projName;
                        ProjectList.Items.Add(item);
                    }
                }
            }

            if (ProjectList.Items.Count != 0)
            {
                ProjectList.SelectedIndex = 0;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            TryParse(true);
        }

        private string[] GetSelectedRoots()
        {
            List<string> rootNames = new List<string>();
            foreach (string item in ProjectList.SelectedItems)
            {
                string projName = item.Trim();
                rootNames.Add(projName);
            }
            return rootNames.ToArray();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string[] rootNames = GetSelectedRoots();
            // update dep graph
            depGraph.GenerateGraphImageForRoots(rootNames);

            // update matrix
            depMatrix.GenerateTableForRoots(rootNames);

            // update builder
            builder.GenerateOrderListForRoots(rootNames);
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            // Update settings
            Settings.Default.ShowSingleNodes = ShowSingleCheckBox.Checked;
            Settings.Default.Save();

            UpdateNodeList();
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            // Update settings
            Settings.Default.ShowChildNodes = ShowChildrenCheckBox.Checked;
            Settings.Default.Save();

            UpdateNodeList();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string solutionConfig = ConfigTextBox.Text;
            Settings.Default.SolutionConfig = solutionConfig;
            Settings.Default.Save();

            if (BuildButton.Text == "Start")
            {
                string solutionSource = SolutionTextBox.Text;
                //ClearBuildLog();
                builder.StartBuild(SolutionParser.VSVersion, solutionSource, solutionConfig);
                BuildButton.Text = "Stop";
                Program.form.Icon = Resources.process;
            } else
            if(BuildButton.Text == "Stop")
            {
                builder.StopBuild();
                BuildButton.Text = "Start";
                Program.form.Icon = Resources.wait;
            }
        }

        public void UpdateBuildButton(string data)
        {
            BuildButton.Text = "Start";
            if (data == "Success")
            {
                Program.form.Icon = Resources.success;
                MessageBox.Show("Build finished with success", "Build", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            if (data == "Fail")
            {
                Program.form.Icon = Resources.failed;
                MessageBox.Show("Build failed. See log for more details.", "Build", MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            builder.StopBuild();
        }

        private void LogCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            builder.SelectLogCombo();
        }

        private void LogCombo_DropDown(object sender, EventArgs e)
        {
            //builder.UpdateLogCombo();
        }

        private void LogCombo_DropDownClosed(object sender, EventArgs e)
        {
            builder.PreSelectLogCombo();
        }

        private void BuildList_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                ListViewItem item = BuildList.GetItemAt(e.X, e.Y);
                if (item != null)
                {
                    var proj = SolutionParser.ProjTable[item.Text];
                    item.Selected = true;
                    BuildContextMenu.Items[0].Enabled = proj.HasLog;
                    BuildContextMenu.Items[1].Enabled = !builder.IsBuilding && proj.IsBuilt;
                    BuildContextMenu.Show(BuildList, e.Location);
                }
            }
        }

        private void showLogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var proj = SolutionParser.ProjTable[BuildList.SelectedItems[0].Text];
            LogCombo.SelectedItem = proj;
        }

        private void rebuildToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var proj = SolutionParser.ProjTable[BuildList.SelectedItems[0].Text];
            proj.Status = Project.BuildStatus.Wait;
        }

        private void BuildList_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (BuildContextMenu.Items[0].Enabled)
                showLogToolStripMenuItem_Click(sender, e);
        }

        private Builder builder;
    }
}