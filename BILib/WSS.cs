using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using Websocket.Client;

namespace BILib
{
    public class WSS
    {
        public static string pttUrl = "https://term.ptt.cc";
        public static string pttWssUrl = "wss://ws.ptt.cc/bbs";

        public bool CheckUserAvailable(string pttUserID, string pttPassword)
        {
            bool isUserLogIn = false;
            // ellen: Register big5 because .NET Core doesn't support big5
            Encoding big5 = Encoding.GetEncoding(950);

            var factory = new Func<ClientWebSocket>(() =>
            {
                var client = new ClientWebSocket
                {
                    Options =
                    {
                        KeepAliveInterval = TimeSpan.FromSeconds(5),
                        // Proxy = ...
                        // ClientCertificates = ...
                    }
                };
                client.Options.SetRequestHeader("Origin", pttUrl);
                return client;
            });

            var url = new Uri(pttWssUrl);

            using (IWebsocketClient client = new WebsocketClient(url, factory))
            {
                client.Name = "Ptt";
                client.ReconnectTimeout = TimeSpan.FromSeconds(30);
                client.ErrorReconnectTimeout = TimeSpan.FromSeconds(30);
                //client.ReconnectionHappened.Subscribe(type =>
                //{
                //    Log.Information($"Reconnection happened, type: {type}, url: {client.Url}");
                //});
                //client.DisconnectionHappened.Subscribe(info =>
                //    Log.Warning($"Disconnection happened, type: {info.Type}"));

                client.MessageReceived.Subscribe(msg =>
                {
                    // ellen: assume the encoding is big5. Not sure if there will be other encoding 
                    string returnedMsg = big5.GetString(msg.Binary);

                    // ellen: temp solution to check if user log in successfully
                    if (returnedMsg.Contains("請按任意鍵繼續"))
                    {
                        isUserLogIn = true;
                    }
                    //Console.WriteLine(returnedMsg);
                    //Log.Information($"Message received: {returnedMsg}");
                });

                client.Start().Wait();

                // ellen: Assume user can log in successfully
                // Need to handle 1. fail auth, 2. too many guests on ptt, 3. user already login
                if (!isUserLogIn)
                {
                    byte[] userInBytes = Encoding.Unicode.GetBytes(pttUserID + Environment.NewLine);
                    byte[] pwInBytes = Encoding.Unicode.GetBytes(pttPassword + Environment.NewLine);
                    byte[] newLineInBytes = Encoding.Unicode.GetBytes(Environment.NewLine);

                    client.Send(userInBytes);
                    client.Send(pwInBytes);

                    // ellen: Send newline to handle Press any key here
                    client.Send(newLineInBytes);
                }

            }
            return isUserLogIn;
        }

    }
}
