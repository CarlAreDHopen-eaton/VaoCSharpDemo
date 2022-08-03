using System;
using CommandLine;
using RestSharp;
using RestSharp.Authenticators;

namespace VaoCSharpDemo
{
   class Program
   {
      private static ProgramOptions mProgramOptions;

      static void Main(string[] args)
      {
         // Read command line arguments.
         mProgramOptions = Parser.Default.ParseArguments<ProgramOptions>(args).Value;

         // Query the VMS system.
         Console.WriteLine($"Checking VMS status (Host:{mProgramOptions.Host}:{mProgramOptions.Port} Secure:{mProgramOptions.UseHttps}).");
         RestResponse response = GetVaoStatus();

         // Write the response or error to the console.
         WriteResponse(response);

         // Allow user to read output
         Console.WriteLine("Press any key to continue.");
         Console.ReadKey();
      }

      private static void WriteResponse(RestResponse response)
      {
         if (response.IsSuccessful)
         {
            Console.WriteLine(response.Content);
         }
         else
         {
            Console.WriteLine($"Request failed, {response.ErrorException}");
            if (response.ErrorException?.InnerException != null)
            {
               Console.WriteLine(response.ErrorException.InnerException);
            }
         }
      }

      private static RestResponse GetVaoStatus()
      {
         var client = GetRestClient();
         RestRequest request = new RestRequest("status", Method.Options);
         RestResponse response = client.Execute(request);
         return response;
      }

      private static RestClient GetRestClient()
      {
         RestClient client;
         string addressLine = string.Format(@"{0}://{1}:{2}", mProgramOptions.UseHttps ? "https" : "http", mProgramOptions.Host, mProgramOptions.Port);
         if (mProgramOptions.UseHttps && mProgramOptions.IgnoreCertificateErrors)
         {
            // Bypass ssl validation check.
            var options = new RestClientOptions(addressLine)
            {
               RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true
            };
            client = new RestClient(options);
         }
         else
         {
            client = new RestClient(addressLine); 
         }

         client.Authenticator = new HttpBasicAuthenticator(mProgramOptions.User, mProgramOptions.Password);
         return client;
      }
   }
}
