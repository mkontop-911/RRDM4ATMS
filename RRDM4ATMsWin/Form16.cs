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
//multilingual
using System.Resources;
using System.Globalization;

namespace RRDM4ATMsWin
{
    public partial class Form16 : Form
    {

        DataTable dtPerformance = new DataTable();
        SqlDataAdapter daPerformance;

        string connectionString = ConfigurationManager.ConnectionStrings
           ["ATMSConnectionString"].ConnectionString;

        string SQLString; 
        
        string WAtmNo;
        string WProcessNm;

        DateTime WDate;
        DateTime WFromDate;
        DateTime WToDate;

        public Form16(string InAtmNo, string InProcessNm, DateTime InDate)
        {
            WAtmNo = InAtmNo; 
            WProcessNm = InProcessNm; 
            WDate = InDate;

            WFromDate = WDate.AddDays(-1).Date;
            WToDate = WDate.AddDays(1).Date; 

            InitializeComponent();
        }

        private void Form16_Load(object sender, EventArgs e)
        {

            SQLString = "Select * FROM [dbo].[PerformanceTrace]"
                  + " WHERE AtmNo=@AtmNo AND StartDt BETWEEN @WFromDate AND @WToDate";

            try
            {
                SqlConnection conn =
                      new SqlConnection(connectionString);
                using (
                    daPerformance = new SqlDataAdapter(SQLString, conn))
                {
                    daPerformance.SelectCommand.Parameters.AddWithValue("@AtmNo", WAtmNo);
                    daPerformance.SelectCommand.Parameters.AddWithValue("@WFromDate", WFromDate);
                    daPerformance.SelectCommand.Parameters.AddWithValue("@WToDate", WToDate);

                    SqlCommandBuilder cmdBldr = new SqlCommandBuilder(daPerformance);

                    daPerformance.Fill(dtPerformance); // ATMs Numbers are now in data set table
                }

            }

            catch (Exception ex)
            {

                string exception = ex.ToString();
                // MessageBox.Show(ex.ToString());
                MessageBox.Show(string.Format("An error occurred: {0}", ex.Message));

            }

            int DtSize = dtPerformance.Rows.Count;
            string DtsSize = DtSize.ToString();
            //           MessageBox.Show(" Table Loaded ", DtsSize);

            dataGridView1.DataSource = dtPerformance.DefaultView;

        }
    }
}
