using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PvpPickUpBehaviour : MonoBehaviour {

    PhotonView _photonView;

    private void Start()
    {
        Debug.Log("pvp spawned");
        _photonView = GetComponent<PhotonView>();
        transform.SetParent(GameObject.Find("Temp Holder").transform);
    }
    // Use this for initialization
    public void GetDestroyed()
    {
        if (GameObject.Find("Connection Status").GetComponent<ConnectionStatus>().Connected)
        {
            Destroy(gameObject);
            _photonView.RPC("networkDestroyPvpPickup", PhotonTargets.Others);
        }
        else
        {
            networkDestroyPvpPickup();
        }
    }

    [PunRPC]
    private void networkDestroyPvpPickup()
    {
        Destroy(gameObject);
    }
}
