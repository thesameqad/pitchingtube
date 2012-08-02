using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ozeki.MediaGateway;
using Ozeki.MediaGateway.Config;

namespace ChatGatewaySample
{
    /// <summary>
    /// Extends the MediaGateway base service functions.
    /// </summary>
    class ChatGateway: MediaGateway 
    {
    
        private Dictionary<string,IClient> chatClients; 
        public ChatGateway( MediaGatewayConfig configuration):base(configuration)
        {
            chatClients=new Dictionary<string, IClient>();
        }


        #region Connection handling

        /// <summary>
        /// Indicates the client connection and registers it if the chatclient dictionary doesn't contain the client nickname.
        /// </summary>
        /// <param name="client">That client reference who call this method.</param>
        /// <param name="parameters">Opional parameters</param>
        public override void OnClientConnect(IClient client,  object[] parameters)
        {
            string nickname = parameters[0] as string;

            if (String.IsNullOrEmpty(nickname))
                return;
            Console.WriteLine("New client '{0}' is trying connect.", nickname);

            if (!chatClients.ContainsKey(nickname))
            {
                chatClients.Add(nickname,client);
                Console.WriteLine("Client '{0}' connected successfully.", nickname);
                ConnectedClientChanged(client);
                base.OnClientConnect(client, parameters);
            }
            else
            {
                Console.WriteLine("Nickname: '{0}' already has been used.",nickname);
            }
        }

        /// <summary>
        ///  Indicates client disconnection and remove client's keyvaluepair from chatclient.
        /// </summary>
        /// <param name="client">That client reference who call this method.</param>
        public override void OnClientDisconnect(IClient client)
        {
            if (chatClients.ContainsValue(client))
            {
                foreach (KeyValuePair<string, IClient> keyValuePair in chatClients)
                {
                    if (keyValuePair.Value==client)
                    {
                        Console.WriteLine("'{0}' client disconnected.", keyValuePair.Key);
                        chatClients.Remove(keyValuePair.Key);
                        break;
                    }
                }
            }
            ConnectedClientChanged(client);
            base.OnClientDisconnect(client);
        }

        /// <summary>
        /// Indicates the client starts publish his IMediastream.
        /// </summary>
        /// <param name="client">That client reference who call this method.</param>
        /// <param name="mediaStream">Published IMediaStream</param>
        public override void OnStreamPublishStart(IClient client, IMediaStream mediaStream)
        {
            Console.WriteLine("client OnStreamPublishStart");
            base.OnStreamPublishStart(client, mediaStream);
        }
        
        /// <summary>
        /// Sends the connected client list to the caller client.
        /// </summary>
        /// <param name="client">That client reference who call this method.</param>
        /// <param name="requestOwnernickName">Caller client nickname</param>
        public void GetConnectedClients(IClient client, string requestOwnernickName)
        {
            List<string> users;

            users = chatClients.Keys.ToList();

            if (users.Contains(requestOwnernickName))
                users.Remove(requestOwnernickName);

            client.InvokeMethod("ConnectedClientsReceived", new object[] {users.ToArray()});
        }

        /// <summary>
        /// Notifies the clients about the connected client list changed.
        /// </summary>
        /// <param name="requestClient">That client reference who call this method.</param>
        private void ConnectedClientChanged(IClient requestClient)
        {
            try
            {
                foreach (var client in chatClients)
                {
                    if (client.Value == requestClient)
                        continue;
                    client.Value.InvokeMethod("ConnectedClientsReceived", new object[] { chatClients.Keys.ToList().ToArray() });
                }
            }
            catch (Exception)
            {}
        }

        #endregion

        #region text message handling

        /// <summary>
        /// Sends a text message to the target client.
        /// </summary>
        /// <param name="client">That client reference who call this method.</param>
        /// <param name="owner">source of text message</param>
        /// <param name="target"></param>
        /// <param name="msg"></param>
        public void SendText(IClient client, string owner, string target, string msg)
        {
            if (chatClients.ContainsKey(target))
            {
                IClient cl;
                chatClients.TryGetValue(target,out cl);
                cl.InvokeMethod("ReceiveMessage", owner, msg);
            }
        }

        #endregion


        #region camera handling

        /// <summary>
        /// Sends camera request to target client.
        /// </summary>
        /// <param name="client">That client reference who call this method.</param>
        /// <param name="owner">Request owner</param>
        /// <param name="target">Client who receive the request.</param>
        /// <param name="isEnable">Bool is indicates the camera start/stop state.</param>
        public void SendCameraRequest(IClient client, string owner, string target, bool isEnable)
        {
            if (chatClients.ContainsKey(target))
            {
                IClient cl;
                chatClients.TryGetValue(target, out cl);
                cl.InvokeMethod("CameraRequestReceived", owner, isEnable);
            }
        }

        /// <summary>
        /// Sends response for a camera request.
        /// </summary>
        /// <param name="client">That client reference who call this method.</param>
        /// <param name="owner"> Client who sends the response.</param>
        /// <param name="target">Client who get the response.</param>
        /// <param name="response">Bool what is indicates the camera request is accepted or rejected.</param>
        public void SendCameraResponse(IClient client, string owner, string target, bool response)
        {
            if (chatClients.ContainsKey(target))
            {
                IClient cl;
                chatClients.TryGetValue(target, out cl);
                cl.InvokeMethod("CameraResponseReceived", owner, response);
            }
        }

        #endregion

        #region audio handling

        /// <summary>
        /// Sends audio request to target client.
        /// </summary>
        /// <param name="client">That client reference who call this method.</param>
        /// <param name="owner">Request owner</param>
        /// <param name="target">Client who receive the request.</param>
        /// <param name="isEnable">Bool is indicates the audio start/stop state.</param>
        public void SendAudioRequest(IClient client, string owner, string target, bool isEnable)
        {
            if (chatClients.ContainsKey(target))
            {
                IClient cl;
                chatClients.TryGetValue(target, out cl);
                cl.InvokeMethod("AudioRequestReceived", owner, isEnable);
            }
        }

        /// <summary>
        /// Sends response for a audio request.
        /// </summary>
        /// <param name="client">That client reference who call this method.</param>
        /// <param name="owner">Client who sends the response.</param>
        /// <param name="target">Client who get the response.</param>
        /// <param name="response">Bool what is indicates the audio request is accepted or rejected.</param>
        public void SendAudioResponse(IClient client, string owner, string target, bool response)
        {
            if (chatClients.ContainsKey(target))
            {
                IClient cl;
                chatClients.TryGetValue(target, out cl);
                cl.InvokeMethod("AudioResponseReceived", owner, response);
            }
        }

        #endregion
    }
}
