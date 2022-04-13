using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ARPlacementManager : MonoBehaviour
{
    public static ARPlacementManager Instance;
    [SerializeField]
    private ARSession session;
    [SerializeField]
    private GameObject objectToPlace;
    [SerializeField]
    private ARPlaneManager arPlaneManager;
    [SerializeField]
    private ARAnchorManager arAnchorManager;
    [SerializeField]
    private TMPro.TextMeshProUGUI objectLocationText;
    [SerializeField]
    private Button togglePlaneDetectionButton;
    [SerializeField]
    private ARRaycastManager raycastManager;
    [SerializeField]
    private GameObject placementIndicator;

    //ray casting
    private List<ARRaycastHit> hits = new List<ARRaycastHit>();
    private Pose hitPose;
    private ARPlane hitPlane;

    private ARAnchor anchor = null;

    //placing and selecting object
    private GameObject objectPlaced = null;

    //placement indicators
    private bool placementPoseIsValid = false;


    // Start is called before the first frame update
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        UpdatePlacementIndicator();
    }

   
    void UpdatePlacementIndicator()
    {
        Vector2 screenPosition = Camera.main.ViewportToScreenPoint(new Vector2(0.5f, 0.5f));
        raycastManager.Raycast(screenPosition, hits, UnityEngine.XR.ARSubsystems.TrackableType.Planes);

        placementPoseIsValid = hits.Count > 0;
        if (placementPoseIsValid)
        {
            hitPose = hits[0].pose;
            hitPlane = arPlaneManager.GetPlane(hits[0].trackableId);

            placementIndicator.SetActive(true);
            placementIndicator.transform.SetPositionAndRotation(hitPose.position, hitPose.rotation);
        }
        else
        {
            placementIndicator.SetActive(false);
        }

    }

    public void PlaceObject()
    {
        if(placementPoseIsValid && anchor == null && objectPlaced == null)
        {
            anchor = arAnchorManager.AttachAnchor(hitPlane, hitPose);
            objectPlaced = Instantiate(objectToPlace, hitPose.position, hitPose.rotation);
            Debug.Log($"instantiated object at {hitPose.position} , {hitPose.rotation}");
            objectLocationText.gameObject.SetActive(true);
            objectLocationText.text = $"Object placed at:\n {hitPose.position} , {hitPose.rotation}";
            //objectPlaced.transform.parent = anchor.transform;

            ARCloudAnchorManager.Instance.QueueAnchor(anchor);
        }
        //if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        //{
        //    Touch touch = Input.GetTouch(0);
        //    Vector2 touchPosition = touch.position;


        //    if (EventSystem.current.IsPointerOverGameObject(touch.fingerId))
        //        return;

        //    Debug.Log("no object placed, placing object...");
        //    if (raycastManager.Raycast(touchPosition, hits, UnityEngine.XR.ARSubsystems.TrackableType.Planes))
        //    {
        //        Debug.Log("hit plane");
        //        var hitPose = hits[0].pose;
                    
        //        if(anchor == null && objectPlaced == null)
        //        {
        //            ARPlane plane = arPlaneManager.GetPlane(hits[0].trackableId);
        //            anchor = arAnchorManager.AttachAnchor(plane, hitPose);
        //            objectPlaced = Instantiate(objectToPlace, hitPose.position, hitPose.rotation);
        //            Debug.Log($"instantiated object at {hitPose.position} , {hitPose.rotation}");
        //            objectLocationText.gameObject.SetActive(true);
        //            objectLocationText.text = $"Object placed at:\n {hitPose.position} , {hitPose.rotation}";
        //            //objectPlaced.transform.parent = anchor.transform;

        //            ARCloudAnchorManager.Instance.QueueAnchor(anchor);

        //        }

        //    }
            
        //}
    }


    public void PlaceAnchor()
    {

    }

    public void ReCreatePlacement(Transform resolvedAnchorTransform)
    {
        objectPlaced = Instantiate(objectToPlace, resolvedAnchorTransform.position, resolvedAnchorTransform.rotation);
        Debug.Log($"instantiate at {resolvedAnchorTransform.position} , {resolvedAnchorTransform.rotation}");
        objectLocationText.gameObject.SetActive(true);
        objectLocationText.text = $"Object re-created at at:\n {resolvedAnchorTransform.position} , {resolvedAnchorTransform.rotation}";
        //objectPlaced.transform.parent = transform;
    }

    public void ClearObjects()
    {
        if (objectPlaced != null)
        {
            Destroy(objectPlaced);
            objectPlaced = null;
        }
    }
    public void ResetScene()
    {
        ClearObjects();
        session.Reset();
    }
    public void TogglePlaneDetection()
    {
        arPlaneManager.enabled = !arPlaneManager.enabled;

        foreach (ARPlane plane in arPlaneManager.trackables)
        {
            plane.gameObject.SetActive(arPlaneManager.enabled);
        }
        togglePlaneDetectionButton.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = arPlaneManager.enabled ? "Disable Plane Detection" : "Enable Plane Detection";
        Debug.Log("toggling button to" + arPlaneManager.enabled.ToString());
    }


}
