using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Reflection;

[RequireComponent(typeof(AudioSource))]
public class Speaker : MonoBehaviour
{
    private float frequency = 0f;
    private float gain = 0f;
    private float increment;
    private float phase;
    private float sampling_frequency = 44100f;

    private AerodynamicParameters aero;//AerodynamicCalculatorスクリプトにアクセスするための変数
    private Rigidbody PlaneRigidbody;

    private bool spk_flag;
    private float currentTime = 0.0f;
    private float sound_duration = 0.0f;
    private float off_duration = 0.0f;
    private float speaker_last_change_time = 0f;
    private float interval = 0.0f;

    private AudioSource audioSource;

    void Start(){
        aero = GameManager.instance.aero;
        PlaneRigidbody = GameManager.instance.game.Plane.GetComponent<Rigidbody>();
        audioSource = this.GetComponent<AudioSource>();
        if (audioSource.clip == null)
        {
            audioSource.clip = Resources.Load<AudioClip>("Audio/SineWave_440Hz");
        }
        audioSource.loop = true;
        audioSource.spatialBlend = 0f;
        audioSource.Play();

        frequency = 1320f;
        gain = 0.01f*Config.AudioVolume;
        interval = 1f;
    }

    void Update(){
        if(audioSource == null)
        {
            audioSource = this.GetComponent<AudioSource>();
        }
        if (PlaneRigidbody == null)
        {
            PlaneRigidbody = GameManager.instance.game.Plane.GetComponent<Rigidbody>();
        }

        if (!audioSource.isPlaying)
        {
            audioSource.Play();
        }

        if(GameManager.instance.game.status == GameParameters.Status.Preparation){
            frequency = 440f;
            interval = 1.0f;
        }


        if (GameManager.instance.game.status == GameParameters.Status.Flight)
        {
            if (aero.TakeOff)
            {
                if (aero.Airspeed > 11f)
                {
                    frequency = 440f;
                }
                else if (aero.Airspeed > 9f)
                {
                    frequency = 880f;
                }
                else
                {
                    frequency = 1320f;
                }

                // if (aero.ALT > 1.5f)
                // {
                //     interval = 0.9f;
                // }
                // else if (aero.ALT > 0.3f)
                // {
                //     //interval = 0.001f*(float)Math.Round(125f + 675f * script.ALT, 0,  MidpointRounding.AwayFromZero);
                //     //interval = 0.001f * (float)Math.Round(125f + 450f * script.ALT, 0, MidpointRounding.AwayFromZero);
                //     // valueには0から1.5の値が入る
                //     interval = 0.001f * (float)Math.Round(125f + (1150f / 3f) * aero.ALT, 0, MidpointRounding.AwayFromZero);
                // }
                // else
                // {
                //     //interval = 0.001f*(float)Math.Round(125f + 675f * script.ALT, 0,  MidpointRounding.AwayFromZero);
                //     //interval = 0.001f * (float)Math.Round(125f + 450f * script.ALT, 0, MidpointRounding.AwayFromZero);
                //     // valueには0から1.5の値が入る
                //     interval = 0.001f * (float)Math.Round(125f + (1150f / 3f) * aero.ALT, 0, MidpointRounding.AwayFromZero);
                // }
            }
        }

        audioSource.pitch = frequency / 440f;

        currentTime += Time.unscaledDeltaTime;
        sound_duration = 0.3f;
        off_duration = 0.6f;

        if (!spk_flag && (currentTime - speaker_last_change_time) > off_duration)
        {
            gain = 0.01f * Config.AudioVolume;
            speaker_last_change_time = currentTime;
            spk_flag = true;
        }
        else if (spk_flag && (currentTime - speaker_last_change_time) > sound_duration)
        {
            gain = 0f;
            speaker_last_change_time = currentTime;
            spk_flag = false;
        }

        if (GameManager.instance.game.status == GameParameters.Status.Splashdown)
        {
            gain = 0f;
        }

        audioSource.volume = gain;
    }

    void FixedUpdate(){
            /*
            if(!onoff){
                if(frameNumber%(interval/0.02m) == 0)//interval[s]ごとにリストに追加
                {
                    gain = 0.1*Config.AudioVolume/50;
                    onoff = true;
                    frameNumber = 0;
                }
            }
            else{
                if(frameNumber%(0.1m/0.02m) == 0)//interval[s]ごとにリストに追加
                {
                    gain = 0;
                    onoff = false;
                    frameNumber = 0;
                }
            }
            */
            /*
            if(frameNumber%(interval/0.02m) == 0)//interval[s]ごとにリストに追加
            {
                if(onoff){
                    gain = 0;
                    onoff = false;
                }
                else
                {
                    gain = 0.1*Config.AudioVolume/50;
                    onoff = true;
                }
            }
            */
        }

	// void OnAudioFilterRead(float[] data, int channels)
	// {
	// 	increment = frequency * 2 * Math.PI / sampling_frequency;

	// 	for (var i = 0; i < data.Length; i = i + channels)
	// 	{
	// 		phase = phase + increment;
	// 		data[i] = (float)(gain*Math.Sin(phase));
	// 		if (channels == 2) data[i + 1] = data[i];
	// 		if (phase > 2 * Math.PI) phase = 0;
	// 	}
	// }
}
