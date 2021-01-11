using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunnedScript : MonoBehaviour {

    [SerializeField] List<MonoBehaviour> _componentsToDeactivate;
    [SerializeField] GameObject _stunnedPanel;
    [SerializeField] float _timeStunned;

    [FMODUnity.EventRef]
    public string _stun;

    // Update is called once per frame
    void Update () {
		
	}

    public void FillTemporary(GameObject stunnedText)
    {
        _stunnedPanel = stunnedText;
    }

    public void NetworkStun(bool pShowStunPanel)
    {
        if (GameObject.Find("Connection Status").GetComponent<ConnectionStatus>().Connected)
        {
            GetComponent<PhotonView>().RPC("Stun", PhotonTargets.All, pShowStunPanel);
        }
        else
        {
            Stun(true);
        }
    }

    [PunRPC]
    public void Stun(bool pShowStunPanel)
    {
        //stun sound
        FMOD.Studio.EventInstance e1 = FMODUnity.RuntimeManager.CreateInstance(_stun);
        e1.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(transform.position));
        e1.start();
        e1.release();
        //Debug.Log("Played sound of Hit");

        foreach (MonoBehaviour comp in _componentsToDeactivate)
        {
            comp.enabled = false;
        }

        if (pShowStunPanel) {
            if (_stunnedPanel != null) {
                _stunnedPanel.SetActive(true);
            }
        }

        if (GetComponent<PhotonView>().isMine || !GameObject.Find("Connection Status").GetComponent<ConnectionStatus>().Connected) {
			if (pShowStunPanel) {
				Invoke("Unstun", _timeStunned);
			}
        }
    }

    private void Unstun()
    {
        if (_stunnedPanel != null)
            _stunnedPanel.SetActive(false);
        foreach (MonoBehaviour comp in _componentsToDeactivate)
        {
            comp.enabled = true;
        }
    }
}
