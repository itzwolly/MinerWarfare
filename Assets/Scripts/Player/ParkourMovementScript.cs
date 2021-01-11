using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParkourMovementScript : Photon.MonoBehaviour
{
    [SerializeField] float _pushUpSide;
    [SerializeField] float _pushUpForward;
    [SerializeField] Vector3 _moveVerticalRaycast;
    [SerializeField] Vector3 _moveRaycast;
    [SerializeField] GameObject _head;
    [SerializeField] float _moveSpeed;
    [SerializeField] float _turnSpeed;
    [SerializeField] float _bouncePower;
    [SerializeField] KeyCode _left;
    [SerializeField] KeyCode _up;
    [SerializeField] KeyCode _right;
    [SerializeField] KeyCode _down;
    [SerializeField] KeyCode _jump;
    [SerializeField] KeyCode _sprint;
    [SerializeField] float _distSide;
    [SerializeField] float _distForward;
    [SerializeField] float _distDown;
    [SerializeField] float _jumpPower;
    [SerializeField] private Camera _camera;
    [SerializeField] private float _halfHeight;
    [SerializeField] private float _offset;
    [SerializeField] private float _angleToMove;
    [SerializeField] private float _sprintMultiplier;

    private Animator myAnimator;
    private LookAt _lookAt;

    float _prevXVal=0;
    float _prevZVal=0;

    Vector3 _botSide;
    Vector3 _topSide;

    Vector3 _leftBack;
    Vector3 _rightBack;
    Vector3 _leftFor;
    Vector3 _rightFor;

    private float _distToGround;

    // Use this for initialization
    float _xRotate;
    float _zRotate;
    bool _jumped;
    bool _onWall;
    Vector3 _jumpDirection;
    private  Rigidbody _rigidBody;
    RaycastHit hit;
    private float _currentMoveSpeed;
    private float _sprintSpeed;
    GameObject _connectionStatus;
    private bool _grounded;

    void Start () {
        _connectionStatus = (GameObject.Find("Connection Status"));
        
        _onWall = false;
        _jumped = false;
        _currentMoveSpeed = _moveSpeed;
        _sprintSpeed = _currentMoveSpeed;

        myAnimator = GetComponent<Animator>();
        _lookAt = GetComponent<LookAt>();
        _rigidBody = gameObject.GetComponent<Rigidbody>();
        _distToGround = GetComponent<Collider>().bounds.extents.y - GetComponent<Collider>().bounds.center.y;
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        //Debug.Log(_connectionStatus.GetComponent<ConnectionStatus>().Connected + "    " + !photonView.isMine);
        //transform.Rotate(0, Input.GetAxis("Mouse X") * _turnSpeed * Time.deltaTime, 0);
        //_head.transform.Rotate(-Input.GetAxis("Mouse Y") * _turnSpeed * Time.deltaTime, 0, 0);

        if (photonView.isMine || !_connectionStatus.GetComponent<ConnectionStatus>().Connected)
        {
            if (Input.GetKey(_left))
            {
                Quaternion endRotation = _lookAt.AlignWithCameraDirection(transform);
                float angle = Quaternion.Angle(transform.rotation, endRotation);

                if (angle < _angleToMove)
                {
                    if (_jumped)
                    {
                        _rigidBody.AddRelativeForce(-Vector3.right * (_sprintSpeed / 2) * Time.fixedDeltaTime, ForceMode.VelocityChange);
                    }
                    else
                    {
                        _rigidBody.AddRelativeForce(-Vector3.right * _sprintSpeed * Time.fixedDeltaTime, ForceMode.VelocityChange);
                    }
                }
            }

            if (Input.GetKey(_right))
            {
                Quaternion endRotation = _lookAt.AlignWithCameraDirection(transform);
                float angle = Quaternion.Angle(transform.rotation, endRotation);

                if (angle < _angleToMove)
                {
                    if (_jumped)
                    {
                        _rigidBody.AddRelativeForce(Vector3.right * (_sprintSpeed / 2) * Time.fixedDeltaTime, ForceMode.VelocityChange);
                    }
                    else
                    {
                        _rigidBody.AddRelativeForce(Vector3.right * _sprintSpeed * Time.fixedDeltaTime, ForceMode.VelocityChange);
                    }
                }
            }

            if (Input.GetKey(_up))
            {
                Quaternion endRotation = _lookAt.AlignWithCameraDirection(transform);
                float angle = Quaternion.Angle(transform.rotation, endRotation);

                if (angle < _angleToMove)
                {
                    if (_jumped)
                    {
                        _rigidBody.AddRelativeForce(Vector3.forward * (_sprintSpeed / 2) * Time.fixedDeltaTime, ForceMode.VelocityChange);
                    }
                    else
                    {
                        _rigidBody.AddRelativeForce(Vector3.forward * _sprintSpeed * Time.fixedDeltaTime, ForceMode.VelocityChange);
                    }
                }
            }

            if (Input.GetKey(_down))
            {
                Quaternion endRotation = _lookAt.AlignWithCameraDirection(transform);
                float angle = Quaternion.Angle(transform.rotation, endRotation);

                if (angle < _angleToMove)
                {
                    if (_jumped)
                    {
                        _rigidBody.AddRelativeForce(-Vector3.forward * (_sprintSpeed / 2) * Time.fixedDeltaTime, ForceMode.VelocityChange);
                    }
                    else
                    {
                        _rigidBody.AddRelativeForce(-Vector3.forward * _sprintSpeed * Time.fixedDeltaTime, ForceMode.VelocityChange);
                    }
                }
            }

            if (Input.GetKeyDown(_sprint))
            {
                _sprintSpeed = _currentMoveSpeed * _sprintMultiplier;
            }
            else if (Input.GetKeyUp(_sprint))
            {
                _sprintSpeed = _currentMoveSpeed;
            }

            if (Input.GetKeyDown(_jump) && IsGrounded())
            {
                if (_connectionStatus.GetComponent<ConnectionStatus>().Connected && photonView.isMine) {
                    photonView.RPC("AnimationMovementSetTrigger", PhotonTargets.All, "jump");
                } else {
                    myAnimator.SetTrigger("jump");
                }

                _rigidBody.AddRelativeForce(Vector3.up * _jumpPower, ForceMode.VelocityChange);
            }
            Vector3 _oldVel = _rigidBody.velocity;

            if (_connectionStatus.GetComponent<ConnectionStatus>().Connected && photonView.isMine)
            {
                if (Mathf.Abs(transform.InverseTransformDirection(_oldVel).z - _prevZVal)>=5)
                {
                    Debug.Log("updating walk animation");
                    photonView.RPC("AnimationSetFoat", PhotonTargets.All, "speed", transform.InverseTransformDirection(_oldVel).z);
                    _prevZVal = transform.InverseTransformDirection(_oldVel).z;
                }
                if (Mathf.Abs(transform.InverseTransformDirection(_oldVel).x - _prevXVal)>=5)
                {
                    Debug.Log("updating walk animation");
                    photonView.RPC("AnimationSetFoat", PhotonTargets.All, "sidespeed", transform.InverseTransformDirection(_oldVel).x);
                    _prevXVal = transform.InverseTransformDirection(_oldVel).x;
                }
            }
            else
            {
                myAnimator.SetFloat("speed", transform.InverseTransformDirection(_oldVel).z);
                myAnimator.SetFloat("sidespeed", transform.InverseTransformDirection(_oldVel).x);
            }

            _rigidBody.velocity = new Vector3(0, _rigidBody.velocity.y, 0);
        }
    }
    

    private bool IsGrounded() {
        return Physics.Raycast(transform.position, -Vector3.up, _distToGround + 0.1f);
    }

    [PunRPC]
    public void AnimationSetFoat(string trigger,float val)
    {
        if (myAnimator != null) {
            myAnimator.SetFloat(trigger, val);
        } else {
            Debug.Log("[ERROR] Animator in ParkourMovementScript is null, in AnimationSetFoat");
        }
    }
    [PunRPC]
    public void AnimationMovementSetTrigger(string trigger)
    {
        if (myAnimator != null) {
            myAnimator.SetTrigger(trigger);
        } else {
            Debug.Log("[ERROR] Animator in ParkourMovementScript is null, in AnimationMovementSetTrigger");
        }
    }
}
