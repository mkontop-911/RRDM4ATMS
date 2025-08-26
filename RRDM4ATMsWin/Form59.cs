using System;
using System.Windows.Forms;
using AxWMPLib;


// using Microsoft.DirectX;
// using Microsoft.DirectX.AudioVideoPlayback;

namespace RRDM4ATMsWin
{
    public partial class VideoWindow : Form
    {
        // Video video;

        public VideoWindow()
        {
            InitializeComponent();
            tbFileName.Text = "C:\\ATMVideo\\Test.avi";
            tbPosition.Text = "3";
        }


        private void VideoWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (axWindowsMediaPlayer.status != "1")
            {
                axWindowsMediaPlayer.Ctlcontrols.stop();
            }
        }

        private void CustomMessageBox_Load(object sender, EventArgs e)
        {

        }

        private void btnSelectFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.ShowDialog();
            tbFileName.Text = ofd.FileName;
        }

        private void btnStartVideo_Click(object sender, EventArgs e)
        {
            double iPos;

            axWindowsMediaPlayer.URL = tbFileName.Text;
            try
            {
                iPos = Convert.ToDouble(tbPosition.Text); // TODO: validate input...
            }
            catch
            {
                iPos = 0;
                tbPosition.Text = "0";
            }

            axWindowsMediaPlayer.Ctlcontrols.currentPosition = iPos;
            // axWindowsMediaPlayer.Height = axWindowsMediaPlayer.ClientSize.Height;
            axWindowsMediaPlayer.Ctlcontrols.play();
        }

        private void btnStopVideo_Click(object sender, EventArgs e)
        {
            axWindowsMediaPlayer.Ctlcontrols.stop();
        }


        private void tbFileName_TextChanged(object sender, EventArgs e)
        {
            axWindowsMediaPlayer.Ctlcontrols.stop();
        }

        private void axWMP_PlayStateChange(object sender, _WMPOCXEvents_PlayStateChangeEvent e)
        {
            /*
             * 
                0 = Undefined
                1 = Stopped (by User)
                2 = Paused
                3 = Playing
                4 = Scan Forward
                5 = Scan Backwards
                6 = Buffering
                7 = Waiting
                8 = Media Ended
                9 = Transitioning
                10 = Ready
                11 = Reconnecting
                12 = Last
             * 
             */
            if (e.newState == 8)
            {
                //Your Code Here
            }
        }

        //public VideoWindow()
        //{
        //    InitializeComponent();
            
            //video = new Video(@"C:\ATMVideo\Test.avi");
            //int height = panelVideo.Height;
            //int width= panelVideo.Width;
            
            //video.Owner = panelVideo;
            //video.Size = new Size(width, height);
            //panelVideo.Size = new Size(width, height);
        }



        //private void button1_Click(object sender, EventArgs e)
        //{
        //    this.Dispose();
        //}

        //private void button2_Click(object sender, EventArgs e)
        //{
           
        //    if (video.State != StateFlags.Running)
        //    {
        //        video.Play();
        //    }
        //}

        //private void button3_Click(object sender, EventArgs e)
        //{
        //    if (video.State != StateFlags.Stopped)
        //    {
        //        video.Stop();
        //    }
        //}
}


