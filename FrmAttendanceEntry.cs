using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace FaceTracking
{
    public partial class FrmAttendanceEntry : Form
    {
        #region Face Detect
        Image<Bgr, Byte> currentFrame;
        Capture grabber;
        HaarCascade face;
        MCvFont font = new MCvFont(FONT.CV_FONT_HERSHEY_TRIPLEX, 0.5d, 0.5d);
        Image<Gray, byte> result = null;
        Image<Gray, byte> gray = null;
        List<Image<Gray, byte>> trainingImages = new List<Image<Gray, byte>>();
        List<string> labels = new List<string>();
        List<string> NamePersons = new List<string>();
        int ContTrain, NumLabels, t;
        string name, names = null;
        #endregion

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            grabber = new Capture();
            grabber.QueryFrame();
            Application.Idle += new EventHandler(FrameGrabber);
            btnBrowse.Enabled = false;
        }

        private void FrmAttendanceEntry_Load(object sender, EventArgs e)
        {

        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            grabber.Dispose();
            bool check = new DbConcept().CheckAttendance(txtRollNo.Text);
            if (!check)
            {
                DbConcept db = new DbConcept();
                int iResult = db.NewAttendance(txtRollNo.Text,true, _userId);
                if (iResult > 0)
                {
                    MessageBox.Show("Attendance Successfully Saved.", "Tracking", MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Attendance Not Saved.", "Tracking", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else
            {
                MessageBox.Show("You have already Attendance Registered.", "Tracking", MessageBoxButtons.OK,
                       MessageBoxIcon.Information);
            }
            txtFirstName.Text = String.Empty;
            txtLastName.Text = String.Empty;
            picBox.Image = null;
            imageBoxFrameGrabber.Image = null;
            btnBrowse.Enabled = true;
            txtRollNo.Text = String.Empty;
        }

        private void FrmAttendanceEntry_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (grabber != null)
            {
                grabber.Dispose();
                grabber = null;
            }
        }

        void FrameGrabber(object sender, EventArgs e)
        { 
            NamePersons.Add("");
            currentFrame = grabber?.QueryFrame()?.Resize(320, 240, Emgu.CV.CvEnum.INTER.CV_INTER_CUBIC);
            gray = currentFrame?.Convert<Gray, Byte>(); 
            MCvAvgComp[][] facesDetected = gray?.DetectHaarCascade(
          face,
          1.2,
          10,
          Emgu.CV.CvEnum.HAAR_DETECTION_TYPE.DO_CANNY_PRUNING,
          new Size(20, 20));
            if(facesDetected !=null && facesDetected.Length > 0)
            {
                foreach (MCvAvgComp f in facesDetected[0])
                {
                    t = t + 1;
                    result = currentFrame.Copy(f.rect).Convert<Gray, byte>().Resize(100, 100, Emgu.CV.CvEnum.INTER.CV_INTER_CUBIC);
                    currentFrame.Draw(f.rect, new Bgr(Color.Red), 2);
                    if (trainingImages?.ToArray().Length != 0)
                    {
                        MCvTermCriteria termCrit = new MCvTermCriteria(ContTrain, 0.001);
                        EigenObjectRecognizer recognizer = new EigenObjectRecognizer(
                           trainingImages?.ToArray(),
                           labels.ToArray(),
                           3000,
                           ref termCrit);

                        name = recognizer?.Recognize(result);
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
                if (!string.IsNullOrEmpty(names))
                {
                    txtRollNo.Text = names.Replace(",", "").Trim();
                    StudetnDto student = new DbConcept().FindStudent(txtRollNo.Text);
                    if (student != null)
                    {
                        txtFirstName.Text = student.FirstName;
                        txtLastName.Text = student.LastName;
                        if(student.Photo != null)
                        {
                            MemoryStream ms = new MemoryStream(student.Photo);
                            picBox.Image = new Bitmap(ms);
                        }
                    }
                    else
                    {
                        txtFirstName.Text = String.Empty;
                        txtLastName.Text = String.Empty;
                        picBox.Image = null;
                    }
                }
                else
                {
                    txtRollNo.Text = "";
                    txtFirstName.Text = String.Empty;
                    txtLastName.Text = String.Empty;
                    picBox.Image = null;
                }
                names = "";
                NamePersons.Clear();
            }
        }
        private readonly int _userId;
        public FrmAttendanceEntry(int userId)
        {
            _userId = userId;
            InitializeComponent();
             face = new HaarCascade("haarcascade_frontalface_default.xml");
            try
            {
                string Labelsinfo = File.ReadAllText(Application.StartupPath + "/TrainedFaces/TrainedLabels.txt");
                string[] Labels = Labelsinfo.Split('%');
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
            catch (Exception e)
            {
                MessageBox.Show("Nothing in binary database, please add at least a face(Simply train the prototype with the Add Face Button).", "Triained faces load", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }
    }
}
