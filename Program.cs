using System;
using RestSharp;
using RestSharp.Authenticators;

namespace VaoCSharpDemo
{
   class Program
   {
      private static bool UseHttps = false;
      private static string Password = "SomePassword";
      private static string User = "SomeUsername";
      private static string Address = "SomeIpAddress";
      private static string Port = "444";

      static void Main(string[] args)
      {
         RestResponse response = GetVaoStatus();
         Console.WriteLine(response.Content);
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
         string addressLine = string.Format(@"{0}://{1}:{2}", UseHttps ? "https" : "http", Address, Port);
         RestClient client = new RestClient(addressLine);
         client.Authenticator = new HttpBasicAuthenticator(User, Password);
         return client;
      }
   }
}
