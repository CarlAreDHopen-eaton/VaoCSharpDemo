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
         Console.WriteLine(response.Content);

         // Allow user to read output
         Console.WriteLine("Press any key to continue.");
         Console.ReadKey();
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
         string addressLine = string.Format(@"{0}://{1}:{2}", mProgramOptions.UseHttps ? "https" : "http", mProgramOptions.Host, mProgramOptions.Port);
         RestClient client = new RestClient(addressLine);
      
         client.Authenticator = new HttpBasicAuthenticator(mProgramOptions.User, mProgramOptions.Password);
         return client;
      }
   }
}
