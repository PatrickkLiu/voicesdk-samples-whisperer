using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Pcx;
using UnityEngine.VFX;

public class PointcloudInstantiator : MonoBehaviour
{
    public BakedPointCloud[] myList;
    public GameObject VFXContainer;

    public GameObject transcriptObject;

    private string transcriptText;    

    
    void Start()
    {
        myList = Resources.LoadAll<BakedPointCloud>("PointCacheData");
        print (myList[0].positionMap);
        InstantiatePointCloud();
    }

    // Update is called once per frame
    void InstantiatePointCloud()
    {
        transcriptText = transcriptObject.GetComponent<TextMeshPro>().text;

        //transcriptText = "Plane";
         // Loop through the asset names and instantiate corresponding assets
        foreach (BakedPointCloud myPointCloud in myList)
        {
            string assetName = myPointCloud.name;

            if (transcriptText.Contains(assetName))
            {
                GameObject newObject = Instantiate(VFXContainer, transform.position, Quaternion.identity) as GameObject;
                VisualEffect pointcloudvfx = newObject.GetComponent<VisualEffect>();
                pointcloudvfx.SetTexture("ColorMap",myPointCloud.colorMap);
                pointcloudvfx.SetTexture("PositionMap",myPointCloud.positionMap);  

            }
            
  
        }
    }   
    

}
