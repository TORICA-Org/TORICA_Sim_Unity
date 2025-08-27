using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class Speaker : MonoBehaviour
{
    private double frequency = 0;
    private double gain = 0;
    private double increment;
    private double phase;
    private double sampling_frequency = 48000;

    private AerodynamicCalculator script;//AerodynamicCalculatorスクリプトにアクセスするための変数
    private Rigidbody PlaneRigidbody;

    private bool spk_flag;
    private float currentTime;
    private float sound_duration = 0.1f;
    private float off_duration;
    private float speaker_last_change_time = 0f;
    private float interval;

    void Start(){
        script = MyGameManeger.instance.Plane.GetComponent<AerodynamicCalculator>();
        PlaneRigidbody = MyGameManeger.instance.Plane.GetComponent<Rigidbody>();

        frequency = 1320;
        gain = 0.1*MyGameManeger.instance.SoundBolume/50;
        interval = 1f;
    }

    void Update(){
        currentTime += Time.deltaTime;
        off_duration = interval - sound_duration;

        if(!spk_flag  && (currentTime - speaker_last_change_time) > off_duration){
            gain = 0.1f*MyGameManeger.instance.SoundBolume/50;
            speaker_last_change_time = currentTime;
            spk_flag  = true;
        }else if(spk_flag  && (currentTime - speaker_last_change_time) > sound_duration){
            gain = 0;
            speaker_last_change_time = currentTime;
            spk_flag = false;
        }
        
        if(!MyGameManeger.instance.EnterFlight){
            gain = 0.1*MyGameManeger.instance.SoundBolume/50;
        }

        if(MyGameManeger.instance.SettingActive){
            gain = 0;
        }


        if(MyGameManeger.instance.EnterFlight){
            if(script.Airspeed > 10.8f){
                frequency = 440;
            }
            else if(script.Airspeed > 9.5f){
                frequency = 880;
            }
            else{
                frequency = 1320;
            }

            if(!MyGameManeger.instance.TakeOff){
                interval = 1.0f;
            }
            else if(script.ALT > 1.5f){
                interval = 0.9f;
            }
            else if(script.ALT > 0.3f){
                //interval = 0.001f*(float)Math.Round(125f + 675f * script.ALT, 0,  MidpointRounding.AwayFromZero);
                //interval = 0.001f * (float)Math.Round(125f + 450f * script.ALT, 0, MidpointRounding.AwayFromZero);
                // valueには0から1.5の値が入る
                interval = 0.001f * (float)Math.Round(125f + (1150f / 3f) * script.ALT, 0, MidpointRounding.AwayFromZero);
            }
            else {
                //interval = 0.001f*(float)Math.Round(125f + 675f * script.ALT, 0,  MidpointRounding.AwayFromZero);
                //interval = 0.001f * (float)Math.Round(125f + 450f * script.ALT, 0, MidpointRounding.AwayFromZero);
                // valueには0から1.5の値が入る
                interval = 0.001f * (float)Math.Round(125f + (1150f / 3f) * script.ALT, 0, MidpointRounding.AwayFromZero);
            }
        }
    }

    void FixedUpdate(){
            /*
            if(!onoff){
                if(frameNumber%(interval/0.02m) == 0)//interval[s]ごとにリストに追加
                {
                    gain = 0.1*MyGameManeger.instance.SoundBolume/50;
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
                    gain = 0.1*MyGameManeger.instance.SoundBolume/50;
                    onoff = true;
                }
            }
            */
        }

	void OnAudioFilterRead(float[] data, int channels)
	{
		increment = frequency * 2 * Math.PI / sampling_frequency;

		for (var i = 0; i < data.Length; i = i + channels)
		{
			phase = phase + increment;
			data[i] = (float)(gain*Math.Sin(phase));
			if (channels == 2) data[i + 1] = data[i];
			if (phase > 2 * Math.PI) phase = 0;
		}
	}
}