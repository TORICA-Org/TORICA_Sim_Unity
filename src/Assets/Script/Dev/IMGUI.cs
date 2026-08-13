using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO; // ファイル入出力

public class IMGUI : MonoBehaviour
{
    [TextArea(3, 15)]
    [Tooltip("備忘録や仕様のメモなどを自由に書き込めます")]
    public string note = "IMGUIはデバッグ用のボタンを実装する仕組みです．";

    private GameManager gm = GameManager.instance;

    void Update()
    {
    }

    void OnGUI()
    {

        /*
        if (GUI.Button(new Rect(10, 10, 300, 150), "VRMode"))
        {
            gm.game.VRMode = !gm.game.VRMode;
        }

        if (GUI.Button(new Rect(10, 210, 300, 150), "CalibrateVR"))
        {
            //CameraManager.CalibrateVR();
        }

        if (GUI.Button(new Rect(10, 10, 100, 50), "Write CSV"))
        {
            using (CsvIO csv = new CsvIO(50, 20))
            {
                csv.Write(1, 1, "Hello");
                csv.Write(2, 1, "World");
                csv.Flush(Path.Combine(Directory.GetParent(Application.dataPath).FullName, "Test.csv"));
            }
        }

        if (GUI.Button(new Rect(10, 70, 100, 50), "Read CSV"))
        {
            using (CsvIO csv = new CsvIO(50, 20))
            {
                csv.Load(Path.Combine(Directory.GetParent(Application.dataPath).FullName, "Test.csv"));
                print(csv.Read(1, 1));
                print(csv.Read(2, 1));
            }
        }
        */
    }
}
