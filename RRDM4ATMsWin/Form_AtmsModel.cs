using System;
using System.Windows.Forms;

//multilingual

namespace RRDM4ATMsWin
{
    public partial class Form_AtmsModel : Form
    {
            string WSignedId;
            int WSignRecordNo;
            int WSecLevel;
            string WOperator;
            int WAction;

        public Form_AtmsModel(string InSignedId, int InSignRecordNo, int InSecLevel, string InOperator, int InAction)
        {
            WSignedId = InSignedId;
            WSignRecordNo = InSignRecordNo;
            WSecLevel = InSecLevel;
            WSecLevel = InSecLevel;
            WOperator = InOperator;
            WAction = InAction;

            InitializeComponent();

            labelToday.Text = DateTime.Now.ToShortDateString();
            pictureBox1.BackgroundImage = appResImg.logo2;
            UserId.Text = InSignedId; 

        }
    }
}
