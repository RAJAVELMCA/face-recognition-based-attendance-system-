using System;
using System.Configuration;
using System.Windows.Forms;

namespace FaceTracking
{
    public partial class frmHome : Form
    {
        private readonly int _userId;
        private readonly string _userName;
        public frmHome(int userId, string userName)
        {
            InitializeComponent();
            _userId = userId;
            _userName = userName;
        }
        private void attendanceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ActiveMdiChild != null)
                ActiveMdiChild.Close();

            FrmAttendanceEntry attend = new FrmAttendanceEntry(_userId);
            attend.MdiParent = this;
            attend.Show();
        }

        private void sampleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ActiveMdiChild != null)
                ActiveMdiChild.Close();

            frmSample attend = new frmSample();
            attend.MdiParent = this;
            attend.Show();
        }
        private void departmentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ActiveMdiChild != null)
                ActiveMdiChild.Close();

            FrmDepartment depart = new FrmDepartment();
            depart.MdiParent = this;
            depart.Show();
        }
        private void newEnrollmentsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ActiveMdiChild != null)
                ActiveMdiChild.Close();

            frmEnrollment enroll = new frmEnrollment();
            enroll.MdiParent = this;
            enroll.Show();
        }
        private void viewEnrollmentsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ActiveMdiChild != null)
                ActiveMdiChild.Close();

            EnrollmentView enroll = new EnrollmentView();
            enroll.MdiParent = this;
            enroll.Show();
        }
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            departmentToolStripMenuItem_Click(sender, e);
        }
        private void newEnrollmentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            newEnrollmentsToolStripMenuItem_Click(sender, e);
        }
        private void viewEnrollmentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            viewEnrollmentsToolStripMenuItem_Click(sender, e);
        }
        private void attendanceToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            attendanceToolStripMenuItem_Click(sender, e);
        }
        private void viewAttendanceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ActiveMdiChild != null)
                ActiveMdiChild.Close();

            FrmAttendanceView view = new FrmAttendanceView();
            view.MdiParent = this;
            view.Show();
        }
        private void frmHome_Load(object sender, EventArgs e)
        {
            int hour = Convert.ToInt32(ConfigurationManager.AppSettings["Hour"]);
            int Minute = Convert.ToInt32(ConfigurationManager.AppSettings["Minute"]);
            sTripAuto.Text = $"Every Day Auto Absense after at {hour}:{Minute}";
            DateTime ConditionDate = DateTime.Now;

            if(ConditionDate >= new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0) &&
               ConditionDate <= new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 11, 59, 0))
            {
                tTripUser.Text = $"Good Morning.. {_userName}";
            }
            else if (ConditionDate >= new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 11, 59, 0) && 
               ConditionDate >= new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 03, 59, 0) )
            {
                tTripUser.Text = $"Good Afternoon.. {_userName}";
            }
            else if (ConditionDate >= new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 3, 59, 0) &&
                ConditionDate >= new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0))
            {
                tTripUser.Text = $"Good Evening.. {_userName}";
            }
 
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            int hour = Convert.ToInt32(ConfigurationManager.AppSettings["Hour"]);
            int Minute = Convert.ToInt32(ConfigurationManager.AppSettings["Minute"]);
            int Second = Convert.ToInt32(ConfigurationManager.AppSettings["Second"]);
            DateTime currenetDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, hour, Minute, Second);
            DateTime ConditionDate = DateTime.Now;
            if (currenetDate <= ConditionDate)
            {
                new DbConcept().UpdateAttendance();
            }
        }
    }
}
