using UnityEngine;
using UnityEngine.XR;
using UnityEngine.EventSystems;

public class MultiCameraDisplay
{
    private GameManager gm;

    private Camera FPSCamera; // 一人称視点Camera
    private Camera TPSCamera; // 三人称視点Camera
    private Camera XRCamera; // XR OriginのCamera

    private GameObject xrEventSystem; // EventSystemを保持するフィールド.

    private bool isSubDisplayActive = false; // サブディスプレイがアクティブかどうかを示すフラグ.

    public MultiCameraDisplay()
    {
        gm = GameManager.instance;

        // Cameraのオブジェクトとコンポーネントを取得.
        GameObject FPSObj = gm.game.Plane.transform.Find("FPSCamera").gameObject;
        FPSCamera = FPSObj.GetComponent<Camera>();
        GameObject TPSObj = gm.game.Plane.transform.Find("TPSCamera").gameObject;
        TPSCamera = TPSObj.GetComponent<Camera>();

        // XR Origin (XR Rig) オブジェクトとCameraコンポーネントを取得
        GameObject XROrigin = GameObject.Find("XR Origin (XR Rig)");
        XRCamera = XROrigin.transform.Find("Camera Offset/Main Camera").GetComponent<Camera>();

        //xrEventSystem = GameObject.Find("XREventSystem");
    }

    /*
    public void ConfigureEventSystem()
    {
        if (xrEventSystem == null) return;

        xrEventSystem.SetActive(false);
    }
    */

    public void SetGameViewRenderMode(bool doRender)
    {
        if (doRender)
        {
            XRSettings.gameViewRenderMode = GameViewRenderMode.LeftEye;
        }
        else
        {
            XRSettings.gameViewRenderMode = GameViewRenderMode.None;
        }
    }

    // ===== サブディスプレイのアクティブ状態を切り替えるメソッド（必要に応じて呼び出す） ==================
    public void ToggleSubDisplay()
    {
        Debug.Log("displays connected: " + Display.displays.Length);
        gm.game.error = true;
        if (!isSubDisplayActive && Display.displays.Length > 1) // サブディスプレイがアクティブでなく、かつ複数のディスプレイが接続されている場合
        {
            isSubDisplayActive = true;
            Display.displays[1].Activate();
            gm.game.errorText = "Sub display activated.";
        }
        else
        {
            isSubDisplayActive = false;
            GameManager.instance.game.errorText = "Sub display deactivated. Please restart this game.";
        }
    }

    /*
    Camera _thirdViewCamera;

    void Start()
    {
        Debug.Log("displays connected: " + Display.displays.Length);
        // Display.displays[0] は主要なデフォルトのディスプレイで、常にオンです。ですから、インデックス 1 から始まります。
        // その他のディスプレイが使用可能かを確認し、それぞれをアクティブにします。

        //for (int i = 1; i < Display.displays.Length; i++)
        //{
        //    Display.displays[i].Activate();
        //}

        //カメラコンポーネントを取得
        _thirdViewCamera = GetComponent<Camera>();
        //PCディスプレイ表示を三人称視点カメラに切り替え
        //OnThirdView();
    }

    public void ActivateSecondDisplay()
    {
        if (Display.displays.Length > 1)
        {
            Display.displays[1].Activate();
        }
    }

    //PCディスプレイにプレイヤー目線を表示
    void OnPlayerView()
    {
        _thirdViewCamera.enabled = false;

        if (GameManager.instance.VRMode)
        {
            XRSettings.gameViewRenderMode = GameViewRenderMode.LeftEye;
        }
    }

    //PCディスプレイにThirdViewCamera映像を表示
    void OnThirdView()
    {
        _thirdViewCamera.enabled = true;

        if (GameManager.instance.VRMode)
        {
            XRSettings.gameViewRenderMode = GameViewRenderMode.None;
        }
    }
    */
}