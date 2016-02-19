using Newtonsoft.Json.Schema;
using Raml.Parser.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NancyRAMLMock.RAMLParsing
{
    public sealed class RamlResourceWrappper: IRamlResource
    {
        public string Path { get; }
        private List<IRamlMethod> ramlMethods;
        public IList<IRamlMethod> RamlMethods => ramlMethods.AsReadOnly();
        private List<string> ramlParameters;
        public IList<string> RamlParameters => ramlParameters.AsReadOnly();

        public RamlResourceWrappper(string route, IEnumerable<Method> ramlMethods)
        {
            this.Path = route;

            ramlParameters = new List<string>();
            Regex rgx = new Regex(@"(?<=\{)\w+(?=\})");     //Extracting parameters from routes i.e. {id} -> id
            foreach (Match match in rgx.Matches(route))
                ramlParameters.Add(match.Value);

            this.ramlMethods = ParseRAMLMethods(ramlMethods);
        }

        public List<IRamlMethod> ParseRAMLMethods(IEnumerable<Method> ramlMethods)
        {
            var methodList = new List<IRamlMethod>();

            foreach (var method in ramlMethods)
                methodList.Add(new RamlMethodWrapper(method));

            return methodList;
        }
    }
}
