using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Odbc;
using System.Data;

using System.Configuration;
using MxUtils.Config;

namespace MxUtils.Database
{
    /// <summary>
    /// Summary description for ODCBRoutines
    /// </summary>
    public class ODBCRoutines : IDatabaseRoutines
    {
        //PRIVATE:
        OdbcConnection _conn = null;
        string _sConn = String.Empty;

        public ODBCRoutines()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        public OdbcConnection Connection { get { return _conn; } }

        public bool LoadConfig()
        {
            try
            {

                if (ConfigRoutines.IsAppConfigDefined("connStr", typeof(string)))
                    _sConn = ConfigRoutines.GetAppConfigValue("connStr").ToString();

                if (string.IsNullOrEmpty(_sConn)) return false;

                _conn = new OdbcConnection(_sConn);

                return true;

            }
            catch (Exception ex)
            {

            }

            return false;

        }

        
 
        public bool ExecuteQuery(string sql, out IDataReader reader)
        {
            reader = null;

            try
            {
                using (OdbcCommand cmd = new OdbcCommand())
                {
                    cmd.Connection = _conn;
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = sql;

                    reader = cmd.ExecuteReader();
                }

                return true;
            }
            catch (Exception ex)
            {

            }

            return false;

        }

        public bool FillDataSet(string sql, out DataSet ds)
        {
            ds = new DataSet();

            try
            {
                if (_conn == null) return false;

                if (string.IsNullOrEmpty(sql)) return false;


                using (OdbcDataAdapter da = new OdbcDataAdapter(sql, _conn))
                {
                    da.Fill(ds, "default_table");
                }

                return true;
            }
            catch (Exception ex)
            {

            }

            return false;
        }

        public bool ExecuteNonQuery(string sql, out int rowsCount)
        {
            rowsCount = 0;

            try
            {
                if (_conn == null) return false;

                if (string.IsNullOrEmpty(sql)) return false;


                using (OdbcCommand cmd = new OdbcCommand())
                {
                    cmd.Connection = _conn;
                    cmd.CommandText = sql;
                    cmd.CommandType = CommandType.Text;
                }

            }
            catch (Exception ex)
            {

            }

            return false;
        }


        public bool OpenConnection()
        {
            try
            {
                if (_conn == null) return false;

                if (_conn.State == ConnectionState.Closed)
                    _conn.Open();

                return true;
            }
            catch (Exception ex)
            {

            }

            return false;
        }


        

        public bool CloseConnection()
        {
            try
            {
                if (_conn == null) return false;

                if (_conn.State != ConnectionState.Closed)
                    _conn.Close();

                return true;
            }
            catch (Exception ex)
            {

            }

            return false;
        }


    }
}