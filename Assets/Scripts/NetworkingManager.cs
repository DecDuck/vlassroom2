using Newtonsoft.Json;
using server.Classes;
using Server.Classes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class NetworkingManager : MonoBehaviour
{
    public Profile profile;

    public int port = 40705;
    public string hostname = "127.0.0.1";

	[SerializeField]
	public List<Packet> toBeHandledPackets = new List<Packet>();

	//For Debugging purposes
	public int toWriteLength;

	public NetworkIO netIO;

	[SerializeField]
	private string lastAttemptedUsername = "username";
	[SerializeField]
	private string lastAttemptedPassword = "password";

    public void Connect()
    {
		netIO = new NetworkIO();
        profile = new Profile();
        profile.Setup();
        profile.client.Connect(hostname, port);
        Debug.Log("Connected");
		new Thread(() => BackgroundRead(profile.client)).Start();
		new Thread(() => BackgroundWrite(profile.client)).Start();
	}

	public async Task sendMessageAsync(Packet packet, TcpClient client)
	{
		if (!client.Connected)
		{
			//Server.WriteLine("Client Disconnected");
			return;
		}
		string stringPacket = JsonConvert.SerializeObject(packet);
		///Server.WriteLine("Sending: " + stringPacket);
		byte[] bytePacket = UnicodeEncoding.Unicode.GetBytes(stringPacket);
		byte[] messageLength = BitConverter.GetBytes((Int64)bytePacket.Length);

		ByteAssembler assembler = new ByteAssembler(bytePacket.Length + messageLength.Length);
		assembler.AddByteArray(messageLength, messageLength.Length);
		assembler.AddByteArray(bytePacket, bytePacket.Length);

		byte[] message = assembler.Create();

		await client.GetStream().WriteAsync(message, 0, message.Length);
		//await client.GetStream().WriteAsync(Encoding.Unicode.GetBytes("hello from the other sideee"), 0, 8);
		//Server.WriteLine("Sent. (from sendMessage()) " + message.Length);
	}
	public async Task<Packet> readMessageAsync(TcpClient client)
	{
		ByteAssembler assembler = new ByteAssembler(8);
		if (!client.Connected)
		{
			//Server.WriteLine("Client Disconnected");
			return new Packet(false);
		}
		while (!assembler.isFull)
		{
			//Server.WriteLine("iteration");
			byte[] readingBytes = new byte[8];
			int usingInt = client.GetStream().Read(readingBytes, 0, 8);
			//Debug.LogError(usingInt + ": Using Int");
			assembler.AddByteArray(readingBytes, usingInt);
		}
		byte[] byteMessageLength = assembler.Create();

		//Debug.LogError(byteMessageLength.Length);

		Int64 messageLength = (Int64)BitConverter.ToInt64(byteMessageLength, 0);

		//Server.WriteLine("Reading " + messageLength.ToString() + " bytes of data");

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

	//Called once in seperate thread.
	public void BackgroundRead(TcpClient client)
	{
		Debug.Log("Started (Background Packet Recieve) Thread");
		try
		{
			while (client.Connected)
			{
				if (!netIO.toDo.Contains(NetworkIO.Operation.Read))
				{
					netIO.toDo.Add(NetworkIO.Operation.Read);
				}
				else if (netIO.toDo.Count > 0)
				{
					if (netIO.toDo[0] == NetworkIO.Operation.Read)
					{
						if(client.Available > 0)
						{
							Packet packet = readMessageAsync(client).Result;
							if (packet == null)
							{
								break;
							}
							toBeHandledPackets.Add(packet);
							Debug.Log("Recieved Packet from " + ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString());
						}
						netIO.toDo.RemoveAt(0);
					}
				}
			}
			Debug.Log("Client Disconnected (Background Packet Recieve) Thread");
		}
		catch (Exception e)
		{
			Debug.Log(e.ToString());
		}

	}

	//Called once in seperate thread.
	public void BackgroundWrite(TcpClient client)
	{
		Debug.Log("Started (Background Packet Write) Thread");
		try
		{
			while (client.Connected)
			{
				toWriteLength = netIO.toWrite.Count;
				if (netIO.toDo.Count > 0)
				{
					if (netIO.toDo.IndexOf(NetworkIO.Operation.Write) == 0)
					{
						sendMessageAsync(netIO.toWrite[0], client);
						netIO.toDo.RemoveAt(0);
						//netIO.toWrite.RemoveAt(0);
						Debug.Log("Sent Packet to " + ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString());
					}
				}
			}
			Debug.Log("Client Disconnected (Background Packet Recieve) Thread");
		}
		catch (Exception e)
		{
			Debug.Log(e.ToString());
			Debug.Log(netIO.toDo.Count);
		}
	}

	public void HandlePacketAsync(Packet packet)
	{
		new Thread(() => HandlePacket(packet));
	}

	//Handles packets
	public bool HandlePacket(Packet packet)
	{
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
					if (profile.username != lastAttemptedUsername || profile.password != lastAttemptedPassword)
					{
						Packet toSend = new Packet();
						profile.uuid = packet.uuid;
						toSend.username = profile.username;
						toSend.password = profile.password;
						toSend.header = Packet.Header.Auth;
						toSend.uuid = profile.uuid;
						netIO.toDo.Add(NetworkIO.Operation.Write);
						netIO.toWrite.Add(toSend);
						toBeHandledPackets.Remove(packet);
						lastAttemptedUsername = profile.username;
						lastAttemptedPassword = profile.password;
						return true;
					}
					break;
			}
		}
		return false;
	}
}
