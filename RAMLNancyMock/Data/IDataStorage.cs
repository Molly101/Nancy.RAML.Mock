using System.Collections.Generic;

namespace NancyRAMLMock.Data
{

    /// <summary>
    /// Database Storage interface
    /// </summary>
    public interface IDataStorage
    {
        DataModel Insert(DataModel model);

        DataModel Update(DataModel model);

        DataModel Drop(DataModel model);

        DataModel Delete(DataModel model);

        DataModel Get(DataModel model);

        IList<DataModel> GetMany(DataModel model);
    }
}
