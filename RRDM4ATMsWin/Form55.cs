using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using RRDM4ATMs;

namespace RRDM4ATMsWin
{
    public partial class Form55 : Form
    {

        RRDMControllerMsgClass Cm = new RRDMControllerMsgClass();

        public Form55(string filter1, string userId)
        {
            InitializeComponent();

                controlerMSGsBindingSource.Filter = filter1;
             //   dataGridView1.Sort(dataGridView1.Columns[0], ListSortDirection.Descending);
                this.controlerMSGsTableAdapter.Fill(this.aTMSDataSet11.ControlerMSGs);

            
            DataView dv;
            dv = new DataView(aTMSDataSet11.ControlerMSGs, "ToUser = '"+userId +"'", "DtTm", DataViewRowState.CurrentRows);
            dataGridView1.DataSource = dv;

            string SelectionFilter = "FromUser = '" + userId +"'"; 
            Cm.ReadControlerMSGsToFillTable(userId);

            dataGridView2.DataSource = Cm.TableControllerMsgs.DefaultView;

            dataGridView2.Columns[0].Width = 50; //  MesNo
            dataGridView2.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView2.Columns[1].Width = 60; // FromUser
            dataGridView2.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView2.Columns[1].Visible = false;

            dataGridView2.Columns[2].Width = 60; // ToUser
            dataGridView2.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            
            dataGridView2.Columns[3].Width = 110;  // DtTm 
            dataGridView2.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView2.Columns[4].Width = 100;  // Type
            dataGridView2.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView2.Columns[5].Width = 320;  // Message
            dataGridView2.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView2.Columns[6].Width = 70;  // Open Msg
            dataGridView2.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView2.Columns[7].Width = 50;  // ReadMsg
            dataGridView2.Columns[7].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
        private void Form55_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'aTMSDataSet11.ControlerMSGs' table. You can move, or remove it, as needed.
            this.controlerMSGsTableAdapter.Fill(this.aTMSDataSet11.ControlerMSGs);
            SetMsgStatus();
        }

        private void SetMsgStatus()
        {
            for (int x = 0; x < dataGridView1.RowCount; x++)
            {
                bool i = false;
                try
                {
                    i = (bool)dataGridView1["ReadMsg", x].Value;
                }
                catch { }

                if (!i)
                {
                    dataGridView1.Rows[x].DefaultCellStyle.BackColor = Color.PowderBlue;
                }
            }
        }

        private int x = -1;

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                x = e.RowIndex;

                Form55a NForm55a = new Form55a((int)dataGridView1["MesNo", x].Value, ((string)dataGridView1["ToUser", x].Value).ToString(),
                    ((string)dataGridView1["FromUser", x].Value).ToString(), (string)dataGridView1["BankId", x].Value,
                    (string)dataGridView1["BranchId", x].Value, (bool)dataGridView1["SeriousMsg", x].Value,
                    (bool)dataGridView1["OpenMsg", x].Value, (bool)dataGridView1["ReadMsg", x].Value,
                    (string)dataGridView1["Message", x].Value);
                NForm55a.Disposed += NForm55a_Disposed;
                NForm55a.Show();

            }
            catch (Exception ex)
            {
                string ErrorOutput = "An error occured in CaseNotes Class............. " + ex.Message;
            }       
            
        }

        void NForm55a_Disposed(object sender, EventArgs e)
        {
            this.controlerMSGsTableAdapter.Fill(this.aTMSDataSet11.ControlerMSGs);
            dataGridView1.Refresh();
            SetMsgStatus();
            if (x >=0) dataGridView1.Rows[x].Selected = true;

            if (y >=0) dataGridView2.Rows[y].Selected = true;
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void label14_Click(object sender, EventArgs e)
        {

        }

        private int y = -1;
        private void dataGridView2_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            y = e.RowIndex;

            Cm.ReadControlerMSGsSeqNo(WSeqNo);

            Form55a NForm55a = new Form55a(Cm.MesNo, Cm.ToUser,
                                           Cm.FromUser, Cm.BankId,
                                           Cm.BranchId, Cm.SeriousMsg,
                                           Cm.OpenMsg, Cm.ReadMsg, Cm.Message);
            NForm55a.Disposed += NForm55a_Disposed;
            NForm55a.Show();

            //(int MesNo, string to,
            //string from, string BankId, 
            //string BranchId, bool SeriousMessage,
            //bool OpenMessage, bool Read, string Message)

            //Form55a NForm55a = new Form55a((int)dataGridView2["MesNo", y].Value, (string)dataGridView2["ToUser", y].Value,
            //    (string)dataGridView2["FromUser", y].Value, (string)dataGridView2["BankId", y].Value, 
            //    (string)dataGridView2["BranchId", y].Value, (bool)dataGridView2["SeriousMsg", y].Value,
            //    (bool)dataGridView2["OpenMsg", y].Value, true, (string)dataGridView2["Message", y].Value);

            /*         Form55a NForm55a = new Form55a((int)dataGridView2["MesNo2", y].Value, ((string)dataGridView2["ToUser2", y].Value).ToString(),
                         ((string)dataGridView2["FromUser2", y].Value).ToString(), (string)dataGridView2["BankId2", y].Value,
                         (string)dataGridView2["BranchId2", y].Value, (bool)dataGridView2["SeriousMsg2", y].Value,
                         (bool)dataGridView2["OpenMsg2", y].Value, true,
                       (string)dataGridView2["Message2", y].Value);
              */
        }

        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
        // ON ROW ENTER FOR GRID 2
        int WSeqNo;
        private void dataGridView2_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView2.Rows[e.RowIndex];

            WSeqNo = (int)rowSelected.Cells[0].Value;

         
        }
    }
}
