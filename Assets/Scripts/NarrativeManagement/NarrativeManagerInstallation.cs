using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus.Voice;
using UnityEngine.InputSystem;
using UnityEngine.VFX;
using Lasp;

[System.Serializable]
public class Preset
{
    public float R;
    public float G;
    public float B;
    //public float conformForce = 0f;
    //public float conformX = 0f;
    //public float conformY = 0f;
    //public float conformZ = 0f;
    public AudioClip music;
}

public class NarrativeManagerInstallation : MonoBehaviour
{
    public float R = 0f;
    public float G = 0f;
    public float B = 0f;
    public float AudioLevel = 0f;
    public float conformForce =0f ;
    public float conformX = 0f;
    public float conformY = 0f;
    public float conformZ = 0f;

    [SerializeField] AudioSource meditationBackground;
    public float fadeDuration = 5f;

    public AudioSource audioSource;
    public AudioClip bell;
    public AudioClip ambient;
    public AudioClip sex;
    public AudioClip kickDrum;
    public AudioClip polymeter;

    public List<Preset> presets;          // List of presets
    int index = 0;
    int lastIndex = -1;             // Initialize the last preset index played with an invalid index

    AppVoiceExperience appVoiceExperience;
    RenderChange renderChange;

    [SerializeField] GameObject ritualCircle;
    [SerializeField] AudioSource sheetInFlux;
    [SerializeField] AudioSource ritualStart;
    [SerializeField] AudioSource ritualEnd;
    [SerializeField] AudioSource loadingBreathing;
    [SerializeField] AudioSource revealingObject;
    [SerializeField] AudioSource noTranscriptionAudioEffect;
    [SerializeField] AudioSource whisperingInstruction;
    [SerializeField] VisualEffect displayPointcloud;
    [SerializeField] VisualEffect ring;


    private const float idleThreshold = 15f; // Duration of silence before auto playing scene
    public AudioLevelTracker audioLevelTracker;
    float microphoneInput = 0f;



    private Coroutine standbyCoroutine;
    private float silenceTimer = 0f;

    void Start()
    {
        GameObject voiceExperienceObject = GameObject.Find("Management");
        appVoiceExperience = voiceExperienceObject.GetComponent<AppVoiceExperience>();
        GameObject crossFader = GameObject.Find("CrossFader");
        renderChange = crossFader.GetComponent<RenderChange>();
    }

    // Update is called once per frame
    void Update()
    {
        microphoneInput = audioLevelTracker.normalizedLevel;
        //print(microphoneInput);

        if (Input.GetKeyDown("r"))
        {
            ResetScene();
        }
        if (Input.GetKeyDown("s"))
        {
            StartStandby();
        }
        if (Input.GetKeyDown("space")) // replace with voice activation
        {
            TurningRed();
            SpeechRecognition();
            displayPointcloud.SendEvent("Stop");

        }
        if (Input.GetKeyDown("p"))
        {
            ActivateRandomPreset();
        }
        if (Input.GetKeyDown("1"))
        {
            SceneActivation(presets[0].R, presets[0].G, presets[0].B, presets[0].music);
        }
        


    }

    void ResetScene()
    {
        audioSource.Stop();
    }

    #region standby


    public void StartStandby()
    {
        if (standbyCoroutine != null)
        {
            StopCoroutine(standbyCoroutine);
        }
        silenceTimer = 0f;
        standbyCoroutine = StartCoroutine(Standby());
    }

    public IEnumerator Standby()
    {

        print("Standby started");
        if (!renderChange.ringScene)
        {
            renderChange.FadeIn();
        }
        whisperingInstruction.Play();



        while (true)
        {
            if (MicrophoneInputDetected())
            {
                print("Input detected");
                silenceTimer = 0f; // Reset the silence timer if sound is detected
                whisperingInstruction.Stop();
                TurningRed();
                SpeechRecognition();
                displayPointcloud.SendEvent("Stop");
                yield break; //exit the  standby coroutine
                //break; // exit the while loop
            }
            else
            {
                silenceTimer += Time.deltaTime; // Increment the silence timer if no sound is detected
                //print("Silence timer: " + silenceTimer);
                if (silenceTimer >= idleThreshold)
                {
                    print("Idle Limit Hit! silenceTimer = " + silenceTimer);
                    print("No input for " + idleThreshold + " seconds, standby for too long");
                    ActivateRandomPreset();
                    yield break; // Exit the standby coroutine
                }
            }

            yield return null; // Wait for the next frame
        }
    }
    #endregion


    bool MicrophoneInputDetected()
    {
        return microphoneInput > 0.99f; // Adjust the threshold as needed
    }

    #region start ritual procedure

    void TurningRed() // start ritual 
    {
        ritualCircle.GetComponent<Animation>().Play("TurningRed");
        ritualStart.Play();
        ring.SetFloat("RingForceMin", 4f);
    }
    void SpeechRecognition()
    {
        appVoiceExperience.Activate();
        print("listening");

    }
    public void TurningBlue() // END ritual
    {
        print("turn blue");
        ritualCircle.GetComponent<Animation>().Play("TurningBlue");
        ring.SetFloat("RingForceMin", -3f);
        print("force set");
        ritualEnd.Play();

    }

    public void NoTranscription() 
    {
        StartCoroutine(WaitForRitualToEndAndStartStandby());
    }
    IEnumerator WaitForRitualToEndAndStartStandby()
    {
        print("Waiting for ritual to end");
        yield return new WaitForSeconds(4.0f);
        print("ritual ended");
        noTranscriptionAudioEffect.Play();
        ritualCircle.GetComponent<Animation>().Play("NoTranscription");
        yield return new WaitForSeconds(2.0f);
        StartStandby();
    }

    public void LoadingObject()
    {
        StartCoroutine(startLoading());
    }

    IEnumerator startLoading()
    {
        yield return new WaitForSeconds(5.0f);
        ritualCircle.GetComponent<Animation>().Play("Pulsing");
        loadingBreathing.Play();
        yield return new WaitForSeconds(30.0f);
        ritualCircle.GetComponent<Animation>().Stop("Pulsing");
    }


    public void DisplayObject()
    {
        StartCoroutine(startDisplaying());
    }

    IEnumerator startDisplaying()
    {
        ritualCircle.GetComponent<Animation>().Play("RevealingObject");
        revealingObject.Play();
        yield return new WaitForSeconds(15.0f);
        ActivateRandomPreset();
        //StartStandby();

    }


    #endregion

    void SentimentMappedToScene()
    {

    }



    void ActivateRandomPreset()
    {
        renderChange.FadeOut();
        if (presets.Count == 0) return; // No presets available

        do
        {
            index = Random.Range(0, presets.Count);
        }
        while (index == lastIndex && presets.Count > 1); // Ensure the new index is different from the last one

        lastIndex = index; // Update the last index to the current one


        Preset selectedPreset = presets[index]; 
        SceneActivation(selectedPreset.R, selectedPreset.G, selectedPreset.B, selectedPreset.music);
        
    }

    void SceneActivation(float red, float green, float blue, AudioClip myAudioClip)
    {
        R = red;
        G = green;
        B = blue;
        audioSource.clip = myAudioClip;
        audioSource.Play();
        print("preset[" + index + "] is activated");
        StartCoroutine(WaitForMusicToEndAndStartStandby());
    }

    IEnumerator WaitForMusicToEndAndStartStandby()
    {
        print("Waiting for music to end");
        while (audioSource.isPlaying)
        {
            yield return null; // wait until next frame
        }
        print("Music ended");
        ResetScene();
        print("Scene Reset");
        StartStandby();

    }
    



}
