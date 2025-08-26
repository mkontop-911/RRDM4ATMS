using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using RRDM4ATMs;

namespace RRDM4ATMsWin
{
    public partial class Form502AccessRights_NOSTRO : Form
    {
        RRDMUsersAccessRights Ur = new RRDMUsersAccessRights();
        RRDMGasParameters Gp = new RRDMGasParameters();

        DateTime NullPastDate = new DateTime(1900, 01, 01);
        //DateTime WDateTime;
      
        string WSelectionCriteria; 
        
        string WSignedId;
        int WSignRecordNo;
        string WOperator;
        int WMode; 
       
        public Form502AccessRights_NOSTRO(string InSignedId, int InSignRecordNo, string InOperator, int InMode)
        {
            WSignedId = InSignedId;
            WSignRecordNo = InSignRecordNo;
            WOperator = InOperator;
            WMode = InMode; // 1 = view, 2 = Update

            InitializeComponent();

            // Set Working Date 
            RRDMGasParameters Gp = new RRDMGasParameters();
            string ParId = "267";
            string OccurId = "1";
            Gp.ReadParametersSpecificId(WOperator, ParId, OccurId, "", "");
            string TestingDate = Gp.OccuranceNm;
            if (TestingDate == "YES")
                labelToday.Text = new DateTime(2017, 03, 01).ToShortDateString();
            else labelToday.Text = DateTime.Now.ToShortDateString();

            pictureBox1.BackgroundImage = appResImg.logo2;

            labelUserId.Text = InSignedId;

            if (WMode == 1)
            {
                buttonUpdate.Hide(); 
            }
            if (WMode == 2)
            {
                buttonUpdate.Show();
            }

            Gp.ReadParametersSpecificId(WOperator, "804", "02", "", "");
            if (Gp.RecordFound == true)
            {
                textBox1.Text = Gp.OccuranceNm;
            }
            else
            {
                textBox1.Text = "Not defined";
            }
            Gp.ReadParametersSpecificId(WOperator, "804", "03", "", "");
           
            if (Gp.RecordFound == true)
            {
                textBox2.Text = Gp.OccuranceNm;
            }
            else
            {
                textBox2.Text = "Not defined"; 
            }
            Gp.ReadParametersSpecificId(WOperator, "804", "04", "", "");
          
            if (Gp.RecordFound == true)
            {
                textBox3.Text = Gp.OccuranceNm;
            }
            else
            {
                textBox3.Text = "Not defined";
            }
            Gp.ReadParametersSpecificId(WOperator, "804", "05", "", "");
           
            if (Gp.RecordFound == true)
            {
                textBox4.Text = Gp.OccuranceNm;
            }
            else
            {
                textBox4.Text = "Not defined";
            }
            Gp.ReadParametersSpecificId(WOperator, "804", "06", "", "");
           
            if (Gp.RecordFound == true)
            {
                textBox5.Text = Gp.OccuranceNm;
            }
            else
            {
                textBox5.Text = "Not defined";
            }
            Gp.ReadParametersSpecificId(WOperator, "804", "07", "", "");
            
            if (Gp.RecordFound == true)
            {
                textBox6.Text = Gp.OccuranceNm;
            }
            else
            {
                textBox6.Text = "Not defined";
            }
            Gp.ReadParametersSpecificId(WOperator, "804", "08", "", "");
            
            if (Gp.RecordFound == true)
            {
                textBox7.Text = Gp.OccuranceNm;
            }
            else
            {
                textBox7.Text = "Not defined";
            }
            Gp.ReadParametersSpecificId(WOperator, "804", "09", "", "");
            
            if (Gp.RecordFound == true)
            {
                textBox8.Text = Gp.OccuranceNm;
            }
            else
            {
                textBox8.Text = "Not defined";
            }
            Gp.ReadParametersSpecificId(WOperator, "804", "10", "", "");
            
            if (Gp.RecordFound == true)
            {
                textBox9.Text = Gp.OccuranceNm;
            }
            else
            {
                textBox9.Text = "Not defined";
            }
            Gp.ReadParametersSpecificId(WOperator, "804", "11", "", "");
            
            if (Gp.RecordFound == true)
            {
                textBox10.Text = Gp.OccuranceNm;
            }
            else
            {
                textBox10.Text = "Not defined";
            }
            Gp.ReadParametersSpecificId(WOperator, "804", "12", "", "");
            
            if (Gp.RecordFound == true)
            {
                textBox11.Text = Gp.OccuranceNm;
            }
            else
            {
                textBox11.Text = "Not defined";
            }

        }
// Load 
        private void Form502_Load(object sender, EventArgs e)
        {
            // 

            WSelectionCriteria = " WHERE Operator ='" + WOperator + "'" 
                                + " AND MainFormId = 'Form1 - NOSTRO - OPERATIONAL'"
                                + " ORDER BY PanelName ";

            Ur.ReadUserAccessRightsFillTable(WSelectionCriteria);

            ShowGrid1();

        }
     

        // Row Enter 
       int WSeqNo ;
  
        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {

            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];

            WSeqNo = (int)rowSelected.Cells[0].Value;
            

        }

        private void ShowGrid1()
        {
            
            dataGridView1.DataSource = Ur.UsersAccessTable.DefaultView;

            if (dataGridView1.Rows.Count == 0)
            {
                //MessageBox.Show("No ATMs Available!");
                return;
            }

            dataGridView1.Columns[0].Width = 50; // SeqNo
            dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[0].Visible = false;

            dataGridView1.Columns[1].Width = 80; // MainFormId
            dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[1].Visible = false; 

            dataGridView1.Columns[2].Width = 130; // PanelName
            dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[2].Visible = true;

            dataGridView1.Columns[3].Width = 200; // ButtonText
            dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[3].Visible = true;

            dataGridView1.Columns[4].Width = 40; // SecLevel2
            dataGridView1.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[4].Visible = true;

            dataGridView1.Columns[5].Width = 40; // SecLevel3
            dataGridView1.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[5].Visible = true;

            dataGridView1.Columns[6].Width = 40; // SecLevel4
            dataGridView1.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[6].Visible = true;

            dataGridView1.Columns[7].Width = 40; // SecLevel5
            dataGridView1.Columns[7].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[8].Width = 40; // SecLevel6
            dataGridView1.Columns[8].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[8].Visible = true;

            dataGridView1.Columns[9].Width = 40; // SecLevel7
            dataGridView1.Columns[9].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[9].Visible = true;

            dataGridView1.Columns[10].Width = 40; // SecLevel8
            dataGridView1.Columns[10].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[11].Width = 40; // SecLevel9
            dataGridView1.Columns[11].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[12].Width = 40; // SecLevel10
            dataGridView1.Columns[12].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[13].Width = 40; // SecLevel11
            dataGridView1.Columns[13].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[14].Width = 40; // SecLevel12
            dataGridView1.Columns[14].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[15].Width = 40; // SecLevel13
            dataGridView1.Columns[15].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[16].Width = 40; // SecLevel14
            dataGridView1.Columns[16].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

        }

        private void buttonFinish_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
// UPDATE 
        private void buttonUpdate_Click(object sender, EventArgs e)
        {
            int K = 0;

            while (K <= (dataGridView1.Rows.Count - 1))
            {
              
                DataGridViewRow rowSelected = dataGridView1.Rows[K];

                Ur.SeqNo = (int)rowSelected.Cells[0].Value;
                WSelectionCriteria = " Where SeqNo =" + Ur.SeqNo; 

                Ur.ReadUserAccessRightsBySelectionCriteria(WSelectionCriteria); 
                Ur.SecLevel02 = (bool)rowSelected.Cells[4].Value;
                Ur.SecLevel03 = (bool)rowSelected.Cells[5].Value;
                Ur.SecLevel04 = (bool)rowSelected.Cells[6].Value;
                Ur.SecLevel05 = (bool)rowSelected.Cells[7].Value;
                Ur.SecLevel06 = (bool)rowSelected.Cells[8].Value;
                Ur.SecLevel07 = (bool)rowSelected.Cells[9].Value;
                Ur.SecLevel08 = (bool)rowSelected.Cells[10].Value;
                Ur.SecLevel09 = (bool)rowSelected.Cells[11].Value;
                Ur.SecLevel10 = (bool)rowSelected.Cells[12].Value;
                Ur.SecLevel11 = (bool)rowSelected.Cells[13].Value;
                Ur.SecLevel12 = (bool)rowSelected.Cells[14].Value;
                Ur.SecLevel13 = (bool)rowSelected.Cells[15].Value;
                Ur.SecLevel14 = (bool)rowSelected.Cells[16].Value;

                Ur.UpdateUsersAccessRights(Ur.SeqNo); 

                K++; // Read Next entry of the table 
            }

            MessageBox.Show("Updated!");
        }
// print 
        private void button1_Click(object sender, EventArgs e)
        {
            string P1 = "Access Rights Per Role" ;

            string P2 = "Second Par";
            string P3 = "Third Par";
            string P4 = WOperator;
            string P5 = WSignedId;

            Form56R59ATMS ReportATMS59 = new Form56R59ATMS(P1, P2, P3, P4, P5);
            ReportATMS59.Show();
        }
    }
}
