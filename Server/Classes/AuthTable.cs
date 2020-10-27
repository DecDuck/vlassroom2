using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace server.Classes
{
	public class AuthTable
	{
		private Dictionary<string, string> table = new Dictionary<string, string>();

		public void AddToTable(string username, string hashPassword)
		{
			table.Add(username, hashPassword);
		}

		public string ToJson()
		{
			return JsonConvert.SerializeObject(this);
		}
		public void FromJson(string json)
		{
			table = JsonConvert.DeserializeObject<AuthTable>(json).table;
		}
		public Dictionary<string, string> authTable
		{
			get { return table; }
		}
	}
}
