using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockHandlerScript : Photon.MonoBehaviour {

    [SerializeField] string _prefabNormal;
    [SerializeField] GameObject _prefabNormalBrush;
    [SerializeField] string _prefabSpecial;
    [SerializeField] GameObject _prefabSpecialBrush;
    [SerializeField] float _minWait;
    [SerializeField] float _maxWait;
    // Use this for initialization
    void Start () {

    }

    public void ResetNormal(Vector3 pos, Quaternion rot)
    {
        float time = Random.Range(_minWait, _maxWait);
        Debug.Log("RESETING THE normal ROCKKKKKK " + time);
        StartCoroutine(networkResetNormalRockAfterBoom(time, pos, rot));
    }

    public void ResetSpecial(Vector3 pos, Quaternion rot)
    {
        float time = Random.Range(_minWait, _maxWait);
        Debug.Log("RESETING THE special ROCKKKKKK " + time);
        StartCoroutine(networkResetSpecialRockAfterBoom(time, pos, rot));
    }
   
    IEnumerator networkResetNormalRockAfterBoom(float time,Vector3 pos, Quaternion rot)
    {
        yield return new WaitForSeconds(time);
        Debug.Log("nomral ROCKKKKKK HAS VEEEN RESEST");
        if (!GameObject.Find("Connection Status").GetComponent<ConnectionStatus>().Connected)
        {
            Instantiate(_prefabNormalBrush, pos, rot);
        }
        else if (photonView.isMine)
        {
            PhotonNetwork.Instantiate(_prefabNormal, pos, rot, 0);
        }

    }

    IEnumerator networkResetSpecialRockAfterBoom(float time, Vector3 pos, Quaternion rot)
    {
        yield return new WaitForSeconds(time);
        Debug.Log("special ROCKKKKKK HAS VEEEN RESEST");
        if (!GameObject.Find("Connection Status").GetComponent<ConnectionStatus>().Connected)
        {
            Instantiate(_prefabSpecialBrush, pos, rot);
        }
        else if (photonView.isMine)
        {
            PhotonNetwork.Instantiate(_prefabSpecial, pos, rot, 0);
        }
    }
}
