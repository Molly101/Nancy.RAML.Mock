using System.Collections.Generic;

namespace NancyRAMLMock.RAMLParsing
{
    public interface IRamlMethod
    {
        string Verb { get; }

        string MediaType { get; }

        bool IsMTJson { get; }

        string JsonSchema { get; }

        IList<IRamlResponse> RamlResponses { get; }
    }
}
