using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    public GameParameters game;
    public AerodynamicParameters aero;
    public AerodynamicCalculator calc;
    public PilotPositionSensor pilot;
    public HoldingDetector hold;
    public CameraManager cm;
    private GameObject FlightSetting = null;

    public static GameManager instance = null;
    public bool FirstLoad;//シミュ起動後最初のシーンロードか否か
    // ===== このインスタンスがロードされたときに実行される ==============
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            FirstLoad = true;
            DontDestroyOnLoad(this.gameObject);
            Log.Append("GameManager Awake.");
        }
        else
        {
            FirstLoad = false;
            Destroy(this.gameObject);
        }
        game = new();
        aero = new();
    }

    // ===== Updateの初回呼び出し前に実行される =============
    private void Start()
    {
        FlightSetting = GameObject.Find("FlightSetting");
        cm = GameObject.Find("CameraManager").GetComponent<CameraManager>();
        calc = new(game, aero);
        pilot = new(aero);
        hold = new(game, cm);
        game.OnStatusChange += OnStatusChange;
        SerialHandler.OnHoldingPositive += hold.PositiveHolding;
        SerialHandler.OnHoldingNegative += hold.NegativeHolding;
        calc.Initialize();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        game.status = GameParameters.Status.Preparation;
        calc?.Initialize(); // シーンロードのたびにRigidbodyが破壊されるので再取得
    }
    // ===== 毎フレーム実行される ==============
    private void Update()
    {
        calc.DevicesUpdate();

        // ----- ゲームの状態を管理 -----
        if (game.status == GameParameters.Status.Flight)
        {
            Time.timeScale = 1f;
        }
        else
        {
            Time.timeScale = 0f;
        }
    }

    private void OnStatusChange()
    {
        if (game.status == GameParameters.Status.Flight)
        {
            Log.Append("[Takeoff!]");
        }
    }

    public void FixedUpdate()
    {
        // ----- シミュレーションの実行 -----
        calc.CalculatorFixedUpdate();
    }
}
