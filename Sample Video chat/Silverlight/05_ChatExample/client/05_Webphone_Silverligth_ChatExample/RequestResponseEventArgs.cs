using System;

namespace _05_Webphone_Silverligth_ChatExample
{
    public class RequestResponseEventArgs : EventArgs
    {
        public RequestResponseEventArgs(string owner, MediaType mediaType , bool responese )
        {
            Owner = owner;
            Response = responese;
            Type = mediaType;
        }


        public bool Response { get; set; }

        public string Owner { get; set; }

        public MediaType Type { get; set; }
    }
}
