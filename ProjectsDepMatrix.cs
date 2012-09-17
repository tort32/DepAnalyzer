using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DepAnalyzer
{
    public partial class ProjectsDepMatrix : UserControl
    {
        private const int box_size = 16;
        private const int box_offset = 2;

        private enum ProjectDependency
        {
            None,
            Direct, // dep link in solution or project reference
            Indirect, // has dep link via graph
            Invalid,
        }

        public ProjectsDepMatrix()
        {
            InitializeComponent();

            MatrixDataGridView.Columns.Clear();

            MatrixDataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            MatrixDataGridView.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            MatrixDataGridView.RowHeadersVisible = false;
            MatrixDataGridView.AllowUserToResizeColumns = false;
            MatrixDataGridView.AllowUserToResizeRows = false;

            MatrixDataGridView.CellPainting += dataGridView_CellPainting;
        }

        public void GenerateTableForRoots(string[] rootNames)
        {
            List<Project> projs = new List<Project>();
            foreach (string rootName in rootNames)
            {
                Project proj = SolutionParser.ProjTable[rootName];
                foreach (Project depProj in proj.ProjectTree)
                {
                    projs.AddUnique(depProj);
                }
            }
            GenerateTableForProjects(projs.ToArray());
        }

        private void GenerateTableForProjects(Project[] projs)
        {
            var table = new DataTable();

            // Generate columns
            var columns = new DataColumn[projs.Length + 1];

            columns[0] = new DataColumn(@"Projects\Deps");
            columns[0].DataType = typeof(string);

            for (int i = 0; i < projs.Length; i++)
            {
                columns[i + 1] = new DataColumn(projs[i].Name);
                columns[i + 1].DataType = typeof(ProjectDependency);
            }

            table.Columns.AddRange(columns);

            // Generate rows
            for (int i = 0; i < projs.Length; i++)
            {
                DataRow row = table.NewRow();
                row[0] = columns[i + 1].ColumnName;
                for (int j = 0; j < projs.Length; j++)
                {
                    if (j == i)
                    {
                        row[j + 1] = ProjectDependency.Invalid;
                    }
                    else
                    {
                        //row[j + 1] = projs[i].DepProjects.Has(projs[j]) ? ProjectDependency.Direct : DataGridViewTriState.False;

                        ProjectDependency dep = ProjectDependency.None;
                        if (projs[i].DepProjects.Has(projs[j]))
                            dep = ProjectDependency.Direct;
                        else if (projs[i].ProjectTree.Has(projs[j]))
                            dep = ProjectDependency.Indirect;
                        else if (projs[i].Parents.Has(projs[j]))
                            dep = ProjectDependency.Invalid;
                        else
                            dep = ProjectDependency.None;
                        row[j + 1] = dep;
                    }
                }
                table.Rows.Add(row);
            }

            MatrixDataGridView.DataSource = table;
            MatrixDataGridView.ColumnHeadersHeight = 10;
            // resize columns
            MatrixDataGridView.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            for (int i = 1; i < columns.Length; i++)
            {
                MatrixDataGridView.Columns[i].Width = box_size + 2 * box_offset;
            }
        }

        private void dataGridView_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            // check that we are in a header cell!
            if (e.RowIndex == -1 && e.ColumnIndex > 0)
            {
                e.PaintBackground(e.ClipBounds, true);
                Rectangle rect = MatrixDataGridView.GetColumnDisplayRectangle(e.ColumnIndex, true);
                Size titleSize = TextRenderer.MeasureText(e.Value.ToString(), e.CellStyle.Font);
                int offset = 4;
                if (MatrixDataGridView.ColumnHeadersHeight < titleSize.Width + 2 * offset)
                {
                    MatrixDataGridView.ColumnHeadersHeight = titleSize.Width + 2 * offset;
                }

                e.Graphics.TranslateTransform(1, titleSize.Width);
                e.Graphics.RotateTransform(-90.0f);

                e.Graphics.DrawString(e.Value.ToString(), MatrixDataGridView.Font, Brushes.Black,
                    new PointF(rect.Y - (MatrixDataGridView.ColumnHeadersHeight - titleSize.Width - offset), rect.X));

                e.Graphics.RotateTransform(90.0f);
                e.Graphics.TranslateTransform(-1, -titleSize.Width);

                e.Handled = true;
            }

            if (e.RowIndex != -1 && e.ColumnIndex > 0)
            {
                var state = (ProjectDependency)e.Value;
                /*if (state == ProjectDependency.Invalid)
                    e.CellStyle.BackColor = Color.Silver;*/

                e.PaintBackground(e.ClipBounds, true);

                Rectangle rect = MatrixDataGridView.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, true);

                var CheckBoxRegion = new Rectangle(rect.X + box_offset, rect.Y + box_offset, box_size, box_size);
                switch (state)
                {
                    case ProjectDependency.None:
                        ControlPaint.DrawCheckBox(e.Graphics, CheckBoxRegion, ButtonState.Normal);
                        break;
                    case ProjectDependency.Direct:
                        ControlPaint.DrawCheckBox(e.Graphics, CheckBoxRegion, ButtonState.Checked);
                        break;
                    case ProjectDependency.Indirect:
                        ControlPaint.DrawCheckBox(e.Graphics, CheckBoxRegion, ButtonState.Checked | ButtonState.Inactive);
                        break;
                    case ProjectDependency.Invalid:
                        ControlPaint.DrawCheckBox(e.Graphics, CheckBoxRegion, ButtonState.Inactive);
                        break;
                }

                e.Handled = true;
            }
        }

    }
}
