using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using RRDM4ATMs;
using System.Configuration;
//multilingual


namespace RRDM4ATMsWin
{
    public partial class Form5NV : Form
    {

        RRDMAtmsClass Ac = new RRDMAtmsClass();

        RRDMMatchingCategories Mc = new RRDMMatchingCategories();
        RRDMNVDisputes Di = new RRDMNVDisputes();
        RRDMNVDisputesTrans Dt = new RRDMNVDisputesTrans();

        RRDMNVStatement_Lines_InternalAndExternal Se = new RRDMNVStatement_Lines_InternalAndExternal();

        RRDMGasParameters Gp = new RRDMGasParameters();

        RRDMUsersRecords Us = new RRDMUsersRecords();

        RRDMNVMT995AndMT996Pool Mt = new RRDMNVMT995AndMT996Pool();

        RRDMBanks Ba = new RRDMBanks();

        RRDMGetUniqueNumber Gu = new RRDMGetUniqueNumber();

        string WD11 = "WD11";
        string WD12 = "WD12";
        string WD13 = "WD13";
        string WD14 = "Y";
        string WD15 = "N";
        string WD16 = "WD16";
        string WD17 = "WD17";
        string WD21 = "WD21";
        string WD22 = "WD22";
        string WD23 = "WD23";
        string WD24 = "WD24";
        string WD25 = "";
        string WD26 = "";
        string WD27 = "";
        string WD28 = "";
        string WD29 = "";
        string WD30 = "";
        string WD31 = "";
        string WD32 = "";
        string WD33 = "";
        string WD34 = "";
        string WD35 = "";
        string WD36 = "";

        string WD37 = "WD37";
        string WD38 = "WD38";

        string WD40 = "WD40";
        string WD41 = "WD41";

        string WSingleOrigin;

        int WDispNo;

        int TempSeqNo;
        string TempOrigin;

        //bool WSelect;

        //decimal DispAmnt;

        string DD;
        string MM;
        string YY;
        string SixCharExternalValDate;
        string SixCharInternalValDate;

        string FirstTwoOfReason;

        bool ExternalPresent;

        bool InternalPresent;

        string UserBankId;

        DateTime NullPastDate = new DateTime(1900, 01, 01);

        string WSelectionCriteria3;

        string WSignedId;
        int WSignRecordNo;
        string WOperator;
        int WFrom;

        string WCategoryId;
        DataTable WTableDisputedTxns = new DataTable();
        int WInDispNo;

        public Form5NV(string InSignedId, int InSignRecordNo, string InOperator,
                   int InFrom, string InCategoryId, DataTable InTableDisputedTxns, int InDispNo)
        {
            WSignedId = InSignedId;
            WSignRecordNo = InSignRecordNo;
            WOperator = InOperator;

            WFrom = InFrom;
            // 1 = call is coming from Reconciliation Manual to create a new dispute 
            // 2 = Call is coming from Dispute Management for updating Dispute   

            WCategoryId = InCategoryId;
            WTableDisputedTxns = InTableDisputedTxns;
            WInDispNo = InDispNo; // it is zero if new dispute or >0 if for updating 

            InitializeComponent();

            pictureBox1.BackgroundImage = appResImg.logo2;
            labelToday.Text = DateTime.Now.ToShortDateString();
            labelUserId.Text = WSignedId;

            textBoxMsgBoard.Text = "Input information for dispute";

            Us.ReadUsersRecord(InSignedId);

            UserBankId = Us.BankId;

            if (UserBankId != WOperator)
            {
                // User belongs to the Bank Not to ITMX
                WSelectionCriteria3 = " AND ( DebitBank ='" + UserBankId + "' OR CreditBank ='" + UserBankId + "' )";
            }
            else
            {
                WSelectionCriteria3 = "";
            }

            Gp.ParamId = "751"; // Dispute methods - Internal Or external Customer   
            comboBoxReasonMT995.DataSource = Gp.GetArrayParametersOccNoAndName(WOperator, Gp.ParamId);
            comboBoxReasonMT995.DisplayMember = "DisplayValue";

            Gp.ParamId = "252"; // Dispute methods - Internal Or external Customer   
            comboBoxReasonInternal.DataSource = Gp.GetParamOccurancesNm(WOperator);
            comboBoxReasonMT995.DisplayMember = "DisplayValue";

            ExternalPresent = false;

            InternalPresent = false;

            textBoxMsgBoard.Text = "Dispute and Swift Message will be created.";

        }
        // Load data 
        bool WAlreadyExist;
        private void Form5NV_Load(object sender, EventArgs e)
        {
            // Initilialised Form's controls 

            if (WFrom == 2) // COMING FROM Dispute Updating 
            {
                textBoxDisputeId.Text = WDispNo.ToString();
                Di.ReadNVDisputesBySeqNo(WOperator, WDispNo);

                if (Di.IsInternalDispute)
                {
                    textBoxBranchId.Text = Di.InternalBranchId;
                    textBoxTellerName.Text = Di.InternalTellerName;
                    tbTellerPhone.Text = Di.InternalTellerPhone;
                    comboBoxReasonInternal.Text = Di.TypeDescription;
                }
                else
                {
                    comboBoxReasonMT995.Text = Di.TypeDescription;
                }
                tbComments.Text = Di.DispComments;
            }

            Mc.ReadMatchingCategorybyActiveCategId(WOperator, WCategoryId);

            textBoxInternalAcc.Text = Mc.InternalAcc;
            textBoxVostroBank.Text = Mc.VostroBank;
            textBoxExternalAcc.Text = Mc.VostroAcc;
            textBoxCcy.Text = Mc.VostroCurr;

            // Fill Table 
            ShowGrid1();

            WAlreadyExist = false;
            //
            // Check If already exists
            //

            TempOrigin = "";

            int I = 0;

            while (I <= (WTableDisputedTxns.Rows.Count - 1))
            {
                //    RecordFound = true;

                TempSeqNo = (int)WTableDisputedTxns.Rows[I]["SeqNo"];
                TempOrigin = (string)WTableDisputedTxns.Rows[I]["Origin"];

                if (TempOrigin == "EXTERNAL")
                {
                    ExternalPresent = true;

                    SelectionCriteria = " WHERE SeqNo =" + TempSeqNo + " AND Origin ='" + TempOrigin + "'";

                    Se.ReadNVStatements_Lines_BySelectionCriteria(SelectionCriteria);
                    if (Se.RecordFound == true)
                    {

                        textBoxExtValDt.Text = Se.StmtLineValueDate.ToString();
                        DD = Se.StmtLineValueDate.Day.ToString();
                        MM = Se.StmtLineValueDate.Month.ToString();
                        YY = Se.StmtLineValueDate.Year.ToString();
                        if (MM.Length == 1) MM = "0" + MM;
                        YY = YY.Substring(2, 2);
                        SixCharExternalValDate = YY + MM + DD;
                        textBoxExtAmt.Text = Se.StmtLineAmt.ToString("#,##0.00");
                        textBoxExtRef.Text = Se.StmtLineRefForAccountOwner;
                    }

                    SelectionCriteria = " WHERE IsExternalFile = 1 AND SeqNoInFile = " + TempSeqNo;
                }
                if (TempOrigin == "INTERNAL")
                {
                    InternalPresent = true;

                    SelectionCriteria = " WHERE SeqNo =" + TempSeqNo + " AND Origin ='" + TempOrigin + "'";

                    Se.ReadNVStatements_Lines_BySelectionCriteria(SelectionCriteria);
                    if (Se.RecordFound == true)
                    {
                        DD = Se.StmtLineValueDate.Day.ToString();
                        MM = Se.StmtLineValueDate.Month.ToString();
                        YY = Se.StmtLineValueDate.Year.ToString();
                        if (MM.Length == 1) MM = "0" + MM;
                        YY = YY.Substring(2, 2);
                        SixCharInternalValDate = YY + MM + DD;
                        textBoxIntValDt.Text = Se.StmtLineValueDate.ToString();
                        textBoxIntAmt.Text = Se.StmtLineAmt.ToString("#,##0.00");
                        textBoxIntRef.Text = Se.StmtLineRefForAccountOwner;
                    }

                    SelectionCriteria = " WHERE IsInternalFile = 1 AND SeqNoInFile = " + TempSeqNo;
                }

                Dt.ReadNVDisputesTransBySelection(WOperator, SelectionCriteria);

                if (Dt.RecordFound == true)
                {
                    MessageBox.Show("Txn with SeqNo = " + TempSeqNo.ToString()
                                  + " In Statement " + TempOrigin + " Already Exist ");
                    WAlreadyExist = true;
                    return;
                }

                I++; // Read Next entry of the table 
            }

            if (WTableDisputedTxns.Rows.Count == 1)
            {
                label13.Text = "TRANSACTION IN DISPUTE";
                labelSetDetails.Text = "DETAILS";
                WSingleOrigin = TempOrigin;

            }
            if (WTableDisputedTxns.Rows.Count > 1)
            {
                WSingleOrigin = "";
                label13.Text = "TRANSACTION SET";
                labelSetDetails.Text = "DETAILS";
            }

            // Check if both presence 

            labelSetDetails.Show();
            //label6.Show();

            panel6.Show();
            panel7.Show();

            if (WSingleOrigin != "")
            {
                if (WSingleOrigin == "EXTERNAL")
                {
                    panel7.Hide();
                }
                else
                {
                    panel6.Hide();
                }
            }
        }

        // On Row Enter 
        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];

            TempSeqNo = (int)WTableDisputedTxns.Rows[e.RowIndex]["SeqNo"];
            TempOrigin = (string)WTableDisputedTxns.Rows[e.RowIndex]["Origin"];

            SelectionCriteria = " WHERE SeqNo =" + TempSeqNo + " AND Origin ='" + TempOrigin + "'";

            Se.ReadNVStatements_Lines_BySelectionCriteria(SelectionCriteria);
        }
        //
        // READ WHAT WAS SELECTED AND FILL DATA AS APPROPRIATE 
        //

        private void comboBoxReasonMT995_SelectedIndexChanged(object sender, EventArgs e)
        {
            int I;

            FirstTwoOfReason = comboBoxReasonMT995.Text.Substring(0, 2);

            if (FirstTwoOfReason == "00")
            {

                //MessageBox.Show("Please make Swift Code Selection");
                return;
            }

            if (FirstTwoOfReason == "11" || FirstTwoOfReason == "12" || FirstTwoOfReason == "47")
            {
                // 11 : Interest Dispute 
                // 12 : Value Date Dispute
                // 47 : Amount In difference 
                // We need one External and One Internal 

                if (WTableDisputedTxns.Rows.Count != 2)
                {
                    MessageBox.Show("In the transaction set we need one External and one Internal");
                    return;
                }

                if (ExternalPresent == true & InternalPresent == true)
                {
                    // OK 
                }
                else
                {
                    MessageBox.Show("In the transaction set we need one External and one Internal");
                    return;
                }

            }

            //label6.Show();
            labelSwiftMessage.Show();
            panel5.Show();

            //
            // FILL SWIFT MESSAGE 
            //          
            // 
            // ONE RECORD
            //
            if (WTableDisputedTxns.Rows.Count == 1 & FirstTwoOfReason == "06")
            {
                // O6 .... 
                I = 0;

                while (I <= (WTableDisputedTxns.Rows.Count - 1))
                {
                    //    RecordFound = true;

                    int TempSeqNo = (int)WTableDisputedTxns.Rows[I]["SeqNo"];
                    string TempOrigin = (string)WTableDisputedTxns.Rows[I]["Origin"];

                    // COMPILE WHAT IS NECESSARY

                    //if (TempOrigin == "EXTERNAL")
                    //{
                    //    SelectionCriteria = " WHERE SeqNo =" + TempSeqNo + " AND Origin ='" + TempOrigin + "'";

                    //    Se.ReadNVStatements_Lines_BySelectionCriteria(SelectionCriteria);
                    //    if (Se.RecordFound == true)
                    //    {
                    //        // Prepare Tag 79
                    //        RRDMNVStatement_APool_External Sp = new RRDMNVStatement_APool_External();
                    //        Sp.ReadNVStatement_APool_ExternalById(WOperator,
                    //                                        Se.StmtTrxReferenceNumber);

                    //        string StmTag20 = ":20:" + Sp.StmtTrxReferenceNumber;// From Statement
                    //        string StmTag25 = ":25:" + Sp.StmtAccountID;
                    //        string StmTag28 = ":28:" + Sp.StmtNumber.ToString() + Sp.StmtSequenceNumber.ToString();
                    //        string StmTag60 = ":60F:" + Sp.StmtOpeningBalanceAmt.ToString();
                    //        string StmTag61 = ":61:" + Se.StmtLineRaw; // From Ext Trans
                    //        string StmTag62 = ":62F:" + Sp.StmtClosingBalanceAmt.ToString();
                    //        textBoxTag79.Text =
                    //              StmTag20 + Environment.NewLine
                    //            + StmTag25 + Environment.NewLine
                    //            + StmTag28 + Environment.NewLine
                    //            + StmTag60 + Environment.NewLine
                    //            + StmTag61 + Environment.NewLine
                    //            + StmTag62
                    //             ;

                    //        // Generate Unique Number 
                    //        int WUnique = Gu.GetNextValue();

                    //        // Complete SWIFT 
                    //        textBoxMTMessageType.Text = "995";
                    //        textBoxMTReceiver.Text = Se.StmtExternalBankID;
                    //        textBoxTag20.Text = "EWB" + WUnique;
                    //        textBoxTag21.Text = Sp.StmtTrxReferenceNumber;
                    //        DD = Sp.StmtClosingBalanceDate.Day.ToString();
                    //        MM = Sp.StmtClosingBalanceDate.Month.ToString();
                    //        YY = Sp.StmtClosingBalanceDate.Year.ToString();
                    //        if (MM.Length == 1) MM = "0" + MM;
                    //        YY = YY.Substring(2, 2);
                    //        string StmtDate = YY + MM + DD;
                    //        textBoxTag11.Text = ":11R:950 " + StmtDate;

                    //    }
                    //}
                    //if (TempOrigin == "INTERNAL")
                    //{
                    //    SelectionCriteria = " WHERE SeqNo =" + TempSeqNo + " AND Origin ='" + TempOrigin + "'";

                    //    Se.ReadNVStatements_Lines_BySelectionCriteria(SelectionCriteria);
                    //    if (Se.RecordFound == true)
                    //    {

                    //        textBoxTag75_1.Text = ":75:/" + FirstTwoOfReason + "/" + SixCharInternalValDate;

                    //        // Narative
                    //        textBoxTag77_1.Text = "Value Date of txn with Ref:" + Se.StmtLineRefForAccountOwner + " to be :" + SixCharInternalValDate
                    //                            + " and NOT :" + SixCharExternalValDate;
                    //        if (textBoxTag77_1.Text.Length > 740)
                    //        {
                    //            MessageBox.Show("Length of Tag75_2 Greater than 35 =" + textBoxTag75_2.Text.Length.ToString());
                    //        }
                    //    }
                    //}

                    I++; // Read Next entry of the table 
                }
            }

            // 
            // Two records 
            //

            if (WTableDisputedTxns.Rows.Count == 2)
            {
                I = 0;

                while (I <= (WTableDisputedTxns.Rows.Count - 1))
                {
                    //    RecordFound = true;

                    int TempSeqNo = (int)WTableDisputedTxns.Rows[I]["SeqNo"];
                    string TempOrigin = (string)WTableDisputedTxns.Rows[I]["Origin"];


                    if (TempOrigin == "EXTERNAL")
                    {
                        SelectionCriteria = " WHERE SeqNo =" + TempSeqNo + " AND Origin ='" + TempOrigin + "'";

                        Se.ReadNVStatements_Lines_BySelectionCriteria(SelectionCriteria);
                        if (Se.RecordFound == true)
                        {
                            // Prepare Tag 79
                            RRDMNVStatement_APool_External Sp = new RRDMNVStatement_APool_External();
                            Sp.ReadNVStatement_APool_ExternalById(WOperator,
                                                            Se.StmtTrxReferenceNumber);

                            string StmTag20 = ":20:" + Sp.StmtTrxReferenceNumber;// From Statement
                            string StmTag25 = ":25:" + Sp.StmtAccountID;
                            string StmTag28 = ":28:" + Sp.StmtNumber.ToString() + Sp.StmtSequenceNumber.ToString();
                            string StmTag60 = ":60F:" + Sp.StmtOpeningBalanceAmt.ToString();
                            string StmTag61 = ":61:" + Se.StmtLineRaw; // From Ext Trans
                            string StmTag62 = ":62F:" + Sp.StmtClosingBalanceAmt.ToString();
                            textBoxTag79.Text =
                                  StmTag20 + Environment.NewLine
                                + StmTag25 + Environment.NewLine
                                + StmTag28 + Environment.NewLine
                                + StmTag60 + Environment.NewLine
                                + StmTag61 + Environment.NewLine
                                + StmTag62
                                 ;

                            // Generate Unique Number 
                            int WUnique = Gu.GetNextValue();

                            // Complete SWIFT 
                            textBoxMTMessageType.Text = "995";
                            textBoxMTReceiver.Text = Se.StmtExternalBankID;
                            textBoxTag20.Text = "EWB" + WUnique;
                            textBoxTag21.Text = Sp.StmtTrxReferenceNumber;
                            DD = Sp.StmtClosingBalanceDate.Day.ToString();
                            MM = Sp.StmtClosingBalanceDate.Month.ToString();
                            YY = Sp.StmtClosingBalanceDate.Year.ToString();
                            if (MM.Length == 1) MM = "0" + MM;
                            YY = YY.Substring(2, 2);
                            string StmtDate = YY + MM + DD;
                            textBoxTag11.Text = ":11R:950 " + StmtDate;


                        }
                    }
                    if (TempOrigin == "INTERNAL")
                    {
                        SelectionCriteria = " WHERE SeqNo =" + TempSeqNo + " AND Origin ='" + TempOrigin + "'";

                        Se.ReadNVStatements_Lines_BySelectionCriteria(SelectionCriteria);
                        if (Se.RecordFound == true)
                        {

                            textBoxTag75_1.Text = ":75:/" + FirstTwoOfReason + "/" + SixCharInternalValDate;

                            // Narative
                            textBoxTag77_1.Text = "Value Date of txn with Ref:" + Se.StmtLineRefForAccountOwner + " to be :" + SixCharInternalValDate
                                                + " and NOT :" + SixCharExternalValDate;
                            if (textBoxTag77_1.Text.Length > 740)
                            {
                                MessageBox.Show("Length of Tag75_2 Greater than 35 =" + textBoxTag75_2.Text.Length.ToString());
                            }
                        }
                    }

                    I++; // Read Next entry of the table 
                }

            }



        }
        //
        // ADD DISPUTE
        // 
        int TempExceptionId;
        private void buttonAddDispute_Click(object sender, EventArgs e)
        {

            // Validation 

            if (WAlreadyExist == true)
            {
                MessageBox.Show("Txn Already Exist In Dispute");
                return;
            }

            if (radioButtonIsExternal.Checked == false & radioButtonIsInternal.Checked == false)
            {
                MessageBox.Show("Please select Dispute Type");
                return;
            }

            // Telephone Validation 
            string Telephone = tbTellerPhone.Text;

            if (Telephone != "")
            {
                System.Text.RegularExpressions.Regex expr = new System.Text.RegularExpressions.Regex
                 (@"^\s*\+?\s*([0-9][\s-]*){9,}$");
                /*
                                ^           # Start of the string
                               \s*       # Ignore leading whitespace
                               \+?       # An optional plus
                               \s*       # followed by an optional space or multiple spaces
                               (
                                [0-9]  # A digit
                                [\s-]* # followed by an optional space or dash or more than one of those
                               )
                               {9,}     # That appears nine or more times
                               $           # End of the string
                 */

                if (expr.IsMatch(Telephone))
                {
                    //   MessageBox.Show("valid telephone");
                }
                else
                {
                    MessageBox.Show("invalid telephone");
                    return;
                }
            }

            // Check If Already Exist 

            // Initialise Dispute Fields 
            if (radioButtonIsExternal.Checked == true)
            {
                Di.IsExternal = true;
                Di.DispType = 02;
                Di.TypeDescription = comboBoxReasonMT995.Text;
                Di.IsInternalDispute = false;
            }
            if (radioButtonIsInternal.Checked == true)
            {
                Di.IsInternalDispute = true;
                Di.DispType = 02;
                Di.TypeDescription = comboBoxReasonInternal.Text;
                Di.IsExternal = false;
            }

            Di.InternalAccNo = textBoxInternalAcc.Text;
            Di.VostroBank = textBoxVostroBank.Text;
            Di.ExternalAccNo = textBoxExternalAcc.Text;

            Di.InternalBranchId = textBoxBranchId.Text;
            Di.InternalTellerName = textBoxTellerName.Text;
            Di.InternalTellerPhone = tbTellerPhone.Text;
            Di.OpenDate = DateTime.Now;

            Gp.ReadParametersSpecificId(WOperator, "605", "1", "", ""); // LIMIT to be solved date // Dispute target dates 
            int QualityRange1 = (int)Gp.Amount;

            Di.TargetDateTm = DateTime.Now.AddDays(QualityRange1);

            Di.OpenByUserId = WSignedId;
            Di.DispComments = tbComments.Text;

            Di.HasOwner = true;
            Di.OwnerId = WSignedId;
            Di.Active = true;
            Di.Operator = WOperator;

            WDispNo = Di.InsertNewNVDispute();

            labelDispID.Show();
            textBoxDisputeId.Show();

            textBoxDisputeId.Text = WDispNo.ToString();

            int TempSeqNo;
            string TempOrigin;

            // READ TABLE AND INSERT DISPUTE TXNS
            int I = 0;

            while (I <= (WTableDisputedTxns.Rows.Count - 1))
            {
                //    RecordFound = true;

                TempSeqNo = (int)WTableDisputedTxns.Rows[I]["SeqNo"];
                TempOrigin = (string)WTableDisputedTxns.Rows[I]["Origin"];

                SelectionCriteria = " WHERE SeqNo =" + TempSeqNo + " AND Origin ='" + TempOrigin + "'";

                Se.ReadNVStatements_Lines_BySelectionCriteria(SelectionCriteria);
                // Initialise

                // Insert record 
                //
                Dt.DispNo = WDispNo;
                Dt.InternalAccNo = Di.InternalAccNo;
                Dt.VostroBank = Di.VostroBank;
                Dt.ExternalAccNo = Di.ExternalAccNo;
                if (TempOrigin == "EXTERNAL")
                {
                    Dt.IsExternalFile = true;
                    Dt.IsInternalFile = false;
                }

                if (TempOrigin == "INTERNAL")
                {
                    Dt.IsExternalFile = false;
                    Dt.IsInternalFile = true;
                }


                Dt.SeqNoInFile = TempSeqNo;
                Dt.DisputedAmt = Se.StmtLineAmt;

                Dt.Operator = WOperator;

                Dt.InsertNVDisputesTran();

                I++; // Read Next entry of the table 

            }
            //
            //TEST
            //

            // Input Dispute MT995 

            Mt.DispNo = WDispNo;
            Mt.Sender = WOperator;

            Mt.MessageType = textBoxMTMessageType.Text;
            Mt.Receiver = textBoxMTReceiver.Text;

            Mt.TrxReferenceNumber = textBoxTag20.Text;
            Mt.RelatedReference = textBoxTag21.Text;

            Mt.QueryLine1 = textBoxTag75_1.Text;
            Mt.QueryLine2 = textBoxTag75_2.Text;

            Mt.Narrative = textBoxTag77_1.Text;

            if (textBoxTag11.Text != "")
            {
                Mt.OriginalMessageMT = (textBoxTag11.Text).Substring(5, 3);

                Mt.OriginalMessageDate = (textBoxTag11.Text).Substring(9, 6);
            }
            else
            {
                Mt.OriginalMessageMT = "";

                Mt.OriginalMessageDate = "";
            }

            Mt.Operator = WOperator;

            int MtNumber = Mt.InsertRecordInNVMT995Pool();


            //Mt.UpdateNVMT995PoolRecordWithMT996Reply(MtNumber); 

            // Update involved records both Internal And External 

            if (FirstTwoOfReason == "12") TempExceptionId = 12;
            if (FirstTwoOfReason == "01") TempExceptionId = 1;
            if (FirstTwoOfReason == "02") TempExceptionId = 2;
            if (FirstTwoOfReason == "47") TempExceptionId = 47;
            if (FirstTwoOfReason == "14") TempExceptionId = 14;

            // 01 We appear not to have debited so far
            // 02 We appear not to have credited so far  
            // 06 This transaction does not appear in your statement of account 
            // 47 Difference in amount - open Dispute and sent 995  
            // 12 Difference in Value Date 
            // 14 Fantom txn - open Dispute and sent 995 
            // 15 Transaction omited by teller - Open Dispute and send email to teller 

            UpdateTxns(TempExceptionId, WDispNo);

            MessageBox.Show("Dispute Has been opened and MT995 created.");
        }
        //
        // METHOD FOR UPDATING All Dispute records
        // 
        private void UpdateTxns(int InExceptionId, int InDispNo)
        {
            int I = 0;

            while (I <= (WTableDisputedTxns.Rows.Count - 1))
            {

                TempSeqNo = (int)WTableDisputedTxns.Rows[I]["SeqNo"];
                TempOrigin = (string)WTableDisputedTxns.Rows[I]["Origin"];

                if (TempOrigin == "EXTERNAL")
                {

                    SelectionCriteria = " WHERE SeqNo =" + TempSeqNo + " AND Origin ='" + TempOrigin + "'";

                    Se.ReadNVStatements_Lines_BySelectionCriteria(SelectionCriteria);
                    if (Se.RecordFound == true)
                    {
                        Se.IsException = true;
                        Se.ExceptionId = InExceptionId;
                        Se.ExceptionNo = InDispNo;
                    }

                    Se.UpdateExternalFooter(WOperator, SelectionCriteria);

                }
                if (TempOrigin == "INTERNAL")
                {
                    SelectionCriteria = " WHERE SeqNo =" + TempSeqNo + " AND Origin ='" + TempOrigin + "'";

                    Se.ReadNVStatements_Lines_BySelectionCriteria(SelectionCriteria);
                    if (Se.RecordFound == true)
                    {
                        Se.IsException = true;
                        Se.ExceptionId = InExceptionId;
                        Se.ExceptionNo = InDispNo;
                    }

                    Se.UpdateInternalFooter(WOperator, SelectionCriteria);

                }

                I++; // Read Next entry of the table 
            }
        }
        string SelectionCriteria;
        public void ShowGrid1()
        {
            dataGridView1.DataSource = WTableDisputedTxns.DefaultView;

            if (dataGridView1.Rows.Count == 0)
            {
                return;
            }

            DataGridViewCellStyle style = new DataGridViewCellStyle();
            style.Format = "N2";

            dataGridView1.Columns[0].Width = 50; // SeqNo
            dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[0].Visible = false;

            dataGridView1.Columns[1].Width = 40; // ColorNo
            dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[1].Visible = false;

            dataGridView1.Columns[2].Width = 70; // MatchingNo
            dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[2].Visible = false;

            dataGridView1.Columns[3].Width = 170; //MatchedType
            dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[3].Visible = false;

            dataGridView1.Columns[4].Width = 85;  // Origin
            dataGridView1.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[4].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);
            dataGridView1.Columns[4].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[4].HeaderCell.Style.Font = new Font("Tahoma", 09, FontStyle.Bold);

            dataGridView1.Columns[5].Width = 105;  // Acc No 
            dataGridView1.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[5].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[5].Visible = false;

            dataGridView1.Columns[6].Width = 40;  // Done 
            dataGridView1.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[6].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[6].Visible = false;

            dataGridView1.Columns[7].Width = 40;  // Code 
            dataGridView1.Columns[7].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[8].Width = 90; //  ValueDate
            dataGridView1.Columns[8].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[8].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);
            dataGridView1.Columns[8].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[8].HeaderCell.Style.Font = new Font("Tahoma", 09, FontStyle.Bold);

            dataGridView1.Columns[9].Width = 90; // EntryDate
            dataGridView1.Columns[9].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[10].Width = 40; // DR/CR
            dataGridView1.Columns[10].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[11].Width = 100; // Amt
            dataGridView1.Columns[11].DefaultCellStyle = style;
            dataGridView1.Columns[11].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView1.Columns[11].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);
            dataGridView1.Columns[11].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[11].HeaderCell.Style.Font = new Font("Tahoma", 09, FontStyle.Bold);

            dataGridView1.Columns[12].Width = 130; // OurRef
            dataGridView1.Columns[12].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[12].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);
            dataGridView1.Columns[12].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[12].HeaderCell.Style.Font = new Font("Tahoma", 09, FontStyle.Bold);

            dataGridView1.Columns[13].Width = 100; // TheirRef
            dataGridView1.Columns[13].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[14].Width = 95; // OtherDetails
            dataGridView1.Columns[14].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[15].Width = 95; // Ccy
            dataGridView1.Columns[15].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[15].Visible = true;

            dataGridView1.Columns[16].Width = 95; // CcyRate
            dataGridView1.Columns[16].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[16].Visible = true;

            dataGridView1.Columns[17].Width = 95; // GLAccount
            dataGridView1.Columns[17].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[17].Visible = false;

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                int TempSeqNo = (int)row.Cells[0].Value;
                string TempOrigin = row.Cells[4].Value.ToString();

                if (TempOrigin == "EXTERNAL")
                {
                    row.DefaultCellStyle.BackColor = Color.Gainsboro;
                    row.DefaultCellStyle.ForeColor = Color.Black;
                }
                else if (TempOrigin == "INTERNAL")
                {
                    row.DefaultCellStyle.BackColor = Color.White;
                    row.DefaultCellStyle.ForeColor = Color.Black;
                }

                SelectionCriteria = " WHERE SeqNo =" + TempSeqNo + " AND Origin ='" + TempOrigin + "'";

                Se.ReadNVStatements_Lines_BySelectionCriteria(SelectionCriteria);

                if (Se.IsException == true)
                {
                    row.DefaultCellStyle.BackColor = Color.Red;
                    row.DefaultCellStyle.ForeColor = Color.Black;
                }
            }
        }


        //AUDIT TRAIL : GET IMAGE AND INSERT IT IN AUDIT TRAIL 
        private void GetMainBodyImageAndStoreIt(string InCategory, string InSubCategory, string InTypeOfChange, int InUser)
        {
            Bitmap SCREENa;
            System.Drawing.Bitmap memoryImage;
            memoryImage = new System.Drawing.Bitmap(panel4.Width, panel4.Height);
            panel4.DrawToBitmap(memoryImage, panel4.ClientRectangle);
            SCREENa = memoryImage;

            AuditTrailClass At = new AuditTrailClass();
            //     At.InsertRecord(InCategory, InSubCategory, InTypeOfChange, InUser, SCREENa);
        }


        // Print Dispute 

        private void button1_Click(object sender, EventArgs e)
        {

            //Create Printing 
            //Di.ReadDispute(WDispNo);
            //WD11 = Di.CustName;
            //WD12 = Di.AccNo;
            //WD13 = Di.CardNo;
            //TEST
            WD14 = "Y";
            WD15 = "N";

            //WD16 = Di.CustPhone;
            //WD17 = Di.CustEmail;

            string TDispType = "251"; // Parameter Id for dispute types

            Gp.ReadParametersSpecificId(WOperator, TDispType.ToString(), Di.DispType.ToString(), "", "");

            WD37 = Gp.OccuranceNm;

            if (Di.DispType == 5)
            {
                //WD38 = Di.OtherDispTypeDescr;
            }
            else
            {
                WD38 = "";
            }

            string WD39 = "";
            //WD40 = Di.DispId.ToString();

            RRDMUsersRecords Us = new RRDMUsersRecords();
            Us.ReadUsersRecord(WSignedId);

            WD41 = Us.UserName;

            Form56R5 ReportDispute = new Form56R5(WD11, WD12, WD13, WD14, WD15, WD16, WD17,
                        WD21, WD22, WD23, WD24,
                        WD25, WD26, WD27, WD28,
                        WD29, WD30, WD31, WD32,
                        WD33, WD34, WD35, WD36,
                        WD37, WD38,
                        WD40, WD41, WOperator, WD39, "WB45", "WB46", "WB47", "WB48", "WB50");

            ReportDispute.Show();
        }

        protected override CreateParams CreateParams
        {
            get
            {

                CreateParams handleParam = base.CreateParams;

                handleParam.ExStyle |= 0x02000000;   // WS_EX_COMPOSITED        

                return handleParam;

            }

        }


        //**********************************************************************
        // START NOTES 
        //**********************************************************************
        // NOTES 
        string Order;

        string WParameter4;
        string WSearchP4;
        RRDMCaseNotes Cn = new RRDMCaseNotes();

        // Add ATTACHements for notes 
        private void buttonNotes2_Click(object sender, EventArgs e)
        {
            if (textBoxDisputeId.Text != "")
            {
                Form197 NForm197;
                string WParameter3 = "";
                string WParameter4 = "Notes For Dispute " + "DispNo: " + Di.SeqNo.ToString();
                string SearchP4 = "";
                string WMode = "Update";
                NForm197 = new Form197(WSignedId, WSignRecordNo, WOperator, "", WParameter3, WParameter4, WMode, SearchP4);
                NForm197.FormClosed += NForm197_FormClosed;
                NForm197.ShowDialog();
            }
            else
            {
                MessageBox.Show("Open dispute and then insert attachements");
            }
        }
        // Set Number of notes 
        void NForm197_FormClosed(object sender, FormClosedEventArgs e)
        {
            // NOTES for final comment 
            Order = "Descending";
            WParameter4 = "Notes For Dispute " + "DispNo: " + Di.SeqNo.ToString();
            WSearchP4 = "";
            Cn.ReadAllNotes(WParameter4, WSignedId, Order, WSearchP4);
            if (Cn.RecordFound == true)
            {
                labelNumberNotes2.Text = Cn.TotalNotes.ToString();
            }
            else labelNumberNotes2.Text = "0";
        }
        // Finish 
        private void buttonFinish_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        // Show Map Info for all tansactions by time 
        private void buttonShowMaps_Click(object sender, EventArgs e)
        {
            Form350_Route NForm350_Route;
            NForm350_Route = new Form350_Route("TRANSACTIONS MADE ON MAP BY TIME");

            NForm350_Route.ShowDialog();
        }

        // Show ALL Previous Disputes 
        private void button2_Click(object sender, EventArgs e)
        {

            if (textBoxVostroBank.TextLength > 0)
            {
                // Data was inputed 
            }
            else
            {
                MessageBox.Show("Please enter data for customer id!");
                return;
            }

        }
        // External 
        private void radioButtonIsExternal_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonIsExternal.Checked == true)
            {
                label28.Show();
                comboBoxReasonMT995.Show();

                label1.Hide();
                comboBoxReasonInternal.Hide();

            }
            else
            {
                label28.Hide();
                comboBoxReasonMT995.Hide();
                labelSwiftMessage.Hide();
                panel5.Hide();

                label1.Show();
                comboBoxReasonInternal.Show();
            }
        }

    }
}

