using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class FlightSettingScreen
{
    private GameManager gm;
    private GameObject flightSettingObj;
    private GameObject connection;
    private GameObject main;

    //private StaticText<string> labelLengthSetting;
    //private StaticText<string> labelForwardLength;
    //private StaticText<string> labelBackwardLength;
    //private DynamicText<float> valueForwardLength;
    //private DynamicText<float> valueBackwardLength;
    //private DynamicSlider lengthForward;
    //private DynamicSlider lengthBackward;

    public FlightSettingScreen()
    {
        gm = GameManager.instance;
        SceneManager.sceneLoaded += Generate;
    }

    public void Generate(Scene scene, LoadSceneMode mode)
    {
        UIBase.DisposeAll(); // `UIBase`の全てのインスタンスを破棄
        Generate();
    }

    public void Generate()
    {
        if (flightSettingObj == null)
        {
            flightSettingObj = GameObject.Find("FlightSetting");
        }

        // ----- Main ----------------------------------------------------------------------------------
        if (main == null)
        {
            main = flightSettingObj.transform.Find("Main").gameObject;
        }

        DynamicText<string> AircraftDataName = new(main,
            "AircraftDataName", () => Config.AircraftDataName);
        RectTransform rectAircraftDataName = AircraftDataName.rectTransform;
        rectAircraftDataName.anchoredPosition = new Vector2(-100, 70);
        rectAircraftDataName.pivot = new Vector2(0f, 0.5f);
        rectAircraftDataName.localScale = new Vector3(0.6f, 0.6f, 1f);
        rectAircraftDataName.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 1000); // RectTransformのx軸方向のサイズを変更する

        // ----- Connection -----------------------------------------------------------------------------
        if (connection == null)
        {
            connection = flightSettingObj.transform.Find("Connection").gameObject;
        }

        StaticText<string> labelLengthSetting = new(connection, "labelLengthSetting", "重心センサーの接続位置");
        RectTransform rectLabelLengthSetting = labelLengthSetting.rectTransform;
        rectLabelLengthSetting.anchoredPosition = new Vector2(150, -10);
        rectLabelLengthSetting.pivot = new Vector2(0f, 1f); // ピボット（ボタン自身の基準点）
        rectLabelLengthSetting.localScale = new Vector3(0.5f, 0.5f, 1f); // テキストのサイズを変更する
        rectLabelLengthSetting.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 600); // RectTransformのx軸方向のサイズを変更する

        StaticText<string> labelForwardLength = new(connection, "labelForwardLength", "前方");
        RectTransform rectLabelForwardLength = labelForwardLength.rectTransform;
        rectLabelForwardLength.anchoredPosition = new Vector2(150, -60);
        rectLabelForwardLength.pivot = new Vector2(0f, 0.5f); // ピボット（ボタン自身の基準点）
        rectLabelForwardLength.localScale = new Vector3(0.5f, 0.5f, 1f); // テキストのサイズを変更する

        StaticText<string> labelBackwardLength = new(connection, "labelBackwardLength", "後方");
        RectTransform rectLabelBackwardLength = labelBackwardLength.rectTransform;
        rectLabelBackwardLength.anchoredPosition = new Vector2(150, -100);
        rectLabelBackwardLength.pivot = new Vector2(0f, 0.5f); // ピボット（ボタン自身の基準点）
        rectLabelBackwardLength.localScale = new Vector3(0.5f, 0.5f, 1f); // テキストのサイズを変更する

        DynamicText<float> valueForwardLength = new(connection, "valueForwardLength", () => { return gm.game.lengthForward; });
        RectTransform rectValueForwardLength = valueForwardLength.rectTransform;
        rectValueForwardLength.anchoredPosition = new Vector2(240, -60);
        rectValueForwardLength.pivot = new Vector2(0f, 0.5f); // ピボット（ボタン自身の基準点）
        rectValueForwardLength.localScale = new Vector3(0.5f, 0.5f, 1f); // テキストのサイズを変更する

        DynamicText<float> valueBackwardLength = new(connection, "valueBackwardLength", () => { return gm.game.lengthBackward; });
        RectTransform rectValueBackwardLength = valueBackwardLength.rectTransform;
        rectValueBackwardLength.anchoredPosition = new Vector2(240, -100);
        rectValueBackwardLength.pivot = new Vector2(0f, 0.5f); // ピボット（ボタン自身の基準点）
        rectValueBackwardLength.localScale = new Vector3(0.5f, 0.5f, 1f);

        DynamicText<string> serialStatus = new(connection, "serialStatus", () => { return SerialHandler.status; });
        RectTransform rectSerialStatus = serialStatus.rectTransform;
        rectSerialStatus.anchoredPosition = new Vector2(-175, 145);
        rectSerialStatus.pivot = new Vector2(0f, 0.5f); // ピボット（ボタン自身の基準点）
        rectSerialStatus.localScale = new Vector3(0.5f, 0.5f, 1f); // テキストのサイズを変更する
        rectSerialStatus.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 1300); // RectTransformのx軸方向のサイズを変更する

        DynamicText<string> serialPort = new(connection, "serialPort", () => { return Config.SerialPort; });
        RectTransform rectSerialPort = serialPort.rectTransform;
        rectSerialPort.anchoredPosition = new Vector2(-175, 90);
        rectSerialPort.pivot = new Vector2(0f, 0.5f); // ピボット（ボタン自身の基準点）
        rectSerialPort.localScale = new Vector3(0.7f, 0.7f, 1f); // テキストのサイズを変更する

        DynamicSlider lengthForward = new(connection, "sliderForwardLength",
            (x) => { gm.game.lengthForward = x; }, () => { return gm.game.lengthForward; }, 0f, 1.0f, 0.01f);
        RectTransform rectForwardLength = lengthForward.rectTransform;
        rectForwardLength.anchoredPosition = new Vector2(300, -60);
        rectForwardLength.pivot = new Vector2(0f, 0.5f); // ピボット（ボタン自身の基準点）
        rectForwardLength.localScale = new Vector3(1f, 1f, 1f); // テキストのサイズを変更する

        DynamicSlider lengthBackward = new(connection, "sliderBackwardLength",
            (x) => { gm.game.lengthBackward = -1 * x; }, () => { return -1 * gm.game.lengthBackward; }, 0f, 1.0f, 0.01f);
        RectTransform rectBackwardLength = lengthBackward.rectTransform;
        rectBackwardLength.anchoredPosition = new Vector2(300, -100);
        rectBackwardLength.pivot = new Vector2(0f, 0.5f); // ピボット（ボタン自身の基準点）
        rectBackwardLength.localScale = new Vector3(1f, 1f, 1f); // テキストのサイズを変更する
    }
}
