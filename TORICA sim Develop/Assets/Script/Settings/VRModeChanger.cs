using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRModeChanger : MonoBehaviour
{
    void Start(){
        #if USE_STEAMVR
        bool VREnable = true;
        // ここにVR用のコントローラー設定などのコードを記述
        #else
        // USE_STEAMVR シンボルが定義されていない（＝SteamVRが無効な）場合の処理
        MyGameManeger.instance.VRMode = false;
        this.gameObject.SetActive(false);
        // ここにPC用のカメラ設定や入力設定のコードを記述
        #endif
    }
    public void ChangeVRMode()
    {
        MyGameManeger.instance.VRMode = !MyGameManeger.instance.VRMode;
    }
}
