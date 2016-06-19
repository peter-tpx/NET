using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace MxUtils.Config
{

   public class ConfigValue
   {
      object _oValue = null;
      bool _bDefined = false;

      public ConfigValue(object poValue, bool pbDefined)
      {
         _oValue = poValue;
         _bDefined = pbDefined;
      }

      public int ToInt()
      {
         try { return (int)Convert.ToInt32(_oValue); }
         catch
         {
            throw new Exception("Conversion error - Incorrect value type!");
         }
      }

      public new string ToString()
      {
         try {
            if (_oValue == null)
               return String.Empty;

            return _oValue.ToString(); 
         }
         catch
         {
            throw new Exception("Conversion error - Incorrect value type!");
         }
      }

      public double ToDouble()
      {
         try { return Convert.ToDouble(_oValue); }
         catch
         {
            throw new Exception("Conversion error - Incorrect value type!");
         }
      }

      public long ToLong()
      {
         try { return (long)Convert.ToInt64(_oValue); }
         catch
         {
            throw new Exception("Conversion error - Incorrect value type!");
         }
      }

      public DateTime ToDateTime()
      {
         try
         {
            DateTime dtTemp = new DateTime();
            if (DateTime.TryParse(_oValue.ToString(), out dtTemp))
               return dtTemp;
            else
            {
               throw new Exception("Conversion error - Incorrect value type!");
            }
         }
         catch
         {
            throw new Exception("Conversion error - Incorrect value type!");
         }
      }

      public bool ToBool()
      {
         try
         {
            string sValue = _oValue.ToString().Trim().ToUpper();

            if (sValue == "1" || sValue == "-1" || sValue == "TRUE" || sValue == "T" || sValue == "YES")
               return true;
            else
               return false;
         }
         catch
         {
            throw new Exception("Conversion error - Incorrect value type!");
         }
      }

      public object Value { get { return _oValue; } }
      public bool bDefined { get { return _bDefined; } }

   }

   public static class ConfigRoutines
   {
      public static ConfigValue GetAppConfigValue(string psKey)
      {
         return GetAppConfigValue(psKey, false);
      }

      public static ConfigValue GetDLLAppConfigValue(object pThis, string psKey)
      {
         return GetDLLAppConfigValue(pThis, psKey, false);
      }

      public static ConfigValue GetDLLAppConfigValue(object pThis, string psKey, bool pbEmptyAsNull)
      {
         try
         {
            // -- adjustment for the DLL
            ConfigValue value = null;

            Configuration conf = ConfigurationManager.OpenExeConfiguration(pThis.GetType().Assembly.Location);
            object oTemp = conf.AppSettings.Settings[psKey].Value.ToString();
            // configuration does not exist
            if (oTemp == null)
               value = new ConfigValue(null, false);
            else
            {
               // -- allow use ?? test while assigning variables
               if (pbEmptyAsNull)
                  if (value.ToString().Trim() == String.Empty)
                     value = null;

               value = new ConfigValue(oTemp, true);
            }

            return value;

         }
         catch (Exception ex)
         {

            return null;
         }

      }

      public static ConfigValue GetAppConfigValue(string psKey, bool pbEmptyAsNull)
      {
         try
         {
            ConfigValue value = null;
           
            object oTemp = ConfigurationSettings.AppSettings[psKey];

            // configuration does not exist
            if (oTemp == null)
               value = new ConfigValue(null, false);
            else
            {
               // -- allow use ?? test while assigning variables
               if(pbEmptyAsNull)
                  if (value.ToString().Trim() == String.Empty)
                     value = null;

               value = new ConfigValue(oTemp, true);
            }

            return value;

         }
         catch (Exception ex)
         {

            return null;
         }

      }


      public static bool IsDLLAppConfigDefined(object pThis, string psKey)
      {
         try
         {
            Configuration conf = ConfigurationManager.OpenExeConfiguration(pThis.GetType().Assembly.Location);
            object oTemp = conf.AppSettings.Settings[psKey].Value.ToString();

            // configuration does not exist
            if (oTemp == null)
               return false;
            else
               return true;

         }
         catch
         {
            // -- other errors
            return false;
         }

      }

      public static bool IsDLLAppConfigDefined2(object pThis, string psKey, Type pType)
      {
         try
         {
            ConfigValue value = null;

            Configuration conf = ConfigurationManager.OpenExeConfiguration(pThis.GetType().Assembly.Location);
            object oTemp = conf.AppSettings.Settings[psKey].Value.ToString();

            // configuration does not exist
            if (oTemp == null)
               return false;
            else
               value = new ConfigValue(oTemp, true);


            // -- check type
            switch (pType.FullName.ToLower())
            {
               case "system.int32":
                  int itemp = value.ToInt();
                  break;

               case "system.datetime":
                  DateTime dttemp = value.ToDateTime();
                  break;

               case "system.long":
                  long ltemp = value.ToInt();
                  break;

               case "system.double":
                  double dtemp = value.ToDouble();
                  break;

               case "system.bool":
                  bool bTemp = value.ToBool();
                  break;

               default:

                  break;

            }

            return true;

         }
         catch
         {
            return false;
         }

      }

      public static bool IsAppConfigDefined2(string psKey)
      {
         try
         {
            object oTemp = ConfigurationSettings.AppSettings[psKey];

            // configuration does not exist
            if (oTemp == null)
               return false;
            else
               return true;

         }
         catch
         {
            // -- other errors
            return false;
         }
      }

      public static bool IsAppConfigDefined(string psKey, Type pType)
      {

         try
         {
            ConfigValue value = null;

            object oTemp = ConfigurationSettings.AppSettings[psKey];

            // configuration does not exist
            if (oTemp == null)
               return false;
            else
               value = new ConfigValue(oTemp, true);


            // -- check type
            switch (pType.FullName.ToLower())
            {
               case "system.int32":
                  int itemp = value.ToInt();
                  break;

               case "system.datetime":
                  DateTime dttemp = value.ToDateTime();
                  break;

               case "system.long":
                  long ltemp = value.ToInt();
                  break;

               case "system.double":
                  double dtemp = value.ToDouble();
                  break;

               case "system.bool":
                  bool bTemp = value.ToBool();
                  break;

               default:

                  break;

            }

            return true;

         }
         catch
         {
            return false;
         }

      }

   }

}
