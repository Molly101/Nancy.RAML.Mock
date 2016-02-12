using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RAMLNancyMock.Data
{

    /// <summary>
    /// Deletes all demos.
    /// </summary>
    public interface IDataStorage
    {
        void Insert(DataModel model);

        long Update(DataModel model);

        void Drop(DataModel model);

        long Delete(DataModel model);

        IList<DataModel> Get(DataModel model);
    }
}
