using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrokenPieceScript : MonoBehaviour {
    float _destroyTime;
    private ConnectionStatus _connectionStatus;
    private PhotonView _photonView;

    // Use this for initialization
    private void Awake() {
        _connectionStatus = GameObject.Find("Connection Status").GetComponent<ConnectionStatus>();
    }

    //[PunRPC]
    //public void networkDestroyBrokenPiece()
    //{
    //    Destroy(gameObject);
    //}
    
    public void SetTime(float time)
    {
        _destroyTime = time;
    }

    //private IEnumerator destroyBrokenPieces() {
    //    yield return new WaitForSeconds(_destroyTime);
    //    _photonView.RPC("networkDestroyBrokenPiece", PhotonTargets.All);
    //}

    public void Launch(Vector3 pos, float explosionForce, float multiplier)
    {
        //if (_photonView == null) {
        //    _photonView = pPhotonView;
        //}

        //if (_photonView.isMine && _connectionStatus.Connected) {
        //    StartCoroutine("destroyBrokenPieces");
        //}
        gameObject.GetComponent<Rigidbody>().AddForce(((transform.position - pos) + (transform.position - transform.parent.transform.position)) * /*(1-dist / bomb.ExplosionRadius) **/ explosionForce * multiplier);
    }
}
