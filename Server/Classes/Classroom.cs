using server.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace server.Classes
{
	class Classroom
	{
		public List<Profile> connectedPeople = new List<Profile>();

		public string classCode = "";

		public Classroom(Classtype type)
		{
			classCode = ((int)type).ToString() + RandomString(5);
		}

		public enum Classtype
		{
			Standard
		}

		private string RandomString(int length)
		{
			const string pool = "abcdefghijklmnopqrstuvwxyz0123456789";
			var builder = new StringBuilder();

			for (var i = 0; i < length; i++)
			{
				var c = pool[new Random().Next(0, pool.Length)];
				builder.Append(c);
			}

			return builder.ToString();
		}
	}

	
}
