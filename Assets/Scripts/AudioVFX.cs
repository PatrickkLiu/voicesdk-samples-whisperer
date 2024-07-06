using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lasp;
using UnityEngine.VFX;


public class AudioVFX : MonoBehaviour
{
    VisualEffect pointcloudvfx;
    AudioLevelTracker tracker;
    public float normalizedVolume;
    // Start is called before the first frame update
    void Start()
    {
        GameObject LevelTracker = GameObject.Find("CustomMicInputTest");
        tracker = LevelTracker.GetComponent<AudioLevelTracker>();
        pointcloudvfx = this.GetComponent<VisualEffect>();
    }

    // Update is called once per frame
    void Update()
    {
        normalizedVolume = tracker.normalizedLevel;
        pointcloudvfx.SetFloat("AudioLevel", normalizedVolume);
    }
}
