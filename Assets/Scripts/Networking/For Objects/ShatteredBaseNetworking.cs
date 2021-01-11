using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShatteredBaseNetworking : Photon.MonoBehaviour
{
    [SerializeField] float smoothing = 10;
    [SerializeField] float _pieceStayTime = 5;
    List<GameObject> _pieces;
    private ConnectionStatus _connectionStatus;


    // Use this for initialization
    Quaternion[] rotations;
    Vector3[] positions;

    void Awake()
    {
        transform.SetParent(GameObject.Find("Temp Holder").transform);
        _pieces = new List<GameObject>();
        rotations = new Quaternion[transform.childCount];
        positions = new Vector3[transform.childCount];
        int i = 0;
        foreach (Transform t in transform)
        {
            _pieces.Add(t.gameObject);
            rotations[i] = t.rotation;
            positions[i] = t.position;
            i++;
        }

        _connectionStatus = GameObject.Find("Connection Status").GetComponent<ConnectionStatus>();

        if (_connectionStatus.Connected && !photonView.isMine)
        {
            StartCoroutine("UpdateData");
        }
    }

    public void DestroyShatteredPiece() {
        GetComponent<PhotonView>().RPC("destroyShatteredRock", PhotonTargets.All);
    }

    private void Update()
    {
        if (_pieces.Count > 0) {
            for (int i = _pieces.Count - 1; i > -1; i--) {
                if (_pieces[i] == null) {
                    _pieces.RemoveAt(i);
                }
            }
        }
    }

    IEnumerator UpdateData()
    {
        while (true)
        {
            for (int i = _pieces.Count-1; i >-1; i--)
            {
                if (_pieces[i] == null)
                    _pieces.RemoveAt(i);
                StartCoroutine("LerpPos", i);
                StartCoroutine("LerpRot", i);
            }
            yield return null;
        }
    }
    IEnumerator LerpPos(int i)
    {
        _pieces[i].transform.position = Vector3.Lerp(_pieces[i].transform.position, positions[i], Time.deltaTime * smoothing);
        yield return null;
    }
    IEnumerator LerpRot(int i)
    {
        _pieces[i].transform.rotation = Quaternion.Lerp(_pieces[i].transform.rotation, rotations[i], Time.deltaTime * smoothing);
        yield return null;
    }

    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        //Debug.Log("Inside OnPhotonSerializeView");

        if (stream.isWriting)
        {
            //stream.SendNext(_pieces.Count); not needed because the list does not change
            for (int i = 0; i < _pieces.Count; i++)
            {
                stream.SendNext(positions[i]);
                stream.SendNext(rotations[i]);
            }
        }
        else
        {
            for (int i = 0; i < _pieces.Count; i++)
            {
                positions[i] = (Vector3)stream.ReceiveNext();
                rotations[i] = (Quaternion)stream.ReceiveNext();
            }
        }
    }

    //[PunRPC]
    //private void DestroyShattered() {
    //    GameObject.Destroy(gameObject);
    //}


    [PunRPC]
    public void destroyShatteredRock() {
        StartCoroutine(destroyShatteredRockPieces());
    }

    private IEnumerator destroyShatteredRockPieces() {
        yield return new WaitForSeconds(_pieceStayTime);
        Destroy(gameObject);
    }
}
