using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elevator : MonoBehaviour
{
    private AerodynamicParameters aero;

    // Start is called before the first frame update
    void Start()
    {
        aero = GameManager.instance.aero;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.localRotation  = Quaternion.AngleAxis(aero.de, Vector3.forward);
    }
}
