using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;

public class OnlineManager : MonoBehaviourPunCallbacks
{
	public static OnlineManager instance;
	[SerializeField]
	GameSettings gameSettings;
	
	private void Awake()
	{
		if(instance == null)
		{
			instance = this;
		}
		else
		{
			if(instance!=this)
			{
				Destroy(instance.gameObject);
				instance = this;
			}
		}
		DontDestroyOnLoad(this);
	}	
	public override void OnEnable()
	{
		base.OnEnable();
		SceneManager.sceneLoaded += OnSceneFinishedLoading;
	}
	public override void OnDisable()
	{
		base.OnDisable();
		SceneManager.sceneLoaded -= OnSceneFinishedLoading;
	}	
	void OnSceneFinishedLoading(Scene scene, LoadSceneMode mode)
	{
		if(scene.buildIndex == 1)
			CreatePlayer();
	}
	void CreatePlayer()
	{
		PhotonNetwork.Instantiate("MultiplayerPrefabs/Player",transform.position,Quaternion.identity,0);
			
	}
	// Start is called before the first frame update
	void Start()
    {
        Debug.Log("Connecting to master");		
		PhotonNetwork.NickName = gameSettings.NickName;
        PhotonNetwork.GameVersion = "0.0.0";
        PhotonNetwork.ConnectUsingSettings();
    }

	public override void OnConnectedToMaster()
	{
		PhotonNetwork.AutomaticallySyncScene = true;
		Debug.Log("OnConnected to master");
	}
	public override void OnDisconnected(DisconnectCause cause)
	{
		Debug.Log("Disconnected: " + cause.ToString());
	}
	public void FindGame()
	{	
		if(!PhotonNetwork.IsConnectedAndReady || PhotonNetwork.CurrentRoom !=null)
			return;		
		PhotonNetwork.JoinRandomRoom();
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
	void StartGame()
	{
		PhotonNetwork.CurrentRoom.IsOpen = false;
		PhotonNetwork.CurrentRoom.IsVisible = false;
		PhotonNetwork.LoadLevel(1);
	}
	public override void OnLeftRoom()
	{
		PhotonNetwork.LoadLevel(0);
	}
	public void ReturnToMainScene()
	{
		PhotonNetwork.Disconnect();
	}
}
