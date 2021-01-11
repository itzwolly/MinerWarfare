using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerNetworkMover : Photon.MonoBehaviour {

    [SerializeField] GameObject _stunnedPanel;
    [SerializeField] GameObject _pvpPanel;
    //[SerializeField] Text _gemText;
    [SerializeField] Text _bombText;
    [SerializeField] Text _pvpText;
    [SerializeField] GameObject _tempHolder;
    [SerializeField] GameObject _playerHolder;
    [SerializeField] GameObject _rockHolder;
    [SerializeField] GameObject _gemHolder;
    [SerializeField] PvpToggle _pvpToggle;
    [SerializeField] GameObject _pauseScreen;
    [SerializeField] GameObject _placeableBombIndicator;

    private RankingsHandler _rankingHandler;

    public delegate void Respawn(float time);
    public event Respawn RespawnMe;

    Vector3 position;
    Quaternion rotation = new Quaternion();
    float smoothing = 10f;

    public void FillValues(/*Text gemText, */Text bombText, Text pvpText, GameObject stunnedPanel,
        GameObject pvpPanel, GameObject tempHolder, GameObject playerHolder, GameObject rockHolder,
        GameObject gemHolder, PvpToggle pPvpToggle, RankingsHandler rankingsHandler, GameObject pPauseScreen, GameObject pPlaceableBombIndicator)
    {
        //_gemText = gemText;
        _gemHolder = gemHolder;
        _bombText = bombText;
        _pvpText = pvpText;
        _pvpPanel = pvpPanel;
        _stunnedPanel = stunnedPanel;
        _tempHolder = tempHolder;
        _playerHolder = playerHolder;
        _rockHolder = rockHolder;
        _pvpToggle = pPvpToggle;
        _rankingHandler = rankingsHandler;
        _pauseScreen = pPauseScreen;
        _placeableBombIndicator = pPlaceableBombIndicator;
    }

    public bool IsMine()
    {
        return photonView.isMine;
    }

    public void LaunchInDirWithForce(Vector3 dir, float force)
    {
        Debug.Log("PRRRRRRA EXPLODEEEEEEED "+force + " : " +dir);
        gameObject.GetComponent<Rigidbody>().AddForce(dir * force);
        //GetComponent<PhotonView>().RPC("NetworkLaunchInDirWithForce", PhotonTargets.All, dir, force);
    }

    [PunRPC]
    public void NetworkLaunchInDirWithForce(Vector3 dir, float force)
    {
        Debug.Log("PRRRRRRA EXPLODEEEEEEED");
        gameObject.GetComponent<Rigidbody>().AddForce(dir * force);
    }
    void Start()
    {
        //Debug.Log("start");
        if (GameObject.Find("Connection Status").GetComponent<ConnectionStatus>().Connected)
        {
            if (photonView.isMine)
            {
                foreach (Transform t in transform)
                {
                    if (t.tag == "Cameras")
                    {
                        //Debug.Log("activating camera");
                        t.gameObject.SetActive(true);
                    }
                }

                _pvpToggle.FillValues(_pvpPanel);
                GetComponent<BombThrowScript>().FillTemporary(_tempHolder, _rockHolder, _playerHolder);
                GetComponent<StunnedScript>().FillTemporary(_stunnedPanel);
                GetComponent<GemCollectionScript>().FillTemporary(_gemHolder, _rankingHandler, _pvpToggle);
                GetComponent<BombPickUpScript>().FillTemporary(_bombText);
                GetComponent<PvpPickUpScript>().FillTemporary(_pvpText, _pvpPanel, _pvpToggle);
                GetComponent<PauseToggle>().FillValues(_pauseScreen);
                GetComponent<PlaceBombScript>().Initialize(_placeableBombIndicator);

                GetComponent<Rigidbody>().useGravity = true;
                gameObject.transform.SetParent(_playerHolder.transform);

                GetComponent<ParkourMovementScript>().enabled = true;
                GetComponent<PlaceBombScript>().enabled = true;
                GetComponent<LookAt>().enabled = true;
                GetComponent<PlayerMeleeScript>().enabled = true;
                GetComponent<BombThrowScript>().enabled = true;
                GetComponent<StunnedScript>().enabled = true;
                GetComponent<GemCollectionScript>().enabled = true;
                GetComponent<BombPickUpScript>().enabled = true;
                GetComponent<PvpPickUpScript>().enabled = true;
                GetComponent<PauseToggle>().enabled = true;

                //GetComponentInChildren<PlayerShooting>().enabled = true;
                //foreach (SimpleMouseRotator rot in GetComponentsInChildren<SimpleMouseRotator>())
                //    rot.enabled = true;
                //foreach (Camera cam in GetComponentsInChildren<Camera>())
                //    cam.enabled = true;

                //transform.Find("Head Joint/First Person Camera/GunCamera/Candy-Cane").gameObject.layer = 11;
            }
            else
            {
                Destroy(GetComponent<Rigidbody>());
                GameObject.Find("NetworkManager").GetComponent<NetworkManager>().GetAllInfo(out _bombText, out _pvpText, out _stunnedPanel,
                                                  out _pvpPanel, out _tempHolder, out _playerHolder, out _rockHolder,
                                                  out _gemHolder, out _pvpToggle, out _pauseScreen, out _placeableBombIndicator);
                gameObject.transform.SetParent(_playerHolder.transform);
                StartCoroutine(UpdateData());
            }
        }
    }

    public string GetNickName() {
        return PhotonNetwork.playerName;
    }

    IEnumerator UpdateData()
    {
        while (true)
        {
            transform.position = Vector3.Lerp(transform.position, position, Time.deltaTime * smoothing);
            transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * smoothing);
            yield return null;
        }
    }

    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        //Debug.Log("Inside OnPhotonSerializeView");
        if (stream.isWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }
        else
        {
            position = (Vector3) stream.ReceiveNext();
            rotation = (Quaternion) stream.ReceiveNext();
        }
    }
}