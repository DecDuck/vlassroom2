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
using Server.Classes;

namespace server
{
	class Server
	{
		public static NetworkingManager manager;
		public static Config config;
		public static CommandManager cManager;

		public static bool online = true;

		public static void Main(string[] args)
		{
			GetConfig();
			if (config.enableCommands)
			{
				cManager = new CommandManager();
				Server.WriteLine("Command Input enabled");
			}
			manager = new NetworkingManager();
			manager.StartListening();
			manager.StartAsyncRecieve();
			
			while (online)
			{
				if(manager.toBeHandledClient.Count > 0)
				{
					manager.HandleClient(manager.toBeHandledClient[0]);
				}
				if(manager.toBeHandledPackets.Count > 0)
				{
					manager.HandlePacket(manager.toBeHandledPackets[0]);
				}
				if (Console.KeyAvailable)
				{
					ConsoleKeyInfo key = Console.ReadKey(true);
					cManager.AddKey(key.KeyChar);
				}
			}
			Server.WriteLine("Command Input disabled");

			manager.StopListening();

			Server.WriteLine("Saving AuthTable");
			IO.StoreAuthTable(manager.table);
			Server.WriteLine("Saved AuthTable");

			Server.WriteLine("Saving Config");
			IO.StoreConfig();
			Server.WriteLine("Saved Config");

			Server.WriteLine("Server stopped");
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

		public static void Stop()
		{
			online = false;
		}

		public static void WriteLine(object toPrint)
		{
			Console.WriteLine("[" + DateTime.UtcNow + "] " + toPrint.ToString());
			if(cManager != null)
			{
				cManager.newCommand = true;
			}
		}
	}
}
