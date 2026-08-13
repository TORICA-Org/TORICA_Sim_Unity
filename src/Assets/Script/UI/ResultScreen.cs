using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

using UnityEngine.Events; // UnityActionのために必要
using UnityEngine.SceneManagement; // `LoadScene`のために必要
using System; // Math

public class ResultScreen
{
    private GameManager gm = GameManager.instance;
    private UIManager ui = UIManager.instance;

    private GameObject basePanel;
    private Rigidbody PlaneRigidbody;

    public static string terminationReason;

    public ResultScreen(GameObject basePanel)
    {
        this.basePanel = basePanel;
        this.PlaneRigidbody = gm.game.Plane.GetComponent<Rigidbody>();
    }

    // ===== パイロット訓練用の結果表示 =========
    public void ShowResultForPilot()
    {
        UnityAction left = () =>
        {
            ui.screen = UIManager.Screens.ResultFourGraphs; // `ResultFourGraphs`に遷移
        };

        UnityAction right = () => {
            ui.screen = UIManager.Screens.ResultTwoGraphs; // `ResultTwoGraphs`に遷移
        };

        ArrowButton(left, right); // `<`ボタンと`>`ボタンの生成

        // 距離を表示するテキストの生成
        float Distance = gm.game.Distance;
        if (GameManager.instance.game.FlightMode == "BirdmanRally") Distance -= 10f;
        string dist = Distance.ToString("0.000") + " m";
        StaticText<string> textDist = new(basePanel, "TextDist", dist, 150.0f);
        GameObject textObj = textDist.gameObject;
        RectTransform textDistRect = textDist.rectTransform;
        textDistRect.anchorMin = new Vector2(0.05f, 0.7f); // アンカーの最小値
        textDistRect.anchorMax = new Vector2(0.5f, 0.7f); // アンカーの最大値
        textDistRect.pivot = new Vector2(0.5f, 0.5f); // ピボット（ボタン自身の基準点）
        textDistRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 800); // RectTransformのx軸方向のサイズを変更する
        textDistRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 400); // RectTransformのy軸方向のサイズを変更する
        textDistRect.anchoredPosition = new Vector2(0, 0); // アンカーを基準にした座標 (pos_x, pos_y) を設定

        // 終了理由を表示するテキストの生成
        StaticText<string> textTerminationReason = new(basePanel, "TextTerminationReason", terminationReason, 100.0f);
        RectTransform rectTextTerminationReason = textTerminationReason.rectTransform;
        rectTextTerminationReason.anchorMin = new Vector2(0.5f, 0.9f); // アンカーの最小値
        rectTextTerminationReason.anchorMax = new Vector2(0.5f, 0.9f); // アンカーの最大値
        rectTextTerminationReason.pivot = new Vector2(0.5f, 0.5f); // ピボット（ボタン自身の基準点）
        rectTextTerminationReason.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 1200); // RectTransformのx軸方向のサイズを変更する
        rectTextTerminationReason.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 200); // RectTransformのy軸方向のサイズを変更する
        rectTextTerminationReason.anchoredPosition = new Vector2(0, 0); // アンカーを基準にした座標 (pos_x, pos_y) を設定

        UnityAction OnClickReloadScene = () =>
        {
            SceneManager.LoadScene("FlightScene"); // `FlightScene`を再読み込み
        };

        // `Retry(R)`ボタンの生成
        ActionButton buttonRetry = new(basePanel, "ButtonRetry", "Retry(R)", 70.0f, OnClickReloadScene);
        RectTransform buttonRetryRect = buttonRetry.rectTransform;
        buttonRetryRect.anchorMin = new Vector2(0.5f, 0.7f); // アンカーの最小値
        buttonRetryRect.anchorMax = new Vector2(0.95f, 0.7f); // アンカーの最大値
        buttonRetryRect.pivot = new Vector2(0.5f, 0.5f); // ピボット（ボタン自身の基準点）
        buttonRetryRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 400); // RectTransformのx軸方向のサイズを変更する
        buttonRetryRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 100); // RectTransformのy軸方向のサイズを変更する
        buttonRetryRect.anchoredPosition = new Vector2(0, 0); // アンカーを基準にした座標 (pos_x, pos_y) を設定

        // トラブルの結果を表示
        string troubles = "";
        troubles += "風速: ";
        troubles += Config.WindSpeed.ToString("0.0") + "m/s";
        troubles += "    風上: ";
        string DirectionText;
        if (Config.WindDirection >= 0)
        {
          DirectionText = "R ";
        }
        else
        {
          DirectionText = "L ";
        }
        DirectionText += Mathf.Abs(Config.WindDirection).ToString("0");
        troubles += DirectionText + "deg" + System.Environment.NewLine;

        troubles += "トラブル:\n";
        float value = (float)Math.Round(GameManager.instance.game.RudderErrorValue,2,MidpointRounding.AwayFromZero);

        switch(GameManager.instance.game.RudderErrorMode){
            case 1:
                troubles += "ラダー"+value+"に固定,\n";
                break;
            case 2:
                troubles += "ラダー"+value+"に確率で固定,\n";
                break;
            case 3:
                troubles += "ラダー"+value+"がニュートラル,\n";
                break;
        }

        value = (float)Math.Round(GameManager.instance.game.CenterOfMassErrorValue,2,MidpointRounding.AwayFromZero);
        if(GameManager.instance.game.CenterOfMassErrorValue != 0){
            troubles += "重心"+value+"ズレ\n";
        }

        troubles += "倍率変化:";

        value = (float)Math.Round(GameManager.instance.game.CenterOfMassRandValue,2,MidpointRounding.AwayFromZero);
        if(GameManager.instance.game.CenterOfMassRandValue != 1){
            troubles += "重心×"+value+",";
        }

        value = (float)Math.Round(GameManager.instance.game.GustRandValue,2,MidpointRounding.AwayFromZero);
        if(GameManager.instance.game.GustRandValue != 0){
            troubles += "風+"+value+",";
        }

        value = (float)Math.Round(GameManager.instance.game.RudderRandValue,2,MidpointRounding.AwayFromZero);
        if(GameManager.instance.game.RudderRandValue != 1){
            troubles += "ラダー×"+value;
        }

        value = (float)Math.Round(GameManager.instance.game.CgeRandValue,2,MidpointRounding.AwayFromZero);
        if(GameManager.instance.game.CgeRandValue != 1){
            troubles += "地面効果×"+value;
        }

        StaticText<string> textTrouble = new(basePanel, "TextTrouble", troubles, 60.0f);
        RectTransform rectTextTrouble = textTrouble.rectTransform;
        rectTextTrouble.anchorMin = new Vector2(0.1f, 0.1f); // アンカーの最小値
        rectTextTrouble.anchorMax = new Vector2(0.9f, 0.5f); // アンカーの最大値
        rectTextTrouble.pivot = new Vector2(0.5f, 0.5f); // ピボット（ボタン自身の基準点）
        rectTextTrouble.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 1500); // RectTransformのx軸方向のサイズを変更する
        rectTextTrouble.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 500); // RectTransformのy軸方向のサイズを変更する
        rectTextTrouble.anchoredPosition = new Vector2(0, 0); // アンカーを基準にした座標 (pos_x, pos_y) を設定


    }

    public void ShowResultTwoGraphs() // 距離と`Retry`ボタン，グラフを2つ表示
    {
        // ボタンに登録するデリゲートをラムダ式で定義
        UnityAction left = () =>
        {
            ui.screen = UIManager.Screens.ResultForPilot; // `ResultFourGraphs`に遷移
        };

        UnityAction right = () =>
        {
            ui.screen = UIManager.Screens.ResultFourGraphs; // `ResultFourGraphs`に遷移
        };

        ArrowButton(left, right); // `<`ボタンと`>`ボタンの生成

        // 左下のグラフの生成
        Chart airdataChart1 = new(basePanel, "ChartAirspeed", "機速", gm.game.AirspeedList);
        RectTransform AirdataChart1Rect = airdataChart1.rectTransform;
        AirdataChart1Rect.anchorMin = new Vector2(0.05f, 0.1f); // アンカーの最小値
        AirdataChart1Rect.anchorMax = new Vector2(0.49f, 0.9f); // アンカーの最大値
        AirdataChart1Rect.pivot = new Vector2(0.5f, 0.5f); // ピボット（ボタン自身の基準点）
        AirdataChart1Rect.anchoredPosition = new Vector2(0, 0); // アンカーを基準にした座標 (pos_x, pos_y) を設定

        // 右下のグラフの生成
        Chart airdataChart2 = new(basePanel, "ChartAlt", "高度", gm.game.AltList);
        RectTransform AirdataChart2Rect = airdataChart2.rectTransform;
        AirdataChart2Rect.anchorMin = new Vector2(0.51f, 0.1f); // アンカーの最小値
        AirdataChart2Rect.anchorMax = new Vector2(0.95f, 0.9f); // アンカーの最大値
        AirdataChart2Rect.pivot = new Vector2(0.5f, 0.5f); // ピボット（ボタン自身の基準点）
        AirdataChart2Rect.anchoredPosition = new Vector2(0, 0); // アンカーを基準にした座標 (pos_x, pos_y) を設定
    }

    public void ShowResultFourGraphs() // グラフを4つ表示
    {
        // ボタンに登録するデリゲートをラムダ式で定義
        UnityAction left = () =>
        {
            ui.screen = UIManager.Screens.ResultTwoGraphs; // `ResultFourGraphs`に遷移
        };

        UnityAction right = () =>
        {
            ui.screen = UIManager.Screens.ResultForPilot; // `ResultFourGraphs`に遷移
        };

        ArrowButton(left, right); // `<`ボタンと`>`ボタンの生成

        // 左上のグラフの生成
        Chart airdataChart1 = new(basePanel, "ChartPitchAlpha", "ピッチ(theta)", gm.game.ThetaList, "迎角(alpha)", gm.game.AlphaList);
        GameObject chartObj = airdataChart1.gameObject;
        RectTransform AirdataChart1Rect = airdataChart1.rectTransform;
        AirdataChart1Rect.anchorMin = new Vector2(0.05f, 0.51f); // アンカーの最小値
        AirdataChart1Rect.anchorMax = new Vector2(0.49f, 0.99f); // アンカーの最大値
        AirdataChart1Rect.pivot = new Vector2(0.5f, 0.5f); // ピボット（ボタン自身の基準点）
        AirdataChart1Rect.anchoredPosition = new Vector2(0, 0); // アンカーを基準にした座標 (pos_x, pos_y) を設定

        // 右上のグラフの生成
        Chart airdataChart2 = new(basePanel, "ChartRollBeta", "ロール(phi)", gm.game.PhiList, "横滑り角(beta)", gm.game.BetaList);
        RectTransform AirdataChart2Rect = airdataChart2.rectTransform;
        AirdataChart2Rect.anchorMin = new Vector2(0.51f, 0.51f); // アンカーの最小値
        AirdataChart2Rect.anchorMax = new Vector2(0.95f, 0.99f); // アンカーの最大値
        AirdataChart2Rect.pivot = new Vector2(0.5f, 0.5f); // ピボット（ボタン自身の基準点）
        AirdataChart2Rect.anchoredPosition = new Vector2(0, 0); // アンカーを基準にした座標 (pos_x, pos_y) を設定

        //左下のグラフの生成
        Chart airdataChart3 = new(basePanel, "ChartCenterOfG", "全備重心", gm.game.CenterOfGList);
        RectTransform AirdataChart3Rect = airdataChart3.rectTransform;
        AirdataChart3Rect.anchorMin = new Vector2(0.05f, 0.01f); // アンカーの最小値
        AirdataChart3Rect.anchorMax = new Vector2(0.49f, 0.49f); // アンカーの最大値
        AirdataChart3Rect.pivot = new Vector2(0.5f, 0.5f); // ピボット（ボタン自身の基準点）
        AirdataChart3Rect.anchoredPosition = new Vector2(0, 0); // アンカーを基準にした座標 (pos_x, pos_y) を設定

        // 右下のグラフの生成
        Chart airdataChart4 = new(basePanel, "ChartRudder", "ラダー", gm.game.drList);
        RectTransform AirdataChart4Rect = airdataChart4.rectTransform;
        AirdataChart4Rect.anchorMin = new Vector2(0.51f, 0.01f); // アンカーの最小値
        AirdataChart4Rect.anchorMax = new Vector2(0.95f, 0.49f); // アンカーの最大値
        AirdataChart4Rect.pivot = new Vector2(0.5f, 0.5f); // ピボット（ボタン自身の基準点）
        AirdataChart4Rect.anchoredPosition = new Vector2(0, 0); // アンカーを基準にした座標 (pos_x, pos_y) を設定
    }

    private void ArrowButton(UnityAction left, UnityAction right)
    {
        // `<`ボタンの生成
        ActionButton buttonChangePageLeft = new(basePanel, "ButtonChangePageLeft", "<", 50.0f, left);
        RectTransform buttonChangePageLeftRect = buttonChangePageLeft.rectTransform;
        buttonChangePageLeftRect.anchorMin = new Vector2(0f, 0.5f); // アンカーの最小値
        buttonChangePageLeftRect.anchorMax = new Vector2(0f, 0.5f); // アンカーの最大値
        buttonChangePageLeftRect.pivot = new Vector2(0f, 0.5f); // ピボット（ボタン自身の基準点）
        buttonChangePageLeftRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 50); // RectTransformのx軸方向のサイズを変更する
        buttonChangePageLeftRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 150); // RectTransformのy軸方向のサイズを変更する
        buttonChangePageLeftRect.anchoredPosition = new Vector2(20, 0); // アンカーを基準にした座標 (pos_x, pos_y) を設定

        // `>`ボタンの生成
        ActionButton buttonChangePageRight = new(basePanel, "ButtonChangePageRight", ">", 50.0f, right);
        GameObject buttonObj = buttonChangePageRight.gameObject;
        RectTransform buttonChangePageRightRect = buttonChangePageRight.rectTransform;
        buttonChangePageRightRect.anchorMin = new Vector2(1f, 0.5f); // アンカーの最小値
        buttonChangePageRightRect.anchorMax = new Vector2(1f, 0.5f); // アンカーの最大値
        buttonChangePageRightRect.pivot = new Vector2(1f, 0.5f); // ピボット（ボタン自身の基準点）
        buttonChangePageRightRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 50); // RectTransformのx軸方向のサイズを変更する
        buttonChangePageRightRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 150); // RectTransformのy軸方向のサイズを変更する
        buttonChangePageRightRect.anchoredPosition = new Vector2(-20, 0); // アンカーを基準にした座標 (pos_x, pos_y) を設定
    }
}

