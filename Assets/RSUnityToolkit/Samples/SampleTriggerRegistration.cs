/*******************************************************************************

INTEL CORPORATION PROPRIETARY INFORMATION
This software is supplied under the terms of a license agreement or nondisclosure
agreement with Intel Corporation and may not be copied or disclosed except in
accordance with the terms of that agreement
Copyright(c) 2012-2014 Intel Corporation. All Rights Reserved.

******************************************************************************/
using UnityEngine;
using System.Collections;
using RSUnityToolkit;

public class SampleTriggerRegistration : MonoBehaviour {

    /* This MonoBehaviour class shows how to use the Send Message Action.
	 * 
	 * In here you will find 2 methods. OnTrigger(Trigger trigger) and OnTrackTrigger(TrackTrigger trigger).
	 * In the Send Message Action you can specify the function name. you can use either of those functions. 
	 * Unity will send the Trigger as the attribute to the function and in here we will cast it (if needed) and display it in the OnGUI method.
	 *
	 */

    // Use this for initialization
    public bool listening;
    public static MainScript main;

	void Start () {
        listening = false;
        main = GameObject.Find("EventSystem").GetComponent<MainScript>();

    }
	
	private TrackTrigger _trigger;
			
	void OnTrigger(Trigger trigger)
	{
		if (trigger is TrackTrigger)
		{
			_trigger = (TrackTrigger)trigger;
            if (    _trigger.Position.x>=0.45f && _trigger.Position.x <= 0.55f 
                    && _trigger.Position.y >= 0.42f && _trigger.Position.y <= 0.55f
                    && _trigger.Position.z >= 0.25f && _trigger.Position.z <= 0.60f)
            {
                TakeScreenshot();
            }
		}
		else 
		{
			Debug.LogError("We assume we suppose to get TrackTrigger in this example. It seems like we didn't");
		}
	}
	
	void OnTrackTrigger(TrackTrigger trigger)
	{
		_trigger = trigger;
	}

    private void TakeScreenshot()
    {
        if (listening)
        {
            main.TakeScreenShot();
        }
    }

}
