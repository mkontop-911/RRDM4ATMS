using System;
using System.Net.Http;
using System.Threading.Tasks;

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

        private static readonly HttpClient _httpClient = new HttpClient();

        public async Task<string> HTTPSendAsync(string sHTTPInputData)
        {
            string sResult;

            try
            {
                //-- create the HTTP GET request
                HttpResponseMessage response = await _httpClient.GetAsync(sHTTPInputData);
                
                // Ensure we throw on error to catch it below and return the error message formatted as expected
                response.EnsureSuccessStatusCode();
                
                sResult = await response.Content.ReadAsStringAsync();
            }
            catch (Exception e)
            {
                //-- something went wrong. Return result.
                sResult = "HTTP Error: " + e.Message;
            }

            //-- return result
            return sResult;
        }
    }
}
