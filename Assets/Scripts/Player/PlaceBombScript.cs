using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(BombThrowScript))]
public class PlaceBombScript : MonoBehaviour {
    [SerializeField] GameObject _bombPlaceblePrefab;
    [SerializeField] KeyCode _bombPlaceKey;
    [SerializeField] int _ticsToShoot;
    [SerializeField] int _ticsToPlace;
    [SerializeField] Vector3 _movePos;
    [SerializeField] private GameObject _bombIndicator;

    BombThrowScript _bombThrowScript;
    int _tics;
    int _bombPlaceTicks;

    
    private Image _bombIndicatorImage = null;
    private bool _dropBomb;

    // Use this for initialization
    void Start() {
        _tics = 0;
        _bombThrowScript = GetComponent<BombThrowScript>();
        if (_bombIndicatorImage == null) {
            _bombIndicatorImage = _bombIndicator.GetComponent<Image>();
        }
    }

    public void Initialize(GameObject pPlacebleBombIndicator) {
        _bombIndicator = pPlacebleBombIndicator;
        _bombIndicatorImage = _bombIndicator.GetComponent<Image>();
    }

        // Update is called once per frame
    void FixedUpdate ()
    {
        if (Input.GetKeyDown(_bombPlaceKey))
        {
            _tics = 0;
            _bombPlaceTicks = 0;
        }

        if (Input.GetKey(_bombPlaceKey) && _bombThrowScript.Bombs > 0) {
            if (_tics >= _ticsToShoot) {
                _bombThrowScript.NotShoot();

                _dropBomb = true;
                if (!_bombIndicator.activeSelf) {
                    _bombIndicator.SetActive(true);
                }
            }

            if (_dropBomb) {
                _bombPlaceTicks++;

                if (_bombPlaceTicks >= _ticsToPlace) {
                    GameObject bomb = null;

                    if (GameObject.Find("Connection Status").GetComponent<ConnectionStatus>().Connected) {
                        bomb = PhotonNetwork.Instantiate("PlacebleBomb", transform.position - transform.forward, transform.rotation, 0);
                    } else {
                        bomb = Instantiate(_bombPlaceblePrefab, transform.parent.position + _movePos + transform.position - transform.forward, transform.rotation);
                    }

                    _bombThrowScript.RemoveBomb();
                    gameObject.GetComponent<BombPickUpScript>().UpdateText();
                    _dropBomb = false;
                    _tics = 0;
                    _bombPlaceTicks = 0;
                }

                _bombIndicatorImage.fillAmount = ((float)_bombPlaceTicks / _ticsToPlace);
            }

            _tics++;

        }

        if (Input.GetKeyUp(_bombPlaceKey))
        {
            if (_bombIndicator.activeSelf) {
                _bombIndicator.SetActive(false);
            }
            _dropBomb = false;
            _tics = 0;
            _bombPlaceTicks = 0;
        }
    }
}
