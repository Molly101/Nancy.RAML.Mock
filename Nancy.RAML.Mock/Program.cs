using Nancy.Hosting.Self;
using NancyRAMLMock.RAMLParsing;
using NLog;
using System;
using System.IO;

namespace NancyRAMLMock
{
    public class Program
    {
        static void Main(string[] args)
        {
            ILogger logger = LogManager.GetLogger(Configuration.LoggerName);

            if (args.Length == 1 && !String.IsNullOrEmpty(args[0]))
                Configuration.RAMLFilePath = args[0];

            if (!File.Exists(Configuration.RAMLFilePath))
            {
                var ex = new FileNotFoundException($"Could not find the specified RAML file \"{Configuration.RAMLFilePath}\"!");

                logger.Error(ex);
                throw ex;
            }

            if (args.Length == 2 && !String.IsNullOrEmpty(args[1]))
                Configuration.ConnectionString = args[1];

            //Open and parse RAML file
            Uri nancyUri = null;
            try
            {
                IRamlDocument ramlDoc = new RamlDocWrapper(Configuration.RAMLFilePath);
                nancyUri = ramlDoc.BaseUri;
            }
            catch (Exception ex)
            {
                logger.Error("RAML parser caused an exception!");
                logger.Error(ex);
                throw;
            }

            //Starting Nancy self-hosted process
            HostConfiguration nancyConfig = new HostConfiguration() { RewriteLocalhost = false };
            NancyHost host = null;
            try
            {
                host = new NancyHost(nancyConfig, nancyUri);
                host.Start();
                logger.Info($"Nancy server is listening on \"{nancyUri}\"! Press [anything] Enter to stop the server!!!");
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                logger.Error("Nancy server startup caused an exception!");
                logger.Error(ex);
                throw;
            }
            finally
            {
                host.Stop();
            }
        }
    }
}
