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

namespace RRDM4ATMsWin
{
    public partial class Form350_Route : Form
    {
        public Form350_Route(string InDescription )
        {
            InitializeComponent();

            labelRouteDescription.Text = InDescription ; 

        }
    }
}
