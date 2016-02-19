using System.Configuration;

namespace NancyRAMLMock
{
    public class Configuration
    {
        public static string RAMLFilePath { get; set; } = "api.raml";

        public static string ConnectionString { get; set; } = @"mongodb://localhost:27017";

        public static string DataBaseName { get; set; } = "NancyRAMLMock";

        public static string LoggerName { get; set; } = "Nancy.Raml.Mock";
    }
}
