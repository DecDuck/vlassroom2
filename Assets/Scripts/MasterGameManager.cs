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

public class MasterGameManager : MonoBehaviour
{
	public static MasterGameManager stn
	{
		get { return FindObjectOfType<MasterGameManager>(); }
	}

	public NetworkingManager manager
	{
		get { return GetComponent<NetworkingManager>(); }
	}

	public TMP_InputField username;
	public TMP_InputField password;

	public void Start()
	{
		DontDestroyOnLoad(this.gameObject);
		manager.Connect();
	}
	public void Update()
	{
		if(manager.toBeHandledPackets.Count > 0)
		{
			manager.StartHandlePacketAsync(manager.toBeHandledPackets[0]);
		}
	}

	public void SetCredential()
	{
		manager.profile.username = username.text;
		manager.profile.password = password.text;
		Debug.Log("Set Credentials");
	}
}
