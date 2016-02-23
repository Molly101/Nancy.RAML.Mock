using Nancy;
using Nancy.Extensions;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using NLog;
using NancyRAMLMock.Data;
using System;
using System.Linq;
using NancyRAMLMock.RAMLParsing;

namespace NancyRAMLMock

{
    public class NANCYRestModule : NancyModule
    {
        private IDataStorage dataStorage;
        private ILogger logger;
        private IRamlDocument ramlDoc;

        public NANCYRestModule(IDataStorage dataStorage, ILogger logger, IRamlDocument ramlDoc)
        {
            this.dataStorage = dataStorage;
            this.logger = logger;
            this.ramlDoc = ramlDoc;

            //selecting only Post without parameter as the last element of the path  (i.e. /movies )
            //selecting Get, Put and Delete with a parameter (some kind of id) as the as last element of the path (i.e. /movies/{id} )
            var requests = from r in ramlDoc.RamlResources
                           from m in r.RamlMethods
                           where (m.Verb == "post" && r.Path.Last() != '}') ||
                                 (m.Verb == "put" || m.Verb == "delete" || m.Verb == "get") && r.Path.Last() == '}'
                           select new RequestDetails(r, m);


            foreach (var request in requests)
            {
                switch (request.Verb)
                {
                    case "post":
                        Post[request.Path] = param => processRequest(param, request, new Func<DataModel, DataModel>(dataStorage.Insert), new Func<DataModel, RequestDetails, Response>(processResp));
                        break;
                    case "get":
                        Get[request.Path] = param => processRequest(param, request, new Func<DataModel, DataModel>(dataStorage.Get), new Func<DataModel, RequestDetails, Response>(processResp));
                        break;
                    case "put":
                        Put[request.Path] = param => processRequest(param, request, new Func<DataModel, DataModel>(dataStorage.Update), new Func<DataModel, RequestDetails, Response>(processResp));
                        break;
                    case "delete":
                        Delete[request.Path] = param => processRequest(param, request, new Func<DataModel, DataModel>(dataStorage.Delete), new Func<DataModel, RequestDetails, Response>(processResp));
                        break;

                }
            }
        }

        private Response processResp(DataModel resultModel, RequestDetails requestDetails)
        {
            var response = new Response();
            if (resultModel.operationSuccesfull)
            {
                response.StatusCode = (HttpStatusCode)requestDetails.SuccessResponseCode;

                if (!String.IsNullOrEmpty(resultModel.jsonModel))
                {
                    response = resultModel.jsonModel;
                    response.ContentType = "application/json";
                }
            }
            else
            {
                response.StatusCode = HttpStatusCode.NotFound;
            };

            return response;
        }

        private Response processRequest(DynamicDictionary parameters, RequestDetails requestDetails, 
            Func<DataModel, DataModel> dataStorageDelegate, Func<DataModel, RequestDetails, Response> responseDelegate)
        {
            var reqDataModel = new DataModel() { Path = Request.Url };
            var response = new Response();

            //If request have parameters - construct the Path and jsonQuery for the Model (i.e. "/movies/{id}")
            if (requestDetails.Parameters != null && requestDetails.Parameters.Any())
            {
                string lastParName = requestDetails.Parameters.Last();                                  //we are assuming that the last parameter in request path is an "id" 
                string lastParValue = parameters[lastParName].Value;                                    //and we can use this parameter name and value to compose json filter for our dataStorage        

                reqDataModel.Path = reqDataModel.Path.Remove(reqDataModel.Path.Length - lastParValue.Length);                     //remove last parameter value from the "/movies/101" => "/movies/", where 101 is some "id"
                reqDataModel.jsonQuery = $"{{\"{lastParName}\":\"{lastParValue}\"}}";                                             // {"lastParameterName":"lastParameterValue"}
            }

            
            JObject reqBobyJsonObj = null;
            string reqBodyJsonStr = Request.Body.AsString();

            //if request body contains Json - we need to validate (try to parse) this Json first
            if (!String.IsNullOrEmpty(reqBodyJsonStr))
            {
                try
                {
                    reqBobyJsonObj = JObject.Parse(reqBodyJsonStr);
                }
                catch (Exception ex)
                {
                    logger.Error($"Incorrect JSON in {requestDetails.Verb.ToUpper()} request!");
                    logger.Error(ex);

                    response = "Bad Request";
                    response.StatusCode = HttpStatusCode.BadRequest;
                    return response;                                        
                }            
            }

            //If request have an associated schema - we need to validate received Json (from request body) with this schema
            if (!String.IsNullOrEmpty(requestDetails.Schema)) { 
                JSchema jsonSchema = JSchema.Parse(requestDetails.Schema);
                if (!reqBobyJsonObj?.IsValid(jsonSchema) ?? true)
                {
                    logger.Error($"JSON in {requestDetails.Verb.ToUpper()} request does not match the RAML schema!!!");
                    response.StatusCode = HttpStatusCode.UnprocessableEntity;
                    return response;
                }
            }

            reqDataModel.jsonModel = reqBodyJsonStr;
            var resultingDataModel = dataStorageDelegate(reqDataModel);
            response = responseDelegate(resultingDataModel, requestDetails);
                
            return response;
        }
      
    }
}
