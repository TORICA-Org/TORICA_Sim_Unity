using System.Collections;
using System.Collections.Generic; // Listを使うために必要
using System.IO;
using UnityEngine;
using XCharts.Runtime; // XChartを使うために必要

public class XChartPrinter : MonoBehaviour
{
    protected GameManager gm;
    protected GameObject systemcontroller;
    protected AirData airdata;

    private Camera chartCamera; // 撮影用カメラ
    private LineChart chart;    // XChartのグラフ本体
    private Vector2Int imageSize = new Vector2Int(1920, 1440); // 保存サイズ

    private XAxis xAxis;
    private YAxis yAxis;

    private Serie serie1;
    private Serie serie2;

    private const int pickedDataNum = 100; // プロットするデータの数（数に根拠はない）
    private int[] pickedIndex = new int[pickedDataNum]; // ピックするデータのインデックスを保持する配列


    private void Start()
    {
        gm = GameManager.instance; // `GameManager.instance`を`gm`として保持
        systemcontroller = GameObject.Find("SystemController"); // `SystemController`への参照を取得
        airdata = systemcontroller.GetComponent<AirData>(); // `AirData`コンポーネントへの参照を取得

        chartCamera = GameObject.Find("ChartCamera").GetComponent<Camera>(); // `ChartCamera`の`Camera`コンポーネントへの参照を取得
        chart = GameObject.Find("ChartForPrinter").GetComponent<LineChart>(); // `ChartForPrinter`の`LineChart`コンポーネントへの参照を取得
        if (chart == null) // `LineChart`コンポーネントが取得できなかったら
        {
            chart = GameObject.Find("ChartForPrinter").AddComponent<LineChart>(); // `LineChart`コンポーネントを追加
            chart.Init(); // グラフの初期化
        }

        chart.GetChartComponent<Title>().show = false; // タイトルの表示
        chart.EnsureChartComponent<Tooltip>().show = false; // グラフ上にカーソルを当てた際の値の表示
        chart.EnsureChartComponent<Legend>().show = true; // 凡例の表示

        xAxis = chart.EnsureChartComponent<XAxis>(); // X軸（横軸：時間）を取得
        xAxis.splitNumber = 7; // 表示する目盛りの数を7に設定
        xAxis.axisLabel.numericFormatter = "f0";

        yAxis = chart.EnsureChartComponent<YAxis>(); // Y軸（縦軸）を取得
    }


    public void ExportAllGraphs() // 外部から呼び出されるpublicな関数
    {
        PickIndex((int)airdata.frameNumber);
        StartCoroutine(ExportProcess()); // コルーチンの開始
        // コルーチンとは
        // 時間経過を伴う処理を複数フレームにわたって実行する仕組み
        // 開始：`StartCoroutine();`
        // 定義：IEnumerator型を返す関数により定義する．yield return文が少なくとも1つ含まれるようにする．
    }


    private void PickIndex(int dataCount) // ピックするpickedDataNum個のインデックス（それ以下ならそのまま）の配列を返す．
    {
        if (dataCount <= pickedDataNum - 1) // データ数がpickedDataNum個以下
        {
            for (int i = 0; i < dataCount; i++)
            {
                pickedIndex[i] = i; // インデックスと各要素を一致させる
            }
            // pickedDataNum = dataCount; // pickedDataNumを更新
        }
        else // データ数がpickedDataNumより大きい
        {
            float indexInterval = (float)dataCount / (float)(pickedDataNum - 1); // pickedDataNum個に分割したときの間隔（小数点以下切り捨て）

            for (int i = 0; i < pickedDataNum - 1; i++)
            {
                if (indexInterval * (float)i > dataCount) // 最終要素が最大値を超えた場合
                {
                    pickedIndex[i] = dataCount; // 最終要素に最大値を代入
                }
                else
                {
                    pickedIndex[i] = (int)Mathf.Round(indexInterval * (float)i); // indexInterval倍の値を次々に代入
                }
                print(pickedIndex[i]);
            }
            print("dataCount: " + dataCount);
        }
    }


    IEnumerator ExportProcess() // グラフ出力処理のコルーチンの定義
    {
        SaveChartThetaAlpha();
        yield return new WaitForSecondsRealtime(1); // 1秒待機（RealtimeなのでTime.timeScaleの影響を受けず，物理演算停止中でも処理が進む．）
        CaptureProcess("ThetaAlpha.png");
        yield return new WaitForSecondsRealtime(1);
        SaveChartPhiBeta();
        yield return new WaitForSecondsRealtime(1);
        CaptureProcess("PhiBeta.png");
        yield return new WaitForSecondsRealtime(1);
        SaveChartAirspeedAlt();
        yield return new WaitForSecondsRealtime(1);
        CaptureProcess("AirspeedAlt.png");
        yield return new WaitForSecondsRealtime(1);
        SaveChartCenterOfG();
        yield return new WaitForSecondsRealtime(1);
        CaptureProcess("CenterOfG.png");
        yield return new WaitForSecondsRealtime(1);
        SaveChartRudder();
        yield return new WaitForSecondsRealtime(1);
        CaptureProcess("Rudder.png");
        yield return new WaitForSecondsRealtime(1);
        StartAtDirPath();
    }


    private void SaveChartThetaAlpha()
    {
        chart.RemoveData(); // データを除去

        serie1 = chart.AddSerie<Line>("ピッチ(Theta)"); // Line型のSerieを追加
        serie2 = chart.AddSerie<Line>("迎角(Alpha)");

        foreach (var serie in chart.series) // Serie型配列要素それぞれに対して
        {
            serie.AnimationEnable(false); // アニメーション無効
            serie.symbol.show = false; // プロット点非表示
        }

        for (int i = 0; i < pickedDataNum; i++)
        {
            chart.AddXAxisData((0.02 * pickedIndex[i]) + "s"); // ピックしたインデックスは各時点でのフレーム数であるため
            chart.AddData(0, gm.game.ThetaList[pickedIndex[i]]); // 系列0(serie1)に追加
            chart.AddData(1, gm.game.AlphaList[pickedIndex[i]]); // 系列1(serie2)に追加
        }

        chart.RefreshChart(); // グラフを更新
    }


    private void SaveChartPhiBeta()
    {
        chart.RemoveData();

        serie1 = chart.AddSerie<Line>("ロール(Phi)");
        serie2 = chart.AddSerie<Line>("横滑り角(Beta)");

        foreach (var serie in chart.series)
        {
            serie.AnimationEnable(false);
            serie.symbol.show = false;
        }

        for (int i = 0; i < pickedDataNum; i++)
        {
            chart.AddXAxisData((0.02 * pickedIndex[i]) + "s");
            chart.AddData(0, gm.game.PhiList[pickedIndex[i]]);
            chart.AddData(1, gm.game.BetaList[pickedIndex[i]]);
        }

        chart.RefreshChart();
    }


    private void SaveChartAirspeedAlt()
    {
        chart.RemoveData();

        serie1 = chart.AddSerie<Line>("対気速度(Airspeed)");
        serie2 = chart.AddSerie<Line>("高度(Alt)");

        foreach (var serie in chart.series)
        {
            serie.AnimationEnable(false);
            serie.symbol.show = false;
        }

        for (int i = 0; i < pickedDataNum; i++)
        {
            chart.AddXAxisData((0.02 * pickedIndex[i]) + "s");
            chart.AddData(0, gm.game.AirspeedList[pickedIndex[i]]);
            chart.AddData(1, gm.game.AltList[pickedIndex[i]]);
        }

        chart.RefreshChart();
    }


    private void SaveChartCenterOfG()
    {
        chart.RemoveData();

        serie1 = chart.AddSerie<Line>("全備重心(CenterOfG)");

        foreach (var serie in chart.series)
        {
            serie.AnimationEnable(false);
            serie.symbol.show = false;
        }

        yAxis.axisLabel.numericFormatter = "f2";

        for (int i = 0; i < pickedDataNum; i++)
        {
            chart.AddXAxisData((0.02 * pickedIndex[i]) + "s");
            chart.AddData(0, gm.game.CenterOfGList[pickedIndex[i]]);
        }

        chart.RefreshChart();
    }


    private void SaveChartRudder()
    {
        chart.RemoveData();

        serie1 = chart.AddSerie<Line>("ラダー角(dr)");

        foreach (var serie in chart.series)
        {
            serie.AnimationEnable(false);
            serie.symbol.show = false;
        }

        for (int i = 0; i < pickedDataNum; i++)
        {
            chart.AddXAxisData((0.02 * pickedIndex[i]) + "s");
            chart.AddData(0, gm.game.drList[pickedIndex[i]]);
        }

        chart.RefreshChart();
    }


    private void CaptureProcess(string fileName) // PNG画像をカメラを使って撮影する関数（Gemini制作）
    {
        // RenderTextureを作成 (使い捨て)
        RenderTexture rt = new RenderTexture(imageSize.x, imageSize.y, 24);
        chartCamera.targetTexture = rt;

        // カメラで撮影（レンダリング）
        chartCamera.Render();

        // RenderTextureからTexture2Dにピクセルを移す
        RenderTexture.active = rt; // ReadPixelsの読み取り元をrtに切り替え
        Texture2D texture = new Texture2D(imageSize.x, imageSize.y, TextureFormat.RGB24, false);
        texture.ReadPixels(new Rect(0, 0, imageSize.x, imageSize.y), 0, 0);
        texture.Apply();

        // 後始末
        chartCamera.targetTexture = null;
        RenderTexture.active = null;
        Destroy(rt);

        // 保存
        SaveTexture(texture, fileName);

        Destroy(texture); // メモリ解放
    }


    private void SaveTexture(Texture2D tex, string fileName) // 保存場所を
    {
        string dirPath = GetGraphsDirPath();
        string fullPath = Path.Combine(dirPath, fileName);
        File.WriteAllBytes(fullPath, tex.EncodeToPNG());
    }

    private void StartAtDirPath()
    {
        string dirPath = GetGraphsDirPath();
        // Debug.Log("XChartグラフの場所を開く: " + dirPath);
        System.Diagnostics.Process.Start(@dirPath);
    }

    private string GetGraphsDirPath()
    {
        // exeと同じ階層/Graphs フォルダのパスを生成
        string dirPath = Path.Combine(Directory.GetParent(Application.dataPath).FullName, "Graphs");
        if (!Directory.Exists(dirPath)) Directory.CreateDirectory(dirPath);
        return dirPath;
    }
}
