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
using System.Drawing.Printing;
using System.Configuration;

//multilingual
using System.Resources;
using System.Globalization;

namespace RRDM4ATMsWin
{
    public partial class FormAtmsModel : Form
    {
            string WSignedId;
            int WSignRecordNo;
            int WSecLevel;
            string WOperator;
            int WAction;

        public FormAtmsModel(string InSignedId, int InSignRecordNo, int InSecLevel, string InOperator, int InAction)
        {
            WSignedId = InSignedId;
            WSignRecordNo = InSignRecordNo;
            WSecLevel = InSecLevel;
            WOperator = InOperator;
            WAction = InAction;

            InitializeComponent();

            labelToday.Text = DateTime.Now.ToShortDateString();
            pictureBox1.BackgroundImage = Properties.Resources.logo2;
            UserId.Text = InSignedId; 

        }
    }
}
