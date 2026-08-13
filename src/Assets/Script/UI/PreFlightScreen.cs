using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Events; // UnityActionのために必要
using UnityEngine.SceneManagement; // `LoadScene`のために必要

public class PreFlightScreen
{
    private GameManager gm;
    private CameraManager cm;
    private UIManager ui;
    private GameObject basePanel;
    private GameObject baseScrollView;
    private GameObject scrollContent; // `Scroll View`を作成したときに自動的にできる子オブジェクト.
    private GameObject uiObj;

    public PreFlightScreen(GameObject basePanel, GameObject baseScrollView)
    {
        gm = GameManager.instance;
        cm = GameObject.Find("CameraManager").GetComponent<CameraManager>();
        ui = UIManager.instance;
        uiObj = ui.gameObject;
        this.basePanel = basePanel;
        this.baseScrollView = baseScrollView;
        scrollContent = baseScrollView.transform.Find("Viewport/Content").gameObject; // `Scroll View`の子オブジェクトである`Viewport/Content`を探して代入.
        // scrollContentの子オブジェクトはスクロールできる.
        // Content Size Fitterをアタッチすると，Contentのサイズが子オブジェクトに合わせて自動的に変わるので，スクロールできる範囲も自動的に変わる.
        // スクロールしたい方を`Preffered Size`にして，もう片方を`Unconstrained`にするのが一般的. 今回は縦スクロールなので，縦を`Preferred Size`，横を`Unconstrained`にするのが良いと思う.
        // `Layout Group`がないと上手く動かないっぽい.
    }

    private List<string> categories = new List<string> { "機体設定", "環境設定", "その他" };

    public void Test()
    {
        StaticText<string> staticText = new(basePanel, "StaticTextTest", "カテゴリー");
        RectTransform staticRect = staticText.rectTransform;
        staticRect.anchoredPosition = new Vector2(0, -50);
        staticRect.localScale = new Vector3(3, 3, 1); // テキストのサイズを変更する
        staticRect.anchorMin = new Vector2(0, 0.5f); // アンカーの最小値
        staticRect.anchorMax = new Vector2(0, 0.5f); // アンカーの最大値
        staticRect.pivot = new Vector2(0, 0.5f); // ピボット（ボタン自身の基準点）
    }

    public void VRSettings()
    {
        UnityAction ChangeVrMode = () =>
        {
            gm.game.VRMode = !gm.game.VRMode;
        };

        ActionButton vrModeButton = new(uiObj, "VRModeButton", "Change VR Mode(V)", ChangeVrMode);
        RectTransform vrModeButtonRect = vrModeButton.rectTransform;
        vrModeButtonRect.anchorMin = new Vector2(1f, 1f); // アンカーの最小値
        vrModeButtonRect.anchorMax = new Vector2(1f, 1f); // アンカーの最大値
        vrModeButtonRect.pivot = new Vector2(1f, 1f); // ピボット（ボタン自身の基準点）
        vrModeButtonRect.localScale = new Vector3(2, 2, 1); // テキストのサイズを変更する
        vrModeButtonRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 230); // RectTransformのx軸方向のサイズを変更する
        vrModeButtonRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 50); // RectTransformのy軸方向のサイズを変更する
        vrModeButtonRect.anchoredPosition = new Vector2(-50, -200); // アンカーを基準にした座標 (pos_x, pos_y) を設定

        UnityAction CalibrateVR = () =>
        {
            cm.CalibrateVR();
        };

        ActionButton calibrateVrButton = new(uiObj, "CalibrateVrButton", "Calibrate HMD(C)", CalibrateVR);
        RectTransform calibrateVrButtonRect = calibrateVrButton.rectTransform;
        calibrateVrButtonRect.anchorMin = new Vector2(1f, 1f); // アンカーの最小値
        calibrateVrButtonRect.anchorMax = new Vector2(1f, 1f); // アンカーの最大値
        calibrateVrButtonRect.pivot = new Vector2(1f, 1f); // ピボット（ボタン自身の基準点）
        calibrateVrButtonRect.localScale = new Vector3(2, 2, 1); // テキストのサイズを変更する
        calibrateVrButtonRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 230); // RectTransformのx軸方向のサイズを変更する
        calibrateVrButtonRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 50); // RectTransformのy軸方向のサイズを変更する
        calibrateVrButtonRect.anchoredPosition = new Vector2(-50, -330); // アンカーを基準にした座標 (pos_x, pos_y) を設定

        DynamicText<bool> dynamicText = new(uiObj, "VRStatus", () => { return gm.game.VRMode; });
        RectTransform textRect = dynamicText.rectTransform;
        textRect.localScale = new Vector3(2, 2, 1); // テキストのサイズを変更する
        textRect.anchorMin = new Vector2(0.7f, 0.7f); // アンカーの最小値
        textRect.anchorMax = new Vector2(0.7f, 0.7f); // アンカーの最大値
        textRect.pivot = new Vector2(1f, 0.5f); // ピボット（ボタン自身の基準点）
        textRect.anchoredPosition = new Vector2(0, 0);
    }
}
