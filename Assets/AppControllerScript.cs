using UnityEngine;
using System.Net.Sockets;
using System;
using System.Text;
using System.IO;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Threading;

public class AppControllerScript : MonoBehaviour {

    public static AppControllerScript AppController;
    
    

    public static float[] dollsPosition;

    public static VignetteState currentState;
    public static VignetteStory currentStory = VignetteStory.BREAKFAST;

    private bool collectingData;
    private bool activeTCPConnection;
    private static bool recordingState;
    private static bool loadNextScene = false;
    private static bool loadQuitApp = false;

    delegate string WorkInvoker();


    // Use this for initialization
    void Start()
    {
        collectingData = false;
        recordingState = false;
        currentState = VignetteState.INTRODUCTION;

    }


    // Update is called once per frame
    void Update()
    {
        //GetDollsPosition();
        switch (currentState)
        {
            case VignetteState.INTRODUCTION:
                
                break;
            case VignetteState.SETUP:
                // this is were we should ask children to set up the the furnitures for the next story
                break;
            case VignetteState.STORYSTEM:
                if (!recordingState)
                {
                    recordingState = true;
                    var comThread = new Thread(() => { Debug.Log("Sending: " + StartRecording()); });
                    comThread.Start();
                }
                break;
            case VignetteState.STORYDATA:
                break;
            case VignetteState.CHILDFEELING:
                break;
            case VignetteState.CHILDFEELINGDATA:
                break;
            case VignetteState.MUMFEELING:
                break;
            case VignetteState.MUMFEELINGDATA:
                break;
            case VignetteState.ENDOFSTORY:
                if (recordingState)
                {
                    recordingState = false;
                    var comThread = new Thread(() => { Debug.Log("Sending: " + StopRecording()); });
                    comThread.Start();
                }
                break;
            default:
                break;
        }
       
        if (loadNextScene)
        {
            loadNextScene = false;
            LoadNextScene();
        }

        if (loadQuitApp)
        {
            loadQuitApp = false;
            SceneManager.LoadScene(7, LoadSceneMode.Single);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            currentState = (VignetteState)(((int)currentState + 1) % 8);
            Debug.Log("Current state: " + currentState);
        }
        if (Input.GetKeyDown(KeyCode.V))
        {
            Debug.Log("Checking command...");
            Debug.Log("Message received: " + GetCommand());
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            //nextVignette();
            Debug.Log("Current state: " + currentState);
        }
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            switch (currentState)
            {
                case VignetteState.SETUP:
                    currentState = VignetteState.STORYSTEM;
                    break;
                case VignetteState.STORYDATA:
                    currentState = VignetteState.CHILDFEELING;
                    break;
                case VignetteState.CHILDFEELINGDATA:
                    currentState = VignetteState.MUMFEELING;
                    break;
                case VignetteState.MUMFEELINGDATA:
                    currentState = VignetteState.ENDOFSTORY;
                    break;
                default:
                    break;
            }
            // Notify the new state of the client app and the key that has been pressed
            new Thread(() => {
                NotifyKeyEvent(KeyCode.Alpha0);
                NotifyingVignetteState(currentState);
            }).Start();
        }
        if (Input.GetKeyDown(KeyCode.R)) {
            if (recordingState)
            {
                var comThread = new Thread(() => { Debug.Log("Sending: " + StopRecording()); });
                comThread.Start();
            }
            else
            {
                var comThread = new Thread(() => { Debug.Log("Sending: " + StartRecording()); });
                comThread.Start();
            }
            recordingState = !recordingState;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            StopRecording();
            Application.Quit();
        }

    }

    void OnApplicationQuit()
    {
        var comThread = new Thread(() => { NotifyQuitApp(); });
        comThread.Start();
    }

    public static string sendCommand(string command)
    {
        TcpClient client;
        Stream stream;
        string commandResult = string.Empty;
        Debug.Log("Sending command: " + command);
        try
        {
            client = new TcpClient("127.0.0.1", 11000);
            stream = client.GetStream();
            byte[] data = new byte[256];
            data = Encoding.ASCII.GetBytes(command);
            stream.Write(data, 0, data.Length);

            using (var ms = new MemoryStream())
            {
                byte[] buffer = new byte[2048]; // read in chunks of 2KB
                int bytesRead;
                while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, bytesRead);
                }
                byte[] result = ms.ToArray();
                commandResult = Encoding.ASCII.GetString(result);
            }

            stream.Close();
            client.Close();
        }
        catch (SocketException e)
        {
            Debug.LogError("Cannot connect to server. " + e.Message);
            commandResult = "error";
        }
        return commandResult;
    }

    

    public static VignetteStory nextVignette(VignetteStory s)
    {
        VignetteStory next = s;
        switch (s)
        {
            case VignetteStory.BREAKFAST:
                next = VignetteStory.NIGHTMARE;
                break;
            case VignetteStory.NIGHTMARE:
                next = VignetteStory.TUMMYACHE;
                break;
            case VignetteStory.TUMMYACHE:
                next = VignetteStory.HOPSCOTCH;
                break;
            case VignetteStory.HOPSCOTCH:
                next = VignetteStory.SHOPPING;
                break;
            case VignetteStory.SHOPPING:
                break;
            default:
                break;
        }

        return s;
    }

    public static void LoadNextScene()
    {
        switch (currentStory)
        {
            case VignetteStory.BREAKFAST:
                currentStory = VignetteStory.NIGHTMARE;
                SceneManager.LoadScene(getVignetteString(currentStory));
                break;
            case VignetteStory.NIGHTMARE:
                currentStory = VignetteStory.TUMMYACHE;
                SceneManager.LoadScene(getVignetteString(currentStory));
                break;
            case VignetteStory.TUMMYACHE:
                currentStory = VignetteStory.HOPSCOTCH;
                SceneManager.LoadScene(getVignetteString(currentStory));
                break;
            case VignetteStory.HOPSCOTCH:
                currentStory = VignetteStory.SHOPPING;
                SceneManager.LoadScene(getVignetteString(currentStory));
                break;
            case VignetteStory.SHOPPING:
                //currentStory = VignetteStory.BREAKFAST;
                break;
            default:
                break;
        }
        var comThread = new Thread(() => { NotifyStartVignette(currentStory); });
        comThread.Start();
    }

    public static void LoadNextState()
    {
         switch (currentState)
         {
             case VignetteState.INTRODUCTION:
                currentState = VignetteState.SETUP;
                 break;
            case VignetteState.SETUP:
                currentState = VignetteState.STORYSTEM;
                break;
             case VignetteState.STORYSTEM:
                currentState = VignetteState.STORYDATA;
                break;
             case VignetteState.STORYDATA:
                currentState = VignetteState.CHILDFEELING;
                break;
             case VignetteState.CHILDFEELING:
                currentState = VignetteState.CHILDFEELINGDATA;
                break;
             case VignetteState.CHILDFEELINGDATA:
                currentState = VignetteState.MUMFEELING;
                break;
             case VignetteState.MUMFEELING:
                currentState = VignetteState.MUMFEELINGDATA;
                break;
             case VignetteState.MUMFEELINGDATA:
                currentState = VignetteState.ENDOFSTORY;
                break;
             case VignetteState.ENDOFSTORY:
                 break;
             default:
                 break;
         }
        var comThread = new Thread(() => { NotifyingVignetteState(currentState); });
        comThread.Start();
    }

    public static string getVignetteString(VignetteStory s)
    {
        string name = "";

        switch(s) {
            case VignetteStory.BREAKFAST:
                name = "Breakfast";
                break;
            case VignetteStory.NIGHTMARE:
                name = "Nightmare";
                break;
            case VignetteStory.TUMMYACHE:
                name = "TummyAche";
                break;
            case VignetteStory.HOPSCOTCH:
                name = "Hopscotch";
                break;
            case VignetteStory.SHOPPING:
                name = "Shopping";
                break;
            default:
                break;
        }

        return name;
    }

    public static string getVignetteStateString(VignetteState s)
    {
        string state = string.Empty;
        switch(s)
        {
            case VignetteState.INTRODUCTION:
                state = "Introduction";
                break;
            case VignetteState.SETUP:
                state = "Mat_Setup";
                break;
            case VignetteState.STORYSTEM:
                state = "Story_Stem_Video";
                break;
            case VignetteState.STORYDATA:
                state = "Story_Data_Recording";
                break;
            case VignetteState.CHILDFEELING:
                state = "Child_Feelings_Video_Prompt";
                break;
            case VignetteState.CHILDFEELINGDATA:
                state = "Child_Feeling_Data_Recording";
                break;
            case VignetteState.MUMFEELING:
                state = "Mum_Feelings_Video_Prompt";
                break;
            case VignetteState.MUMFEELINGDATA:
                state = "Mum_Feelings_Data_Recording";
                break;
            case VignetteState.ENDOFSTORY:
                state = "End_Of_Story";
                break;
            default:
                break;
        }
        return state;
    }

    public static void WaitForEncodingAndLoadNextScene()
    {
        IAsyncResult result;
        WorkInvoker method = StopRecording;
        Debug.Log("Current story " + getVignetteString(currentStory));
        if (currentStory != VignetteStory.SHOPPING)
            result = method.BeginInvoke(Done, null);
        else result = method.BeginInvoke(GoToEnd, null);
    }

    private static void GoToEnd(IAsyncResult ar)
    {
        Debug.Log("Going to end.");
        loadQuitApp = true;
    }

    private static void Done(IAsyncResult result)
    {
        //Debug.Log("I am done.");
        //WorkInvoker method = (WorkInvoker)result.AsyncState;
        //string command = method.EndInvoke(result);
        //Debug.Log("Task is done. Command is: " + command);
        //if (command.Equals("Encoding success"))
        loadNextScene = true;
    }

    // Network messages *********************************************************************************

    // Get doll position using Real Sense. Not used anymore
    public void GetDollsPosition()
    {
        string result = sendCommand("get_dolls_position");
        string[] sdata = result.Split(',');
        for (int i = 0; i < sdata.Length; i++)
        {
            dollsPosition[i] = float.Parse(sdata[i]);
        }
        Console.WriteLine("dollX: " + dollsPosition[0]);
    }

    // ?
    public string GetCommand()
    {
        return sendCommand("get_command");
    }

    // Start collecting the data
    public static string StartRecording()
    {
        recordingState = true;
        return sendCommand("start_collecting");

    }

    // Stop collecting the data
    public static string StopRecording()
    {
        recordingState = false;
        return sendCommand("stop_collecting");
    }

    // Notifying the start of the app
    public static string NotifyStart()
    {
        return sendCommand("start_client");
    }

    // Notifying the start of a vignette
    public static string NotifyStartVignette(VignetteStory s)
    {
        return sendCommand("start_vignette " + getVignetteString(s));
    }

    // Notifying the state of a vignette
    public static string NotifyingVignetteState(VignetteState s)
    {
        return sendCommand("change_state " + getVignetteStateString(s));
    }

    // Notifying when a button has been pressed
    public static string NotifyKeyEvent(KeyCode key)
    {
        return sendCommand("key_press " + key.ToString());
    }

    // Notifying when the app is closing
    public static string NotifyQuitApp()
    {
        return sendCommand("quit");
    }

    public enum VignetteState { INTRODUCTION, SETUP, STORYSTEM, STORYDATA, CHILDFEELING, CHILDFEELINGDATA, MUMFEELING, MUMFEELINGDATA, ENDOFSTORY };
    public enum VignetteStory { BREAKFAST, NIGHTMARE, TUMMYACHE, HOPSCOTCH, SHOPPING};
}
    