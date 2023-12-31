using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalController : MonoBehaviour
{
    [SerializeField]
    float _xRange = 4.5f;
    [SerializeField]
    float _zRange = 4.5f;

    public void ResetPos()
    {
        float xPos = Random.Range(-_xRange, _xRange);
        float zPos = Random.Range(-_zRange, _zRange);

        transform.position = new Vector3(xPos, 0, -5.0f+zPos);
    }
}
