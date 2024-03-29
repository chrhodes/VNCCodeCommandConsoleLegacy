﻿using System;
using System.Threading;

using VNC;

using ADSTA = VNCCodeCommandConsole.Data.ApplicationDataSetTableAdapters;

namespace VNCCodeCommandConsole.Data
{

    partial class ApplicationDataSet
    {
        private static int BASE_ERRORNUMBER = ErrorNumbers.APPERROR;
        private const string LOG_APPNAME = Common.LOG_APPNAME;

        // This is used to single thread access to the dataset
        static readonly object _concurrencyLock = new object();

        public ADSTA.ApplicationUsageTableAdapter ApplicationUsageTA { get; set; }

        Data.ApplicationDataSetTableAdapters.TableAdapterManager _taManager = null;

        public Data.ApplicationDataSetTableAdapters.TableAdapterManager TaManager
        {
            get
            {
                if (_taManager == null)
                {
                    try
                    {
                        // Create the TableAdapterManager.  
                        // This enables the cascading updates/deletes of the dataset to work.
                        _taManager = new ADSTA.TableAdapterManager();

                        #region Create the TableAdapters for all the tables

                        ApplicationUsageTA = new ADSTA.ApplicationUsageTableAdapter();

                        #endregion

                        #region Hook the table adapters to the table manager

                        _taManager.ApplicationUsageTableAdapter = ApplicationUsageTA;

                        #endregion

                        #region And update all the connection strings.

                        _taManager.Connection.ConnectionString = Config.ConnectionString;

                        ApplicationUsageTA.Connection.ConnectionString = Config.ConnectionString;

                        #endregion
                    }
                    catch (Exception ex)
                    {
                        //Log.Error(string.Format("ConnectionString:>{0}<", Config.SQLMonitorDBConnection), LOG_APPNAME, CLASS_BASE_ERRORNUMBER + 1);
                        //Log.Error(ex, LOG_APPNAME, CLASS_BASE_ERRORNUMBER + 2);
                    }
                }

                return _taManager;
            }
            set
            {
                _taManager = value;
            }
        }

        public void LoadApplicationDataSetFromDB(Data.ApplicationDataSet applicationDS)
        {
#if TRACE
            long startTicksTotal = Log.Trace("Enter", LOG_APPNAME, BASE_ERRORNUMBER + 3);
#endif
            try
            {
                long startTicks = 0;

                Log.Info("Clearing ApplicationDataSet...", LOG_APPNAME);
                applicationDS.Clear();
                Common.DataFullyLoaded = false;

                LoadMainTables(applicationDS);

                // Load the rest of the tables

                Thread t = new Thread(() => LoadTablesInBackGround(applicationDS));
                t.Start();

            }
            catch (Exception ex)
            {
                Log.Error(string.Format("ConnectionString:>{0}<", Config.ConnectionString), LOG_APPNAME, BASE_ERRORNUMBER + 60);
                Log.Error(ex, LOG_APPNAME, BASE_ERRORNUMBER + 61);
            }

#if TRACE
            Log.Trace("End", LOG_APPNAME, BASE_ERRORNUMBER + 62, startTicksTotal);
#endif
        }

        /// <summary>
        /// Load the tables that are most often used.
        /// </summary>
        /// <param name="applicationDS"></param>
        private void LoadMainTables(ApplicationDataSet applicationDS)
        {
            lock (_concurrencyLock)
            {
                TaManager.ApplicationUsageTableAdapter.Fill(applicationDS.ApplicationUsage);
            }
        }

        private void LoadTablesInBackGround(Data.ApplicationDataSet applicationDS)
        {
            //// Might be able to do this in parallel after we figure out locking.
            //// For now just do serially.

            //Thread t1 = new Thread(() => LoadInstanceContentTables(applicationDS));
            //t1.Start();

            //Thread t2 = new Thread(() => LoadSnapShotTables(applicationDS));
            //t2.Start();

            //Thread t3 = new Thread(() => LoadDBContentTables(applicationDS));
            //t3.Start();

            //Thread t4 = new Thread(() => LoadJobServerTables(applicationDS));
            //t4.Start();

            Common.DataFullyLoaded = true;
        }

        partial class ApplicationUsageDataTable
        {
        }
    }
}
