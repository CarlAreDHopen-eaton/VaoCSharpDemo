using CommandLine;

namespace VaoCSharpDemo
{
   public class ProgramOptions
   {
      [Option('s', "secure", Required = false, HelpText = "Use secure connection.")]
      public bool UseHttps { get; set; } = false;

      [Option('p', "password", Required = true, HelpText = "Password used to connect.")]
      public string Password { get; set; }

      [Option('u', "user", Required = true, HelpText = "User used to connect.")]
      public string User { get; set; }

      [Option('h', "host", Required = true, HelpText = "Host/IP to connect to.")]
      public string Host { get; set; }

      [Option('t', "port", Required = false, HelpText = "Port to connect to (Default port: 444).")]
      public int Port { get; set; } = 444;

      [Option('i', "ignoreCert", Required = false, HelpText = "Ignore certificate errors (Default: false).")]
      public bool IgnoreCertificateErrors { get; set; } = false;

   }
}