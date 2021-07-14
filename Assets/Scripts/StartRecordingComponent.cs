using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartRecordingComponent : MonoBehaviour
{
    public StopRecordingComponent stopComp;
    public Text statusText;
    public HandRecorder recorder;

    private void Awake()
    {
        recorder.IsWaitingForWebRequest += Recorder_IsWaitingForWebRequest;
    }

    private void Recorder_IsWaitingForWebRequest(object sender, bool e)
    {
        this.enabled = !e;
    }

    public void StartRecording()
    {        
        stopComp.StartedRecording();
        statusText.text = "Recording: On";
        statusText.color = Color.green;
    }    

    public void StoppedRecording()
    {
        
    }

    private void OnDestroy()
    {
        recorder.IsWaitingForWebRequest -= Recorder_IsWaitingForWebRequest;
    }
}
