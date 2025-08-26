using System.Data;
using System.Windows.Forms;
using System.Configuration;
//multilingual

namespace RRDM4ATMsWin
{
    public partial class Form36NOSTRO : Form
    {
        string connectionString = ConfigurationManager.ConnectionStrings
             ["ATMSConnectionString"].ConnectionString;

        string WSignedId;
        int WSignRecordNo;
        string WOperator;
     
        DataTable WTable = new DataTable();
        string WHeading;
        int WAction;

        public Form36NOSTRO(string InSignedId, int InSignRecordNo, string InOperator,  
            DataTable  InTable, decimal[] Inyvalues2, string InHeading, int InAction )
        {
            WSignedId = InSignedId;
            WSignRecordNo = InSignRecordNo;
            WOperator = InOperator;
       
            WTable = InTable;
            WHeading = InHeading; 
            WAction = InAction;

            InitializeComponent();

            if (WAction == 4) chart3.Show();
            else chart3.Hide();

            if (WAction == 1) // 
            {
                label14.Text = WHeading;
                // Set chart data source  
                chart1.DataSource = WTable.DefaultView;
                chart1.Series[0].Name = "PercentEfficiency";
                // Set series members names for the X and Y values  
                chart1.Series[0].XValueMember = "Date";
                chart1.Series[0].YValueMembers = "PercentEfficiency";

                // Data bind to the selected data source  
                chart1.DataBind();
            }

            if (WAction == 2) // Users
            {
                label14.Text = WHeading;
                chart1.ChartAreas[0].AxisX.ScaleView.Zoomable = true;

                chart1.DataSource = WTable.DefaultView;
                chart1.Series[0].Name = "TotalNumberProcessed";
                // Set series members names for the X and Y values  
                chart1.Series[0].XValueMember = "UserId";
                chart1.Series[0].YValueMembers = "TotalNumberProcessed";

                // Data bind to the selected data source  
                chart1.DataBind();
            }

            if (WAction == 3) // ATMs Turnover 
            {
                label14.Text = WHeading;
                // Set chart data source  
                chart1.DataSource = WTable.DefaultView;
                chart1.Series[0].Name = "PercentEfficiency";
                //chart1.Series.Add("Avg_CrAmount");
                // Set series members names for the X and Y values  
                chart1.Series[0].XValueMember = "Bank";
                chart1.Series[0].YValueMembers = "PercentEfficiency";
                //chart1.Series[1].XValueMember = "Date";
                //chart1.Series[1].YValueMembers = "Avg_CrAmount";

                // Data bind to the selected data source  
                chart1.DataBind();
            }

            if (WAction == 4) //
            {
                label14.Text = WHeading;

                string[] xvalues2 =
                   { "0 - 4", "4 - 7", "7 - 15", "15 - 30","30 - 60"," > 60" };
                // Set series members names for the X and Y values 
                chart3.Series[0].Points.DataBindXY(xvalues2, Inyvalues2);
            }
         
        }
    }
}
