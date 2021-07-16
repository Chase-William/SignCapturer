using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleComponent : MonoBehaviour
{
    public GameObject Toggle_A;
    public GameObject Toggle_B;
    public GameObject Toggle_C;
    public GameObject Toggle_F;
    public GameObject Toggle_Q;

    public GameObject UserDannels;
    public GameObject UserAaron;
    public GameObject UserKelvin;
    public GameObject UserRoshan;
    public GameObject UserChase;

    public TextMesh WaitingForServer;

    public HandRecorder recorderer;

    private bool recording;
    public bool Recording
    {
        get => recording;
        private set
        {
            recording = value;
            textMesh.text = value ? $"Recording {User} | {Sign}" : "Start Recording";
        }
    }

    public string User
        => RadioBtn.ActiveInterable["user"]
            .gameObject.transform.GetChild(0).GetChild(5).gameObject.GetComponent<TextMesh>().text.ToLower();

    public char Sign
    {
       get
       {
            var letterTextMesh = RadioBtn.ActiveInterable["letter"]
                .gameObject.transform.GetChild(0).GetChild(4).gameObject.GetComponent<TextMesh>();
            return letterTextMesh.text[letterTextMesh.text.Length - 1];
       }
    }

    TextMesh textMesh;

    // Start is called before the first frame update
    void Start()
    {
        WaitingForServer.gameObject.SetActive(false);
        textMesh = this.gameObject.GetComponentInChildren<TextMesh>();        
        recorderer.RecorderIsReady += Recorderer_RecorderIsReady;
    }

    private void Recorderer_RecorderIsReady()
        => ShowUI();    

    public void ToggleRecording()
    {  
        if (!Recording)
            if (string.IsNullOrEmpty(User) || Sign == default)
            {
                // Error
                return;
            }
        Recording = !Recording;        
        if (Recording)
        {
            HideUI();
            recorderer.StartRecording(User, Sign);
        }            
        else
        {
            this.gameObject.SetActive(false);
            WaitingForServer.gameObject.SetActive(true);
            recorderer.StopRecording();            
        }
    }

    void HideUI()
    {
        Toggle_A.SetActive(false);
        Toggle_B.SetActive(false);
        Toggle_C.SetActive(false);
        Toggle_F.SetActive(false);
        Toggle_Q.SetActive(false);
        UserDannels.SetActive(false);
        UserAaron.SetActive(false);
        UserKelvin.SetActive(false);
        UserRoshan.SetActive(false);
        UserChase.SetActive(false);
    }

    void ShowUI()
    {
        WaitingForServer.gameObject.SetActive(false);
        Toggle_A.SetActive(true);
        Toggle_B.SetActive(true);
        Toggle_C.SetActive(true);
        Toggle_F.SetActive(true);
        Toggle_Q.SetActive(true);

        UserDannels.SetActive(true);
        UserAaron.SetActive(true);
        UserKelvin.SetActive(true);
        UserRoshan.SetActive(true);
        UserChase.SetActive(true);
        this.gameObject.SetActive(true);
    }

    void ToggleUIActivation()
    {
        var isActive = !Recording;

        Toggle_A.SetActive(isActive);
        Toggle_B.SetActive(isActive);
        Toggle_C.SetActive(isActive);
        Toggle_F.SetActive(isActive);
        Toggle_Q.SetActive(isActive);

        UserDannels.SetActive(isActive);
        UserAaron.SetActive(isActive);
        UserKelvin.SetActive(isActive);
        UserRoshan.SetActive(isActive);
        UserChase.SetActive(isActive);
    }
}
