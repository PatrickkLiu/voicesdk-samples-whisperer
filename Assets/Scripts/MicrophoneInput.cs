using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MicrophoneInput : MonoBehaviour
{
    public AudioSource source;
    public int buffer = 4096;
    private string mic;

    // Start is called before the first frame update
    void Start()
    {
        mic = Microphone.devices[0];
        Debug.Log(mic);
    }

    // Update is called once per frame
    void Update()
    {
        //if(!Microphone.IsRecording(mic)){

            source.clip = Microphone.Start(mic, true, 1, 48000);
            while (!(Microphone.GetPosition(null) > buffer)) { }
            source.loop = true;
            source.Play();
            Debug.Log("mic was not recording");
        //} 


    }

    void OnDestroy()
    {
        Microphone.End(mic);
        source.Stop();

    }

    void OnDisable()
    {
        Microphone.End(mic);
        source.Stop();
    }
}
