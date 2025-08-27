using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyCameraController : MonoBehaviour
{
    private GameObject HorizontalLine;
    private Camera FPSCamera; // Camera
    private Camera TPSCamera; // Camera
    private bool VRModeNow;

    // Start is called before the first frame update
    void Start()
    {
        if (MyGameManeger.instance.VRMode)
        {
            FPSCamera = MyGameManeger.instance.Plane.transform.Find("VR_Item(Clone)/[CameraRig]/FPSCamera").gameObject.GetComponent<Camera>();
        }
        else
        {
            FPSCamera = MyGameManeger.instance.Plane.transform.Find("FPSCamera").gameObject.GetComponent<Camera>();
        }
        TPSCamera = MyGameManeger.instance.Plane.transform.Find("TPSCamera").gameObject.GetComponent<Camera>();
        HorizontalLine = GameObject.Find("HUD").transform.Find("HorizontalLine").gameObject;
        VRModeNow = MyGameManeger.instance.VRMode;

        //MyGameManeger.instance.CameraSwitch = true:FPS false:TPS
        TPSCamera.enabled = !MyGameManeger.instance.CameraSwitch;
        FPSCamera.enabled = MyGameManeger.instance.CameraSwitch;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown("v")){SwitchCamera();}

        if(VRModeNow != MyGameManeger.instance.VRMode){
            if(MyGameManeger.instance.VRMode){
                FPSCamera = MyGameManeger.instance.Plane.transform.Find("VR_Item(Clone)/[CameraRig]/FPSCamera").gameObject.GetComponent<Camera>();
                VRModeNow = MyGameManeger.instance.VRMode;
                TPSCamera.enabled = !MyGameManeger.instance.CameraSwitch;
                FPSCamera.enabled = MyGameManeger.instance.CameraSwitch;
            }else{
                FPSCamera = MyGameManeger.instance.Plane.transform.Find("FPSCamera").gameObject.GetComponent<Camera>();
                VRModeNow = MyGameManeger.instance.VRMode;
                TPSCamera.enabled = !MyGameManeger.instance.CameraSwitch;
                FPSCamera.enabled = MyGameManeger.instance.CameraSwitch;
            }
        }

    }

    void SwitchCamera()
    {
        FPSCamera.enabled = !FPSCamera.enabled;
        TPSCamera.enabled = !TPSCamera.enabled;
        MyGameManeger.instance.CameraSwitch = !MyGameManeger.instance.CameraSwitch;
        MyGameManeger.instance.HorizontalLineActive = MyGameManeger.instance.HorizontalLineActive & !MyGameManeger.instance.CameraSwitch;
        HorizontalLine.SetActive(MyGameManeger.instance.HorizontalLineActive);
    }
}
