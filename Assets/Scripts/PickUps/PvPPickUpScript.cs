using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PvpPickUpScript : MonoBehaviour
{
    [SerializeField] Text _pvpPickUpText;
    [SerializeField] GameObject _pvpEnabledPopup;
    [SerializeField] KeyCode _pvpActivateKey;
    [SerializeField] PvpToggle _pvpToggle;

    int _pvpPickedUp;
    PhotonView _photonView;
    // Use this for initialization
    void Start () {
        _pvpPickedUp = 0;

        if (GameObject.Find("Connection Status").GetComponent<ConnectionStatus>().Connected) {
            _photonView = GetComponent<PhotonView>();
        }
    }

    public void FillTemporary(Text pvpText, GameObject pvpEnable, PvpToggle pPvpToggle)
    {
        _pvpEnabledPopup = pvpEnable;
        _pvpPickUpText = pvpText;
        _pvpToggle = pPvpToggle;
    }


    // Update is called once per frame
    private void Update () {
        if (Input.GetKeyDown(_pvpActivateKey) && !_pvpToggle.PvpEnabled && _pvpPickedUp > 0) {
            //Enable();
            if (PhotonNetwork.playerList.Length > 0) {
                _pvpToggle.CallEnable();
                _pvpPickedUp--;
                _pvpPickUpText.text = _pvpPickedUp.ToString();
            }
        }
	}

    private void OnTriggerEnter(Collider other) {
        if (other.transform.tag == "PvPPickUp") {
            _pvpPickedUp++;
            _pvpPickUpText.text = _pvpPickedUp.ToString();
            other.gameObject.GetComponent<PvpPickUpBehaviour>().GetDestroyed();
        }
    }
}
