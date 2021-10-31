using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NetworkManager : MonoBehaviourPunCallbacks
{
	private const string PlayerRoomDetail = "roomdetails";
	private const string PlayerName = "playername";
	private const string WhitePlayerTrun= "whiteplayertrun";
	private const byte MAX_PLAYERS = 2;
	private string _PlayerName = "";
	public Text networkText = null;

	public static NetworkManager Instance = null;
	void Awake()
	{		
		PhotonNetwork.AutomaticallySyncScene = true;

		if(Instance==null)
        {
			Instance = this;
        }
        
	}

	//public void SetDependencies(MultiplayerChessGameController chessGameController)
	//{
	//	this.chessGameController = chessGameController;
	//}

	public void Connect()
	{
		print("Connect");

		if (PhotonNetwork.IsConnected)
		{
			PhotonNetwork.JoinRandomRoom();
			//PhotonNetwork.JoinRandomRoom(new ExitGames.Client.Photon.Hashtable() { { PlayerName, _PlayerName } }, MAX_PLAYERS);

		}
		else
		{
			PhotonNetwork.ConnectUsingSettings();
		}
	}

	private void Update()
	{
		networkText.text=PhotonNetwork.NetworkClientState.ToString();
		//Debug.Log("state : "+networkText.text);
	}

	#region Photon Callbacks

	public override void OnConnectedToMaster()
	{
		print("OnConnectedToMaster");

		Debug.LogError($"Connected to server. Looking for random room ");
		PhotonNetwork.JoinRandomRoom();
		//PhotonNetwork.JoinRandomRoom(new ExitGames.Client.Photon.Hashtable() { { PlayerName, _PlayerName } }, MAX_PLAYERS);

	}

	public override void OnJoinRandomFailed(short returnCode, string message)
	{
		print("OnJoinRandomFailed");

		Debug.LogError($"Joining random room failed becuse of {message}. Creating new one with player");
		PhotonNetwork.CreateRoom(null, new RoomOptions
		{
			CustomRoomPropertiesForLobby = new string[] { "3D-Chess" },
			IsOpen=true,
			IsVisible=true,
			MaxPlayers = MAX_PLAYERS,
			CustomRoomProperties = new ExitGames.Client.Photon.Hashtable() { { PlayerName, _PlayerName} }
		});
		//PhotonNetwork.CreateRoom(null);
	}

	public override void OnJoinedRoom()
	{
		print("OnJoinedRoom : "+ PhotonNetwork.IsMasterClient);
		MainMenu.OnJoinRoomCompletedEvent?.Invoke(true);
		Debug.LogError($"Player {PhotonNetwork.LocalPlayer.ActorNumber} joined a room with : {PhotonNetwork.CurrentRoom.CustomProperties[PlayerName]}");
		
		if (PhotonNetwork.CurrentRoom.PlayerCount > 1 && !PhotonNetwork.IsMasterClient)
			MainMenu.GameStartEvent?.Invoke();
	}

	public override void OnLeftRoom()
	{
		print("OnLeftRoom");
		PhotonNetwork.Disconnect();
		MainMenu.GameLoadDataEvent?.Invoke(false);
	}

	public override void OnPlayerEnteredRoom(Player newPlayer)
	{
		print("OnPlayerEnteredRoom");

		Debug.LogError($"Player {newPlayer.ActorNumber} entered a room {newPlayer.CustomProperties[PlayerName]}");

		if(PhotonNetwork.CurrentRoom.PlayerCount>1)
			MainMenu.GameStartEvent?.Invoke();
	}
	#endregion

	public void SetPlayerName(string name)
	{
		_PlayerName = name;
		PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable() { { PlayerName, _PlayerName} });
	}

	public string GetPlayerName()
	{		
		return (string)PhotonNetwork.LocalPlayer.CustomProperties[PlayerName];
	}

	public void SetWhitePlayerTurn(bool value)
	{
		PhotonNetwork.CurrentRoom.SetCustomProperties(new ExitGames.Client.Photon.Hashtable() { { WhitePlayerTrun, value } });
	}

	public bool GetWhitePlayerTrun()
	{
		return (bool)PhotonNetwork.CurrentRoom.CustomProperties[WhitePlayerTrun];
	}
	//public void SetPlayerTeam(int teamInt)
	//{
	//	if (PhotonNetwork.CurrentRoom.PlayerCount > 1)
	//	{
	//		var player = PhotonNetwork.CurrentRoom.GetPlayer(1);
	//		if (player.CustomProperties.ContainsKey(TEAM))
	//		{
	//			var occupiedTeam = player.CustomProperties[TEAM];
	//			teamInt = (int)occupiedTeam == 0 ? 1 : 0;
	//		}
	//	}
	//	PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable() { { TEAM, teamInt } });
	//	//gameInitializer.InitializeMultiplayerController();
	//	//chessGameController.SetupCamera((TeamColor)teamInt);
	//	//chessGameController.SetLocalPlayer((TeamColor)teamInt);
	//	//chessGameController.StartNewGame();
	//}



	internal bool IsRoomFull()
	{
		print("IsRoomFull");
		return PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers;
	}

}
