using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data.SqlClient;
using System.Configuration;


namespace RRDM4ATMs
{
    public class RRDMReconcMaskRecordsLocation
    {

  
        // Variables

        public int SeqNo;
        public string CategoryId;
        public int RMCycle; 

        public string Mask;

        public string FileId01;
        public int SeqNo01;

        public string FileId02;
        public int SeqNo02;

        public string FileId03;
        public int SeqNo03;

        public string FileId04;
        public int SeqNo04;

        public string FileId05;
        public int SeqNo05;

        //string JSonString;

        // Define the data table 
        public DataTable JsonSelectedTable = new DataTable();

        public int TotalSelected;

        int I ;
        string WhatFile;

        string SqlString; // Do not delete

        public bool RecordFound;
        public bool ErrorFound;
        public string ErrorOutput;

        string connectionString = ConfigurationManager.ConnectionStrings
           ["ATMSConnectionString"].ConnectionString;

        RRDMReconcMatchedUnMatchedVisaAuthorClass Rmc = new RRDMReconcMatchedUnMatchedVisaAuthorClass(); 

        DateTime NullPastDate = new DateTime(1900, 01, 01);
       
        // Methods 
        // READ ATMs Main For ATM 
        // 
        public void ReadtblReconcMaskSpecific(int InSeqNo, string InOperator, string InCategory, int InRMCycle)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            I = 0;

            // Read Cycle for all Files 
            RRDMReconcCategoriesMatchingSessions Rs = new RRDMReconcCategoriesMatchingSessions();
            Rs.ReadReconcCategoriesMatchingSessionsByRmCycle(InOperator, InRMCycle);

            JsonSelectedTable = new DataTable();
            JsonSelectedTable.Clear();

            TotalSelected = 0;

            // DATA TABLE ROWS DEFINITION 
            JsonSelectedTable.Columns.Add("RowNo", typeof(int));
            JsonSelectedTable.Columns.Add("UniqueNo", typeof(int));
            JsonSelectedTable.Columns.Add("Category", typeof(string));
            JsonSelectedTable.Columns.Add("Mask", typeof(string));
            JsonSelectedTable.Columns.Add("FileName", typeof(string));
            JsonSelectedTable.Columns.Add("CardNo", typeof(string));
            JsonSelectedTable.Columns.Add("Description", typeof(string));
            JsonSelectedTable.Columns.Add("Amount", typeof(string));
            JsonSelectedTable.Columns.Add("DateTime", typeof(string));
            //JsonSelectedTable.Columns.Add("MatchedDate", typeof(string));

            SqlString = "SELECT *"
              + " FROM [RRDM_Reconciliation].[dbo].[tblReconcMask] "
              + " WHERE SeqNo = @SeqNo ";
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@SeqNo", InSeqNo);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;                  

                            SeqNo = (int)rdr["SeqNo"];
                            
                            CategoryId = (string)rdr["CategoryId"];
                            RMCycle = (int)rdr["RMCycle"];

                            Mask = (string)rdr["Mask"];
                            //JSonString = (string)rdr["JSonString"];

                            FileId01 = (string)rdr["FileId01"];
                            SeqNo01 = (int)rdr["SeqNo01"];

                            FileId02 = (string)rdr["FileId02"];
                            SeqNo02 = (int)rdr["SeqNo02"];

                            FileId03 = (string)rdr["FileId03"];
                            SeqNo03 = (int)rdr["SeqNo03"];

                            FileId04 = (string)rdr["FileId04"];
                            SeqNo04 = (int)rdr["SeqNo04"];

                            FileId05 = (string)rdr["FileId05"];
                            SeqNo05 = (int)rdr["SeqNo05"];


                            DataRow RowSelected = JsonSelectedTable.NewRow();

                            // First File BancNet
                            RowSelected["RowNo"] = 1 ;
                            RowSelected["UniqueNo"] = SeqNo;
                            RowSelected["Category"] = InCategory;
                            RowSelected["Mask"] = Mask;

                         
                            RowSelected["FileName"] = Rs.FileId11; // Logical File Name 
                                //WhatFile = Rs.FileId12; // Physical File Name
                            WhatFile = FileId01; // Physical File Name 01
                            Rmc.ReadMatchedORUnMatchedFileSpecificRecordBySeqNo(WhatFile, SeqNo01);

                                if (Rmc.RecordFound == true)
                                {
                                    RowSelected["CardNo"] = Rmc.CardNumber;
                                    RowSelected["Description"] = Rmc.TransDescr;
                                    RowSelected["Amount"] = Rmc.TransAmount;
                                    RowSelected["DateTime"] = Rmc.TransDate.ToString();
                                    //RowSelected["MatchedDate"] = Rmc.SystemMatchingDtTm.ToString();
                                }
                                else
                                {
                                    RowSelected["CardNo"] = "No Record";
                                    RowSelected["Description"] = "No Record";
                                    RowSelected["Amount"] = 0 ;
                                    RowSelected["DateTime"] = "No Record";
                                    //RowSelected["MatchedDate"] = "No Record";
                                }
                            

                            // ADD ROW
                            JsonSelectedTable.Rows.Add(RowSelected);

                            //*****************************************
                            // Second File Switch
                            //*****************************************

                            RowSelected = JsonSelectedTable.NewRow();

                            RowSelected["RowNo"] = 2;

                            RowSelected["UniqueNo"] = SeqNo;
                            RowSelected["Category"] = InCategory;
                            RowSelected["Mask"] = Mask;

                            RowSelected["FileName"] = Rs.FileId21; // Logical File Name 
                                //WhatFile = Rs.FileId22; // Physical File Name
                            WhatFile = FileId02; // Physical File Name 02
                            Rmc.ReadMatchedORUnMatchedFileSpecificRecordBySeqNo(WhatFile, SeqNo02);

                            if (Rmc.RecordFound == true)
                            {
                                RowSelected["CardNo"] = Rmc.CardNumber;
                                RowSelected["Description"] = Rmc.TransDescr;
                                RowSelected["Amount"] = Rmc.TransAmount;
                                RowSelected["DateTime"] = Rmc.TransDate.ToString();
                                //RowSelected["MatchedDate"] = Rmc.SystemMatchingDtTm.ToString();
                            }
                            else
                            {
                                RowSelected["CardNo"] = "No Record";
                                RowSelected["Description"] = "No Record";
                                RowSelected["Amount"] = 0;
                                RowSelected["DateTime"] = "No Record";
                                //RowSelected["MatchedDate"] = "No Record";
                            }               

                                // ADD ROW
                                JsonSelectedTable.Rows.Add(RowSelected);

                                //**********************************************
                                // Third File T24
                                //**********************************************

                                if (Rs.FileId31 != "")
                                {

                                    RowSelected = JsonSelectedTable.NewRow();

                                    RowSelected["RowNo"] = 3;

                                    RowSelected["UniqueNo"] = SeqNo;
                                    RowSelected["Category"] = InCategory;
                                    RowSelected["Mask"] = Mask;

                                    RowSelected["FileName"] = Rs.FileId31; // Logical File Name 
                                    //WhatFile = Rs.FileId32; // Physical File Name
                                    WhatFile = FileId03; // Physical File Name 03
                                    Rmc.ReadMatchedORUnMatchedFileSpecificRecordBySeqNo(WhatFile, SeqNo03);

                                    if (Rmc.RecordFound == true)
                                    {
                                        RowSelected["CardNo"] = Rmc.CardNumber;
                                        RowSelected["Description"] = Rmc.TransDescr;
                                        RowSelected["Amount"] = Rmc.TransAmount;
                                        RowSelected["DateTime"] = Rmc.TransDate.ToString();
                                        //RowSelected["MatchedDate"] = Rmc.SystemMatchingDtTm.ToString();
                                    }
                                    else
                                    {
                                        RowSelected["CardNo"] = "No Record";
                                        RowSelected["Description"] = "No Record";
                                        RowSelected["Amount"] = 0;
                                        RowSelected["DateTime"] = "No Record";
                                        //RowSelected["MatchedDate"] = "No Record";
                                    }

                                    // ADD ROW
                                    JsonSelectedTable.Rows.Add(RowSelected);
                                }
                                else
                                {
                                    // Do nothing 
                                }

                              


                            //**********************************************
                                // Forth file 
                            //**********************************************

                                if (Rs.FileId41 != "")
                                {
                                    RowSelected = JsonSelectedTable.NewRow();

                                    RowSelected["RowNo"] = 4;

                                    RowSelected["UniqueNo"] = SeqNo;
                                    RowSelected["Category"] = InCategory;
                                    RowSelected["Mask"] = Mask;

                                    RowSelected["FileName"] = Rs.FileId41; // Logical File Name 
                                    //WhatFile = Rs.FileId42; // Physical File Name
                                    WhatFile = FileId04; // Physical File Name 04
                                    Rmc.ReadMatchedORUnMatchedFileSpecificRecordBySeqNo(WhatFile, SeqNo04);

                                    if (Rmc.RecordFound == true)
                                    {
                                        RowSelected["CardNo"] = Rmc.CardNumber;
                                        RowSelected["Description"] = Rmc.TransDescr;
                                        RowSelected["Amount"] = Rmc.TransAmount;
                                        RowSelected["DateTime"] = Rmc.TransDate.ToString();
                                        //RowSelected["MatchedDate"] = Rmc.SystemMatchingDtTm.ToString();
                                    }
                                    else
                                    {
                                        RowSelected["CardNo"] = "No Record";
                                        RowSelected["Description"] = "No Record";
                                        RowSelected["Amount"] = 0;
                                        RowSelected["DateTime"] = "No Record";
                                        //RowSelected["MatchedDate"] = "No Record";
                                    }

                                    // ADD ROW
                                    JsonSelectedTable.Rows.Add(RowSelected);
                                }
                                else
                                {
                                    // Do nothing 
                                }

                                                     

                                //**********************************************
                                // Fifth file 
                                //**********************************************
                          
                                if (Rs.FileId51 != "")
                                {
                                    RowSelected = JsonSelectedTable.NewRow();

                                    RowSelected["RowNo"] = 5;

                                    RowSelected["UniqueNo"] = SeqNo;
                                    RowSelected["Category"] = InCategory;
                                    RowSelected["Mask"] = Mask;

                                    RowSelected["FileName"] = Rs.FileId51; // Logical File Name 
                                    //WhatFile = Rs.FileId52; // Physical File Name
                                    WhatFile = FileId05; // Physical File Name 02
                                    Rmc.ReadMatchedORUnMatchedFileSpecificRecordBySeqNo(WhatFile, SeqNo05);

                                    if (Rmc.RecordFound == true)
                                    {
                                        RowSelected["CardNo"] = Rmc.CardNumber;
                                        RowSelected["Description"] = Rmc.TransDescr;
                                        RowSelected["Amount"] = Rmc.TransAmount;
                                        RowSelected["DateTime"] = Rmc.TransDate.ToString();
                                        //RowSelected["MatchedDate"] = Rmc.SystemMatchingDtTm.ToString();
                                    }
                                    else
                                    {
                                        RowSelected["CardNo"] = "No Record";
                                        RowSelected["Description"] = "No Record";
                                        RowSelected["Amount"] = 0;
                                        RowSelected["DateTime"] = "No Record";
                                        //RowSelected["MatchedDate"] = "No Record";
                                    }

                                    // ADD ROW
                                    JsonSelectedTable.Rows.Add(RowSelected);
                                }
                                else
                                {
                                    // Do nothing 
                                }                 
                           
                        }

                        // Close Reader
                        rdr.Close();
                    }

                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {

                    conn.Close();
                    ErrorFound = true;
                    ErrorOutput = "An error occured in ReadtblReconcMaskSpecific............. " + ex.Message;

                }
        }

    }
}
