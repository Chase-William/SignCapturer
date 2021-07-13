using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit;
using System;
using UnityEngine;
using System.IO;
using System.Net.Http;

public class HandRecorder : MonoBehaviour
{
    StreamWriter writer;

    IMixedRealityHand hand;

    TrackedHandJoint[] joints;

    public bool IsRecording { get; set; }

    private string filePath;   

    private void Awake()
    {
        Debug.Log("Persistent Data Path" + Application.persistentDataPath);
        filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\hand_log.csv");
        //FileStream fs;
        //if (!File.Exists(path))
        //{
        //    Debug.LogWarning("Creating File that needs to exist");
        //    fs = File.Create(path);
        //}
        //else
        //{
        //    fs = File.OpenWrite(path);
        //}
        if (!File.Exists(filePath))
            File.Create(filePath);
        hand = HandJointUtils.FindHand(Handedness.Right);        
    }

    private void WriteHeader()
    {
        string header = "Wrist,Palm,ThumbMetacarpalJoint,ThumbProximalJoint,ThumbDistalJoint,ThumbTip,IndexMetacarpal,IndexKnuckle,IndexMiddleJoint,IndexDistalJoint,IndexTip,MiddleMetacarpal,MiddleKnuckle,MiddleMiddleJoint,MiddleDistalJoint,MiddleTip,RingMetacarpal,RingKnuckle,RingMiddleJoint,RingDistalJoint,RingTip,PinkyMetacarpal,PinkyKnuckle,PinkyMiddleJoint,PinkyDistalJoint,PinkyTip";
        string[] headers = header.Split(',');
        string targetHeader;
        for (int i = 0; i < headers.Length; i++)
        {
            targetHeader = headers[i];
            writer.Write(targetHeader + "_x," + targetHeader + "_y," + targetHeader + "_z,");
        }
        writer.Write("\"Label\""+ "\r\n");
    }

    public async void ToggleRecording()
    {        
        IsRecording = !IsRecording;
        if (!IsRecording)
        {
            writer?.Close();
            writer?.Dispose();

            // we need to send a request with multipart/form-data
            var multiForm = new MultipartFormDataContent();

            if (!File.Exists(filePath))
            {
                Debug.LogError("Hand log output file is not present when trying to retrieve for sending across network.");
                return;
            }

            // add file and directly upload it
            FileStream fs = File.OpenRead(filePath);
            multiForm.Add(new StreamContent(fs), "hand_log.csv", Path.GetFileName(filePath));

            using HttpClient client = new HttpClient();

            // send request to API
            var url = "https://sheltered-peak-61041.herokuapp.com/upload";
            var response = await client.PostAsync(url, multiForm);
        }
        else
        {
            writer = new StreamWriter(filePath, false);
            WriteHeader();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        //thumbObject = Instantiate(sphereMarker, this.transform);
        joints = (TrackedHandJoint[])Enum.GetValues(typeof(TrackedHandJoint));
    }

    // Update is called once per frame
    void Update()
    {
        if (IsRecording)
            Record();
    }    

    /// <summary>
    /// Records to file the vector3 coords of all the hand joints.
    /// </summary>
    private void Record()
    {
        Debug.Log("[Update->Record] Recording Joints...");
        if (hand == null)
        {
            hand = HandJointUtils.FindHand(Handedness.Right);
            Debug.Log("IMixedRealityHand is null");
            if (hand == null)
                return;
        }

        if (!hand.IsPositionAvailable)
        {
            Debug.Log("IMixedRealityHand isn't providing position data, skipped update");
            return;
        }
        else if (hand.TrackingState != TrackingState.Tracked)
        {
            Debug.Log("Cannot track hand");
            return;
        }
        else if (!hand.Enabled)
        {
            Debug.Log("not enabled");
            return;
        }

        //System.Random rnd = new System.Random();
        TrackedHandJoint[] joints = (TrackedHandJoint[])Enum.GetValues(typeof(TrackedHandJoint));
        int stopComma = joints.Length - 1;
        for (int i = 1; i < joints.Length; i++)
        {
            if (HandJointUtils.TryGetJointPose(joints[i], Handedness.Right, out MixedRealityPose pose))
            {
                if (i < stopComma)
                    writer.Write(GetPositionFormatted(pose.Position) + ",");
                else
                    writer.Write(GetPositionFormatted(pose.Position) + "," + 1);
            }
            else
                writer.Write("null, ");
        }
        writer.Write("\n");
    }

    private string GetPositionFormatted(in Vector3 vec)
        =>  $"{vec.x},{vec.y},{vec.z}";    
    
}