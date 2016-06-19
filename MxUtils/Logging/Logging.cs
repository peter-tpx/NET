using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;


namespace MxUtils.Logging
{
   public interface ILogging
   {
      //event EventHandler<LogEntryArgs> OnLogEntry;

      void LogEntry(string psModule, string psMessage);

   }

   // -- Error Logs 

   public class LogEntryArgs : EventArgs
   {
      public string sModule { get; set; }
      public string sMessage { get; set; }

      public LogEntryArgs(string psModule, string psMessage)
      {
         sMessage = psMessage;
         sModule = psModule;
      }

   }

   [Flags]
   public enum XLogInfo
   {
      LogDate = 0x80,
      LogTime = 0x40,
      LogMSec = 0x20,
      System = 0x08,
      User = 0X04,
      Level = 0x02,
      Module = 0x01
   }

   public enum XLogIndent : int
   {
      Main = 1,
      Sub = 2,
      Detail = 3
   }

   public enum XLogLevel : int
   {
      Unknown = -1,
      None = 0,
      Error = 1,
      Info = 3,
      Warning = 6,
      Debug = 9
   }


   //TEST - Create queued log buffer to resolve "jamming"
   // -- singleton - self instantiating
   public class XLogging
   {
      const long DEFAULT_MAX_LOG_RECORDS = 60000;

      //static XLogging _Ptr = null;

      //PRIVATE:
      string _sLogFile = "";
      string _sLogPath = "";
      string _sLogFilePfx = "";

      long _lLogRec = 0;
      long _lMaxLogRec = DEFAULT_MAX_LOG_RECORDS;

      object _LogSync = new object();
      bool _bRunningFlag = false;
      bool _bLoggingEnabled = false;

     // string _sRowPrefix = "";

      XLogInfo _xlogInfo;

      XLogLevel _xlogLevel = XLogLevel.Warning;

      Queue<string> _queueLog = new Queue<string>();

      Thread _bwEventProcessor = null;



      ////CONSTRUCTOR:
      //static XLogging()
      //{
      //   if (_Ptr == null)
      //      _Ptr = new XLogging();
      //}

      //public static XLogging Log { get { return _Ptr; } }


      public void Init(string psPath, string psLogFilePfx, XLogLevel pLevel, XLogInfo pLogInfo)
      {
         _xlogLevel = pLevel;
         _xlogInfo = pLogInfo;

         SetLogPath(psPath, psLogFilePfx);
      }

      public void Write(string psModule, string psMessage)
      {
         if ((int)XLogLevel.Info <= (int)_xlogLevel)
         {
            string sFmtMsg = GenerateRowPrefix(_xlogInfo, XLogLevel.Info, "") + GetIndent((int)XLogIndent.Main) + psMessage;

            EnqueueLogEntry(sFmtMsg);
         }
      }


      public void WriteExceptionLogEntry(string psModule, Exception ex)
      {
          try
          {
              WriteERROR(0, psModule, "Error: " + ex.Message);
              WriteERROR(1, psModule, ex.Source);
              WriteERROR(1, psModule, ex.StackTrace);

              //TODO -- Add inner exception
          }
          catch { }
      }
 

      //public void WriteExceptionLogEntry(int piDetail, string psModule, Exception ex)
      //{
      //    try
      //    {
      //        if (_bLoggingEnabled)
      //        {
      //            if ((int)piDetail <= (int)_iLevel)
      //            {

      //                string sUserID = GetCurrentUserID();
      //                string sDetLabel = GetDetailLabel(piDetail);

      //                string sFmtMsg = string.Format("{0:HH:mm:ss}.{1:000} {2,-10} [{4,-5}] {3,-50} : ", DateTime.Now, DateTime.Now.Millisecond, sUserID, psModule, sDetLabel);

      //                //                  string sFmtMsg = string.Format("{0:hh:mm:ss}.{1:000} - {2,-10} - {3,-50} - ", DateTime.Now, DateTime.Now.Millisecond, sUserID, psModule);

      //                EnqueueLogEntry(string.Format("{0} Error:{1}", sFmtMsg, ex.Message));
      //                EnqueueLogEntry(string.Format("{0} Source:{1}", sFmtMsg, ex.Source));
      //                EnqueueLogEntry(string.Format("{0} Stack:{1}", sFmtMsg, ex.StackTrace));

      //                if (ex.Data != null)
      //                {
      //                    if (ex.Data.Count > 0)
      //                    {
      //                        EnqueueLogEntry(string.Format("{0} --- Exception Data:", sFmtMsg));

      //                        for (int i = 0; i < ex.Data.Count; i++)
      //                            EnqueueLogEntry(string.Format("{0} Data#{1}: {2}", sFmtMsg, i, ex.Data));
      //                    }
      //                }

      //                if (ex.InnerException != null)
      //                {
      //                    EnqueueLogEntry(string.Format("{0} --- Inner Exception:", sFmtMsg));

      //                    EnqueueLogEntry(string.Format("{0} Inner Error:{1}", sFmtMsg, ex.InnerException.Message));
      //                    EnqueueLogEntry(string.Format("{0} Inner Source:{1}", sFmtMsg, ex.InnerException.Source));
      //                    EnqueueLogEntry(string.Format("{0} Inner Stack:{1}", sFmtMsg, ex.InnerException.StackTrace));
      //                }
      //            }
      //        }
      //    }
      //    catch { }

      //}


      public void Write(XLogLevel pLevel, XLogIndent pIndent, string psModule, string psMessage)
      {
         this.Write(pLevel, (int)pIndent, psModule, psMessage);
      }

      public void Write(XLogLevel pLevel, int pIndent, string psModule, string psMessage)
      {
         if ((int)pLevel <= (int)_xlogLevel)
         {
            string sFmtMsg = GenerateRowPrefix(_xlogInfo, pLevel, "") + GetIndent(pIndent) + psMessage;

            EnqueueLogEntry(sFmtMsg);
         }
      }

      public void WriteINFO(int pIndent, string psModule, string psMessage)
      {
         if ((int)XLogLevel.Info <= (int)_xlogLevel)
         {
            string sFmtMsg = GenerateRowPrefix(_xlogInfo, XLogLevel.Info, "") + GetIndent(pIndent) + psMessage;

            EnqueueLogEntry(sFmtMsg);
         }
      }


      public void WriteDEBUG(int pIndent, string psModule, string psMessage)
      {
         if ((int)XLogLevel.Debug <= (int)_xlogLevel)
         {
            string sFmtMsg = GenerateRowPrefix(_xlogInfo, XLogLevel.Debug, "") + GetIndent(pIndent) + psMessage;

            EnqueueLogEntry(sFmtMsg);
         }
      }

      public void WriteERROR(int pIndent, string psModule, string psMessage)
      {
         if ((int)XLogLevel.Error <= (int)_xlogLevel)
         {
            string sFmtMsg = GenerateRowPrefix(_xlogInfo, XLogLevel.Error, "") + GetIndent(pIndent) + psMessage;

            EnqueueLogEntry(sFmtMsg);
         }
      }

      private string GetIndent(int pIndent)
      {
         string sIndent = "";
         if (pIndent == (int)XLogIndent.Main)
            sIndent = string.Empty;

         if (pIndent == (int)XLogIndent.Sub)
            sIndent = "   ";


         if (pIndent == (int)XLogIndent.Detail)
            sIndent = "     ";

         return sIndent;
      }

      public void WriteLogEntry(object sender, LogEntryArgs e)
      {
         try
         {
            if (_bLoggingEnabled)
            {
               string sUserID = System.Security.Principal.WindowsIdentity.GetCurrent().Name.ToString();
               string sFmtMsg = string.Format("{0:HH:mm:ss}.{1:000} - {2,-15} - {3}", DateTime.Now, DateTime.Now.Millisecond, sUserID, e.sMessage);

               EnqueueLogEntry(sFmtMsg);
            }
         }
         catch { }
      }

      public void WriteLogEntry(string psModule, string psMessage)
      {
         try
         {
            if (_bLoggingEnabled)
            {
               //string sUserID = GetCurrentUserID();
               //string sFmtMsg = string.Format("{0:HH:mm:ss}.{1:000} - {2,-15} - {3,-15} - {4}", DateTime.Now, DateTime.Now.Millisecond, sUserID, psModule, psMessage);

               string sFmtMsg = GenerateRowPrefix(_xlogInfo, XLogLevel.Info, psModule);

               EnqueueLogEntry(sFmtMsg);
            }
         }
         catch { }
      }


      public void WritePlainLogEntry(string psMessage)
      {
         try
         {
            if (_bLoggingEnabled)
            {
               //string sUserID = GetCurrentUserID();
               //string sFmtMsg = string.Format("{0:HH:mm:ss}.{1:000} - {2,-15} - {3}", DateTime.Now, DateTime.Now.Millisecond, sUserID, psMessage);

               string sFmtMsg = string.Format("{0:yyyy-MM-dd} {1:HH:mm:ss} {2} ", DateTime.Today, DateTime.Now, psMessage);


               EnqueueLogEntry(sFmtMsg);
            }
         }
         catch { }
      }

      public void WriteLogEntry(string psMessage)
      {
         try
         {
            if (_bLoggingEnabled)
            {
               //string sUserID = GetCurrentUserID();
               //string sFmtMsg = string.Format("{0:HH:mm:ss}.{1:000} - {2,-15} - {3}", DateTime.Now, DateTime.Now.Millisecond, sUserID, psMessage);

               string sFmtMsg = GenerateRowPrefix(_xlogInfo, XLogLevel.Info, "") + psMessage;


               EnqueueLogEntry(sFmtMsg);
            }
         }
         catch { }
      }


      public void StartLogging()
      {

         _bLoggingEnabled = true;

         if (!_bRunningFlag)
         {
             if (_bAsyncLogFlag)
             {
                 _bwEventProcessor = new Thread(new ThreadStart(ProcessLoggingLoop));
                 _bwEventProcessor.Start();
             }
             _bRunningFlag = true;
         }

         _lLogRec = 0;
         // -- alternative method - using system timer
         //if (!_bRunningFlag)
         //{
         //   _timerProcess = new Timer(new TimerCallback(ProcessLog), _synchEvent, 100, 100);
         //   _bRunningFlag = true;
         //}

         WritePlainLogEntry(" ----------------------------- Logging Started --------------------------- ");

      }

      public void StopLogging()
      {
         WritePlainLogEntry(" ----------------------------- Logging Stopped --------------------------- ");

         //         _timerProcess.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
         if(_bAsyncLogFlag)
          _bwEventProcessor.Abort();

         _bLoggingEnabled = false;
         _bRunningFlag = false;

         // -- clear all unwritten entries
         CleanUpLog();

      }

      #region --- Private Methods ---

      private string GetCurrentUserID()
      {
         string sUserID = "";
         try
         {
            sUserID = System.Security.Principal.WindowsIdentity.GetCurrent().Name.ToString();
            int iPos = sUserID.IndexOf('\\'); // -- domain name is specified
            if (iPos >= 0)
               sUserID = sUserID.Substring(iPos + 1);
         }
         catch { }
         return sUserID;
      }

      private string GenerateRowPrefix(XLogInfo pInfoParams, XLogLevel pLevel, string psModule)
      {
         StringBuilder sb = new StringBuilder();


         if ((pInfoParams & XLogInfo.LogDate) == XLogInfo.LogDate)
            sb.AppendFormat("{0:yyyy-MM-dd}", DateTime.Today);

         if ((pInfoParams & XLogInfo.LogTime) == XLogInfo.LogTime)
         {
            if (sb.Length > 0) sb.Append(" ");
            sb.AppendFormat("{0:HH:mm:ss}", DateTime.Now);
         }

         if ((pInfoParams & XLogInfo.LogMSec) == XLogInfo.LogMSec)
         {
            if (sb.Length > 0) sb.Append(".");
            sb.AppendFormat("{0:000}", DateTime.Now.Millisecond);
         }

         if ((pInfoParams & XLogInfo.User) == XLogInfo.User)
         {
            if (sb.Length > 0) sb.Append(" - ");
            sb.AppendFormat("{0:-15}", GetCurrentUserID());
         }

         //TODO -- add session info
         if ((pInfoParams & XLogInfo.Level) == XLogInfo.Level)
         {
            if (sb.Length > 0)
               sb.AppendFormat(" [{0:-10}]", pLevel.ToString());
            else
               sb.AppendFormat("{0:-10}", pLevel.ToString());
         }

         if ((pInfoParams & XLogInfo.Module) == XLogInfo.Module)
         {
            if (sb.Length > 0) sb.Append(" - ");
            sb.AppendFormat("{0:-15}", psModule);
         }

         return sb.ToString();
      }

      private void SetLogPath(string psPath, string psLogFilePfx)
      {

         if (psLogFilePfx != String.Empty)
         {
            _sLogFilePfx = psLogFilePfx;

            _sLogFile = psPath + "\\" + _sLogFilePfx + string.Format("_{0:yyyy_MM_dd_HH_mm}", DateTime.Now) + ".log";
            //            _sLogFile = psPath + "\\" + psLogFilePfx + string.Format("_{0:yyyy-MM-dd}", DateTime.Today) + "_" + GetCurrentUserID() + ".log";
         }
         else
            _sLogFile = psPath + "\\Log" + string.Format("_{0:yyyy_MM_dd_HH_mm}", DateTime.Now) + ".log";

         _sLogPath = psPath;

      }

      private void ResetLogName()
      {
         if (_sLogFilePfx != String.Empty)
         {
            _sLogFile = _sLogPath + "\\" + _sLogFilePfx + string.Format("_{0:yyyy_MM_dd_HH_mm}", DateTime.Now) + ".log";
            //            _sLogFile = psPath + "\\" + psLogFilePfx + string.Format("_{0:yyyy-MM-dd}", DateTime.Today) + "_" + GetCurrentUserID() + ".log";
         }
         else
            _sLogFile = _sLogPath + "\\Log" + string.Format("_{0:yyyy_MM_dd_HH:mm}", DateTime.Now) + ".log";

      }

      bool _bAsyncLogFlag = false;

      private void EnqueueLogEntry(string psMessage)
      {

          if (!_bAsyncLogFlag)
          {
              WriteLog2Disk(psMessage);
          }
          else
          {
              // -- queue the request
              lock (_queueLog)
              {
                  //++_lLogRec;

                  //if (_lLogRec > _lMaxLogRec)
                  //{
                  //   if (_sLogFilePfx != String.Empty)
                  //   {
                  //      _sLogFile = _sLogPath + "\\" + _sLogFilePfx + string.Format("_{0:yyyy-MM-dd_hh_mm}", DateTime.Now) + ".log";
                  //      //            _sLogFile = psPath + "\\" + psLogFilePfx + string.Format("_{0:yyyy-MM-dd}", DateTime.Today) + "_" + GetCurrentUserID() + ".log";
                  //   }
                  //   else
                  //      _sLogFile = _sLogPath + "\\Log" + string.Format("_{0:yyyy-MM-dd_hh:mm}", DateTime.Now) + ".log";

                  //   _lLogRec = 0;

                  //}

                  _queueLog.Enqueue(psMessage);

                  Monitor.Pulse(_queueLog);
              }

              if (_bRunningFlag)
              {
                  if (_bwEventProcessor.ThreadState == ThreadState.Stopped)
                  {
                      _bwEventProcessor.Start();
                  }
              }

              // -- Alternative mechanism
              //if (!_bRunningFlag)
              //{
              //   _timerProcess = new Timer(new TimerCallback(ProcessLog), _synchEvent, 100, 100);
              //   _bRunningFlag = true;
              //}
          }
      }

      //      Thread _threadProcessor = null;


      private void ProcessLoggingLoop()
      {

         while (_bRunningFlag)
         {
            if (Monitor.TryEnter(_queueLog, 100))
            {
               if (_queueLog.Count == 0)
                  Monitor.Wait(_queueLog);

               Queue<string> procQueue = new Queue<string>();

               while (_queueLog.Count > 0)
               {
                  procQueue.Enqueue(_queueLog.Dequeue());
               }

               Monitor.Exit(_queueLog);

               while (procQueue.Count > 0)
               {
                  string sMsg = procQueue.Dequeue();

                  byte iFailCount = 0;

                  while (!(WriteLog2Disk(sMsg) || iFailCount > 5))
                  {
                     iFailCount++;
                     // -- wait and attempt to repeat the operation
                     Thread.Sleep(200);
                  }
               }

            }

         }
      }

      //private void ProcessLog(Object state)
      //{

      //   if (Monitor.TryEnter(_queueLog,100))
      //   {
      //      if (_bRunningFlag)
      //         _timerProcess.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);

      //      Queue<string> procQueue = new Queue<string>();

      //      while (_queueLog.Count > 0)
      //      {
      //         procQueue.Enqueue(_queueLog.Dequeue());
      //      }

      //      // release the queue - to make it available for new entires
      //      Monitor.Exit(_queueLog);  


      //      while (procQueue.Count>0)
      //      {
      //         string sMsg = procQueue.Dequeue();

      //         byte iFailCount = 0;

      //         while (!(WriteLog2Disk(sMsg) || iFailCount > 5))
      //         {
      //            iFailCount++;
      //            // -- wait and attempt to repeat the operation
      //            Thread.Sleep(200);  
      //         }

      //      }

      //      if (_bRunningFlag)
      //         _timerProcess.Change(100, 100);
      //   }
      //}


      private void CleanUpLog()
      {
         lock (_queueLog)
         {
            while (_queueLog.Count > 0)
            {
               string sMsg = _queueLog.Dequeue();
               Console.WriteLine("CleanUpLog - Writing Message:{0}", sMsg);

               WriteLog2Disk(sMsg);
            }
         }

      }


      private bool WriteLog2Disk(string psMessage)
      {
         bool bRet = false;
         //TEST ONLY     return false;
         try
         {
            if (_bLoggingEnabled)
            {

               lock (_LogSync)
               {
                  System.IO.StreamWriter file = new System.IO.StreamWriter(_sLogFile, true);
                  file.WriteLine(psMessage);
                  file.Close();
                  bRet = true;
               }
            }
         }
         catch (Exception ex)
         {
            bRet = false;
         }

         return bRet;
      }

      #endregion
   }




}

