using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundVolumeController : MonoBehaviour
{
    [SerializeField]private Text scoreText;
    private Slider CurrentSlider;
    // Start is called before the first frame update
    void Start()
    {
        CurrentSlider = GetComponent<Slider>();

        CurrentSlider.value = Config.AudioVolume;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Method()
    {
        scoreText.text = CurrentSlider.value.ToString();
        GameManager.instance.game.SettingChanged = true;
        Config.AudioVolume = CurrentSlider.value;
    }
}
