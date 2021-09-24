using System;  
using System.Collections.Generic;  
using System.Net;  
using System.Net.Sockets;  
using System.IO;  
using MyLibrary;
using System.Threading; 
using System.Runtime.Serialization.Formatters.Binary;
namespace Server  
{
    class HandleClient
    {
        Program program = new Program();
        public void BroadCast(string msg)  
        {  
            foreach (TcpClient client in Program.tcpClientsList)  
            {   
                //broadcast to all client except sender
                StreamWriter sWriter = new StreamWriter(client.GetStream());  
                sWriter.WriteLine(msg);  
                sWriter.Flush();    
            }  
        }

        public void ClientListener(object obj)  
        {  
            TcpClient tcpClient = (TcpClient)obj;  
            
            while (true)  
            {
                try
                {
                    BinaryFormatter binaryFormatter = new BinaryFormatter();
                    PlayerState playerState = new PlayerState();
                    playerState = (PlayerState) binaryFormatter.Deserialize(tcpClient.GetStream());
                    
                    switch (playerState.State)
                    {
                        case(PlayerState.state.CLONE):
                        {
                            BroadCast("A Clone joined the battle!");
                            CloneTrooper cloneTrooper = new CloneTrooper();
                            cloneTrooper = (CloneTrooper) binaryFormatter.Deserialize(tcpClient.GetStream());
                            BroadCast("Welcome "+cloneTrooper.legion+" "+ cloneTrooper.rank);
                            Console.WriteLine("Clone "+cloneTrooper.legion+" "+ cloneTrooper.rank+" joined the battle! Hit the clankers!");
                            break;
                        }
                        case(PlayerState.state.DROID):
                        {
                            BroadCast("A Droid joined the battle!");
                            BattleDroid battleDroid = new BattleDroid();
                            battleDroid = (BattleDroid) binaryFormatter.Deserialize(tcpClient.GetStream());
                            BroadCast("A "+battleDroid.type+" number "+ battleDroid.serialNumber+" Roger! Roger!");
                            Console.WriteLine("Droid "+battleDroid.type+" "+ battleDroid.serialNumber+" joined the battle! Destroy those clones!");
                            break;                
                        }
                    }

                }
                catch (Exception e)  
                {  
                    Console.WriteLine(e.Message);  
                    program.removeClient(tcpClient);
                    BroadCast("A player has left the server");
                    break;  
                }  
            }
        }
    }  
    class Program  
    {  
        public static TcpListener tcpListener;  
        public static List<TcpClient> tcpClientsList = new List<TcpClient>();  

        static void Main(string[] args)  
        {
            HandleClient handleClient = new HandleClient();

            //start process
            tcpListener = new TcpListener(IPAddress.Any, 5000);  
            tcpListener.Start();  
            Console.WriteLine("Server created");
            while (true)  
            {
                //add clients to list
                TcpClient tcpClient = tcpListener.AcceptTcpClient();  
                tcpClientsList.Add(tcpClient);

                //start listener
                Thread startListen = new Thread(() => handleClient.ClientListener(tcpClient));
                startListen.Start();
            }          
        }

        public void removeClient(TcpClient client)
        {
            tcpClientsList.Remove(client);
        }
    }  
} 