using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PvpToggle : Photon.MonoBehaviour {
    [SerializeField] private int _pvpTime;
    [SerializeField] private float _pvpEnabledPopupTimer;
    [SerializeField] private Image _pvpImage;
    [SerializeField] private Sprite[] _images; // 0 is disabled, 1 is enabled

    private bool _enabled;
    private int _pvpTimer;
    private float _pvpTimeOffset;
    private GameObject _pvpEnabledPopup;
    private PhotonView _photonView;

    public int PvpTimer {
        get { return _pvpTimer; }
        set { _pvpTimer = value; }
    }
    public float pvpTimeOffset {
        get { return _pvpTimeOffset; }
    }
    public bool PvpEnabled {
        get { return _enabled; }
        set { _enabled = value; }
    }

    private void Start() {
        _enabled = false;
        _pvpTimer = _pvpTime;
        _photonView = GetComponent<PhotonView>();
    }

    public void FillValues(GameObject pPvpEnabledPopup) {
        _pvpEnabledPopup = pPvpEnabledPopup;
    }

    [PunRPC]
    private void Enable() {
        if (GameObject.Find("Connection Status").GetComponent<ConnectionStatus>().Connected) {
            _enabled = true;
            _pvpTimeOffset = Time.time;
            _pvpEnabledPopup.SetActive(true);
            _photonView.RPC("callSoundChangePart", PhotonTargets.All, 2.0f);
            StartCoroutine(ChangeSprite(false));
            StartCoroutine(Stop(false, _pvpTime));
        }
    }
    
    public void CallEnable() {
        _photonView.RPC("Enable", PhotonTargets.All);
    }

    [PunRPC]
    private void callSoundChangePart(float pPart) {
        SoundManager.Instance.ChangePart(pPart);
    }

    public void UpdateHUD(Text pText, int pAmount) {
        pText.text = pAmount.ToString();
    }

    private IEnumerator ChangeSprite(bool pEnable) {
        yield return new WaitForSeconds(_pvpEnabledPopupTimer);
        _pvpEnabledPopup.SetActive(pEnable);
        _pvpImage.sprite = _images[1];
    }

    private IEnumerator Stop(bool pEnable, float pDelay) {
        yield return new WaitForSeconds(pDelay);
        _enabled = false;
        _pvpImage.sprite = _images[0];
        _pvpEnabledPopup.SetActive(pEnable);
        _photonView.RPC("callSoundChangePart", PhotonTargets.All, 1.0f);
    }
}
