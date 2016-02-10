using Nancy;
using Nancy.Extensions;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RAMLNancyMock
{
    public class RestModule : NancyModule
    {
        private static ConcurrentDictionary<string, string> dataStorage = new ConcurrentDictionary<string, string>();

        public RestModule()
        {
            var ramlDoc = new RAML(Program.ramlFilePath);

            var routes = from r in ramlDoc.Routes
                            from m in r.Methods
                            where (m.verb == "post" && r.Parameters.Count == 0) || 
                                  (m.verb == "put" || m.verb == "delete" || m.verb == "get") && r.Parameters.Count == 1
                         select new {
                                route = r.route,
                                parameter = r.Parameters,
                                verb = m.verb,
                                schema = m.schema,
                                responces = m.Responses
                            };

            foreach(var r in routes)
            {
                switch(r.verb)
                {
                    case "post":
                        Post[r.route] = param => postFx(param, r.schema);
                        break;
                    case "get":
                        Get[r.route] = param => getFx(param, r.parameter.Single());
                        break;

                }
            }


        }

        private Response getFx(DynamicDictionary parameters, string parameterName)
        {
            string responseString;
            dataStorage.TryGetValue(parameters[parameterName], out responseString);

            Response response = responseString;
            response.StatusCode = (HttpStatusCode) 200;

            return response;
        }

        private Response postFx(DynamicDictionary parameters, JSchema schema)
        {
            string requestString = Request.Body.AsString();
            JObject requestJson = JObject.Parse(requestString);
            bool valid = requestJson.IsValid(schema);

            dataStorage.TryAdd(requestJson.GetValue("id").ToString(), requestString);
            Response response = requestString;
            response.StatusCode = HttpStatusCode.OK;

            return response;
        }
    }
}
