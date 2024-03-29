﻿using System;
using System.Drawing;
using System.Net.Http;
using System.Windows;

using DevExpress.Xpf.Editors;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.API.Native;

using Microsoft.AspNet.SignalR.Client;

namespace VNCCodeCommandConsole.User_Interface.User_Controls
{
    /// <summary>
    /// Interaction logic for wndDX_ExploreInstances.xaml
    /// </summary>
    public partial class wucLiveLogEditor : wucDXBase
    {

        #region Enums, Fields, Properties

        public String UserName { get; set; }
        public IHubProxy HubProxy { get; set; }
        //private string ServerURI = "http://localhost:8095/signalr";
        public HubConnection Connection { get; set; }

        #endregion

        #region Constructors

        public wucLiveLogEditor()
        {
            try
            {
                InitializeComponent();
            }
            catch (Exception ex)
            {

            }

            //int primaryScreenHeight = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height;
            //int primaryScreenWidth = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width;

            //this.Width = (primaryScreenWidth * 9) / 10;
            //this.Height = (primaryScreenHeight * 9) / 10;
        }

        #endregion

        #region Initialization

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            //System.Windows.Data.CollectionViewSource myCollectionViewSource = (System.Windows.Data.CollectionViewSource)this.Resources["serversInstancesViewSource"];
            //// Things work if this line is present.  Testing to see if it works without 6/13/2012
            //// Yup, still works.  Don't need this line as it is done in the XAML.
            //myCollectionViewSource.Source = EyeOnLife.Common.ApplicationDataSet.Instances;

            System.Windows.Data.CollectionViewSource myCollectionViewSource = (System.Windows.Data.CollectionViewSource)this.Resources["serversViewSource"];
            // Things work if this line is present.  Testing to see if it works without 6/13/2012
            // Yup, still works.  Don't need this line as it is done in the XAML.
            //myCollectionViewSource.Source = EyeOnLife.Common.ApplicationDataSet.Servers;

            InitializeLogStream();
        }

        private void InitializeLogStream()
        {
            recLogStream.ActiveViewType = (RichEditViewType)cbeRichEditViewType.SelectedIndex;
            recLogStream.ActiveView.BackColor = System.Drawing.Color.Black;

            Document doc = recLogStream.Document;

            DevExpress.XtraRichEdit.API.Native.Section section = doc.Sections[0];

            section.Page.PaperKind = System.Drawing.Printing.PaperKind.B4;
            section.Page.Landscape = true;
            section.Margins.Left = 0.1f;
            section.Margins.Right = 0.1f;
        }

        #endregion

        #region Private Methods


        //AppendFormattedText(recLogStream, Color color)
        #region Connection Events

        void AppendFormattedMessage(DevExpress.Xpf.RichEdit.RichEditControl richEditControl, string formattedMessage)
        {
            // This works
            //richEditControl.Text += formattedMessage;
            // This throws not Rtf format exception
            //richEditControl.RtfText += formattedMessage;

            try
            {

                Document doc = richEditControl.Document;

                doc.BeginUpdate();

                doc.AppendText(formattedMessage);
                //doc.AppendRtfText(formattedMessage);

                doc.EndUpdate();
            }
            catch (Exception ex)
            {
                string exception = ex.ToString();
                string innerException = ex.InnerException.ToString();
                int x = 3;
            }

            //doc.BeginUpdate();

            //recLogStream.Document.AppendRtfText(formattedMessage);

            //doc.EndUpdate();

            //DocumentPosition docPosition = doc.CaretPosition;

            //recLogStream.Document.InsertRtfText(docPosition, formattedMessage);

            //recLogStream.RtfText += formattedMessage;

            //Range range = new Range();

            //CharacterProperties cp = doc.BeginUpdateCharacters(docPosition.);

            //recLogStream.RtfText += range.ToString();
        }

        void AppendColorFormattedMessage(DevExpress.Xpf.RichEdit.RichEditControl richEditControl, string formattedMessage, Color color)
        {
            try
            {

                //doc.BeginUpdate();

                //// This updates the entire document, sigh
                //doc.DefaultCharacterProperties.ForeColor = Color.Red;

                //doc.AppendText(formattedMessage);

                //doc.EndUpdate();

                Document doc = richEditControl.Document;

                DocumentRange newRange = doc.AppendText(formattedMessage);
                CharacterProperties charProp = doc.BeginUpdateCharacters(newRange);
                charProp.ForeColor = color;
                doc.EndUpdateCharacters(charProp);
            }
            catch (Exception ex)
            {
                string exception = ex.ToString();
                string innerException = ex.InnerException.ToString();
            }
        }

        private async void ConnectAsync()
        {
            Connection = new HubConnection(ServerURI.Text);
            Connection.Closed += Connection_Closed;
            Connection.Error += Connection_Error;
            Connection.Received += Connection_Received;
            Connection.Reconnected += Connection_Reconnected;
            Connection.Reconnecting += Connection_Reconnecting;
            Connection.StateChanged += Connection_StateChanged;
            HubProxy = Connection.CreateHubProxy("MyHub");

            //Handle incoming event from server: use Invoke to write to console from SignalR's thread

            string formattedMessage = "";

            HubProxy.On<string, string>("AddUserMessage", (name, message) =>
                this.Dispatcher.Invoke(() =>
                {
                    formattedMessage = String.Format("{0}: {1}\n", name, message);

                    AppendFormattedMessage(recLogStream, formattedMessage);
                }
                )
            );

            HubProxy.On<string>("AddMessage", (message) =>
                this.Dispatcher.Invoke(() =>
                {
                    //recLogStream.Text += String.Format("{0}\n", message);

                    //if (message.Contains("1002"))
                    //{
                    //    //Document doc = recLogStream.Document;
                    //    //DocumentPosition docPosition = doc.CaretPosition;
                    //    //recLogStream.Document.InsertRtfText(docPosition, message);
                    //    //string boldLeader = @"{\rtlch\fcs1 \ab\af0\afs48 \ltrch\fcs0 \b\fs48\cf1\insrsid5995062 }";
                    //    //string boldTrailer = @"{\rtlch\fcs1 \ab\af0\afs48 \ltrch\fcs0 \b\fs48\insrsid5995062 \par }";

                    //    //formattedMessage = string.Format("{0}{1}{2}\n", boldLeader, message, boldTrailer);

                    //    formattedMessage = String.Format("{0}\r", message);
                    //    AppendRedFormattedMessage(recLogStream, formattedMessage);
                    //}
                    //else
                    //{
                    //    formattedMessage = String.Format("{0}\r", message);
                    //    AppendFormattedMessage(recLogStream, formattedMessage);
                    //}

                    formattedMessage = String.Format("{0}\r", message);
                    AppendFormattedMessage(recLogStream, formattedMessage);
                })
            );

            HubProxy.On<string, int>("AddPriorityMessage", (message, priority) =>
                this.Dispatcher.Invoke(() =>
                {
                    Boolean displayMessage = false;

                    // For now treat the whole message the same.
                    formattedMessage = String.Format("{0}\r", message);

                    // TODO(crhodes)
                    // Make this more clever, perhaps a bit field
                    // But this may be plenty fast enough just long :(

                    switch (priority)
                    {
                        #region Info

                        case 100:
                            if (ceInfo00.IsChecked == true)
                            {
                                displayMessage = ColorFormatMessage(formattedMessage, Color.White);
                            }
                            break;

                        case 101:
                            if (ceInfo01.IsChecked == true)
                            {
                                displayMessage = ColorFormatMessage(formattedMessage, Color.White);
                            }
                            break;

                        case 102:
                            if (ceInfo02.IsChecked == true)
                            {
                                displayMessage = ColorFormatMessage(formattedMessage, Color.White);
                            }
                            break;

                        case 103:
                            if (ceInfo00.IsChecked == true)
                            {
                                displayMessage = ColorFormatMessage(formattedMessage, Color.White);
                            }
                            break;

                        case 104:
                            if (ceInfo04.IsChecked == true)
                            {
                                displayMessage = ColorFormatMessage(formattedMessage, Color.White);
                            }
                            break;

                        case 105:
                            if (ceInfo05.IsChecked == true)
                            {
                                displayMessage = ColorFormatMessage(formattedMessage, Color.White);
                            }
                            break;

                        #endregion

                        #region Debug

                        case 1000:
                            if (ceDebug00.IsChecked == true)
                            {
                                displayMessage = ColorFormatMessage(formattedMessage, Color.White);
                            }
                            break;

                        case 1001:
                            if (ceDebug01.IsChecked == true)
                            {
                                displayMessage = ColorFormatMessage(formattedMessage, Color.White);
                            }
                            break;

                        case 1002:
                            if (formattedMessage.Contains("Enter"))
                            {
                                if (ceDebug02Enter.IsChecked == true)
                                {
                                   displayMessage = ColorFormatMessage(formattedMessage, Color.Red); 
                                }                          
                            }
                            else if (ceDebug02Exit.IsChecked == true)
                            {
                                displayMessage = ColorFormatMessage(formattedMessage, Color.Red);
                            }
                            break;

                        case 1003:
                            if (ceDebug03.IsChecked == true)
                            {
                                displayMessage = ColorFormatMessage(formattedMessage, Color.White);
                            }
                            break;

                        case 1004:
                            if (ceDebug04.IsChecked == true)
                            {
                                displayMessage = ColorFormatMessage(formattedMessage, Color.White);
                            }
                            break;

                        case 1005:
                            if (ceDebug05.IsChecked == true)
                            {
                                displayMessage = ColorFormatMessage(formattedMessage, Color.White);
                            }
                            break;

                        #endregion

                        #region Trace00 - Trace09

                        case 10000: // PAGE_LOAD - FORM_LOAD
                            if (ceTrace00.IsChecked == true)
                            {
                                displayMessage = ColorFormatMessage(formattedMessage, Color.Lime);
                            }
                            break;

                        case 10001: // EVENTHANDLER
                            if (ceTrace01.IsChecked == true)
                            {
                                displayMessage = ColorFormatMessage(formattedMessage, Color.Cyan);
                            }
                            break;

                        case 10002: // STATUS
                            if (ceTrace02.IsChecked == true) displayMessage = true;
                            break;

                        case 10003: // REDIRECT_TRANSFER
                            if (ceTrace03.IsChecked == true)
                            {
                                displayMessage = ColorFormatMessage(formattedMessage, Color.GreenYellow);
                            }
                            break;

                        case 10004: // POLLING
                            if (ceTrace04.IsChecked == true)
                            {
                                displayMessage = ColorFormatMessage(formattedMessage, Color.BurlyWood);
                            }
                            break;

                        case 10005: // ERROR_TRACE
                            if (ceTrace05.IsChecked == true)
                            {
                                displayMessage = ColorFormatMessage(formattedMessage, Color.Yellow);
                            }
                            break;

                        case 10006: // EASESYS_IO
                            if (ceTrace06.IsChecked == true)
                            {
                                displayMessage = ColorFormatMessage(formattedMessage, Color.DarkCyan);
                            }
                            break;

                        case 10007: // UI_CONTROL
                            if (ceTrace07.IsChecked == true)
                            {
                                displayMessage = ColorFormatMessage(formattedMessage, Color.LightPink);
                            }
                            break;

                        case 10008: // UTILITY
                            if (ceTrace08.IsChecked == true)
                            {
                                displayMessage = ColorFormatMessage(formattedMessage, Color.SlateGray);
                            }
                            break;

                        case 10009: // OPERATION
                            if (ceTrace09.IsChecked == true)
                            {
                                displayMessage = ColorFormatMessage(formattedMessage, Color.White);
                            }
                            break;

                        #endregion

                        #region Trace10 - Trace19

                        case 10010: // APPLICATION_SESSION
                            if (ceTrace10.IsChecked == true)
                            {
                                displayMessage = ColorFormatMessage(formattedMessage, Color.Plum);
                            }
                            break;

                        case 10011: // SYSTEM_CONFIG
                            if (ceTrace11.IsChecked == true)
                            {
                                displayMessage = ColorFormatMessage(formattedMessage, Color.Orange);
                            }
                            break;

                        case 10012: // FILE_DIR_IO
                            if (ceTrace12.IsChecked == true)
                            {
                                displayMessage = ColorFormatMessage(formattedMessage, Color.Chocolate);
                            }
                            break;

                        case 10013: // DATABASE_IO
                            if (ceTrace13.IsChecked == true)
                            {
                                displayMessage = ColorFormatMessage(formattedMessage, Color.Olive);
                            }
                            break;

                        case 10014: // SECURITY
                            if (ceTrace14.IsChecked == true)
                            {
                                displayMessage = ColorFormatMessage(formattedMessage, Color.Fuchsia);
                            }
                            break;

                        case 10015: // ERROR_TRACE_LOW
                            if (ceTrace15.IsChecked == true)
                            {
                                displayMessage = ColorFormatMessage(formattedMessage, Color.Yellow);
                            }
                            break;

                        case 10016: // EASESYS_IO_MED
                            if (ceTrace16.IsChecked == true)
                            {
                                displayMessage = ColorFormatMessage(formattedMessage, Color.DarkCyan);
                            }
                            break;

                        case 10017: // UI_CONTROL_MED
                            if (ceTrace17.IsChecked == true)
                            {
                                displayMessage = ColorFormatMessage(formattedMessage, Color.LightPink);
                            }
                            break;

                        case 10018: // UTILITY_MED
                            if (ceTrace18.IsChecked == true)
                            {
                                displayMessage = ColorFormatMessage(formattedMessage, Color.SlateGray);
                            }
                            break;

                        case 10019: // OPERATION_LOW - DEFAULT
                            if (ceTrace19.IsChecked == true)
                            {
                                displayMessage = ColorFormatMessage(formattedMessage, Color.White);
                            }
                            break;

                        #endregion

                        #region Trace20 - Trace29

                        case 10020: // APPLICAITON_SESSION_LOW
                            if (ceTrace20.IsChecked == true)
                            {
                                displayMessage = ColorFormatMessage(formattedMessage, Color.Plum);
                            }
                            break;

                        case 10021: // SYSTEM_CONFIG_LOW
                            if (ceTrace21.IsChecked == true)
                            {
                                displayMessage = ColorFormatMessage(formattedMessage, Color.Orange);
                            }
                            break;

                        case 10022: // FILE_DIR_IO_LOW
                            if (ceTrace22.IsChecked == true)
                            {
                                displayMessage = ColorFormatMessage(formattedMessage, Color.Chocolate);
                            }
                            break;

                        case 10023: // DATABASE_IO_LOW
                            if (ceTrace23.IsChecked == true)
                            {
                                displayMessage = ColorFormatMessage(formattedMessage, Color.Olive);
                            }
                            break;

                        case 10024: // SECURITY_LOW
                            if (ceTrace24.IsChecked == true)
                            {
                                displayMessage = ColorFormatMessage(formattedMessage, Color.Fuchsia);
                            }
                            break;

                        case 10025: // CLEAR_INITIALIZE
                            if (ceTrace25.IsChecked == true)
                            {
                                displayMessage = ColorFormatMessage(formattedMessage, Color.White);
                            }
                            break;

                        case 10026: // EASESYS_IO_LOW
                            if (ceTrace26.IsChecked == true)
                            {
                                displayMessage = ColorFormatMessage(formattedMessage, Color.DarkCyan);
                            }
                            break;

                        case 10027: // UI_CONTROL_LOW
                            if (ceTrace27.IsChecked == true)
                            {
                                displayMessage = ColorFormatMessage(formattedMessage, Color.LightPink);
                            }
                            break;

                        case 10028: // UTILITY_LOW
                            if (ceTrace28.IsChecked == true)
                            {
                                displayMessage = ColorFormatMessage(formattedMessage, Color.SlateGray);
                            }
                            break;

                        case 10029:
                            if (ceTrace29.IsChecked == true)
                            {
                                displayMessage = ColorFormatMessage(formattedMessage, Color.White);
                            }
                            break;

                        #endregion

                        default:
                            displayMessage = true;
                            break;
                    }

                    if (displayMessage)
                    {
                        //recLogStream.Text += String.Format("{0}\n", message);

                        //if (message.Contains("1002"))
                        //{
                        //    //Document doc = recLogStream.Document;
                        //    //DocumentPosition docPosition = doc.CaretPosition;
                        //    //recLogStream.Document.InsertRtfText(docPosition, message);
                        //    //string boldLeader = @"{\rtlch\fcs1 \ab\af0\afs48 \ltrch\fcs0 \b\fs48\cf1\insrsid5995062 }";
                        //    //string boldTrailer = @"{\rtlch\fcs1 \ab\af0\afs48 \ltrch\fcs0 \b\fs48\insrsid5995062 \par }";

                        //    //formattedMessage = string.Format("{0}{1}{2}\n", boldLeader, message, boldTrailer);

                        //    formattedMessage = String.Format("{0}\r", message);
                        //    AppendRedFormattedMessage(recLogStream, formattedMessage);
                        //}
                        //else
                        //{
                        //    formattedMessage = String.Format("{0}\r", message);
                        //    AppendFormattedMessage(recLogStream, formattedMessage);
                        //}

                        formattedMessage = String.Format("{0}\r", message);
                        AppendFormattedMessage(recLogStream, formattedMessage);
                    }
                })
            );

            try
            {
                await Connection.Start();
            }
            catch (HttpRequestException)
            {
                StatusText.Content = "Unable to connect to server: Start server before connecting clients.";
                //No connection: Don't enable Send button or show chat UI
                return;
            }

            //Show chat UI; hide login UI
            SignInPanel.Visibility = Visibility.Collapsed;
            ChatPanel.Visibility = Visibility.Visible;
            btnSend.IsEnabled = true;
            btnSendPriority.IsEnabled = true;
            tbMessage.Focus();
            formattedMessage = "Connected to server at " + ServerURI + "\n";

            AppendFormattedMessage(recLogStream, formattedMessage);
        }

        private bool ColorFormatMessage(string formattedMessage, Color color)
        {
            bool displayMessage = false;
            int messageIndex = 0;

            messageIndex = GetNthIndex(formattedMessage, '|', 6);
            try
            {
                if (messageIndex++ > 0)
                {
                    string prefixMessage = formattedMessage.Substring(0, messageIndex);
                    AppendFormattedMessage(recLogStream, prefixMessage);

                    string colorMessage = formattedMessage.Substring(messageIndex);
                    AppendColorFormattedMessage(recLogStream, colorMessage, color);
                }
                else
                {
                    AppendColorFormattedMessage(recLogStream, formattedMessage, color);
                }
            }
            catch (Exception ex)
            {
                AppendColorFormattedMessage(recLogStream, ex.ToString(), Color.Red);
            }

            return displayMessage;
        }

        void Connection_Reconnected()
        {
            var dispatcher = Application.Current.Dispatcher;
            string formattedMessage = "Connection_Reconnected\n";

            dispatcher.Invoke(() => AppendFormattedMessage(recLogStream, formattedMessage));
        }

        void Connection_Reconnecting()
        {
            var dispatcher = Application.Current.Dispatcher;
            string formattedMessage = "Connection_Reconnecting\n";

            dispatcher.Invoke(() => AppendFormattedMessage(recLogStream, formattedMessage));
        }

        void Connection_StateChanged(StateChange obj)
        {
            var dispatcher = Application.Current.Dispatcher;
            var formattedMessage = string.Format("Connection_StateChanged {0,15} -> {1,-15}\n", obj.OldState, obj.NewState);

            dispatcher.Invoke(() => AppendFormattedMessage(recLogStream, formattedMessage));
        }

        private void Connection_Received(string obj)
        {
            //var dispatcher = Application.Current.Dispatcher;
            //string formattedMessage = "Connection_Received\n";

            //dispatcher.Invoke(() => AppendFormattedMessage(recLogStream, formattedMessage));
        }

        private void Connection_Error(Exception obj)
        {
            var dispatcher = Application.Current.Dispatcher;
            var formattedMessage = string.Format("Connection_Error >{0}<\n", obj.GetBaseException().ToString());

            dispatcher.Invoke(() => AppendFormattedMessage(recLogStream, formattedMessage));
        }

        /// <summary>
        /// If the server is stopped, the connection will time out after 30 seconds (default), and the 
        /// Closed event will fire.
        /// </summary>
        void Connection_Closed()
        {
            //Hide chat UI; show login UI
            var dispatcher = Application.Current.Dispatcher;
            dispatcher.Invoke(() => ChatPanel.Visibility = Visibility.Collapsed);
            dispatcher.Invoke(() => btnSendPriority.IsEnabled = false);
            dispatcher.Invoke(() => btnSend.IsEnabled = false);
            dispatcher.Invoke(() => recLogStream.Text += "You have been disconnected.\n");
            dispatcher.Invoke(() => SignInPanel.Visibility = Visibility.Visible);
        }

        #endregion

        #endregion

        #region Event Handlers

        private void btnSignIn_Click(object sender, RoutedEventArgs e)
        {
            UserName = UserNameTextBox.Text;
            //Connect to server (use async method to avoid blocking UI thread)
            if (!String.IsNullOrEmpty(UserName))
            {
                StatusText.Visibility = Visibility.Visible;
                StatusText.Content = "Connecting to server...";
                ConnectAsync();
            }
        }

        private void btnSend_Click(object sender, RoutedEventArgs e)
        {
            HubProxy.Invoke("Send", UserName, tbMessage.Text);
            tbMessage.Text = String.Empty;

            tbMessage.Focus();
        }

        private void btnSendPriority_Click(object sender, RoutedEventArgs e)
        {
            HubProxy.Invoke("SendPriority", tbMessage.Text, Int32.Parse(tbMessagePriority.Text));
            tbMessage.Text = String.Empty;
            tbMessage.Focus();
        }

        private void recLogStream_TextChanged(object sender, EventArgs e)
        {
            lbLastEntry.Content = DateTime.Now.ToString("HH:mm:ss.fff");

            // TODO(crhodes)
            // See if can do this with a RichEditControl control

            //if (ceAutoUpdate.IsChecked == true)
            //{
            //    recLogStream.Focus();
            //    recLogStream.SelectionStart = recLogStream.Text.Length;
            //}
        }

        private void btnUpdateScreen_Click(object sender, RoutedEventArgs e)
        {
            // TODO(crhodes)
            // See if can do this with a RichEditControl control

            //recLogStream.Focus();
            //recLogStream.SelectionStart = recLogStream.Text.Length;
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            //recLogStream.Clear();
            recLogStream.Text = "";
            InitializeLogStream();
        }

        private void btnCopy_Click(object sender, RoutedEventArgs e)
        {
            recLogStream.SelectAll();
            recLogStream.Copy();
        }

        private void btnInfoToggle_Click(object sender, RoutedEventArgs e)
        {
            if ((String)btnInfoToggle.Content == "All Off")
            {
                ceInfo00.IsChecked = false;
                ceInfo01.IsChecked = false;
                ceInfo02.IsChecked = false;
                ceInfo03.IsChecked = false;
                ceInfo04.IsChecked = false;
                ceInfo05.IsChecked = false;
                
                btnInfoToggle.Content = "All On";
            }
            else
            {
                ceInfo00.IsChecked = true;
                ceInfo01.IsChecked = true;
                ceInfo02.IsChecked = true;
                ceInfo03.IsChecked = true;
                ceInfo04.IsChecked = true;
                ceInfo05.IsChecked = true;

                btnInfoToggle.Content = "All Off";
            }
        }

        private void btnDebugToggle_Click(object sender, RoutedEventArgs e)
        {
            if ((String)btnDebugToggle.Content == "All Off")
            {
                ceDebug00.IsChecked = false;
                ceDebug01.IsChecked = false;
                ceDebug02Enter.IsChecked = false;
                ceDebug02Exit.IsChecked = false;
                ceDebug03.IsChecked = false;
                ceDebug04.IsChecked = false;
                ceDebug05.IsChecked = false;

                btnDebugToggle.Content = "All On";
            }
            else
            {
                ceDebug00.IsChecked = true;
                ceDebug01.IsChecked = true;
                ceDebug02Enter.IsChecked = true;
                ceDebug02Exit.IsChecked = true;
                ceDebug03.IsChecked = true;
                ceDebug04.IsChecked = true;
                ceDebug05.IsChecked = true;

                btnDebugToggle.Content = "All Off";
            }
        }

        private void btnTrace00_09Toggle_Click(object sender, RoutedEventArgs e)
        {
            if ((String)btnTrace00_09Toggle.Content == "All Off")
            {
                ceTrace00.IsChecked = false;
                ceTrace01.IsChecked = false;
                ceTrace02.IsChecked = false;
                ceTrace03.IsChecked = false;
                ceTrace04.IsChecked = false;
                ceTrace05.IsChecked = false;
                ceTrace06.IsChecked = false;
                ceTrace07.IsChecked = false;
                ceTrace08.IsChecked = false;
                ceTrace09.IsChecked = false;

                btnTrace00_09Toggle.Content = "All On";
            }
            else
            {
                ceTrace00.IsChecked = true;
                ceTrace01.IsChecked = true;
                ceTrace02.IsChecked = true;
                ceTrace03.IsChecked = true;
                ceTrace04.IsChecked = true;
                ceTrace05.IsChecked = true;
                ceTrace06.IsChecked = true;
                ceTrace07.IsChecked = true;
                ceTrace08.IsChecked = true;
                ceTrace09.IsChecked = true;

                btnTrace00_09Toggle.Content = "All Off";
            }
        }

        private void btnTrace10_19Toggle_Click(object sender, RoutedEventArgs e)
        {
            if ((String)btnTrace10_19Toggle.Content == "All Off")
            {
                ceTrace10.IsChecked = false;
                ceTrace11.IsChecked = false;
                ceTrace12.IsChecked = false;
                ceTrace13.IsChecked = false;
                ceTrace14.IsChecked = false;
                ceTrace15.IsChecked = false;
                ceTrace16.IsChecked = false;
                ceTrace17.IsChecked = false;
                ceTrace18.IsChecked = false;
                ceTrace19.IsChecked = false;

                btnTrace10_19Toggle.Content = "All On";
            }
            else
            {
                ceTrace10.IsChecked = true;
                ceTrace11.IsChecked = true;
                ceTrace12.IsChecked = true;
                ceTrace13.IsChecked = true;
                ceTrace14.IsChecked = true;
                ceTrace15.IsChecked = true;
                ceTrace16.IsChecked = true;
                ceTrace17.IsChecked = true;
                ceTrace18.IsChecked = true;
                ceTrace19.IsChecked = true;

                btnTrace10_19Toggle.Content = "All Off";
            }
        }

        private void btnTrace20_29Toggle_Click(object sender, RoutedEventArgs e)
        {
            if ((String)btnTrace20_29Toggle.Content == "All Off")
            {
                ceTrace20.IsChecked = false;
                ceTrace21.IsChecked = false;
                ceTrace22.IsChecked = false;
                ceTrace23.IsChecked = false;
                ceTrace24.IsChecked = false;
                ceTrace25.IsChecked = false;
                ceTrace26.IsChecked = false;
                ceTrace27.IsChecked = false;
                ceTrace28.IsChecked = false;
                ceTrace29.IsChecked = false;

                btnTrace20_29Toggle.Content = "All On";
            }
            else
            {
                ceTrace20.IsChecked = true;
                ceTrace21.IsChecked = true;
                ceTrace22.IsChecked = true;
                ceTrace23.IsChecked = true;
                ceTrace24.IsChecked = true;
                ceTrace25.IsChecked = true;
                ceTrace26.IsChecked = true;
                ceTrace27.IsChecked = true;
                ceTrace28.IsChecked = true;
                ceTrace29.IsChecked = true;

                btnTrace20_29Toggle.Content = "All Off";
            }
        }

        private void btnToggle_Click(object sender, RoutedEventArgs e)
        {
            // TODO(crhodes)
            // Figure out way to explore all child controls inside of capture filter and set IsChecked

            if ((String)btnToggle.Content == "All Off")
            {
                ceInfo00.IsChecked = false;
                ceInfo01.IsChecked = false;
                ceInfo02.IsChecked = false;
                ceInfo03.IsChecked = false;
                ceInfo04.IsChecked = false;

                ceDebug00.IsChecked = false;
                ceDebug01.IsChecked = false;
                ceDebug02Enter.IsChecked = false;
                ceDebug02Exit.IsChecked = false;
                ceDebug03.IsChecked = false;
                ceDebug04.IsChecked = false;

                ceTrace00.IsChecked = false;
                ceTrace01.IsChecked = false;
                ceTrace02.IsChecked = false;
                ceTrace03.IsChecked = false;
                ceTrace04.IsChecked = false;
                ceTrace05.IsChecked = false;
                ceTrace06.IsChecked = false;
                ceTrace07.IsChecked = false;
                ceTrace08.IsChecked = false;
                ceTrace09.IsChecked = false;

                ceTrace10.IsChecked = false;
                ceTrace11.IsChecked = false;
                ceTrace12.IsChecked = false;
                ceTrace13.IsChecked = false;
                ceTrace14.IsChecked = false;
                ceTrace15.IsChecked = false;
                ceTrace16.IsChecked = false;
                ceTrace17.IsChecked = false;
                ceTrace18.IsChecked = false;
                ceTrace19.IsChecked = false;

                ceTrace20.IsChecked = false;
                ceTrace21.IsChecked = false;
                ceTrace22.IsChecked = false;
                ceTrace23.IsChecked = false;
                ceTrace24.IsChecked = false;
                ceTrace25.IsChecked = false;
                ceTrace26.IsChecked = false;
                ceTrace27.IsChecked = false;
                ceTrace28.IsChecked = false;
                ceTrace29.IsChecked = false;

                btnInfoToggle.Content = "All On";
                btnDebugToggle.Content = "All On";
                btnTrace00_09Toggle.Content = "All On";
                btnTrace10_19Toggle.Content = "All On";
                btnTrace20_29Toggle.Content = "All On";
                btnToggle.Content = "All On";
            }
            else
            {
                ceInfo00.IsChecked = true;
                ceInfo01.IsChecked = true;
                ceInfo02.IsChecked = true;
                ceInfo03.IsChecked = true;
                ceInfo04.IsChecked = true;

                ceDebug00.IsChecked = true;
                ceDebug01.IsChecked = true;
                ceDebug02Enter.IsChecked = true;
                ceDebug02Exit.IsChecked = true;
                ceDebug03.IsChecked = true;
                ceDebug04.IsChecked = true;

                ceTrace00.IsChecked = true;
                ceTrace01.IsChecked = true;
                ceTrace02.IsChecked = true;
                ceTrace03.IsChecked = true;
                ceTrace04.IsChecked = true;
                ceTrace05.IsChecked = true;
                ceTrace06.IsChecked = true;
                ceTrace07.IsChecked = true;
                ceTrace08.IsChecked = true;
                ceTrace09.IsChecked = true;

                ceTrace10.IsChecked = true;
                ceTrace11.IsChecked = true;
                ceTrace12.IsChecked = true;
                ceTrace13.IsChecked = true;
                ceTrace14.IsChecked = true;
                ceTrace15.IsChecked = true;
                ceTrace16.IsChecked = true;
                ceTrace17.IsChecked = true;
                ceTrace18.IsChecked = true;
                ceTrace19.IsChecked = true;

                ceTrace20.IsChecked = true;
                ceTrace21.IsChecked = true;
                ceTrace22.IsChecked = true;
                ceTrace23.IsChecked = true;
                ceTrace24.IsChecked = true;
                ceTrace25.IsChecked = true;
                ceTrace26.IsChecked = true;
                ceTrace27.IsChecked = true;
                ceTrace28.IsChecked = true;
                ceTrace29.IsChecked = true;

                btnInfoToggle.Content = "All Off";
                btnDebugToggle.Content = "All Off";
                btnTrace00_09Toggle.Content = "All Off";
                btnTrace10_19Toggle.Content = "All Off";
                btnTrace20_29Toggle.Content = "All Off";
                btnToggle.Content = "All Off";
            }
        }

        #endregion

        private void CbeRichEditViewType_EditValueChanged(object sender, EditValueChangedEventArgs e)
        {
            InitializeLogStream();
        }
        int GetNthIndex(string s, char c, int n)
        {
            int count = 0;

            for (int i = 0; i < s.Length; i++)
            {
                if (s[i] == c)
                {
                    count++;
                    if (count == n)
                    {
                        return i;
                    }
                }
            }
            return -1;
            // = s.TakeWhile(c => n -= (c == t ? 1 : 0)) > 0).Count();
        }
    }
}
