using System;
using System.Drawing;
using System.Windows.Forms;
using RRDM4ATMs;

namespace RRDM4ATMsWin
{
    public partial class Form72 : Form
    {

        // THIS FORM IS USED TO FALITATE NEEDS OF FORM 71
      

        RRDMSessionsTracesReadUpdate Ta = new RRDMSessionsTracesReadUpdate(); 

        RRDMUsersRecords Us = new RRDMUsersRecords();

        

        string WSignedId;
        int WSignRecordNo;
        string WOperator;
   //     bool WPrive;
        string WAtmNo;
        int WSesNo;

        public Form72(string InSignedId, int InSignRecordNo, string InOperator, string InAtmNo, int InSesNo)
        {
            WSignedId = InSignedId;
            WSignRecordNo = InSignRecordNo;
            WOperator = InOperator;
     //       WPrive = InPrive;
            WAtmNo = InAtmNo;
            WSesNo = InSesNo; 

            InitializeComponent();
        }
 
        private void Form72_Load(object sender, EventArgs e)
        {
            RRDMUserSignedInRecords Usi = new RRDMUserSignedInRecords();
            Usi.ReadSignedActivityByKey(WSignRecordNo);

            if (Usi.ProcessNo == 11) // With 11 Go to UCForm51b = Notes 
            {
                label1.Text = "Repl Cycle Notes Status"; 
                label1.Font = new Font(label1.Font.FontFamily, 22, FontStyle.Bold);
                UCForm51b Step51b = new UCForm51b();
                panel1.Controls.Add(Step51b);
                Step51b.Dock = DockStyle.Fill;
                Step51b.UCForm51bPar(WSignedId, WSignRecordNo, WOperator, WAtmNo, WSesNo);
                Step51b.SetScreen();
            }


            if (Usi.ProcessNo == 12) // With 12 Go to UCForm51c = Deposits 
            {
                label1.Text = "Repl Cycle Deposits Status";
                label1.Font = new Font(label1.Font.FontFamily, 22, FontStyle.Bold);
                UCForm51c Step51c = new UCForm51c();
                panel1.Controls.Add(Step51c);
                Step51c.Dock = DockStyle.Fill;
                Step51c.UCForm51cPar(WSignedId, WSignRecordNo, WOperator, WAtmNo, WSesNo);
            }

            if (Usi.ProcessNo == 13) // With 13 Go to UCForm51a = Physical Check  
            {
                label1.Text = "Repl Cycle Physical Check Status";
                label1.Font = new Font(label1.Font.FontFamily, 22, FontStyle.Bold);
                UCForm51a Step51a = new UCForm51a();
                panel1.Controls.Add(Step51a);
                Step51a.Dock = DockStyle.Fill;
                Step51a.UCForm51aPar(WSignedId, WSignRecordNo, WOperator, WAtmNo, WSesNo);
                Step51a.SetScreen();
            }
            
        }  

    }
}
