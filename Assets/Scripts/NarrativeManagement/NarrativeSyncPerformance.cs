using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class NarrativeSyncPerformance : MonoBehaviour
{
    VisualEffect pointcloudvfx;
    NarrativeManagerPerformance narrativeManager;
    MusicVisualizer musicVisualizer;

    // Start is called before the first frame update
    void Start()
    {
        GameObject narrativeObject = GameObject.Find("NarrativeManager");
        GameObject visualizerObject = GameObject.Find("AudioVisualizer");
        narrativeManager = narrativeObject.GetComponent<NarrativeManagerPerformance>();
        musicVisualizer = visualizerObject.GetComponent<MusicVisualizer>();
        pointcloudvfx = this.GetComponent<VisualEffect>();
    }

    // Update is called once per frame
    void Update()
    {
        Sync();
    }

    void Sync()
    {
        pointcloudvfx.SetFloat("R", narrativeManager.R);
        pointcloudvfx.SetFloat("G", narrativeManager.G);
        pointcloudvfx.SetFloat("B", narrativeManager.B);
        pointcloudvfx.SetFloat("ConformX", narrativeManager.conformX);
        pointcloudvfx.SetFloat("ConformY", narrativeManager.conformY);
        pointcloudvfx.SetFloat("ConformZ", narrativeManager.conformZ);
        pointcloudvfx.SetFloat("ConformForce", narrativeManager.conformForce);
        pointcloudvfx.SetFloat("AudioLevel", musicVisualizer.amplitude);

    }
}
