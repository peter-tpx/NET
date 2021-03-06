﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.OleDb;
using System.Data;

using System.Configuration;
using MxUtils.Config;

namespace MxUtils.Database
{
    /// <summary>
    /// Summary description for ODCBRoutines
    /// </summary>
    public class OleDbRoutines : IDatabaseRoutines
    {
        //PRIVATE:
        OleDbConnection _conn = null;
        string _sConn = String.Empty;

        public OleDbRoutines()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        public OleDbConnection Connection { get { return _conn; } }

        public bool LoadConfig()
        {
            try
            {

                if (ConfigRoutines.IsAppConfigDefined("connStr", typeof(string)))
                    _sConn = ConfigRoutines.GetAppConfigValue("connStr").ToString();

                if (string.IsNullOrEmpty(_sConn)) return false;

                _conn = new OleDbConnection(_sConn);

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
                using (OleDbCommand cmd = new OleDbCommand())
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


                using (OleDbDataAdapter da = new OleDbDataAdapter(sql, _conn))
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


                using (OleDbCommand cmd = new OleDbCommand())
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