using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfViewSetter : MonoBehaviour
{
    public static Camera MyCamera;

    // Start is called before the first frame update
    void Start()
    {
        MyCamera = this.gameObject.GetComponent<Camera>();
        MyCamera.fieldOfView = GameManager.instance.game.FieldOfView;
    }
}
