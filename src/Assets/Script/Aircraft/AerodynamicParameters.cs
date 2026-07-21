using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AerodynamicParameters
{
    // public

    [System.NonSerialized] public float Airspeed = 0.000f; // Airspeed [m/s]
    [System.NonSerialized] public float alpha = 0.000f; // Angle of attack [deg]
    [System.NonSerialized] public float beta = 0.000f; // Side slip angle [deg]
    [System.NonSerialized] public float de = 0.000f; // Elevator angle [deg]
    [System.NonSerialized] public float dr = 0.000f; // Rudder angle [deg]
    public float drRatio;
    [System.NonSerialized] public float dh = 0.000f; // Movement of c.g. [-]
    [System.NonSerialized] public float LocalGustMag = 0.000f; // Magnitude of local gust [m/s]
    [System.NonSerialized] public float LocalGustDirection = 0.000f; // Magnitude of local gust [m/s]
    [System.NonSerialized] public float nz = 0.000f; // Load factor [-]

    [System.NonSerialized] public float Groundspeed = 0.000f; // Groundspeed [m/s]
    [System.NonSerialized] public float ALT = 0.000f;

    //計算で用いるセンサー値

    // [System.NonSerialized] public float massLeft;//左ひずみの値[kg]
    [System.NonSerialized] public float massForward;//右ひずみの値[kg]
    [System.NonSerialized] public float massBackward;//後方左ひずみの値[kg]
    // [System.NonSerialized] public float massBackwardLeft;//後方右ひずみの値[kg]

    [System.NonSerialized] public float centerOfMass = 0.000f; // 全体重心計算結果[m] pitchGravity
    static public float centerOfMassPilotRaw = 0.2f; // 補正前重心計算結果[m] pitchGravityPilot
    [System.NonSerialized] public float centerOfMassPilot; // 補正済重心計算結果[m] 定常状態(pitchGravity=0)のパイロット重心 pitchGravityPilotS

    // GameManager.csへ移動
    //[System.NonSerialized] public float centerOfMassPilotOffset; // 重心位置のオフセット値[m]

    [System.NonSerialized] public float massLeftRightS;//定常状態の前センサーの値(合計値ではなく一つのセンサーの値)
    [System.NonSerialized] public float massBackwardS;//定常状態の後センサーの値(合計値ではなく一つのセンサーの値)

    // Phisics

    static protected float rho = 1.164f;
    static protected float hE0 = 10.500f; // Altitude at Take-off [m]

    // At Cruise without Ground Effect

    static protected float Airspeed0; // Magnitude of ground speed [m/s]
    static protected float alpha0; // Angle of attack [deg]
    static protected float CDp0; // Parasitic drag [-]
    static protected float Cmw0; // Pitching momentum [-]
    static protected float CLMAX; // Lift Coefficient [-]
    static protected float CL0 = 0.000f; // Lift Coefficient [-]
    static protected float CLw0 = 0.000f; // Lift Coefficient [-]
    static protected float CLt0 = 0.000f; // Tail Coefficient [-]
    static protected float epsilon0 = 0.000f; // Downwash

    // Plane

    static protected bool Downwash; // Conventional Tail: True, T-Tail: False
    static protected float CL = 0.000f; // Lift Coefficient [-]
    static protected float CD = 0.000f; // Drag Coefficient [-]
    static protected float Cx = 0.000f; // X Force Coefficient [-]
    static protected float Cy = 0.000f; // Y Force Coefficient [-]
    static protected float Cz = 0.000f; // Z Force Coefficient [-]
    static protected float Cl = 0.000f; // Rolling momentum [-]
    static protected float Cm = 0.000f; // Pitching momentum [-]
    static protected float Cn = 0.000f; // Yawing momentum [-]
    static protected float dh0 = 0.000f; // Initial Mouse Position

    // Wing

    static protected float Sw; // Wing area of wing [m^2]
    static protected float bw; // Wing span [m]
    static protected float cMAC; // Mean aerodynamic chord [m]
    static public float aw; // Wing Lift Slope [1/deg]

    static protected float ac;
    static protected float cg;

    static protected float hw; // Length between Wing a.c. and c.g. [-] ac-cg

    static protected float hw0;
    static protected float lt0;

    static protected float AR; // Aspect Ratio [-]
    static protected float ew; // Wing efficiency [-]
    static protected float CLw = 0.000f; // Lift Coefficient [-]

    // Tail

    static protected float St; // Wing area of tail [m^2]
    static protected float at; // Tail Lift Slope [1/deg]
    static protected float lt; // Length between Tail a.c. and c.g. [m]
    static protected float VH; // Tail Volume [-]
    static protected float deMAX; // Maximum elevator angle [deg]
    static protected float tau; // Control surface angle of attack effectiveness [-]
    static protected float CLt = 0.000f; // Lift Coefficient [-]

    // Fin

    static protected float drMAX; // Maximum rudder angle

    // Ground Effect

    static protected float CGEMIN; // Minimum Ground Effect Coefficient [-]
    static protected float CGE = 0f; // Ground Effect Coefficient: CDiGE/CDi [-]

    // Stability derivatives

    static protected float Cyb; // [1/deg]
    static protected float Cyp; // [1/rad]
    static protected float Cyr; // [1/rad]
    static protected float Cydr; // [1/deg]
    static protected float Cnb; // [1/deg]
    static protected float Cnp; // [1/rad]
    static protected float Cnr; // [1/rad]
    static protected float Cndr; // [1/deg]
    static protected float Clb; // [1/deg]
    static protected float Clp; // [1/rad]
    static protected float Clr; // [1/rad]
    static protected float Cldr; // [1/deg]

    // Gust

    static protected Vector3 Gust = Vector3.zero; // Gust [m/s]

    // Rotation

    static protected float phi; // ロール[deg]
    static protected float theta;  // ピッチ[deg]
    static protected float psi; // ヨー[deg]

    protected Rigidbody PlaneRigidbody;

    // ----- 設計値（重心センサーのキャリブレーションや慣性モーメントの算出に使用） -----
    // 全備

    static public float massDefault; // 設計上の全重量[kg]
    static public float centerOfMassDefault; // 設計上の全体重心位置[m]
    static public float IyyDefault; // 設計上のピッチ慣性モーメント[kg*m^2]

    // 空虚
    //static public float massAircraft; // 空虚の機体重量[kg] // 既出
    //static public float centerOfMassAircraft; // 空虚の機体重心位置[m] // 既出
    // パイロット
    static public float massPilotDefault; // 設計上のパイロット重量[kg]

    // ----------------------------------------------------------------------------

    //追加機体データ

    // GameManager.csに移動（DontDestroyであってほしい）
    //static public float gm.lengthForward = 0.660f;//フレーム前方(フレーム＋センサー部分)から桁(原点)位置[m]
    //static public float gm.lengthBackward = 0.330f;//フレーム後方(フレームの端)から桁(原点)位置[m]
    static public float centerOfMassAircraft;//機体のみ全重心(パイロットなし,ピッチのみ)[m]

    static public float massAircraft;//機体のみ全重量[kg]

    static public float massPilot;//設計上のパイロット重量[kg]

    //static protected float SensorPositionY = 0.645f;//桁中心から垂直に線を超音波センサーの位置までおろした時の線の長さ[m]
    //static protected float SensorPositionZ = 0.0f;//↑の到達点から超音波センサーまでの長さ[m]
    //static protected float AircraftHight = 0.74f;//プラホからコクピ下部までの長さ[m]

    static protected bool PlusData;//追加機体データが存在するか

    //計算結果データ

    static protected float hw2;//	主翼空力中心と全機重心の距離（cMACで無次元化）（再計算バージョン）

    //翼持ちデータ

    static protected float YMin;//翼持ちの最小荷重(機体のみ重量/2)
    static protected float YrMax;//右翼持ちの許容最大荷重
    static protected float YlMax;//左翼持ちの許容最大荷重
    static protected float YrMoment;//右翼持ち本人がかけるモーメント
    static protected float YlMoment;//左翼持ち本人がかけるモーメント

    static protected float YL;//機体中心から翼持ち棒までの長さ[m]


    public static GameObject Aircraft;
    static protected GameObject SensorPoint;
    
    protected bool AddTaleForce;


}
