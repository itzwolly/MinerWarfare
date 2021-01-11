using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemBehaviourScript : MonoBehaviour {
    // Use this for initialization
    [SerializeField] float _resetPower;
    [SerializeField] bool _randomBounce;

    private void Start()
    {

        transform.SetParent(GameObject.Find("Gems Holder").transform);
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.transform.tag == "Gem")
        {
            if (collision.transform.position.y > transform.position.y)
            {
                Vector3 dir;
                if (_randomBounce)
                {
                    dir = new Vector3(Random.value * 2 - 1, _resetPower * 2, Random.value * 2 - 1);
                }
                else
                {
                    dir = collision.transform.position - transform.position;
                    dir.Normalize();
                    dir *= _resetPower*2;
                    dir.y = 1;
                }
                collision.transform.gameObject.GetComponent<Rigidbody>().AddForce(dir * _resetPower, ForceMode.Impulse);
            }
        }
    }

    public void GetDestroyed()
    {
        if (GameObject.Find("Connection Status").GetComponent<ConnectionStatus>().Connected)
        {
            GetComponent<PhotonView>().RPC("networkDestroyGem", PhotonTargets.All);
        }
        else
        {
            networkDestroyGem();
        }
    }
    [PunRPC]
    private void networkDestroyGem()
    {
        Destroy(gameObject);
    }
}
