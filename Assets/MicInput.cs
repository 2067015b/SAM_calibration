using UnityEngine;
using System;

public class MicInput : MonoBehaviour
{

    public static float MaxVolume;

    private string _device;


    void InitialiseMicrophone()
    {
        if (_device == null) _device = Microphone.devices[0];
        audioClip = Microphone.Start(_device, true, 999, 44100);
    }

    void StopMicrophone()
    {
        Microphone.End(_device);
    }


    AudioClip audioClip = new AudioClip();
    int sampleLength = 128;

    //Get data from microphone into audioclip.
    float inputVolume()
    {
        
            float maxVolume = 0;
            float[] waveData = new float[sampleLength];
            int micPosition = Microphone.GetPosition(null)-(sampleLength+1); 
            if (micPosition < 0) return 0;
            audioClip.GetData(waveData, micPosition);
            for (int i = 0; i < sampleLength; i++) {
                float wavePeak = waveData[i] * waveData[i];
                if (maxVolume < wavePeak) {
                    maxVolume = wavePeak;
                }
            }
            return maxVolume;
    }



    void Update()
    {
        MaxVolume = inputVolume();
    }

    bool initialized;
    // Initialise microphone when the scene starts.
    void OnEnable()
    {   
        InitialiseMicrophone();
        initialized = true;
    }

    //Disable the microphone when loading a new level or quit application.
    void OnDisable()
    {
        StopMicrophone();
    }

    void OnDestroy()
    {
        StopMicrophone();
    }


    // Initialise and disable the microphone on application focus.
    void OnApplicationFocus(bool focus)
    {
        if (focus)
        {

            if (!initialized)
            {
                InitialiseMicrophone();
                initialized = true;
                Console.WriteLine("InitialiseMicrophone");
            }
        }
        if (!focus)
        {
            StopMicrophone();
            initialized = false;

        }
    }
}

