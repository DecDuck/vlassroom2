using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server.Classes
{
	class NetworkIO
	{
		public enum Operation
		{
			Read,
			Write
		}

		public Dictionary<string, List<Operation>> toDo = new Dictionary<string, List<Operation>>();

		public List<Packet> toWrite = new List<Packet>();
	}
}
