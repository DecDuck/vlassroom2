using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;
using Vlassroom_Library;
using System.Linq;
using TMPro;
using Server.Classes;
using server.Classes;

public class MasterGameManager : MonoBehaviour
{
	public static MasterGameManager stn
	{
		get { return FindObjectOfType<MasterGameManager>(); }
	}

	public NetworkingManager manager;

	public TMP_InputField username;
	public TMP_InputField password;

	public int packetUpdate = 1;

	public float packetTimer;

	public LoginCreatureComforts loginCreatureComforts;

	public StudentConfigController studentConfigController;
	public TeacherConfigController teacherConfigController;
	public PrincipalConfigController principalConfigController;

	public void Start()
	{
		manager = GetComponent<NetworkingManager>();
		loginCreatureComforts = FindObjectOfType<LoginCreatureComforts>();
		DontDestroyOnLoad(this.gameObject);
		manager.Connect();
	}
	public void UpdateConfig()
	{
		if (manager.profile.loggedIn)
		{
			switch (manager.profile.permissions)
			{
				case server.Classes.Permissions.Permission.Student:
					studentConfigController.gameObject.SetActive(true);
					loginCreatureComforts.gameObject.SetActive(false);
					studentConfigController.Load();
					studentConfigController.usernameText.text = "Logged in as: " + manager.profile.username;
					break;
				case server.Classes.Permissions.Permission.Teacher:
					teacherConfigController.gameObject.SetActive(true);
					loginCreatureComforts.gameObject.SetActive(false);
					teacherConfigController.Load();
					teacherConfigController.usernameText.text = "Logged in as: " + manager.profile.username;
					break;
				case server.Classes.Permissions.Permission.Principal:
					principalConfigController.gameObject.SetActive(true);
					loginCreatureComforts.gameObject.SetActive(false);
					principalConfigController.Load();
					principalConfigController.usernameText.text = "Logged in as: " + manager.profile.username;
					break;
			}
		}
	}
	public void FixedUpdate()
	{
		packetTimer += Time.deltaTime;
		if(packetTimer > packetUpdate)
		{
			PacketUpdate();
			packetTimer = 0.0f;
		}
	}
	public void PacketUpdate()
	{
		if (manager.toBeHandledPackets.Count > 0)
		{
			manager.HandlePacket(manager.toBeHandledPackets[manager.toBeHandledPackets.Count - 1]);
		}
	}

	public void CreateClassroom()
	{
		if(manager.profile.permissions == server.Classes.Permissions.Permission.Principal || manager.profile.permissions == server.Classes.Permissions.Permission.Teacher)
		{
			Packet packet = new Packet();
			packet.header = Packet.Header.Request;
			packet.request.request = Request.RequestType.CreateClassroom;
			packet.uuid = manager.profile.uuid;
			manager.netIO.toWrite = packet;
			manager.netIO.toDo.Add(NetworkIO.Operation.Write);
		}
	}

	public void JoinClassroom()
	{

	}

	public void CreateUser()
	{
		if(manager.profile.permissions == server.Classes.Permissions.Permission.Principal)
		{
			Packet packet = new Packet();
			packet.header = Packet.Header.Request;
			packet.request.request = Request.RequestType.CreateUser;
			packet.uuid = manager.profile.uuid;
			//Debug.Log(manager.profile.uuid);
			packet.username = principalConfigController.newUserUsername.text;
			packet.password = principalConfigController.newUserPasssword.text;
			packet.permission = (server.Classes.Permissions.Permission)principalConfigController.newUserPermission.value;
			manager.netIO.toWrite = packet;
			manager.netIO.toDo.Add(NetworkIO.Operation.Write);
		}
	}

	public void SetCredential()
	{
		manager.profile.username = username.text;
		manager.profile.password = password.text;
		Debug.Log("Set Credentials");
	}
}
