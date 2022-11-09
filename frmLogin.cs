using System;
using System.Windows.Forms;
using System.IO;
namespace FaceTracking
{
    public partial class frmLogin : Form
    {
        public frmLogin()
        {
            InitializeComponent();
        }
        private void lnkForgetpassword_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            frmForgetPassword fPassword = new frmForgetPassword();
            fPassword.ShowDialog();
        }
        private void lnkNewUser_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            frmNewUser newUser = new frmNewUser();
            newUser.ShowDialog();
        }
        private void btnCancel_Click(object sender, EventArgs e)
        {
            Core.ClearTextBox(txtUserName, txtPassword);
        }
        private void btnLogin_Click(object sender, EventArgs e)
        {
            try
            {
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
                DbConcept db = new DbConcept();
                UserDto user = db.Login(txtUserName.Text, Core.Encrypt(txtPassword.Text));
                if (user.UserId > 0)
                {
                    if (!File.Exists(Application.StartupPath + "/TrainedFaces/TrainedLabels.txt"))
                    {
                        File.WriteAllText(Application.StartupPath + "/TrainedFaces/TrainedLabels.txt", String.Empty);
                    }
                    frmHome home = new frmHome(user.UserId,user.UserName);
                    this.Hide();
                    home.ShowDialog();
                }
                else
                {
                    MessageBox.Show("Invalid User Name / Password.", "Tracking", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtUserName.Focus();
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Tracking", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}