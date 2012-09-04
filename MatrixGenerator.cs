using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Windows.Forms;
using System.Drawing;

namespace DepAnalyzer
{
    class MatrixGenerator
    {
        private const int box_size = 16;
        private const int box_offset = 2;

        public MatrixGenerator(DataGridView view)
        {
            dataGridView = view;

            dataGridView.Columns.Clear();

            dataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dataGridView.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            dataGridView.RowHeadersVisible = false;
            dataGridView.AllowUserToResizeColumns = false;
            dataGridView.AllowUserToResizeRows = false;

            dataGridView.CellPainting += dataGridView_CellPainting;
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
            columns[0].DataType = typeof (string);

            for (int i = 0; i < projs.Length; i++)
            {
                columns[i + 1] = new DataColumn(projs[i].mName);
                columns[i + 1].DataType = typeof (DataGridViewTriState);
            }

            table.Columns.AddRange(columns);

            // Generate rows
            for (int i = 0; i < projs.Length; i++)
            {
                DataRow row = table.NewRow();
                row[0] = columns[i + 1].ColumnName;
                for (int j = 0; j < projs.Length; j++)
                {
                    if (j <= i)
                    {
                        row[j + 1] = DataGridViewTriState.NotSet;
                    }
                    else
                    {
                        row[j + 1] = projs[i].DepProjects.Has(projs[j]) ? DataGridViewTriState.True : DataGridViewTriState.False;
                    }
                }
                table.Rows.Add(row);
            }

            dataGridView.DataSource = table;
            dataGridView.ColumnHeadersHeight = 10;
            // resize columns
            dataGridView.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            for (int i = 1; i < columns.Length; i++)
            {
                dataGridView.Columns[i].Width = box_size + 2 * box_offset;
            }
        }

        void dataGridView_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            // check that we are in a header cell!
            if (e.RowIndex == -1 && e.ColumnIndex > 0)
            {
                e.PaintBackground(e.ClipBounds, true);
                Rectangle rect = dataGridView.GetColumnDisplayRectangle(e.ColumnIndex, true);
                Size titleSize = TextRenderer.MeasureText(e.Value.ToString(), e.CellStyle.Font);
                int offset = 4;
                if (dataGridView.ColumnHeadersHeight < titleSize.Width + 2 * offset)
                {
                    dataGridView.ColumnHeadersHeight = titleSize.Width + 2 * offset;
                }

                e.Graphics.TranslateTransform(1, titleSize.Width);
                e.Graphics.RotateTransform(-90.0F);

                e.Graphics.DrawString(e.Value.ToString(), dataGridView.Font, Brushes.Black, new PointF(rect.Y - (dataGridView.ColumnHeadersHeight - titleSize.Width - offset), rect.X));

                e.Graphics.RotateTransform(90.0F);
                e.Graphics.TranslateTransform(-1, -titleSize.Width);

                e.Handled = true;
            }

            if (e.RowIndex != -1 && e.ColumnIndex > 0)
            {
                e.PaintBackground(e.ClipBounds, true);

                Rectangle rect = dataGridView.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, true);

                var CheckBoxRegion = new Rectangle(rect.X + box_offset, rect.Y + box_offset, box_size, box_size);

                var state = (DataGridViewTriState) e.Value;
                switch (state)
                {
                    case DataGridViewTriState.NotSet:
                        ControlPaint.DrawCheckBox(e.Graphics, CheckBoxRegion, ButtonState.Inactive);
                    break;
                    case DataGridViewTriState.True:
                        ControlPaint.DrawCheckBox(e.Graphics, CheckBoxRegion, ButtonState.Checked);
                    break;
                    case DataGridViewTriState.False:
                        ControlPaint.DrawCheckBox(e.Graphics, CheckBoxRegion, ButtonState.Normal);
                    break;
                }

                e.Handled = true;
            }
        }

        private DataGridView dataGridView = null;
    }
}
