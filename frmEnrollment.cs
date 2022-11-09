using AForge.Video;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace FaceTracking
{
    public partial class frmEnrollment : Form
    {

        private byte[] _photoByte;

        #region Face Detect
        Image<Bgr, Byte> currentFrame;
        Capture grabber;
        HaarCascade face;
        MCvFont font = new MCvFont(FONT.CV_FONT_HERSHEY_TRIPLEX, 0.5d, 0.5d);
        Image<Gray, byte> result, TrainedFace = null;
        Image<Gray, byte> gray = null;
        List<Image<Gray, byte>> trainingImages = new List<Image<Gray, byte>>();
        List<string> labels = new List<string>();
        List<string> NamePersons = new List<string>();
        int ContTrain, NumLabels, t;
        string name, names = null;
        #endregion
        public frmEnrollment()
        {
            InitializeComponent();
            face = new HaarCascade("haarcascade_frontalface_default.xml");
            try
            {
                string Labelsinfo = File.ReadAllText(Application.StartupPath + "/TrainedFaces/TrainedLabels.txt");
                string[] Labels = Labelsinfo.Split('%');
                if (Labels.Length > 1)
                {
                    NumLabels = Convert.ToInt16(Labels[0]);
                    ContTrain = NumLabels;
                    string LoadFaces;

                    for (int tf = 1; tf < NumLabels + 1; tf++)
                    {
                        LoadFaces = "face" + tf + ".bmp";
                        trainingImages.Add(new Image<Gray, byte>(Application.StartupPath + "/TrainedFaces/" + LoadFaces));
                        labels.Add(Labels[tf]);
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Tracking", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
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
                else if (cboxGender.SelectedIndex == 3)
                {
                    Gender = "Third Gender";
                }
                DbConcept db = new DbConcept();
                int Age = AgeCalculate(Convert.ToDateTime(dtDob.Text));
                int iResult = db.NewEntrollment(txtRollNo.Text, txtFirstName.Text,
                    txtLastName.Text, Age, txtContact.Text, txtAddress.Text, Gender, Convert.ToDateTime(dtDob.Text), _photoByte, Convert.ToInt32(cmbDepartment.SelectedValue));
                if (iResult > 0)
                {
                    MessageBox.Show("New Enrollment Successfully Created.", "Tracking", MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("New Enrollment Not Created.", "Tracking", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Tracking", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            btnReset.PerformClick();
        }
        private int AgeCalculate(DateTime anniversaire)
        {
            DateTime now = DateTime.Today;
            int age = now.Year - anniversaire.Year;
            if (anniversaire > now.AddYears(-age))
                age--;
            return age;
        }
        private void btnBrowse_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtRollNo.Text))
            {
                MessageBox.Show("Please Enter Roll No.", "Tracking", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtRollNo.Focus();
                return;
            }
            var student = new DbConcept().FindStudent(txtRollNo.Text);
            if (string.IsNullOrEmpty(student.RollNo))
            {

                string[] StudentRollNumber = File.ReadAllLines(Application.StartupPath + "/TrainedFaces/TrainedLabels.txt");
                if (StudentRollNumber.Length > 0)
                {
                    foreach (string RollNo in StudentRollNumber)
                    {
                        string[] studentNames = RollNo.Split('%');
                        if (studentNames.Length > 0)
                        {
                            foreach (string rNo in studentNames)
                            {
                                if (rNo == txtRollNo.Text)
                                {
                                    _ = MessageBox.Show("You have already Registered.", "Tracking", MessageBoxButtons.OK,
                      MessageBoxIcon.Information);
                                    return;
                                }
                            }
                        }
                    }
                }

                BtnCapture.Enabled = true;
                grabber = new Capture();
                grabber.QueryFrame();
                Application.Idle += new EventHandler(FrameGrabber);
                btnBrowse.Enabled = false;
            }
            else
            {
                _ = MessageBox.Show("You have already Registered.", "Tracking", MessageBoxButtons.OK,
                         MessageBoxIcon.Information);
            }
        }
        private void frmEnrollment_Load(object sender, EventArgs e)
        {
            Core.ClearTextBox(txtAddress, txtContact, txtFirstName, txtLastName, txtRollNo);
            picBox.Image = null;
            btnSubmit.Enabled = false;
            BtnCapture.Enabled = false;
            cboxGender.SelectedIndex = 0;
            dtDob.Value = DateTime.Now;
            cmbDepartment.Items.Insert(0, "Select");
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

        private void frmEnrollment_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (grabber != null){
                grabber.Dispose();
                grabber = null;
            }
        }

        private void txtContact_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            Core.ClearTextBox(txtAddress, txtContact, txtFirstName, txtLastName, txtRollNo);
            picBox.Image = null;
            cboxGender.SelectedIndex = 0;
            cmbDepartment.SelectedIndex = 0;
            dtDob.Value = DateTime.Now;
            imageBoxFrameGrabber.Image = null;
            imageBox1.Image = null;
            btnSubmit.Enabled = false;
            BtnCapture.Enabled = false;
            btnBrowse.Enabled = true;
        }
        private void BtnCapture_Click(object sender, EventArgs e)
        {
            try
            {
                btnSubmit.Enabled = true;
                ContTrain = ContTrain + 1;
                gray = grabber.QueryGrayFrame().Resize(320, 240, Emgu.CV.CvEnum.INTER.CV_INTER_CUBIC);
                MCvAvgComp[][] facesDetected = gray.DetectHaarCascade(
                face,
                1.2,
                10,
                Emgu.CV.CvEnum.HAAR_DETECTION_TYPE.DO_CANNY_PRUNING,
                new Size(20, 20));
                foreach (MCvAvgComp f in facesDetected[0])
                {
                    TrainedFace = currentFrame.Copy(f.rect).Convert<Gray, byte>();
                    break;
                }
                TrainedFace = result.Resize(100, 100, Emgu.CV.CvEnum.INTER.CV_INTER_CUBIC);
                trainingImages.Add(TrainedFace);
                labels.Add(txtRollNo.Text);

                picBox.Image = TrainedFace.ToBitmap();
                imageBox1.Image = TrainedFace;

                File.WriteAllText(Application.StartupPath + "/TrainedFaces/TrainedLabels.txt", trainingImages.ToArray().Length.ToString() + "%");
                for (int i = 1; i < trainingImages.ToArray().Length + 1; i++)
                {
                    trainingImages.ToArray()[i - 1].Save(Application.StartupPath + "/TrainedFaces/face" + i + ".bmp");
                    File.AppendAllText(Application.StartupPath + "/TrainedFaces/TrainedLabels.txt", labels.ToArray()[i - 1] + "%");
                }
                grabber.Dispose();
                grabber = null;
            }
            catch
            {
                grabber.Dispose();
                grabber = null;
                MessageBox.Show("Enable the face detection first", "Training Fail", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
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
                btnBrowse.Focus();
                return false;
            }
            _photoByte = ImageToByte(picBox.Image);
            return true;
        }
        void FrameGrabber(object sender, EventArgs e)
        {
            if (grabber != null)
            {
                NamePersons.Add("");
                currentFrame = grabber.QueryFrame().Resize(320, 240, Emgu.CV.CvEnum.INTER.CV_INTER_CUBIC);
                gray = currentFrame.Convert<Gray, Byte>();
                MCvAvgComp[][] facesDetected = gray.DetectHaarCascade(
              face,
              1.2,
              10,
              Emgu.CV.CvEnum.HAAR_DETECTION_TYPE.DO_CANNY_PRUNING,
              new Size(20, 20));
                foreach (MCvAvgComp f in facesDetected[0])
                {
                    t = t + 1;
                    result = currentFrame.Copy(f.rect).Convert<Gray, byte>().Resize(100, 100, Emgu.CV.CvEnum.INTER.CV_INTER_CUBIC);
                    currentFrame.Draw(f.rect, new Bgr(Color.Red), 2);
                    if (trainingImages.ToArray().Length != 0)
                    {
                        MCvTermCriteria termCrit = new MCvTermCriteria(ContTrain, 0.001);
                        EigenObjectRecognizer recognizer = new EigenObjectRecognizer(
                           trainingImages.ToArray(),
                           labels.ToArray(),
                           3000,
                           ref termCrit);
                        name = recognizer.Recognize(result);
                        name = String.Empty;
                        currentFrame.Draw(name, ref font, new Point(f.rect.X - 2, f.rect.Y - 2), new Bgr(Color.LightGreen));
                    }
                    NamePersons[t - 1] = name;
                    NamePersons.Add("");
                }
                t = 0;
                for (int nnn = 0; nnn < facesDetected[0].Length; nnn++)
                {
                    names = names + NamePersons[nnn] + ", ";
                }
                imageBoxFrameGrabber.Image = currentFrame;
                names = "";
                NamePersons.Clear();
            }
        }
    }
}