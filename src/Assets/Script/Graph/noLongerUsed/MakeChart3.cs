using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
#if INPUT_SYSTEM_ENABLED
using Input = XCharts.Runtime.InputHelper;
#endif
using XCharts.Runtime;

[DisallowMultipleComponent]
[ExecuteInEditMode]
public class MakeChart3 : MakeCharts
{
    protected void OnEnable()
    {
        DataN = 2;
    }

    protected override void AddData()
    {
        try{
            if(GameManager.instance.game.AirspeedList.Count >= i && GameManager.instance.game.AltList.Count >= i)
            {
                chart.AddXAxisData(i*airdata.interval  +"s");
                chart.AddData(0, GameManager.instance.game.AirspeedList[i]);
                chart.AddData(1, GameManager.instance.game.AltList[i]);
                //chart.AddData(0, GameManager.instance.game.PhiList[i]);
                //chart.AddData(1, GameManager.instance.game.BetaList[i]);
                i++;
            }
        }
        catch(System.Exception){

        }
    }
    
    protected override void SetAxis(){
            for(int e = 0;e < DataN;e++){
            var Chart = chart.AddSerie<Line>();
            if(e==0||e==1){
                Chart.yAxisIndex = 0;
            }
            else{
                Chart.yAxisIndex = 1;
            }
        }
    }
}
