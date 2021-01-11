using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMeleeScript : Photon.MonoBehaviour {
    [SerializeField] private float _attackDist;
    [SerializeField] private float _angleToMove;
    private LookAt _lookAt;

    [FMODUnity.EventRef]
    public string _hitRock;

    [FMODUnity.EventRef]
    public string _pickaxeSwing;

    GameObject _connectionStatus;

    private Animator myAnimator;
    private bool _turn = false;
    private Quaternion _endRotation;
    private float _angle;

    // Use this for initialization
    void Start ()
    {
        _connectionStatus = (GameObject.Find("Connection Status"));
        myAnimator = GetComponent<Animator>();
        _lookAt = GetComponent<LookAt>();
    }

    // Update is called once per frame
    void Update()
    {
            if (_turn)
            {
                //myAnimator.SetTrigger("kick");
                //hit empty sound
                FMOD.Studio.EventInstance e1 = FMODUnity.RuntimeManager.CreateInstance(_pickaxeSwing);
                e1.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(transform.position));
                e1.start();
                e1.release();
                //Debug.Log("Played sound of Hit");

                _endRotation = _lookAt.AlignWithCameraDirection(transform);
                _angle = Quaternion.Angle(transform.rotation, _endRotation);

                if (_angle < _angleToMove)
                {
                    if (_connectionStatus.GetComponent<ConnectionStatus>().Connected && photonView.isMine)
                    {
                        photonView.RPC("AnimationSetTrigger", PhotonTargets.All, "kick");
                    }
                    else if (!_connectionStatus.GetComponent<ConnectionStatus>().Connected)
                    {
                        myAnimator.SetTrigger("kick");
                    }
                   
                    if (_lookAt.HasHit)
                    {
                        if (_lookAt.Hit.transform.tag == "Rock")
                        {
                            if (_lookAt.Hit.transform.gameObject.GetComponent<RockScript>() != null)
                            {

                                //hit rock sound
                                FMOD.Studio.EventInstance e = FMODUnity.RuntimeManager.CreateInstance(_hitRock);
                                e.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(transform.position));
                                e.start();
                                e.release();
                                //Debug.Log("Played sound of Hit");


                                float dist = (transform.position - _lookAt.Hit.transform.position).magnitude;
                                if (dist <= _attackDist)
                                {
                                    _lookAt.Hit.transform.gameObject.GetComponent<RockScript>().GetHit(gameObject);
                                }
                            }
                            else
                            {
                                Debug.Log("[Error] The rock you are hitting does not have a RockLifeScript!");
                            }
                        }
                    }
                    _turn = false;
                }
            }
            
            if (Input.GetMouseButtonDown(0))
            {
                _turn = true;
            }
        }
    
    [PunRPC]
    public void AnimationSetTrigger(string trigger)
    {
        myAnimator.SetTrigger(trigger);
    }
}
