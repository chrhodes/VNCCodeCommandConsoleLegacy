﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Xml.Linq;

using Microsoft.CodeAnalysis.VisualBasic;

using VNC;

using VNCCA = VNC.CodeAnalysis;
using VNCSW = VNC.CodeAnalysis.SyntaxWalkers;

namespace VNCCodeCommandConsole.User_Interface.User_Controls
{
    public partial class wucCommandsFind : wucDXBase
    {
        private static int CLASS_BASE_ERRORNUMBER = ErrorNumbers.APPERROR;
        private const string LOG_APPNAME = Common.LOG_APPNAME;

        #region Constructors

        public wucCommandsFind()
        {
#if TRACE
            long startTicks = Log.Trace15("Enter", LOG_APPNAME);
#endif
            try
            {
                InitializeComponent();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                MessageBox.Show(ex.InnerException.ToString());
            }
#if TRACE
            Log.Trace15("End", LOG_APPNAME, startTicks);
#endif
        }

        #endregion

        #region Initialization

        //private void LoadControlContents()
        //{
        //    //try
        //    //{
        //    //    wucFind_Picker.PopulateControlFromFile(Common.cCONFIG_FILE);
        //    //}
        //    //catch (Exception ex)
        //    //{
        //    //    MessageBox.Show(ex.ToString());
        //    //}
        //}

        internal override void OnLoaded(object sender, RoutedEventArgs e)
        {
#if TRACE
            long startTicks = Log.Trace15("Enter", LOG_APPNAME);
#endif
            // Cheat and force outcome if not using dat
            Common.DataFullyLoaded = true;
            User_Interface.Helper.ValidateDataFullyLoaded();
            LoadControlContents();

            try
            {
                if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
                {
                    //dataGrid.ItemsSource = VNCWPFUserControls.Common.ApplicationDataSet.ApplicationUsage;
                }

                //ViewMode.ApplyAuthorization(this);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
#if TRACE
            Log.Trace15("End", LOG_APPNAME, startTicks);
#endif
        }

        #endregion

        #region Event Handlers
        private void btnMethodNames_Click(object sender, RoutedEventArgs e)
        {
            string fileNameAndPath = CodeExplorerContext.teSourceFile.Text;

            var sourceCode = "";

            using (var sr = new StreamReader(fileNameAndPath))
            {
                sourceCode = sr.ReadToEnd();
            }

            List<String> methodNames = VNCCA.Helpers.VB.GetMethodNames(sourceCode);
        }

        private void btnInvocationExpressionInTryCatchWalker_Click(object sender, RoutedEventArgs e)
        {
            Helper.ProcessOperation(DisplayInvocationExpressionInTryCatchWalkerVB, CodeExplorer, CodeExplorerContext, CodeExplorer.configurationOptions);
        }

        private void btnVariableDeclaratorWalker_Click(object sender, RoutedEventArgs e)
        {
            Helper.ProcessOperation(DisplayMultipleVariableDeclaratorWalkerVB, CodeExplorer, CodeExplorerContext, CodeExplorer.configurationOptions);
        }

        private void btnHttpContextWalker_Click(object sender, RoutedEventArgs e)
        {
            var tag = ((System.Windows.Controls.Button)sender).Tag.ToString();

            DisplayHttpContextWalkerVB(CodeExplorerContext, tag);
        }

        #endregion

        #region Main Function Routines

        StringBuilder DisplayInvocationExpressionInTryCatchWalkerVB(VNCCA.SearchTreeCommandConfiguration commandConfiguration)
        {
            var walker = new VNCSW.VB.InvocationExpressionInTryCatch();

            //return VNCCA.Helpers.VB.InvokeVNCSyntaxWalker(sb,
            //    (bool)ceInvocationExpressionInTryCatchUseRegEx.IsChecked, teInvocationExpressionInTryCatchRegEx.Text,
            //    matches, crcMatchesToString, crcMatchesToFullString, tree, walker, CodeExplorer.configurationOptions.GetConfigurationInfo());

            commandConfiguration.WalkerPattern.UseRegEx = (bool)ceInvocationExpressionInTryCatchUseRegEx.IsChecked;
            commandConfiguration.WalkerPattern.RegEx = teInvocationExpressionInTryCatchRegEx.Text;
            commandConfiguration.CodeAnalysisOptions = CodeExplorer.configurationOptions.GetConfigurationInfo();

            return VNCCA.Helpers.VB.InvokeVNCSyntaxWalker(walker,
                commandConfiguration);
        }

        private void DisplayHttpContextWalkerVB(wucCodeExplorerContext codeExplorerContext, string context)
        {
            StringBuilder sb = new StringBuilder();
            Dictionary<string, Int32> matches = new Dictionary<string, Int32>();

            var sourceCode = "";

            // If a source file has been specified use it

            if (codeExplorerContext.teSourceFile.Text.Length > 0)
            {
                string fileNameAndPath = codeExplorerContext.teSourceFile.Text;

                if (!File.Exists(fileNameAndPath))
                {
                    sb.AppendLine(string.Format("File ({0}) does not exist.  Check path and name.", fileNameAndPath));
                    goto PrematureExitBummer;
                }

                if ((Boolean)CodeExplorer.configurationOptions.ceListImpactedFilesOnly.IsChecked)
                {
                    sb.AppendLine("Would Process these files ....");
                    sb.AppendLine(string.Format("  {0}", fileNameAndPath));
                }
                else
                {
                    sb.AppendLine(string.Format("Processing ({0}) ...", fileNameAndPath));

                    using (var sr = new StreamReader(fileNameAndPath))
                    {
                        sourceCode = sr.ReadToEnd();
                    }

                    var tree = VisualBasicSyntaxTree.ParseText(sourceCode);

                    string pattern = string.Empty;

                    VNCSW.VB.HttpContextCurrentInvocationExpression walker = null;
                    walker = new VNCCA.SyntaxWalkers.VB.HttpContextCurrentInvocationExpression(context);
                    walker.Messages = sb;
                    walker.Matches = matches;

                    walker.Visit(tree.GetRoot());
                }
            }
            else
            {
                // Otherwise, go see if one or more files has been selected

                if (codeExplorerContext.cbeSourceFile.SelectedItems.Count > 0)
                {
                    if ((Boolean)CodeExplorer.configurationOptions.ceListImpactedFilesOnly.IsChecked)
                    {
                        sb.AppendLine("Would Process these files ....");
                    }

                    foreach (XElement file in codeExplorerContext.cbeSourceFile.SelectedItems)
                    {
                        string filePath = codeExplorerContext.teSourcePath.Text + wucCodeExplorerContext.GetFilePath(file);

                        if ( ! File.Exists(filePath))
                        {
                            sb.AppendLine(string.Format("ERROR File ({0}) does not exist.  Check config file", filePath));
                            continue;
                        }

                        if ((Boolean)CodeExplorer.configurationOptions.ceListImpactedFilesOnly.IsChecked)
                        {
                            sb.AppendLine(string.Format("  {0}", filePath));
                        }
                        else
                        {
                            sb.AppendLine(string.Format("Processing ({0}) ...", filePath));

                            using (var sr = new StreamReader(filePath))
                            {
                                sourceCode = sr.ReadToEnd();
                            }

                            var tree = VisualBasicSyntaxTree.ParseText(sourceCode);

                            string pattern = string.Empty;

                            VNCSW.VB.HttpContextCurrentInvocationExpression walker = null;
                            walker = new VNCSW.VB.HttpContextCurrentInvocationExpression(context);
                            walker.Messages = sb;
                            walker.Matches = matches;

                            walker.Visit(tree.GetRoot());
                        }
                    }
                }
            }

            sb.AppendLine("Summary");

            foreach (var item in matches.OrderBy(v => v.Key).Select(k => k.Key))
            {
                sb.AppendLine(string.Format("Count: {0,3} {1} ", matches[item], item));
            }

 PrematureExitBummer:
            CodeExplorer.teSourceCode.Text = sb.ToString();
        }

        private StringBuilder DisplayMultipleVariableDeclaratorWalkerVB(VNCCA.SearchTreeCommandConfiguration commandConfiguration)
        {
            var walker = new VNCSW.VB.MultipleVariableDeclarator();

            walker.HasAttributes = (bool)CodeExplorer.configurationOptions.ceHasAttributes.IsChecked;

            //return VNCCA.Helpers.VB.InvokeVNCSyntaxWalker(sb,
            //    (bool)ceVariablesUseRegEx.IsChecked, teVariableRegEx.Text,
            //    matches, crcMatchesToString, crcMatchesToFullString, tree, walker, CodeExplorer.configurationOptions.GetConfigurationInfo());

            commandConfiguration.WalkerPattern.UseRegEx = (bool)ceVariablesUseRegEx.IsChecked;
            commandConfiguration.WalkerPattern.RegEx = teVariableRegEx.Text;
            commandConfiguration.CodeAnalysisOptions = CodeExplorer.configurationOptions.GetConfigurationInfo();

            return VNCCA.Helpers.VB.InvokeVNCSyntaxWalker(walker,
                commandConfiguration);
        }

        #endregion

        #region Utility Methods


        #endregion
    }
}
