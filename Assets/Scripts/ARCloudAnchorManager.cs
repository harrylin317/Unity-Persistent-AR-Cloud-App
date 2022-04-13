using System.Collections;
using System.Collections.Generic;
using Google.XR.ARCoreExtensions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.ARFoundation;
using TMPro;


public class AnchorCreatedEvent : UnityEvent<Transform>{}

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

    private ARAnchor pendingHostAnchor = null;

    private ARCloudAnchor cloudAnchor = null;

    private string anchorIdToResolve = null;

    private bool anchorHostInProgress = false;

    private bool anchorResolveInProgress = false;

    private float safeToResolvePassed = 0;

    private AnchorCreatedEvent cloudAnchorCreatedEvent = null;

    FeatureMapQuality quality;

    private float timer = 0f;
    private bool countTimer = false;


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
        cloudAnchorCreatedEvent.AddListener((t) => ARPlacementManager.Instance.ReCreatePlacement(t));
    }

    private Pose GetCameraPose()
    {
        return new Pose(arCamera.transform.position, arCamera.transform.rotation);
    }


    //Cloud Anchor Cycle
    public void QueueAnchor(ARAnchor arAnchor)
    {
        Debug.Log("Queing anchor");

        pendingHostAnchor = arAnchor;
    }

    public void HostAnchor()
    {
        Debug.Log("HostAnchor call in progress");

        //recommended up to 30 sec of scanning before calling host anchor
        quality = arAnchorManager.EstimateFeatureMapQualityForHosting(GetCameraPose());
        Debug.Log($"Feature Map Quality is: {quality}");

        if(pendingHostAnchor == null)
        {
            Debug.LogError("no pending anchor to host");
            return;
        }


        cloudAnchor = arAnchorManager.HostCloudAnchor(pendingHostAnchor, 1);
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
        Debug.Log("Resolve call in progress");

        if (PlayerPrefs.HasKey("SavedCloudAnchorID"))
        {
            Debug.Log($"has key {PlayerPrefs.GetString("SavedCloudAnchorID")}");
            anchorIdToResolve = PlayerPrefs.GetString("SavedCloudAnchorID");
        }

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
            Debug.Log($"Successfully hosted anchor: {anchorIdToResolve}");
            PlayerPrefs.SetString("SavedCloudAnchorID", anchorIdToResolve);
            PlayerPrefs.Save();
            Debug.Log($"Saved ID to PlayerPref: {PlayerPrefs.GetString("SavedCloudAnchorID", anchorIdToResolve)}");


        }
        else if(cloudAnchorState != CloudAnchorState.TaskInProgress)
        {
            Debug.LogError($"Error while hosting cloud anchor {cloudAnchorState}");
            anchorHostInProgress = false;
            progressText.text = "Hosting Failed";

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
            cloudAnchorCreatedEvent?.Invoke(cloudAnchor.transform);

        }
        else if (cloudAnchorState != CloudAnchorState.TaskInProgress)
        {
            Debug.LogError($"Error while resolving cloud anchor {cloudAnchorState}");
            anchorResolveInProgress = false;
            progressText.text = "Resolving Failed";

        }
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
                Debug.Log($"resolving Anchor ID: {anchorIdToResolve}");
                CheckResolveProgress();
            }
        }
        else
        {
            safeToResolvePassed -= Time.deltaTime * 1.0f;
        }

    }
}
