using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utility
{
    public static float GetDistance(GameObject pGameObject, GameObject pTarget)
    {
        return (pGameObject.transform.position - pTarget.transform.position).magnitude;
    }
    public static float GetDistance(Vector3 pGameObject, Vector3 pTarget)
    {
        return (pGameObject - pTarget).magnitude;
    }

    public static Vector3 ConnectingVector (Vector3 from, Vector3 to)
    {
        return to-from;
    }

    public static bool WithinDistance(GameObject pGameObject, GameObject pTarget, float pDist) {
        if (pGameObject != null && pTarget != null) {
            return (pGameObject.transform.position - pTarget.transform.position).magnitude < pDist;
        } else {
            return false;
        }
    }
	
	public static bool WithinDistanceSQ(GameObject pGameObject, GameObject pTarget, float pDist)
    {
		if (pGameObject != null && pTarget != null) {
			return (pGameObject.transform.position - pTarget.transform.position).sqrMagnitude < pDist;
		} else {
			return false;
		}
    }
}
