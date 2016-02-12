using MongoDB.Bson;
using MongoDB.Driver;
using Nancy;
using Nancy.Bootstrapper;
using Nancy.TinyIoc;
using NLog;
using RAMLNancyMock.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NancyRAMLMock
{
    public class Bootstrapper : DefaultNancyBootstrapper
    {
        protected override void ConfigureApplicationContainer(TinyIoCContainer container)
        {
            base.ConfigureApplicationContainer(container);

            var client = new MongoClient(Configuration.ConnectionString);
            container.Register(client.GetDatabase(Configuration.DataBaseName));

            var collection = client.GetDatabase(Configuration.DataBaseName).GetCollection<BsonDocument>("restaurants");
            container.Register<IDataStorage, MongoDbDataStorage>();

            container.Register<ILogger>((c, p) => LogManager.GetLogger(Configuration.LoggerName));
        }
    }
}
