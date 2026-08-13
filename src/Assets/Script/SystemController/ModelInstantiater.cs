using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelInstantiater : MonoBehaviour
{
    public void OnEnables() // StartMethod.csにより呼び出し
    {
        if (GameManager.instance.game.PlaneName == null)
        {
            GameManager.instance.game.PlaneName = GameManager.instance.game.DefaultPlane;
        }

        InstantiateModel(GameManager.instance.game.PlaneName);
        Debug.Log($"Instantiated Aircraft: {GameManager.instance.game.PlaneName}");
    }

    static public void InstantiateModel(string planeName)
    {
        GameObject PlaneObj = (GameObject)Resources.Load(planeName);
        var obj = Instantiate(PlaneObj, new Vector3(PlaneObj.transform.position.x, PlaneObj.transform.position.y, PlaneObj.transform.position.z), Quaternion.identity);
        obj.name = PlaneObj.name;
        GameManager.instance.game.Plane = GameObject.Find(GameManager.instance.game.PlaneName);
    }
}
