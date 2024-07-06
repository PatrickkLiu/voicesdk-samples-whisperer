using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus.Voice;
using UnityEngine.InputSystem;
using UnityEngine.VFX;

public class NarrativeManagerPerformance : MonoBehaviour
{
    public float R;
    public float G;
    public float B;
    public float AudioLevel;
    public float conformForce;
    public float conformX;
    public float conformY;
    public float conformZ;

    [SerializeField] AudioSource meditationBackground;
    public float fadeDuration = 5f;

    public AudioSource audioSource;
    public AudioClip bell;
    public AudioClip ambient;
    public AudioClip sex;
    public AudioClip kickDrum;
    public AudioClip polymeter;

    AppVoiceExperience appVoiceExperience;

    [SerializeField] GameObject ritualCircle;
    [SerializeField] AudioSource sheetInFlux;
    [SerializeField] AudioSource ritualStart;
    [SerializeField] AudioSource ritualEnd;
    [SerializeField] AudioSource loadingBreathing;
    [SerializeField] AudioSource revealingObject;
    [SerializeField] VisualEffect displayPointcloud;
    [SerializeField] InputAction _actionKnob1 = null;
    [SerializeField] InputAction _actionKnob2 = null;
    [SerializeField] InputAction _actionKnob3 = null;
    [SerializeField] InputAction _actionKnob4 = null;
    [SerializeField] InputAction _actionKnob5 = null;
    [SerializeField] InputAction _actionKnob6 = null;
    [SerializeField] InputAction _actionKnob7 = null;
    // Start is called before the first frame update
    void Start()
    {
        GameObject voiceExperienceObject = GameObject.Find("Management");
        appVoiceExperience = voiceExperienceObject.GetComponent<AppVoiceExperience>();
        _actionKnob1.Enable();
        _actionKnob2.Enable();
        _actionKnob3.Enable();
        _actionKnob4.Enable();
        _actionKnob5.Enable();
        _actionKnob6.Enable();
        _actionKnob7.Enable();
    }

    // Update is called once per frame
    void Update()
    {
        //scene 0 bell meditation
        if (Input.GetKeyDown("a"))
        {
            //SceneActivation("meditation", bell);
            StartCoroutine(Meditation());

        }

        //scene 1 ambient
        if (Input.GetKeyDown("s"))
        {
            //SceneActivation("scene1", ambient);
            sheetInFlux.Play();
        }

        if (Input.GetKeyDown("d"))
        {
            SceneActivation("scene2", sex);
        }

        if (Input.GetKeyDown("f"))
        {
            //SceneActivation("scene3", kickDrum);
        }

        if (Input.GetKeyDown("g"))
        {
            //SceneActivation("scene4", polymeter);
        }



        if (Input.GetKeyDown("r"))
        {
            ResetScene();
        }

        if (Input.GetKeyDown("space"))
        {
            TurningRed();
            SpeechRecognition();
            displayPointcloud.SendEvent("Stop");

        }
        ReadKnob();
        //ReadKnob(_actionKnob1,R);
        //ReadKnob(_actionKnob2, G);
        //ReadKnob(_actionKnob3, B);
        //ReadKnob(_actionKnob4, conformForce);


    }

    /*
    void ReadKnob(InputAction action, float parameter)
    {
        var value = action.ReadValue<float>();
        parameter = value;
        print(value);
        //parameter = (value - 0.5f)*10f;
        //print(value);
        //_transform.localScale = Vector3.one * value;
    }*/
    
    
    void ReadKnob()
    {
        var value1 = _actionKnob1.ReadValue<float>();
        var value2 = _actionKnob2.ReadValue<float>();
        var value3 = _actionKnob3.ReadValue<float>();
        var value4 = _actionKnob4.ReadValue<float>();
        var value5 = _actionKnob5.ReadValue<float>();
        var value6 = _actionKnob6.ReadValue<float>();
        var value7 = _actionKnob7.ReadValue<float>();

        print(value1);
        R = (value1-0.5f)*10;
        G= (value2-0.5f)*10;
        B = (value3-0.5f)*10;
        conformForce = (value4 - 0.5f) * 20;
        conformX = (value5 ) * 20;
        conformY = (value6 - 0.5f) * 30;
        conformZ = (value7 - 0.5f) * 30;



    }

    IEnumerator Meditation()
    {
        meditationBackground.Play();
        ritualCircle.GetComponent<Animation>().Play("Meditation");
        yield return new WaitForSeconds(110.0f);
        float startVolume = audioSource.volume;

        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            meditationBackground.volume = Mathf.Lerp(startVolume, 0.15f, elapsedTime / fadeDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

    }

    void SceneActivation(string scene, AudioClip myAudioClip)
    {
        print(scene);
        //this.GetComponent<Animation>().Play(scene);
        audioSource.clip = myAudioClip;
        audioSource.Play();
    }

    void ResetScene()
    {
        //this.GetComponent<AudioSource>().Stop();
        //this.GetComponent<Animation>().Play("reset");
        audioSource.Stop();
    }

    void SpeechRecognition()
    {
        appVoiceExperience.Activate();
        print("listening");


    }

    void TurningRed() // start ritual 
    {
        ritualCircle.GetComponent<Animation>().Play("TurningRed");
        ritualStart.Play();
    }

    public void TurningBlue() // END ritual
    {
        ritualCircle.GetComponent<Animation>().Play("TurningBlue");
        ritualEnd.Play();
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
        ritualCircle.GetComponent<Animation>().Play("RevealingObject");
        revealingObject.Play();
    }
}
