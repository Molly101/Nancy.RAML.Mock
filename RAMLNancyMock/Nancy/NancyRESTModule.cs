using Nancy;
using Nancy.Extensions;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using NLog;
using NancyRAMLMock.Data;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                switch(request.Verb)
                {
                    case "post":
                        Post[request.Path] = param => processRequest(param, request,  dataStorage.Insert);
                        break;
                    //case "get":
                    //    Get[request.Path] = param => getFx(param, request);
                    //    break;
                    //case "put":
                    //    Put[request.Path] = param => putFx(param, request);
                    //    break;
                    //case "delete":
                    //    Delete[request.Path] = param => deleteFx(param, request);
                    //    break;

                }
            }
        }

        private Response upsertRes(DataModel resultModel, int successCode)
        {
            var response = new Response()
            {
                ContentType = "application/json",
                StatusCode = (HttpStatusCode)successCode
            };

            response = resultModel.jsonModel;

           return response;
        }


        //private Response getFx(DynamicDictionary parameters, RequestDetails request)
        //{
        //    string lastParName = request.Parameters.Last();                                     //we are taking last parameter as an "id" 
        //    string lastParValue = parameters[lastParName].Value;                                //and we will use this parameter name and its value to search in dataStorage

        //    var res = dataStorage.Get(new DataModel {
        //        Path = Request.Path.Replace($"{lastParValue}", ""),                             // "/movies/101" => "/movies/", where 101 is some "id"
        //        jsonQuery = $"{{\"{lastParName}\":\"{lastParValue}\"}}"                         // {"lastParameterName":"lastParameterValue"}
        //    });

        //    Response response = new Nancy.Response();
        //    if (res != null)
        //    {
        //        response = res.jsonModel;
        //        response.ContentType = "application/json";
        //        response.StatusCode = (HttpStatusCode)request.SuccessResponseCode;
        //    }
        //    else
        //    { 
        //        response.StatusCode = HttpStatusCode.NotFound;                                  
        //    }
        //    return response;
        //}

        //private Response putFx(DynamicDictionary parameters, RequestDetails request)
        //{
        //    string lastParName = request.Parameters.Last();
        //    string lastParValue = parameters[lastParName].Value;

        //    string requestString = Request.Body.AsString();
        //    var response = new Response();

        //    JObject requestJson = null;
        //    try
        //    {
        //        requestJson = JObject.Parse(requestString);
        //    }
        //    catch (Exception ex)
        //    {
        //        logger.Error("Incorrect JSON in PUT request!");
        //        logger.Error(ex);

        //        response = "Bad Request";
        //        response.StatusCode = HttpStatusCode.BadRequest;
        //        return response;
        //    }

        //    JSchema jsonSchema = (request.Schema != null) ? JSchema.Parse(request.Schema) : null;
        //    bool valid = (jsonSchema != null && requestJson != null) ? requestJson.IsValid(jsonSchema) : false;

        //    if (valid)
        //    { 
        //        var res = dataStorage.Update(new DataModel
        //        {
        //            Path = Request.Path.Replace($"{lastParValue}", ""),
        //            jsonQuery = $"{{\"{lastParName}\":\"{lastParValue}\"}}",
        //            jsonModel = requestString
        //        });

        //        if (res != null)
        //        {
        //            response = res.jsonModel;
        //            response.ContentType = "application/json";
        //            response.StatusCode = (HttpStatusCode) request.SuccessResponseCode;
        //        }
        //        else
        //        {
        //            response.StatusCode = HttpStatusCode.NotFound;
        //        }
        //    }
        //    else
        //    {
        //        logger.Error("JSON in PUT request does not match RAML schema!!!");
        //        response.StatusCode = HttpStatusCode.UnprocessableEntity;
        //    }

        //    return response;
        //}

        //private Response deleteFx(DynamicDictionary parameters, RequestDetails request)
        //{
        //    string lastParName = request.Parameters.Last();
        //    string lastParValue = parameters[lastParName].Value;

        //    var res = dataStorage.Delete(new DataModel
        //    {
        //        Path = Request.Path.Replace($"{lastParValue}", ""),
        //        jsonQuery = $"{{\"{lastParName}\":\"{lastParValue}\"}}",
        //    });

        //    return (res) ? (HttpStatusCode)request.SuccessResponseCode : HttpStatusCode.NotFound;
        //}


        private Response processRequest(DynamicDictionary parameters, RequestDetails requestDetails, 
            Func<DataModel, DataModel> dataStorageDelegate) //, Func<DataModel, int, Response> ResponseFromDBOpResult
        {
            var dataModel = new DataModel() { Path = Request.Path };
            var response = new Response();

            //If request have parameters - construct the Path and jsonQuery for the Model (i.e. "/movies/{id}")
            if (requestDetails.Parameters != null && requestDetails.Parameters.Any())
            {
                string lastParName = requestDetails.Parameters.Last();                                  //we are assuming that the last parameter in request path is an "id" 
                string lastParValue = parameters[lastParName].Value;                                    //and we can use this parameter name and value to compose json filter for our dataStorage        

                dataModel.Path.Remove(dataModel.Path.Length - lastParValue.Length);                     //remove last parameter value from the "/movies/101" => "/movies/", where 101 is some "id"
                dataModel.jsonQuery = $"{{\"{lastParName}\":\"{lastParValue}\"}}";                      // {"lastParameterName":"lastParameterValue"}
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

            dataModel.jsonModel = reqBodyJsonStr;

            return ResponseFromDBOpResult(dataStorageDelegate(dataModel), requestDetails.SuccessResponseCode);
        }
      
    }
}
