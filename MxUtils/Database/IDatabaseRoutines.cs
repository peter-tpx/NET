using System;
using System.Data;

namespace MxUtils.Database
{
    public interface IDatabaseRoutines
    {
        bool CloseConnection();
        bool ExecuteNonQuery(string sql, out int rowsCount);
        bool LoadConfig();
        bool OpenConnection();
        bool ExecuteQuery(string sql, out IDataReader reader);
        bool FillDataSet(string sql, out global::System.Data.DataSet ds);
    }

}
