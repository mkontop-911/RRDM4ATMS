using System.Data;
using System.Windows.Forms;
using System.Configuration;
//multilingual

namespace RRDM4ATMsWin
{
    public partial class Form35 : Form
    {
        string connectionString = ConfigurationManager.ConnectionStrings
             ["ATMSConnectionString"].ConnectionString;

        string WSignedId;
        int WSignRecordNo;
        string WOperator;
     
        DataTable WTable = new DataTable();
        string WHeading;
        int WAction;

        public Form35(string InSignedId, int InSignRecordNo, string InOperator,  
            DataTable  InTable, string InHeading, int InAction )
        {
            WSignedId = InSignedId;
            WSignRecordNo = InSignRecordNo;
            WOperator = InOperator;
       
            WTable = InTable;
            WHeading = InHeading; 
            WAction = InAction;

            InitializeComponent();
            if (WAction == 0)
            {
                label14.Text = WHeading; 
                // Set chart1  data source  
                chart1.DataSource = WTable.DefaultView;
                chart1.Series[0].Name = "Cash_Dispensed";
                // Set series members names for the X and Y values  
                chart1.Series[0].XValueMember = "Date";
                chart1.Series[0].YValueMembers = "Dispensed";

                // Data bind to the selected data source  
                chart1.DataBind();
            }

            if (WAction == 1) // Replenishment 
            {
                label14.Text = WHeading; 
                // Set chart2 data source  
                chart1.DataSource = WTable.DefaultView;
                chart1.Series[0].Name = "Avg_Repl";
                chart1.Series.Add("TotErr");
                chart1.Series.Add("NotReconc");
                // Set series members names for the X and Y values  
                chart1.Series[0].XValueMember = "Date";
                chart1.Series[0].YValueMembers = "Avg_Repl";
                chart1.Series[1].XValueMember = "Date";
                chart1.Series[1].YValueMembers = "TotErr";
                chart1.Series[2].XValueMember = "Date";
                chart1.Series[2].YValueMembers = "NotReconc";

                // Data bind to the selected data source  
                chart1.DataBind();
            }

            if (WAction == 2) // Users
            {
                label14.Text = WHeading; 
                // Set chart2 data source  
                chart1.DataSource = WTable.DefaultView;
                chart1.Series[0].Name = "TotErr";
                chart1.Series.Add("NotReconc");
                // Set series members names for the X and Y values  
                chart1.Series[0].XValueMember = "UserId";
                chart1.Series[0].YValueMembers = "TotErr";
                chart1.Series[1].XValueMember = "UserId";
                chart1.Series[1].YValueMembers = "NotReconc";

                // Data bind to the selected data source  
                chart1.DataBind();
            }

            if (WAction == 3) // ATMs Turnover 
            {
                label14.Text = WHeading; 
                chart1.DataSource = WTable.DefaultView;
                chart1.Series[0].Name = "Avg_DrTrans";
                chart1.Series.Add("Avg_DrAmount");

                // Set series members names for the X and Y values  
                chart1.Series[0].XValueMember = "Date";
                chart1.Series[0].YValueMembers = "Avg_DrTrans";
                chart1.Series[1].XValueMember = "Date";
                chart1.Series[1].YValueMembers = "Avg_DrAmount";

                // Data bind to the selected data source  
                chart1.DataBind();
            }

            if (WAction == 4) // Cit providers
            {
                label14.Text = WHeading;
                // Set chart2 data source  
                chart1.DataSource = WTable.DefaultView;
                chart1.Series[0].Name = "Avg_Repl";
                chart1.Series.Add("TotErr");
                chart1.Series.Add("NotReconc");
                // Set series members names for the X and Y values  
                chart1.Series[0].XValueMember = "CitId";
                chart1.Series[0].YValueMembers = "Avg_Repl";
                chart1.Series[1].XValueMember = "CitId";
                chart1.Series[1].YValueMembers = "TotErr";
                chart1.Series[2].XValueMember = "CitId";
                chart1.Series[2].YValueMembers = "NotReconc";

                // Data bind to the selected data source  
                chart1.DataBind();
            }

            if (WAction == 5) // ATMs Disputes 
            {
                label14.Text = WHeading; 
                // Set chart2 data source  
                chart1.DataSource = WTable.DefaultView;
                chart1.Series[0].Name = "No_Of_Tran Disp";
                // Set series members names for the X and Y values  
                chart1.Series[0].XValueMember = "Disp_Date";
                chart1.Series[0].YValueMembers = "No_Of_Tran_Disp";
                // Data bind to the selected data source  
                chart1.DataBind();
            }

            if (WAction == 6) // Profitability  
            {

                label14.Text = WHeading;

                // Set chart2 data source  
                chart1.DataSource = WTable.DefaultView;
                chart1.Series[0].Name = "COST";
                chart1.Series.Add("REVENEW");
                chart1.Series.Add("PROFIT");
                // Set series members names for the X and Y values  
                chart1.Series[0].XValueMember = "ATMNO";
                chart1.Series[0].YValueMembers = "COST";
                chart1.Series[1].XValueMember = "ATMNO";
                chart1.Series[1].YValueMembers = "REVENEW";
                chart1.Series[2].XValueMember = "ATMNO";
                chart1.Series[2].YValueMembers = "PROFIT";

                // Data bind to the selected data source  
                chart1.DataBind();
            }

            if (WAction == 31) // Last Month turnover  
            {
                label14.Text = WHeading;
                // Set chart data source  
                chart1.DataSource = WTable.DefaultView;
                chart1.Series[0].Name = "Disp_Amt";
                // Set series members names for the X and Y values  
                chart1.Series[0].XValueMember = "Date";
                chart1.Series[0].YValueMembers = "Disp_Amt";

                // Data bind to the selected data source  
                chart1.DataBind();

            }
        }
    }
}
