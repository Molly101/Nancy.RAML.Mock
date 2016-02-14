using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NancyRAMLMock.RAMLParsing
{
    public interface IRamlResource
    {
        string Path { get; }

        IList<string> RamlParameters { get; }

        IList<IRamlMethod> RamlMethods { get; }
    }
}
