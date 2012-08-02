using System;
using System.Windows;
using System.Windows.Controls;

namespace _05_Webphone_Silverligth_ChatExample
{
    public partial class CWindowCameraRequest : ChildWindow
    {
        public event EventHandler<RequestResponseEventArgs> ResponseSelected;

        private string requestOwner;
        private MediaType mType;

        public CWindowCameraRequest(string owner, MediaType type)
        {
            InitializeComponent();
            requestOwner = owner;
            this.mType = type;
            lbltext.Text = String.Format("{0} request received from {1}\n",type, owner);
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            OnResponseSelected(true);
            this.DialogResult = true;

        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            OnResponseSelected(false);
            this.DialogResult = false;
        }

        private void OnResponseSelected(bool response)
        {
            if (ResponseSelected!=null)
                ResponseSelected(this,new RequestResponseEventArgs(requestOwner, mType,  response));
        }
    }
}

