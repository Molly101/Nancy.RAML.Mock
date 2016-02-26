using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Schema.Generation;
using System;
using System.Collections.Generic;
using System.IO;

namespace NancyRAMLMock
{   /// <summary>
    /// Global application configuration, must be initialized via LoadConfiguration method providing path to valid .JSON configuration file!
    /// </summary>{
    public class Configuration
    {
        private static ConfigurationJson configJson = null;

        public static string DefaultFileName { get; } = "ApiConfig.json";
        public static string ConfigFileName { get; private set; }

        public static string MockUri => configJson?.MockUri;
        public static string RAMLFilePath => configJson?.RAMLFilePath;
        public static string MongoConnectionString => configJson?.MongoConnectionString;
        public static string DataBaseName => configJson?.DataBaseName;

        public static void LoadConfiguration(string configFile)
        {
            JsonTextReader reader = new JsonTextReader(File.OpenText(configFile));
            JSchemaValidatingReader validatingReader = new JSchemaValidatingReader(reader);
            JSchemaGenerator generator = new JSchemaGenerator();
            validatingReader.Schema = generator.Generate(typeof(ConfigurationJson));

            JsonSerializer serializer = new JsonSerializer();
            configJson = serializer.Deserialize<ConfigurationJson>(validatingReader);

            ConfigFileName = configFile;
        }

    } 
}
