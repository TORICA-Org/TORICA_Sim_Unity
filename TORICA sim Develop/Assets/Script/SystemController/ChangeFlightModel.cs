using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ChangeFlightModel : MonoBehaviour
{
    private bool s=false;
    private bool sameLoad;
    
    // Start is called before the first frame update
    public void OnEnables()
    {
        Dropdown FlightModelDdtmp; /*  */
        List<string> FlightModelList = new List<string>();

        //Optionsに表示する文字列をリストに追加
        //
        FlightModelList.Add("isoSim1");
        FlightModelList.Add("isoSim2");

        //DropdownコンポーネントのOptionsという項目にOptionsのリストがありました
        //それを編集するためにDropdownコンポーネントを取得
        FlightModelDdtmp = GetComponent<Dropdown>();

        //一度すべてのOptionsをクリア
        FlightModelDdtmp.ClearOptions();

        //リストを追加
        FlightModelDdtmp.AddOptions(FlightModelList);
        s=false;

        //if(MyGameManeger.instance.PlaneName == null){
        //    MyGameManeger.instance.PlaneName=DefaultPlane;
        //}
        if(MyGameManeger.instance.FlightModel == MyGameManeger.instance.DefaultFlightModel){
            sameLoad=true;
        }

        FlightModelDdtmp.value = FlightModelList.IndexOf(MyGameManeger.instance.FlightModel);
    }
    
    public void OnSelected()
    {
        if(s || MyGameManeger.instance.FirstLoad || sameLoad){
            Dropdown FlightModelDdtmp;

            //DropdownコンポーネントをGet
            FlightModelDdtmp = GetComponent<Dropdown>();

            //Dropdownコンポーネントから選択されている文字を取得
            string selectedvalue = FlightModelDdtmp.options[FlightModelDdtmp.value].text;

            MyGameManeger.instance.FlightModel = selectedvalue;

            Time.timeScale=1f;
            SceneManager.LoadScene("FlightScene");
        }else{
            s=true;
        }

    }
}
