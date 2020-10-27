using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SqlServer.Server;
using server.Classes;
using Server.Classes;

namespace server.Classes
{
	class CommandManager
	{
		public string currentCommand = "";

		private int oldCommandLength = 0;

		public bool newCommand = true;

		public void AddKey(char key)
		{
			if(key == '\b')
			{
				if(currentCommand.Length > 0)
				{
					currentCommand = currentCommand.Substring(0, currentCommand.Length - 1);
				}
			}
			else if (key.ToString() == "\r")
			{
				ExecuteCommand();
			}
			else
			{
				currentCommand = currentCommand + key.ToString();
			}

			//Server.WriteLine(currentCommand);
			if(!newCommand)
			{
				Console.SetCursorPosition(0, Console.CursorTop - 1);
			}
			if (oldCommandLength > currentCommand.Length)
			{
				Console.WriteLine(">" + currentCommand + " ");
				oldCommandLength = currentCommand.Length;
			}
			else
			{
				Console.WriteLine(">" + currentCommand);
				oldCommandLength = currentCommand.Length;
			}
			newCommand = false;
		}

		public void ExecuteCommand()
		{
			string[] args = currentCommand.Split(' ');
			string command = args[0];

			try
			{
				switch (command.ToLower())
				{
					case "addauth":
						string username = args[1];
						string password = Server.manager.GetHashString(args[2]);
						Server.manager.table.AddToTable(username, password);
						Server.WriteLine("Added " + username + " to AuthTable");
						break;
					case "save":
						string savetype = args[1];
						if (savetype.ToLower() == "authtable")
						{
							IO.StoreAuthTable(Server.manager.table);
						}
						if (savetype.ToLower() == "config")
						{
							IO.StoreConfig();
						}
						break;
					case "load":
						string loadtype = args[1];
						if (loadtype.ToLower() == "authtable")
						{
							IO.ReadAuthTable();
						}
						if (loadtype.ToLower() == "config")
						{
							IO.ReadConfig();
						}
						break;
					case "stop":
						Server.Stop();
						break;
					default:
						Server.WriteLine("Unknown command: " + command.ToLower());
						break;
				}
			}catch (IndexOutOfRangeException e){
				Server.WriteLine("Not enough arguments");
			}
			currentCommand = "";
			newCommand = true;
		}
		
	}
}
