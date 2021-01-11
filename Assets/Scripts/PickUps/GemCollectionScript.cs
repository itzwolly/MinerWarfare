using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GemCollectionScript : MonoBehaviour
{
    [SerializeField] float _maxDistForSuck;
    [SerializeField] private GameObject _gemsHolder;
    [SerializeField] private GameObject _gemBrush;
    [SerializeField] private float _launchPower;
    [SerializeField] PvpToggle _pvpToggle;
    [SerializeField] private RankingsHandler _rankingsHandler;

    [FMODUnity.EventRef]
    public string _pickup;

    private int _gemsCollected;
    private ConnectionStatus _connectionStatus;

	public int GemsCollected {
        get { return _gemsCollected; }
        set { _gemsCollected = value; }
    }

    private void Awake() {
        _connectionStatus = GameObject.Find("Connection Status").GetComponent<ConnectionStatus>();
        _rankingsHandler = GameObject.FindGameObjectWithTag("Rankings").GetComponent<RankingsHandler>();
    }

    // Use this for initialization
    void Start () {
        _gemsCollected = 0;

        if (_connectionStatus.Connected)
        {
            if (!GetComponent<PhotonView>().isMine)
            {
                GameObject.Find("NetworkManager").GetComponent<NetworkManager>().GetGemHolder(out _gemsHolder, out _pvpToggle);
                //_gemsHolder = GameObject.Find("Gems Holder");
            }
        }
    }
	
    public void FillTemporary(GameObject gemHolder, RankingsHandler rankingHandler, PvpToggle pPvpToggle)
    {
        _gemsHolder = gemHolder;
        _pvpToggle = pPvpToggle;
        _rankingsHandler = rankingHandler;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (/*GetComponent<PhotonView>().isMine &&*/ _connectionStatus.Connected)
        {
            if (other.transform.tag == "Gem")
            {
                //click sound
                FMOD.Studio.EventInstance e1 = FMODUnity.RuntimeManager.CreateInstance(_pickup);
                e1.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(transform.position));
                e1.start();
                e1.release();
                //Debug.Log("Played sound of Hit");
                _gemsCollected++;
                PhotonPlayer pPlayer = PhotonNetwork.player;
                if (GetComponent<PhotonView>().isMine) {
                    GetComponent<PhotonView>().RPC("CallSetScore", PhotonTargets.All, pPlayer, _gemsCollected);
                }
                other.gameObject.GetComponent<GemBehaviourScript>().GetDestroyed();
            }
        }
        else /*if (!_connectionStatus.Connected)*/
        {
            if (other.transform.tag == "Gem")
            {
                other.gameObject.GetComponent<GemBehaviourScript>().GetDestroyed();
            }
        }
    }

    private void Update()
    {
        foreach(Transform gem in _gemsHolder.transform)
        {
            //Debug.Log(gem.name);
            if(Utility.WithinDistanceSQ(gameObject,gem.gameObject,_maxDistForSuck))
            {
                gem.gameObject.GetComponent<GemNetworkBehaviour>().NotOnGround();
                if(gem.gameObject.GetComponent<Rigidbody>()==null)
                {
                    gem.gameObject.AddComponent<Rigidbody>();
                }
                //Debug.Log("pulling");
                gem.transform.GetComponent<Rigidbody>().AddForce(Utility.ConnectingVector(gem.transform.position,transform.position),ForceMode.Impulse);
            }
        }
    }

    [PunRPC]
    public void CallSetScore(PhotonPlayer pPlayer, int pGemsCollected) {
        _rankingsHandler.SetScore(pGemsCollected, pPlayer);
    }

    public void LoseGems(int pPercentage) {
        if (GetComponent<PhotonView>().isMine) {
            if (_pvpToggle.PvpEnabled && _gemsCollected > 0) {
                int gemLoss = (int) Mathf.Floor(_gemsCollected / 100f * pPercentage);
                DropGems(gemLoss, _launchPower);
                _gemsCollected -= gemLoss;
                Debug.Log("Inside LoseGems(): " + _gemsCollected + " gems");
                PhotonPlayer pPlayer = PhotonNetwork.player;
                GetComponent<PhotonView>().RPC("CallSetScore", PhotonTargets.All, pPlayer, _gemsCollected);
            }
        }
    }

    private void DropGems(int pAmount, float pLaunchPower) {
        for (int i = 0; i < pAmount; i++) {
            Vector3 dir = new Vector3(Random.value * 2 - 1, 1, Random.value * 2 - 1);
            GameObject gem = null;

            if (GameObject.Find("Connection Status").GetComponent<ConnectionStatus>().Connected)
            {
                gem = PhotonNetwork.Instantiate("Gem", transform.position + dir, transform.rotation, 0);
            }
            else
            {
                gem = Instantiate(_gemBrush, transform.position + dir, transform.rotation);
            }
            gem.transform.SetParent(_gemsHolder.transform);

            gem.GetComponent<Rigidbody>().AddForce(dir * pLaunchPower, ForceMode.Impulse);
        }
    }
}
