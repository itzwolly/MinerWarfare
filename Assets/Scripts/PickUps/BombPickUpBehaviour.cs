using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombPickUpBehaviour : MonoBehaviour {

    // Use this for initialization
    private void Start()
    {
        transform.SetParent(GameObject.Find("Temp Holder").transform);
    }


    public void GetDestroyed()
    {
        if (GameObject.Find("Connection Status").GetComponent<ConnectionStatus>().Connected)
        {
            Destroy(gameObject);
            GetComponent<PhotonView>().RPC("networkDestroyBombPickup", PhotonTargets.Others);
        }
        else
        {
            networkDestroyBombPickup();
        }
    }
    [PunRPC]
    private void networkDestroyBombPickup()
    {
        Destroy(gameObject);
    }
}
