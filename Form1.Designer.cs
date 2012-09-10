namespace DepAnalyzer
{
	partial class MainForm
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.ListViewItem listViewItem4 = new System.Windows.Forms.ListViewItem("asd", 2);
            System.Windows.Forms.ListViewItem listViewItem5 = new System.Windows.Forms.ListViewItem("asdasdasdasdasd", 1);
            System.Windows.Forms.ListViewItem listViewItem6 = new System.Windows.Forms.ListViewItem("asdasd", 0);
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.GraphContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.copyImageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.GraphPanel = new System.Windows.Forms.Panel();
            this.GraphPictureBox = new System.Windows.Forms.PictureBox();
            this.ProjectList = new System.Windows.Forms.ListBox();
            this.SolutionTextBox = new System.Windows.Forms.TextBox();
            this.BrowseButton = new System.Windows.Forms.Button();
            this.ParseButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.ProjectTabControl = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.MatrixDataGridView = new System.Windows.Forms.DataGridView();
            this.Column1 = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.LogCombo = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.ConfigTextBox = new System.Windows.Forms.TextBox();
            this.BuildList = new System.Windows.Forms.ListView();
            this.ProjectColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.BuildContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.showLogToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rebuildToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ProjectStatusImages = new System.Windows.Forms.ImageList(this.components);
            this.LogTextBox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.BuildButton = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.ShowSingleCheckBox = new System.Windows.Forms.CheckBox();
            this.ShowChildrenCheckBox = new System.Windows.Forms.CheckBox();
            this.GraphContextMenu.SuspendLayout();
            this.GraphPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.GraphPictureBox)).BeginInit();
            this.ProjectTabControl.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.MatrixDataGridView)).BeginInit();
            this.tabPage3.SuspendLayout();
            this.BuildContextMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // GraphContextMenu
            // 
            this.GraphContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.copyImageToolStripMenuItem});
            this.GraphContextMenu.Name = "contextMenuStrip1";
            this.GraphContextMenu.Size = new System.Drawing.Size(139, 26);
            // 
            // copyImageToolStripMenuItem
            // 
            this.copyImageToolStripMenuItem.Name = "copyImageToolStripMenuItem";
            this.copyImageToolStripMenuItem.Size = new System.Drawing.Size(138, 22);
            this.copyImageToolStripMenuItem.Text = "Copy Image";
            this.copyImageToolStripMenuItem.Click += new System.EventHandler(this.copyImageToolStripMenuItem_Click);
            // 
            // GraphPanel
            // 
            this.GraphPanel.AutoScroll = true;
            this.GraphPanel.AutoScrollMargin = new System.Drawing.Size(10, 10);
            this.GraphPanel.AutoScrollMinSize = new System.Drawing.Size(10, 10);
            this.GraphPanel.Controls.Add(this.GraphPictureBox);
            this.GraphPanel.Location = new System.Drawing.Point(6, 6);
            this.GraphPanel.Name = "GraphPanel";
            this.GraphPanel.Size = new System.Drawing.Size(830, 649);
            this.GraphPanel.TabIndex = 1;
            // 
            // GraphPictureBox
            // 
            this.GraphPictureBox.ContextMenuStrip = this.GraphContextMenu;
            this.GraphPictureBox.Location = new System.Drawing.Point(3, 3);
            this.GraphPictureBox.Name = "GraphPictureBox";
            this.GraphPictureBox.Size = new System.Drawing.Size(251, 163);
            this.GraphPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.GraphPictureBox.TabIndex = 0;
            this.GraphPictureBox.TabStop = false;
            // 
            // ProjectList
            // 
            this.ProjectList.FormattingEnabled = true;
            this.ProjectList.Location = new System.Drawing.Point(12, 97);
            this.ProjectList.Name = "ProjectList";
            this.ProjectList.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.ProjectList.Size = new System.Drawing.Size(136, 628);
            this.ProjectList.TabIndex = 2;
            this.ProjectList.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged);
            // 
            // SolutionTextBox
            // 
            this.SolutionTextBox.Location = new System.Drawing.Point(90, 12);
            this.SolutionTextBox.Name = "SolutionTextBox";
            this.SolutionTextBox.Size = new System.Drawing.Size(752, 20);
            this.SolutionTextBox.TabIndex = 3;
            // 
            // BrowseButton
            // 
            this.BrowseButton.Location = new System.Drawing.Point(848, 9);
            this.BrowseButton.Name = "BrowseButton";
            this.BrowseButton.Size = new System.Drawing.Size(75, 23);
            this.BrowseButton.TabIndex = 4;
            this.BrowseButton.Text = "Browse...";
            this.BrowseButton.UseVisualStyleBackColor = true;
            this.BrowseButton.Click += new System.EventHandler(this.button1_Click);
            // 
            // ParseButton
            // 
            this.ParseButton.Location = new System.Drawing.Point(929, 9);
            this.ParseButton.Name = "ParseButton";
            this.ParseButton.Size = new System.Drawing.Size(75, 23);
            this.ParseButton.TabIndex = 5;
            this.ParseButton.Text = "Parse";
            this.ParseButton.UseVisualStyleBackColor = true;
            this.ParseButton.Click += new System.EventHandler(this.button2_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(72, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Solution path:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 81);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(101, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "Project Dep graphs:";
            // 
            // ProjectTabControl
            // 
            this.ProjectTabControl.Controls.Add(this.tabPage1);
            this.ProjectTabControl.Controls.Add(this.tabPage2);
            this.ProjectTabControl.Controls.Add(this.tabPage3);
            this.ProjectTabControl.Location = new System.Drawing.Point(154, 38);
            this.ProjectTabControl.Name = "ProjectTabControl";
            this.ProjectTabControl.SelectedIndex = 0;
            this.ProjectTabControl.Size = new System.Drawing.Size(850, 687);
            this.ProjectTabControl.TabIndex = 8;
            // 
            // tabPage1
            // 
            this.tabPage1.BackColor = System.Drawing.Color.Transparent;
            this.tabPage1.Controls.Add(this.GraphPanel);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.tabPage1.Size = new System.Drawing.Size(842, 661);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Graph";
            // 
            // tabPage2
            // 
            this.tabPage2.BackColor = System.Drawing.Color.Transparent;
            this.tabPage2.Controls.Add(this.MatrixDataGridView);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(842, 661);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Matrix";
            // 
            // MatrixDataGridView
            // 
            this.MatrixDataGridView.AllowUserToAddRows = false;
            this.MatrixDataGridView.AllowUserToDeleteRows = false;
            this.MatrixDataGridView.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.MatrixDataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.MatrixDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.MatrixDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1,
            this.Column2});
            this.MatrixDataGridView.Location = new System.Drawing.Point(6, 6);
            this.MatrixDataGridView.Name = "MatrixDataGridView";
            this.MatrixDataGridView.ReadOnly = true;
            this.MatrixDataGridView.Size = new System.Drawing.Size(830, 649);
            this.MatrixDataGridView.TabIndex = 0;
            // 
            // Column1
            // 
            this.Column1.HeaderText = "Column1";
            this.Column1.Name = "Column1";
            this.Column1.ReadOnly = true;
            // 
            // Column2
            // 
            this.Column2.HeaderText = "Column2";
            this.Column2.Name = "Column2";
            this.Column2.ReadOnly = true;
            // 
            // tabPage3
            // 
            this.tabPage3.BackColor = System.Drawing.Color.Transparent;
            this.tabPage3.Controls.Add(this.LogCombo);
            this.tabPage3.Controls.Add(this.label5);
            this.tabPage3.Controls.Add(this.ConfigTextBox);
            this.tabPage3.Controls.Add(this.BuildList);
            this.tabPage3.Controls.Add(this.LogTextBox);
            this.tabPage3.Controls.Add(this.label4);
            this.tabPage3.Controls.Add(this.BuildButton);
            this.tabPage3.Controls.Add(this.label3);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(842, 661);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Build";
            // 
            // LogCombo
            // 
            this.LogCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.LogCombo.FormattingEnabled = true;
            this.LogCombo.Items.AddRange(new object[] {
            "<Build Log>"});
            this.LogCombo.Location = new System.Drawing.Point(218, 8);
            this.LogCombo.Name = "LogCombo";
            this.LogCombo.Size = new System.Drawing.Size(409, 21);
            this.LogCombo.TabIndex = 15;
            this.LogCombo.DropDown += new System.EventHandler(this.LogCombo_DropDown);
            this.LogCombo.SelectedIndexChanged += new System.EventHandler(this.LogCombo_SelectedIndexChanged);
            this.LogCombo.DropDownClosed += new System.EventHandler(this.LogCombo_DropDownClosed);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(633, 12);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(97, 13);
            this.label5.TabIndex = 14;
            this.label5.Text = "Build configuration:";
            // 
            // ConfigTextBox
            // 
            this.ConfigTextBox.Location = new System.Drawing.Point(736, 9);
            this.ConfigTextBox.Name = "ConfigTextBox";
            this.ConfigTextBox.Size = new System.Drawing.Size(100, 20);
            this.ConfigTextBox.TabIndex = 13;
            // 
            // BuildList
            // 
            this.BuildList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.ProjectColumn});
            this.BuildList.ContextMenuStrip = this.BuildContextMenu;
            listViewItem4.StateImageIndex = 0;
            listViewItem5.Checked = true;
            listViewItem5.StateImageIndex = 1;
            listViewItem6.Checked = true;
            listViewItem6.StateImageIndex = 2;
            this.BuildList.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem4,
            listViewItem5,
            listViewItem6});
            this.BuildList.LargeImageList = this.ProjectStatusImages;
            this.BuildList.Location = new System.Drawing.Point(6, 35);
            this.BuildList.MultiSelect = false;
            this.BuildList.Name = "BuildList";
            this.BuildList.ShowGroups = false;
            this.BuildList.Size = new System.Drawing.Size(150, 620);
            this.BuildList.StateImageList = this.ProjectStatusImages;
            this.BuildList.TabIndex = 12;
            this.BuildList.UseCompatibleStateImageBehavior = false;
            this.BuildList.View = System.Windows.Forms.View.Details;
            this.BuildList.MouseClick += new System.Windows.Forms.MouseEventHandler(this.BuildList_MouseClick);
            this.BuildList.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.BuildList_MouseDoubleClick);
            // 
            // ProjectColumn
            // 
            this.ProjectColumn.Text = "Projects";
            this.ProjectColumn.Width = 146;
            // 
            // BuildContextMenu
            // 
            this.BuildContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showLogToolStripMenuItem,
            this.rebuildToolStripMenuItem});
            this.BuildContextMenu.Name = "BuildContextMenu";
            this.BuildContextMenu.Size = new System.Drawing.Size(160, 48);
            // 
            // showLogToolStripMenuItem
            // 
            this.showLogToolStripMenuItem.Name = "showLogToolStripMenuItem";
            this.showLogToolStripMenuItem.Size = new System.Drawing.Size(159, 22);
            this.showLogToolStripMenuItem.Text = "Show log";
            this.showLogToolStripMenuItem.Click += new System.EventHandler(this.showLogToolStripMenuItem_Click);
            // 
            // rebuildToolStripMenuItem
            // 
            this.rebuildToolStripMenuItem.Name = "rebuildToolStripMenuItem";
            this.rebuildToolStripMenuItem.Size = new System.Drawing.Size(159, 22);
            this.rebuildToolStripMenuItem.Text = "Mark for rebuild";
            this.rebuildToolStripMenuItem.Click += new System.EventHandler(this.rebuildToolStripMenuItem_Click);
            // 
            // ProjectStatusImages
            // 
            this.ProjectStatusImages.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ProjectStatusImages.ImageStream")));
            this.ProjectStatusImages.TransparentColor = System.Drawing.Color.Transparent;
            this.ProjectStatusImages.Images.SetKeyName(0, "wait.png");
            this.ProjectStatusImages.Images.SetKeyName(1, "process.png");
            this.ProjectStatusImages.Images.SetKeyName(2, "success.png");
            this.ProjectStatusImages.Images.SetKeyName(3, "failed.png");
            // 
            // LogTextBox
            // 
            this.LogTextBox.Location = new System.Drawing.Point(162, 35);
            this.LogTextBox.MaxLength = 10240000;
            this.LogTextBox.Multiline = true;
            this.LogTextBox.Name = "LogTextBox";
            this.LogTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.LogTextBox.Size = new System.Drawing.Size(674, 620);
            this.LogTextBox.TabIndex = 11;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(162, 11);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(50, 13);
            this.label4.TabIndex = 10;
            this.label4.Text = "Build log:";
            // 
            // BuildButton
            // 
            this.BuildButton.Location = new System.Drawing.Point(72, 6);
            this.BuildButton.Name = "BuildButton";
            this.BuildButton.Size = new System.Drawing.Size(75, 23);
            this.BuildButton.TabIndex = 9;
            this.BuildButton.Text = "Start Build";
            this.BuildButton.UseVisualStyleBackColor = true;
            this.BuildButton.Click += new System.EventHandler(this.button3_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 11);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(60, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "Build order:";
            // 
            // ShowSingleCheckBox
            // 
            this.ShowSingleCheckBox.AutoSize = true;
            this.ShowSingleCheckBox.Location = new System.Drawing.Point(12, 38);
            this.ShowSingleCheckBox.Name = "ShowSingleCheckBox";
            this.ShowSingleCheckBox.Size = new System.Drawing.Size(115, 17);
            this.ShowSingleCheckBox.TabIndex = 9;
            this.ShowSingleCheckBox.Text = "Show single nodes";
            this.ShowSingleCheckBox.UseVisualStyleBackColor = true;
            // 
            // ShowChildrenCheckBox
            // 
            this.ShowChildrenCheckBox.AutoSize = true;
            this.ShowChildrenCheckBox.Location = new System.Drawing.Point(12, 61);
            this.ShowChildrenCheckBox.Name = "ShowChildrenCheckBox";
            this.ShowChildrenCheckBox.Size = new System.Drawing.Size(110, 17);
            this.ShowChildrenCheckBox.TabIndex = 10;
            this.ShowChildrenCheckBox.Text = "Show child nodes";
            this.ShowChildrenCheckBox.UseVisualStyleBackColor = true;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1016, 741);
            this.Controls.Add(this.ShowChildrenCheckBox);
            this.Controls.Add(this.ShowSingleCheckBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.ProjectTabControl);
            this.Controls.Add(this.SolutionTextBox);
            this.Controls.Add(this.ParseButton);
            this.Controls.Add(this.ProjectList);
            this.Controls.Add(this.BrowseButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainForm";
            this.Text = "Solution Dependency Analyzer";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.GraphContextMenu.ResumeLayout(false);
            this.GraphPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.GraphPictureBox)).EndInit();
            this.ProjectTabControl.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.MatrixDataGridView)).EndInit();
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            this.BuildContextMenu.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.PictureBox GraphPictureBox;
		private System.Windows.Forms.Panel GraphPanel;
		private System.Windows.Forms.ListBox ProjectList;
		private System.Windows.Forms.ContextMenuStrip GraphContextMenu;
		private System.Windows.Forms.ToolStripMenuItem copyImageToolStripMenuItem;
        private System.Windows.Forms.TextBox SolutionTextBox;
        private System.Windows.Forms.Button BrowseButton;
        private System.Windows.Forms.Button ParseButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TabControl ProjectTabControl;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.CheckBox ShowSingleCheckBox;
        private System.Windows.Forms.CheckBox ShowChildrenCheckBox;
        private System.Windows.Forms.DataGridView MatrixDataGridView;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Column1;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Column2;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.TextBox LogTextBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button BuildButton;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ImageList ProjectStatusImages;
        private System.Windows.Forms.ListView BuildList;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox ConfigTextBox;
        private System.Windows.Forms.ColumnHeader ProjectColumn;
        private System.Windows.Forms.ContextMenuStrip BuildContextMenu;
        private System.Windows.Forms.ToolStripMenuItem rebuildToolStripMenuItem;
        private System.Windows.Forms.ComboBox LogCombo;
        private System.Windows.Forms.ToolStripMenuItem showLogToolStripMenuItem;

	}
}

