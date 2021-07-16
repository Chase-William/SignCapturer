using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit;
using System;
using UnityEngine;
using System.IO;
using System.Net.Http;

public class HandRecorder : MonoBehaviour
{
    public event Action RecorderIsReady;

    const string FILE_EXTENSION = ".csv";

    StreamWriter writer;
    IMixedRealityHand hand;
    TrackedHandJoint[] joints;
    FileStream fs;
    string filePath;    
    
    public bool IsRecording { get; private set; }

    string fileName;

    private string username;
    private char letter;

    private void Awake()
    {
        Debug.Log("Persistent Data Path" + Application.persistentDataPath);        
        hand = HandJointUtils.FindHand(Handedness.Right);        
    }  
    
    public void StartRecording(string _username, char _letter)
    {
        if (IsRecording)
            return;
        IsRecording = true;
        username = _username;
        letter = _letter;
    }

    public async void StopRecording()
    {
        IsRecording = false;        
        try
        {
            writer?.Flush();
            writer?.Close();
            writer?.Dispose();
            writer = null;

            // we need to send a request with multipart/form-data
            var multiForm = new MultipartFormDataContent();

            if (!File.Exists(filePath))
            {
                Debug.LogError("Hand log output file is not present when trying to retrieve for sending across network.");
                return;
            }

            // add file and directly upload it
            fs = File.OpenRead(filePath);            
            multiForm.Add(new StreamContent(fs), $"{fileName}", Path.GetFileName(filePath));
            multiForm.Headers.Add("fileName", fileName);
            using HttpClient client = new HttpClient();

            // send request to API
            var url = $"https://sheltered-peak-61041.herokuapp.com/save";
            var response = await client.PostAsync(url, multiForm);
            
        }    
        catch (Exception ex)
        {
            Debug.Log("StopRecording: " + ex);
        } 
        finally
        {
            RecorderIsReady?.Invoke();
            fs.Close();
            fs.Dispose();
        }
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
        writer.Write("\"Label\"" + "\r\n");
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
            SaveJoints();
    }    

    /// <summary>
    /// Records to file the vector3 coords of all the hand joints.
    /// </summary>
    private void SaveJoints()
    {
        try
        {
            if (writer == null)
            {
                fileName = ($"hand_log_{username}_{letter}" + FILE_EXTENSION);
                Debug.Log("FileName: " + fileName);


                filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), fileName);
                //filePath = Path.Combine("D:\\Dev\\data", fileName);

                Debug.Log("FilePath: " + filePath);
                if (!File.Exists(filePath))
                    File.Create(filePath);
                writer = new StreamWriter(filePath, false);
                WriteHeader();
            }

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
            int letterAsNum = (int)letter;
            for (int i = 1; i < joints.Length; i++)
            {
                if (HandJointUtils.TryGetJointPose(joints[i], Handedness.Right, out MixedRealityPose pose))
                {
                    if (i < stopComma)
                        writer.Write(GetPositionFormatted(pose.Position) + ",");
                    else
                        writer.Write(GetPositionFormatted(pose.Position) + "," + letterAsNum);
                }
                else
                    writer.Write("null, ");
            }
            writer.Write("\n");
        }        
        catch (Exception ex)
        {
            Debug.Log("SaveJoints: " + ex);
        }
    }

    private string GetPositionFormatted(in Vector3 vec)
        =>  $"{vec.x},{vec.y},{vec.z}";    
    
}