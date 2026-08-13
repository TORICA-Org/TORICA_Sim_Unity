using UnityEngine;

public class CameraFollowTarget : MonoBehaviour
{
    private GameObject target;

    private Vector3 offset;

    // Start is called before the first frame update
    void Start()
    {
        target = GameManager.instance.aero.Aircraft;
        // ゲーム開始時点のカメラとターゲットの距離（オフセット）を取得
        offset = gameObject.transform.position - target.transform.position;
    }

    /// <summary>
    /// プレイヤーが移動した後にカメラが移動するようにするためにLateUpdateにする。
    /// </summary>
    void LateUpdate()
    {
        // カメラの位置をターゲットの位置にオフセットを足した場所にする。
        gameObject.transform.position = target.transform.position + offset;
    }
}