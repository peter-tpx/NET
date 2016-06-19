using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MxUtils.SQL
{
    public static class SQLBuilder
    {
        // -- safe mode disallow UPDATE and DELETE statements w/o conditons
        public static bool SafeMode = false; 

        public static string  SELECT_stm(string table, string whereCond, params SQLField[] fields)
        {
            string sql = String.Empty;

            string sql1 = string.Empty;

            if (fields.Length > 0)
            {
                for (int i = 0; i < fields.Length; i++)
                    sql1 += (string.IsNullOrEmpty(sql1) ? "" : ",") + fields[i].Name;

                string sql2 = String.Empty;
                if (!string.IsNullOrEmpty(whereCond))
                    sql2 = " WHERE " + whereCond;

                sql = string.Format("SELECT {1} FROM {0} {2}", table, sql1, sql2);
            }

            return sql;
        }


        public static string INSERT_stm(string table, params SQLField[] fields)
        {

            string sql = String.Empty;

            string sql1 = string.Empty;
            string sql2 = string.Empty;

            if (fields.Length > 0)
            {
                for (int i = 0; i < fields.Length; i++)
                {
                    sql1 += (string.IsNullOrEmpty(sql1) ? "" : ",") + fields[i].Name;

                    sql2 += (string.IsNullOrEmpty(sql2) ? "" : ",") + fields[i].SQLValue;
                }

                sql = string.Format("INSERT INTO {0} ({1}) VALUES({2}) ", table, sql1, sql2);

            }
                 return sql;
        }


        public static string UPDATE_stm(string table, string whereCond, params SQLField[] fields)
        {

            string sql = String.Empty;

            string sql1 = string.Empty;

            if (fields.Length > 0)
            {
                for (int i = 0; i < fields.Length; i++)
                    sql1 += (string.IsNullOrEmpty(sql1) ? "" : ",")
                        + string.Format("{0}={1}", fields[i].Name, fields[i].SQLValue);

                string sql2 = string.Empty;
                if (!string.IsNullOrEmpty(whereCond))
                    sql2 = " WHERE " + whereCond;


                if (SafeMode && string.IsNullOrEmpty(whereCond))
                    return string.Empty;

                sql = string.Format("UPDATE {0} SET {1}{2} ", table,sql1,sql2);

            }
            return sql;
        }

        public static string UPDATE_stm(string table, SQLField whereID , params SQLField[] fields)
        {

            string sql = String.Empty;

            string sql1 = string.Empty;

            if (fields.Length > 0)
            {
                for (int i = 0; i < fields.Length; i++)
                    sql1 += (string.IsNullOrEmpty(sql1) ? "" : ",") + string.Format("{0}={1}", fields[i].Name, fields[i].SQLValue);

                string sql2 = string.Empty;

                if(whereID !=null)
                   sql2 =  " WHERE " + string.Format("{0}={1}", whereID.Name, whereID.SQLValue);

                if (SafeMode && whereID == null)
                    return string.Empty;

                sql = string.Format("UPDATE {0} SET {1}{2} ", table, sql1, sql2);

            }

            return sql;
        }

        public static string DELETE_stm(string table, SQLField whereID)
        {

            string sql = String.Empty;

            string sql1 = string.Empty;

            if (whereID != null)
                sql1 = " WHERE " + string.Format("{0}={1}", whereID.Name, whereID.SQLValue);

            if (SafeMode && whereID == null)
                return string.Empty;

            sql = string.Format("DELETE {0} {1} ", table, sql1);

            return sql;
        }

        public static string DELETE_stm(string table, string whereCond)
        {

            string sql = String.Empty;

            string sql1 = String.Empty;
            if (!string.IsNullOrEmpty(whereCond))
                sql1 = " WHERE " + whereCond;

            if (SafeMode && string.IsNullOrEmpty(whereCond))
                return string.Empty;

            sql = string.Format("DELETE {0} {1} ", table, sql1);

            return sql;
        }


        public static string WHERE_stm(params SQLField[] fields)
        {

            string sql = String.Empty;

            if (fields.Length > 0)
            {
                for (int i = 0; i < fields.Length; i++)
                {
                    sql += (string.IsNullOrEmpty(sql) ? "" : " AND ") + string.Format("{0}={1}", fields[i].Name, fields[i].SQLValue);
                }
            }

            return sql;
        }
   
    }

    public class SQLField
    {
        public string Name { get; private set; }
        public Type type {get; private set;}
        public object Value { get; set; }

        public string SQLValue
        {
            get
            {
                return GetSQLValue(Value, this.type);
            }
        }

        public SQLField(string name, object value)
        {
            this.Name = name;
            this.Value = value;
        }

        public static string GetSQLValue(object fldValue, Type fldType)
        {
            string sqlValue = String.Empty;
            string delim = "";

            if (fldValue == null)
                sqlValue = "NULL";                
            else
            {
                string sType = fldValue.GetType().FullName;

                switch (sType)
                {
                    case "System.Byte":
                        delim = "";

                        break;

                    case "System.Char":
                        delim = "";

                        break;

                    case "System.Int":
                    case "System.Int16":
                    case "System.Int32":
                        delim = "";

                        break;

                    case "System.Int64":
                    case "System.Long":
                        delim = "";

                        break;

                    case "System.Single":
                    case "System.Double":
                    case "System.Decimal":
                    case "System.Float":
                        delim = "";

                        break;

                    case "System.String":
                        delim = "\'";

                        break;

                    case "System.DateTime":
                        delim = "'";
                        fldValue = string.Format("{0:yyyy-MMM-dd HH:mm:ss}", fldValue);

                        break;

                    case "System.Boolean":
                        delim = "";
                        bool b = false;
                        if (bool.TryParse(fldValue.ToString(), out b))
                            fldValue = b ? 1 : 0;

                        break;

                    default:
                        delim = "";

                        break;
                }

                sqlValue = string.Format("{0}{1}{0}", delim, fldValue);
            }

            return sqlValue;
        }

    }
}
