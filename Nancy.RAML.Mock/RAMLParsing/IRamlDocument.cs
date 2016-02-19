using System;
using System.Collections.Generic;

namespace NancyRAMLMock.RAMLParsing
{
    public interface IRamlDocument
    {
        Uri BaseUri { get; }

        IList<IRamlResource> RamlResources { get; }
    }
}
