using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class NetworkManager : MonoBehaviour {
    [SerializeField] int _numberOfPlayerPerRoom;
    [SerializeField] string _sceneToLoad;
    [SerializeField] Text _connectionText;
    [SerializeField] Transform[] _spawnPoints;
    //[SerializeField] Camera _deathCamera;
    
    //[SerializeField] GameObject _playerHolder;
    
    [SerializeField] GameObject _stunnedPanel;
    [SerializeField] GameObject _pvpPanel;
    //[SerializeField] Text _gemText;
    [SerializeField] Text _bombText;
    [SerializeField] Text _pvpText;
    [SerializeField] GameObject _tempHolder;
    [SerializeField] GameObject _playerHolder;
    [SerializeField] GameObject _rockHolder;
    [SerializeField] GameObject _gemHolder;
    [SerializeField] private PvpToggle _pvpToggle;
    [SerializeField] private RankingsHandler _rankingHandler;
    [SerializeField] private GameObject _pauseScreen;
    [SerializeField] private GameTime _gameTime;
    [SerializeField] private GameObject _placeableBombIndicator;
 
    GameObject _player;
    RankingsHandler _rankHandler;

    public GameObject CurrentPlayer {
        get { return _player; }
    }

    bool _connected;

    private void Awake()
    {
        _connected = GameObject.Find("Connection Status").GetComponent<ConnectionStatus>().Connected;
        _rankHandler = GameObject.FindGameObjectWithTag("Rankings").GetComponent<RankingsHandler>();
    }

    bool _inLobby;

    void Start()
    {
        if (_connected)
        {
            JoinRoom();
        }
        else
        {
            PhotonNetwork.logLevel = PhotonLogLevel.Full;
            //Write where to connect, for custom server
            PhotonNetwork.ConnectUsingSettings("4"); //the "1" is the version number to stop
                                                     //people with different versions playing
            PhotonNetwork.automaticallySyncScene = true;

        }
    }

    void Update()
    {
        if (!_connected && _inLobby) {
            Debug.Log((PhotonNetwork.countOfPlayers));

            if ((PhotonNetwork.countOfPlayers) >= _numberOfPlayerPerRoom) {
                GameObject.Find("Connection Status").GetComponent<ConnectionStatus>().Connected = true;
                SceneManager.LoadScene(_sceneToLoad);
            }
        }

        _connectionText.text = PhotonNetwork.connectionStateDetailed.ToString();
    }

    public void OnJoinedLobby() {
        //Debug.Log("connected to lobby");
        _inLobby = true;

        //JoinRoom();
    }

    private void JoinRoom()
    {
        //Debug.Log("Joining Room");

        RoomOptions ro = new RoomOptions() { IsVisible = true, MaxPlayers = 5 };
        PhotonNetwork.JoinOrCreateRoom("Andrei", ro, TypedLobby.Default);

        _inLobby = false;
    }

    void OnJoinedRoom() {
        //Debug.Log("started spawning");
        StartSpawnProcess(0f);

    }

    void OnPhotonPlayerConnected(PhotonPlayer newPlayer) {
        //Debug.Log("Player connected with id: " + newPlayer.UserId);
        //GetComponent<PhotonView>().RPC("CallRankingOnConnect", PhotonTargets.All, newPlayer);
    }

    void OnPhotonPlayerDisconnected(PhotonPlayer otherPlayer) {
        GetComponent<PhotonView>().RPC("CallRankingsOnDisconnect", PhotonTargets.All, otherPlayer);
    }

    [PunRPC]
    private void CallRankingOnConnect(PhotonPlayer pNewPlayer) {
        //Debug.Log(pNewPlayer.NickName + " has connected!");
        _rankHandler.UpdateRankingsOnOtherConnected(pNewPlayer);
    }

    [PunRPC]
    private void CallRankingsOnDisconnect(PhotonPlayer pOtherPlayer) {
        //Debug.Log(pOtherPlayer.NickName + " has been disconnected!");
        _rankHandler.UpdateRankingsOnDisconnect(pOtherPlayer);
    }

   void StartSpawnProcess(float respawnTime)
   {
       //_deathCamera.enabled = true;
       StartCoroutine("SpawnPlayer", respawnTime);
   }

    public void GetAllInfo(out Text bombText, out Text pvpText, out GameObject stunnedPanel,
        out GameObject pvpPanel, out GameObject tempHolder, out GameObject playerHolder, out GameObject rockHolder,
        out GameObject gemHolder, out PvpToggle pvpToggle, out GameObject pPauseScreen, out GameObject pPlaceableBombIndicator)
    {
        //gemText = _gemText;
        gemHolder = _gemHolder;
        bombText = _bombText;
        pvpText = _pvpText;
        pvpPanel = _pvpPanel;
        stunnedPanel = _stunnedPanel;
        tempHolder = _tempHolder;
        playerHolder = _playerHolder;
        rockHolder = _rockHolder;
        pvpToggle = _pvpToggle;
        pPauseScreen = _pauseScreen;
        pPlaceableBombIndicator = _placeableBombIndicator;
    }

    public void GetGemHolder(out GameObject gemHolder, out PvpToggle pvpToggle)
    {
        gemHolder = _gemHolder;
        pvpToggle = _pvpToggle;
    }

    public void GetBombHolders(out GameObject rockHolder, out GameObject playerHolder, out GameObject tempHolder)
    {
        rockHolder = _rockHolder;
        playerHolder = _playerHolder;
        tempHolder = _tempHolder;
    }

    IEnumerator SpawnPlayer(float respawnTime) {
        yield return new WaitForSeconds(respawnTime);

        int index = Random.Range(0, _spawnPoints.Length);

        _player = PhotonNetwork.Instantiate(LifeTimeManager.Instance.SkinName,
                                           _spawnPoints[index].position,
                                           _spawnPoints[index].rotation,
                                           0);
										   
        _player.GetComponent<PlayerNetworkMover>().FillValues(_bombText,
            _pvpText, _stunnedPanel, _pvpPanel, _tempHolder, _playerHolder, _rockHolder, _gemHolder, _pvpToggle, _rankingHandler, _pauseScreen, _placeableBombIndicator);
        //_player.GetComponent<PlayerNetworkMover>().LateStart();

        _player.GetComponent<PlayerNetworkMover>().RespawnMe += StartSpawnProcess;
        _player.transform.SetParent(_playerHolder.transform);
        _player.GetComponent<BombThrowScript>().Initialized = true;

        if (GameObject.Find("Connection Status").GetComponent<ConnectionStatus>().Connected)
        {
            GetComponent<PhotonView>().RPC("CallCreateRankings", PhotonTargets.All);
        }

        if (PhotonNetwork.player.IsMasterClient) {
            _gameTime.StartCoroutine(_gameTime.HandleStartTimer());
        }
    }

    [PunRPC]
    private void CallCreateRankings() {
        _rankHandler.CreateRankings();
    }
}