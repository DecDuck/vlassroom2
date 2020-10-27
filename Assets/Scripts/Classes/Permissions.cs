using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace server.Classes
{
	[Serializable]
	public class Permissions
	{
		public enum Permission
		{
			Principal,
			Teacher,
			Student
		}
	}
}
