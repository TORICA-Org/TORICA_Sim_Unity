using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.XR;

public class CameraManager : MonoBehaviour
{
    [TextArea(5, 15)]
    [Tooltip("備忘録や仕様のメモなどを自由に書き込めます")]
    public string note = "複数ディスプレイにも対応した画面切り替えとVRの制御を行っています．" +
        "XR Origin (XR Rig)とXR Interaction Managerを格納し，VRの原点を決めています．";

    private GameManager gm;
    private UIManager ui;

    private GameObject FPSObj; // 一人称視点Cameraのオブジェクト
    private Camera FPSCamera; // 一人称視点Camera

    private GameObject TPSObj; // 三人称視点Cameraのオブジェクト
    private Camera TPSCamera; // 三人称視点Camera

    private GameObject SideObj; // サイドビューCameraのオブジェクト
    private Camera SideCamera; // サイドビューCamera

    private GameObject XROrigin; // VR用のXR Origin (XR Rig)
    private Camera XRCamera = null; // XR OriginのCamera

    private ManualXRControl manualXRControl = new(); // VR制御用クラスを保持するフィールド.
    private MultiCameraDisplay multiCameraDisplay; // カメラ制御用クラスを保持するフィールド.

    private bool VRModePre = false; // VRモードが前回のフレームで有効かどうかを保持するフィールド

    private AudioListener audioListener; // AudioListenerコンポーネントを保持するフィールド

    // GameManager.csへ移動
    //private Vector3 caribrationOffset = Vector3.zero; // キャリブレーションのオフセットを保持するフィールド.
    //private Quaternion calibrationRotationOffset = Quaternion.identity; // 回転のキャリブレーションのオフセットを保持するフィールド.

    // ===== オブジェクトが生成された際に実行されるメソッド =====================================
    private void Start()
    {
        gm = GameManager.instance; // `GameManager`のインスタンスを取得して代入.
        ui = UIManager.instance; // `UIManager`のインスタンスを取得して代入.


        // Cameraのオブジェクトとコンポーネントを取得.
        FPSObj = GameManager.instance.game.Plane.transform.Find("FPSCamera").gameObject;
        FPSCamera = FPSObj.GetComponent<Camera>();
        TPSObj = GameManager.instance.game.Plane.transform.Find("TPSCamera").gameObject;
        TPSCamera = TPSObj.GetComponent<Camera>();
        SideObj = GameObject.Find("SideCamera");
        SideCamera = SideObj.GetComponent<Camera>();

        //ui.SetRenderCamera(TPSCamera); // UIManagerにFPSCameraを渡す.

        // XR Origin (XR Rig) オブジェクトとCameraコンポーネントを取得
        XROrigin = GameObject.Find("XR Origin (XR Rig)");
        XRCamera = XROrigin.transform.Find("Camera Offset/Main Camera").GetComponent<Camera>();
        multiCameraDisplay = new();
        //multiCameraDisplay.ConfigureEventSystem(); // EventSystemの設定を行う.

        XRSettings.gameViewRenderMode = GameViewRenderMode.None;

        // AudioListenerコンポーネントをMain Cameraから取得して保持する.
        audioListener = transform.Find("XR Origin (XR Rig)/Camera Offset/Main Camera").GetComponent<AudioListener>();
    }

    // ===== 毎フレーム実行されるメソッド =================================================
    private void Update()
    {
        // "f5"キーが押されたらカメラを切り替える.
        if (Input.GetKeyDown("f5") && !gm.game.VRMode)
        {
            Config.MainCamera = !Config.MainCamera;
        }
        // "v"キーが押されたらVRモードを切り替える.
        if (Input.GetKeyDown("v"))
        {
            gm.game.VRMode = !gm.game.VRMode;
        }
        // "c"キーが押されたらVRのキャリブレーションを行う.
        if (Input.GetKeyDown("c"))
        {
            CalibrateVR();
        }
        //
        if (Input.GetKeyDown("f6"))
        {
            multiCameraDisplay.ToggleSubDisplay();
        }

        if (gm.game.VRMode && !gm.game.isVRInitialized)
        {
            EnebleVR();
        }
        else if (!gm.game.VRMode && gm.game.isVRInitialized)
        {
            DisableVR();
        }

        SyncCameraFlag();

        // `XR Origin`は`GameManager.instance.Plane`の子にせず，スクリプトで追従させる.
        // `XR Origin`の親であるCameraManager(=XROrigin)をFPSカメラの位置と合わせる.
        Vector3 FPSPosition = FPSObj.transform.position; // FPSカメラの位置を保存.
        XROrigin.transform.position = FPSPosition - gm.game.caribrationOffset; // CameraManagerの位置をFPSカメラの位置に合わせる（キャリブレーションオフセットを考慮）.

        Vector3 FPSRotation = FPSObj.transform.rotation.eulerAngles; // FPSカメラの回転を保存.
        XROrigin.transform.rotation = Quaternion.Inverse(gm.game.calibrationRotationOffset) * FPSObj.transform.rotation; // CameraManagerの回転をキャリブレーションオフセットに合わせる.

        //Debug.Log("HMD Z Axis Movement: " + GetZAxisMovement());
        //Debug.Log("displays connected: " + Display.displays.Length);

        // サイドビューカメラの位置設定.
        Vector3 sideObjOffset = new(10f, 3f, -10f); // FPSカメラに対して右前方に配置.
        SideObj.transform.position = FPSObj.transform.position + sideObjOffset; // サイドビューカメラの位置をFPSカメラの位置にオフセットして設定.
        SideObj.transform.LookAt(gm.game.Plane.transform); // サイドビューカメラをFPSカメラの方に向ける.

        if (gm.game.VRMode != VRModePre)
        {
            VRModePre = gm.game.VRMode;
        }
    }

    // ===== VRモードを有効にするメソッド =================================================
    private void EnebleVR()
    {
        gm.game.isVRInitialized = true; // VR初期化済のフラグを立てる.

        // VRのカメラとtargetDisplayが被るとエラーとなるが，`enabled = false`なら大丈夫.
        FPSCamera.targetDisplay = 0; // 固定.
        TPSCamera.targetDisplay = 1; // 一時的に固定.
        XRCamera.targetDisplay = 0; // インスペクターでも`Display 1`に設定済みだが念の為.

        FPSCamera.enabled = false; // FPSカメラを無効にする（VRモードではXR OriginのCameraが使用されるため）.
        XRCamera.enabled = true; // XRカメラを有効にする.

        // 他のカメラのTargetEyeをNoneにする（念の為上書き）.
        //FPSCamera.stereoTargetEye = StereoTargetEyeMask.None;
        //TPSCamera.stereoTargetEye = StereoTargetEyeMask.None;

        // audioListener.enabled = false; // AudioListenerを無効にする（VRモードではXR OriginのAudioListenerが使用されるため）.

        try
        {
            StartCoroutine(manualXRControl.StartXRCoroutine());
            multiCameraDisplay.SetGameViewRenderMode(false); // ゲームビューにVR映像を出力しない.
        }
        catch (System.Exception e)
        {
            Debug.LogWarning("VR init error: " + e.Message);

            manualXRControl.StopXR();
            gm.game.VRMode = false;
            gm.game.isVRInitialized = false; // 初期化に失敗した場合はフラグを下ろす.

            return;
        }

        TPSCamera.targetDisplay = 0; // 固定.

        Vector3 zAxisRotation = new(0f, 90f, 0f);
        XROrigin.transform.eulerAngles = zAxisRotation; // CameraManagerの向きをFPSカメラと同様にy軸について90deg回転させる.
    }

    // ===== VRモードを無効にするメソッド =================================================
    private void DisableVR()
    {
        gm.game.isVRInitialized = false; // VR初期化済のフラグを下ろす.
        manualXRControl.StopXR(); // VRを停止する.
        XRCamera.enabled = false;
        FPSCamera.enabled = true;
    }

    // ===== カメラの状態をフラグと同期するメソッド =================================================
    private void SyncCameraFlag()
    {
        //int displayNum = -1;

        if (gm.game.isVRInitialized)
        {
            //displayNum = XRCamera.targetDisplay; // これはおそらく0になるはず.
            return;
        }

        /*
        if (Config.IsMainDisplayTPS)
        {
            TPSCamera.targetDisplay = displayNum + 1; // VR OFF -> 0, VR ON -> 1
            FPSCamera.targetDisplay = displayNum + 2; // VR OFF -> 1, VR ON -> 2
        }
        else
        {
            TPSCamera.targetDisplay = displayNum + 2; // VR OFF -> 1, VR ON -> 2
            FPSCamera.targetDisplay = displayNum + 1; // VR OFF -> 0, VR ON -> 1
        }
        */
        if (Config.MainCamera)
        {
            TPSCamera.targetDisplay = 0; // VR OFF -> 0
            FPSCamera.targetDisplay = 1; // VR OFF -> 1
        }
        else
        {
            TPSCamera.targetDisplay = 1; // VR OFF -> 1
            FPSCamera.targetDisplay = 0; // VR OFF -> 0
        }
    }

    // ===== キャリブレーションを行うメソッド（staticなのでインスタンス化無しで呼べる） ============
    public void CalibrateVR()
    {
        if (!XRCamera || !gm.game.isVRInitialized)
        {
            Debug.LogWarning("Caribration failed: XRCamera is not initialized.");
            return;
        }

        // カメラにとっては，前後: z軸，左右: x軸である．

        //Vector3 vrCameraLocalOffset = XRCamera.transform.localPosition; // XRカメラのローカル位置を取得.
        // XRCameraのローカル位置はグローバル座標と同じように，前後: z軸，上下: y軸，左右: x軸で保持されている.

        // 現在のXRカメラのローカル位置をキャリブレーションオフセットとして保存.
        //caribrationOffset.x = vrCameraLocalOffset.z; // 前後方向のオフセット.
        //caribrationOffset.y = vrCameraLocalOffset.y; // 上下方向のオフセット.
        //caribrationOffset.z = vrCameraLocalOffset.x; // 左右方向のオフセット.
        // CameraManagerの向きはFPSカメラと同様にy軸について90deg回転しているため，前後: x軸，上下: y軸，左右: z軸である.

        Vector3 vrCameraGrobalOffset = XRCamera.transform.position - XROrigin.transform.position; // XRカメラのグローバル位置からCameraManagerのグローバル位置を引いてオフセットを取得.
        Debug.Log("offsetX: " + vrCameraGrobalOffset.x + "\toffsetY: " + vrCameraGrobalOffset.y + "\toffsetZ: " + vrCameraGrobalOffset.z); // オフセットをログに出力.
        gm.game.caribrationOffset = vrCameraGrobalOffset; // オフセットとして代入.

        Quaternion relativeRotation = Quaternion.Inverse(XROrigin.transform.rotation) * XRCamera.transform.rotation; // XRカメラのヨーをキャリブレーションオフセットとして保存（ピッチとロールは無視）.
        Debug.Log("offsetYaw: " + XRCamera.transform.rotation.eulerAngles.y); // 回転のオフセットをログに出力.
        gm.game.calibrationRotationOffset = Quaternion.Euler(0, relativeRotation.eulerAngles.y, 0);
    }

    // ===== VRゴーグルの前後移動量を返すメソッド ============================
    public float GetZAxisMovement()
    {
        if (!XRCamera || !gm.game.isVRInitialized)
        {
            Debug.LogWarning("Getting movement failed: XRCamera is not initialized.");
            return 0f;
        }
        return XRCamera.transform.localPosition.z; // 前後移動量を返す.
    }

    // ===== オブジェクトが破棄された際に実行されるメソッド ======================================
    private void OnDestroy()
    {
        if (gm == null) return;
        if (gm.game == null) return;
        // オブジェクトが破棄される際にVRを停止する.
        if (gm.game.isVRInitialized)
        {
            gm.game.isVRInitialized = false;
            manualXRControl.StopXR();
        }
    }

    // ===== アプリケーションが終了する際に実行されるメソッド ===================================
    private void OnApplicationQuit()
    {
        // アプリケーションが終了する際にVRを停止する.
        if (gm.game.isVRInitialized)
        {
            gm.game.isVRInitialized = false;
            manualXRControl.StopXR();
        }
    }
}
