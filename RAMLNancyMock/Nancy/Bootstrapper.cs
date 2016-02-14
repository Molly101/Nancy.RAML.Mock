using MongoDB.Driver;
using Nancy;
using Nancy.TinyIoc;
using NLog;
using NancyRAMLMock.Data;
using NancyRAMLMock.RAMLParsing;

namespace NancyRAMLMock
{
    public class Bootstrapper : DefaultNancyBootstrapper
    {
        protected override void ConfigureApplicationContainer(TinyIoCContainer container)
        {
            base.ConfigureApplicationContainer(container);

            var client = new MongoClient(Configuration.ConnectionString);
            container.Register((c,p) => client.GetDatabase(Configuration.DataBaseName));
            container.Register<IDataStorage, MongoDbDataStorage>();

            container.Register<ILogger>((c, p) => LogManager.GetLogger(Configuration.LoggerName));

            container.Register<IRamlDocument>((c, p) => new RamlDocWrapper(Configuration.RAMLFilePath));
        }
    }
}
