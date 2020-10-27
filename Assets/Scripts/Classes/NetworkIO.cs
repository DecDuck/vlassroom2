using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Classes
{
	[Serializable]
	public class NetworkIO
	{
		public enum Operation
		{
			Read,
			Write
		}

		public List<Operation> toDo = new List<Operation>();

		public Packet toWrite;
	}
}
