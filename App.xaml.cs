﻿using DevExpress.Xpf.Core;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

using VNC;

namespace VNCCodeCommandConsole
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static int BASE_ERRORNUMBER = ErrorNumbers.APPERROR;
        private const string LOG_APPNAME = Common.LOG_APPNAME;

        public App()
        {
            Log.Info("SignalRDelay", Common.LOG_APPNAME);  Thread.Sleep(100); 
            long startTicks = Log.CONSTRUCTOR("Enter", LOG_APPNAME);

            var defaultThemes = Theme.Themes;

            Theme.RemoveTheme(Theme.Office2007BlackName);
            Theme.RemoveTheme(Theme.Office2007BlueName);
            Theme.RemoveTheme(Theme.Office2007SilverName);

            Theme.RemoveTheme(Theme.Office2010BlackName);
            Theme.RemoveTheme(Theme.Office2010BlueName);
            Theme.RemoveTheme(Theme.Office2010SilverName);

            Theme.RemoveTheme(Theme.Office2013Name);
            Theme.RemoveTheme(Theme.Office2013TouchName);
            Theme.RemoveTheme(Theme.Office2013LightGrayName);
            Theme.RemoveTheme(Theme.Office2013LightGrayTouchName);
            Theme.RemoveTheme(Theme.Office2013DarkGrayName);
            Theme.RemoveTheme(Theme.Office2013DarkGrayTouchName);

            Theme.RemoveTheme(Theme.Office2016ColorfulName);
            Theme.RemoveTheme(Theme.Office2016ColorfulTouchName);
            Theme.RemoveTheme(Theme.Office2016WhiteTouchName);
            Theme.RemoveTheme(Theme.Office2016BlackTouchName);

            Theme.RemoveTheme(Theme.DXStyleName);
            Theme.RemoveTheme(Theme.DeepBlueName);
            Theme.RemoveTheme(Theme.SevenName);
            Theme.RemoveTheme(Theme.VS2010Name);
            Theme.RemoveTheme(Theme.TouchlineDarkName);
            Theme.RemoveTheme(Theme.LightGrayName);
            Theme.RemoveTheme(Theme.HybridAppName);

            var newThemes = Theme.Themes;

            // TODO(crhodes)
            // Cannot loop across and delete themes.  Read-Only collection.
            // Add to manually remove above.  ;(

            //foreach (var theme in defaultThemes)
            //{
            //    var name = theme.Name;

            //    if (name.Contains("2013") || name.Contains("2010") || name.Contains("2007"))
            //    {
            //        Theme.RemoveTheme(name);
            //    }
            //}

            ApplicationThemeHelper.ApplicationThemeName = Data.Config.DefaultUITheme;

            Log.CONSTRUCTOR("Exit", LOG_APPNAME, startTicks);
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {

            long startTicks = Log.APPLICATION_START("Enter", LOG_APPNAME);

            try
            {
                AppDomain.CurrentDomain.UnhandledException +=
                              new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

                Common.CurrentUser = new WindowsPrincipal(WindowsIdentity.GetCurrent());

                if (Data.Config.ADBypass)
                {
                    Common.IsAdministrator = true;
                    Common.IsBetaUser = true;
                }
                else
                {
                    if (!Data.Config.AD_Users_AllowAll)
                    {

                        bool isAuthorizedUser = VNC.ActiveDirectory.Helper.CheckGroupMembership(
                            //"maward", 
                            Common.CurrentUser.Identity.Name,
                            Data.Config.ADGroup_Users,
                            Data.Config.AD_Domain);

                        if (!isAuthorizedUser)
                        {
                            MessageBox.Show(string.Format("You must be a member of {0}\\{1} to run this application.",
                                Data.Config.AD_Domain, Data.Config.ADGroup_Users));
                            return;
                        }
                    }

                    Common.IsAdministrator = VNC.ActiveDirectory.Helper.CheckDirectGroupMembership(
                        Common.CurrentUser.Identity.Name,
                        Data.Config.ADGroup_Administrators,
                        Data.Config.AD_Domain);

                    Common.IsBetaUser = VNC.ActiveDirectory.Helper.CheckDirectGroupMembership(
                        Common.CurrentUser.Identity.Name,
                        Data.Config.ADGroup_BetaUsers,
                        Data.Config.AD_Domain);

                    Common.IsDeveloper = Common.CurrentUser.Identity.Name.Contains("crhodes") ? true : false;

                    // Next lines are for testing UI only.  Comment out for normal operation.
                    //Common.IsAdministrator = false;   
                    //Common.IsBetaUser = false; 
                    //Common.IsDeveloper = false;
                }

                // Cannot do here as the Common.ApplicationDataSet has not been loaded.  Need to move here or do later.
                // For now this is in DXRibbonWindowMain();

                //var eventMessage = "Started";
                //SQLInformation.Helper.IndicateApplicationUsage(LOG_APPNAME, DateTime.Now, currentUser.Identity.Name, eventMessage);

                // Launch the main window.



                // TODO(crhodes)
                // Would be cool to start this with a Window specified in the App.Config file

                //User_Interface.Windows.SplashScreen _window1 = new User_Interface.Windows.SplashScreen();
                User_Interface.Windows.DXRibbonWindowMain _window1 = new User_Interface.Windows.DXRibbonWindowMain();
                //User_Interface.Windows.About _window1 = new User_Interface.Windows.About();

                //String windowArgs = string.Empty;
                // Check for arguments; if there are some build the path to the package out of the args.
                //if (args.Args.Length > 0 && args.Args[0] != null)
                //{
                //    for (int i = 0; i < args.Args.Length; ++i)
                //    {
                //        windowArgs = args.Args[i];
                //        switch (i)
                //        {
                //            case 0: // Patient Id
                //                //patientId = windowArgs;
                //                break;
                //        }
                //    }
                //}

                _window1.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                MessageBox.Show(ex.InnerException.ToString());
            }

            Log.APPLICATION_START("Exit", LOG_APPNAME, startTicks);
        }

        private void Application_SessionEnding(object sender, SessionEndingCancelEventArgs e)
        {
#if TRACE
            long startTicks = Log.APPLICATION_END("Enter", LOG_APPNAME, BASE_ERRORNUMBER + 0);
#endif

#if TRACE
            Log.APPLICATION_END("Exit", LOG_APPNAME, BASE_ERRORNUMBER + 0, startTicks);
#endif
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
#if TRACE
            long startTicks = Log.APPLICATION_END("Enter", LOG_APPNAME, BASE_ERRORNUMBER + 0);
#endif

#if TRACE
            Log.APPLICATION_END("Exit", LOG_APPNAME, BASE_ERRORNUMBER + 0, startTicks);
#endif
        }

        private void Application_Deactivated(object sender, EventArgs e)
        {
#if TRACE
            long startTicks = Log.APPLICATION_END("Enter", LOG_APPNAME, BASE_ERRORNUMBER + 0);
#endif

#if TRACE
            Log.APPLICATION_END("Exit", LOG_APPNAME, BASE_ERRORNUMBER + 0, startTicks);
#endif
        }

        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
#if TRACE
            long startTicks = Log.Debug("Enter", LOG_APPNAME, BASE_ERRORNUMBER + 0);
#endif
            MessageBox.Show("Application_DispatcherUnhandledException: " + e.Exception.Message, LOG_APPNAME, MessageBoxButton.OK, MessageBoxImage.Warning);
            MessageBox.Show("Application_DispatcherUnhandledException Inner: " + e.Exception.InnerException.Message, LOG_APPNAME, MessageBoxButton.OK, MessageBoxImage.Warning);

            //e.Handled = true;   // Use if can handle exception that is thrown
            e.Handled = false;  // Default

#if TRACE
            Log.Debug("Exit", LOG_APPNAME, BASE_ERRORNUMBER + 0, startTicks);
#endif
        }

        void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = e.ExceptionObject as Exception;
            MessageBox.Show(ex.Message, "Uncaught Thread Exception",
                            MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
