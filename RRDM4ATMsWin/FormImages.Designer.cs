namespace TestImages
{
    partial class FormImages
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
            this.textBoxImageID = new System.Windows.Forms.TextBox();
            this.textBoxImageDescr = new System.Windows.Forms.TextBox();
            this.labelImageID = new System.Windows.Forms.Label();
            this.labelImageDesr = new System.Windows.Forms.Label();
            this.pictureBox = new System.Windows.Forms.PictureBox();
            this.buttonShow = new System.Windows.Forms.Button();
            this.buttonBrowse = new System.Windows.Forms.Button();
            this.buttonSave = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // textBoxImageID
            // 
            this.textBoxImageID.Location = new System.Drawing.Point(120, 39);
            this.textBoxImageID.Name = "textBoxImageID";
            this.textBoxImageID.Size = new System.Drawing.Size(138, 20);
            this.textBoxImageID.TabIndex = 0;
            // 
            // textBoxImageDescr
            // 
            this.textBoxImageDescr.Location = new System.Drawing.Point(120, 104);
            this.textBoxImageDescr.Name = "textBoxImageDescr";
            this.textBoxImageDescr.Size = new System.Drawing.Size(138, 20);
            this.textBoxImageDescr.TabIndex = 1;
            // 
            // labelImageID
            // 
            this.labelImageID.AutoSize = true;
            this.labelImageID.Location = new System.Drawing.Point(31, 42);
            this.labelImageID.Name = "labelImageID";
            this.labelImageID.Size = new System.Drawing.Size(50, 13);
            this.labelImageID.TabIndex = 2;
            this.labelImageID.Text = "Image ID";
            this.labelImageID.Click += new System.EventHandler(this.label1_Click);
            // 
            // labelImageDesr
            // 
            this.labelImageDesr.AutoSize = true;
            this.labelImageDesr.Location = new System.Drawing.Point(34, 107);
            this.labelImageDesr.Name = "labelImageDesr";
            this.labelImageDesr.Size = new System.Drawing.Size(60, 13);
            this.labelImageDesr.TabIndex = 3;
            this.labelImageDesr.Text = "Description";
            this.labelImageDesr.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // pictureBox
            // 
            this.pictureBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox.Location = new System.Drawing.Point(368, 30);
            this.pictureBox.Name = "pictureBox";
            this.pictureBox.Size = new System.Drawing.Size(324, 236);
            this.pictureBox.TabIndex = 4;
            this.pictureBox.TabStop = false;
            this.pictureBox.Click += new System.EventHandler(this.pictureBox_Click);
            // 
            // buttonShow
            // 
            this.buttonShow.Location = new System.Drawing.Point(146, 155);
            this.buttonShow.Name = "buttonShow";
            this.buttonShow.Size = new System.Drawing.Size(75, 23);
            this.buttonShow.TabIndex = 5;
            this.buttonShow.Text = "Show";
            this.buttonShow.UseVisualStyleBackColor = true;
            this.buttonShow.Click += new System.EventHandler(this.buttonShow_Click);
            // 
            // buttonBrowse
            // 
            this.buttonBrowse.Location = new System.Drawing.Point(368, 282);
            this.buttonBrowse.Name = "buttonBrowse";
            this.buttonBrowse.Size = new System.Drawing.Size(75, 23);
            this.buttonBrowse.TabIndex = 6;
            this.buttonBrowse.Text = "Browse";
            this.buttonBrowse.UseVisualStyleBackColor = true;
            this.buttonBrowse.Click += new System.EventHandler(this.buttonBrowse_Click);
            // 
            // buttonSave
            // 
            this.buttonSave.Location = new System.Drawing.Point(617, 282);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(75, 23);
            this.buttonSave.TabIndex = 7;
            this.buttonSave.Text = "Save";
            this.buttonSave.UseVisualStyleBackColor = true;
            this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
            // 
            // FormImages
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(789, 366);
            this.Controls.Add(this.buttonSave);
            this.Controls.Add(this.buttonBrowse);
            this.Controls.Add(this.buttonShow);
            this.Controls.Add(this.pictureBox);
            this.Controls.Add(this.labelImageDesr);
            this.Controls.Add(this.labelImageID);
            this.Controls.Add(this.textBoxImageDescr);
            this.Controls.Add(this.textBoxImageID);
            this.Name = "FormImages";
            this.Text = "FormImages";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxImageID;
        private System.Windows.Forms.TextBox textBoxImageDescr;
        private System.Windows.Forms.Label labelImageID;
        private System.Windows.Forms.Label labelImageDesr;
        private System.Windows.Forms.PictureBox pictureBox;
        private System.Windows.Forms.Button buttonShow;
        private System.Windows.Forms.Button buttonBrowse;
        private System.Windows.Forms.Button buttonSave;
    }
}

