using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StopRecordingComponent : MonoBehaviour
{
    public StartRecordingComponent startComp;
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

    public void StartedRecording()
    {

    }     

    public void StopRecording()
    {
        startComp.StoppedRecording();
        statusText.text = "Recording: Off";
        statusText.color = Color.red;
    }

    private void OnDestroy()
    {
        recorder.IsWaitingForWebRequest -= Recorder_IsWaitingForWebRequest;
    }
}
