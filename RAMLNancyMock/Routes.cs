using Newtonsoft.Json.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RAMLNancyMock
{
    enum Verbs { delete, get, head, option, post, put, patch }; //Nancy supports the following methods DELETE, GET, HEAD, OPTIONS, POST, PUT and PATCH

    class Route
    {
        public readonly string _route;
        private List<Method> _methods;
        private List<string> _parameters;

        public Route(string route)
        {
            _route = route;
            _methods = new List<Method>();
            _parameters = new List<string>();

            Regex rgx = new Regex(@"(?<=\{)\w+(?=\})");     //Extracting parameters from routes i.e. {id} -> id
            foreach (Match match in rgx.Matches(route))
                _parameters.Add(match.Value);

        }

        public void AddMethod(Verbs verb, JSchema schema, string mediaType)
        {
           //TO DO
        }

        class Method
        {
            private Verbs verb;
            private JSchema schema;
            private string mediaType;
            List<Responce> reponces;
        }

        class Responce
        {
            private int returnCode;
            private string Description;
        }
    }

     
    
}
