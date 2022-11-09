using AForge.Video;
using AForge.Video.DirectShow;
using Emgu.CV.Structure;
using Emgu.CV;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;

namespace FaceTracking
{
    public partial class frmSample : Form
    {
       
        public frmSample()
        {
            InitializeComponent();
        }
        private Image<Bgr, Byte> myImage;
        private void GetImage()
        {
            string sFile;
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "Image File (*.jpg;*.bmp;*.gif,*.JPEG)|*.jpg;*.bmp;*.gif,*.JPEG";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                sFile = openFileDialog1.FileName;
                myImage = new Image<Bgr, byte>(sFile);
                picBox.Image = myImage.ToBitmap();
                picBox.SizeMode = PictureBoxSizeMode.StretchImage;
            } 
        }

        private void button4_Click(object sender, System.EventArgs e)
        {
            GetImage();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var gray = myImage.Convert<Gray, Byte>().Convert<Gray, double>();
            picBox.Image = gray.ToBitmap();
        }
    }
}