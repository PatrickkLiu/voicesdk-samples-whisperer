using UnityEngine;
using UnityEngine.VFX;

public class MusicVisualizer : MonoBehaviour
{
    public int numberOfBands = 8; // Number of frequency bands
    public float sensitivity = 100.0f;
    public float scaleMultiplier = 10.0f;

    private float[] spectrumData;
    private float[] frequencyBands;
    public AudioSource myAudio;
    public float amplitude;
    //public VisualEffect vfx;

    void Start()
    {
        spectrumData = new float[1024];
        frequencyBands = new float[numberOfBands];
    }

    void Update()
    {
        AnalyzeAudio();
        CreateFrequencyBands();
        VisualizeAudio();
    }

    void AnalyzeAudio()
    {
        // Get spectrum data from the output
        //AudioListener.GetSpectrumData(spectrumData, 0, FFTWindow.Rectangular);
        myAudio.GetSpectrumData(spectrumData, 0, FFTWindow.Rectangular);
    }

    void CreateFrequencyBands()
    {
        int count = 0;
        int sampleCount = spectrumData.Length / numberOfBands;

        for (int i = 0; i < numberOfBands; i++)
        {
            float average = 0;

            for (int j = 0; j < sampleCount; j++)
            {
                average += spectrumData[count] * (count + 1); // Weighted average
                count++;
            }

            average /= count;
            frequencyBands[i] = average * sensitivity;
        }
    }

    void VisualizeAudio()
    {
        /*
        // Use frequencyBands to update visual elements
        for (int i = 0; i < numberOfBands; i++)
        {
            // Example: Scale a cube's height based on frequency band amplitude
            float scaleY = Mathf.Clamp01(frequencyBands[i] * scaleMultiplier);
            // Assuming you have a cube named "visualCube" for each frequency band
            GameObject visualCube = GameObject.Find("visualCube" + i);
            visualCube.transform.localScale = new Vector3(1, scaleY, 1);
        }
        */

        amplitude = Mathf.Clamp01(frequencyBands[3] * scaleMultiplier);
        //for each VisualEffect vfx in vfxList
        //vfx.SetFloat("AudioLevel", amplitude);
    }
}

