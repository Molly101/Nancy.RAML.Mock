using NancyRAMLMock.RAMLParsing;
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
        IList<IRamlResponse> Responces => Method.RamlResponses;
        public string Schema => (Method.IsMTJson) ? Method.JsonSchema : null;
        
        public RequestDetails(IRamlResource resource, IRamlMethod method)
        {
            Resource = resource;
            Method = method;
        }
    }
}
