using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Lidgren.Network;
using System.Threading;

namespace ComNet_SV
{
    public class Server
    {
        private static NetConfiguration config;
        private static NetServer server;
        private static NetBuffer readBuffer;
        public void start(string name, int connections, int port)
        {
            //Create configuration.
            config = new NetConfiguration(name);
            config.MaxConnections = connections;
            config.Port = port;
            //Create the server!
            server = new NetServer(config);
            server.SetMessageTypeEnabled( NetMessageType.ConnectionApproval, true);
            server.Start();
            //Create a buffer.
            readBuffer = server.CreateBuffer();
            Debug.PrintEvent("Server is running! press escape to exit server");
            consoleIdle( true );
        }

        /**
         * The thread is now stuck in a while loop, forever waiting for an escape keypress.
         **/
        public void consoleIdle(bool idle)
        {
            while (idle)
            {
                NetMessageType type;
                NetConnection sender;
                while (server.ReadMessage(readBuffer, out type, out sender))
                {
                    /**
                     * You must now put your code in here that you want the server to idle with, for instance, here is where you would init the comment sending system
                     **/

                    switch (type)
                    {
                        case NetMessageType.DebugMessage:
                            Debug.PrintDebug(readBuffer.ReadString());
                            break;
                        case NetMessageType.ConnectionApproval:
                            Debug.PrintEvent(string.Format("Connection Approval from {0}: {1}", sender, readBuffer.ReadString()));
                            break;
                        case NetMessageType.ConnectionRejected:
                            Debug.PrintError(string.Format("Connection Rejected from {0}: {1}", sender, readBuffer.ReadString()));
                            break;
                        case NetMessageType.StatusChanged:
                            string statusMessage = readBuffer.ReadString();
                            NetConnectionStatus newStatus = (NetConnectionStatus)readBuffer.ReadByte();
                            Debug.PrintEvent(string.Format("New status from {0} : {1} ( '{2}' )", sender, newStatus, statusMessage));
                            break;
                        case NetMessageType.Data:
                            string msg = readBuffer.ReadString();
                            decideCommand(msg, sender);
                            Debug.PrintEvent(string.Format("New message from {0}: {1}", sender, msg));
                            break;
                    }
                }

                while (Console.KeyAvailable)
                {
                    ConsoleKeyInfo key = Console.ReadKey();
                    if (key.Key == ConsoleKey.Escape)
                        idle = false;
                }
                Thread.Sleep(1);
            }
            server.Shutdown("Server has been exited by the admin.");
        }

        private void decideCommand(string command, NetConnection client)
        {
            string[] tasks = command.Split(' ');

            if (tasks[0] == "get")
            {
                if (tasks[1] == "url")
                {
                    //TODO: Obtain URL
                    return;
                }

                if (tasks[1] == "comments")
                {
                    //TODO: Return with comments ( client )
                    return;
                }
            }

            if (tasks[0] == "upload")
            {
                if (tasks[1] == "stats")
                {
                    //TODO: Recieved statistics from the user in the form of a table
                    return;
                }
            }
        }

        public static void Send(string information, NetConnection client)
        {
            NetBuffer prepareMessage = server.CreateBuffer();
            prepareMessage.Write(information);
            server.SendMessage(prepareMessage, client, NetChannel.ReliableInOrder1);
        }

        public static void SendToAll( string information )
        {
            NetBuffer prepareMessage = server.CreateBuffer();
            prepareMessage.Write(information);
            server.SendToAll(prepareMessage, NetChannel.ReliableInOrder1);
        }

        public static List<String> curConnectionIP()
        {
            List<String> connections = new List<String>();

            foreach (NetConnection client in server.Connections)
            {
                connections.Add( client.RemoteEndpoint.Address.ToString() );
            }

            return connections;
        }

        public static List<NetConnection> curConnectedClients()
        {
            List<NetConnection> connections = new List<NetConnection>();

            foreach (NetConnection client in server.Connections)
            {
                connections.Add(client);
            }

            return connections;
        }
    }
}
