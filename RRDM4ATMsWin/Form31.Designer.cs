namespace RRDM4ATMsWin
{
    partial class Form31
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
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea2 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend2 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label12 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.buttonShowAll = new System.Windows.Forms.Button();
            this.buttonShow = new System.Windows.Forms.Button();
            this.textBoxAtmNo = new System.Windows.Forms.TextBox();
            this.labelAtmNo = new System.Windows.Forms.Label();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.chart1 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.chart2 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.label14 = new System.Windows.Forms.Label();
            this.buttonFinish = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chart2)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.panel1.Controls.Add(this.label12);
            this.panel1.Controls.Add(this.panel2);
            this.panel1.Location = new System.Drawing.Point(3, 82);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1003, 613);
            this.panel1.TabIndex = 0;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.label12.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(119)))), ((int)(((byte)(192)))));
            this.label12.Location = new System.Drawing.Point(39, 30);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(294, 18);
            this.label12.TabIndex = 14;
            this.label12.Text = "TRANS FOR CIT:  No ACCOUNT : No";
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.White;
            this.panel2.Controls.Add(this.buttonShowAll);
            this.panel2.Controls.Add(this.buttonShow);
            this.panel2.Controls.Add(this.textBoxAtmNo);
            this.panel2.Controls.Add(this.labelAtmNo);
            this.panel2.Controls.Add(this.textBox2);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Controls.Add(this.textBox1);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Controls.Add(this.chart1);
            this.panel2.Controls.Add(this.dataGridView1);
            this.panel2.Controls.Add(this.chart2);
            this.panel2.ForeColor = System.Drawing.Color.Black;
            this.panel2.Location = new System.Drawing.Point(42, 60);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(937, 542);
            this.panel2.TabIndex = 1;
            // 
            // buttonShowAll
            // 
            this.buttonShowAll.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(119)))), ((int)(((byte)(192)))));
            this.buttonShowAll.FlatAppearance.BorderSize = 2;
            this.buttonShowAll.FlatAppearance.MouseDownBackColor = System.Drawing.Color.White;
            this.buttonShowAll.FlatAppearance.MouseOverBackColor = System.Drawing.Color.LightSteelBlue;
            this.buttonShowAll.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonShowAll.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(119)))), ((int)(((byte)(192)))));
            this.buttonShowAll.Location = new System.Drawing.Point(828, 511);
            this.buttonShowAll.Name = "buttonShowAll";
            this.buttonShowAll.Size = new System.Drawing.Size(94, 27);
            this.buttonShowAll.TabIndex = 10;
            this.buttonShowAll.Text = "Show All";
            this.buttonShowAll.UseVisualStyleBackColor = true;
            this.buttonShowAll.Click += new System.EventHandler(this.button1_Click);
            // 
            // buttonShow
            // 
            this.buttonShow.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(119)))), ((int)(((byte)(192)))));
            this.buttonShow.FlatAppearance.BorderSize = 2;
            this.buttonShow.FlatAppearance.MouseDownBackColor = System.Drawing.Color.White;
            this.buttonShow.FlatAppearance.MouseOverBackColor = System.Drawing.Color.LightSteelBlue;
            this.buttonShow.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonShow.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(119)))), ((int)(((byte)(192)))));
            this.buttonShow.Location = new System.Drawing.Point(848, 478);
            this.buttonShow.Name = "buttonShow";
            this.buttonShow.Size = new System.Drawing.Size(74, 27);
            this.buttonShow.TabIndex = 9;
            this.buttonShow.Text = "Show";
            this.buttonShow.UseVisualStyleBackColor = true;
            this.buttonShow.Click += new System.EventHandler(this.button2_Click);
            // 
            // textBoxAtmNo
            // 
            this.textBoxAtmNo.Location = new System.Drawing.Point(735, 480);
            this.textBoxAtmNo.Name = "textBoxAtmNo";
            this.textBoxAtmNo.Size = new System.Drawing.Size(100, 20);
            this.textBoxAtmNo.TabIndex = 8;
            // 
            // labelAtmNo
            // 
            this.labelAtmNo.AutoSize = true;
            this.labelAtmNo.Location = new System.Drawing.Point(660, 485);
            this.labelAtmNo.Name = "labelAtmNo";
            this.labelAtmNo.Size = new System.Drawing.Size(47, 13);
            this.labelAtmNo.TabIndex = 7;
            this.labelAtmNo.Text = "ATM No";
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(823, 391);
            this.textBox2.Name = "textBox2";
            this.textBox2.ReadOnly = true;
            this.textBox2.Size = new System.Drawing.Size(100, 20);
            this.textBox2.TabIndex = 5;
            this.textBox2.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(741, 396);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(78, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Total Credits(+)";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(823, 365);
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.Size = new System.Drawing.Size(100, 20);
            this.textBox1.TabIndex = 3;
            this.textBox1.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(741, 368);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(73, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Total Debits(-)";
            // 
            // chart1
            // 
            chartArea1.Name = "ChartArea1";
            this.chart1.ChartAreas.Add(chartArea1);
            legend1.Name = "Legend1";
            this.chart1.Legends.Add(legend1);
            this.chart1.Location = new System.Drawing.Point(18, 356);
            this.chart1.Name = "chart1";
            series1.ChartArea = "ChartArea1";
            series1.Legend = "Legend1";
            series1.Name = "Series1";
            this.chart1.Series.Add(series1);
            this.chart1.Size = new System.Drawing.Size(417, 150);
            this.chart1.TabIndex = 1;
            this.chart1.Text = "chart1";
            this.chart1.Click += new System.EventHandler(this.chart1_Click);
            this.chart1.MouseEnter += new System.EventHandler(this.chart1_MouseEnter);
            this.chart1.MouseLeave += new System.EventHandler(this.chart1_MouseLeave);
            // 
            // dataGridView1
            // 
            this.dataGridView1.BackgroundColor = System.Drawing.Color.White;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.GridColor = System.Drawing.Color.LightGray;
            this.dataGridView1.Location = new System.Drawing.Point(15, 12);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView1.Size = new System.Drawing.Size(910, 338);
            this.dataGridView1.TabIndex = 0;
            // 
            // chart2
            // 
            this.chart2.BorderlineColor = System.Drawing.Color.Black;
            this.chart2.BorderlineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Solid;
            chartArea2.Name = "ChartArea1";
            this.chart2.ChartAreas.Add(chartArea2);
            legend2.Name = "Legend1";
            this.chart2.Legends.Add(legend2);
            this.chart2.Location = new System.Drawing.Point(102, 12);
            this.chart2.Name = "chart2";
            series2.ChartArea = "ChartArea1";
            series2.Legend = "Legend1";
            series2.Name = "Series1";
            this.chart2.Series.Add(series2);
            this.chart2.Size = new System.Drawing.Size(770, 338);
            this.chart2.TabIndex = 6;
            this.chart2.Text = "chart2";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(119)))), ((int)(((byte)(192)))));
            this.label14.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Bold);
            this.label14.ForeColor = System.Drawing.Color.White;
            this.label14.Location = new System.Drawing.Point(27, 18);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(320, 29);
            this.label14.TabIndex = 337;
            this.label14.Text = "Statement of transactions  ";
            // 
            // buttonFinish
            // 
            this.buttonFinish.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.buttonFinish.FlatAppearance.BorderSize = 2;
            this.buttonFinish.FlatAppearance.MouseDownBackColor = System.Drawing.Color.White;
            this.buttonFinish.FlatAppearance.MouseOverBackColor = System.Drawing.Color.LightSteelBlue;
            this.buttonFinish.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonFinish.ForeColor = System.Drawing.Color.White;
            this.buttonFinish.Location = new System.Drawing.Point(911, 698);
            this.buttonFinish.Margin = new System.Windows.Forms.Padding(2, 2, 10, 2);
            this.buttonFinish.Name = "buttonFinish";
            this.buttonFinish.Size = new System.Drawing.Size(71, 27);
            this.buttonFinish.TabIndex = 338;
            this.buttonFinish.Text = "Finish";
            this.buttonFinish.UseVisualStyleBackColor = true;
            this.buttonFinish.Click += new System.EventHandler(this.buttonFinish_Click);
            // 
            // Form31
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(119)))), ((int)(((byte)(192)))));
            this.ClientSize = new System.Drawing.Size(1008, 730);
            this.Controls.Add(this.buttonFinish);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.panel1);
            this.Name = "Form31";
            this.Text = "Form31";
            this.Load += new System.EventHandler(this.Form31_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chart2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.DataVisualization.Charting.Chart chart1;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataVisualization.Charting.Chart chart2;
        private System.Windows.Forms.TextBox textBoxAtmNo;
        private System.Windows.Forms.Label labelAtmNo;
        private System.Windows.Forms.Button buttonShow;
        private System.Windows.Forms.Button buttonShowAll;
        private System.Windows.Forms.Button buttonFinish;
    }
}