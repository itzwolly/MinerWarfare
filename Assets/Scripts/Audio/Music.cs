using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

namespace CombatSystem
{
    public class Music : MonoBehaviour
    {
        [FMODUnity.EventRef] public string _Music;

        public static Music instance;
        private FMOD.Studio.EventInstance _musicStart;
        private Vector3 _oldPos;

        private void Awake() {
            if (instance != null)
            {
                Destroy(this);
            }
            else
            {
                instance = this;
            }
        }

        void Start()
        {
            _musicStart = FMODUnity.RuntimeManager.CreateInstance(_Music);
            SetParameter(_musicStart, "Health", 100);
            _musicStart.start();
        }

        public void ChangePart(float _value)
        {
            SetParameter(_musicStart, "Song Part", _value);
        }

        public void ChangeBattleIntensity(float _value)
        {
            SetParameter(_musicStart, "Battle", _value);
        }
    

        void SetParameter(FMOD.Studio.EventInstance e, string name, float value)
        {
            FMOD.Studio.ParameterInstance parameter;
            e.getParameter(name, out parameter);

            parameter.setValue(value);
        }

    }
}