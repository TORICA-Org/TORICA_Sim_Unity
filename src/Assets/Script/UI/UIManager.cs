using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ===== 以下，共存できない =====
using UnityEngine.UI; // uGUI

// using UnityEngine.UIElements; // UI Toolkit
// ===== ================ =====
using TMPro;
using UnityEngine.Events;　//UnityAction使うにはこれ忘れないように
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [TextArea(5, 15)]
    [Tooltip("備忘録や仕様のメモなどを自由に書き込めます")]
    public string note = "BasePanelにボタンやXChartをInstantiateして，UIを組み立てる方式です．" +
    "ページが切り替わっている用に見えて，同じPanel上で組み立て直しています．" +
    "`UIClasses`にUI作成用のヘルパークラスがあります．";

    public static UIManager instance = null; // シングルトンパターンのインスタンス.

    private GameManager gm;
    private GameObject ui;
    private Canvas canvas;

    private GameObject basePanel;
    private GameObject baseScrollView;

    private PreFlightScreen preFlight;
    private FlightSettingScreen flightSetting;
    private ResultScreen result;

    // ===== 画面管理 ===========================================
    private bool isPause = false; // 一時停止状態を示すフラグ. ぜひ実装してほしい.

    public enum Screens
    {
        None, // 非表示.
        InFlight, // フライト中.

        // ===== ResultScreenのページ. =====
        ResultForPilot, // 結果（パイロット用）
        ResultTwoGraphs, // 結果（グラフ2つ）.
        ResultFourGraphs, // 結果（グラフ4つ）.

        // ===== PreFlightScreenのページ. =====
        PreFlightTest, // フライト前の設定画面.
        PreFlightVRSettings, // VRの設定画面.
        FlightSetting // 以前のUIに対して描画するモード
    }

    public Screens screen = Screens.None;
    private Screens previousScreen = Screens.None; // 前回の画面状態を保存する変数.
    // ==========================================================

    private void Start()
    {
        gm = GameManager.instance; // `GameManager`のインスタンスを取得して代入.
        ui = this.gameObject; // このゲームオブジェクト，つまり`UIManager`を`ui`に代入.

        basePanel = GameObject.Find("BasePanel"); // `BasePanel`という名前のゲームオブジェクトを探して代入.
        baseScrollView = GameObject.Find("BaseScrollView"); // `BaseScrollView`という名前のゲームオブジェクトを探して代入. これは遊びで作った.
        // 設定項目が多いので，スクロールしてカテゴリーを超えて見れるようになったら嬉しい.
        // 左端の列にカテゴリーを列挙し，スクロール位置でカテゴリーの文字をハイライトするのが理想.

        basePanel.SetActive(false); // `BasePanel`を非アクティブにする.
        baseScrollView.SetActive(false); // `BaseScrollView`を非アクティブにする.

        canvas = ui.GetComponent<Canvas>(); // `Canvas`コンポーネントを取得して代入.
        if (canvas == null)
        {
            canvas = ui.AddComponent<Canvas>(); // `Canvas`コンポーネントがない場合は追加.
        }
        if (ui.GetComponent<CanvasScaler>() == null) // `CanvasScaler`コンポーネントがない場合は追加.
        {
            CanvasScaler scaler = ui.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
        }

        // VRのInputModuleを回避する目的.
        //canvas.renderMode = RenderMode.ScreenSpaceCamera; // `Canvas`の`renderMode`を`ScreenSpaceCamera`に設定.

        canvas.enabled = false; // 最初は画面を非表示にしておく.

        preFlight = new(basePanel, baseScrollView); // `PreFlightScreen`をインスタンス化.
        result = new(basePanel); // `ResultScreen`をインスタンス化.
        flightSetting = new();

        screen = Screens.PreFlightTest;
    }

    private void Update()
    {
        if (gm.game.status == GameParameters.Status.Preparation)
        {
            //screen = Screens.None;
            screen = Screens.FlightSetting;
            basePanel.SetActive(false); // `BasePanel`を非アクティブにする.
            baseScrollView.SetActive(false); // `BaseScrollView`を非アクティブにする.
            canvas.enabled = false;

            // テストコード.
            /*
            screen = Screens.PreFlightTest; // 上から重ねて表示してるだけ. 通常使用するためには`Screens.None`.
            basePanel.SetActive(true); // `Test`が必要なければ`false`.
            baseScrollView.SetActive(true); // `Test`が必要なければ`false`.
            canvas.enabled = true; // `Test`が必要なければ`false`.
            */
            /*
            screen = Screens.PreFlightVRSettings;
            basePanel.SetActive(false);
            baseScrollView.SetActive(false);
            canvas.enabled = true;
            */
        }
        else if (gm.game.status == GameParameters.Status.Flight) // フライト中.
        {
            screen = Screens.InFlight;
            basePanel.SetActive(false); // `BasePanel`を非アクティブにする.
            baseScrollView.SetActive(false); // `BaseScrollView`を非アクティブにする.
            canvas.enabled = false;
        }
        else if (gm.game.status == GameParameters.Status.Splashdown) // 着水.
        {
            canvas.enabled = true;
            basePanel.SetActive(true); // `BasePanel`をアクティブにする.
            baseScrollView.SetActive(false); // `BaseScrollView`を非アクティブにする.
            if (screen != Screens.ResultForPilot && screen != Screens.ResultFourGraphs && screen != Screens.ResultTwoGraphs)
            {
                screen = Screens.ResultForPilot;
            }
        }

        RefleshScreen(); // 画面を更新.
    }

    // ===== RenderCameraを設定する関数 ===========================================
    public void SetRenderCamera(Camera cam)
    {
        canvas.worldCamera = cam; // `Canvas`の`worldCamera`を引数のカメラに設定.
    }

    // ===== 画面を更新する関数 ===========================================
    public void RefleshScreen()
    {
        if (screen != previousScreen)
        {
            previousScreen = screen; // 現在の画面状態を保存.

            UIBase.DisposeAll(); // `UIBase`の全てのインスタンスを破棄.

            switch (screen)
            {
                case Screens.InFlight:
                    break;

                case Screens.ResultForPilot:
                    result.ShowResultForPilot();
                    break;

                case Screens.ResultTwoGraphs:
                    result.ShowResultTwoGraphs();
                    break;

                case Screens.ResultFourGraphs:
                    result.ShowResultFourGraphs();
                    break;

                case Screens.PreFlightTest:
                    preFlight.Test();
                    break;

                case Screens.PreFlightVRSettings:
                    preFlight.VRSettings();
                    break;

                case Screens.FlightSetting:
                    flightSetting.Generate();
                    break;

                default:
                    break;
            } // switch (screen)
        } // if (isScreenChanged)
    } // RefleshScreen()

    private void Awake()
    {
        if (instance == null) // シングルトンのインスタンスが存在しない場合は、現在のインスタンスを割り当てる.
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject); // このゲームオブジェクトをシーンが切り替わっても破棄されないようにする.
        }
        else
        {
            Destroy(this.gameObject); // シングルトンのインスタンスが既に存在する場合は、現在のゲームオブジェクトを破棄する.
        }
    }
}
