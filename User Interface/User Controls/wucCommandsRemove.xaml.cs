﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

using DevExpress.Xpf.Editors;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.VisualBasic;

using VNC;
using VNC.CodeAnalysis;

using VNCCA = VNC.CodeAnalysis;
using VNCSR = VNC.CodeAnalysis.SyntaxRewriters;

namespace VNCCodeCommandConsole.User_Interface.User_Controls
{
    public partial class wucCommandsRemove : wucDXBase
    {
        private static int CLASS_BASE_ERRORNUMBER = ErrorNumbers.APPERROR;
        private const string LOG_APPNAME = Common.LOG_APPNAME;

        #region Constructors

        public wucCommandsRemove()
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
        private void ceCommentOut_MethodBlock_EditValueChanged(object sender, EditValueChangedEventArgs e)
        {
            var isChecked = (Boolean)((CheckEdit)sender).IsChecked.Value;
            // TODO(crhodes)
            // What would be cool is to look for the parent that is a TextBlock.
            // For now be boring and give it a name.
            teRemove_MethodBlock.Text = isChecked ? "CommentOut MethodBlock" : "Remove MethodBlock";
        }
        private void btnRemove_MethodBlock_Click(object sender, RoutedEventArgs e)
        {
            ProcessOperation(Remove_MethodBlockVB, CodeExplorer.configurationOptions);
        }
        private void ceCommentOut_ExpressionStatement_EditValueChanged(object sender, EditValueChangedEventArgs e)
        {
            var isChecked = (Boolean)((CheckEdit)sender).IsChecked.Value;
            // TODO(crhodes)
            // What would be cool is to look for the parent that is a TextBlock.
            // For now be boring and give it a name.
            teRemove_ExpressionStatement.Text = isChecked ? "CommentOut ExpressionStatement" : "Remove ExpressionStatement";
        }
        private void ceCommentOut_FieldDeclaration_EditValueChanged(object sender, EditValueChangedEventArgs e)
        {
            var isChecked = (Boolean)((CheckEdit)sender).IsChecked.Value;
            // TODO(crhodes)
            // What would be cool is to look for the parent that is a TextBlock.
            // For now be boring and give it a name.
            teRemove_FieldDeclaration.Text = isChecked ? "CommentOut FieldDeclaration" : "Remove FieldDeclaration";

        }
        private void btnRemove_FieldDeclaration_Click(object sender, RoutedEventArgs e)
        {
            ProcessOperation(Remove_FieldDeclarationVB, CodeExplorer.configurationOptions);
        }

        private void btnRemove_ExpressionStatement_Click(object sender, RoutedEventArgs e)
        {
            ProcessOperation(Remove_ExpressionStatementVB, CodeExplorer.configurationOptions);
        }

        #endregion

        #region Main Function Routines

        private StringBuilder CommentOut_InvocationExpressionVB(VNCCA.RewriteFileCommandConfiguration commandConfiguration, out bool performedReplacement)
        {
            performedReplacement = false;

            var rewriter = new VNC.CodeAnalysis.SyntaxRewriters.VB.CommentOutSingleLineInvocationExpression(commandConfiguration.TargetPattern, teComment.Text);

            rewriter.Messages = commandConfiguration.Results;

            rewriter._configurationOptions = commandConfiguration.CodeAnalysisOptions;

            SyntaxNode newNode = rewriter.Visit(commandConfiguration.SyntaxTree.GetRoot());

            performedReplacement = VNCSR.Helpers.SaveFileChanges(commandConfiguration, newNode);

            return commandConfiguration.Results;
        }

        private StringBuilder Remove_ExpressionStatementVB(VNCCA.RewriteFileCommandConfiguration commandConfiguration, out bool performedReplacement)
        {
            performedReplacement = false;

            var rewriter = new VNC.CodeAnalysis.SyntaxRewriters.VB.RemoveExpressionStatement(
                commandConfiguration.TargetPattern,
                (Boolean)ceCommentOut_FieldDeclaration.IsChecked, teComment.Text);

            rewriter.Messages = commandConfiguration.Results;

            rewriter._configurationOptions = commandConfiguration.CodeAnalysisOptions;

            SyntaxNode newNode = rewriter.Visit(commandConfiguration.SyntaxTree.GetRoot());

            performedReplacement = VNCSR.Helpers.SaveFileChanges(commandConfiguration, newNode);

            return commandConfiguration.Results;
        }

        private StringBuilder Remove_FieldDeclarationVB(VNCCA.RewriteFileCommandConfiguration commandConfiguration, out bool performedReplacement)
        {
            performedReplacement = false;
            VNCCA.SyntaxNode.FieldDeclarationLocation fieldDeclarationLocation = VNCCA.SyntaxNode.FieldDeclarationLocation.Class;

            // TODO(crhodes)
            // Go look at EyeOnLife and see how to do this in a cleaner way.

            switch (lbeFieldDeclarationLocation.EditValue.ToString())
            {
                case "Class":
                    fieldDeclarationLocation = VNCCA.SyntaxNode.FieldDeclarationLocation.Class;
                    break;

                case "Module":
                    fieldDeclarationLocation = VNCCA.SyntaxNode.FieldDeclarationLocation.Module;
                    break;

                case "Structure":
                    fieldDeclarationLocation = VNCCA.SyntaxNode.FieldDeclarationLocation.Structure;
                    break;
            }

            var rewriter = new VNC.CodeAnalysis.SyntaxRewriters.VB.RemoveFieldDeclaration(
                commandConfiguration.TargetPattern, fieldDeclarationLocation,
                (Boolean)ceCommentOut_FieldDeclaration.IsChecked, teComment.Text);

            rewriter.Messages = commandConfiguration.Results;

            rewriter._configurationOptions = commandConfiguration.CodeAnalysisOptions;

            SyntaxNode newNode = rewriter.Visit(commandConfiguration.SyntaxTree.GetRoot());

            performedReplacement = VNCSR.Helpers.SaveFileChanges(commandConfiguration, newNode);

            return commandConfiguration.Results;
        }

        private StringBuilder Remove_MethodBlockVB(RewriteFileCommandConfiguration commandConfiguration, out bool performedReplacement)
        {
            performedReplacement = false;

            var rewriter = new VNC.CodeAnalysis.SyntaxRewriters.VB.RemoveMethodBlock(
                commandConfiguration.TargetPattern,
                (Boolean)ceCommentOut_MethodBlock.IsChecked, teComment.Text);

            rewriter.Messages = commandConfiguration.Results;

            rewriter._configurationOptions = commandConfiguration.CodeAnalysisOptions;

            SyntaxNode newNode = rewriter.Visit(commandConfiguration.SyntaxTree.GetRoot());

            performedReplacement = VNCSR.Helpers.SaveFileChanges(commandConfiguration, newNode);

            return commandConfiguration.Results;

        }

        //private StringBuilder RewriteInvocationExpressionVB(VNCCA.RewriteFileCommandConfiguration commandConfiguration, out bool performedReplacement)
        //{
        //    performedReplacement = false;

        //    var rewriter = new VNC.CodeAnalysis.SyntaxRewriters.VB.InvocationExpression(
        //        commandConfiguration.TargetPattern, commandConfiguration.ReplacementPattern);

        //    rewriter.Messages = commandConfiguration.Results;

        //    rewriter._configurationOptions = commandConfiguration.CodeAnalysisOptions;

        //    SyntaxNode newNode = rewriter.Visit(commandConfiguration.SyntaxTree.GetRoot());

        //    string fileSuffix = CodeExplorer.configurationOptions.ceAddFileSuffix.IsChecked.Value ? CodeExplorer.configurationOptions.teFileSuffix.Text : "";

        //    performedReplacement = VNCSR.Helpers.SaveFileChanges(commandConfiguration, newNode);

        //    return commandConfiguration.Results;
        //}

        //private StringBuilder WrapSQLExecuteXCallsInDALHelperVB(VNCCA.RewriteFileCommandConfiguration commandConfiguration, out bool performedReplacement)
        //{
        //    performedReplacement = false;

        //    var rewriter = new VNC.CodeAnalysis.SyntaxRewriters.VB.WrapSQLExecuteXCallsInDALHelper(
        //        commandConfiguration.TargetPattern);

        //    rewriter._configurationOptions = commandConfiguration.CodeAnalysisOptions;

        //    rewriter.Messages = commandConfiguration.Results;

        //    SyntaxNode newNode = rewriter.Visit(commandConfiguration.SyntaxTree.GetRoot());

        //    string fileSuffix = CodeExplorer.configurationOptions.ceAddFileSuffix.IsChecked.Value ? CodeExplorer.configurationOptions.teFileSuffix.Text : "";

        //    performedReplacement = VNCSR.Helpers.SaveFileChanges(commandConfiguration, newNode);

        //    return commandConfiguration.Results;
        //}
        //        StringBuilder WrapSQLFillCallsInDALHelperVB(VNCCA.RewriteFileCommandConfiguration commandConfiguration, out bool performedReplacement)
        //{
        //    {
        //        performedReplacement = false;

        //        var rewriter = new VNC.CodeAnalysis.SyntaxRewriters.VB.WrapSQLFillCallsInDALHelper(commandConfiguration.TargetPattern);

        //        rewriter._configurationOptions = commandConfiguration.CodeAnalysisOptions;

        //        rewriter.Messages = commandConfiguration.Results;

        //        SyntaxNode newNode = rewriter.Visit(commandConfiguration.SyntaxTree.GetRoot());

        //        string fileSuffix = CodeExplorer.configurationOptions.ceAddFileSuffix.IsChecked.Value ? CodeExplorer.configurationOptions.teFileSuffix.Text : "";

        //        performedReplacement = VNCSR.Helpers.SaveFileChanges(commandConfiguration, newNode);

        //        return commandConfiguration.Results;
        //    }
        //}
        #endregion

        #region Utility Routines
        void ProcessOperation(VNCCA.Types.RewriteFileCommand command,
            wucConfigurationOptions configurationOptions)
        {
            StringBuilder sb = new StringBuilder();
            CodeExplorer.teSourceCode.Clear();
            CodeExplorer.teSourceCode.InvalidateVisual();

            string projectFullPath = CodeExplorerContext.teProjectFile.Text;

            //string newInvocationExpression = teNewInvocationExpression.Text;
            //string targetInvocationExpression = teTargetInvocationExpression.Text;

            var filesToProcess = CodeExplorerContext.GetFilesToProcess();

            Dictionary<string, Int32> replacements = new Dictionary<string, int>();

            if (filesToProcess.Count > 0)
            {
                if ((Boolean)configurationOptions.ceListImpactedFilesOnly.IsChecked)
                {
                    sb.AppendLine("Would Process these files ....");
                }

                foreach (string filePath in filesToProcess)
                {
                    try
                    {
                        if ((Boolean)configurationOptions.ceListImpactedFilesOnly.IsChecked)
                        {
                            sb.AppendLine(string.Format("  {0}", filePath));
                        }
                        else
                        {
                            StringBuilder sbFileResults = new StringBuilder();

                            var sourceCode = "";

                            using (var sr = new System.IO.StreamReader(filePath))
                            {
                                sourceCode = sr.ReadToEnd();
                            }

                            Boolean performedReplacement = false;

                            // 
                            // This is where the action happens
                            //

                            SyntaxTree tree = VisualBasicSyntaxTree.ParseText(sourceCode);

                            VNCCA.RewriteFileCommandConfiguration rewriteFileCommandConfiguration = new VNCCA.RewriteFileCommandConfiguration();
                            rewriteFileCommandConfiguration.Results = sbFileResults;
                            rewriteFileCommandConfiguration.SyntaxTree = tree;
                            rewriteFileCommandConfiguration.FilePath = filePath;
                            rewriteFileCommandConfiguration.Replacements = replacements;

                            rewriteFileCommandConfiguration.WalkerPattern.UseRegEx = (bool)ceReplacementTargetUseRegEx.IsChecked;
                            rewriteFileCommandConfiguration.TargetPattern = teTargetInvocationExpression.Text;
                            rewriteFileCommandConfiguration.CodeAnalysisOptions = CodeExplorer.configurationOptions.GetConfigurationInfo();

                            sbFileResults = command(rewriteFileCommandConfiguration, out performedReplacement);
                            //sbFileResults = command(sbFileResults, tree, filePath, targetInvocationExpression, newInvocationExpression, replacements, out performedReplacement);

                            if ((bool)configurationOptions.ceAlwaysDisplayFileName.IsChecked || (sbFileResults.Length > 0))
                            {
                                sb.AppendLine("Searching " + filePath);
                            }

                            if (performedReplacement)
                            {
                                sb.AppendLine("Rewrote " + filePath);
                            }

                            sb.Append(sbFileResults.ToString());
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                    }
                }
            }
            else
            {
                sb.AppendLine("No files selected to process");
            }

            CodeExplorer.teSourceCode.Text = sb.ToString();
        }


        #endregion
    }
}
