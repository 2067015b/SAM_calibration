using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TextScript : MonoBehaviour
{
    public Text Text1;
    public Text Text2;
    public Text Text3;
    public Text Text4;

    public AudioSource textAudio2;
    public AudioSource textAudio3;
    public AudioSource textAudio4;

    private int framecount;
    private int enabled;

    void Start()
    {
        Text2.enabled = false;
        Text3.enabled = false;
        Text4.enabled = false;
        framecount = 0;
        enabled = 1;
    }

    void Update()
    {
        framecount++;

        if (framecount>=400)
        {
            switch (enabled) {
                case 1:
                    Text1.enabled = false;
                    Text2.enabled = true;
                    textAudio2.Play();
                    framecount = 0;
                    enabled = 2;
                    break;
                case 2:
                    Text2.enabled = false;
                    Text3.enabled = true;
                    textAudio3.Play();
                    framecount = 0;
                    enabled = 3;
                    break;

                case 3:
                    Text3.enabled = false;
                    Text4.enabled = true;
                    textAudio4.Play();
                    framecount = 0;
                    enabled = 4;
                    break;
                case 4:
                    if (Input.GetKey("return"))
                    {
                        Application.LoadLevel(1);
                    }
                    break;
            }
        }
    }

}
