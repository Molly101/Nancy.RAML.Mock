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
        public static string ramlFilePath = @"F:\Project\movies.raml";        //ToDo: args[1]

        static void Main(string[] args)
        {
            //Open and parse RAML file
            var ramlDoc = new RAMLDocument(ramlFilePath);
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
