using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainScript : MonoBehaviour {

    public static SampleTriggerRegistration triggerRegistration;

    public Light directionalLight;
    public Camera camera;

    public Text text1;
    public Text text2;
    public Text text3;
    public Text text4;

    public AudioSource audioText1;
    public AudioSource audioText2;
    public AudioSource audioText3;
    public AudioSource audioText4;

    private int framecount;

    private SmoothingUtility _translationSmoothingUtility = new SmoothingUtility();
    private SmoothingUtility _rotationSmoothingUtility = new SmoothingUtility();

    private Texture2D screenShot;

    private bool taken = false;

    // Use this for initialization
    void Start ()
    {
        text2.enabled = false;
        text3.enabled = false;
        text4.enabled = false;
        triggerRegistration = GameObject.Find("Cube").GetComponent<SampleTriggerRegistration>();
    }
	
	// Update is called once per frame
	void Update () {
        framecount++;
        if (framecount == 300 && !taken)
        {
            text1.enabled = false;
            text2.enabled = true;
            audioText2.Play();
        }
        else if (framecount == 300 && taken)
        {
            text3.enabled = false;
            text4.enabled = true;
            audioText4.Play();
        }
        else if (framecount == 600 && !taken)
        {
            triggerRegistration.listening = true;
        }
        else if (framecount == 599 && taken)
        {
            Application.Quit();
        }

        else if (framecount > 600 && taken)
        {
            GameObject picture = GameObject.Find("Picture");
            text2.enabled = false;
            text3.enabled = true;
            audioText3.Play();
            Camera.main.transform.position = new Vector3(picture.transform.position.x + 2.3f, picture.transform.position.y + 3f, Camera.main.transform.position.z + 1f);

            framecount = 0;

        }
    }

    public void TakeScreenShot()
    {
        int resWidth = 475;
        int resHeight = 477;
        RenderTexture rt = new RenderTexture(resWidth, resHeight, 24);
        camera.targetTexture = rt;
        screenShot = new Texture2D(resWidth, resHeight, TextureFormat.RGB24, false);
        camera.Render();
        RenderTexture.active = rt;
        screenShot.ReadPixels(new Rect(0, 0, resWidth, resHeight), 0, 0);
        screenShot.Apply();
        camera.targetTexture = null;
        RenderTexture.active = null;
        Destroy(rt);

        string filename = "screen1.png";

        byte[] bytes = screenShot.EncodeToPNG();
        System.IO.File.WriteAllBytes(filename, bytes);

        Debug.Log(string.Format("Took screenshot to: {0}", filename));

        Sprite tempSprite = Sprite.Create(screenShot, new Rect(0, 0, resWidth, resHeight), new Vector2(0, 0));
        GameObject.Find("Picture").GetComponent<SpriteRenderer>().sprite = tempSprite;

        directionalLight.intensity = 5f;
        directionalLight.intensity = 3f;
        directionalLight.intensity = 1f;

        taken = true;

        triggerRegistration.listening = false;

        return;
    }
}
