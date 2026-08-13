using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AutoFactorSetter : MonoBehaviour
{
    private float DefaultPitchGravity;
    private AerodynamicCalculator script;
    [SerializeField] private InputField inputField;//ポート番号入力フィールド

    private GameManager gm = GameManager.instance;

    // Start is called before the first frame update
    private void Start()
    {
    }

    public void OnPush()
    {
        gm.pilot.Reset();
    }
}
