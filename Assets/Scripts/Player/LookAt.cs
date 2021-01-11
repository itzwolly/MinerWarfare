using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAt : MonoBehaviour {
    private RaycastHit _hit;
    private bool _hasHit;

    [SerializeField] private Camera _camera;

    public RaycastHit Hit {
        get { return _hit; }
    }

    public bool HasHit {
        get { return _hasHit; }
    }

    private void Update() {
        CastRay();
    }

    public void CastRay() {
        Ray ray = _camera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        if (Physics.Raycast(ray, out _hit)) {
            if (!_hasHit) {
                _hasHit = true;
            }
        } else {
            if (_hasHit) {
                _hasHit = false;
            }
        }
    }

    public Quaternion AlignWithCameraDirection(Transform pTransform, float pSpeed = 0.5f) {
        // Player looks at camera direction
        Vector3 direction = new Vector3(_camera.transform.forward.x, 0, _camera.transform.forward.z);
        Vector3 newRotation = Quaternion.LookRotation(direction).eulerAngles;

        pTransform.rotation = Quaternion.Slerp(pTransform.rotation, Quaternion.Euler(newRotation), pSpeed);

        return Quaternion.Euler(newRotation);
    }
}
