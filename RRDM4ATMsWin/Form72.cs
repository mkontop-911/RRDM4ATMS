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
using System.Data.SqlClient;

namespace RRDM4ATMsWin
{
    public partial class Form72 : Form
    {

        // THIS FORM IS USED TO FALITATE NEEDS OF FORM 71
      

        RRDMTracesReadUpdate Ta = new RRDMTracesReadUpdate(); 

        RRDMUsersAndSignedRecord Us = new RRDMUsersAndSignedRecord();

        

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
            Us.ReadSignedActivityByKey(WSignRecordNo);

            if (Us.ProcessNo == 11) // With 11 Go to UCForm51b = Notes 
            {
                label1.Text = "Repl Cycle Notes Status"; 
                label1.Font = new Font(label1.Font.FontFamily, 22, FontStyle.Bold);
                UCForm51b Step51b = new UCForm51b();
                panel1.Controls.Add(Step51b);
                Step51b.Dock = DockStyle.Fill;
                Step51b.UCForm51bPar(WSignedId, WSignRecordNo, WOperator, WAtmNo, WSesNo);
                Step51b.SetScreen();
            }


            if (Us.ProcessNo == 12) // With 12 Go to UCForm51c = Deposits 
            {
                label1.Text = "Repl Cycle Deposits Status";
                label1.Font = new Font(label1.Font.FontFamily, 22, FontStyle.Bold);
                UCForm51c Step51c = new UCForm51c();
                panel1.Controls.Add(Step51c);
                Step51c.Dock = DockStyle.Fill;
                Step51c.UCForm51cPar(WSignedId, WSignRecordNo, WOperator, WAtmNo, WSesNo);
            }

            if (Us.ProcessNo == 13) // With 13 Go to UCForm51a = Physical Check  
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
