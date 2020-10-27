using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace server.Classes
{
	public class Profile
	{
		public string username;
		public string uuid;

		public string hashedPassword;

		public string password;

		public TcpClient client;

		public Avatar avatar;

		public Permissions.Permission permissions;

		public Transform transform;

		public CurrentAction action;

		public bool writeThreadStarted = false;
		public bool readThreadStarted = false;

		public bool loggedIn = false;
		public int waitTime = 200;

		public bool beenSetup
		{
			get { return writeThreadStarted && readThreadStarted; }
		}

		public void Setup()
		{
			username = "";
			uuid = "";

			hashedPassword = "";

			client = new TcpClient();

			avatar = new Avatar();

			permissions = Permissions.Permission.Student;

			transform = new Transform();

			action = new CurrentAction();
			action.inActive = false;
			action.isSitting = false;
			action.raisingHand = false;
			action.reading = false;
			action.talking = false;
			action.writing = false;
		}
	}
}
