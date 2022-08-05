using System;
using System.Globalization;
using CommandLine;
using RestSharp;
using RestSharp.Authenticators;
using System.Text.Json;

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
         WriteResponse(response);
         Console.WriteLine();

         if (response.IsSuccessful)
         {
            // Query the VMS system for status messages.
            Console.WriteLine($"Requesting VMS status messages (Host:{mProgramOptions.Host}:{mProgramOptions.Port} Secure:{mProgramOptions.UseHttps}).");
            response = GetVaoStatusMessages(60);
            WriteResponse(response, true);
            Console.WriteLine();

            // Query the VMS system. for camera list.
            Console.WriteLine($"Requesting VMS camera list (Host:{mProgramOptions.Host}:{mProgramOptions.Port} Secure:{mProgramOptions.UseHttps}).");
            response = GetVaoCameraList();
            WriteResponse(response, true);
            Console.WriteLine();
         }

         // Allow user to read output
         Console.WriteLine("Press any key to continue.");
         Console.ReadKey();
      }

      private static void WriteResponse(RestResponse response, bool isJsonResponse = false)
      {
         if (response.IsSuccessful)
         {
            string strResponse = response.Content;
            if (isJsonResponse)
            {
               strResponse = FormatJson(strResponse);
            }

            Console.WriteLine($"Response:");
            var originalColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine(strResponse);
            Console.ForegroundColor = originalColor;
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

      private static RestResponse GetVaoStatusMessages(int iMinutesBeforeNow)
      {
         var client = GetRestClient();
         RestRequest request = new RestRequest("status", Method.Get);
         request.AddHeader("If-Modified-Since", DateTime.Now.AddMinutes(-iMinutesBeforeNow).ToString(CultureInfo.InvariantCulture));
         RestResponse response = client.Execute(request);
         return response;
      }

      private static RestResponse GetVaoCameraList()
      {
         var client = GetRestClient();
         RestRequest request = new RestRequest("inputs", Method.Get);
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

      public static string FormatJson(string unPrettyJson)
      {
         var options = new JsonSerializerOptions()
         {
            WriteIndented = true
         };

         var jsonElement = JsonSerializer.Deserialize<JsonElement>(unPrettyJson);
         return JsonSerializer.Serialize(jsonElement, options);
      }
   }
}
