using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Ozeki.MediaGateway;
using Ozeki.MediaGateway.SilverlightService;

namespace _05_Webphone_Silverligth_ChatExample
{
    public partial class MainPage : UserControl
    {
        private CWindowConnection conWindow;

        private MediaConnection connection;
        private MediaStreamSender streamSender;
        private MediaStreamReceiver streamReceiver;
        private AudioPlayer audioPlayer;
        private Microphone microphone;
        private Camera camera;

        public ObservableCollection<string> connectedUsers;

        private bool cameraIsEnable;
        private string cameraEnableWith;

        private CWindowCameraRequest winMediaReq;
        private bool audioIsEnable;
        private string audioEnableWith;

        public MainPage()
        {
            microphone=Microphone.GetMicrophone();
            audioPlayer=new AudioPlayer();
            camera = Camera.GetCamera();
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(MainPage_Loaded);
        }

        /// <summary>
        /// It initializes the connection window and open it.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            connectedUsers = new ObservableCollection<string>();
            listConnectedUsers.ItemsSource = connectedUsers;
            conWindow = new CWindowConnection();
            conWindow.ConnectedSuccessfully += new EventHandler<GenericEventArgs<MediaConnection>>(conWindow_ConnectedSuccessfully);
            conWindow.Show();
        }

        /// <summary>
        /// It initializes set GUI component values and creates new instance of MediaStreamSender for send media data through server in further action.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void conWindow_ConnectedSuccessfully(object sender, GenericEventArgs<MediaConnection> e)
        {
            connection = e.Item;
            rectOffline.Visibility = System.Windows.Visibility.Collapsed;
            lblNickName.Text = conWindow.txtNickName.Text;
            txtChatLog.Text += "Connected successfuly.\n";
            connection.Client = this;
            connection.InvokeOnConnection("GetConnectedClients",lblNickName.Text);
            streamSender=new MediaStreamSender(connection);
            streamSender.StreamStateChanged += new EventHandler<GenericEventArgs<StreamState>>(streamSender_StreamStateChanged);
            streamSender.Publish(lblNickName.Text); 
        }

        /// <summary>
        /// Indicates statechanges of streamReceiver object.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void streamReceiver_StreamStateChanged(object sender, GenericEventArgs<StreamState> e)
        {
            txtChatLog.Text += String.Format("Stream receiver {0} \n", e.Item.ToString());
        }

        /// <summary>
        /// Indicates statechanges of streamSender object.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void streamSender_StreamStateChanged(object sender, GenericEventArgs<StreamState> e)
        {
            txtChatLog.Text += String.Format("Stream sender {0} \n", e.Item.ToString());
        }

        /// <summary>
        /// Receives the connected client list from server.
        /// </summary>
        /// <param name="connectedUsers"> String array with connected client' nickname.</param>
        public void ConnectedClientsReceived(string[] connectedUsers)
        {
            this.connectedUsers.Clear();
            foreach (string user in connectedUsers)
            {
                if (user==lblNickName.Text)
                    continue;
                this.connectedUsers.Add(user);    
            }
        }

        #region TextMessage handling
        /// <summary>
        /// Receive messages from server.
        /// </summary>
        /// <param name="owner"> The client who sent the message.</param>
        /// <param name="message">Message data</param>
        public void ReceiveMessage(string owner, string message)
        {
            txtChatLog.Text += String.Format("{0}: {1}\n", owner, message);
        }

        /// <summary>
        /// Send text message to the selected client.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSend_Click(object sender, RoutedEventArgs e)
        {
            if (listConnectedUsers.SelectedItem != null)
            {
                txtChatLog.Text += String.Format("{0}: {1}\n", lblNickName.Text, txtMsgInput.Text);
                connection.InvokeOnConnection("SendText", lblNickName.Text, listConnectedUsers.SelectedItem, txtMsgInput.Text);
                txtMsgInput.Text = "";
            }
            else
                MessageBox.Show("You must select a connected user to send message.");
        }

        private void txtMsgInput_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key==Key.Enter)
                btnSend_Click(null,null);
        }
        #endregion

        #region Camera handling

        /// <summary>
        /// Send a video conversation request to the selected client.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCamera_Click(object sender, RoutedEventArgs e)
        {
            if (cameraIsEnable)
            {
                streamSender.DettachCamera();
                connection.InvokeOnConnection("SendCameraRequest",lblNickName.Text,cameraEnableWith,false);
                btnCamera.Content = "Enable video";
                cameraIsEnable = false;
                DestroyStreamReceiver();
                remoteVideoPlayer.Reset();
                return;
                
            }
            if (listConnectedUsers.SelectedItem != null && (CaptureDeviceConfiguration.AllowedDeviceAccess || CaptureDeviceConfiguration.RequestDeviceAccess()))
            {
               
                cameraIsEnable = !cameraIsEnable;
                txtChatLog.Text += "Please wait for other person response\n";
                connection.InvokeOnConnection("SendCameraRequest", lblNickName.Text, listConnectedUsers.SelectedItem, cameraIsEnable);
            }
            else
                MessageBox.Show("You must select a connected user, to enable camera chat.");
        }

        /// <summary>
        /// Receive the camera request from other connected client.
        /// </summary>
        /// <param name="owner">The request owner who sent request.</param>
        /// <param name="isEnable">If true video request received otherwise the video conversation hang up by other party.</param>
        public void CameraRequestReceived(string owner, bool isEnable)
        {
            if (isEnable)
            {
                winMediaReq = new CWindowCameraRequest(owner,MediaType.Video);
                winMediaReq.ResponseSelected += new EventHandler<RequestResponseEventArgs>(winMedia_ResponseSelected);
                winMediaReq.Show();
            }
            else
            {
                streamSender.DettachCamera();
                DestroyStreamReceiver();
                txtChatLog.Text += String.Format("{0} disabled his/her camera.\n", owner);
                btnCamera.Content = "Enable video chat";
                cameraIsEnable = false;
                remoteVideoPlayer.Reset();
            }
        }


        /// <summary>
        /// Receives responese for a sent camera request.
        /// </summary>
        /// <param name="owner"> The responding party nickname.</param>
        /// <param name="response"> If true, camera request is accepted otherwise don't accepted.</param>
        public void CameraResponseReceived(string owner, bool response)
        {
            if (response)
            {
                CreateAndSetupStreamReceiver(owner, MediaType.Video);
                streamSender.AttachCamera(camera);
                localVideoPlayer.AttachCamera(camera);
                cameraEnableWith = owner;
                txtChatLog.Text += String.Format("{0} accepted your camera request.\n",owner);
                btnCamera.Content = "Disable video";
                cameraIsEnable = true;
            }
            else
            {
                
                txtChatLog.Text += String.Format("{0} rejected your camera request.",owner);
            }
        }

        /// <summary>
        /// Reset streamReceiver object.
        /// </summary>
        private void DestroyStreamReceiver()
        {
            if (streamReceiver!=null)
            {
                remoteVideoPlayer.AttachCamera(null);
                streamReceiver.Close();
                streamReceiver.StreamStateChanged -= new EventHandler<GenericEventArgs<StreamState>>(streamReceiver_StreamStateChanged);
                streamReceiver = null;
            }
        }

        #endregion

        #region Audio handling
        /// <summary>
        /// Sends an audio conversation request to the selected client.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAudio_Click(object sender, RoutedEventArgs e)
        {
            if (audioIsEnable)
            {
                streamSender.DettachMicrophone();
                connection.InvokeOnConnection("SendAudioRequest", lblNickName.Text, audioEnableWith, false);
                btnAudio.Content = "Enable audio";
                audioIsEnable = false;
                DestroyStreamReceiver();
                
                return;

            }
            if (listConnectedUsers.SelectedItem != null && (CaptureDeviceConfiguration.AllowedDeviceAccess || CaptureDeviceConfiguration.RequestDeviceAccess()))
            {

                audioIsEnable = !audioIsEnable;
                txtChatLog.Text += "Please wait for other person response.\n";
                connection.InvokeOnConnection("SendAudioRequest", lblNickName.Text, listConnectedUsers.SelectedItem, audioIsEnable);
            }
            else
                MessageBox.Show("You must select a connected user, to enable audio chat.");

        }

        /// <summary>
        /// Receive the audio request from other client.
        /// </summary>
        /// <param name="owner">The request owner who sent request.</param>
        /// <param name="isEnable">If true audio request received otherwise the audio conversation hang up by other party.</param>
        public void AudioRequestReceived(string owner, bool isEnable)
        {
            if (isEnable)
            {
                winMediaReq = new CWindowCameraRequest(owner, MediaType.Audio);
                winMediaReq.ResponseSelected += new EventHandler<RequestResponseEventArgs>(winMedia_ResponseSelected);
                winMediaReq.Show();
            }
            else//close audio conversation
            {
                streamSender.DettachMicrophone();
                DestroyStreamReceiver();
                txtChatLog.Text += String.Format("{0} disabled his/her microphone.\n", owner);
                btnAudio.Content = "Enable audio";
                audioIsEnable = false;
            }
        }

        /// <summary>
        /// Receives responese for a sent audio request.
        /// </summary>
        /// <param name="owner">The responding party nickname.</param>
        /// <param name="response">If true, camera request is accepted otherwise don't accepted.</param>
        public void AudioResponseReceived(string owner, bool response)
        {
            if (response)
            {
                CreateAndSetupStreamReceiver(owner, MediaType.Audio);
                streamSender.AttachMicrophone(microphone);
                audioEnableWith = owner;
                txtChatLog.Text += String.Format("{0} accepted your audio request.\n", owner);
                btnAudio.Content = "Disable audio";
                audioIsEnable = true;
            }
            else
            {
                txtChatLog.Text += String.Format("{0} rejected your audio request.", owner);
            }
        }


        #endregion

        #region mediaCommon
        /// <summary>
        /// Initializes the MediaStreamReceiver for receive media data.
        /// </summary>
        /// <param name="playerName"></param>
        /// <param name="mediaType"></param>
        public void CreateAndSetupStreamReceiver(string playerName, MediaType mediaType)
        {
            if (streamReceiver == null)
            {
                streamReceiver = new MediaStreamReceiver(connection);
                streamReceiver.StreamStateChanged +=
                    new EventHandler<GenericEventArgs<StreamState>>(streamReceiver_StreamStateChanged);
                streamReceiver.Play(playerName);
            }
            if (mediaType == MediaType.Audio)
                audioPlayer.AttachMediaStreamReceiver(streamReceiver);
                //streamReceiver.AttachAudioPlayer(audioPlayer);
            else
            {
                remoteVideoPlayer.AttachMediaStreamReceiver(streamReceiver);
            }
        }

        /// <summary>
        /// Sends the selected response to the request owner who sent the media request and initializes the MediaStreamSender object to send audi/video data.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void winMedia_ResponseSelected(object sender, RequestResponseEventArgs e)
        {
            winMediaReq.ResponseSelected -= new EventHandler<RequestResponseEventArgs>(winMedia_ResponseSelected);
            if (e.Type==MediaType.Video)
                connection.InvokeOnConnection("SendCameraResponse", lblNickName.Text, e.Owner, e.Response);
            else
                connection.InvokeOnConnection("SendAudioResponse", lblNickName.Text, e.Owner, e.Response);

            if (e.Response)
            {
                switch (e.Type)
                {
                    case MediaType.Video:
                         txtChatLog.Text += "Camera request accepted\n";
                         CreateAndSetupStreamReceiver(e.Owner, MediaType.Video);

                        streamSender.AttachCamera(camera);
                        localVideoPlayer.AttachCamera(camera);
                        btnCamera.Content = "Disable video chat";
                        cameraIsEnable = true;
                        cameraEnableWith = e.Owner;
                        break;
                    case MediaType.Audio:
                         txtChatLog.Text += "Audio request accepted\n";
                         streamSender.AttachMicrophone(microphone);
                         CreateAndSetupStreamReceiver(e.Owner, MediaType.Audio);
                        btnAudio.Content = "Disable audio chat";
                        audioIsEnable = true;
                        audioEnableWith = e.Owner;
                        break;
                }
            }
            else
            {
                txtChatLog.Text +=String.Format("{0} request rejected\n",e.Type);
            }
        }

        #endregion

        private void btnCameraTest_Click(object sender, RoutedEventArgs e)
        {
            localVideoPlayer.AttachCamera(camera);
        }
    }
}
