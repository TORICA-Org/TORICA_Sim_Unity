using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;


public class ChangeAircraft : MonoBehaviour
{
    private bool sameLoad;

    private GameObject LoadingTextObj;
    private TMP_Text LoadingText;

    private Dropdown AircraftDd;
    string selectedvaluePre;

    public void OnEnables()
    {
        List<string> AircraftList = new List<string>();

        //Optionsに表示する文字列をリストに追加
        AircraftList.Add("Tatsumi");
        AircraftList.Add("Ray");
        AircraftList.Add("Mio");
        AircraftList.Add("QX-18");
        AircraftList.Add("QX-19");
        AircraftList.Add("QX-20");
        AircraftList.Add("ARG-2");
        AircraftList.Add("ORCA18");
        AircraftList.Add("UL01B");
        AircraftList.Add("ORCA18");
        AircraftList.Add("ORCA22");
        AircraftList.Add("Gardenia");
        AircraftList.Add("Aria");
        AircraftList.Add("Camellia");

        //DropdownコンポーネントのOptionsという項目にOptionsのリストがありました
        //それを編集するためにDropdownコンポーネントを取得
        AircraftDd = GetComponent<Dropdown>();

        //一度すべてのOptionsをクリア
        AircraftDd.ClearOptions();

        //リストを追加
        AircraftDd.AddOptions(AircraftList);

        //if(GameManager.instance.game.PlaneName == null){
        //    GameManager.instance.game.PlaneName=DefaultPlane;
        //}
        if(GameManager.instance.game.PlaneName == GameManager.instance.game.DefaultPlane){
            sameLoad=true;
        }

        AircraftDd.value = AircraftList.IndexOf(GameManager.instance.game.PlaneName);
    }

    private void Start()
    {
        LoadingTextObj = GameObject.Find("LoadingText");
        LoadingText = LoadingTextObj.GetComponent<TMP_Text>();
        LoadingText.text = "";
    }

    private void Update()
    {
        if (LoadingTextObj == null)
        {
            LoadingTextObj = GameObject.Find("LoadingText");
        }
        if (LoadingTextObj != null && LoadingText == null)
        {
            LoadingText = LoadingTextObj.GetComponent<TMP_Text>();
        }

        //DropdownコンポーネントをGet
        if (AircraftDd == null)
        {
            AircraftDd = GetComponent<Dropdown>();
        }

        //Dropdownコンポーネントから選択されている文字を取得
        string selectedvalue = AircraftDd.options[AircraftDd.value].text;
        if (selectedvalue != selectedvaluePre && GameManager.instance.game.PlaneName != selectedvalue)
        {
            LoadingText.text = $"Loading {selectedvalue}...";
            Debug.Log($"Loading {selectedvalue}...");
            GameManager.instance.game.PlaneName = selectedvalue;
            ModelInstantiater.InstantiateModel(GameManager.instance.game.PlaneName);
            SceneManager.LoadScene("FlightScene");
            selectedvaluePre = selectedvalue;
        }
    }
}
