using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace FaceTracking
{
    public partial class FrmAttendanceView : Form
    {
        List<AttendanceDto> attendance = new List<AttendanceDto>();
        public FrmAttendanceView()
        {
            InitializeComponent();
        }

        void LoadGrid()
        {
            attendance = new DbConcept().GetAttendance();
            dgvAttendance = Core.Grid<List<AttendanceDto>>(dgvAttendance, attendance);
        }

        private void FrmAttendanceView_Load(object sender, EventArgs e)
        {
            LoadGrid();
            dgvAttendance.ClearSelection();
            DtFrom.Value = DateTime.Now.AddDays(-30);
        }

        private void BtnSearch_Click(object sender, EventArgs e)
        {
            string rollNumber = txtRollNo.Text;
            bool Absence = rBtnAbsence.Checked;
            bool Presence = rbtnPresence.Checked;
            DateTime from = Convert.ToDateTime(DtFrom.Text);
            DateTime to = Convert.ToDateTime(DtTo.Text);

            var filter = attendance.Where(w => (w.EntryDate.Date >= from.Date && w.EntryDate.Date <= to.Date) && (string.IsNullOrEmpty(rollNumber) || w.RollNumber.Contains(rollNumber) )
            && (!Absence || w.Type == "Absence") && (!Presence || w.Type == "Presence")
            ).ToList();
            dgvAttendance = Core.Grid<List<AttendanceDto>>(dgvAttendance, filter);
            dgvAttendance.ClearSelection();
        }

        void Formatting()
        {
            int pre = 0;
            int abs = 0;
            foreach (DataGridViewRow row in dgvAttendance.Rows)
            {
                if (row.Cells[5].Value.ToString() == "Absence")
                {
                    row.Cells[5].Style.BackColor = Color.Red;
                    abs++;
                }
                if (row.Cells[5].Value.ToString() == "Presence")
                {
                    row.Cells[5].Style.BackColor = Color.Green;
                    pre++;
                }
            }
            LblPresence.Text = $"Total Number Presence {pre}" ;
            LblAbsence.Text = $"Total Number Absence {abs}";
        }

        private void dgvAttendance_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            Formatting();
        }

        private void dgvAttendance_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void prtbt_Click(object sender, EventArgs e)
        {
            
        }
    }
}