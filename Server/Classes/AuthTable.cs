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
		private Dictionary<string, Permissions.Permission> permissionTable = new Dictionary<string, Permissions.Permission>();

		public void AddToTable(string username, string hashPassword, int permission)
		{
			table.Add(username, hashPassword);
			permissionTable.Add(username, (Permissions.Permission)permission);
		}

		public string ToJson()
		{
			return JsonConvert.SerializeObject(this);
		}
		public void FromJson(string json)
		{
			table = JsonConvert.DeserializeObject<AuthTable>(json).table;
			permissionTable = JsonConvert.DeserializeObject<AuthTable>(json).perTable;
		}
		public Dictionary<string, string> authTable
		{
			get { return table; }
		}
		public Dictionary<string, Permissions.Permission> perTable
		{
			get { return permissionTable; }
		}
	}
}
