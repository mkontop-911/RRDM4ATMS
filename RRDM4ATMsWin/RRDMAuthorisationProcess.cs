using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using System.Windows.Forms;
using System.Data.SqlClient;
using System.Configuration;

namespace RRDM4ATMsWin
{
    class RRDMAuthorisationProcess
    {
        //      
        //
       public int SeqNumber;

       public string Requestor; 
       public string Authoriser;

       public string Origin; // Values 
                             // Dispute Action
                             // Replenishment
                             // Reconciliation 
       public int TranNo; 
       public int DisputeNumber; 
       public int DisputeTransaction;
// Following two fields needed for Replenishment 
       public string AtmNo;
       public int ReplCycle; 

       public string AuthDecision; // Yes, NO
       public string AuthComment;
       public DateTime DateOriginated;
       public DateTime DateAuthorised;
       public int Stage; // 1=Just Created by Requestor , 
                         // 2= Authorizer had receieved a mmessage , 
                         // 3=Authorizer took action, 
                         // 4=message was received by Originator, 
                         // 5 = Work completed 

       public bool Transfered;
       public DateTime TransferedDate;
       public string ReasonOfTransfer; 

       public bool OpenRecord; 
       public string Operator;

        public int TotalNumberforUser;

        public bool RecordFound;
        public bool ErrorFound;
        public string ErrorOutput;

        public string MessageOut; 

        string connectionString = ConfigurationManager.ConnectionStrings
           ["ATMSConnectionString"].ConnectionString;

        //
        // READ Authorization Record for a SequenceNumber 
        //
        public void ReadAuthorizationSpecific(int InSeqNumber)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            string SqlString = "SELECT *"
               + " FROM [dbo].[AuthorizationTable]"
               + " WHERE SeqNumber = @SeqNumber";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@SeqNumber", InSeqNumber);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            SeqNumber = (int)rdr["SeqNumber"];

                            Requestor = (string)rdr["Requestor"];
                            Authoriser = (string)rdr["Authoriser"];

                            Origin = (string)rdr["Origin"];

                            TranNo = (int)rdr["TranNo"];
                            DisputeNumber = (int)rdr["DisputeNumber"];
                            DisputeTransaction = (int)rdr["DisputeTransaction"];

                            AtmNo = (string)rdr["AtmNo"];
                            ReplCycle = (int)rdr["ReplCycle"];

                            AuthDecision = (string)rdr["AuthDecision"];
                            AuthComment = (string)rdr["AuthComment"];

                            DateOriginated = (DateTime)rdr["DateOriginated"];
                            DateAuthorised = (DateTime)rdr["DateAuthorised"];

                            Stage = (int)rdr["Stage"];

                            Transfered = (bool)rdr["Transfered"];
                            TransferedDate = (DateTime)rdr["TransferedDate"];

                            ReasonOfTransfer = (string)rdr["ReasonOfTransfer"];
                            
                            OpenRecord = (bool)rdr["OpenRecord"];
                            Operator = (string)rdr["Operator"];

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
                    ErrorOutput = "An error occured in Authorization Class............. " + ex.Message;
                }
        }

        //
        // READ Authorization Records 
        //
        public void ReadAuthorizations(string InAuthoriser)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            string SqlString = "SELECT *"
               + " FROM [dbo].[AuthorizationTable]"
               + " WHERE Authoriser = @Authoriser";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@Authoriser", InAuthoriser);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            SeqNumber = (int)rdr["SeqNumber"];

                            Requestor = (string)rdr["Requestor"];
                            Authoriser = (string)rdr["Authoriser"];

                            Origin = (string)rdr["Origin"];

                            TranNo = (int)rdr["TranNo"];
                            DisputeNumber = (int)rdr["DisputeNumber"];
                            DisputeTransaction = (int)rdr["DisputeTransaction"];

                            AtmNo = (string)rdr["AtmNo"];
                            ReplCycle = (int)rdr["ReplCycle"];

                            AuthDecision = (string)rdr["AuthDecision"];
                            AuthComment = (string)rdr["AuthComment"];

                            DateOriginated = (DateTime)rdr["DateOriginated"];
                            DateAuthorised = (DateTime)rdr["DateAuthorised"];

                            Stage = (int)rdr["Stage"];

                            Transfered = (bool)rdr["Transfered"];
                            TransferedDate = (DateTime)rdr["TransferedDate"];
                            ReasonOfTransfer = (string)rdr["ReasonOfTransfer"];

                            OpenRecord = (bool)rdr["OpenRecord"];
                            Operator = (string)rdr["Operator"];

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
                    ErrorOutput = "An error occured in Authorization Class............. " + ex.Message;
                }
        }

        //
        // READ Authorization Records 
        //
        public void ReadAuthorizationsUserTotal(string InUser)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            TotalNumberforUser = 0;

            string SqlString = "SELECT *"
               + " FROM [dbo].[AuthorizationTable]"
               + " WHERE (Requestor= @Requestor OR Authoriser = @Authoriser) AND OpenRecord = 1";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@Requestor", InUser);
                        cmd.Parameters.AddWithValue("@Authoriser", InUser);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            TotalNumberforUser = TotalNumberforUser + 1; 

                            SeqNumber = (int)rdr["SeqNumber"];

                            Requestor = (string)rdr["Requestor"];
                            Authoriser = (string)rdr["Authoriser"];

                            OpenRecord = (bool)rdr["OpenRecord"];
                            Operator = (string)rdr["Operator"];

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
                    ErrorOutput = "An error occured in Authorization Class............. " + ex.Message;
                }
        }

        //
        // READ Authorization Record for a Dispute and a transaction 
        //
        public void ReadAuthorizationForDisputeAndTransaction(int InDisputeNumber, int InDisputeTransaction)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            string SqlString = "SELECT *"
               + " FROM [dbo].[AuthorizationTable]"
               + " WHERE DisputeNumber = @DisputeNumber AND DisputeTransaction = @DisputeTransaction ";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@DisputeNumber", InDisputeNumber);
                        cmd.Parameters.AddWithValue("@DisputeTransaction", InDisputeTransaction);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            SeqNumber = (int)rdr["SeqNumber"];

                            Requestor = (string)rdr["Requestor"];
                            Authoriser = (string)rdr["Authoriser"];

                            Origin = (string)rdr["Origin"];

                            TranNo = (int)rdr["TranNo"];
                            DisputeNumber = (int)rdr["DisputeNumber"];
                            DisputeTransaction = (int)rdr["DisputeTransaction"];

                            AtmNo = (string)rdr["AtmNo"];
                            ReplCycle = (int)rdr["ReplCycle"];

                            AuthDecision = (string)rdr["AuthDecision"];
                            AuthComment = (string)rdr["AuthComment"];

                            DateOriginated = (DateTime)rdr["DateOriginated"];
                            DateAuthorised = (DateTime)rdr["DateAuthorised"];

                            Stage = (int)rdr["Stage"];

                            Transfered = (bool)rdr["Transfered"];
                            TransferedDate = (DateTime)rdr["TransferedDate"];
                            ReasonOfTransfer = (string)rdr["ReasonOfTransfer"];

                            OpenRecord = (bool)rdr["OpenRecord"];
                            Operator = (string)rdr["Operator"];

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
                    ErrorOutput = "An error occured in Authorization Class............. " + ex.Message;
                }
        }

        //
        // READ Authorization Record for Replenishement and Reconciliation  , ATMNo and Repl Cycle  
        //
        public void ReadAuthorizationForReplenishmentReconcSpecific(string InAtmNo, int InReplCycle)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            string SqlString = "SELECT *"
               + " FROM [dbo].[AuthorizationTable]"
               + " WHERE AtmNo = @AtmNo AND ReplCycle = @ReplCycle ";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        cmd.Parameters.AddWithValue("@ReplCycle", InReplCycle);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            SeqNumber = (int)rdr["SeqNumber"];

                            Requestor = (string)rdr["Requestor"];
                            Authoriser = (string)rdr["Authoriser"];

                            Origin = (string)rdr["Origin"];

                            TranNo = (int)rdr["TranNo"];
                            DisputeNumber = (int)rdr["DisputeNumber"];
                            DisputeTransaction = (int)rdr["DisputeTransaction"];

                            AtmNo = (string)rdr["AtmNo"];
                            ReplCycle = (int)rdr["ReplCycle"];

                            AuthDecision = (string)rdr["AuthDecision"];
                            AuthComment = (string)rdr["AuthComment"];

                            DateOriginated = (DateTime)rdr["DateOriginated"];
                            DateAuthorised = (DateTime)rdr["DateAuthorised"];

                            Stage = (int)rdr["Stage"];

                            Transfered = (bool)rdr["Transfered"];
                            TransferedDate = (DateTime)rdr["TransferedDate"];
                            ReasonOfTransfer = (string)rdr["ReasonOfTransfer"];

                            OpenRecord = (bool)rdr["OpenRecord"];
                            Operator = (string)rdr["Operator"];

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
                    ErrorOutput = "An error occured in Authorization Class............. " + ex.Message;
                }
        }
       
        // Insert Authorization Record 
        //
        public void InsertAuthorizationRecord()
        {

            ErrorFound = false;
            ErrorOutput = "";

            string cmdinsert = "INSERT INTO [dbo].[AuthorizationTable]"
                    + "([Requestor], [Authoriser], [Origin],  "
                    + " [TranNo], [DisputeNumber], [DisputeTransaction], "
                    + " [AtmNo], [ReplCycle], "
                    + " [DateOriginated], [Stage], [Operator] )"
                    + " VALUES (@Requestor, @Authoriser, @Origin,"
                    + " @TranNo, @DisputeNumber, @DisputeTransaction, "
                    + " @AtmNo, @ReplCycle, "
                    + " @DateOriginated, @Stage, @Operator )";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =

                       new SqlCommand(cmdinsert, conn))
                    {
                        
                        cmd.Parameters.AddWithValue("@Requestor", Requestor);
                        cmd.Parameters.AddWithValue("@Authoriser", Authoriser);

                        cmd.Parameters.AddWithValue("@Origin", Origin);
                      
                        cmd.Parameters.AddWithValue("@TranNo", TranNo);
                        cmd.Parameters.AddWithValue("@DisputeNumber", DisputeNumber);
                        cmd.Parameters.AddWithValue("@DisputeTransaction", DisputeTransaction);

                        cmd.Parameters.AddWithValue("@AtmNo", AtmNo);
                        cmd.Parameters.AddWithValue("@ReplCycle", ReplCycle);

                        cmd.Parameters.AddWithValue("@DateOriginated", DateOriginated);

                        cmd.Parameters.AddWithValue("@Stage", Stage);
                      
                        cmd.Parameters.AddWithValue("@Operator", Operator);

                        //rows number of record got updated

                        int rows = cmd.ExecuteNonQuery();
                        //    if (rows > 0) textBoxMsg.Text = " RECORD INSERTED IN SQL ";
                        //    else textBoxMsg.Text = " Nothing WAS UPDATED ";

                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();
                    ErrorFound = true;
                    ErrorOutput = "An error occured in Authorization Class............. " + ex.Message;
                }
        }
        //
        // UPDATE Authorization Record 
        // 
        public void UpdateAuthorisationRecord( int InSeqNumber)
        {

            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE [dbo].[AuthorizationTable] SET "
                            + " AuthDecision = @AuthDecision, AuthComment = @AuthComment, "
                            + " DateAuthorised = @DateAuthorised, Stage = @Stage, "
                            + " Transfered = @Transfered, TransferedDate = @TransferedDate," 
                            + " ReasonOfTransfer = @ReasonOfTransfer,"
                            + "OpenRecord = @OpenRecord "
                            + " WHERE SeqNumber = @SeqNumber", conn))
                    {
                        cmd.Parameters.AddWithValue("@SeqNumber", InSeqNumber);

                        cmd.Parameters.AddWithValue("@AuthDecision", AuthDecision);
                        cmd.Parameters.AddWithValue("@AuthComment", AuthComment);
                        cmd.Parameters.AddWithValue("@DateAuthorised", DateAuthorised);

                        cmd.Parameters.AddWithValue("@Stage", Stage);

                        cmd.Parameters.AddWithValue("@Transfered", Transfered);
                        cmd.Parameters.AddWithValue("@TransferedDate", TransferedDate);

                        cmd.Parameters.AddWithValue("@ReasonOfTransfer", ReasonOfTransfer);

                        cmd.Parameters.AddWithValue("@OpenRecord", OpenRecord);

                        //rows number of record got updated

                        int rows = cmd.ExecuteNonQuery();
                        //             if (rows > 0) textBoxMsg.Text = " ATMs Table UPDATED ";
                        //            else textBoxMsg.Text = " Nothing WAS UPDATED ";

                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();
                    ErrorFound = true;
                    ErrorOutput = "An error occured in Authorization Update method Class............. " + ex.Message;
                }
        }
// READ LAST AUTHORIAZATION FOR THIS ORIGINATOR 
        public void FindAuthorizationLastNo(string InRequestor, int InTranNo)
        {
            ErrorFound = false;
            ErrorOutput = "";

            string SqlString = "SELECT * FROM AuthorizationTable"
                   + " WHERE SeqNumber = (SELECT MAX(SeqNumber) FROM AuthorizationTable) AND Requestor = @Requestor AND TranNo = @TranNo";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@Requestor", InRequestor);
                        cmd.Parameters.AddWithValue("@TranNo", InTranNo);
                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            SeqNumber = (int)rdr["SeqNumber"];

                            Requestor = (string)rdr["Requestor"];
                            Authoriser = (string)rdr["Authoriser"];

                            Origin = (string)rdr["Origin"];

                            TranNo = (int)rdr["TranNo"];
                            DisputeNumber = (int)rdr["DisputeNumber"];
                            DisputeTransaction = (int)rdr["DisputeTransaction"];

                            AtmNo = (string)rdr["AtmNo"];
                            ReplCycle = (int)rdr["ReplCycle"];

                            AuthDecision = (string)rdr["AuthDecision"];
                            AuthComment = (string)rdr["AuthComment"];

                            DateOriginated = (DateTime)rdr["DateOriginated"];
                            DateAuthorised = (DateTime)rdr["DateAuthorised"];

                            Stage = (int)rdr["Stage"];

                            Transfered = (bool)rdr["Transfered"];
                            TransferedDate = (DateTime)rdr["TransferedDate"];

                            ReasonOfTransfer = (string)rdr["ReasonOfTransfer"];

                            OpenRecord = (bool)rdr["OpenRecord"];

                            Operator = (string)rdr["Operator"];

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
                    ErrorOutput = "An error occured in Authorization Update method Class............. " + ex.Message;

                }
        }
        //
           // READ Authorization Records to find if this user has to Authorize 
        //
        public void CheckOutstandingAuthorizationsStage1(string InSignedId)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            string SqlString = "SELECT *"
               + " FROM [dbo].[AuthorizationTable]"
               + " WHERE Authoriser = @InSignedId AND Stage = 1 AND OpenRecord = 1";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@InSignedId", InSignedId);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            SeqNumber = (int)rdr["SeqNumber"];

                            Requestor = (string)rdr["Requestor"];
                            Authoriser = (string)rdr["Authoriser"];

                            Origin = (string)rdr["Origin"];

                            TranNo = (int)rdr["TranNo"];
                            DisputeNumber = (int)rdr["DisputeNumber"];
                            DisputeTransaction = (int)rdr["DisputeTransaction"];

                            AtmNo = (string)rdr["AtmNo"];
                            ReplCycle = (int)rdr["ReplCycle"];

                            AuthDecision = (string)rdr["AuthDecision"];
                            AuthComment = (string)rdr["AuthComment"];

                            DateOriginated = (DateTime)rdr["DateOriginated"];
                            DateAuthorised = (DateTime)rdr["DateAuthorised"];

                            Stage = (int)rdr["Stage"];

                            Transfered = (bool)rdr["Transfered"];
                            TransferedDate = (DateTime)rdr["TransferedDate"];

                            ReasonOfTransfer = (string)rdr["ReasonOfTransfer"];

                            OpenRecord = (bool)rdr["OpenRecord"];
                            Operator = (string)rdr["Operator"];

                            //**********************
                            if (Stage == 1) Stage = 2; // Message was sent to Authorizer
                      
                            UpdateAuthorisationRecord(SeqNumber);

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
                    ErrorOutput = "An error occured in Authorization Class............. " + ex.Message;
                }
        }

        //
        // READ Authorization Records to find if this user has to Authorize 
        //
        public void CheckOutstandingAuthorizationsStage3(string InSignedId)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            string SqlString = "SELECT *"
               + " FROM [dbo].[AuthorizationTable]"
               + " WHERE Requestor = @InSignedId AND Stage = 3 AND OpenRecord = 1";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@InSignedId", InSignedId);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            SeqNumber = (int)rdr["SeqNumber"];

                            Requestor = (string)rdr["Requestor"];
                            Authoriser = (string)rdr["Authoriser"];

                            Origin = (string)rdr["Origin"];

                            TranNo = (int)rdr["TranNo"];
                            DisputeNumber = (int)rdr["DisputeNumber"];
                            DisputeTransaction = (int)rdr["DisputeTransaction"];

                            AtmNo = (string)rdr["AtmNo"];
                            ReplCycle = (int)rdr["ReplCycle"];

                            AuthDecision = (string)rdr["AuthDecision"];
                            AuthComment = (string)rdr["AuthComment"];

                            DateOriginated = (DateTime)rdr["DateOriginated"];
                            DateAuthorised = (DateTime)rdr["DateAuthorised"];

                            Stage = (int)rdr["Stage"];

                            Transfered = (bool)rdr["Transfered"];
                            TransferedDate = (DateTime)rdr["TransferedDate"];

                            ReasonOfTransfer = (string)rdr["ReasonOfTransfer"];

                            OpenRecord = (bool)rdr["OpenRecord"];
                            Operator = (string)rdr["Operator"];

                            //**********************
                          
                            if (Stage == 3) Stage = 4; // Message Was sent to Originator 
                            UpdateAuthorisationRecord(SeqNumber);

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
                    ErrorOutput = "An error occured in Authorization Class............. " + ex.Message;
                }
        }
        //
        // DELETE Authorization record 
        //
        public void DeleteAuthorisationRecord(int InSeqNumber)
        {

            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("DELETE FROM [dbo].[AuthorizationTable] "
                            + " WHERE SeqNumber =  @SeqNumber ", conn))
                    {
                        cmd.Parameters.AddWithValue("@SeqNumber", InSeqNumber);

                        //rows number of record got updated

                        int rows = cmd.ExecuteNonQuery();
                        //             if (rows > 0) textBoxMsg.Text = " ATMs Table UPDATED ";
                        //            else textBoxMsg.Text = " Nothing WAS UPDATED ";

                    }
                    // Close conn
                    conn.Close();
                }

                catch (Exception ex)
                {
                    conn.Close();
                    ErrorFound = true;
                    ErrorOutput = "An error occured in Authorization Class............. " + ex.Message;
                }

        }
        //
        // Get Message ONe for Replen and Reconciliation 
        //
        public void GetMessageOne(string InAtmNo, int InSesNo, bool InAuthoriser, bool InRequestor, bool Reject)
        {
            //RRDMUsersAndSignedRecord Us = new RRDMUsersAndSignedRecord(); 
            //Us.ReadSignedActivityByKey(InSignRecordNo);


            // InOrigin      ..... 55 or 56  

            ReadAuthorizationForReplenishmentReconcSpecific(InAtmNo, InSesNo);

            if (RecordFound == true & OpenRecord == true)
            {
                if (InRequestor == true & Stage < 4) // Coming from originator and authorisation still outstanding 
                {
                    MessageOut = "A message was sent to authoriser. Refresh for progress monitoring.";
                }

                if (InAuthoriser == true & Stage == 2 )
                {
                    MessageOut = "Proceed to authorisation.";
                }

                if (InAuthoriser == true & Stage == 3)
                {
                    if (AuthDecision == "YES")
                    {
                        MessageOut = "Authorisation Made - Accepted ";
                    }
                    if (AuthDecision == "NO")
                    {
                        MessageOut = "Authorisation Made - Rejected ";
                    }           
                }

                if (InAuthoriser == true & Stage == 4)
                {
                    MessageOut = "Requestor at the stage of Finishing.";
                }

                if (InRequestor == true & Stage == 4 & AuthDecision == "YES")
                {
                    MessageOut = "Authorisation made. Replenishment workflow can finish! ";
                }
            }

            if (Reject == true)
            {
                MessageOut = "Review and redo authorisation. ";
            }

        }

        //
        // Get Message TWO  --- dispute 
        //
        public void GetMessageTwo(int InDisputeNumber, int InDispTranNo, bool InAuthoriser, bool InRequestor, bool Reject)
        {
            //RRDMUsersAndSignedRecord Us = new RRDMUsersAndSignedRecord(); 
            //Us.ReadSignedActivityByKey(InSignRecordNo);


            // InOrigin      ..... 55 or 56  

            ReadAuthorizationForDisputeAndTransaction(InDisputeNumber, InDispTranNo);

            if (RecordFound == true & OpenRecord == true)
            {
                if (InRequestor == true & Stage < 4) // Coming from originator and authorisation still outstanding 
                {
                    MessageOut = "A message was sent to authoriser. Refresh for progress monitoring.";
                }

                if (InAuthoriser == true & Stage == 2)
                {
                    MessageOut = "Proceed to authorisation.";
                }

                if (InAuthoriser == true & Stage == 3)
                {
                    if (AuthDecision == "YES")
                    {
                        MessageOut = "Authorisation Made - Accepted ";
                    }
                    if (AuthDecision == "NO")
                    {
                        MessageOut = "Authorisation Made - Rejected ";
                    }
                }

                if (InAuthoriser == true & Stage == 4)
                {
                    MessageOut = "Requestor at the stage of Finishing.";
                }

                if (InRequestor == true & Stage == 4 & AuthDecision == "YES")
                {
                    MessageOut = "Authorisation made. Workflow can finish! ";
                }
            }

            if (Reject == true)
            {
                MessageOut = "Review and redo authorisation. ";
            }

        }
    }
}
