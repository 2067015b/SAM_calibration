using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class kittenEvents : MonoBehaviour {

    public Text text1;
    public Text text2;
    public Text text3;
    public Text text4;
    public Text text5;

    public AudioSource text1audio;
    public AudioSource text2audio;
    public AudioSource text3audio;
    public AudioSource text4audio;
    public AudioSource text5audio;

    public AudioSource meow;

    public GameObject boy;

    private int counter = 0;
    private int counter2 = 0;
    private bool meowed = false;
    private bool found = false;
    private bool loadNewLevel = false;
    private bool microphone = false;
    private string device;

    

	// Use this for initialization
	void Start () {
        text1.enabled = true;
        text2.enabled = false;
        text3.enabled = false;
        text4.enabled = false;
        text5.enabled = false;
        GetComponent<Animation>()["Run"].layer = 1;
        GetComponent<Animation>()["Run"].wrapMode = WrapMode.Once;
        GetComponent<Animation>()["Meow"].layer = 1;
        GetComponent<Animation>()["Meow"].wrapMode = WrapMode.Once;
    }
	
	// Update is called once per frame
	void Update () {

        counter++;
        if (loadNewLevel)
        {
            counter2++;
        }
        if (counter == 250 && !meowed) { 
            text1.enabled = false;
            text2.enabled = true;
            text2audio.Play();
        }
        else if (counter == 500) { 
            text2.enabled = false;
            text3.enabled = true;
            text3audio.Play();
        }

        else if (!meowed && counter > 500)
        {
            if (motionCalibrated())
            {
                text3.enabled = false;
                text4.enabled = true;
                text4audio.Play();
            }
        }
        else if (!found && meowed && counter > 1350){
            if (micCalibrated())
            {
                text4.enabled = false;
                text5.enabled = true;
                text5audio.Play();


                transform.LookAt(boy.transform);

                GetComponent<Animation>().Play("Run");
                float step = 16.0f * Time.deltaTime;
                transform.position = Vector3.MoveTowards(transform.position, boy.transform.position, step);

                if (transform.position.x >= (boy.transform.position.x-7))
                {
                    found = true;
                    loadNewLevel = true;
                }
            }
        }
        else if (found && meowed && counter2 > 250)
        {
            Application.LoadLevel(2);
        }
        else if (!found && meowed && counter2 > 500)
        {
            Application.LoadLevel(3);
        }
    }


    bool motionCalibrated()
    {
        if (AppControllerScript.sendCommand("is_dolla_connected") == "true" && AppControllerScript.sendCommand("is_dollb_connected") == "true")
        {
            if (counter > 1200)
            {
                meow.Play();
                GetComponent<Animation>().Play("Meow");
                meowed = true;
                return true;
            }
            return false;
        }
        else
        {
            Application.LoadLevel(3);
            return false;
        }
    }

    bool micCalibrated()
    {
        if (!microphone)
        {
            microphone = (MicInput.MaxVolume > 0.1f);
        }
        return microphone;
    }
}
