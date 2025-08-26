using System;
using System.Text;
using System.Net.Mail;

namespace RRDM4ATMs
{
    public class RRDMEmailClass2 : Logger
    {
        public RRDMEmailClass2() : base() { }


        RRDMBanks Ba = new RRDMBanks(); 

        public bool MessageSent;

        //public bool RecordFound;
        public bool ErrorFound;
        public string ErrorOutput;

        public void SendEmail(string Operator, string Recipient, string Subject, string EmailBody)
        {
            if (Operator == "BCAIEGCX")
            {
                return;
            }

            try
            {
            MailMessage message = new MailMessage();
            message.To.Add(Recipient);
            message.Subject = Subject;
            message.Body = EmailBody;

            MessageSent = false;

            Ba.ReadBank(Operator);

            string SenderEmail = Ba.SenderEmail ;
            string SenderUserName = Ba.SenderUserName ;
            string SenderPassword = Ba.SenderPassword;
            string SenderSmtpClient = Ba.SenderSmtpClient ;
            int SenderPort = Ba.SenderPort;

            //string SenderEmail = "kartessystem@gmail.com";
            //string SenderUsername = "kartessystem@gmail.com";
            //string SenderPassword = "gassystem";
            //string SenderSmtpClient = "smtp.gmail.com";
            //int SenderPort = 587;
           
                message.From = new MailAddress(SenderEmail);
                SmtpClient smtp = new SmtpClient(SenderSmtpClient);
                if (SenderPort != 0)
                {
                    smtp.Port = SenderPort;
                }
                smtp.EnableSsl = true;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new System.Net.NetworkCredential(SenderUserName, SenderPassword);

                smtp.Send(message);

                MessageSent = true; 
            }
            catch (Exception ex)
            {

                CatchDetails(ex);
            }
        }
    }
}
