using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class OnlineManager : MonoBehaviourPunCallbacks
{
	[SerializeField]
	GameSettings gameSettings;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Connecting to master");
		PhotonNetwork.AutomaticallySyncScene = true;
		PhotonNetwork.NickName = gameSettings.NickName;
        PhotonNetwork.GameVersion = "0.0.1";
        PhotonNetwork.ConnectUsingSettings();
    }

	public override void OnConnectedToMaster()
	{
		Debug.Log("OnConnected to master");
	}
	public override void OnDisconnected(DisconnectCause cause)
	{
		Debug.Log("Disconnected: " + cause.ToString());
	}
	public void FindGame()
	{
		if(!PhotonNetwork.IsConnected)
			return;		
		PhotonNetwork.JoinRandomRoom();
	}
	void CreateRoom()
	{
		//Creamos las opciones de la sala
		RoomOptions roomOptions = new RoomOptions();
		//Numero maxiomo de jugadores
		roomOptions.MaxPlayers = 2;
		//Tiempo de espera antes de hechar a un jugador estar desconectado
		roomOptions.PlayerTtl = 30;
		//Tiempo de espera antes de eliminar la sala cuando no hay ningun jugador conectado
		roomOptions.EmptyRoomTtl = 30;
		PhotonNetwork.CreateRoom(null,roomOptions,null);
	}
	public override void OnPlayerEnteredRoom(Player newPlayer)
	{
		if(PhotonNetwork.CurrentRoom.PlayerCount == 2)
			StartGame();
	}
	public override void OnJoinRandomFailed(short returnCode, string message)
	{
		CreateRoom();
	}
	public override void OnCreatedRoom()
	{
		Debug.Log("Room Created Successfully");
	}
	public override void OnCreateRoomFailed(short returnCode, string message)
	{
		Debug.Log("Room Creation failed " + message);
	}
	void StartGame()
	{
		PhotonNetwork.CurrentRoom.IsOpen = false;
		PhotonNetwork.CurrentRoom.IsVisible = false;
		PhotonNetwork.LoadLevel(1);
	}
	#region
	public GameSettings GameSettings{get{return gameSettings;}}
	#endregion
}
