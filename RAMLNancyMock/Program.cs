using Nancy;
using Nancy.Hosting.Self;
using Nancy.TinyIoc;
using NLog;
using Raml.Parser.Expressions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NancyRAMLMock
{
    public class Program
    {
        static void Main(string[] args)
        {
            #region Debug args[0]
#if DEBUG
            args = new[] { @"D:\Project\RAMLNancyMock\RAMLSamples\movies.raml" };
#endif
            #endregion

            var logger = LogManager.GetLogger(Configuration.LoggerName);

            if (!String.IsNullOrEmpty(args[0]))
                Configuration.RAMLFilePath = args[0];

            if (!File.Exists(Configuration.RAMLFilePath))
            {
                var ex = new FileNotFoundException($"Could not find the specified RAML file \"{Configuration.RAMLFilePath}\"!");

                logger.Error(ex);
                throw ex;
            }

            if (args.Length==2 && !String.IsNullOrEmpty(args[1]))
                Configuration.ConnectionString = args[1];

            //Open and parse RAML file
            var ramlDoc = new RAML(Configuration.RAMLFilePath);
            Uri nancyUri = ramlDoc.BaseUri;

            //Starting Nancy self-hosted process
            HostConfiguration nancyConfig = new HostConfiguration() { RewriteLocalhost = false };
            using (var host = new NancyHost(nancyConfig, nancyUri))
            {
                host.Start();
                logger.Info($"Nancy server is listening on \"{nancyUri}\"! Press [anything] Enter to stop the server!!!");
                Console.ReadLine();
            }

        }


    }

    
}
