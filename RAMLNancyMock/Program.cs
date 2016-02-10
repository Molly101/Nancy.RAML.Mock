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

namespace RAMLNancyMock
{
    public class Program
    {
        public static Logger logger = LogManager.GetCurrentClassLogger();     //NLog initiaization (via NLog.config)
        public static string ramlFilePath;        

        static void Main(string[] args)
        {
            #if DEBUG
                args = new[] { @"D:\Project\RAMLNancyMock\RAMLSamples\movies.raml" };
            #endif

            ramlFilePath = args[0];
            if (!File.Exists(ramlFilePath))
                throw new FileNotFoundException($"Could not find the specified RAML file \"{ramlFilePath}\"!");

            //Open and parse RAML file
            var ramlDoc = new RAML(ramlFilePath);
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
