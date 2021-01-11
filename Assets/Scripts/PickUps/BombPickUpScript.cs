using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BombPickUpScript : MonoBehaviour {
    [SerializeField] private Text _bombPickUpText;
    [SerializeField] private BombThrowScript _throw;


    [FMODUnity.EventRef]
    public string _pickup;

    // Use this for initialization
    void Start() {
        //_throw = gameObject.GetComponent<BombThrowScript>();
        _bombPickUpText.text = _throw.Bombs.ToString();
    }

    public void FillTemporary(Text bombText) {
        _bombPickUpText = bombText;
        _bombPickUpText.text = _throw.Bombs.ToString();
    }

    public void UpdateText()
    {
        if (GetComponent<PhotonView>().isMine || GameObject.Find("Connection Status").GetComponent<ConnectionStatus>().Connected) {
            _bombPickUpText.text = _throw.Bombs.ToString();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "BombPickUp")
        {
            //click sound
            FMOD.Studio.EventInstance e1 = FMODUnity.RuntimeManager.CreateInstance(_pickup);
            e1.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(transform.position));
            e1.start();
            e1.release();
            //Debug.Log("Played sound of Hit");
            Debug.Log(other.transform.name);
            other.gameObject.GetComponent<BombPickUpBehaviour>().GetDestroyed();
            _throw.AddBomb();

            if (GetComponent<PhotonView>().isMine || !GameObject.Find("Connection Status").GetComponent<ConnectionStatus>().Connected) { 
                _bombPickUpText.text = _throw.Bombs.ToString();
            }
        }
    }
}
