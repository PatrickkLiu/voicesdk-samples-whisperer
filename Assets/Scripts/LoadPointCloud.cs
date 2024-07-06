using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class LoadPointCloud : MonoBehaviour
{
    public Object[] textureObjects;
    int currentIndex = 0;

    List<Texture2D> textures = new List<Texture2D>();
    public float sphereRadius = 5f;
    //[SerializeField] VisualEffect pointcloudvfx;
    [SerializeField] GameObject VfxPrefab;

    [SerializeField] List<AudioClip> audioClips = new List<AudioClip>();
    [SerializeField] AudioSource audioSourcePrefab;
    void Start()
    {
        textureObjects = Resources.LoadAll("Textures", typeof(Texture2D));
 
        Load();

    }

    void Load()
    {
        
        foreach (var o in textureObjects)
        {
            textures.Add(o as Texture2D);
        }
    }


    void Update()
    {
        if (Input.GetKeyDown("c"))
        {
            if (textures.Count > 0)
            {
                Texture2D colorMap = textures[currentIndex];
                Texture2D positionMap = textures[currentIndex + 1];
                Vector3 randomPosition = UnityEngine.Random.insideUnitSphere * sphereRadius;

                GameObject StoryVFX = Instantiate(VfxPrefab, randomPosition, Quaternion.identity) as GameObject;
                VisualEffect storyPointcloud = StoryVFX.GetComponent<VisualEffect>();
                storyPointcloud.SetTexture("ColorMap", colorMap);
                storyPointcloud.SetTexture("PositionMap", positionMap);

                AutoOrbitCamera autoOrbitCamera = FindObjectOfType<AutoOrbitCamera>();
                if (autoOrbitCamera != null)
                {
                    autoOrbitCamera.SetNewRandomPosition(randomPosition);
                }
                PlayRandomAudioClip();

                Debug.Log("Selected ColorMap: " + colorMap.name);
                Debug.Log("Selected PositionMap: " + positionMap.name);

                currentIndex += 2;
                if (currentIndex >= textures.Count)
                {
                    currentIndex = 0; // Loop back to the beginning if we reached the end
                }
            }
            else
            {
                Debug.LogError("No textures loaded.");
            }
        }
    }

    void PlayRandomAudioClip()
    {
        if (audioClips.Count > 0)
        {
            int randomIndex = Random.Range(0, audioClips.Count);
            AudioClip randomClip = audioClips[randomIndex];

            AudioSource audioSource = Instantiate(audioSourcePrefab, transform.position, Quaternion.identity);
            audioSource.clip = randomClip;
            audioSource.Play();
        }
        else
        {
            Debug.LogError("No audio clips loaded.");
        }
    }


}
