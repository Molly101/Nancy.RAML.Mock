using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Schema.Generation;
using System;
using System.Collections.Generic;
using System.IO;

namespace NancyRAMLMock
{
    public class Configuration
    {
        private Dictionary<string, string> jsonConfig;
        public string LoggerName { get; } = "Nancy.Raml.Mock";

        public Uri MockUri => new Uri(jsonConfig["MockUri"]);
        public string RAMLFilePath => jsonConfig["RAMLFilePath"];
        public string MongoConnectionString => jsonConfig["MongoConnectionString"];
        public string DataBaseName => jsonConfig["DataBaseName"];

        public Configuration(string configFile)
        {
            JsonTextReader reader = new JsonTextReader(File.OpenText(configFile));
            JSchemaValidatingReader validatingReader = new JSchemaValidatingReader(reader);
            JSchemaGenerator generator = new JSchemaGenerator();
            validatingReader.Schema = generator.Generate(typeof(Dictionary<string, string>)); 

            JsonSerializer serializer = new JsonSerializer();
            jsonConfig = serializer.Deserialize<Dictionary<string,string>>(validatingReader);
        }
    } 
}
