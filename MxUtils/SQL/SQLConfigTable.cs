using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.OleDb;

namespace MxUtils.SQL
{

    public class ControlTable
   {

      static ControlTable _instance = null;

      public static void CreateInstance(string psEnv)
      {
         if (_instance == null)
            _instance = new ControlTable(psEnv);
      }

      public static ControlTable Instance { get { return _instance; }}

      private class ControlTableItem
      {
         public string sMainKey { get; set; }
         public string sSubKey { get; set; }
         public string sData { get; set; }
         public string sDataType { get; set; }
         public string sKeyType { get; set; }
         public string sMode { get; set; }
      }

      string _sEnvironment = "*DEFAULT";

      List<ControlTable.ControlTableItem> _list = new List<ControlTableItem>();


      List<ControlTableItem> _SelItems = null;
      int _ItemIdx = -1;


      public ControlTable(string psEnv)
      {
         this.SetEnvironment(psEnv);
      }

      public void SetEnvironment(string psEnv)
      {
         _sEnvironment = string.IsNullOrEmpty(psEnv) ? "*DEFAULT" : psEnv;
      }

      public string GetFirstDataItem(string psMainKey, string psSubKey)
      {
         _SelItems = _list.FindAll(delegate(ControlTableItem it) { return it.sMainKey == psMainKey && it.sSubKey == psSubKey; });

         if (_SelItems.Count > 0)
         {
            _ItemIdx = 0;
            return _SelItems[_ItemIdx].sData;
         }

         return "";
      }

      public int GetDataItemsCount()
      {
         if (_SelItems != null)
            return _SelItems.Count;
         else
            return 0;
      }

      public string GetNextDataItem()
      {

         if (_SelItems != null)
            if (_ItemIdx < _SelItems.Count - 1)  // still some item left
            {
               _ItemIdx++;
               return _SelItems[_ItemIdx].sData;
            }

         return "";
      }

      public string[] GetDataItems()
      {
         if (_SelItems != null)
            return _SelItems.ConvertAll<string>(delegate(ControlTableItem c) { return c.sData; }).ToArray();

         return null;
      }

      public Dictionary<string, string> GetSubKeyItems(string psMainKey)
      {
         Dictionary<string, string> dic = new Dictionary<string, string>();

         List<KeyValuePair<string, string>> ls =
            _list.ConvertAll<KeyValuePair<string, string>>(
               delegate(ControlTableItem c) { return new KeyValuePair<string, string>(c.sSubKey, c.sData); });

         foreach (KeyValuePair<string, string> item in ls)
            dic.Add(item.Key, item.Value);

         return dic;

      }

      public string[,] GetSubKeyItemArray(string psMainKey)
      {
         List<ControlTableItem> ls1 = _list.FindAll(delegate(ControlTableItem c) { return c.sMainKey.ToUpper() == psMainKey.ToUpper(); });
         string[,] arr = new string[ls1.Count, 2];
         for (int i = 0; i < ls1.Count; i++)
         {
            arr[i, 0] = ls1[i].sSubKey;
            arr[i, 1] = ls1[i].sData;
         }


         return arr;
      }

      string _sTable = String.Empty;


      public void Load(OleDbConnection pConn, string psTable)
      {

         string sSQL = string.Format("SELECT * FROM {0} WHERE Environment=\'{1}\'",psTable, _sEnvironment); 
         OleDbCommand cmd = null;
         OleDbDataReader reader = null;

         _sTable = psTable;

         if (pConn != null)
         {
            try
            {
               if (pConn.State == ConnectionState.Closed)
                  pConn.Open();

               cmd = new OleDbCommand(sSQL, pConn);
               reader = cmd.ExecuteReader();

               while (reader.Read())
               {
                  ControlTableItem item = new ControlTableItem();
                  item.sMainKey = reader["MainKey"].ToString();
                  item.sSubKey = reader["SubKey"].ToString();
                  item.sData = reader["Data"].ToString();
                  item.sDataType = reader["DataType"].ToString();
                  item.sKeyType = reader["ControlKeyType"].ToString();
                  item.sMode = reader["Mode"].ToString();

                  _list.Add(item);
               }

            }
            catch (Exception ex)
            {

            }
            finally
            {
               reader.Close();
               reader.Dispose();
               cmd.Dispose();
            }
         }
      }
   }
}


