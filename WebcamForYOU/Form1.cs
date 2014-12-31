using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AForge.Video.DirectShow;
using AForge.Video;
using System.Threading;

namespace WebcamForYOU
{
    public partial class Form1 : Form
    {
        public FilterInfoCollection CamsCollection;
        public VideoCaptureDevice Cam = null;
        int cameraSelected = 0;
        int selectedIndex = 0;
        int mirror = 0;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            CamsCollection = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            comboBox1.Items.Clear();
            mypic.SizeMode = PictureBoxSizeMode.StretchImage;

            foreach (FilterInfo fi in CamsCollection)
            {
                comboBox1.Items.Add(fi.Name);
            }

            this.KeyDown += Form1_KeyDown;
            this.MaximizeBox = false;
            comboBox1.SelectedIndexChanged += comboBox1_SelectedIndexChanged;
            
        }

        /// <summary>
        /// This method is called each time a new value is selected from the 
        /// combobox. It selects the new camera selected by the user.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedIndex = comboBox1.SelectedIndex;
            CameraStart();
        }

        void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                ImgSave();
            }
        }

        #region Camera Start code
        /// <summary>
        /// This method Starts the camera.
        /// </summary>
        void CameraStart()
        {
            try
            {
                Cam = new VideoCaptureDevice(CamsCollection[selectedIndex].MonikerString);
                Cam.NewFrame += Cam_NewFrame;
                Cam.Start();
                cameraSelected = 1;

            }
            catch (Exception ex)
            {
                MessageBox.Show("No video input device found\n\nThe application will now EXIT... :(");
                Thread.Sleep(200);
                Application.Exit();
            }
        }
        #endregion

        void Cam_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            Bitmap img = (Bitmap)eventArgs.Frame.Clone();
            if (mirror == 0)
            {
                mypic.Image = img;
            }
            else
            {
                img.RotateFlip(RotateFlipType.RotateNoneFlipX);
                mypic.Image = img;
            }
        }

        private void ImgSave()
        {
            try
            {
                MainSaveDialog.Filter = "(*.jpg)|*.jpg|AllFiles(*.*)|*.*";
                if (MainSaveDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    mypic.Image.Save(MainSaveDialog.FileName);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Please select a valid camera first..!!");
            }
        }

        private void clickButton_Click(object sender, EventArgs e)
        {
            if (cameraSelected == 1)
            {
                ImgSave();
            }
        }

        private void ExitButton_Click(object sender, EventArgs e)
        {
            try
            {
                Cam.SignalToStop();
                Cam.WaitForStop();
                Cam.Stop();
            }
            catch(NullReferenceException ex)
            {
                
            }
            finally
            {
                MessageBox.Show("SEE YOU NEXT TIME..!! :)");
                Application.Exit();
            }  
        }

        private void mirrorCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (mirrorCheckBox.Checked == true)
            {
                mirror = 1;
            }
            else
            {
                mirror = 0;
            }
        }
    }
}
