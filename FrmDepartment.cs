using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FaceTracking
{
    public partial class FrmDepartment : Form
    {
        public FrmDepartment()
        {
            InitializeComponent();
            btnReset.PerformClick();
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(TxtDepartmentName.Text))
            {
                MessageBox.Show("Please Enter Department Name.", "Tracking", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                TxtDepartmentName.Focus();
                return;
            }
            if (string.IsNullOrEmpty(TxtDepartmentCode.Text))
            {
                MessageBox.Show("Please Enter Department Code.", "Tracking", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                TxtDepartmentCode.Focus();
                return;
            }
            int iResult = 0;
            if (btnSubmit.Text == "Save")
            {
                iResult = new DbConcept().NewDepartment(TxtDepartmentName.Text, TxtDepartmentCode.Text);
                if (iResult > 0)
                {
                    MessageBox.Show("Department details Successfully Saved.", "Tracking", MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Department details Not Saved.", "Tracking", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else if (btnSubmit.Text == "Update")
            {
                iResult = new DbConcept().UpdateDepartment(Convert.ToInt32(this.Tag), TxtDepartmentName.Text, TxtDepartmentCode.Text);
                if (iResult > 0)
                {
                    MessageBox.Show("Department details Successfully Updated.", "Tracking", MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Department details Not Updated.", "Tracking", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            LoadGrid();
            btnReset_Click(sender, e);
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            Core.ClearTextBox(TxtDepartmentCode, TxtDepartmentName);
            btnSubmit.Text = "Save";
            this.Tag = null;
        }

        void LoadGrid()
        {
            DataTable dt = new DbConcept().GetAllDepartments();
            dgvDepartment = Core.Grid(dgvDepartment, dt);
        }

        private void FrmDepartment_Load(object sender, EventArgs e)
        {

            LoadGrid();
        }

        private void dgvDepartment_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            var x = (DataGridView)sender;
            if (x.Columns[e.ColumnIndex] is DataGridViewButtonColumn)
            {
                DataGridViewRow row = this.dgvDepartment.Rows[e.RowIndex];
                if (e.ColumnIndex == 3)
                {
                    
                    TxtDepartmentCode.Text = row.Cells["DepartmentCode"].Value.ToString();
                    TxtDepartmentName.Text = row.Cells["DepartmentName"].Value.ToString();
                    this.Tag = row.Cells["DepartmentId"].Value.ToString();
                    btnSubmit.Text = "Update";
                }
                if (e.ColumnIndex == 4)
                {
                    this.Tag = row.Cells["DepartmentId"].Value.ToString();
                    DialogResult dialog = MessageBox.Show("Do you want to delete.?", "Tracking", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (dialog == DialogResult.Yes)
                    {
                        int iResult = new DbConcept().DeleteDepartment(Convert.ToInt32(this.Tag));
                        if (iResult > 0)
                        {
                            MessageBox.Show("Department details Successfully Deleted.", "Tracking", MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show("Department details Not Deleted.", "Tracking", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                        btnReset.PerformClick();
                        LoadGrid();
                    }
                }
            }
        }
    }
}
