using Nancy;
using Nancy.Hosting.Self;
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
    class Program
    {
        static void Main(string[] args)
        {
            Logger logger = LogManager.GetCurrentClassLogger();     //NLog initiaization (via NLog.config)
            Uri nancyUri = new Uri("http://localhost:52190");   //ToDo: ramlDoc.BaseUri
            string ramlFilePath = @"F:\Project\box.raml";       //ToDo: args[1]

            RAMLDocument ramlDoc = null;                
            try {
                ramlDoc = new RAMLDocument(ramlFilePath);
            }
            catch(FileNotFoundException fnfEx)          //Incorrect RAML file path
            {
                logger.Error(fnfEx);
                return;
            }
            catch(AggregateException aggrEx)            //Exceptions received from assync RamlParser
            {
                foreach (var iEx in aggrEx.InnerExceptions)
                    logger.Error($"RamlParser error: {iEx}");
                return;
            }


            logger.Info("URI {0}", ramlDoc.BaseUri);
            var a = ramlDoc.Routes;


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
