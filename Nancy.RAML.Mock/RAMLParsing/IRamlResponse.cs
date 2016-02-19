using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NancyRAMLMock.RAMLParsing
{
    public interface IRamlResponse
    {
        string HttpReturnCode { get; }

        string Description { get; }

        string MediaType { get; }

        bool IsMTJson { get; }

        string JsonSchema { get; }
    }
}
