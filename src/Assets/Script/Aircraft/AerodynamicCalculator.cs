using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.Text;
using System.IO;
using System;

public class AerodynamicCalculator
{
    //設計データ書き込み用変数
    //protected string customCsvPath;//ファイルパス

    //protected string fileName = "CustomPlaneData.csv";//ファイル名
    //public static List<List<string>> CsvList = new List<List<string>>();//CSVファイルリスト
    //protected bool CanReadCsv = false;
    private GameParameters game;
    private AerodynamicParameters aero;

    private GameManager gm = GameManager.instance; // MyGameManagerをgmとして宣言
    private CameraManager cm;

    private bool DynamicPitchInertiaInitialized = false;

    public AerodynamicCalculator(GameParameters _game, AerodynamicParameters _aero)
    {
        game = _game;
        aero = _aero;
    }

    private void SetAircraft()
    {
        if (gm.game.PlaneName != null)
        {
            aero.Aircraft = GameObject.Find(gm.game.PlaneName);
        }
        else
        {
            gm.game.PlaneName = gm.game.DefaultPlane;
            aero.Aircraft = GameObject.Find(gm.game.PlaneName);
        }
    }

    // Start is called before the first frame update
    public void Initialize()
    {
        SetAircraft();
        cm = GameObject.Find("CameraManager").GetComponent<CameraManager>();

        // Get rigidbody component
        aero.PlaneRigidbody = aero.Aircraft.GetComponent<Rigidbody>();
        aero.Aircraft.transform.rotation = Quaternion.Euler(0.0f, Config.TakeoffYaw, 0.0f);

        aero.SensorPoint = GameObject.Find("SensorPoint");

        //設計データ読み込み用
        // fileName = gm.game.PlaneName + ".csv";
        // customCsvPath = gm.game.PlaneName + ".csv";
        // customCsvPath = Path.Combine(Directory.GetParent(Application.dataPath).FullName, "CustomPlaneData.csv");
        // Debug.Log("File path: " + customCsvPath);
        // ReadFile();

        // Input Specifications
        InputSpecifications();

        //pitchGravityPilotS = ((PlaneRigidbody.mass*pitchGravity)-(massAircraft*centerOfMassAircraft))/massPilot;
        //Debug.Log(massAircraft+","+centerOfMassAircraft+","+massPilot+","+gm.game.lengthForward+","+gm.game.lengthBackward);
        /*
        if (gm.game.massPilotReal == 0)
        {//体重入力省略の場合の処理
            gm.game.massPilotReal = massPilot;
        }
        */
        if (aero.massAircraft != 0 && aero.centerOfMassAircraft != 0 && aero.massPilot != 0 && gm.game.lengthForward != 0 && gm.game.lengthBackward != 0)
        {
            aero.centerOfMassPilot = -1 * aero.massAircraft * aero.centerOfMassAircraft / aero.massPilot;

            //今までのやつ
            /*
            massLeftRightS = (massPilot*(pitchGravityPilotS+gm.game.lengthBackward)/(gm.game.lengthForward+gm.game.lengthBackward))/2;
            massBackwardS = (massPilot - massLeftRightS*2)/2;
            */

            // =====AutoFactorSetter.csへ移植=====
            /*
            if(gm.game.VRMode){//HMDの質量を加算
                float massPilotVR=gm.game.massPilotReal+0.588f;
                massLeftRightS = (massPilotVR*(pitchGravityPilotS+gm.game.lengthBackward)/(gm.game.lengthForward+gm.game.lengthBackward)); // 前部荷重の理論値
                massBackwardS = (massPilotVR - massLeftRightS); // 後部荷重の理論値
            }
            else{
                float massPilotNonVR=gm.game.massPilotReal;
                massLeftRightS = (massPilotNonVR*(pitchGravityPilotS+gm.game.lengthBackward)/(gm.game.lengthForward+gm.game.lengthBackward)); // 前部荷重の理論値
                massBackwardS = (massPilotNonVR - massLeftRightS); // 後部荷重の理論値
            }
            */
        }

        aero.YMin = aero.massAircraft / 2;
        aero.YrMax = 80.0f;
        aero.YlMax = 80.0f;

        CalculatorInitialize();
    }

    public void DevicesUpdate()//フライトモデルに関わらず実行されるINPUT関連の処理
    {
        float centerOfMassPrev = aero.centerOfMass;
        float centerOfMassPilotPrev = aero.centerOfMassPilot;

        if (Config.OverridePilotMass != 0)
        {
            aero.massPilot = Config.OverridePilotMass;
            aero.massAircraft = AircraftData.mass - aero.massPilot;
        }

        if (Config.UseMousePitchControl)
        {//マウスコントロール
            if (aero.massPilot != 0)
            {
                aero.centerOfMassPilot = -aero.massAircraft * aero.centerOfMassAircraft / aero.massPilot;
                aero.centerOfMassPilotRaw = aero.centerOfMassPilot + ((Input.mousePosition.y - aero.dh0) * Config.MouseSensitivity) * 0.0002f;
            }
            else
            {
                aero.centerOfMassPilot = 0;
                aero.centerOfMassPilotRaw = 0;
            }
            //pitchGravity = ((pitchGravityPilot*massPilot)+(centerOfMassAircraft*massAircraft))/PlaneRigidbody.mass;
            aero.centerOfMass = (gm.game.CenterOfMassErrorValue + ((Input.mousePosition.y - aero.dh0) * Config.MouseSensitivity) * 0.0002f) * gm.game.CenterOfMassRandValue;
        }

        if (Input.GetAxisRaw("GStick") != 0)
        {//ゲームパッドコントロールのトリガー
            if (aero.massPilot != 0)
            {
                aero.centerOfMassPilot = -aero.massAircraft * aero.centerOfMassAircraft / aero.massPilot;
                aero.centerOfMassPilotRaw = aero.centerOfMassPilot - Input.GetAxisRaw("GStick") * 0.10f;
            }
            else
            {
                aero.centerOfMassPilot = 0;
                aero.centerOfMassPilotRaw = 0;
            }
            aero.centerOfMass = (gm.game.CenterOfMassErrorValue + ((aero.centerOfMassPilotRaw * aero.massPilot) + (aero.centerOfMassAircraft * aero.massAircraft)) / aero.PlaneRigidbody.mass) * gm.game.CenterOfMassRandValue;
        }

        if (SerialHandler.Available)//フレームコントロール
        {
            // マイコン側でkgに変換する
            gm.pilot.UpdateLoadcellFactor();

            aero.massForward  = SerialHandler.massForwardRaw;
            aero.massBackward = SerialHandler.massBackwardRaw;

            aero.massForward  *= gm.pilot.loadcellFactor;
            aero.massBackward *= gm.pilot.loadcellFactor;

            aero.massPilot = aero.massForward + aero.massBackward;

            // 重心フレーム上での桁中心モーメントについて，（前後センサにかかる荷重によるモーメント）＝（パイロットの体重によるモーメント）とし，その両辺をパイロットの体重で割った式
            aero.centerOfMassPilotRaw = (gm.game.lengthForward * aero.massForward + gm.game.lengthBackward * aero.massBackward) / (aero.massForward + aero.massBackward); // 補正前のパイロット重心[m]

            // 補正
            aero.centerOfMassPilot = aero.centerOfMassPilotRaw + aero.centerOfMassPilotOffset; // 補正後のパイロット重心[m]

            // 桁中心モーメントについて，（パイロット体重と空虚重量〈パイロットなしの機体重量〉によるモーメント）＝（全備重量によるモーメント）とし，その両辺を全備重量で割った式
            aero.centerOfMass = (aero.massPilot * aero.centerOfMassPilot + aero.massAircraft * aero.centerOfMassAircraft) / (aero.massPilot + aero.massAircraft);

            if (-0.4f < aero.centerOfMass && aero.centerOfMass < 0.4f)//外れ値除去処理(基本的に重心は±0.4を超えることはない
            { }
            else
            {
                Debug.Log("外れ値除去成功！");
                aero.centerOfMass = centerOfMassPrev;
                aero.centerOfMassPilot = centerOfMassPilotPrev;
            }
        }
        // Get control surface angles
        aero.de = 0.000f;
        aero.dr = 0.000f;

        if (!SerialHandler.Available)
        {
            aero.de = Input.GetAxisRaw("Vertical") * aero.deMAX;
            aero.dr = -Input.GetAxisRaw("Horizontal") * aero.drMAX * gm.game.RudderRandValue;
        }
        if (Input.GetMouseButton(0)) { aero.dr = aero.drMAX * gm.game.RudderRandValue; }
        else if (Input.GetMouseButton(1)) { aero.dr = -aero.drMAX * gm.game.RudderRandValue; }

        if (Input.GetAxisRaw("Trigger") * aero.drMAX != 0)
        {
            aero.dr = -Input.GetAxisRaw("Trigger") * aero.drMAX * gm.game.RudderRandValue;
        }

        if (SerialHandler.Available)
        {
            //↓必要な処理
            // dr = ((JoyStickNow - gm.game.JoyStick0) / gm.game.JoyStickFactor) * drMAX * gm.game.RudderRandValue;
            aero.dr = aero.drMAX * SerialHandler.rudder * gm.game.RudderRandValue; // ラダー駆動角
            // Debug.Log($"dr: {dr}");
        }

        if (gm.game.RudderError && gm.game.RudderErrorMode != 0)
        {
            if (gm.game.RudderErrorMode == 1)
            {
                aero.dr = gm.game.RudderErrorValue * aero.drMAX;
            }
            else if (gm.game.RudderErrorMode == 2)
            {
                if (UnityEngine.Random.Range(0.0f, 1.0f) < 0.5f)
                {
                    aero.dr = gm.game.RudderErrorValue * aero.drMAX;
                }
            }
            else if (gm.game.RudderErrorMode == 3)
            {
                aero.dr += gm.game.RudderErrorValue * aero.drMAX;
            }
        }

        // VR Only Mode (重心センサーを使う場合は使用しない)
        if (Config.VrOnlyMode)
        {
            aero.massPilot = 68.0f; // [kg]

            aero.centerOfMassPilot = cm.GetZAxisMovement(); // パイロット重心は直接取得できる.

            // 桁中心モーメントについて，（パイロット体重と空虚重量〈パイロットなしの機体重量〉によるモーメント）＝（全備重量によるモーメント）とし，その両辺を全備重量で割った式
            aero.centerOfMass = (aero.massPilot * aero.centerOfMassPilot + aero.massAircraft * aero.centerOfMassAircraft) / (aero.massPilot + aero.massAircraft);
        }
    }
    public void InputSpecifications()
    {
        AircraftData.Load(Config.AircraftDataName);

        // 機体の重量と慣性モーメント - 6
        aero.PlaneRigidbody.mass = AircraftData.mass;
        aero.PlaneRigidbody.centerOfMass = AircraftData.centerOfMass;
        aero.PlaneRigidbody.inertiaTensor = AircraftData.inertiaTensor;
        aero.PlaneRigidbody.inertiaTensorRotation = AircraftData.inertiaTensorRotation;
        aero.massAircraft = AircraftData.massAircraft;
        aero.centerOfMassAircraft = AircraftData.centerOfMassAircraft;

        // 巡航時 - 5
        aero.Airspeed0 = AircraftData.Airspeed0;
        aero.alpha0 = AircraftData.alpha0;
        aero.CDp0 = AircraftData.CDp0;
        aero.Cmw0 = AircraftData.Cmw0;
        aero.CLMAX = AircraftData.CLMAX;

        // 主翼 - 7
        aero.Sw = AircraftData.Sw;
        aero.bw = AircraftData.bw;
        aero.cMAC = AircraftData.cMAC;
        aero.aw = AircraftData.aw;
        aero.hw = AircraftData.hw;
        aero.ew = AircraftData.ew;
        aero.AR = AircraftData.AR;

        // 水平尾翼 - 7
        aero.Downwash = AircraftData.Downwash;
        aero.St = AircraftData.St;
        aero.at = AircraftData.at;
        aero.lt = AircraftData.lt;
        aero.deMAX = AircraftData.deMAX;
        aero.tau = AircraftData.tau;
        aero.VH = AircraftData.VH;

        // 垂直尾翼 - 1
        aero.drMAX = AircraftData.drMAX;

        // 地面効果 - 1
        aero.CGEMIN = AircraftData.CGEMIN;

        // 安定微係数 - 12
        aero.Cyb = AircraftData.Cyb;
        aero.Cyp = AircraftData.Cyp;
        aero.Cyr = AircraftData.Cyr;
        aero.Cydr = AircraftData.Cydr;
        aero.Clb = AircraftData.Clb;
        aero.Clp = AircraftData.Clp;
        aero.Clr = AircraftData.Clr;
        aero.Cldr = AircraftData.Cldr;
        aero.Cnb = AircraftData.Cnb;
        aero.Cnp = AircraftData.Cnp;
        aero.Cnr = AircraftData.Cnr;
        aero.Cndr = AircraftData.Cndr;

        // 離陸 - 1
        aero.YL = AircraftData.YL;
    }

    public void CalculatorInitialize()
    {
        // Set take-off speed
        if (GameManager.instance.game.FlightMode == "BirdmanRally")
        {
            //GameManager.instance.game.Airspeed_TO = 5.0f; // Airspeed at take-off [m/s]
            aero.PlaneRigidbody.velocity = Vector3.zero;
        }
        else if (GameManager.instance.game.FlightMode == "TestFlight")
        { //
            aero.PlaneRigidbody.velocity = new Vector3(
                aero.Airspeed0 * Mathf.Cos(Mathf.Deg2Rad * aero.alpha0) * Mathf.Cos(Mathf.Deg2Rad * Config.TakeoffYaw),
                -aero.Airspeed0 * Mathf.Sin(Mathf.Deg2Rad * aero.alpha0),
                -aero.Airspeed0 * Mathf.Cos(Mathf.Deg2Rad * aero.alpha0) * Mathf.Sin(Mathf.Deg2Rad * Config.TakeoffYaw)
            );
        }

        // Calculate CL at cluise
        aero.CL0 = (aero.PlaneRigidbody.mass * Physics.gravity.magnitude) / (0.5f * aero.rho * aero.Airspeed0 * aero.Airspeed0 * aero.Sw);
        aero.CLt0 = (aero.Cmw0 + aero.CL0 * aero.hw) / (aero.VH + (aero.St / aero.Sw) * aero.hw);
        aero.CLw0 = aero.CL0 - (aero.St / aero.Sw) * aero.CLt0;
        if (aero.Downwash) { aero.epsilon0 = (aero.CL0 / (Mathf.PI * aero.ew * aero.AR)) * Mathf.Rad2Deg; }

        aero.dh0 = Screen.height / 2f; // Initial Mouse Position

        //Debug.Log(aero.CLw0);
        aero.hw0 = aero.hw;
        aero.lt0 = aero.lt;
    }

    public void CalculatorFixedUpdate()
    {


    　　　　//Debug.Log("isoSim1 FixedUpdate");
        //入力系統
        //リジットボディに代入
        aero.PlaneRigidbody.centerOfMass = new Vector3(aero.centerOfMass, aero.PlaneRigidbody.centerOfMass.y, aero.PlaneRigidbody.centerOfMass.z);

        //hwに代入する重心位置(%MAC)を計算
        aero.hw2 = aero.hw0 - (aero.centerOfMass / aero.cMAC);
        //hwに代入
        aero.hw = aero.hw2;

        aero.lt = aero.lt0 + aero.centerOfMass;

        //if (GameManager.instance.game.PlaneName == "Tatsumi")
        //{
        //    //float Iyy = (85.6f * centerOfMass * centerOfMass) + (38.63f * centerOfMass) + 1241.85f;
        //    Vector3 tensor = PlaneRigidbody.inertiaTensor;
        //    //tensor.y = Iyy;
        //    tensor.x = 5f;
        //    PlaneRigidbody.inertiaTensor = tensor;
        //}

        // Velocity and AngularVelocity
        float u = aero.Aircraft.transform.InverseTransformDirection(aero.PlaneRigidbody.velocity).x;
        float v = -aero.Aircraft.transform.InverseTransformDirection(aero.PlaneRigidbody.velocity).z;
        float w = -aero.Aircraft.transform.InverseTransformDirection(aero.PlaneRigidbody.velocity).y;
        float p = -aero.Aircraft.transform.InverseTransformDirection(aero.PlaneRigidbody.angularVelocity).x * Mathf.Rad2Deg;
        float q = aero.Aircraft.transform.InverseTransformDirection(aero.PlaneRigidbody.angularVelocity).z * Mathf.Rad2Deg;
        float r = aero.Aircraft.transform.InverseTransformDirection(aero.PlaneRigidbody.angularVelocity).y * Mathf.Rad2Deg;
        float hE = aero.PlaneRigidbody.position.y;
        float Distance = (aero.PlaneRigidbody.position - GameManager.instance.game.PlatformPosition).magnitude - 10f;

        // Force and Momentum
        Vector3 AerodynamicForce = Vector3.zero;
        Vector3 AerodynamicMomentum = Vector3.zero;
        Vector3 TakeoffForce = Vector3.zero;

        // Hoerner and Borst (Modified)
        aero.CGE = (aero.CGEMIN + 33f * Mathf.Pow((hE / aero.bw), 1.5f)) / (1f + 33f * Mathf.Pow((hE / aero.bw), 1.5f));
        if (GameManager.instance.game.FlightMode == "BirdmanRally" && Distance < -0.5f)
        {
            //CGE = (CGEMIN+33f*Mathf.Pow((hE/bw),1.5f))/(1f+33f*Mathf.Pow((hE/bw),1.5f));
            aero.CGE = (aero.CGEMIN + 33f * Mathf.Pow((1.5f / aero.bw), 1.5f)) / (1f + 33f * Mathf.Pow((1.5f / aero.bw), 1.5f));
        }
        //Debug.Log(CGE);
        //if (GameManager.instance.game.MousePitchControl){
        //    dh = -(Input.mousePosition.y-dh0)*0.0002f*GameManager.instance.game.MouseSensitivity;
        //}

        // Gust
        aero.LocalGustMag = (Wind.GetMagnitude(Distance) + GameManager.instance.game.GustRandValue) * Mathf.Pow((hE / aero.hE0), 1f / 7f);
        aero.Gust = Quaternion.AngleAxis(Wind.GetDirection(Distance), Vector3.up) * (Vector3.right * aero.LocalGustMag);
        Vector3 LocalGust = aero.Aircraft.transform.InverseTransformDirection(aero.Gust);
        float ug = LocalGust.x + 1e-10f;
        float vg = -LocalGust.z;
        float wg = -LocalGust.y;
        if (ug > 0) { aero.LocalGustDirection = Mathf.Atan(vg / (ug + 1e-10f)) * Mathf.Rad2Deg; }
        else { aero.LocalGustDirection = Mathf.Atan(vg / (ug + 1e-10f)) * Mathf.Rad2Deg + vg / Mathf.Abs((vg + 1e-10f)) * 180; }

        // Calculate angles
        aero.Airspeed = Mathf.Sqrt((u + ug) * (u + ug) + (v + vg) * (v + vg) + (w + wg) * (w + wg));
        aero.Groundspeed = Mathf.Sqrt(u * u + v * v);
        if (aero.SensorPoint != null)
        {
            aero.ALT = aero.SensorPoint.transform.position.y;
        }
        //Debug.Log(Groundspeed);
        aero.alpha = Mathf.Atan((w + wg) / (u + ug)) * Mathf.Rad2Deg;
        //Debug.Log(alpha);

        aero.beta = Mathf.Atan((v + vg) / aero.Airspeed) * Mathf.Rad2Deg;

        // Wing and Tail
        aero.CLw = aero.CLw0 + aero.aw * (aero.alpha - aero.alpha0);
        aero.CLt = aero.CLt0 + aero.at * ((aero.alpha + GameManager.instance.game.TailSetDeg - aero.alpha0) + (1f - aero.CGE * (aero.CLw / aero.CLw0)) * aero.epsilon0 + aero.de * aero.tau + ((aero.lt - aero.dh * aero.cMAC) / aero.Airspeed) * q);
        if (Mathf.Abs(aero.CLw) > aero.CLMAX) { aero.CLw = (aero.CLw / Mathf.Abs(aero.CLw)) * aero.CLMAX; } // Stall
        if (Mathf.Abs(aero.CLt) > aero.CLMAX) { aero.CLt = (aero.CLt / Mathf.Abs(aero.CLt)) * aero.CLMAX; } // Stall

        // Lift and Drag
        aero.CL = aero.CLw + (aero.St / aero.Sw) * aero.CLt; // CL
        aero.CD = aero.CDp0 * (1f + Mathf.Abs(Mathf.Pow((aero.alpha / 9f), 3f))) + ((aero.CL * aero.CL) / (Mathf.PI * aero.ew * aero.AR)) * aero.CGE; // CD

        // Force
        aero.Cx = aero.CL * Mathf.Sin(Mathf.Deg2Rad * aero.alpha) - aero.CD * Mathf.Cos(Mathf.Deg2Rad * aero.alpha); // Cx
        aero.Cy = aero.Cyb * aero.beta + aero.Cyp * (1f / Mathf.Rad2Deg) * ((p * aero.bw) / (2f * aero.Airspeed)) + aero.Cyr * (1f / Mathf.Rad2Deg) * ((r * aero.bw) / (2f * aero.Airspeed)) + aero.Cydr * aero.dr; // Cy
        aero.Cz = -aero.CL * Mathf.Cos(Mathf.Deg2Rad * aero.alpha) - aero.CD * Mathf.Sin(Mathf.Deg2Rad * aero.alpha); // Cz

        // Torque
        aero.Cl = aero.Clb * aero.beta + aero.Clp * (1f / Mathf.Rad2Deg) * ((p * aero.bw) / (2f * aero.Airspeed)) + aero.Clr * (1f / Mathf.Rad2Deg) * ((r * aero.bw) / (2f * aero.Airspeed)) + aero.Cldr * aero.dr; // Cl
        aero.Cm = aero.Cmw0 + aero.CLw * aero.hw - aero.VH * aero.CLt + aero.CL * aero.dh; // Cm
        aero.Cn = aero.Cnb * aero.beta + aero.Cnp * (1f / Mathf.Rad2Deg) * ((p * aero.bw) / (2f * aero.Airspeed)) + aero.Cnr * (1f / Mathf.Rad2Deg) * ((r * aero.bw) / (2f * aero.Airspeed)) + aero.Cndr * aero.dr; // Cn

        AerodynamicForce.x = 0.5f * aero.rho * aero.Airspeed * aero.Airspeed * aero.Sw * aero.Cx;
        AerodynamicForce.y = 0.5f * aero.rho * aero.Airspeed * aero.Airspeed * aero.Sw * (-aero.Cz);
        AerodynamicForce.z = 0.5f * aero.rho * aero.Airspeed * aero.Airspeed * aero.Sw * (-aero.Cy);
        //Debug.Log("CLt"+CLt+"CL"+CL+"Cz"+Cz+"z"+AerodynamicForce.y);
        AerodynamicMomentum.x = 0.5f * aero.rho * aero.Airspeed * aero.Airspeed * aero.Sw * aero.bw * (-aero.Cl);//roll
        AerodynamicMomentum.y = 0.5f * aero.rho * aero.Airspeed * aero.Airspeed * aero.Sw * aero.bw * aero.Cn;//yaw
        AerodynamicMomentum.z = 0.5f * aero.rho * aero.Airspeed * aero.Airspeed * aero.Sw * aero.cMAC * aero.Cm;//pitch

        if (GameManager.instance.game.FlightMode == "BirdmanRally" && Distance < -0.5f)
        {
            //Debug.Log("Dist: " + Distance);
            CalculateRotation();

            float W = aero.PlaneRigidbody.mass * Physics.gravity.magnitude;//重力
            float L = 0.5f * aero.rho * aero.Airspeed * aero.Airspeed * aero.Sw * (aero.Cx * Mathf.Sin(Mathf.Deg2Rad * aero.theta) - aero.Cz * Mathf.Cos(Mathf.Deg2Rad * aero.theta));//揚力
            float N = (W - L) * Mathf.Cos(Mathf.Deg2Rad * 3.5f); // N=(W-L)*cos(3.5deg)//翼持ちの抵抗力
            float P = (aero.PlaneRigidbody.mass * Config.TakeoffSpeed * Config.TakeoffSpeed) / (2f * 10f); // P=m*Vto*Vto/2*L//推進力

            //離陸方向をYaw回転に合わせて水平方向に修正
            //Vector3 takeoffDirection = Quaternion.Euler(0f, Config.TakeoffYaw, 0f) * Vector3.forward;
            //TakeoffForce = takeoffDirection * P;

            //TakeoffForce.y = N*Mathf.Cos(Mathf.Deg2Rad*3.5f);

            //float TOFh = P;
            //float TOFv = N*Mathf.Cos(Mathf.Deg2Rad*3.5f);
            //TakeoffForce.x = TOFv*Mathf.Sin(GameManager.instance.game.TailRotation) + TOFh*Mathf.Cos(GameManager.instance.game.TailRotation);
            //TakeoffForce.y = TOFv*Mathf.Cos(GameManager.instance.game.TailRotation) - TOFh*Mathf.Sin(GameManager.instance.game.TailRotation);
            //Debug.Log("Power:"+P);

            TakeoffForce.x = P * Mathf.Cos(Mathf.Deg2Rad * Config.TakeoffYaw);
            TakeoffForce.y = N * Mathf.Cos(Mathf.Deg2Rad * 3.5f);
            TakeoffForce.z = -P * Mathf.Sin(Mathf.Deg2Rad * Config.TakeoffYaw);

            AerodynamicForce.z = 0f;
            AerodynamicMomentum.x = 0f;//
            AerodynamicMomentum.y = 0f;

            //transform.rotation = Quaternion.Euler(transform.localEulerAngles.x, transform.localEulerAngles.y, GameManager.instance.game.TailRotation);
            //PlaneRigidbody.constraints = RigidbodyConstraints.FreezePositionZ;

            if (AerodynamicMomentum.x <= 0)
            {//左から右に吹く風 左翼がより大きな揚力を生む
                if (Mathf.Abs(AerodynamicMomentum.x) > aero.YL * aero.YMin)
                {//左翼が翼持ちの手を離れている状態
                    //Debug.Log("A1");
                    aero.YlMoment = 0;//既に翼持ちを離れている為、翼持ちはモーメントを与えられない
                }
                else
                {
                    //Debug.Log("B1");
                    aero.YlMoment = -aero.YL * aero.YMin - AerodynamicMomentum.x;//翼持ちが与えるモーメントは、機体を支える最小限のモーメントから風が与えるそれを引いた値である
                }

                if (Mathf.Abs(AerodynamicMomentum.x + aero.YlMoment) <= aero.YL * aero.YrMax)
                {//右翼持ちにまだ余裕がある状態
                    //Debug.Log("C1");
                    aero.YrMoment = -(AerodynamicMomentum.x + aero.YlMoment);//翼持ちに風と逆の翼持ちのモーメントを足した大きな負荷が掛かるが、まだ耐えられる
                }
                else
                {
                    //Debug.Log("D1");
                    aero.YrMoment = aero.YL * aero.YrMax;//つり合いが取れずに右翼持ちのモーメントが足りない状態
                }
            }
            else
            {//右から左に吹く風 右翼がより大きな揚力を生む
                if (Mathf.Abs(AerodynamicMomentum.x) > aero.YL * aero.YMin)
                {//右翼が翼持ちの手を離れている状態
                    //Debug.Log("A2");
                    aero.YrMoment = 0;
                }
                else
                {
                    //Debug.Log("B2");
                    aero.YrMoment = aero.YL * aero.YMin - AerodynamicMomentum.x;
                }

                if (Mathf.Abs(AerodynamicMomentum.x + aero.YrMoment) <= aero.YL * aero.YlMax)
                {//左翼持ちにまだ余裕がある状態
                    //Debug.Log("C2");
                    aero.YlMoment = AerodynamicMomentum.x + aero.YrMoment;
                }
                else
                {
                    //Debug.Log("D2");
                    aero.YlMoment = aero.YL * aero.YlMax;
                }
            }
            //Debug.Log("YlMoment:"+aero.YlMoment+"YrMoment:"+aero.YrMoment+"aeroX:"+AerodynamicMomentum.x);
            //AerodynamicMomentum.x += aero.YrMoment + aero.YlMoment;//最終的なロールモーメントの計算//一旦消す
            aero.TakeOff = false;
        }
        else
        {
            aero.TakeOff = true;
            //PlaneRigidbody.constraints = RigidbodyConstraints.None;
        }
        //else if(GameManager.instance.game.FlightMode=="BirdmanRally" && !AddTaleForce){
        //    AddTaleForce =true;
        //}
        //Debug.Log(AerodynamicForce.z);

       　if (Config.EnableDynamicPitchInertia)　// ピッチの慣性モーメントを可変に
       　{
            if (!DynamicPitchInertiaInitialized)
            {
                DynamicPitchInertiaInitialize();
            }
            if (DynamicPitchInertiaInitialized)
            {
                DynamicPitchInertiaFixedUpdate();
            }
       　}

        aero.PlaneRigidbody.AddRelativeForce(AerodynamicForce, ForceMode.Force);
        aero.PlaneRigidbody.AddRelativeTorque(AerodynamicMomentum, ForceMode.Force);
        aero.PlaneRigidbody.AddForce(TakeoffForce, ForceMode.Force);
        aero.nz = AerodynamicForce.y / (aero.PlaneRigidbody.mass * Physics.gravity.magnitude);
    }

    void CalculateRotation()
    {
        float q1 = GameManager.instance.game.Plane.transform.rotation.x;
        float q2 = -GameManager.instance.game.Plane.transform.rotation.y;
        float q3 = -GameManager.instance.game.Plane.transform.rotation.z;
        float q4 = GameManager.instance.game.Plane.transform.rotation.w;
        float C11 = q1 * q1 - q2 * q2 - q3 * q3 + q4 * q4;
        float C22 = -q1 * q1 + q2 * q2 - q3 * q3 + q4 * q4;
        float C12 = 2f * (q1 * q2 + q3 * q4);
        float C13 = 2f * (q1 * q3 - q2 * q4);
        float C32 = 2f * (q2 * q3 - q1 * q4);

        aero.phi = -Mathf.Atan(-C32 / C22) * Mathf.Rad2Deg;
        aero.theta = -Mathf.Asin(C12) * Mathf.Rad2Deg;
        aero.psi = -Mathf.Atan(-C13 / C11) * Mathf.Rad2Deg;
    }

    void DynamicPitchInertiaInitialize()
    {
        DynamicPitchInertiaInitialized = true;
        // 定常においてはパイロット体重によるモーメントと空虚重量によるモーメントの和が設計上の重心位置と等しくなることから，設計上のパイロットの重心位置 pilotCenterOfMassDefaultを求める
        float centerOfMassPilotDefault = (AircraftData.mass * AircraftData.centerOfMass.x - aero.massAircraft * aero.centerOfMassAircraft) / aero.massPilot;
        // 設計値の慣性モーメントからパイロット（質点）による慣性モーメントを引き算して，空虚のx = 0における慣性モーメント IyyAircraftOriginを求める
        float IyyAircraftOrigin = AircraftData.inertiaTensor.z - aero.massPilot * Mathf.Pow(centerOfMassPilotDefault, 2);
        // 平行軸の定理の逆を用いて，空虚の機体重心位置における慣性モーメント IyyAircraftCenterOfMassを求める
        aero.IyyAircraftCenterOfMass = IyyAircraftOrigin - aero.massAircraft * Mathf.Pow(aero.centerOfMassAircraft, 2);
        // Debug.Log($"IyyAircraftCOG: {aero.IyyAircraftCenterOfMass} ({DynamicPitchInertiaInitialized})");

        if (aero.IyyAircraftCenterOfMass < 0 || double.IsNaN(aero.IyyAircraftCenterOfMass)) // モーメントが負になる場合やNaNになる場合の対処
        {
            // Debug.Log($"massPilot: {aero.massPilot}"); // massPilotが負の場合であることがほとんど
            DynamicPitchInertiaInitialized = false;
        }
        else
        {
            Debug.Log("[DynamicPitchInertia] Initialized! - IyyAircraftCenterOfMass: " + aero.IyyAircraftCenterOfMass);
        }
    }

    void DynamicPitchInertiaFixedUpdate()
    {
        // 変化する重心位置における現在のパイロット（質点）による慣性モーメント IyyPilotを求める
        float IyyPilot = aero.massPilot * Mathf.Pow(aero.centerOfMassPilot - aero.centerOfMass, 2);
        // 全備の重心位置と空虚の機体重心位置の差から，平行軸の定理より現在の空虚の機体の慣性モーメント IyyAircraftを求める
        float IyyAircraft = aero.IyyAircraftCenterOfMass + aero.massAircraft * Mathf.Pow(aero.centerOfMass - aero.centerOfMassAircraft, 2);
        // これらの和が現在の全備の慣性モーメント Iyy
        float Iyy = IyyPilot + IyyAircraft;

        // Debug.Log($"IyyPilot: {IyyPilot}, IyyAircraft: {IyyAircraft}, Iyy: {Iyy}");

        // ex.) inertiaTensor = new Vector3(961f, 1024f, 80f); //Ixx, Izz, Iyy
        Vector3 tensor = aero.PlaneRigidbody.inertiaTensor;
        tensor.z = Iyy;
        aero.PlaneRigidbody.inertiaTensor = tensor;
    }
}
