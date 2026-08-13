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
public class MakeChart1 : MakeCharts
{
    protected void OnEnable()
    {
        DataN = 2;
    }

    protected override void AddData()
    {   
        chart.AddXAxisData(i*airdata.interval  +"s");

        chart.AddData(0, GameManager.instance.game.ThetaList[i]);
        chart.AddData(1, GameManager.instance.game.AlphaList[i]);

        i++;
        //Debug.Log("d");
    }
    
    protected override void SetAxis()
    {
            for(int e = 0;e < DataN;e++){
            var Chart = chart.AddSerie<Line>("Theta");
            chart.AddSerie<Line>("Alpha");
            if (e==0||e==1){
                Chart.yAxisIndex = 0;
            }
            else{
                Chart.yAxisIndex = 1;
            }
        }
    }
}
