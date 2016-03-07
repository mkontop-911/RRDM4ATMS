namespace RRDM4ATMsWin
{
    partial class Form350_Route
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
            this.labelRouteDescription = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // labelRouteDescription
            // 
            this.labelRouteDescription.AutoSize = true;
            this.labelRouteDescription.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.labelRouteDescription.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(119)))), ((int)(((byte)(192)))));
            this.labelRouteDescription.Location = new System.Drawing.Point(26, 29);
            this.labelRouteDescription.Name = "labelRouteDescription";
            this.labelRouteDescription.Size = new System.Drawing.Size(164, 22);
            this.labelRouteDescription.TabIndex = 268;
            this.labelRouteDescription.Text = "RouteDescription";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::RRDM4ATMsWin.Properties.Resources.BancNet_Route;
            this.pictureBox1.Location = new System.Drawing.Point(29, 72);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(916, 650);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // Form350_Route
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(913, 732);
            this.Controls.Add(this.labelRouteDescription);
            this.Controls.Add(this.pictureBox1);
            this.Name = "Form350_Route";
            this.Text = "Form350_Route";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label labelRouteDescription;
    }
}