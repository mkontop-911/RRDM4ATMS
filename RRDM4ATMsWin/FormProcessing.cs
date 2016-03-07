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
using System.Configuration;

namespace RRDM4ATMsWin
{
    public partial class FormProcessing : Form
    {
        int TotalSize = 0;

        public FormProcessing(int size)
        {
            TotalSize = size;
           
            InitializeComponent();
            label1.Text = "";
          
        }

        private void FormProcessing_Load(object sender, EventArgs e)
        {
          //  progressBar1.Value = 100 ;
        }
        
      public void SetStatus(string status, int Number) 
      { 
           label1.Text = status;

           progressBar1.Value = Number*100/TotalSize;

           progressBar1.Refresh();

           Application.DoEvents();
        }

       private void FormProcessing_FormClosed(object sender, FormClosedEventArgs e)
        {
            Cursor.Current = DefaultCursor;
        }

    }
}
