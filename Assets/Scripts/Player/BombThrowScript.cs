using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LookAt))]
public class BombThrowScript : MonoBehaviour {
    [SerializeField] int _startingBombs;
    [SerializeField] float _throwPower;
    [SerializeField] Vector3 _throwDirection;
    [SerializeField] float _coolDownTime;
    [SerializeField] GameObject _bombPrefab;
    [SerializeField] GameObject _tempHolder;
    [SerializeField] GameObject _rockHolder;
    [SerializeField] GameObject _playerHolder;
    [SerializeField] KeyCode _bombKey;
    [SerializeField] private GameObject _head;
    [SerializeField] private float _angleToMove;

    [FMODUnity.EventRef]
    public string _bombThrow;

    int _currentBombs;
    private LookAt _lookAt;
    private bool _onCooldown = false;
    private bool _turn = false;

    private Quaternion _endRotation;
    private float _angle;
    private bool _initialized = false;

    public bool Initialized {
        get { return _initialized; }
        set { _initialized = value; }
    }

    private bool _notShoot;

    public void NotShoot()
    {
        _notShoot = true;
    }

    public int Bombs
    {
        get
        {
            return _currentBombs;
        }
    }

    public void FillTemporary(GameObject tempHolder, GameObject rockHolder, GameObject playerHolder)
    {
        _tempHolder = tempHolder;
        _rockHolder = rockHolder;
        _playerHolder = playerHolder;
    }

    public void AddBomb()
    {
        _currentBombs++;
    }
    public void RemoveBomb()
    {
        _currentBombs--;
    }

    // Use this for initialization

    void Start () {
        _lookAt = GetComponent<LookAt>();
        _currentBombs = _startingBombs;
        if(!GameObject.Find("Connection Status").GetComponent<ConnectionStatus>().Connected)
        {
            Initialized = true;
        }
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        if (_turn) {
            if(_currentBombs > 0) {
                //bomb throw sound
                FMOD.Studio.EventInstance e = FMODUnity.RuntimeManager.CreateInstance(_bombThrow);
                e.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(transform.position));
                e.start();
                e.release();
                //Debug.Log("Played sound of BombThrow");

                //_lookAt.AlignWithCameraDirection(transform);
                
                //bomb.transform.position = _head.transform.position + transform.forward * 0.4f;

                _endRotation = _lookAt.AlignWithCameraDirection(transform);
                _angle = Quaternion.Angle(transform.rotation, _endRotation);

                if (_angle < _angleToMove)
                {
                    GameObject bomb;
                    if (GameObject.Find("Connection Status").GetComponent<ConnectionStatus>().Connected)
                    {
                        bomb = PhotonNetwork.Instantiate("Bomb", _head.transform.position + transform.forward * 0.7f, transform.rotation, 0);
                    }
                    else
                    {
                        bomb = Instantiate(_bombPrefab, transform.parent.position + _head.transform.position + transform.forward * 0.7f, transform.rotation);
                    }
                    //bomb.transform.position = _head.transform.position + transform.forward * 0.4f;

                    // TODO: Check out fam
                    //bomb.transform.SetParent(_tempHolder.transform);
                    //bomb.GetComponent<BombBehabiourScript>().GiveLists(_playerHolder, _rockHolder);

                    if (_lookAt.HasHit) {
                        float distance = (transform.position - _lookAt.Hit.point).magnitude;
                        if (distance <= _throwPower) {
                            bomb.GetComponent<Rigidbody>().AddForce((_throwDirection + _head.transform.forward * distance), ForceMode.Impulse);
                        } else {
                            bomb.GetComponent<Rigidbody>().AddForce((_throwDirection + _head.transform.forward * _throwPower), ForceMode.Impulse);
                        }
                    } else {
                        bomb.GetComponent<Rigidbody>().AddForce((_throwDirection + _head.transform.forward * _throwPower), ForceMode.Impulse);
                    }
                    _turn = false;
                    _currentBombs--;
                    gameObject.GetComponent<BombPickUpScript>().UpdateText();
                    StartCoroutine(ActivateCoolDown());
                }
            }
        }

        if (_initialized)
        {
            if(Input.GetKeyDown(_bombKey))
            {
                _notShoot = false;
            }
            if (!_onCooldown && Input.GetKeyUp(_bombKey))
            {
                if (!_notShoot)
                {
                    _turn = true;
                }
            }
        }
    }

    private IEnumerator ActivateCoolDown() {
        _onCooldown = true;
        yield return new WaitForSeconds(_coolDownTime);
        _onCooldown = false;
    }
}
