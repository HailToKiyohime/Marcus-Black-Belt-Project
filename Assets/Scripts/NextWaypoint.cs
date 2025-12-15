using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextWaypoint : MonoBehaviour
{
    [Header("Success → forwards")]
    public GameObject[] possibleWaypoints;

    [Header("Fail → backwards (optional)")]
    public GameObject[] possiblePastWaypoints;

    public Transform PickWaypoint()
    {
        return possibleWaypoints[Random.Range(0, possibleWaypoints.Length)].transform;
    }

    public bool TryPickPast(out Transform t)
    {
        if (possiblePastWaypoints != null && possiblePastWaypoints.Length > 0)
        {
            t = possiblePastWaypoints[Random.Range(0, possiblePastWaypoints.Length)].transform;
            return true;
        }
        t = null;
        return false;
    }
}
