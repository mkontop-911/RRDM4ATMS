namespace RRDM4ATMsWin
{
    partial class UCForm276c_AUDI_FirstStep
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UCForm276c_AUDI_FirstStep));
            this.labelAuthHeading = new System.Windows.Forms.Label();
            this.buttonRefresh = new System.Windows.Forms.Button();
            this.buttonAuthor = new System.Windows.Forms.Button();
            this.panelAuthor = new System.Windows.Forms.Panel();
            this.buttonAuthHistory = new System.Windows.Forms.Button();
            this.buttonAuthorise = new System.Windows.Forms.Button();
            this.buttonReject = new System.Windows.Forms.Button();
            this.labelAuthComm = new System.Windows.Forms.Label();
            this.textBoxComment = new System.Windows.Forms.TextBox();
            this.labelAuthStatus = new System.Windows.Forms.Label();
            this.labelAuthoriser = new System.Windows.Forms.Label();
            this.labelDtAuthRequest = new System.Windows.Forms.Label();
            this.labelRequestor = new System.Windows.Forms.Label();
            this.labelNumberNotes2 = new System.Windows.Forms.Label();
            this.buttonNotes2 = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.buttonAllActions = new System.Windows.Forms.Button();
            this.buttonGLTxns = new System.Windows.Forms.Button();
            this.panel7 = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxTotalPairs = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.textBoxTotalAmt = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.textBoxTotalInLines = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.panelAuthor.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel7.SuspendLayout();
            this.SuspendLayout();
            // 
            // labelAuthHeading
            // 
            this.labelAuthHeading.AutoSize = true;
            this.labelAuthHeading.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.labelAuthHeading.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(119)))), ((int)(((byte)(192)))));
            this.labelAuthHeading.Location = new System.Drawing.Point(175, 328);
            this.labelAuthHeading.Name = "labelAuthHeading";
            this.labelAuthHeading.Size = new System.Drawing.Size(193, 18);
            this.labelAuthHeading.TabIndex = 404;
            this.labelAuthHeading.Text = "AUTHORISER SECTION";
            this.labelAuthHeading.Visible = false;
            // 
            // buttonRefresh
            // 
            this.buttonRefresh.BackColor = System.Drawing.Color.Transparent;
            this.buttonRefresh.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(119)))), ((int)(((byte)(192)))));
            this.buttonRefresh.FlatAppearance.BorderSize = 2;
            this.buttonRefresh.FlatAppearance.CheckedBackColor = System.Drawing.Color.Transparent;
            this.buttonRefresh.FlatAppearance.MouseDownBackColor = System.Drawing.Color.White;
            this.buttonRefresh.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonRefresh.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.buttonRefresh.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(119)))), ((int)(((byte)(192)))));
            this.buttonRefresh.Location = new System.Drawing.Point(702, 350);
            this.buttonRefresh.Name = "buttonRefresh";
            this.buttonRefresh.Size = new System.Drawing.Size(85, 28);
            this.buttonRefresh.TabIndex = 402;
            this.buttonRefresh.Text = "Refresh";
            this.buttonRefresh.UseVisualStyleBackColor = false;
            this.buttonRefresh.Click += new System.EventHandler(this.buttonRefresh_Click);
            // 
            // buttonAuthor
            // 
            this.buttonAuthor.BackColor = System.Drawing.Color.Transparent;
            this.buttonAuthor.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(119)))), ((int)(((byte)(192)))));
            this.buttonAuthor.FlatAppearance.BorderSize = 2;
            this.buttonAuthor.FlatAppearance.CheckedBackColor = System.Drawing.Color.Transparent;
            this.buttonAuthor.FlatAppearance.MouseDownBackColor = System.Drawing.Color.White;
            this.buttonAuthor.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonAuthor.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.buttonAuthor.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(119)))), ((int)(((byte)(192)))));
            this.buttonAuthor.Location = new System.Drawing.Point(702, 296);
            this.buttonAuthor.Name = "buttonAuthor";
            this.buttonAuthor.Size = new System.Drawing.Size(85, 29);
            this.buttonAuthor.TabIndex = 401;
            this.buttonAuthor.Text = "Authorise";
            this.buttonAuthor.UseVisualStyleBackColor = false;
            this.buttonAuthor.Click += new System.EventHandler(this.buttonAuthor_Click);
            // 
            // panelAuthor
            // 
            this.panelAuthor.BackColor = System.Drawing.Color.White;
            this.panelAuthor.Controls.Add(this.buttonAuthHistory);
            this.panelAuthor.Controls.Add(this.buttonAuthorise);
            this.panelAuthor.Controls.Add(this.buttonReject);
            this.panelAuthor.Controls.Add(this.labelAuthComm);
            this.panelAuthor.Controls.Add(this.textBoxComment);
            this.panelAuthor.Controls.Add(this.labelAuthStatus);
            this.panelAuthor.Controls.Add(this.labelAuthoriser);
            this.panelAuthor.Controls.Add(this.labelDtAuthRequest);
            this.panelAuthor.Controls.Add(this.labelRequestor);
            this.panelAuthor.Location = new System.Drawing.Point(178, 349);
            this.panelAuthor.Name = "panelAuthor";
            this.panelAuthor.Size = new System.Drawing.Size(518, 234);
            this.panelAuthor.TabIndex = 403;
            // 
            // buttonAuthHistory
            // 
            this.buttonAuthHistory.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(119)))), ((int)(((byte)(192)))));
            this.buttonAuthHistory.FlatAppearance.BorderSize = 2;
            this.buttonAuthHistory.FlatAppearance.MouseDownBackColor = System.Drawing.Color.White;
            this.buttonAuthHistory.FlatAppearance.MouseOverBackColor = System.Drawing.Color.LightSteelBlue;
            this.buttonAuthHistory.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonAuthHistory.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(119)))), ((int)(((byte)(192)))));
            this.buttonAuthHistory.Location = new System.Drawing.Point(405, 177);
            this.buttonAuthHistory.Name = "buttonAuthHistory";
            this.buttonAuthHistory.Size = new System.Drawing.Size(64, 24);
            this.buttonAuthHistory.TabIndex = 364;
            this.buttonAuthHistory.Text = "History";
            this.buttonAuthHistory.UseVisualStyleBackColor = true;
            this.buttonAuthHistory.Click += new System.EventHandler(this.buttonAuthHistory_Click);
            // 
            // buttonAuthorise
            // 
            this.buttonAuthorise.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(119)))), ((int)(((byte)(192)))));
            this.buttonAuthorise.FlatAppearance.BorderSize = 2;
            this.buttonAuthorise.FlatAppearance.MouseDownBackColor = System.Drawing.Color.White;
            this.buttonAuthorise.FlatAppearance.MouseOverBackColor = System.Drawing.Color.LightSteelBlue;
            this.buttonAuthorise.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonAuthorise.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(119)))), ((int)(((byte)(192)))));
            this.buttonAuthorise.Location = new System.Drawing.Point(405, 115);
            this.buttonAuthorise.Name = "buttonAuthorise";
            this.buttonAuthorise.Size = new System.Drawing.Size(64, 24);
            this.buttonAuthorise.TabIndex = 363;
            this.buttonAuthorise.Text = "Authorise ";
            this.buttonAuthorise.UseVisualStyleBackColor = true;
            this.buttonAuthorise.Click += new System.EventHandler(this.buttonAuthorise_Click);
            // 
            // buttonReject
            // 
            this.buttonReject.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(119)))), ((int)(((byte)(192)))));
            this.buttonReject.FlatAppearance.BorderSize = 2;
            this.buttonReject.FlatAppearance.MouseDownBackColor = System.Drawing.Color.White;
            this.buttonReject.FlatAppearance.MouseOverBackColor = System.Drawing.Color.LightSteelBlue;
            this.buttonReject.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonReject.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(119)))), ((int)(((byte)(192)))));
            this.buttonReject.Location = new System.Drawing.Point(405, 146);
            this.buttonReject.Name = "buttonReject";
            this.buttonReject.Size = new System.Drawing.Size(64, 24);
            this.buttonReject.TabIndex = 362;
            this.buttonReject.Text = "Reject";
            this.buttonReject.UseVisualStyleBackColor = true;
            this.buttonReject.Click += new System.EventHandler(this.buttonReject_Click);
            // 
            // labelAuthComm
            // 
            this.labelAuthComm.AutoSize = true;
            this.labelAuthComm.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.labelAuthComm.ForeColor = System.Drawing.Color.Black;
            this.labelAuthComm.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.labelAuthComm.Location = new System.Drawing.Point(16, 98);
            this.labelAuthComm.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelAuthComm.Name = "labelAuthComm";
            this.labelAuthComm.Size = new System.Drawing.Size(123, 13);
            this.labelAuthComm.TabIndex = 361;
            this.labelAuthComm.Text = "Authoriser Comment ";
            // 
            // textBoxComment
            // 
            this.textBoxComment.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.textBoxComment.Location = new System.Drawing.Point(12, 115);
            this.textBoxComment.Multiline = true;
            this.textBoxComment.Name = "textBoxComment";
            this.textBoxComment.Size = new System.Drawing.Size(376, 86);
            this.textBoxComment.TabIndex = 360;
            // 
            // labelAuthStatus
            // 
            this.labelAuthStatus.AutoSize = true;
            this.labelAuthStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.labelAuthStatus.ForeColor = System.Drawing.Color.MediumSeaGreen;
            this.labelAuthStatus.Location = new System.Drawing.Point(9, 65);
            this.labelAuthStatus.Name = "labelAuthStatus";
            this.labelAuthStatus.Size = new System.Drawing.Size(76, 18);
            this.labelAuthStatus.TabIndex = 359;
            this.labelAuthStatus.Text = "Status  : ";
            // 
            // labelAuthoriser
            // 
            this.labelAuthoriser.AutoSize = true;
            this.labelAuthoriser.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.labelAuthoriser.Location = new System.Drawing.Point(9, 27);
            this.labelAuthoriser.Name = "labelAuthoriser";
            this.labelAuthoriser.Size = new System.Drawing.Size(76, 13);
            this.labelAuthoriser.TabIndex = 358;
            this.labelAuthoriser.Text = "Authoriser : ";
            // 
            // labelDtAuthRequest
            // 
            this.labelDtAuthRequest.AutoSize = true;
            this.labelDtAuthRequest.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.labelDtAuthRequest.Location = new System.Drawing.Point(9, 47);
            this.labelDtAuthRequest.Name = "labelDtAuthRequest";
            this.labelDtAuthRequest.Size = new System.Drawing.Size(112, 13);
            this.labelDtAuthRequest.TabIndex = 357;
            this.labelDtAuthRequest.Text = "Date of Request : ";
            // 
            // labelRequestor
            // 
            this.labelRequestor.AutoSize = true;
            this.labelRequestor.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.labelRequestor.Location = new System.Drawing.Point(9, 6);
            this.labelRequestor.Name = "labelRequestor";
            this.labelRequestor.Size = new System.Drawing.Size(77, 13);
            this.labelRequestor.TabIndex = 356;
            this.labelRequestor.Text = "Requestor : ";
            // 
            // labelNumberNotes2
            // 
            this.labelNumberNotes2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelNumberNotes2.AutoSize = true;
            this.labelNumberNotes2.BackColor = System.Drawing.Color.Gainsboro;
            this.labelNumberNotes2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.labelNumberNotes2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.labelNumberNotes2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(119)))), ((int)(((byte)(198)))));
            this.labelNumberNotes2.Location = new System.Drawing.Point(477, 231);
            this.labelNumberNotes2.Name = "labelNumberNotes2";
            this.labelNumberNotes2.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.labelNumberNotes2.Size = new System.Drawing.Size(15, 15);
            this.labelNumberNotes2.TabIndex = 396;
            this.labelNumberNotes2.Text = "2";
            // 
            // buttonNotes2
            // 
            this.buttonNotes2.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("buttonNotes2.BackgroundImage")));
            this.buttonNotes2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.buttonNotes2.FlatAppearance.BorderColor = System.Drawing.Color.DarkRed;
            this.buttonNotes2.FlatAppearance.BorderSize = 0;
            this.buttonNotes2.FlatAppearance.MouseDownBackColor = System.Drawing.Color.White;
            this.buttonNotes2.FlatAppearance.MouseOverBackColor = System.Drawing.Color.LightSteelBlue;
            this.buttonNotes2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonNotes2.ForeColor = System.Drawing.Color.Black;
            this.buttonNotes2.Location = new System.Drawing.Point(433, 231);
            this.buttonNotes2.Margin = new System.Windows.Forms.Padding(2);
            this.buttonNotes2.Name = "buttonNotes2";
            this.buttonNotes2.Size = new System.Drawing.Size(56, 52);
            this.buttonNotes2.TabIndex = 395;
            this.buttonNotes2.UseVisualStyleBackColor = true;
            this.buttonNotes2.Click += new System.EventHandler(this.buttonNotes2_Click);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.panel1.Controls.Add(this.buttonAllActions);
            this.panel1.Controls.Add(this.buttonGLTxns);
            this.panel1.Controls.Add(this.panel7);
            this.panel1.Controls.Add(this.labelNumberNotes2);
            this.panel1.Controls.Add(this.buttonNotes2);
            this.panel1.Location = new System.Drawing.Point(178, 27);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(518, 298);
            this.panel1.TabIndex = 400;
            // 
            // buttonAllActions
            // 
            this.buttonAllActions.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(119)))), ((int)(((byte)(192)))));
            this.buttonAllActions.FlatAppearance.BorderSize = 2;
            this.buttonAllActions.FlatAppearance.MouseDownBackColor = System.Drawing.Color.White;
            this.buttonAllActions.FlatAppearance.MouseOverBackColor = System.Drawing.Color.LightSteelBlue;
            this.buttonAllActions.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonAllActions.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(119)))), ((int)(((byte)(192)))));
            this.buttonAllActions.Location = new System.Drawing.Point(408, 43);
            this.buttonAllActions.Margin = new System.Windows.Forms.Padding(2, 1, 2, 1);
            this.buttonAllActions.Name = "buttonAllActions";
            this.buttonAllActions.Size = new System.Drawing.Size(101, 27);
            this.buttonAllActions.TabIndex = 452;
            this.buttonAllActions.Text = "All Actions";
            this.buttonAllActions.UseVisualStyleBackColor = true;
            this.buttonAllActions.Click += new System.EventHandler(this.buttonAllActions_Click);
            // 
            // buttonGLTxns
            // 
            this.buttonGLTxns.BackColor = System.Drawing.Color.Transparent;
            this.buttonGLTxns.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(119)))), ((int)(((byte)(192)))));
            this.buttonGLTxns.FlatAppearance.BorderSize = 2;
            this.buttonGLTxns.FlatAppearance.MouseDownBackColor = System.Drawing.Color.White;
            this.buttonGLTxns.FlatAppearance.MouseOverBackColor = System.Drawing.Color.LightSteelBlue;
            this.buttonGLTxns.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonGLTxns.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(119)))), ((int)(((byte)(192)))));
            this.buttonGLTxns.Location = new System.Drawing.Point(408, 74);
            this.buttonGLTxns.Name = "buttonGLTxns";
            this.buttonGLTxns.Size = new System.Drawing.Size(101, 33);
            this.buttonGLTxns.TabIndex = 451;
            this.buttonGLTxns.Text = "All Accounting";
            this.buttonGLTxns.UseVisualStyleBackColor = false;
            this.buttonGLTxns.Click += new System.EventHandler(this.buttonGLTxns_Click);
            // 
            // panel7
            // 
            this.panel7.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel7.Controls.Add(this.label2);
            this.panel7.Controls.Add(this.textBoxTotalPairs);
            this.panel7.Controls.Add(this.label5);
            this.panel7.Controls.Add(this.textBoxTotalAmt);
            this.panel7.Controls.Add(this.label4);
            this.panel7.Controls.Add(this.label16);
            this.panel7.Controls.Add(this.textBoxTotalInLines);
            this.panel7.Location = new System.Drawing.Point(19, 8);
            this.panel7.Name = "panel7";
            this.panel7.Size = new System.Drawing.Size(384, 285);
            this.panel7.TabIndex = 450;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(111, 92);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(77, 13);
            this.label2.TabIndex = 476;
            this.label2.Text = "Total GL Pairs ";
            // 
            // textBoxTotalPairs
            // 
            this.textBoxTotalPairs.Location = new System.Drawing.Point(193, 89);
            this.textBoxTotalPairs.Name = "textBoxTotalPairs";
            this.textBoxTotalPairs.ReadOnly = true;
            this.textBoxTotalPairs.Size = new System.Drawing.Size(54, 20);
            this.textBoxTotalPairs.TabIndex = 475;
            this.textBoxTotalPairs.Text = "0";
            this.textBoxTotalPairs.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(113, 65);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(70, 13);
            this.label5.TabIndex = 473;
            this.label5.Text = "Total Amount";
            // 
            // textBoxTotalAmt
            // 
            this.textBoxTotalAmt.Location = new System.Drawing.Point(193, 62);
            this.textBoxTotalAmt.Name = "textBoxTotalAmt";
            this.textBoxTotalAmt.ReadOnly = true;
            this.textBoxTotalAmt.Size = new System.Drawing.Size(107, 20);
            this.textBoxTotalAmt.TabIndex = 472;
            this.textBoxTotalAmt.Text = "0";
            this.textBoxTotalAmt.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(109, 37);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(74, 13);
            this.label4.TabIndex = 471;
            this.label4.Text = "Inputted Lines";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label16.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(119)))), ((int)(((byte)(192)))));
            this.label16.Location = new System.Drawing.Point(109, 9);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(118, 16);
            this.label16.TabIndex = 470;
            this.label16.Text = "Resulted Totals";
            // 
            // textBoxTotalInLines
            // 
            this.textBoxTotalInLines.Location = new System.Drawing.Point(193, 34);
            this.textBoxTotalInLines.Name = "textBoxTotalInLines";
            this.textBoxTotalInLines.ReadOnly = true;
            this.textBoxTotalInLines.Size = new System.Drawing.Size(54, 20);
            this.textBoxTotalInLines.TabIndex = 469;
            this.textBoxTotalInLines.Text = "0";
            this.textBoxTotalInLines.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.label6.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(39)))), ((int)(((byte)(119)))), ((int)(((byte)(192)))));
            this.label6.Location = new System.Drawing.Point(181, 7);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(178, 17);
            this.label6.TabIndex = 451;
            this.label6.Text = "VALIDATION ANALYSIS";
            // 
            // UCForm276c_AUDI_FirstStep
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.Controls.Add(this.label6);
            this.Controls.Add(this.labelAuthHeading);
            this.Controls.Add(this.buttonRefresh);
            this.Controls.Add(this.buttonAuthor);
            this.Controls.Add(this.panelAuthor);
            this.Controls.Add(this.panel1);
            this.Name = "UCForm276c_AUDI_FirstStep";
            this.Size = new System.Drawing.Size(1024, 768);
            this.panelAuthor.ResumeLayout(false);
            this.panelAuthor.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel7.ResumeLayout(false);
            this.panel7.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label labelAuthHeading;
        private System.Windows.Forms.Button buttonRefresh;
        private System.Windows.Forms.Button buttonAuthor;
        private System.Windows.Forms.Panel panelAuthor;
        private System.Windows.Forms.Button buttonAuthHistory;
        private System.Windows.Forms.Button buttonAuthorise;
        private System.Windows.Forms.Button buttonReject;
        private System.Windows.Forms.Label labelAuthComm;
        private System.Windows.Forms.TextBox textBoxComment;
        private System.Windows.Forms.Label labelAuthStatus;
        private System.Windows.Forms.Label labelAuthoriser;
        private System.Windows.Forms.Label labelDtAuthRequest;
        private System.Windows.Forms.Label labelRequestor;
        private System.Windows.Forms.Label labelNumberNotes2;
        private System.Windows.Forms.Button buttonNotes2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel7;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxTotalPairs;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textBoxTotalAmt;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.TextBox textBoxTotalInLines;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button buttonAllActions;
        private System.Windows.Forms.Button buttonGLTxns;
    }
}
