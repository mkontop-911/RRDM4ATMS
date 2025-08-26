using System;
using System.Net;
using System.IO;

namespace bulksms_dotnetlib
{
    public class CBSMS_Http
    {
        /* Overview : HTTPSend wraps the .NET Web Request function in a .NET DLL 
         * Usage    : HTTPSend (string URL)  
         * Return   : string result (containing either an error or a valid response)
         * 
         * This fucntion is useable, but should ideally be extended to return a more
         * structure response to the calling application.
         * 
         * No copyright on this - feel free to modify and use as necessary. Just rename 
         * the output DLL to prevent future updates updating your work.
         * 
         * for more info on using this with the BulkSMS API - contact info@bulksms.co.uk
         */

        public string HTTPSend(string sHTTPInputData)
        {

            string sResult;                         //-- the result to be returned from this function
            HttpWebResponse httpResponse = null;    //-- the response buffer used after the Request (HTTP GET) operation


            try
            {

                //-- create the HTTP GET request
                HttpWebRequest httpReq = (HttpWebRequest)WebRequest.Create(sHTTPInputData);

                //-- now get the response
                httpResponse = (HttpWebResponse)(httpReq.GetResponse());
                StreamReader input = new StreamReader(httpResponse.GetResponseStream());
                sResult = input.ReadToEnd();
            }

            catch (Exception e)
            {
                //-- something went wrong. Return result.
                sResult = "HTTP Error: " + e.Message;
            }

            finally
            {
                //-- clean up
                if (httpResponse != null)
                    httpResponse.Close();
            }

            //-- return result
            return sResult;
        }
    }
}
