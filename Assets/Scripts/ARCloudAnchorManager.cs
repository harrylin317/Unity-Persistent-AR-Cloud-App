using System.Collections;
using System.Collections.Generic;
using Google.XR.ARCoreExtensions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.ARFoundation;
using TMPro;
using System;


public class AnchorCreatedEvent : UnityEvent<Transform, ARCloudAnchorHistory>{}

public class ARCloudAnchorManager : MonoBehaviour
{

    public static ARCloudAnchorManager Instance;
    [SerializeField]
    private Camera arCamera = null; 
    
    [SerializeField]
    private float resolveAnchorPassedTimeout = 10.0f;

    [SerializeField]
    private TMPro.TextMeshProUGUI progressText;

    private ARAnchorManager arAnchorManager = null;

    private ARCloudAnchorHistory pendingHostAnchor = new ARCloudAnchorHistory(0,"", null);

    private ARCloudAnchorHistory currentResolvingAnchor = new ARCloudAnchorHistory(0,"", null);

    private List<ARCloudAnchor> pendingCloudAnchors = new List<ARCloudAnchor>();

    private ARCloudAnchor cloudAnchor = null;

    private string anchorIdToResolve = null;

    private bool anchorHostInProgress = false;

    private bool anchorResolveInProgress = false;

    private float safeToResolvePassed = 0;

    private AnchorCreatedEvent cloudAnchorCreatedEvent = null;

    FeatureMapQuality quality;

    private float timer = 0f;
    private bool countTimer = false;

    private string cloudAnchorsStorageKey = "SavedCloudAnchorID";
    private int storageLimit = 10;


    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        cloudAnchorCreatedEvent = new AnchorCreatedEvent();
        cloudAnchorCreatedEvent.AddListener((t, d) => ARPlacementManager.Instance.ReCreatePlacement(t, d));
    }

    private Pose GetCameraPose()
    {
        return new Pose(arCamera.transform.position, arCamera.transform.rotation);
    }


    //Cloud Anchor Cycle
    public void QueueAnchor(ARCloudAnchorHistory queuedAnchor)
    {
        Debug.Log("Queing anchor");
        if(pendingHostAnchor.arAnchor == null && queuedAnchor.arAnchor != null)
        {
            Debug.Log($"Queued anchor with prefabIndex: {queuedAnchor.PrefabIndex}");

            pendingHostAnchor = queuedAnchor;
        }
        else
        {
            Debug.Log("A anchor is currently being hosted");
        }
    }

    public void HostAnchor()
    {
        Debug.Log("HostAnchor call in progress");

        //recommended up to 30 sec of scanning before calling host anchor
        quality = arAnchorManager.EstimateFeatureMapQualityForHosting(GetCameraPose());
        Debug.Log($"Feature Map Quality is: {quality}");

        if(pendingHostAnchor.arAnchor == null)
        {
            Debug.LogError("no pending anchor to host");
            return;
        }


        cloudAnchor = arAnchorManager.HostCloudAnchor(pendingHostAnchor.arAnchor, 1);
        progressText.gameObject.SetActive(true);
        progressText.text = "Hosting...";

        if (cloudAnchor == null)
        {
            Debug.LogError("unable to host cloud anchor");
            progressText.text = "Hosting failed";

        }
        else
        {
            anchorHostInProgress = true;

            countTimer = true;
        }
    }

    public void ResolveAnchor()
    {
        if(currentResolvingAnchor.AnchorID == null)
        {
            Debug.Log("No ongoing resolves, starting to resolve");
            ARCloudAnchorHistoryCollection dataCollection = LoadCloudAnchorHistory();
            //if (PlayerPrefs.HasKey(cloudAnchorsStorageKey))
            //{
            //    Debug.Log($"has key {PlayerPrefs.GetString(cloudAnchorsStorageKey)}");
            //    anchorIdToResolve = PlayerPrefs.GetString(cloudAnchorsStorageKey);
            //}
            //else
            //{
            //    Debug.Log("No saved cloud anchor found, aborting...");
            //    return;

            //}
            currentResolvingAnchor = dataCollection.Collection[0];
            Debug.Log("Resolving data: " + currentResolvingAnchor);
            anchorIdToResolve = currentResolvingAnchor.AnchorID;

            quality = arAnchorManager.EstimateFeatureMapQualityForHosting(GetCameraPose());
            cloudAnchor = arAnchorManager.ResolveCloudAnchorId(anchorIdToResolve);
            progressText.gameObject.SetActive(true);
            progressText.text = "Resolving...";

            if (cloudAnchor == null)
            {
                Debug.LogError($"unable to resolve cloud anchor: {anchorIdToResolve}");
                progressText.text = $"unable to resolve cloud anchor: {anchorIdToResolve}";

            }
            else
            {
                anchorResolveInProgress = true;
                countTimer = true;

            }
        }
        else
        {
            Debug.Log("An anchor is currenting getting resolved, please wait till it finish");
            return;
        }

    }

    private void CheckHostingProgress()
    {
        CloudAnchorState cloudAnchorState = cloudAnchor.cloudAnchorState;
        if(cloudAnchorState == CloudAnchorState.Success)
        {
            progressText.text = $"Hosting success! \nMapping quality: {quality}\n Time took to host: " + timer.ToString("F2");
            countTimer = false;
            timer = 0;
            anchorHostInProgress = false;

            anchorIdToResolve = cloudAnchor.cloudAnchorId;

            int count = LoadCloudAnchorHistory().Collection.Count;
            DateTime creationTime = DateTime.Now;

            ARCloudAnchorHistory saveAnchorHistory = new ARCloudAnchorHistory("CloudAnchor" + count, anchorIdToResolve, creationTime, pendingHostAnchor.PrefabIndex, pendingHostAnchor.ObjectName);
            SaveCloudAnchorHistory(saveAnchorHistory);
            //PlayerPrefs.SetString(cloudAnchorsStorageKey, anchorIdToResolve);
            //PlayerPrefs.Save();
            Debug.Log($"Save anchor prefab index is: {saveAnchorHistory.PrefabIndex}");
            Debug.Log($"Successfully hosted anchor: {anchorIdToResolve}");
            Debug.Log($"Saved to PlayerPref: {PlayerPrefs.GetString(cloudAnchorsStorageKey)}");


            pendingHostAnchor = new ARCloudAnchorHistory(0, "", null);



        }
        else if(cloudAnchorState != CloudAnchorState.TaskInProgress)
        {
            Debug.LogError($"Error while hosting cloud anchor {cloudAnchorState}");
            anchorHostInProgress = false;
            progressText.text = "Hosting Failed";
            pendingHostAnchor = new ARCloudAnchorHistory(0, "", null);


        }
    }

    private void CheckResolveProgress()
    {
        CloudAnchorState cloudAnchorState = cloudAnchor.cloudAnchorState;
        if (cloudAnchorState == CloudAnchorState.Success)
        {
            progressText.text = $"Resolve success! \nMapping quality: {quality}\n Time took to resolve: " + timer.ToString("F2");
            countTimer = false;
            timer = 0;

            anchorResolveInProgress = false;
            Debug.Log($"Successfully Resolved Cloud Anchor: {cloudAnchor.cloudAnchorId}");
            Debug.Log($"resolved cloud anchor position: {cloudAnchor.transform.position},{cloudAnchor.transform.rotation} ");
            cloudAnchorCreatedEvent?.Invoke(cloudAnchor.transform, currentResolvingAnchor);
            currentResolvingAnchor = new ARCloudAnchorHistory(0, "", null);

        }
        else if (cloudAnchorState != CloudAnchorState.TaskInProgress)
        {
            Debug.LogError($"Error while resolving cloud anchor {cloudAnchorState}");
            anchorResolveInProgress = false;
            progressText.text = "Resolving Failed";
            currentResolvingAnchor = new ARCloudAnchorHistory(0, "", null);

        }
    }

    public void ClearPlayerPrefsHistory()
    {
        PlayerPrefs.DeleteAll();
        Debug.Log("Cleared all PlayerPrefs");
    }

    private ARCloudAnchorHistoryCollection LoadCloudAnchorHistory()
    {
        if (PlayerPrefs.HasKey(cloudAnchorsStorageKey))
        {
            Debug.Log("Loading History");
            Debug.Log($"Getting history: {PlayerPrefs.GetString(cloudAnchorsStorageKey)}");

            var history = JsonUtility.FromJson<ARCloudAnchorHistoryCollection>(PlayerPrefs.GetString(cloudAnchorsStorageKey));
            Debug.Log("finished getting history");

            // Remove all records created more than 24 hours and update stored history.
            DateTime current = DateTime.Now;
            history.Collection.RemoveAll(data => current.Subtract(data.CreatedTime).Days > 0);
            PlayerPrefs.SetString(cloudAnchorsStorageKey, JsonUtility.ToJson(history));
            return history;
        }
        else
        {
            Debug.Log("No saved cloud anchor found, creating an empty history");
            return new ARCloudAnchorHistoryCollection();
        }

    }
    private void SaveCloudAnchorHistory(ARCloudAnchorHistory data)
    {
        var history = LoadCloudAnchorHistory();

        // Sort the data from latest record to oldest record which affects the option order in
        // multiselection dropdown.
        history.Collection.Add(data);
        history.Collection.Sort((left, right) => right.CreatedTime.CompareTo(left.CreatedTime));

        // Remove the oldest data if the capacity exceeds storage limit.
        if (history.Collection.Count > storageLimit)
        {
            Debug.Log("Exceeded Storage limit, removing the oldest saved cloud anchor");
            history.Collection.RemoveRange(storageLimit, history.Collection.Count - storageLimit);
        }

        PlayerPrefs.SetString(cloudAnchorsStorageKey, JsonUtility.ToJson(history));
    }

    void Update()
    {
        //timer
        if (countTimer)
        {
            timer += 1 * Time.deltaTime;
        }

        //checking for host result
        if (anchorHostInProgress)
        {
            CheckHostingProgress();
            return;
        }

        //checking for resolve result
        if(anchorResolveInProgress && safeToResolvePassed <= 0)
        {
            safeToResolvePassed = resolveAnchorPassedTimeout;
            if (!string.IsNullOrEmpty(anchorIdToResolve))
            {
                //Debug.Log($"resolving Anchor ID: {anchorIdToResolve}");
                CheckResolveProgress();
            }
        }
        else
        {
            safeToResolvePassed -= Time.deltaTime * 1.0f;
        }

    }
}
