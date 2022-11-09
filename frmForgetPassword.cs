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
    public partial class frmForgetPassword : Form
    {
      
        public frmForgetPassword( )
        {
            InitializeComponent(); 
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnGetPassword_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(txtUserName.Text))
                {
                    MessageBox.Show("Please Enter User Name.", "Tracking", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtUserName.Focus();
                    return;
                }
                DbConcept db = new DbConcept();
                string password = db.ForgetPassword(txtUserName.Text);
                MessageBox.Show($"User Password is {password}", "Tracking", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Tracking", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Core.ClearTextBox(txtUserName);
        }

        private void lblUserName_Click(object sender, EventArgs e)
        {

        }
    }
}
