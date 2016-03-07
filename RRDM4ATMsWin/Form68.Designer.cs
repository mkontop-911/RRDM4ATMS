namespace RRDM4ATMsWin
{
    partial class Form68
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
            this.panel2 = new System.Windows.Forms.Panel();
            this.buttonPrintTrace = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.Heading1 = new System.Windows.Forms.Label();
            this.textBoxTraceNo = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.buttonPrintFull = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.AutoScroll = true;
            this.panel1.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.panel1.Controls.Add(this.panel2);
            this.panel1.Location = new System.Drawing.Point(12, 46);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(984, 638);
            this.panel1.TabIndex = 1;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.White;
            this.panel2.Controls.Add(this.buttonPrintFull);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Controls.Add(this.textBoxTraceNo);
            this.panel2.Controls.Add(this.buttonPrintTrace);
            this.panel2.Controls.Add(this.button3);
            this.panel2.Controls.Add(this.dataGridView1);
            this.panel2.Location = new System.Drawing.Point(14, 18);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(967, 608);
            this.panel2.TabIndex = 0;
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
            this.buttonPrintTrace.Location = new System.Drawing.Point(207, 573);
            this.buttonPrintTrace.Name = "buttonPrintTrace";
            this.buttonPrintTrace.Size = new System.Drawing.Size(84, 24);
            this.buttonPrintTrace.TabIndex = 334;
            this.buttonPrintTrace.Text = "Print Trace";
            this.buttonPrintTrace.UseVisualStyleBackColor = false;
            this.buttonPrintTrace.Click += new System.EventHandler(this.buttonPrintTrace_Click);
            // 
            // button3
            // 
            this.button3.BackColor = System.Drawing.Color.Transparent;
            this.button3.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(119)))), ((int)(((byte)(192)))));
            this.button3.FlatAppearance.BorderSize = 2;
            this.button3.FlatAppearance.MouseDownBackColor = System.Drawing.Color.White;
            this.button3.FlatAppearance.MouseOverBackColor = System.Drawing.Color.LightSteelBlue;
            this.button3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(119)))), ((int)(((byte)(192)))));
            this.button3.Location = new System.Drawing.Point(876, 573);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(88, 24);
            this.button3.TabIndex = 333;
            this.button3.Text = "Video Clip";
            this.button3.UseVisualStyleBackColor = false;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AllowUserToOrderColumns = true;
            this.dataGridView1.BackgroundColor = System.Drawing.Color.White;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.GridColor = System.Drawing.Color.LightGray;
            this.dataGridView1.Location = new System.Drawing.Point(6, 3);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView1.Size = new System.Drawing.Size(958, 550);
            this.dataGridView1.TabIndex = 0;
            this.dataGridView1.RowEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_RowEnter);
            // 
            // Heading1
            // 
            this.Heading1.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.Heading1.AutoSize = true;
            this.Heading1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Heading1.ForeColor = System.Drawing.Color.White;
            this.Heading1.Location = new System.Drawing.Point(11, 15);
            this.Heading1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.Heading1.Name = "Heading1";
            this.Heading1.Size = new System.Drawing.Size(181, 24);
            this.Heading1.TabIndex = 243;
            this.Heading1.Text = "E_Journal Drilling ";
            // 
            // textBoxTraceNo
            // 
            this.textBoxTraceNo.Location = new System.Drawing.Point(87, 573);
            this.textBoxTraceNo.Name = "textBoxTraceNo";
            this.textBoxTraceNo.Size = new System.Drawing.Size(100, 20);
            this.textBoxTraceNo.TabIndex = 335;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(119)))), ((int)(((byte)(192)))));
            this.label1.Location = new System.Drawing.Point(7, 576);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(75, 13);
            this.label1.TabIndex = 336;
            this.label1.Text = "Trace Number";
            // 
            // buttonPrintFull
            // 
            this.buttonPrintFull.BackColor = System.Drawing.Color.Transparent;
            this.buttonPrintFull.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(119)))), ((int)(((byte)(192)))));
            this.buttonPrintFull.FlatAppearance.BorderSize = 2;
            this.buttonPrintFull.FlatAppearance.MouseDownBackColor = System.Drawing.Color.White;
            this.buttonPrintFull.FlatAppearance.MouseOverBackColor = System.Drawing.Color.LightSteelBlue;
            this.buttonPrintFull.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonPrintFull.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(119)))), ((int)(((byte)(192)))));
            this.buttonPrintFull.Location = new System.Drawing.Point(340, 573);
            this.buttonPrintFull.Name = "buttonPrintFull";
            this.buttonPrintFull.Size = new System.Drawing.Size(82, 24);
            this.buttonPrintFull.TabIndex = 337;
            this.buttonPrintFull.Text = "Print Full";
            this.buttonPrintFull.UseVisualStyleBackColor = false;
            this.buttonPrintFull.Click += new System.EventHandler(this.buttonPrintFull_Click);
            // 
            // Form68
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(119)))), ((int)(((byte)(192)))));
            this.ClientSize = new System.Drawing.Size(1008, 730);
            this.Controls.Add(this.Heading1);
            this.Controls.Add(this.panel1);
            this.Name = "Form68";
            this.Text = "Form68";
            this.Load += new System.EventHandler(this.Form68_Load);
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Label Heading1;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button buttonPrintTrace;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxTraceNo;
        private System.Windows.Forms.Button buttonPrintFull;
    }
}