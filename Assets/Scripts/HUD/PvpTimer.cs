using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PvpTimer : MonoBehaviour {
    [SerializeField] private Text _pvpTimer;
    [SerializeField] private PvpToggle _pvpToggle;

    private int _timer;
    private float _offset;

    // Update is called once per frame
    private void FixedUpdate () {
        //Debug.Log(gameObject.name+"------------------------------------------------------------------");
        if (_pvpToggle.PvpEnabled) {
            _timer = Convert.ToInt32(_pvpToggle.PvpTimer - (Time.fixedTime - _pvpToggle.pvpTimeOffset));

            int min = _timer / 60;
            int sec = _timer % 60;

            _pvpTimer.text = String.Format("{0:00}:{1:00}", min, sec);
        } else {
            if (_timer == 0 && _pvpTimer.text != "") {
                _pvpTimer.text = "";
            }
        }
    }
}
