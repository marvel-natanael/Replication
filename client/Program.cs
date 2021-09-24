using System;  
using System.Net.Sockets;  
using System.IO;  
using MyLibrary;
using System.Threading;
using System.Runtime.Serialization;  
using System.Runtime.Serialization.Formatters.Binary; 
namespace Client
{
	class Read
		{
			public void ReadMessage(object obj)
			{
				TcpClient tcpClient = (TcpClient)obj;  
				StreamReader streamReader = new StreamReader(tcpClient.GetStream());  
	
				while (true)  
				{  
					try  
					{ 
						//read incoming message 
						string message = streamReader.ReadLine();  
						Console.WriteLine(message);  
					}  
					catch (Exception e)  
					{  
						Console.WriteLine(e.Message);  
						break;  
					}  
				}             
			}
		}
	class Program
	{
		static void Main(string[] args)
		{
			Read read = new Read();
			try
			{
				TcpClient tcpClient = new TcpClient("127.0.0.1", 5000);

				Thread thread = new Thread(read.ReadMessage);
				thread.Start(tcpClient);

				NetworkStream networkStream = tcpClient.GetStream();
				while (true)
				{
					if(tcpClient.Connected)
					{
						Console.WriteLine("Choose your faction!\n1.CLONE\n2.DROID");
						string input = Console.ReadLine();
						PlayerState myState = new PlayerState();
						BinaryFormatter binaryFormatter = new BinaryFormatter();
						switch (int.Parse(input))
						{
							case(1):
							{
								CloneTrooper cloneTrooper = new CloneTrooper();
								cloneTrooper.State = PlayerState.state.CLONE;
								binaryFormatter.Serialize(networkStream, cloneTrooper);
								//there should be reply here
								Console.WriteLine("What's your rank?");
								cloneTrooper.rank = Console.ReadLine();
								Console.WriteLine("What's your legion?");
								cloneTrooper.legion = Console.ReadLine();
								//confirmation
								binaryFormatter.Serialize(networkStream, cloneTrooper);
								break;
							}
							case(2):
							{
								BattleDroid battleDroid = new BattleDroid();
								battleDroid.State = PlayerState.state.DROID;
								binaryFormatter.Serialize(networkStream, battleDroid);
								//reply here
								Console.WriteLine("What's your serial number?");
								battleDroid.serialNumber = int.Parse(Console.ReadLine());
								Console.WriteLine("What's your type?");
								battleDroid.type = Console.ReadLine();
								//confirmation
								binaryFormatter.Serialize(networkStream, battleDroid);
								break;
							}
							default:
							{
								Console.WriteLine("Wrong Type!");
								break;
							}
						}
						Console.ReadLine();
					}
				}
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
			}
		}
	}
}