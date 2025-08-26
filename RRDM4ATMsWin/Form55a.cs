using System;
using System.Windows.Forms;
using RRDM4ATMs;

namespace RRDM4ATMsWin
{
    public partial class Form55a : Form
    {
        public Form55a(int MesNo, string to, string from, string BankId, string BranchId, bool SeriousMessage, bool OpenMessage, bool Read,string Message)
        {
            InitializeComponent();

            labelTo.Text = to;
            labelFrom.Text = from;
            labelBankId.Text = BankId;
            labelBranchId.Text = BranchId;
            checkBoxSerious.Checked = SeriousMessage;
            checkBoxOpen.Checked = OpenMessage;
            textBox1.Text = Message;

            if (!Read)
            {
                SetMessageAsRead(MesNo);
            }
        }

        private void SetMessageAsRead(int MsgId)
        {
            RRDMControllerMsgClass MsgClass = new RRDMControllerMsgClass();
            MsgClass.UpdateMsgAsRead(MsgId);

        }

        private void Form55a_Load(object sender, EventArgs e)
        {

            textBox1.Select(0, 0);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
