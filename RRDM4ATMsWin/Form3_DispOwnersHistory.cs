using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using RRDM4ATMs;
using System.Configuration;
using System.Diagnostics;

namespace RRDM4ATMsWin
{
    public partial class Form3_DispOwnersHistory : Form
    {

        RRDMUsersAndSignedRecord Us = new RRDMUsersAndSignedRecord();
        RRDMDisputesTableClass Di = new RRDMDisputesTableClass();
        RRDMDisputesOwnersHistory Dh = new RRDMDisputesOwnersHistory();

       
        string WSignedId;
        int WSignRecordNo;
        string WOperator;
        int WDispId;

        public Form3_DispOwnersHistory(string InSignedId, int SignRecordNo, string InOperator, int InDispId)
        {
            WSignedId = InSignedId;
            WSignRecordNo = SignRecordNo;
            WOperator = InOperator;
            WDispId = InDispId;

            InitializeComponent();
        }
// Load 
        private void Form3_DispOwnersHistory_Load(object sender, EventArgs e)
        {

            // Find Dispute details 
            Di.ReadDispute(WDispId);

            labelDispId.Text = "Dispute Id: " + WDispId.ToString();

            if (Di.HasOwner == true)
            {
                Us.ReadUsersRecord(Di.OwnerId);

                labelCurrentOwnerId.Text = "Current Owner Id:   " + Us.UserId;
                labelCuurentOwnerNm.Text = "Current Owner Name: " + Us.UserName;
            }
            else
            {
                labelCurrentOwnerId.Text = "Dispute Has no Specific Owner yet. ";
                labelCuurentOwnerNm.Hide();
            }

            Dh.ReadDisputeOwnersHistory(WOperator, WDispId); 

            dataGridView1.DataSource = Dh.OwnersSelected.DefaultView;

            //// DATA TABLE ROWS DEFINITION 
            //OwnersSelected.Columns.Add("SeqNumber", typeof(int));
            //OwnersSelected.Columns.Add("OwnerId", typeof(string));
            //OwnersSelected.Columns.Add("OwnerName", typeof(string));
            //OwnersSelected.Columns.Add("StartDate", typeof(DateTime));
            //OwnersSelected.Columns.Add("EndDate", typeof(DateTime));
            //OwnersSelected.Columns.Add("CreatorId", typeof(string));
            //OwnersSelected.Columns.Add("Reason", typeof(string));

        }

// ON ROW ENTER 
        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {

            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];

            int WSeqNumber = (int)rowSelected.Cells[0].Value;

            Dh.ReadDisputeOwnersRecordSpecificSeqNo(WOperator, WSeqNumber); 

            Us.ReadUsersRecord(Dh.OwnerId);

            labelOfficerId.Text = "Officer Id   : " + Us.UserId;
            labelOfficerName.Text = "Officer Name : " + Us.UserName; 

        }
// Finish 
        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.Dispose(); 
        }

    }
}
