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

        DynamicText<float> dynamicText = new(scrollContent, "TextDistUpper", () => { return gm.massBackwardFactor; });
        GameObject textObj = dynamicText.gameObject;
        RectTransform textRect = dynamicText.rectTransform;
        textRect.localScale = new Vector3(3, 3, 1); // テキストのサイズを変更する
        textRect.anchorMin = new Vector2(0, 0); // アンカーの最小値
        textRect.anchorMax = new Vector2(0, 0); // アンカーの最大値
        textRect.pivot = new Vector2(0, 0); // ピボット（ボタン自身の基準点）
        textRect.anchoredPosition = new Vector2(0, 0);

        DynamicDropdown dynamicDropdown = new(scrollContent, "DropdownTest", categories, (x) => { Debug.Log("Selected: " + x); });
        RectTransform dpdnRect = dynamicDropdown.rectTransform;
        dpdnRect.anchoredPosition = new Vector2(0, -100);
        dpdnRect.localScale = new Vector3(3, 3, 1); // ドロップダウンのサイズを変更する
        dpdnRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 300); // RectTransformのx軸方向のサイズを変更する

        DynamicInputField<float> input = new(scrollContent, "InputTest", "Input Value!!!",
            (x) => { gm.massBackwardFactor = x; }, () => { return gm.massBackwardFactor; });
        RectTransform inputRect = input.rectTransform;
        inputRect.anchorMin = new Vector2(0.5f, 0.5f);
        inputRect.anchorMax = new Vector2(0.5f, 0.5f);
        inputRect.localScale = new Vector3(2, 2, 1);

        DynamicSlider slider = new(scrollContent, "SliderTestUpper",
            (x) => { gm.massBackwardFactor = x; }, () => { return gm.massBackwardFactor; }, 0.0f, 1.0f, 0.1f);
        GameObject sliderObj = slider.gameObject;
        RectTransform sliderRect = slider.rectTransform;
        sliderRect.anchoredPosition = new Vector2(300, 200);
        sliderRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 300); // RectTransformのx軸方向のサイズを変更する

        DynamicText<float> dynamicText1 = new(scrollContent, "TextDistLower", () => { return gm.massForwardFactor; }, 50.0f);
        dynamicText1.rectTransform.anchoredPosition = new Vector2(0, 100);

        DynamicSlider slider1 = new(scrollContent, "SliderTestLower",
            (x) => { gm.massBackwardFactor = x; }, () => { return gm.massBackwardFactor; }, 0.0f, 1.0f, 0.1f);
        slider1.rectTransform.anchoredPosition = new Vector2(300, 100);

        DynamicSlider slider2 = new(scrollContent, "SliderTestLower",
            (x) => { gm.massBackwardFactor = x; }, () => { return gm.massBackwardFactor; }, 0.0f, 1.0f, 0.1f);
        slider2.rectTransform.anchoredPosition = new Vector2(300, -500);
    }

    public void VRSettings()
    {
        UnityAction ChangeVrMode = () =>
        {
            gm.VRMode = !gm.VRMode;
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

        UnityAction CaribrateVR = () =>
        {
            cm.CaribrateVR();
        };

        ActionButton caribrateVrButton = new(uiObj, "CaribrateVrButton", "Caribrate HMD(C)", CaribrateVR);
        RectTransform caribrateVrButtonRect = caribrateVrButton.rectTransform;
        caribrateVrButtonRect.anchorMin = new Vector2(1f, 1f); // アンカーの最小値
        caribrateVrButtonRect.anchorMax = new Vector2(1f, 1f); // アンカーの最大値
        caribrateVrButtonRect.pivot = new Vector2(1f, 1f); // ピボット（ボタン自身の基準点）
        caribrateVrButtonRect.localScale = new Vector3(2, 2, 1); // テキストのサイズを変更する
        caribrateVrButtonRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 230); // RectTransformのx軸方向のサイズを変更する
        caribrateVrButtonRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 50); // RectTransformのy軸方向のサイズを変更する
        caribrateVrButtonRect.anchoredPosition = new Vector2(-50, -330); // アンカーを基準にした座標 (pos_x, pos_y) を設定

        DynamicText<bool> dynamicText = new(uiObj, "VRStatus", () => { return gm.VRMode; });
        RectTransform textRect = dynamicText.rectTransform;
        textRect.localScale = new Vector3(2, 2, 1); // テキストのサイズを変更する
        textRect.anchorMin = new Vector2(0.7f, 0.7f); // アンカーの最小値
        textRect.anchorMax = new Vector2(0.7f, 0.7f); // アンカーの最大値
        textRect.pivot = new Vector2(1f, 0.5f); // ピボット（ボタン自身の基準点）
        textRect.anchoredPosition = new Vector2(0, 0);
    }
}
