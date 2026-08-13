/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ===== 以下，共存できない =====
using UnityEngine.UI; // uGUI
// using UnityEngine.UIElements; // UI Toolkit
// ===== ================ =====
using TMPro;
using UnityEngine.Events;　//UnityAction使うにはこれ忘れないように
using XCharts.Runtime; // XChartを使うために必要

using System; // `Action`を使用するために必要
using System.Threading; // 並列処理のために必要
using System.Threading.Tasks; // `Task`型を使用するために必要


public class UIHelper : MonoBehaviour
{

*/

    /*
    // ===== 初期設定 =====
    protected GameManager gm;
    protected Rigidbody PlaneRigidbody;
    protected GameObject basePanel;

    public delegate float Getter();
    private delegate void ValueChangeHander();
    private ValueChangeHander valueChangeHander;

    public void InitUIHelper()
    {
        gm = GameManager.instance;
        PlaneRigidbody = gm.game.Plane.GetComponent<Rigidbody>();
        basePanel = GameObject.Find("BasePanel");
    }
    */


    /*
    // ===== ボタンの生成 ==========================
    public static GameObject NewButtonObj(GameObject parent, string objectName, string displayContent, float fontSize, UnityAction callback)
    {
        GameObject DefaultButton = (GameObject)Resources.Load("UIParts/DefaultButton");

        // 生成時に直接親キャンバスを指定
        GameObject btnObj = Instantiate(DefaultButton, parent.transform, false);

        // その他の設定
        btnObj.name = objectName;
        TextMeshProUGUI btnTmp = btnObj.transform.Find("Text").gameObject.GetComponent<TextMeshProUGUI>();
        btnTmp.text = displayContent;
        btnTmp.enableAutoSizing = false;
        btnTmp.fontSize = fontSize;

        // クリックイベントを付与
        // コールバックはデリゲート(Unity Action)により取得
        btnObj.GetComponent<Button>().onClick.AddListener(callback);

        return btnObj;
    }
    */


    /*
    // ===== スライダーの生成 ==========================
    public static GameObject NewSliderObj(GameObject parent, string objectName, float minVal, float maxVal, float initVal)
    {

        GameObject DefaultSlider = (GameObject)Resources.Load("UIParts/DefaultSlider");

        // 生成時に直接親キャンバスを指定
        GameObject sliderObj = Instantiate(DefaultSlider, parent.transform, false);

        // その他の設定
        sliderObj.name = objectName;

        Slider slider = sliderObj.GetComponent<Slider>();
        slider.minValue = minVal;
        slider.maxValue = maxVal;
        slider.value = initVal;

        return sliderObj; // GameObjectを返す
    }
    */


    /*
    // ===== テキストの生成 ==========================
    public static GameObject NewTextObj(GameObject parent, string objectName, float fontSize)
    {
        GameObject DefaultText = (GameObject)Resources.Load("UIParts/DefaultText");

        // 生成時に直接親キャンバスを指定
        GameObject textObj = Instantiate(DefaultText, parent.transform, false);

        // その他の設定
        textObj.name = objectName;
        TextMeshProUGUI textTmp = textObj.GetComponent<TextMeshProUGUI>();
        textTmp.enableAutoSizing = false;
        textTmp.fontSize = fontSize;
        textTmp.alignment = TextAlignmentOptions.Center; // 中央寄せ

        return textObj;
    }
    */


    /*
    // ===== グラフの生成 ==========================
    public static GameObject NewChartObj(GameObject parent, string objectName, 
        string name1, List<float> list1, 
        string name2 = null, List<float> list2 = null)
    {
        GameObject AirdataChart = (GameObject)Resources.Load("UIParts/AirdataChart");

        CanvasGroup cg = AirdataChart.GetComponent<CanvasGroup>();
        if(!cg)
        {
            cg = AirdataChart.AddComponent<CanvasGroup>();
        }
        cg.alpha = 0; // 透明度を0（透明）に.

        // 生成時に直接親キャンバスを指定.
        GameObject chartObj = Instantiate(AirdataChart, parent.transform, false);

        chartObj.name = objectName;

        SetChartData(chartObj, name1, list1, name2, list2);

        return chartObj;
    }

    private static async void SetChartData(GameObject chartObj, string name1, List<float> list1, string name2, List<float> list2) // グラフの描写処理は非同期的に行う（安定化のため）
    {
        await Task.Yield(); // 1フレーム待機

        if (!chartObj) return;

        using (XChartManager cm = new XChartManager(chartObj.transform.Find("LineChart").GetComponent<LineChart>())) // インスタンスはこの`using`ブロック内でのみ有効
        {
            cm.CreateChart(name1, list1, name2, list2);
        }; // `using`ブロックを出ると，インスタンスは安全に破棄される

        CanvasGroup cg = chartObj.GetComponent<CanvasGroup>();

        while(cg.alpha < 1 && cg != null)
        {
            await Task.Delay(10); // 0.01秒待機
            if(!cg)
            {
                break;
            }
            cg.alpha += 0.05f; // フェードイン（サイズ設定が遅れ，画面が白くフラッシュする対策として実装．かなりそれっぽい．）.
        }
    }

    */


    /*
    // ===== その他のユーティリティ関数 ==========================
    public static void DestroyAllChildren(GameObject parentObj) // 全ての子オブジェクトを破棄 -> Disposeによる破棄に変更.
    {
        foreach (Transform child in parentObj.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
    }
    */


// }
