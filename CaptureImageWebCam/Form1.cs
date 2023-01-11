using DirectShowLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CaptureImageWebCam
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private string _path = @"./system";
        private OpenCvSharp.VideoCapture capture;
        private bool IsCapture;
        private Image Image = null;

        private bool statebtnCaptue = false;

        private void Form1_Load(object sender, EventArgs e)
        {
            // Get drive web cam to list
            var videoDevices = new List<DsDevice>(DsDevice.GetDevicesOfCat(FilterCategory.VideoInputDevice));

            foreach (var device in videoDevices)
            {
                comboBox1.Items.Add(device.Name);
            }
            if (comboBox1.Items.Count > 0)
            {
                comboBox1.SelectedIndex = 0;
            }


            // Chack path
            if (!Directory.Exists(_path))
            {
                Directory.CreateDirectory(_path);
            }
        }

        private void btnConn_Click(object sender, EventArgs e)
        {
            int deviceIndex = comboBox1.SelectedIndex;
            capture = new OpenCvSharp.VideoCapture(deviceIndex);
            capture.Open(deviceIndex);
            SetSizeImage();

            timerVideo.Start();
        }
        private void SetSizeImage(string name = "HD")
        {
            int _height = 1080;
            int _width = 1920;
            switch (name.ToUpper())
            {
                case "HD":
                    _height = 720;
                    _width = 1280;
                    break;
                case "FULLHD":
                    _height = 1080;
                    _width = 1920;
                    break;
            }

            capture.Set(OpenCvSharp.VideoCaptureProperties.FrameHeight, _height);
            capture.Set(OpenCvSharp.VideoCaptureProperties.FrameWidth, _width);
        }
        private void timerVideo_Tick(object sender, EventArgs e)
        {
            if (capture.IsOpened() && capture != null)
            {
                try
                {
                    using (OpenCvSharp.Mat frame = new OpenCvSharp.Mat())
                    {
                        capture.Read(frame);
                        if (!frame.Empty())
                        {
                            pictureBox1.Image = OpenCvSharp.Extensions.BitmapConverter.ToBitmap(frame);
                            if (statebtnCaptue)
                            {
                                using (Image image = OpenCvSharp.Extensions.BitmapConverter.ToBitmap(frame))
                                {
                                    saveFileImage(image);
                                }
                            }
                           
                        }
                    }
                }
                catch (Exception)
                {
                    pictureBox1.Image = null;
                }
                finally
                {
                }
            }
        }

        private void btnCapture_Click(object sender, EventArgs e)
        {
            if (!capture.IsOpened() || capture == null)
                return;


            if (rAuto.Checked && statebtnCaptue == false)
            {
                statebtnCaptue = true;
                // Set Text 
                btnCapture.Text = "Run..";

                toolStripStatusLabel1.Text = "Save...";
               
            }
            else
            if (rAuto.Checked && statebtnCaptue)
            {
                statebtnCaptue = false;
                btnCapture.Text = "Capture";
                toolStripStatusLabel1.Text = "Redy";
            }
            else if (rManual.Checked)
            {
                if (pictureBox1.Image != null)
                {
                    saveFileImage(pictureBox1.Image);
                    toolStripStatusLabel1.Text = "Save";
                }
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (capture != null)
            {
                capture.Dispose();
                pictureBox1.Image = null;
            }

            timerVideo.Stop();
        }

        private String saveFileImage(Image image)
        {
            string filename = Guid.NewGuid().ToString();
            string _path = Path.Combine(this._path, "images");
            // Chack path
            if (!Directory.Exists(_path))
            {
                Directory.CreateDirectory(_path);
            }
            _path = Path.Combine(this._path, "images",filename+".jpg");
            image.Save(_path);

            toolStripStatusLabel2.Text = filename + ".jpg";
            return filename + "jpg";
        }
    }
}
