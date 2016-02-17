using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NancyRAMLMock.Data
{
    /// <summary>
    /// Data Storage model
    /// </summary>
    public class DataModel
    {
        public string jsonModel { get; set; }

        public string jsonSchema { get; set; }

        public string jsonQuery { get; set; }

        public string Path { get; set; }

        public bool operationSuccesfull { get; set; }
    }


}
