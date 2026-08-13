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

    public float massForward;//右ひずみの値[kg]
    public float massBackward;//後方左ひずみの値[kg]
    public float centerOfMass = 1.000f; // 全体重心計算結果[m] pitchGravity
    public float centerOfMassPilotRaw = 0.2f; // 補正前重心計算結果[m] pitchGravityPilot　// `PilotPosition.cs`へ移動
    public float centerOfMassPilot; // 補正済重心計算結果[m] 定常状態(pitchGravity=0)のパイロット重心
    public float centerOfMassPilotOffset; // 重心位置のオフセット値[m]

    // Phisics

    public float rho = 1.164f;
    public float hE0 = 10.500f; // Altitude at Take-off [m]

    // At Cruise without Ground Effect

    public float Airspeed0; // Magnitude of ground speed [m/s]
    public float alpha0; // Angle of attack [deg]
    public float CDp0; // Parasitic drag [-]
    public float Cmw0; // Pitching momentum [-]
    public float CLMAX; // Lift Coefficient [-]
    public float CL0 = 0.000f; // Lift Coefficient [-]
    public float CLw0 = 0.000f; // Lift Coefficient [-]
    public float CLt0 = 0.000f; // Tail Coefficient [-]
    public float epsilon0 = 0.000f; // Downwash

    // Plane

    public bool Downwash; // Conventional Tail: True, T-Tail: False
    public float CL = 0.000f; // Lift Coefficient [-]
    public float CD = 0.000f; // Drag Coefficient [-]
    public float Cx = 0.000f; // X Force Coefficient [-]
    public float Cy = 0.000f; // Y Force Coefficient [-]
    public float Cz = 0.000f; // Z Force Coefficient [-]
    public float Cl = 0.000f; // Rolling momentum [-]
    public float Cm = 0.000f; // Pitching momentum [-]
    public float Cn = 0.000f; // Yawing momentum [-]
    public float dh0 = 0.000f; // Initial Mouse Position

    // Wing

    public float Sw; // Wing area of wing [m^2]
    public float bw; // Wing span [m]
    public float cMAC; // Mean aerodynamic chord [m]
    public float aw; // Wing Lift Slope [1/deg]

    public float ac;
    public float cg;

    public float hw; // Length between Wing a.c. and c.g. [-] ac-cg

    public float hw0;
    public float lt0;

    public float AR; // Aspect Ratio [-]
    public float ew; // Wing efficiency [-]
    public float CLw = 0.000f; // Lift Coefficient [-]

    // Tail

    public float St; // Wing area of tail [m^2]
    public float at; // Tail Lift Slope [1/deg]
    public float lt; // Length between Tail a.c. and c.g. [m]
    public float VH; // Tail Volume [-]
    public float deMAX; // Maximum elevator angle [deg]
    public float tau; // Control surface angle of attack effectiveness [-]
    public float CLt = 0.000f; // Lift Coefficient [-]

    // Fin

    public float drMAX; // Maximum rudder angle

    // Ground Effect

    public float CGEMIN; // Minimum Ground Effect Coefficient [-]
    public float CGE = 0f; // Ground Effect Coefficient: CDiGE/CDi [-]

    // Stability derivatives

    public float Cyb; // [1/deg]
    public float Cyp; // [1/rad]
    public float Cyr; // [1/rad]
    public float Cydr; // [1/deg]
    public float Cnb; // [1/deg]
    public float Cnp; // [1/rad]
    public float Cnr; // [1/rad]
    public float Cndr; // [1/deg]
    public float Clb; // [1/deg]
    public float Clp; // [1/rad]
    public float Clr; // [1/rad]
    public float Cldr; // [1/deg]

    // Gust

    public Vector3 Gust = Vector3.zero; // Gust [m/s]

    // Rotation

    public float phi; // ロール[deg]
    public float theta;  // ピッチ[deg]
    public float psi; // ヨー[deg]

    public Rigidbody PlaneRigidbody;

    // ----- 設計値（重心センサーのキャリブレーションや慣性モーメントの算出に使用） -----
    // 全備
    public float massDefault; // 設計上の全重量[kg]
    public float centerOfMassDefault; // 設計上の全体重心位置[m]
    // public float IyyDefault; // 設計上のピッチ慣性モーメント[kg*m^2] // AircraftDataから持ってこれる

    // 空虚
    // public float massAircraft; // 空虚の機体重量[kg] // 既出
    // public float centerOfMassAircraft; // 空虚の機体重心位置[m] // 既出
    // public float IyyAircraftOrigin; // 原点における空虚の慣性モーメント
    public float IyyAircraftCenterOfMass; // 重心位置における空虚の慣性モーメント

    // パイロット
    // public float centerOfMassPilotDefault; // 設計上のパイロット重心位置[m]
    public float massPilotDefault; // 設計上のパイロット重量[kg]

    // ----------------------------------------------------------------------------

    //追加機体データ

    // GameManager.csに移動（DontDestroyであってほしい）
    //public float gm.lengthForward = 0.660f;//フレーム前方(フレーム＋センサー部分)から桁(原点)位置[m]
    //public float gm.lengthBackward = 0.330f;//フレーム後方(フレームの端)から桁(原点)位置[m]
    public float centerOfMassAircraft;//機体のみ全重心(パイロットなし,ピッチのみ)[m]

    public float massAircraft;//機体のみ全重量[kg]

    public float massPilot;//設計上のパイロット重量[kg]

    //public float SensorPositionY = 0.645f;//桁中心から垂直に線を超音波センサーの位置までおろした時の線の長さ[m]
    //public float SensorPositionZ = 0.0f;//↑の到達点から超音波センサーまでの長さ[m]
    //public float AircraftHight = 0.74f;//プラホからコクピ下部までの長さ[m]

    public bool PlusData;//追加機体データが存在するか

    //計算結果データ

    public float hw2;//	主翼空力中心と全機重心の距離（cMACで無次元化）（再計算バージョン）

    //翼持ちデータ

    public float YMin;//翼持ちの最小荷重(機体のみ重量/2)
    public float YrMax;//右翼持ちの許容最大荷重
    public float YlMax;//左翼持ちの許容最大荷重
    public float YrMoment;//右翼持ち本人がかけるモーメント
    public float YlMoment;//左翼持ち本人がかけるモーメント

    public float YL;//機体中心から翼持ち棒までの長さ[m]


    public GameObject Aircraft;
    public GameObject SensorPoint;

    public bool AddTaleForce;

    public bool TakeOff;
}
