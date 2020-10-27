﻿using server.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Classes
{
	[Serializable]
	public class Packet
	{
		public bool notConnected = false;

		public enum Header
		{
			DefaultDONOTUSE,
			Auth,
			Position,
			Request,
			Server
		}
		//User packet
		public Header header;
		public string username;
		public string password;

		public Transform transform;

		public Request request;

		public string uuid;

		//Server packet
		public List<Profile> profile;

		public Packet()
		{
			header = Header.DefaultDONOTUSE;
			username = "";
			password = "";
			transform = new Transform();
			request = new Request();
		}
		public Packet(bool connected)
		{
			notConnected = connected;
			header = Header.DefaultDONOTUSE;
			username = "";
			password = "";
			transform = new Transform();
			request = new Request();
		}
	}
}
