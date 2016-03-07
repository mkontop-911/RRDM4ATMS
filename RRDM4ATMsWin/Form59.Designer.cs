namespace RRDM4ATMsWin
{
    partial class VideoWindow
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(VideoWindow));
            this.panel1 = new System.Windows.Forms.Panel();
            this.axWindowsMediaPlayer = new AxWMPLib.AxWindowsMediaPlayer();
            this.tbFileName = new System.Windows.Forms.TextBox();
            this.tbPosition = new System.Windows.Forms.TextBox();
            this.lblFileName = new System.Windows.Forms.Label();
            this.lblPosition = new System.Windows.Forms.Label();
            this.btnStartVideo = new System.Windows.Forms.Button();
            this.btnStopVideo = new System.Windows.Forms.Button();
            this.btnSelectFile = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.axWindowsMediaPlayer)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.Controls.Add(this.btnSelectFile);
            this.panel1.Controls.Add(this.btnStopVideo);
            this.panel1.Controls.Add(this.btnStartVideo);
            this.panel1.Controls.Add(this.lblPosition);
            this.panel1.Controls.Add(this.lblFileName);
            this.panel1.Controls.Add(this.tbPosition);
            this.panel1.Controls.Add(this.tbFileName);
            this.panel1.Controls.Add(this.axWindowsMediaPlayer);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(2);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(700, 651);
            this.panel1.TabIndex = 2;
            // 
            // axWindowsMediaPlayer
            // 
            this.axWindowsMediaPlayer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.axWindowsMediaPlayer.Enabled = true;
            this.axWindowsMediaPlayer.Location = new System.Drawing.Point(12, 12);
            this.axWindowsMediaPlayer.Name = "axWindowsMediaPlayer";
            this.axWindowsMediaPlayer.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axWindowsMediaPlayer.OcxState")));
            this.axWindowsMediaPlayer.Size = new System.Drawing.Size(676, 465);
            this.axWindowsMediaPlayer.TabIndex = 0;
            // 
            // tbFileName
            // 
            this.tbFileName.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbFileName.Location = new System.Drawing.Point(22, 513);
            this.tbFileName.Name = "tbFileName";
            this.tbFileName.Size = new System.Drawing.Size(411, 20);
            this.tbFileName.TabIndex = 1;
            this.tbFileName.TextChanged += new System.EventHandler(this.tbFileName_TextChanged);
            // 
            // tbPosition
            // 
            this.tbPosition.Location = new System.Drawing.Point(476, 513);
            this.tbPosition.Name = "tbPosition";
            this.tbPosition.Size = new System.Drawing.Size(115, 20);
            this.tbPosition.TabIndex = 2;
            // 
            // lblFileName
            // 
            this.lblFileName.AutoSize = true;
            this.lblFileName.Location = new System.Drawing.Point(19, 497);
            this.lblFileName.Name = "lblFileName";
            this.lblFileName.Size = new System.Drawing.Size(116, 13);
            this.lblFileName.TabIndex = 6;
            this.lblFileName.Text = "Video File being played";
            // 
            // lblPosition
            // 
            this.lblPosition.AutoSize = true;
            this.lblPosition.Location = new System.Drawing.Point(503, 496);
            this.lblPosition.Name = "lblPosition";
            this.lblPosition.Size = new System.Drawing.Size(118, 13);
            this.lblPosition.TabIndex = 8;
            this.lblPosition.Text = "Start Position (seconds)";
            // 
            // btnStartVideo
            // 
            this.btnStartVideo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnStartVideo.Location = new System.Drawing.Point(22, 574);
            this.btnStartVideo.Name = "btnStartVideo";
            this.btnStartVideo.Size = new System.Drawing.Size(131, 43);
            this.btnStartVideo.TabIndex = 9;
            this.btnStartVideo.Text = "Start Video";
            this.btnStartVideo.UseVisualStyleBackColor = true;
            this.btnStartVideo.Click += new System.EventHandler(this.btnStartVideo_Click);
            // 
            // btnStopVideo
            // 
            this.btnStopVideo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnStopVideo.Location = new System.Drawing.Point(197, 574);
            this.btnStopVideo.Name = "btnStopVideo";
            this.btnStopVideo.Size = new System.Drawing.Size(131, 43);
            this.btnStopVideo.TabIndex = 10;
            this.btnStopVideo.Text = "Stop Video";
            this.btnStopVideo.UseVisualStyleBackColor = true;
            this.btnStopVideo.Click += new System.EventHandler(this.btnStopVideo_Click);
            // 
            // btnSelectFile
            // 
            this.btnSelectFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnSelectFile.Location = new System.Drawing.Point(476, 574);
            this.btnSelectFile.Name = "btnSelectFile";
            this.btnSelectFile.Size = new System.Drawing.Size(131, 43);
            this.btnSelectFile.TabIndex = 11;
            this.btnSelectFile.Text = "Select File";
            this.btnSelectFile.UseVisualStyleBackColor = true;
            this.btnSelectFile.Click += new System.EventHandler(this.btnSelectFile_Click);
            // 
            // VideoWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(700, 651);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "VideoWindow";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "ATM Video";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.VideoWindow_FormClosing);
            this.Load += new System.EventHandler(this.CustomMessageBox_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.axWindowsMediaPlayer)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox tbFileName;
        private AxWMPLib.AxWindowsMediaPlayer axWindowsMediaPlayer;
        private System.Windows.Forms.Button btnSelectFile;
        private System.Windows.Forms.Button btnStopVideo;
        private System.Windows.Forms.Button btnStartVideo;
        private System.Windows.Forms.Label lblPosition;
        private System.Windows.Forms.Label lblFileName;
        private System.Windows.Forms.TextBox tbPosition;
    }
}