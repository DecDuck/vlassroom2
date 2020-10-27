using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO;
using System.Runtime.Remoting;
using System.Net;
using System.Security;
using Microsoft.SqlServer.Server;
using Vlassroom_Library;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using server;
using server.Classes;
using System.Net.Mail;

namespace server
{
	class Server
	{
		public static NetworkingManager manager;
		public static Config config;

		public static void Main(string[] args)
		{
			GetConfig();
			manager = new NetworkingManager();
			manager.StartListening();
			manager.StartAsyncRecieve();
			while (true)
			{
				if(manager.toBeHandledClient.Count > 0)
				{
					manager.HandleClient(manager.toBeHandledClient[0]);
				}
				if(manager.toBeHandledPackets.Count > 0)
				{
					manager.HandlePacket(manager.toBeHandledPackets[0]);
				}
			}
		}

		public static void GetConfig()
		{
			if(File.Exists(Directory.GetCurrentDirectory() + "/config.json"))
			{
				config = IO.ReadConfig();
			}
			else
			{
				config = new Config();
				IO.StoreConfig();
			}
		}

		public static void WriteLine(object toPrint)
		{
			Console.WriteLine("[" + DateTime.UtcNow + "] " + toPrint.ToString());
		}
	}
}
