using bulksms_dotnetlib;

Console.WriteLine("Testing RRDMSMSLib...");

var smsClient = new CBSMS_Http();
string testUrl = "https://www.example.com";

Console.WriteLine($"Sending request to: {testUrl}");

try
{
    string result = await smsClient.HTTPSendAsync(testUrl);
    Console.WriteLine("Response received (first 100 chars):");
    Console.WriteLine(result.Substring(0, Math.Min(result.Length, 100)));
}
catch (Exception ex)
{
    Console.WriteLine($"An error occurred: {ex.Message}");
}

Console.WriteLine("Test completed.");
