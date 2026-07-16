using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class Config
{
    // ===== setプロパティで外部のスクリプトからの変更を検知するための共通の処理メソッド. ===========================
    private static bool SetProperty<T>(ref T backingField, T value)
    {
        // 値が同じなら false を返して終了
        if (EqualityComparer<T>.Default.Equals(backingField, value))
            return false;

        // 値を更新
        backingField = value;

        // クラス内の共通処理メソッドを呼ぶ
        Flush();

        // 値が更新されたら true を返す
        return true;
    }

    // ---------------------------------------------------------------------
    // 1変数につき，以下の3つを定義する:
    //
    // 1. 内部変数（例: `private static string aircraftModelName`）
    // 2. デフォルト値（例: `private static readonly string defaultAircraftModelName`）
    // 3. アクセサ（例: `public static string AircraftModelName`）
    //
    // さらに、以下のような内容を実装する:
    // - 変数の内容を`Config.txt`に書き込むための処理（`Flush()`）
    // - 変数の内容を`Config.txt`から読み込むための処理（`Load()`）
    // - （必要なら）値の妥当性を検査して、修正するためのメソッド（`Validate()`）
    // ---------------------------------------------------------------------

    // 設定値の内部変数とアクセサ.  

    private static readonly string defaultAircraftModelName = "Tatsumi";
    private static string aircraftModelName = defaultAircraftModelName;
    public static string AircraftModelName
    {
        get => aircraftModelName;
        set => SetProperty(ref aircraftModelName, value);
    }

    private static readonly string defaultAircraftDataName = "ARG-2";
    private static string aircraftDataName = defaultAircraftDataName;
    public static string AircraftDataName
    {
        get => aircraftDataName;
        set => SetProperty(ref aircraftDataName, value);
    }
    
    private static readonly bool defaultShowHUD = true;
    private static bool showHUD = defaultShowHUD;
    public static bool ShowHUD
    {
        get => showHUD;
        set => SetProperty(ref showHUD, value);
    }

    private static readonly bool defaultShowHorizontalLine = false;
    private static bool showHorizontalLine = defaultShowHorizontalLine;
    public static bool ShowHorizontalLine
    {
        get => showHorizontalLine;
        set => SetProperty(ref showHorizontalLine, value);
    }

    private static readonly bool defaultMainCamera = true;
    private static bool mainCamera = defaultMainCamera;
    public static bool MainCamera
    {
        get => mainCamera;
        set => SetProperty(ref mainCamera, value);
    }

    private static readonly bool defaultUseMousePitchControl = false;
    private static bool useMousePitchControl = defaultUseMousePitchControl;
    public static bool UseMousePitchControl
    {
        get => useMousePitchControl;
        set => SetProperty(ref useMousePitchControl, value);
    }

    private static readonly float defaultMouseSensitivity = 1.0f;
    private static float mouseSensitivity = defaultMouseSensitivity;
    public static float MouseSensitivity
    {
        get => mouseSensitivity;
        set => SetProperty(ref mouseSensitivity, value);
    }

    private static readonly string defaultSerialPort = "None";
    private static string serialPort = defaultSerialPort;
    public static string SerialPort
    {
        get => serialPort;
        set => SetProperty(ref serialPort, value);
    }

    private static readonly float defaultRudderZero = 7500.0f;
    private static float rudderZero = defaultRudderZero;
    public static float RudderZero
    {
        get => rudderZero;
        set => SetProperty(ref rudderZero, value);
    }

    private static readonly float defaultRudderMax = 7722.2f;
    private static float rudderMax = defaultRudderMax;
    public static float RudderMax
    {
        get => rudderMax;
        set => SetProperty(ref rudderMax, value);
    }

    private static readonly bool defaultRudderReverse = false;
    private static bool rudderReverse = defaultRudderReverse;
    public static bool RudderReverse
    {
        get => rudderReverse;
        set => SetProperty(ref rudderReverse, value);
    }

    private static readonly bool defaultExportLog = false;
    private static bool exportLog = defaultExportLog;
    public static bool ExportLog
    {
        get => exportLog;
        set => SetProperty(ref exportLog, value);
    }
    
    private static readonly float defaultWindMagnitude = 0.0f;
    private static float windMagnitude = defaultWindMagnitude;
    public static float WindMagnitude
    {
        get => windMagnitude;
        set => SetProperty(ref windMagnitude, value);
    }

    private static readonly float defaultWindDirection = 0.0f;
    private static float windDirection = defaultWindDirection;
    public static float WindDirection
    {
        get => windDirection;
        set => SetProperty(ref windDirection, value);
    }

    private static readonly string defaultWindRangeSpec = "None";
    private static string windRangeSpec = defaultWindRangeSpec;
    public static string WindRangeSpec
    {
        get => windRangeSpec;
        set => SetProperty(ref windRangeSpec, value);
    }

    private static readonly float defaultTakeoffSpeed = 6.5f;
    private static float takeoffSpeed = defaultTakeoffSpeed;
    public static float TakeoffSpeed
    {
        get => takeoffSpeed;
        set => SetProperty(ref takeoffSpeed, value);
    }

    private static readonly float defaultTakeoffRoll = 0.0f;
    private static float takeoffRoll = defaultTakeoffRoll;
    public static float TakeoffRoll
    {
        get => takeoffRoll;
        set => SetProperty(ref takeoffRoll, value);
    }

    private static readonly float defaultTakeoffPitch = 0.0f;
    private static float takeoffPitch = defaultTakeoffPitch;
    public static float TakeoffPitch
    {
        get => takeoffPitch;
        set => SetProperty(ref takeoffPitch, value);
    }

    private static readonly float defaultTakeoffYaw = 0.0f;
    private static float takeoffYaw = defaultTakeoffYaw;
    public static float TakeoffYaw
    {
        get => takeoffYaw;
        set => SetProperty(ref takeoffYaw, value);
    }

    private static readonly bool defaultRandomizeWind = false;
    private static bool randomizeWind = defaultRandomizeWind;
    public static bool RandomizeWind
    {
        get => randomizeWind;
        set => SetProperty(ref randomizeWind, value);
    }


    private static void Validate()
    {
        // ----- AircraftName -----------------------------------------------------------------------------
        int indexOfAircraft = AircraftData.Names.FindIndex(name => string.Compare(name, aircraftDataName, ignoreCase: true) == 0);
        if (indexOfAircraft == -1)
        {
            aircraftDataName = defaultAircraftDataName;
        }
        else
        {
            aircraftDataName = AircraftData.Names[indexOfAircraft]; // 大文字小文字を無視してCSVファイルの機体名と一致するものを探し、見つかったものに置き換える.
        }
    }

    // `Config.txt`を保存する`Application.dataPath`へのフルパス.
    private static string _sourceDirectory = Directory.GetParent(Application.dataPath).FullName;

    // `Config.txt`の名前.
    private static string _fileName = "Config.txt";

    // `Config.txt`のフルパスを構築.
    private static string _configFilePath = Path.Combine(_sourceDirectory, _fileName);

    // ファイルシステムの変更を監視する`FileSystemWatcher`.
    private static FileSystemWatcher watcher;

    // 最後に書き込みをおこなった時間を記録する変数（クラスのメンバとして定義）
    private static DateTime _lastFlushTime = DateTime.MinValue;

    // `ConfigIO`クラスのインスタンス.
    private static readonly int recordNum = 100; // インスタンスで扱える行数を指定.
    private static readonly ConfigIO config = new(recordNum);

    private static SynchronizationContext context = SynchronizationContext.Current;

    // ===== ゲームのシーンがロードされる前に一度だけ呼び出される初期化メソッド. =========================
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)] // ゲームのシーンがロードされる前にこのメソッドを呼び出すための属性.
    private static void Initialize()
    {
        Sync();

        // `FileSystemWatcher`を初期化して、`Config.txt`の変更を監視.
        watcher = new FileSystemWatcher(_sourceDirectory, _fileName);
        // ファイルが変更されたときのイベントハンドラーを登録.
        watcher.Changed += (sender, e) =>
        {
            Debug.Log("Config change detected.");
            // 前回の同期から 1000ミリ秒（1秒）以内の連続呼び出しは無視する
            if ((DateTime.Now - _lastFlushTime).TotalMilliseconds < 1000) {
                return;
            }
            Sync(); // 同期を実行
        };

        watcher.Created += (sender, e) => Sync();
        watcher.Deleted += (sender, e) => Sync();
        watcher.Renamed += (sender, e) => Sync();
        watcher.Error += (sender, e) =>
            Debug.LogError($"FileSystemWatcher error: {e.GetException()}");
        // 監視を開始.
        watcher.EnableRaisingEvents = true;
        Debug.Log("ConfigManager initialized and watching for changes in Config.txt.");
    }

    // ===== 設定を`Config.txt`と同期する ===================================================
    public static void Sync()
    {
        Load();
        Flush();

        // メインスレッドの文脈に処理を非同期的に戻す.
        context.Post(
            (_) =>
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name); // シーンを再ロード.
            },
            null
        );
        Debug.Log("Config synced.");
    }

    // ===== 設定を`Config.txt`から読み込む ================================================
    private static void Load()
    {
        if (File.Exists(_configFilePath)) // `Config.txt`が存在する場合.
        {
            try
            {
                config.Load(_configFilePath); // 変更されたファイルの内容を再読み込み.
                Debug.Log("Config file reloaded. Check start.");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to reload config file: {ex.Message}");
            }

            aircraftModelName = CheckContent("AircraftModelName", defaultAircraftModelName);
            aircraftDataName = CheckContent("AircraftDataName", defaultAircraftDataName);
            showHUD = CheckContent("ShowHUD", defaultShowHUD);
            showHorizontalLine = CheckContent("ShowHorizontalLine", defaultShowHorizontalLine);
            mainCamera = CheckContent("MainCamera", defaultMainCamera);
            useMousePitchControl = CheckContent("UseMousePitchControl", defaultUseMousePitchControl);
            mouseSensitivity = CheckContent("MouseSensitivity", defaultMouseSensitivity);
            serialPort = CheckContent("SerialPort", defaultSerialPort);
            rudderZero = CheckContent("RudderZero", defaultRudderZero);
            rudderMax = CheckContent("RudderMax", defaultRudderMax);
            rudderReverse = CheckContent("RudderReverse", defaultRudderReverse);
            exportLog = CheckContent("ExportLog", defaultExportLog);
            randomizeWind = CheckContent("RandomizeWind", defaultRandomizeWind);
            windMagnitude = CheckContent("WindMagnitude", defaultWindMagnitude);
            windDirection = CheckContent("WindDirection", defaultWindDirection);
            windRangeSpec = CheckContent("WindRangeSpec", defaultWindRangeSpec);
            takeoffSpeed = CheckContent("TakeoffSpeed", defaultTakeoffSpeed);
            takeoffRoll = CheckContent("TakeoffRoll", defaultTakeoffRoll);
            takeoffPitch = CheckContent("TakeoffPitch", defaultTakeoffPitch);
            takeoffYaw = CheckContent("TakeoffYaw", defaultTakeoffYaw);

            Validate(); // 読み込んだ値の妥当性を検査して、必要に応じて修正.
        }
    }

    // ===== 内容を確認し，存在すれば値を返す =====
    private static T CheckContent<T>(string key, T defaultValue) // 全ての型に対応するジェネリック型
    {
        for (int i = 1; i <= recordNum; i++) // 全ての行で繰り返し.
        {
            if (string.Compare(config.Read(i, 1), key, ignoreCase: true) == 0) // 大文字小文字を無視して該当行の文字列とkeyを比較 -> 等しいなら.
            {
                // Debug.Log("Content exsists: " + key);
                string valueString = config.Read(i, 2); // 該当行の`=`右側にある値を読み込む.

                if (String.IsNullOrWhiteSpace(valueString)) // 値が空白のみで構成されている場合はデフォルト値を返す.
                {
                    Debug.LogWarning($"Value for {key} is empty or whitespace. Using default value: {defaultValue}");
                    return defaultValue;
                }

                try
                {
                    if (typeof(T) == typeof(Enum)) // valueの型がEnumなら
                    {
                        return (T)Enum.Parse(typeof(T), valueString, true);
                    }
                    else // valueの型がEnum以外なら
                    {
                        return (T)Convert.ChangeType(valueString, typeof(T));
                    }
                }
                catch (Exception e)
                {
                    Debug.LogWarning($"Failed convert config string: {e.Message}");
                }
            }
        }
        return defaultValue; // 見つからなかった場合は指定されたデフォルト値を返す. 
    }

    // ===== `Config.txt`の内容を生成して書き込む ====================================
    static public bool Flush()
    {
        config.Clear();
        int linenumber = 1;
        
        void newLine() => linenumber++; // Configの行を1つ進めるローカル関数

        void addConfig(string key, string value) // Configに設定を追加するローカル関数
        {
            config.Write(linenumber, 1, key);
            config.Write(linenumber, 2, value);
            newLine();
        };

        void addString(string str) // Configに文字列を追加するローカル関数
        {
            config.Write(linenumber, 1, "// " + str);
            newLine();
        };

        addString("----- Configurations -----");
        newLine();

        addString("使用する機体の3Dモデル");
        addConfig("AircraftModelName", AircraftModelName);
        newLine();

        addString("使用する機体諸元（以下に利用可能な機体が列挙されます）");
        addString("--------------------------------------------------------");
        string availableAircrafts = "";
        for (int i = 0; i < AircraftData.Names.Count; i++)
        {
            availableAircrafts += AircraftData.Names[i] + ", ";
            if (i % 5 == 0 && i != 0) // 5機体ごとに改行を入れる.
            {
                availableAircrafts += Environment.NewLine;
                addString(availableAircrafts);
                availableAircrafts = "";
            }
        }
        if(availableAircrafts != "") addString(availableAircrafts);
        addString("--------------------------------------------------------");
        addConfig("AircraftDataName", AircraftDataName);
        newLine();

        addString("飛行中の画面の周囲に情報を表示する(True/False)");
        addConfig("ShowHUD", ShowHUD.ToString());
        newLine();

        addString("水平線を表示する(True/False)");
        addConfig("ShowHorizontalLine", ShowHorizontalLine.ToString());
        newLine();

        addString("メインディスプレイを三人称視点にする(True/False)");
        addConfig("MainCamera", MainCamera.ToString());
        newLine();

        addString("マウスによる重心移動を有効化する(True/False)");
        addConfig("UseMousePitchControl", UseMousePitchControl.ToString());
        newLine();

        addString($"マウス感度を設定する(初期値: {defaultMouseSensitivity:0.0})");
        addConfig("MouseSensitivity", MouseSensitivity.ToString("0.0"));
        newLine();

        addString($"シリアルポートの名前(初期値: {defaultSerialPort})");
        addString($"(Windowsでは`COM1`など，Macでは`/dev/tty.usbmodem1421`など)");
        addString($"候補：{string.Join(", ", SerialHandler.serialPortsString)}");
        addConfig("SerialPort", SerialPort);
        newLine();

        addString($"ニュートラルにおけるラダー入力値(初期値: {defaultRudderZero:0.0})");
        addConfig("RudderZero", RudderZero.ToString("0.0"));
        newLine();

        addString($"最大入力におけるラダー入力値(初期値: {defaultRudderMax:0.0})");
        addConfig("RudderMax", RudderMax.ToString("0.0"));
        newLine();

        addString($"ラダー入力値の反転(初期値: {defaultRudderReverse})");
        addConfig("RudderReverse", RudderReverse.ToString());
        newLine();

        addString("フライトログの出力を有効化する(True/False)");
        addConfig("ExportLog", ExportLog.ToString());
        newLine();

        addString($"風速[m/s](初期値: {defaultWindMagnitude:0.0})");
        addConfig("WindMagnitude", WindMagnitude.ToString("0.0"));
        newLine();

        addString($"風上[deg](範囲: [L]-180.0 ↔ [R]180.0) / 初期値: {defaultWindDirection:0.0}");
        addConfig("WindDirection", WindDirection.ToString("0.0"));
        newLine();

        addString($"風の範囲指定(※「風速」「風上」の設定を上書きします / 初期値: {defaultWindRangeSpec})");
        addString("以下の形式に従って入力してください（数値範囲はマイクラと同様）:");
        addString("    風速:風向@開始距離..終了距離; 風速:風向@開始距離..終了距離; ...");
        addString("設定例:");
        addString("    設定なし（「風速」「風上」の設定が適用されます）: `None`");
        addString("    100mまで90degから2m/s，それ以降は90degから3m/s): `2:90@..100; 3:90@100..` ");
        addString("    300mから400mまでは-90degから1m/s，それ以外は風なし): `1:-90@300..400`");
        addConfig("WindRangeSpec", WindRangeSpec);
        newLine();

        addString($"離陸時のスピード[m/s](初期値: {defaultTakeoffSpeed:0.0})");
        addConfig("TakeoffSpeed", TakeoffSpeed.ToString("0.0"));
        newLine();

        addString($"離陸時のロール[deg](初期値: {defaultTakeoffRoll:0.0})");
        addConfig("TakeoffRoll", TakeoffRoll.ToString("0.0"));
        newLine();

        addString($"離陸時のピッチ[deg](初期値: {defaultTakeoffPitch:0.0})");
        addConfig("TakeoffPitch", TakeoffPitch.ToString("0.0"));
        newLine();

        addString($"離陸時のヨー[deg](初期値: {defaultTakeoffYaw:0.0})");
        addConfig("TakeoffYaw", TakeoffYaw.ToString("0.0"));
        newLine();

        addString("定常風をランダム化する(True/False)");
        addConfig("RandomizeWind", RandomizeWind.ToString());
        newLine();

        try
        {
            config.Flush(_configFilePath); // 変更されたファイルの内容を再度書き込み.
            Debug.Log("Config file flushed.");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to sync config file: {ex.Message}");
            return false;
        }

        _lastFlushTime = DateTime.Now; // 最後に書き込みをおこなった時間を現在時刻に更新

        return true;
    }
}
