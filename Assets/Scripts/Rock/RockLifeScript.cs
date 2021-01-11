using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockLifeScript : RockScript
{
    [SerializeField] int _hitPoints;
    [SerializeField] GameObject _gemsHolder;
    [SerializeField] float _launchPower;
    [SerializeField] int _minGems;
    [SerializeField] int _maxGems;
    [SerializeField] GameObject _tempHolder;

    [SerializeField] GameObject _bombDropBrush;
    [SerializeField] GameObject _pvpDropBrush;
    [SerializeField] GameObject _gemBrush;

    [Range(0,1)]
    [SerializeField] float _bombDropChance;
    [Range(0, 1)]
    [SerializeField] float _pvPDropChance;
    // Use this for initialization
    bool _dropBomb;
    bool _dropPvP;
    int nr;
    Vector3[] directions;
    Vector3 _bombDir;
    Vector3 _pvpDir;
    void Start ()
    {
        _gemsHolder = GameObject.Find("Gems Holder");
        _tempHolder = GameObject.Find("Temp Holder");
        transform.SetParent(GameObject.Find("Rock Holder").transform);
        nr = rand.Next(_minGems, _maxGems);
        directions = new Vector3[nr];
        for(int i=0;i<nr;i++)
        {
            directions[i] = new Vector3(Random.value * 2 - 1, Random.value, Random.value * 2 - 1);
        }
        if (Random.value <= _bombDropChance)
            _dropBomb = true;
        if (Random.value <= _pvPDropChance)
            _dropPvP = true;
        _bombDir = new Vector3(Random.value * 2 - 1, 1, Random.value * 2 - 1);
        _pvpDir = new Vector3(Random.value * 2 - 1, 1, Random.value * 2 - 1);
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public override void GetHit(GameObject hitter)
    {
        Debug.Log("hit rock");
        _hitPoints--;
        if(_hitPoints<1)
        {
            base.GetHit(hitter);
        }
    }

    public override void GetDestroyed(Vector3 pos, float explosionForce)
    {
        base.GetDestroyed(pos,explosionForce);

        Debug.Log("Nonrmal rock go BOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOM");
        //Transform rockHolder = transform.parent;
        RockHandlerScript rockHandlerScript = transform.parent.gameObject.GetComponent<RockHandlerScript>();
        rockHandlerScript.ResetNormal(transform.position,transform.rotation);

        for (int i = 0; i < nr; i++)
        {
            //Debug.Log(i);
            GameObject gem;
            if (GameObject.Find("Connection Status").GetComponent<ConnectionStatus>().Connected)
            {
                gem = PhotonNetwork.Instantiate("Gem", transform.position + directions[i], transform.rotation, 0);
            }
            else
            {
                gem = Instantiate(_gemBrush, transform.position + directions[i], transform.rotation);
            }
            gem.transform.SetParent(_gemsHolder.transform);

            gem.GetComponent<Rigidbody>().AddForce(directions[i] * _launchPower, ForceMode.Impulse);
        }

        if (_dropBomb)
        {
            GameObject bombDrop;
            if (GameObject.Find("Connection Status").GetComponent<ConnectionStatus>().Connected)
            {
                bombDrop = PhotonNetwork.Instantiate("BombPickUp", transform.position + _bombDir, transform.rotation, 0);
            }
            else
            {
                bombDrop = Instantiate(_bombDropBrush, transform.position + _bombDir, transform.rotation);
            }
            bombDrop.transform.SetParent(_tempHolder.transform);

            bombDrop.GetComponent<Rigidbody>().AddForce(_bombDir * _launchPower, ForceMode.Impulse);
        }

        if (_dropPvP)
        {
            GameObject pvpDrop;
            if (GameObject.Find("Connection Status").GetComponent<ConnectionStatus>().Connected)
            {
                pvpDrop = PhotonNetwork.Instantiate("PvPPickUp", transform.position + _pvpDir, transform.rotation, 0);
            }
            else
            {
                pvpDrop = Instantiate(_pvpDropBrush, transform.position + _pvpDir, transform.rotation);
            }
            pvpDrop.transform.SetParent(_tempHolder.transform);

            pvpDrop.GetComponent<Rigidbody>().AddForce(_pvpDir * _launchPower, ForceMode.Impulse);
        }
        if (GameObject.Find("Connection Status").GetComponent<ConnectionStatus>().Connected)
        {
            GetComponent<PhotonView>().RPC("networkDestroyNormalRock", PhotonTargets.All);
        }
        else
        {
            //networkDestroyNormalRock();
            Destroy(gameObject);

        }
    }

    [PunRPC]
    void networkDestroyNormalRock()
    {
        Destroy(gameObject);
    }
}

