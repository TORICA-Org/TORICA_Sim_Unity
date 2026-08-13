/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO; // ファイル入出力

// これは，CustomPlaneData.csv の初回生成用スクリプトです.

public class CsvData
{

// ===== 逐語的文字列リテラルで長文の埋め込み =====
private string CustomPlaneData = @"Enable CustomPlaneData (true/false),false,,,,,,,,
// Plane,,,,,,,,,
mass,100,,,,// Ground Effect,,,,
centerOfMass,0,0.053,0,,CGEMIN,0.23,,,
inertiaTensor,993.022,1028.254,50.789,,,,,,
inertiaTensorRotation,-5.102,,,,// Stability derivatives,,,wing,fin
// Specification At Cruise without Ground Effect,,,,,Cyb,-0.005465,,-0.003331,-0.002134
Airspeed0,10.5,,,,Cyp,-0.541162,,-0.536174,-0.004988
alpha0,0,,,,Cyr,0.170132,,0.135214,0.034918
CDp0,0.015,,,,Cydr,0.002134,,,0.002134
Cmw0,-0.132,,,,Clb,-0.004723,,-0.004679,-0.000044
CLMAX,1.4,,,,Clp,-0.794553,,-0.794451,-0.000102
// Wing,,,,,Clr,0.20325,,0.202538,0.000712
Sw,17.92,,,,Cldr,0.000044,,,0.000044
bw,24.514,,,,Cnb,-0.000285,,-0.00059,0.000305
cMAC,0.812,,,,Cnp,-0.100557,,-0.101269,0.000712
aw,0.103,,,,Cnr,-0.007178,,-0.002193,-0.004985
hw,0.1,,,,Cndr,-0.000305,,,-0.000305
ew,0.987,,,,,,,,
AR,33.534,,,,//追加機体データ,tatsumiフレーム,rayフレーム,,
// Tail,,,,前センサーから吊り具(桁中心)までの長さ[m],lengthForward,0.865,0.66,,0.92
Downwash,true,,,吊り具(桁中心)から後センサーまでの長さ[m],lengthBackward,0.435,0.58,,0.27
St,1.736,,,機体のみ重心位置[m],aircraftCenterOfMass,-0.225,,,
at,0.079,,,機体のみ質量[kg],aircraftMass,48,,,
lt,2.9,,,機体中心からハードポイントの位置[m],YL,2.8,,,
deMAX,10,,,,,,,,
tau,1,,,,,,,,
VH,0.346,,,,,,,,
bt,3,,,,,,,,
,,,,,,,,,
// Fin (Extra),,,,,,,,,
Sf,0.63,,,,,,,,
bf,1.25,,,,,,,,
af,0.061,,,,,,,,
lf,3.5,,,,,,,,
tau,1,,,,,,,,
zf,0.5,,,,,,,,
VF,0.005019455,,,,,,,,
drMAX,10,,,,,,,,";
// ================================================


    private string path = Path.Combine(Directory.GetParent(Application.dataPath).FullName, "CustomPlaneData.csv");

    public void MakeCustomPlaneDataFile()
    {
        if (!File.Exists(path))
        {
            File.WriteAllText(path, CustomPlaneData);
        }
        else
        {
            Debug.LogWarning("File already exists: " + path);
        }
    }
}

*/