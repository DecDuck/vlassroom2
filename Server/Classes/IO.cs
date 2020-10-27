using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace server.Classes
{
	class IO
	{

		public static void StoreAuthTable(AuthTable table)
		{
			using (StreamWriter writer = new StreamWriter(Directory.GetCurrentDirectory() + Server.config.storeFile))
			{
				writer.Write(table.ToJson());
				writer.Close();
			}
			Server.WriteLine("Saved AuthTable");
		}
		public static AuthTable ReadAuthTable()
		{
			AuthTable table = new AuthTable();
			table.FromJson(File.ReadAllText(Directory.GetCurrentDirectory() + Server.config.storeFile));
			Server.WriteLine("Loaded AuthTable");
			return table;
		}
		public static void StoreConfig()
		{
			using (StreamWriter writer = new StreamWriter(Directory.GetCurrentDirectory() + "/config.json"))
			{
				writer.Write(JsonConvert.SerializeObject(Server.config));
				writer.Close();
			}
			Server.WriteLine("Saved Config");
		}
		public static Config ReadConfig()
		{
			Server.WriteLine("Loaded Config");
			return JsonConvert.DeserializeObject<Config>(File.ReadAllText(Directory.GetCurrentDirectory() + "/config.json"));
		}

		internal static void StoreConfig(object v)
		{
			throw new NotImplementedException();
		}
	}
}
