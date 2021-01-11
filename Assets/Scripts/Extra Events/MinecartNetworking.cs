using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinecartNetworking : MonoBehaviour {

    [SerializeField] float _explosionForce;
    [SerializeField] float _playerExplosionForce = 1;
    [Range(0, 100)] [SerializeField] int _gemLossPercentage;
    [SerializeField] float _addedForce = 1f;
    [SerializeField] float _velocityMultiplier = 0f;

    Vector3 _velocity;
    Vector3 _prevPosition;

    [FMODUnity.EventRef]
    public string _cart;


    Vector3 position;
    Vector3 velocity;
    Quaternion rotation = new Quaternion();
    float smoothing = 10f;
    // Use this for initialization
    void Start () {

        //cart sound
        FMOD.Studio.EventInstance e1 = FMODUnity.RuntimeManager.CreateInstance(_cart);
        e1.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(transform.position));
        e1.start();
        e1.release();
        //Debug.Log("Played sound of Hit");

        position = transform.position;
        rotation = transform.rotation;
        _prevPosition = transform.position;
		if(!PhotonNetwork.isMasterClient)
        {
            StartCoroutine("UpdateData");
        }
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        _velocity = transform.position - _prevPosition;
        _prevPosition = transform.position;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.transform.tag=="Player")
        {
            GetComponent<Collider>().enabled = false;
            Invoke("ReanableCollision",1);
            GameObject obj = collision.gameObject;
            if (obj.GetComponent<Rigidbody>() != null)
            {
                //Debug.Log("BOOOOOM SHAKA LAKA");
                Vector3 explosion = (obj.transform.position - transform.position).normalized;
                explosion.y = _addedForce;
                explosion *= _explosionForce * _playerExplosionForce;
                //obj.GetComponent<PlayerNetworkMover>().LaunchInDirWithForce((obj.transform.position - transform.position).normalized+ _addedForce, _explosionForce);
                obj.GetComponent<Rigidbody>().AddForce(explosion, ForceMode.Impulse);

                // Lose Gems
                GemCollectionScript gemColl = obj.GetComponent<GemCollectionScript>();
                gemColl.LoseGems(_gemLossPercentage);

                StunnedScript stun = obj.GetComponent<StunnedScript>();
                if (stun != null)
                {
                    stun.NetworkStun(true);
                }
            }
        }
    }

    void ReanableCollision()
    {
        GetComponent<Collider>().enabled = true;
    }

    IEnumerator UpdateData()
    {
        while (true)
        {
            transform.position = Vector3.Lerp(transform.position, position, Time.deltaTime * smoothing);
            transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * smoothing);
            _velocity = velocity;
            yield return null;
        }
    }

    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        //Debug.Log("Inside OnPhotonSerializeView");
        if (stream.isWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(_velocity);
            stream.SendNext(transform.rotation);
        }
        else
        {
            position = (Vector3)stream.ReceiveNext();
            velocity = (Vector3)stream.ReceiveNext();
            rotation = (Quaternion)stream.ReceiveNext();
        }
    }
}
