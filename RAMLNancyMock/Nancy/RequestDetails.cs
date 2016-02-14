using NancyRAMLMock.RAMLParsing;
using System.Collections.Generic;

namespace NancyRAMLMock
{
    public class RequestDetails
    {
        IRamlResource resource { get; }
        IRamlMethod method { get; }

        public string Path => resource.Path;
        public IList<string> Parameters => resource.RamlParameters;
        public string Verb => method.Verb;
        IList<IRamlResponse> Responces => method.RamlResponses;
        
        public RequestDetails(IRamlResource resource, IRamlMethod method)
        {
            this.resource = resource;
            this.method = method;
        }
    }
}
