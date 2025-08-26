using System.Windows.Forms;

namespace RRDM4ATMsWin
{
    public partial class Form350_Route : Form
    {
        public Form350_Route(string InDescription )
        {
            InitializeComponent();

            labelRouteDescription.Text = InDescription ;

            pictureBox1.BackgroundImage = appResImg.Pafos_Route; 

        }
    }
}
