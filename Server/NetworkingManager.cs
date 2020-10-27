using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Schema;
using Newtonsoft.Json;
using server.Classes;
using Server.Classes;
using System.Security.Cryptography;

namespace server
{
	class NetworkingManager
	{
		public AuthTable table;

		public TcpListener tcpListener = new TcpListener(40705);
		public List<Profile> toBeHandledClient = new List<Profile>();
		public Dictionary<string, Profile> activePeople = new Dictionary<string, Profile>();
		public List<Packet> toBeHandledPackets = new List<Packet>();

		public bool shuttingDown = false;

		public NetworkIO netIO;

		public async Task sendMessageAsync(Packet packet, TcpClient client)
		{
			if (!client.Connected)
			{
				Server.WriteLine("Client Disconnected");
				return;
			}
			string stringPacket = JsonConvert.SerializeObject(packet);
			Server.WriteLine("Sending: " + stringPacket);
			byte[] bytePacket = UnicodeEncoding.Unicode.GetBytes(stringPacket);
			byte[] messageLength = BitConverter.GetBytes((Int64)bytePacket.Length);

			ByteAssembler assembler = new ByteAssembler(bytePacket.Length + messageLength.Length);
			assembler.AddByteArray(messageLength, messageLength.Length);
			assembler.AddByteArray(bytePacket, bytePacket.Length);

			byte[] message = assembler.Create();

			await client.GetStream().WriteAsync(message, 0, message.Length);
			//await client.GetStream().WriteAsync(Encoding.Unicode.GetBytes("hello from the other sideee"), 0, 8);
			Server.WriteLine("Sent. (from sendMessage()) " + message.Length);
		}
		public async Task<Packet> readMessageAsync(TcpClient client)
		{
			ByteAssembler assembler = new ByteAssembler(8);
			if (!client.Connected)
			{
				Server.WriteLine("Client Disconnected");
				return new Packet(false);
			}
			while (!assembler.isFull)
			{
				byte[] readingBytes = new byte[8];
				int usingInt = client.GetStream().Read(readingBytes, 0, 8);
				assembler.AddByteArray(readingBytes, usingInt);
			}
			byte[] byteMessageLength = assembler.Create();
			
			Int64 messageLength = (Int64)BitConverter.ToInt64(byteMessageLength, 0);

			Server.WriteLine("Reading " + messageLength.ToString() + " bytes of data");

			assembler.Clear(messageLength);

			while (client.Available < messageLength)
			{

			}

			while (!assembler.isFull)
			{
				byte[] byteMessage = new byte[messageLength];
				int usingInt = client.GetStream().Read(byteMessage, 0, byteMessage.Length);
				assembler.AddByteArray(byteMessage, usingInt);
			}
			string stringMessage = UnicodeEncoding.Unicode.GetString(assembler.Create());

			//Server.WriteLine("Read.");

			return JsonConvert.DeserializeObject<Packet>(stringMessage);
		}

		public void StartListening()
		{
			GetAuth();
			netIO = new NetworkIO();
			Server.WriteLine("Created NetworkIO manager");
			tcpListener.Start();
			Server.WriteLine("Started Listening");
		}
		public void StopListening()
		{
			shuttingDown = true;
			Server.WriteLine("Stopping recieving");
			tcpListener.Stop();
			Server.WriteLine("Stopped recieving");
		}

		public void GetAuth()
		{
			if (File.Exists(Directory.GetCurrentDirectory() + Server.config.storeFile))
			{
				Server.WriteLine("Attempting to get AuthTable");
				table = IO.ReadAuthTable();
				Server.WriteLine("AuthTable read. Contents: " + JsonConvert.SerializeObject(table));
			}
			else
			{
				Server.WriteLine("Attempting to create new AuthTable");
				table = new AuthTable();
				IO.StoreAuthTable(table);
			}
		}

		public void BackgroundRead(Profile pclient)
		{
			TcpClient client = pclient.client;
			Server.WriteLine("Started (Background Packet Read) Thread");
			try
			{
				while (client.Connected)
				{
					if (shuttingDown)
					{
						Server.WriteLine("Stopped (Background Packet Read) Thread");
						return;
					}
					if (!netIO.toDo[pclient.uuid].Contains(NetworkIO.Operation.Read))
					{
						netIO.toDo[pclient.uuid].Add(NetworkIO.Operation.Read);
					}
					else if(netIO.toDo.Count > 0)
					{
						if(netIO.toDo[pclient.uuid][0] == NetworkIO.Operation.Read)
						{
							//Server.WriteLine("Called a read");
							if(client.Available > 0)
							{
								Packet packet = readMessageAsync(client).Result;
								if (packet == null)
								{
									break;
								}
								toBeHandledPackets.Add(packet);
								Server.WriteLine("Recieved Packet from " + ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString());
							}
							netIO.toDo[pclient.uuid].RemoveAt(0);
						}
					}
				}
				Server.WriteLine("Client Disconnected (Background Packet Read) Thread");
			}
			catch (Exception e)
			{
				Server.WriteLine(e.ToString());
			}
			
		}
		public void BackgroundWrite(Profile pclient)
		{
			TcpClient client = pclient.client;
			Server.WriteLine("Started (Background Packet Write) Thread");
			try
			{
				while (client.Connected)
				{
					if (shuttingDown)
					{
						Server.WriteLine("Stopped (Background Packet Write) Thread");
						return;
					}
					if (netIO.toDo[pclient.uuid].Count > 0)
					{
						//Server.WriteLine(netIO.toDo[pclient.uuid].ToArray().ToString());
						//Server.WriteLine(JsonConvert.SerializeObject(netIO.toDo[pclient.uuid].ToArray()));
						if (netIO.toDo[pclient.uuid].IndexOf(NetworkIO.Operation.Write) == 0)
						{
							//Server.WriteLine("Sending packet...");
							sendMessageAsync(netIO.toWrite[0], client);
							netIO.toDo[pclient.uuid].RemoveAt(0);
							Server.WriteLine("Sent Packet to " + ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString());
						}
					}
				}
				Server.WriteLine("Client Disconnected (Background Packet Recieve) Thread");
			}
			catch (Exception e)
			{
				Server.WriteLine(e.ToString());
			}

		}

		public void HandlePacket(Packet packet)
		{
			if(packet == null)
			{
				return;
			}
			if (packet.header == Packet.Header.Request)
			{
				switch (packet.request.request)
				{
					//Server should not send this
					case Request.RequestType.CreateClassroom:
						break;
					//Server should not send this
					case Request.RequestType.JoinClassroom:
						break;
					//Server should not send this
					case Request.RequestType.LeaveClassroom:
						break;
					case Request.RequestType.AuthRequest:
						break;
				}
			}
			else if (packet.header == Packet.Header.Auth)
			{
				Server.WriteLine("Recieved Auth Packet");
				if(activePeople[packet.uuid] != null)
				{
					if (table.authTable.ContainsKey(packet.username))
					{
						if(table.authTable[packet.username] == GetHashString(packet.password))
						{
							activePeople[packet.uuid].username = packet.username;
							activePeople[packet.uuid].password = packet.password;
							activePeople[packet.uuid].loggedIn = true;
						}
						else
						{
							Server.WriteLine("Password is not correct " + packet.password);
							Authenticate(activePeople[packet.uuid]);
						}
					}
					else
					{
						Server.WriteLine("Username does not have an entry " + packet.username);
						Authenticate(activePeople[packet.uuid]);
					}
				}
				toBeHandledPackets.Remove(packet);
			}
		}
		public byte[] GetHash(string inputString)
		{
			using (HashAlgorithm algorithm = SHA256.Create())
				return algorithm.ComputeHash(Encoding.UTF8.GetBytes(inputString));
		}

		public string GetHashString(string inputString)
		{
			StringBuilder sb = new StringBuilder();
			foreach (byte b in GetHash(inputString))
				sb.Append(b.ToString("X2"));

			return sb.ToString();
		}


		public void StartAsyncRecieve()
		{
			Server.WriteLine("Started (Background Client Connection Creation) Thread");
			new Thread(() => Recieve()).Start();
		}

		public void Recieve()
		{
			while (!shuttingDown)
			{
				if (shuttingDown)
				{
					return;
				}
				try
				{
					TcpClient handledClient = tcpListener.AcceptTcpClient();
					Server.WriteLine("Client Connected");
					Profile profile = new Profile();
					profile.Setup();
					profile.uuid = (DateTime.Now.ToString() + new Random().Next(1000, 9999).ToString());
					profile.client = handledClient;
					netIO.toDo.Add(profile.uuid, new List<NetworkIO.Operation>());
					new Thread(() => BackgroundRead(profile)).Start();
					new Thread(() => BackgroundWrite(profile)).Start();
					toBeHandledClient.Add(profile);
				}
				catch(Exception e)
				{
					return;
				}
				
				//Server.WriteLine("Client Connect Info: " + JsonConvert.SerializeObject(profile));
			}
			Server.WriteLine("Stopped Background Recieve");
		}

		public void HandleClient(Profile pclient)
		{
			if(pclient.username == "")
			{
				Authenticate(pclient);
			}
			toBeHandledClient.Remove(pclient);
		}

		public void Authenticate(Profile pclient)
		{
			Thread.Sleep(pclient.waitTime);
			TcpClient client = pclient.client;
			//Server.WriteLine("Requesting Auth from " + ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString());
			Packet authRequest = new Packet();
			authRequest.header = Packet.Header.Request;
			authRequest.uuid = pclient.uuid;
			authRequest.request.request = Request.RequestType.AuthRequest;
			netIO.toWrite.Add(authRequest);
			netIO.toDo[pclient.uuid].Add(NetworkIO.Operation.Write);
			if (!activePeople.Keys.Contains(pclient.uuid))
			{
				activePeople.Add(pclient.uuid, pclient);
			}
			Server.WriteLine("Requested Auth from " + ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString());
		}
	}
}
