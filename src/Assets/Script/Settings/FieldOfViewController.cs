using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FieldOfViewController : MonoBehaviour
{
    [SerializeField]private Text scoreText;
    private Slider CurrentSlider;
    //public static float Save=90;
    // Start is called before the first frame update
    void Start()
    {
        CurrentSlider = GetComponent<Slider>();

        CurrentSlider.value = GameManager.instance.game.FieldOfView;
    }

    public void Method()
    {
        FieldOfViewSetter.MyCamera.fieldOfView = CurrentSlider.value ;
        scoreText.text = CurrentSlider.value.ToString();
        GameManager.instance.game.SettingChanged = true;
        GameManager.instance.game.FieldOfView = CurrentSlider.value;
    }
}
