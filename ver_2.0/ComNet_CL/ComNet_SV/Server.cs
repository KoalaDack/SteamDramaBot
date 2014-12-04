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
        private static NetPeer peer;
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
            Debug.Print("Server is running! press escape to exit server", ConsoleColor.Blue);
            consoleIdle( true );
        }

        public void consoleIdle(bool idle)
        {
            while (idle)
            {
                NetMessageType type;
                NetConnection sender;
                while (server.ReadMessage(readBuffer, out type, out sender))
                {
                    switch (type)
                    {
                        case NetMessageType.DebugMessage:
                            Debug.Print(readBuffer.ReadString(), ConsoleColor.White);
                            break;
                        case NetMessageType.ConnectionApproval:
                            Debug.Print(string.Format("Connection Approval from {0}: {1}",sender, readBuffer.ReadString()), ConsoleColor.DarkMagenta);
                            break;
                        case NetMessageType.StatusChanged:
                            string statusMessage = readBuffer.ReadString();
                            NetConnectionStatus newStatus = (NetConnectionStatus)readBuffer.ReadByte();
                            Debug.Print(string.Format("New status from {0} : {1} ( '{2}' )", sender, newStatus, statusMessage), ConsoleColor.DarkRed);
                            break;
                        case NetMessageType.Data:
                            string msg = readBuffer.ReadString();
                            decideCommand(msg, sender);
                            Debug.Print(string.Format("New message from {0}: {1}", sender, msg), ConsoleColor.Cyan);
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
            server.Shutdown("Server has been exited by the client");
        }

        private static void decideCommand(string command, NetConnection client)
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

        private static void Send(string information, NetConnection client)
        {
            NetBuffer prepareMessage = server.CreateBuffer();
            prepareMessage.Write(information);
            server.SendMessage(prepareMessage, client, NetChannel.ReliableInOrder1);
        }

        private static void SendToAll( string information )
        {
            NetBuffer prepareMessage = server.CreateBuffer();
            prepareMessage.Write(information);
            server.SendToAll(prepareMessage, NetChannel.ReliableInOrder1);
        }

        private static List<String> connectionsList()
        {
            List<String> connections = new List<String>();

            foreach (NetConnection client in server.Connections)
            {
                connections.Add( client.RemoteEndpoint.Address.ToString() );
            }

            return connections;
        }
    }
}
