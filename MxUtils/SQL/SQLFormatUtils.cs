using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.OleDb;

namespace MxUtils.SQL
{
   public class SQLUtils
   {
      private enum EDriverType
      {
         Unknown,
         ADOMDB,
         ADOSQL
      }


 
      public static string SQLFormat(object poValue)
      {
         //TODO - Complete the support for all value type (reuse SQLRoutines library)
         const string DELIM = "\'";
         string sRetVal = "";
         if (poValue is string)
            sRetVal = DELIM + (string)poValue + DELIM;
         else if (poValue == null)
            sRetVal = "null";

         else if (poValue is DateTime)
            sRetVal = string.Format("'{0:yyyy-MMM-dd hh:mm}'", poValue);
         else
            sRetVal = poValue.ToString();
         //WARNING - DATE Format is not yet implemented!!

         return sRetVal;
      }

      public static object CvtEmptyText2Null(string psValue)
      {
         if (psValue == "" || psValue == String.Empty)
            return null;
         else
            return psValue;
      }



      public static object IsNull(object poValue, object poDefault)
      {
         if (poValue == null)
         {
            if (poDefault == null)
            {
               if (poValue is string)
                  return "";
               else if (poValue is int)
                  return (int)0;
               else if (poValue is double)
                  return (int)0;
               else if (poValue is float)
                  return (int)0;
               else if (poValue is Int16)
                  return (int)0;
               else if (poValue is Int32)
                  return (int)0;
               else if (poValue is Int64)
                  return (int)0;
               else if (poValue is DateTime)
                  return (DateTime)(new DateTime());
               else if (poValue is object)
                  return (object)null;
               else
                  return null;
            }
            else
               return poDefault;
         }
         else
            return poValue;

      }

   }
}
