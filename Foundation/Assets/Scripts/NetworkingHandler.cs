﻿using UnityEngine;
using System.Collections;

public class NetworkingHandler : MonoBehaviour {
	private int MAX_PLAYERS = 2;
    private string connectIP = "127.0.0.1";
	private int connectPort = 25001;
	private int NUM_PLAYERS = 0;
	private string playersField = "1";
	private bool hostButton, stopButton, connectButton, disconnectButton;

	void OnGUI() {
		if (Network.isServer) {
			// Player started a server

			GUILayout.Label("Hosting on " + Network.player.ipAddress);
			GUILayout.Label(NUM_PLAYERS + " player(s) connected");
			stopButton = GUILayout.Button("Stop");

			if (stopButton) {
				Network.Disconnect();
				Application.LoadLevel("game");
			}
		} else if (Network.isClient) {
			// Player connected to a server

			GUILayout.Label("Connected to " + Network.player.ipAddress);
			disconnectButton = GUILayout.Button("Disconnect");

			if (disconnectButton) {
				Network.Disconnect();
				Application.LoadLevel("game");
			}
		} else {
			// Player is has not started or connected to server

			// Host GUI
			GUILayout.Label("Number of Players");
			playersField = GUILayout.TextField(playersField);
			hostButton = GUILayout.Button("Start Server");
			if (hostButton) {
				int.TryParse(playersField, out NUM_PLAYERS);
				//Debug.Log("NUM_PLAYERS = " + NUM_PLAYERS);
				if (NUM_PLAYERS >= 1 && NUM_PLAYERS <= MAX_PLAYERS) {
					StartServer();
				}
			}

			// Connect GUI
			connectIP = GUILayout.TextField(connectIP);
			connectButton = GUILayout.Button("Connect");
			if (connectButton) {
				ConnectToServer();
			}
		}
	}

	void Start() {

	}

	private void StartServer()
    {
        Network.InitializeServer(10, connectPort, !Network.HavePublicAddress());
        Debug.Log("Player " + Network.player + " connected from " + Network.player.ipAddress + ":" + Network.player.port);
    }

    private void ConnectToServer() 
    {
    	Network.Connect(connectIP, connectPort);
    }

    void OnServerInitialized() 
	{
		Debug.Log("Server " + Network.player.ipAddress + ":" + Network.player.port + " initialized and ready");
		GameHandler.Instance.AddPlayer(Network.player);
	}

	void OnPlayerConnected(NetworkPlayer player) 
	{
		Debug.Log("Player " + Network.player + " connected from " + Network.player.ipAddress + ":" + Network.player.port);
		/*GameHandler.Instance.AddPlayerToServer(player);
		if (GameHandler.Instance.NUM_PLAYERS == this.NUM_PLAYERS) {
			Debug.Log("All players connected to server");
			GameHandler.Instance.AddPlayersToClients();
		}*/
	}

	void OnPlayerDisconnected(NetworkPlayer player) 
	{
		//GameHandler.Instance.RemovePlayer(player);
		//GameHandler.Instance.networkView.RPC("RemovePlayer", RPCMode.All, player);
		Debug.Log("Player disconnected from: " + player.ipAddress + ":" + player.port + " = " + player);
	}

	void OnConnectedToServer() 
	{
		Debug.Log ("This CLIENT has connected to server " + MasterServer.ipAddress + ":" + MasterServer.port);
		GameHandler.Instance.networkView.RPC("AddPlayer", RPCMode.Server, Network.player);
	}

	void OnDisconnectedFromServer(NetworkDisconnection info) 
	{
		Debug.Log("This CLIENT has disconnected from a server OR this SERVER was just shut down");
	}

	void OnFailedToConnect(NetworkConnectionError error)
	{
		Debug.Log("Could not connect to server: " + error);
	}
}