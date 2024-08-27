using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeechTest : MonoBehaviour
{
    public void Response()
    {
        print("OnResponse! - ARE");
    }
    public void PartialResponse()
    {
        print("OnPartialResponse! - ARE");
    }
    public void ValidatePartialResponse()
    {
        print("OnValidatePartialResponse! - ARE");
    }

    public void Error()
    {
        print("OnError! - ARE");
    }
    public void Aborting()
    {
        print("OnAborting! - ARE");
    }
    public void Abortted()
    {
        print("OnAborted! - ARE");
    }
    public void RequestCompleted()
    {
        print("OnRequestCompleted! - ARE");
    }


    public void RequestOptionSetup()
    {
        print("OnRequestOptionSetup! - ADE");
    }

    public void RequestCreated()
    {
        print("OnRequestCreated! - ADE");
    }


    public void TimeOut()
    {
        print("Timeout! - ADE");
    }

    public void Inactivity()
    {
        print("Inactivity! - ADE");
    }
    public void Deactivation()
    {
        print("Deactivation! - ADE"); //not very likely -- we didn't deactivate anywhere in the code
    }
    public void MicDataSent()
    {
        print("MicDataSent! - ADE");
    }
    public void MinimalWakeThresholdHit()
    {
        print("MinimalWakeThresholdHit! - ADE");
    }

}
