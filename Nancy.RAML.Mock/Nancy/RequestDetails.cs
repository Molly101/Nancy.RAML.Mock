using NancyRAMLMock.RAMLParsing;
using System;
using System.Collections.Generic;

namespace NancyRAMLMock
{
    public class RequestDetails
    {
        public IRamlResource Resource { get; }
        public IRamlMethod Method { get; }

        public string Path => Resource.Path;
        public IList<string> Parameters => Resource.RamlParameters;
        public string Verb => Method.Verb;
        IList<IRamlResponse> Responses => Method.RamlResponses;
        public string Schema => (Method.IsMTJson) ? Method.JsonSchema : null;

        public RequestDetails(IRamlResource resource, IRamlMethod method)
        {
            Resource = resource;
            Method = method;
        }

        public int SuccessResponseCode
        {
            get
            {
                int respCode = 200;

                foreach (var r in Responses)
                {
                    if (Int32.TryParse(r.HttpReturnCode, out respCode) && respCode / 200 == 1)
                        break;
                    else
                        respCode = 200;
                }

                return respCode;
            }
        }

    }
}
