using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

public class BombBehabiourScript : MonoBehaviour {

    [SerializeField] int _maxBounces;
    [SerializeField] float _explosionRadius;
    [SerializeField] float _explosionForce;
    [SerializeField] float _playerExplosionForce=1;
    [Range(0, 100)] [SerializeField] int _gemLossPercentage;
    [SerializeField] float _addedForce= 1f;
    [SerializeField] private ParticleSystem _psystem;
    private bool _playedParticle;
    GameObject _playerHolder;
    GameObject _rockHolder;
    GameObject _tempHolder;
    GameObject _camera;
    Animator _cameraAnimator;

    [FMODUnity.EventRef]
    public string _bombExplode;

    int _currentBounces;
    List<GameObject> _players = new List<GameObject>();
    List<GameObject> _rocks = new List<GameObject>();

    PhotonView _photonView;

    PhotonView _view;

    private void Awake()
    {
        //if (!GameObject.Find("Connection Status").GetComponent<ConnectionStatus>().Connected)
        //{
        //    Destroy(GetComponent<PhotonView>());
        //}
        if (!GameObject.Find("Connection Status").GetComponent<ConnectionStatus>().Connected)
        {
            _playerHolder = GameObject.Find("Player Holder");
            _rockHolder = GameObject.Find("Rock Holder");
            _tempHolder = GameObject.Find("Temp Holder");
        }
    }

    // Use this for initialization
    void Start () {

        _camera = GameObject.Find("CM PlayerFollow");
       

        _photonView = GetComponent<PhotonView>();
        _currentBounces = 0;
        if(GameObject.Find("Connection Status").GetComponent<ConnectionStatus>().Connected)
            GameObject.Find("NetworkManager").GetComponent<NetworkManager>().GetBombHolders(out _rockHolder,out _playerHolder, out _tempHolder);
        transform.SetParent(_tempHolder.transform);

        foreach (Transform t in _playerHolder.transform)
        {
            _players.Add(t.gameObject);
        }
        foreach (Transform t in _rockHolder.transform)
        {
            _rocks.Add(t.gameObject);
        }
    }

    public float ExplosionForce
    {
        get
        {
            return _explosionForce;
        }
    }

    public float ExplosionRadius
    {
        get
        {
            return _explosionRadius;
        }
    }


	// Update is called once per frame
	void Update () {

	}

    private void OnCollisionEnter(Collision collision)
    {
        //Debug.Log("Bounce BOunce BOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOM");

        if (collision.transform.tag == "Ground")
        {
            if (_currentBounces >= _maxBounces)
            {
                NetworkExplode();
                //Debug.Log("[BombBehaviourScript] Called NetworkExplode() in Ground");
            }
            else
            {
                _currentBounces++;
            }
        }
        else if(collision.transform.tag =="Player" || collision.transform.tag == "Rock")
        {
            _currentBounces++;
            NetworkExplode();
            //Debug.Log("[BombBehaviourScript] Called NetworkExplode() in Player OR Rock");
        }  else {
            _currentBounces++;
        }
    }

    private void NetworkExplode()
    {
        
        if (GameObject.Find("Connection Status").GetComponent<ConnectionStatus>().Connected)
        {
            GetComponent<PhotonView>().RPC("Explode", PhotonTargets.All, transform.position);
        }
        else
        {
            Explode(transform.position);
        }
    }

    [PunRPC]
    private void Explode(Vector3 pos)
    {
        //Debug.Log("explode");

        if (_camera != null)
        {
            _cameraAnimator = _camera.GetComponent<Animator>();
            _cameraAnimator.SetTrigger("CamShake");
        }

        //bomb explosion sound
        FMOD.Studio.EventInstance e = FMODUnity.RuntimeManager.CreateInstance(_bombExplode);
        e.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(transform.position));
        e.start();
        e.release();
        //Debug.Log("Played sound of BombExplode");

        if (!_playedParticle)
        {
            _psystem.transform.SetParent(null);
            _psystem.transform.position = transform.position;
            _psystem.Play();
        }
        _playedParticle = true;

        foreach (GameObject obj in _rocks)
        {
            if (Utility.WithinDistance(obj, gameObject, _explosionRadius))
            {
                obj.GetComponent<RockScript>().GetDestroyed(transform.position,ExplosionForce);
            }
        }

        //Debug.Log(_players.Count);
        foreach(GameObject obj in _players)
        {
            //Debug.Log(obj.transform.name);
            float dist = Utility.GetDistance(obj.transform.position, pos);
            if(dist < _explosionRadius)
            {
                //Debug.Log("In Range");

				if (obj.GetComponent<Rigidbody>() != null) {
                    Vector3 explosion = (obj.transform.position - pos).normalized;
                    explosion.y = _addedForce;
                    explosion *= _explosionForce * _playerExplosionForce;
                    //obj.GetComponent<PlayerNetworkMover>().LaunchInDirWithForce((obj.transform.position - transform.position).normalized+ _addedForce, _explosionForce);
                    obj.GetComponent<Rigidbody>().AddForce(explosion, ForceMode.Impulse);

                    // Lose Gems
                    GemCollectionScript gemColl = obj.GetComponent<GemCollectionScript>();
                    gemColl.LoseGems(_gemLossPercentage);

                    StunnedScript stun = obj.GetComponent<StunnedScript>();
					if (stun != null) {
						stun.NetworkStun(true);
					}
				}
            }
        }

        //if(_photonView.isMine)
        //PhotonNetwork.Destroy(gameObject);

        if (GameObject.Find("Connection Status").GetComponent<ConnectionStatus>().Connected)
        {
            GetComponent<PhotonView>().RPC("networkDestroyBomb", PhotonTargets.All);
        }
        else
        {
            networkDestroyBomb();
        }
    }

    [PunRPC]
    void networkDestroyBomb()
    {
        //Destroy(gameObject);
        GetComponent<MeshRenderer>().enabled = false;
        Destroy(gameObject);
        //StartCoroutine("DestroyBomb");
    }

    //[PunRPC]
    //IEnumerator DestroyBomb()
    //{
    //    yield return new WaitForSeconds(0.69f);
    //    Destroy(gameObject);

    //}

    // calling the RPC
}
