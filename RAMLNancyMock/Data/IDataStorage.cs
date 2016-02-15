using System.Collections.Generic;

namespace NancyRAMLMock.Data
{

    /// <summary>
    /// Database Storage interface
    /// </summary>
    public interface IDataStorage
    {
        void Insert(DataModel model);

        DataModel Update(DataModel model);

        void Drop(DataModel model);

        bool Delete(DataModel model);

        DataModel Get(DataModel model);

        IList<DataModel> GetMany(DataModel model);
    }
}
