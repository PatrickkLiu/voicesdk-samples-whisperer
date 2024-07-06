using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityGoogleDrive;
using Pcx;
using UnityEngine.VFX; 

public class GoogleDriveDownloadTest : MonoBehaviour
{
    public byte[] Downloadedcontent;
    public byte[] dataBody;
    CustomizedPlyImporter customizedPlyImporter;

    //public VisualEffect pointcloudvfx;
    public GameObject VfxPrefab;

    /// variables for list files
    private GoogleDriveFiles.ListRequest listRequest;
    private Dictionary<string, string> listResults;
    private string query = string.Empty;
    [Range(1, 1000)]
    public int ResultsPerPage = 100;
    // Specify the target folder ID
    string folderId = "1gW5nCbCOE50BBriPgjY4b7gbFclKgo2E";

    // Initial state (you would typically store this in a database or memory)
    List<string> initialFileIds = new List<string>();


    public float sphereRadius = 5f;
    [SerializeField] GameObject displayVFX;
    NarrativeManagerInstallation narrativeManagerInstallation; // switch between performance and installation using boolean? if true
    NarrativeManagerPerformance narrativeManagerPerformance;
    dynamic narrativeManager; 
    bool performance = false;


    void Awake()
    {
        customizedPlyImporter = GetComponent<CustomizedPlyImporter>();
        LoadInitialFileIds();
        //StartCoroutine(MonitorDriveFolder());

    }
    // Start is called before the first frame update
    void Start()
    {
        //StartCoroutine("FileDownload");
        GameObject narrativeObject = GameObject.Find("NarrativeManager");
        if (narrativeObject != null)
        {
            narrativeManagerInstallation = narrativeObject.GetComponent<NarrativeManagerInstallation>();
            narrativeManagerPerformance = narrativeObject.GetComponent<NarrativeManagerPerformance>();
            if (narrativeManagerInstallation.enabled) {
                narrativeManager = narrativeManagerInstallation;
            }
            else
            {
                narrativeManager = narrativeManagerPerformance;
            }
        }
        StartCoroutine(MonitorDriveFolder());
    }
    


    private void LoadInitialFileIds()
    {
        /// Load the initial state by listing files in the target folder and storing their IDs

        var request = GoogleDriveFiles.List(); //initiates a request to retrieve a list of files
        request.Q = string.Format("'{0}' in parents", folderId); //Here, you set the query (Q) parameter of the request to filter files based on the folderId: retrieve files that are contained within the folder with the ID your-folder-id.
        // string.Format(...): This is a C# string formatting method used to create a string with placeholders for values to be inserted.
        //'{0}' is a placeholder for the folderId.
        //in parents: This is a query operator used to specify that you want to list files that are located inside a folder.
        request.Fields = new List<string> { "files(id)" };
        // This line specifies which fields you want to retrieve for each file in the response.
        //In this case, you are requesting only the 'id' field for each file
        var fileListRequest = request.Send();
        //sends the request to Google Drive to fetch the list of files.
        // returns a GoogleDriveRequestYieldInstruction<FileList>.

        ///an event handler to the OnDone event of the fileListRequest. The OnDone event is triggered when the file list request is completed,
        ///and it passes the resulting FileList as a parameter to the event handler.
        //+=: The += operator is used to add an event handler to an event
        //The fileList parameter will hold the result of the request, which is a FileList object.
        // => {} is a lambda function, it define small, inline functions without giving them a name or a separate definition.
        fileListRequest.OnDone += fileList =>
        {
            initialFileIds.Clear();
            foreach (var file in fileList.Files)
            {
                print(file.Id);
                initialFileIds.Add(file.Id);
            }
        }; //  => { ... }; the ; is used to terminate the lambda expression. 
    }

        
    public IEnumerator MonitorDriveFolder()
    {
        while (true)
        {
            // List files in the folder
            var request = GoogleDriveFiles.List();
            request.Q = string.Format("'{0}' in parents", folderId);
            request.Fields = new List<string> { "files(id)" };
            var fileListRequest = request.Send();

            fileListRequest.OnDone += fileList =>
            {
                foreach (var file in fileList.Files)
                {
                    // Check if the file is new
                    if (!initialFileIds.Contains(file.Id))
                    {
                        // Download the new file using UnityGoogleDrive
                        print(file.Id);
                        StartCoroutine(FileDownload(file.Id));
                        // Store the file ID in the initial state
                        initialFileIds.Add(file.Id);
                    }
                }
            };
            // Wait for a specific interval before the next check
            yield return new WaitForSeconds(2); //detect new files for every 2 seconds
        }
    }


    // Update is called once per frame
    void Update()
    {

    }


    public IEnumerator FileDownload(string id)
    {
        print("coroutine started");
        var request = GoogleDriveFiles.Download(id); // earth
        //var request = GoogleDriveFiles.Download("1-1OszTMnEhY6zEI6YywZdPAX5t7tL5gL"); //whale
        
        print("variable set");
        yield return
        request.Send();
        print("request sent");
        print(request.IsError);
        print(request.ResponseData.Content);
        print("content printed");
        Downloadedcontent = request.ResponseData.Content;
        print(Downloadedcontent.Length);

        // Remove the header and only leave the bodydata
        int bytesToRemove = 178;

        if (Downloadedcontent.Length > bytesToRemove)
        {
            dataBody = new byte[Downloadedcontent.Length - bytesToRemove];
            Array.Copy(Downloadedcontent, bytesToRemove, dataBody, 0, dataBody.Length);

            // Now, `newArray` contains the data with the first 178 bytes removed.
            // You can use `newArray` as needed.

            
        }
        else
        {
            // Handle the case where the original array is too small to remove 178 bytes.
            Debug.LogError("Original array is smaller than the specified bytes to remove.");
        }

        // display databody
        /*
        for (int i = 0; i <dataBody.Length; i++)
        {
             Debug.Log("Byte[" + i + "]: " + dataBody[i]);
        }
        */

        //convert databody byte array to memory stream
        using (MemoryStream stream = new MemoryStream(dataBody))
        {
            // You can now use 'stream' as a Stream to read or write data.
            // For example, you can read from the stream.
            //byte[] buffer = new byte[(dataBody.Length)];
            //int bytesRead = stream.Read(buffer, 0, buffer.Length);

            // Display the content of the byte array
            //print("Bytes read: " + bytesRead);
            //print("Contents: " + BitConverter.ToString(buffer));

            var data = customizedPlyImporter.ImportAsCustomPointCloud(stream);
            print(data.positionMap);
            print(data.colorMap);
            narrativeManager.DisplayObject();
            Vector3 randomPosition = UnityEngine.Random.insideUnitSphere * sphereRadius;
            GameObject StoryVFX = Instantiate(VfxPrefab, randomPosition, Quaternion.identity) as GameObject;
            VisualEffect storyPointcloud = StoryVFX.GetComponent<VisualEffect>();
            VisualEffect displayPointcloud = displayVFX.GetComponent<VisualEffect>();
            storyPointcloud.SetTexture("ColorMap",data.colorMap);
            storyPointcloud.SetTexture("PositionMap",data.positionMap);
            displayPointcloud.SetTexture("ColorMap", data.colorMap);
            displayPointcloud.SetTexture("PositionMap", data.positionMap);
            displayPointcloud.SendEvent("Start");






            AutoOrbitCamera autoOrbitCamera = FindObjectOfType<AutoOrbitCamera>();
            if (autoOrbitCamera != null)
            {
                autoOrbitCamera.SetNewRandomPosition(randomPosition);
            }

        }




    }

    void GenerateDisplayObject()
    {

    }

    
}
