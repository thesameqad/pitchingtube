using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Ozeki.MediaGateway;

namespace _05_Webphone_Silverligth_ChatExample
{
    public partial class CWindowConnection : ChildWindow
    {

        private MediaConnection connection;
        public event EventHandler<GenericEventArgs<MediaConnection>> ConnectedSuccessfully;

        public CWindowConnection( )
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(CWindowConnection_Loaded);
            txtNickName.Focus();
        }

        void CWindowConnection_Loaded(object sender, RoutedEventArgs e)
        {
          
            connection = new MediaConnection("127.0.0.1:4502/SilverlightMediaGateway");
            connection.ConnectionStateChanged += new EventHandler<GenericEventArgs<ConnectionState>>(connection_ConnectionStateChanged);
            
        }

        void connection_ConnectionStateChanged(object sender, GenericEventArgs<ConnectionState> e)
        {
            switch (e.Item)
            {
                case ConnectionState.Success:
                    lblStatus.Text = "Online";
                    if (ConnectedSuccessfully!= null)
                    {
                        ConnectedSuccessfully(this, new GenericEventArgs<MediaConnection>(connection));
                    }
                    this.DialogResult = true;
                    break;
                case ConnectionState.Failed:
                    lblStatus.Text = "Connection failed";
                    break;
                case ConnectionState.Closed:
                    lblStatus.Text = "Closed";
                    break;
            }
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            lblStatus.Text = "Connecting...";
            OKButton.IsEnabled = false;
            txtNickName.IsEnabled = false;
            connection.Connect(txtNickName.Text);
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void txtNickName_TextChanged(object sender, TextChangedEventArgs e)
        {
            OKButton.IsEnabled = !String.IsNullOrWhiteSpace(txtNickName.Text);   
        }

        private void txtNickName_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter && !String.IsNullOrWhiteSpace(txtNickName.Text))
            {
                OKButton_Click(null, null);
            }
        }
    }
}

