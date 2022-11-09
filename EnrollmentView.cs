using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace FaceTracking
{
    public partial class EnrollmentView : Form
    {
        public EnrollmentView()
        {
            InitializeComponent();
        }

        private byte[] _photoByte;

        void LoadGrid()
        {
            DataTable dt = new DbConcept().GetAllEnrollment();
            dgvEnrollment = Core.Grid(dgvEnrollment, dt);
        }

        private void EnrollmentView_Load(object sender, EventArgs e)
        {
            BtnBrowse.Enabled = false;
            btnSubmit.Enabled = false;
            btnSubmit.Text = "Update";
            LoadGrid();
            DataTable dt = new DbConcept().GetAllDepartments();
            cmbDepartment.DisplayMember = "DepartmentCode";
            cmbDepartment.ValueMember = "DepartmentId";
            DataRow Drw;
            Drw = dt.NewRow();
            Drw.ItemArray = new object[] { 0, "-Select-" };
            dt.Rows.InsertAt(Drw, 0);
            cmbDepartment.DataSource = dt;
            cmbDepartment.SelectedIndex = 0;
        }
        private byte[] GetImage()
        {
            string sFile;
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            byte[] imgByte = null;
            openFileDialog1.Filter = "Image File (*.jpg;*.bmp;*.gif,*.JPEG)|*.jpg;*.bmp;*.gif,*.JPEG";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                sFile = openFileDialog1.FileName;
                picBox.Image = System.Drawing.Bitmap.FromFile(sFile);
                picBox.SizeMode = PictureBoxSizeMode.StretchImage;
                using (MemoryStream mStream = new MemoryStream())
                {
                    picBox.Image.Save(mStream, picBox.Image.RawFormat);
                    imgByte = mStream.ToArray();
                }
            }
            return imgByte;
        }

        private void dgvEnrollment_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            var x = (DataGridView)sender;
            if (x.Columns[e.ColumnIndex] is DataGridViewButtonColumn)
            {
                DataGridViewRow row = this.dgvEnrollment.Rows[e.RowIndex];
                if (e.ColumnIndex == 13)
                {
                    this.Tag = row.Cells["EnrollmentId"].Value.ToString();
                    txtAddress.Text = row.Cells["Address"].Value.ToString();
                    txtContact.Text = row.Cells["Contact"].Value.ToString();
                    txtFirstName.Text = row.Cells["FirstName"].Value.ToString();
                    txtLastName.Text = row.Cells["LastName"].Value.ToString();
                    txtRollNo.Text = row.Cells["RollNo"].Value.ToString();
                    dtDob.Text = row.Cells["Dob"].Value.ToString();
                    string departmentName = row.Cells["Department"].Value.ToString();
                    cmbDepartment.SelectedIndex = cmbDepartment.FindString(departmentName);

                    byte[] imageBtye = (byte[])dgvEnrollment.CurrentRow.Cells["StudentPhoto"].Value;
                    string Gender = row.Cells["Gender"].Value.ToString();
                    if(Gender == "Male")
                    {
                        cboxGender.SelectedIndex = 1;
                    }
                    else  if(Gender == "Female")
                    {
                        cboxGender.SelectedIndex = 2;
                    }
                    MemoryStream ms = new MemoryStream(imageBtye);
                    picBox.Image = new Bitmap(ms);
                    btnSubmit.Text = "Update";
                    BtnBrowse.Enabled = true;
                    btnSubmit.Enabled = true;
                }
                if (e.ColumnIndex == 14)
                {
                    this.Tag = row.Cells["EnrollmentId"].Value.ToString();
                    DialogResult dialog = MessageBox.Show("Do you want to delete.?", "Tracking", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (dialog == DialogResult.Yes)
                    {
                        int iResult = new DbConcept().DeleteEntrollment(Convert.ToInt32(this.Tag));
                        if (iResult > 0)
                        {
                            MessageBox.Show("Enrollment details Successfully Deleted.", "Tracking", MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show("Enrollment details Not Deleted.", "Tracking", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                        btnReset.PerformClick();
                        LoadGrid();
                    }
                }
            }
        }
        private int AgeCalculate(DateTime anniversaire)
        {
            DateTime now = DateTime.Today;
            int age = now.Year - anniversaire.Year;
            if (anniversaire > now.AddYears(-age))
                age--;
            return age;
        }
        private bool Validation()
        {
            if (string.IsNullOrEmpty(txtRollNo.Text))
            {
                MessageBox.Show("Please Enter Roll No.", "Tracking", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtRollNo.Focus();
                return false;
            }
            if (string.IsNullOrEmpty(txtFirstName.Text))
            {
                MessageBox.Show("Please Enter First Name.", "Tracking", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtFirstName.Focus();
                return false;
            }
            if (string.IsNullOrEmpty(txtLastName.Text))
            {
                MessageBox.Show("Please Enter Last Name.", "Tracking", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtLastName.Focus();
                return false;
            }
            if (cmbDepartment.SelectedIndex == 0)
            {
                MessageBox.Show("Please Select Department.", "Tracking", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbDepartment.Focus();
                return false;
            }
            if (string.IsNullOrEmpty(txtContact.Text))
            {
                MessageBox.Show("Please Enter Contact.", "Tracking", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtContact.Focus();
                return false;
            }
            if (string.IsNullOrEmpty(txtAddress.Text))
            {
                MessageBox.Show("Please Enter Address.", "Tracking", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtAddress.Focus();
                return false;
            }
            if (cboxGender.SelectedIndex == 0)
            {
                MessageBox.Show("Please Select Gender.", "Tracking", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cboxGender.Focus();
                return false;
            }
            int Age = AgeCalculate(Convert.ToDateTime(dtDob.Text));
            if (Age <= 18)
            {
                MessageBox.Show("Please Select Dob .", "Tracking", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                dtDob.Focus();
                return false;
            }
            if (picBox.Image == null)
            {
                MessageBox.Show("Please Select Student Photo .", "Tracking", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                BtnBrowse.Focus();
                return false;
            }
            _photoByte = ImageToByte(picBox.Image);
            return true;
        }
        private byte[] ImageToByte(Image img)
        {
            ImageConverter converter = new ImageConverter();
            return (byte[])converter.ConvertTo(img, typeof(byte[]));
        }
        private void btnSubmit_Click(object sender, EventArgs e)
        {
            try
            {
                if (!Validation())
                {
                    return;
                }
                string Gender = string.Empty;
                if (cboxGender.SelectedIndex == 1)
                {
                    Gender = "Male";
                }
                else if (cboxGender.SelectedIndex == 2)
                {
                    Gender = "Female";
                }
                DbConcept db = new DbConcept();
                int Age = AgeCalculate(Convert.ToDateTime(dtDob.Text));
                int iResult = db.UpdateEntrollment( Convert.ToInt32(this.Tag), txtRollNo.Text, txtFirstName.Text,
                    txtLastName.Text, Age, txtContact.Text, txtAddress.Text, Gender, Convert.ToDateTime(dtDob.Text), _photoByte, Convert.ToInt32(cmbDepartment.SelectedValue));
                if (iResult > 0)
                {
                    MessageBox.Show("Enrollment Successfully Updated.", "Tracking", MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                    btnReset.PerformClick();
                }
                else
                {
                    MessageBox.Show("Enrollment Not Updated.", "Tracking", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                btnReset_Click(sender,e);
                LoadGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Tracking", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnBrowse_Click(object sender, EventArgs e)
        {
            _photoByte = GetImage();
            
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            Core.ClearTextBox(txtAddress, txtContact, txtFirstName, txtLastName, txtRollNo);
            picBox.Image = null;
            cboxGender.SelectedIndex = 0;
            cmbDepartment.SelectedIndex = 0;
            dtDob.Value = DateTime.Now;
            this.Tag = null;
            dgvEnrollment.ClearSelection();
            BtnBrowse.Enabled = false;
            btnSubmit.Enabled = false;
        }
    }
}