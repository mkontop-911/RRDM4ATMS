namespace RRDM4ATMsWin
{
    partial class Form67
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.buttonPrintTrace = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxTraceNo = new System.Windows.Forms.TextBox();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.label14);
            this.panel1.Controls.Add(this.panel2);
            this.panel1.Location = new System.Drawing.Point(12, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(887, 638);
            this.panel1.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(119)))), ((int)(((byte)(192)))));
            this.label1.Location = new System.Drawing.Point(309, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(162, 18);
            this.label1.TabIndex = 330;
            this.label1.Text = "XXXXXXXXXXXXXX";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.label14.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(119)))), ((int)(((byte)(192)))));
            this.label14.Location = new System.Drawing.Point(17, 18);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(286, 18);
            this.label14.TabIndex = 329;
            this.label14.Text = "ATM JOURNAL FOR REPL. CYCLE: ";
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.White;
            this.panel2.Controls.Add(this.buttonPrintTrace);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Controls.Add(this.textBoxTraceNo);
            this.panel2.Controls.Add(this.dataGridView1);
            this.panel2.Location = new System.Drawing.Point(14, 39);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(859, 587);
            this.panel2.TabIndex = 0;
            // 
            // dataGridView1
            // 
            this.dataGridView1.BackgroundColor = System.Drawing.Color.White;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.GridColor = System.Drawing.Color.LightGray;
            this.dataGridView1.Location = new System.Drawing.Point(17, 15);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.Size = new System.Drawing.Size(827, 530);
            this.dataGridView1.TabIndex = 0;
            // 
            // buttonPrintTrace
            // 
            this.buttonPrintTrace.BackColor = System.Drawing.Color.Transparent;
            this.buttonPrintTrace.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(119)))), ((int)(((byte)(192)))));
            this.buttonPrintTrace.FlatAppearance.BorderSize = 2;
            this.buttonPrintTrace.FlatAppearance.MouseDownBackColor = System.Drawing.Color.White;
            this.buttonPrintTrace.FlatAppearance.MouseOverBackColor = System.Drawing.Color.LightSteelBlue;
            this.buttonPrintTrace.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonPrintTrace.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(119)))), ((int)(((byte)(192)))));
            this.buttonPrintTrace.Location = new System.Drawing.Point(222, 556);
            this.buttonPrintTrace.Name = "buttonPrintTrace";
            this.buttonPrintTrace.Size = new System.Drawing.Size(125, 24);
            this.buttonPrintTrace.TabIndex = 342;
            this.buttonPrintTrace.Text = "Print Trace Journal";
            this.buttonPrintTrace.UseVisualStyleBackColor = false;
            this.buttonPrintTrace.Click += new System.EventHandler(this.buttonPrintTrace_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(119)))), ((int)(((byte)(192)))));
            this.label2.Location = new System.Drawing.Point(14, 558);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(75, 13);
            this.label2.TabIndex = 341;
            this.label2.Text = "Trace Number";
            // 
            // textBoxTraceNo
            // 
            this.textBoxTraceNo.Location = new System.Drawing.Point(106, 558);
            this.textBoxTraceNo.Name = "textBoxTraceNo";
            this.textBoxTraceNo.Size = new System.Drawing.Size(100, 20);
            this.textBoxTraceNo.TabIndex = 340;
            // 
            // Form67
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(119)))), ((int)(((byte)(192)))));
            this.ClientSize = new System.Drawing.Size(911, 662);
            this.Controls.Add(this.panel1);
            this.Name = "Form67";
            this.Text = "Form67";
            this.Load += new System.EventHandler(this.Form67_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button buttonPrintTrace;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxTraceNo;
    }
}