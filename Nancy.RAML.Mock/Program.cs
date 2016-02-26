using Nancy.Hosting.Self;
using NancyRAMLMock.RAMLParsing;
using Newtonsoft.Json;
using NLog;
using System;
using System.IO;

namespace NancyRAMLMock
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var logger = LogManager.GetLogger("ServerStartup");

            try
            {
                LoadApplicationConfiguration(args);
            }
            catch(Exception ex) when (ex is FileNotFoundException || ex is InvalidOperationException || ex is JsonReaderException) 
            {
                logger.Error("Cannot load configuration file!");
                logger.Error(ex);
                return;
            }

            logger.Info($"Using {Configuration.ConfigFileName} configuration file.");

            Uri nancyUri = new Uri(Configuration.MockUri);
            //Starting Nancy self-hosted process
            HostConfiguration nancyConfig = new HostConfiguration() { RewriteLocalhost = false };
            using(var host = new NancyHost(nancyConfig, nancyUri))
            {
                host.Start();
                logger.Info($"Nancy server is listening on \"{nancyUri}\". Press [anything] Enter to stop the server!");
                Console.ReadLine();
            }
        }

        public static void LoadApplicationConfiguration(string[] args)
        {
            if(args.Length > 1)
                throw new InvalidOperationException("Invalid command line parameters!");

            if (args.Length == 0)                   // if no file name passed as command line argument - use default
                args = new[] { Configuration.DefaultFileName };

            if (!String.IsNullOrEmpty(args[0]) && File.Exists(args[0]))
            { 
                Configuration.LoadConfiguration(args[0]);
            }
            else
            {
                throw new FileNotFoundException($"Configuration file {args[0]} not found!");
            }
        }

    }
}
