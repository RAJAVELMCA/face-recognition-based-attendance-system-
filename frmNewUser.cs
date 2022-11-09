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
    public partial class frmNewUser : Form
    { 
        public frmNewUser()
        {
            InitializeComponent();
        }


        private void btnCancel_Click(object sender, EventArgs e)
        {
            Core.ClearTextBox(txtUserName, txtPassword, txtConPassword, txtFirstName, txtLastName);
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(txtFirstName.Text))
                {
                    MessageBox.Show("Please Enter First Name.", "Tracking", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtFirstName.Focus();
                    return;
                }
                if (string.IsNullOrEmpty(txtLastName.Text))
                {
                    MessageBox.Show("Please Enter Last Name.", "Tracking", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtLastName.Focus();
                    return;
                }
                if (string.IsNullOrEmpty(txtUserName.Text))
                {
                    MessageBox.Show("Please Enter User Name.", "Tracking", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtUserName.Focus();
                    return;
                }
                if (string.IsNullOrEmpty(txtPassword.Text))
                {
                    MessageBox.Show("Please Enter Password.", "Tracking", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtPassword.Focus();
                    return;
                }
                if (string.IsNullOrEmpty(txtConPassword.Text))
                {
                    MessageBox.Show("Please Enter Confirmation Password.", "Tracking", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtConPassword.Focus();
                    return;
                }
                if (txtPassword.Text != txtConPassword.Text)
                {
                    MessageBox.Show("Password mismatch, Please Check.", "Tracking", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtPassword.Focus();
                    return;
                }

                DbConcept db = new DbConcept();
                int iResult = db.NewUser(txtUserName.Text, txtFirstName.Text, txtLastName.Text, txtPassword.Text);
                if (iResult > 0)
                {
                    MessageBox.Show("New user Successfully Created.", "Tracking", MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("New user Not Created.", "Tracking", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Tracking", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}