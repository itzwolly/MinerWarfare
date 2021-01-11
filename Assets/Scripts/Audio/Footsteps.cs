using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class Footsteps : MonoBehaviour
{
    [FMODUnity.EventRef]
    public string _playerFootsteps;

    private float m_Surface;
   
    //private float m_Snow;

    private float _playerDistanceTraveled = 0.0f;
    private Vector3 _oldPos;

    void Start()
    {

    }

    void Update()
    {
       // Debug.Log(_playerDistanceTraveled);
        _playerDistanceTraveled += (transform.position - _oldPos).magnitude;
        if (_playerDistanceTraveled >= 2.75f)//TODO: Play footstep sound based on position from headbob script
        {
            PlayFootsteps(transform.position);

            _playerDistanceTraveled = 0.0f;
            
        }

        _oldPos = transform.position;
    }

    void PlayFootsteps(Vector3 _playerPos)
    {
        m_Surface = 0.0f;

         RaycastHit hit;

        if (Physics.Raycast(_playerPos, Vector3.down, out hit))
        {
            //Debug.Log(hit.collider.gameObject.layer);
            if (hit.collider.gameObject.layer == 15)
            {
                //Rough Stone
                m_Surface = 4.0f;

            }

            if (hit.collider.gameObject.layer == 13)
            {
                //Wood
                m_Surface = 3.0f;

            }
            if (hit.collider.gameObject.layer == 12)
            {
                //Ground
                m_Surface = 2.0f;

            }

            if (hit.collider.gameObject.layer == 14)
            {
                //Road
                m_Surface = 1.0f;

            }

           
            //playing footsteps
            FMOD.Studio.EventInstance e = FMODUnity.RuntimeManager.CreateInstance(_playerFootsteps);
            e.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(_playerPos));
            //Debug.Log(m_Surface);
            SetParameter(e, "Surface", m_Surface);

            e.start();
            e.release(); 
            SetParameter(e, "Surface", 0f);
        }
    }

    void SetParameter(FMOD.Studio.EventInstance e, string name, float value)
    {
        FMOD.Studio.ParameterInstance parameter;
        e.getParameter(name, out parameter);

        parameter.setValue(value);
    }

}