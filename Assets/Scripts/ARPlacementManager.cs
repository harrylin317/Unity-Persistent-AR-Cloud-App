using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ARPlacementManager : MonoBehaviour
{
    public static ARPlacementManager Instance;

    [Header("AR Foundation")]
    [SerializeField]
    private ARSession session;
    [SerializeField]
    private ARPlaneManager arPlaneManager;
    [SerializeField]
    private ARAnchorManager arAnchorManager;
    [SerializeField]
    private ARRaycastManager raycastManager;
    
        
    //ray casting
    private List<ARRaycastHit> hits = new List<ARRaycastHit>();
    private Pose placementIndicatorPose;
    private ARPlane hitPlane;

    private ARAnchor anchor = null;

    //placing and selecting object
    private GameObject objectPlaced = null;

    //placement indicators
    private bool placementPoseIsValid = false;

    //screen touching
    private bool onTouchHold = false;
    private Vector2 touchPosition = default;

    //placing and selecting object
    [Header("Object Placement")]
    [SerializeField]
    private GameObject objectToPlace;
    [SerializeField]
    private GameObject placementIndicator;
    [SerializeField]
    private List<GameObject> objectToPlaceList = new List<GameObject>();
    private int objectToPlaceIndex = 0;
    [HideInInspector]
    public GameObject selectedObject = null;
    private ARPlane selectedObjectPlane = null;
    private GameObject newObjectPlaced = null;
    private List<GameObject> objectPlacedList = new List<GameObject>();
    [SerializeField]
    private int maxObjectPlacedCount = 5;
    private int placedObjectCount = 0;
    private int currentNameNum = -1;

    [Header("UI")]
    //UI
    [SerializeField]
    private Button togglePlaneDetectionButton;
    [SerializeField]
    private GameObject selectObjectButtons;
    [SerializeField]
    private Button placeObjectButton;
    [SerializeField]
    private TMPro.TextMeshProUGUI objectLocationText;
    [SerializeField]
    private TMPro.TextMeshProUGUI scanningText;
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
        DragObject();
        UpdateUI();
    }

    void UpdateUI()
    {

        if (selectedObject != null)
        {
            placeObjectButton.gameObject.SetActive(false);
            selectObjectButtons.SetActive(true);
            //changeObjectButtons.SetActive(false);
        }
        else
        {
            if (placementPoseIsValid)
            {
                scanningText.gameObject.SetActive(false);
                placeObjectButton.gameObject.SetActive(true);
                //changeObjectButtons.SetActive(true);

            }
            else
            {
                scanningText.gameObject.SetActive(true);
                placeObjectButton.gameObject.SetActive(false);
                //changeObjectButtons.SetActive(false);
            }
            selectObjectButtons.SetActive(false);
        }
    }

    void UpdatePlacementIndicator()
    {
        Vector2 screenPosition = Camera.main.ViewportToScreenPoint(new Vector2(0.5f, 0.5f));
        raycastManager.Raycast(screenPosition, hits, UnityEngine.XR.ARSubsystems.TrackableType.Planes);

        placementPoseIsValid = hits.Count > 0;
        if (placementPoseIsValid)
        {
            placementIndicatorPose = hits[0].pose;
            hitPlane = arPlaneManager.GetPlane(hits[0].trackableId);

            placementIndicator.SetActive(true);
            placementIndicator.transform.SetPositionAndRotation(placementIndicatorPose.position, placementIndicatorPose.rotation);
        }
        else
        {
            placementIndicator.SetActive(false);
        }

    }

    public void PlaceObject()
    {
        if(placementPoseIsValid && placedObjectCount < maxObjectPlacedCount && selectedObject == null) {
            Debug.Log("placing object " + objectToPlaceList[objectToPlaceIndex].name);

            newObjectPlaced = Instantiate(objectToPlaceList[objectToPlaceIndex], placementIndicator.transform.position, placementIndicator.transform.rotation);
            newObjectPlaced.GetComponent<Outline>().enabled = false;
            //set initialized hit plane to the object
            selectedObjectPlane = newObjectPlaced.GetComponent<anchorPlaneLocation>().anchorPlane;
            selectedObjectPlane = hitPlane;
            placedObjectCount++;

            if (currentNameNum == -1)
            {
                newObjectPlaced.name = placedObjectCount.ToString() + "-" + objectToPlaceList[objectToPlaceIndex].name;
            }
            else
            {
                newObjectPlaced.name = currentNameNum.ToString() + "-" + objectToPlaceList[objectToPlaceIndex].name;
                currentNameNum = -1;
            }

            objectPlacedList.Add(newObjectPlaced);

            objectLocationText.gameObject.SetActive(true);
            objectLocationText.text = $"Object placed at:\n {placementIndicatorPose.position} , {placementIndicatorPose.rotation}";

            Debug.Log("placed object " + newObjectPlaced.name);
        }
        else
        {
            Debug.Log("maximum object reached: " + placedObjectCount.ToString());
            return;
        }

        /*if (placementPoseIsValid && anchor == null && objectPlaced == null)
        {
            anchor = arAnchorManager.AttachAnchor(hitPlane, placementIndicatorPose);
            objectPlaced = Instantiate(objectToPlace, placementIndicatorPose.position, placementIndicatorPose.rotation);
            Debug.Log($"instantiated object at {placementIndicatorPose.position} , {placementIndicatorPose.rotation}");
            objectLocationText.gameObject.SetActive(true);
            objectLocationText.text = $"Object placed at:\n {placementIndicatorPose.position} , {placementIndicatorPose.rotation}";
            //objectPlaced.transform.parent = anchor.transform;

            ARCloudAnchorManager.Instance.QueueAnchor(anchor);
        }*/

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
        for (int i = 0; i < objectPlacedList.Count; i++)
        {
            Destroy(objectPlacedList[i].gameObject);
        }
        objectPlacedList = new List<GameObject>();
        selectedObject = null;
        placedObjectCount = 0;
        currentNameNum = -1;

        Debug.Log("clearing scene");
    }

    public void DeleteObject()
    {
        if (selectedObject != null)
        {
            int dashIndex = selectedObject.name.IndexOf('-');
            currentNameNum = int.Parse(selectedObject.name.Substring(0, dashIndex));
            Debug.Log("removing: " + selectedObject.name);
            Debug.Log("currentNameNum: " + currentNameNum.ToString());

            objectPlacedList.Remove(selectedObject);
            Destroy(selectedObject.gameObject);
            selectedObject = null;
            placedObjectCount--;
            if (placedObjectCount == 0)
            {
                currentNameNum = -1;
            }
        }

        foreach (var x in objectPlacedList)
        {
            Debug.Log("currently in list: " + x.name);
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

    void DragObject()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            touchPosition = touch.position;

            //prevent touch object under the UI
            if (EventSystem.current.IsPointerOverGameObject(touch.fingerId))
                return;


            if (touch.phase == TouchPhase.Began)
            {

                Ray ray = Camera.main.ScreenPointToRay(touch.position);
                RaycastHit hitObject;
                if (Physics.Raycast(ray, out hitObject))
                {
                    if (hitObject.collider.gameObject.tag == "Spawnable")
                    {

                        //select object
                        for (int i = 0; i < objectPlacedList.Count; i++)
                        {
                            if (objectPlacedList[i].name == hitObject.collider.gameObject.name)
                            {

                                //switch to the other object if an object is previously selected
                                if (selectedObject != null)
                                {
                                    selectedObject.GetComponent<Outline>().enabled = false;
                                }
                                selectedObject = objectPlacedList[i];
                                selectedObjectPlane = newObjectPlaced.GetComponent<anchorPlaneLocation>().anchorPlane;
                                Debug.Log("currently selected: " + selectedObject.name);
                                selectedObject.GetComponent<Outline>().enabled = true;
                            }
                        }
                    }

                    Debug.Log("Name is: " + hitObject.collider.gameObject.name);
                    Debug.Log("Tag is: " + hitObject.collider.gameObject.tag);

                }
                else
                {
                    //if no object is touched and an object is previously selected, deselect the object
                    if (selectedObject != null)
                    {
                        selectedObject.GetComponent<Outline>().enabled = false;
                    }
                    selectedObject = null;
                    Debug.Log("no object selected");

                }
            }

            if (touch.phase == TouchPhase.Moved)
            {
                touchPosition = touch.position;
                onTouchHold = true;

            }

            if (touch.phase == TouchPhase.Ended)
            {
                onTouchHold = false;


            }

            //if (onTouchHold && selectedObject != null && touchingObject)
            if (onTouchHold && selectedObject != null)
            {
                if (raycastManager.Raycast(touchPosition, hits, UnityEngine.XR.ARSubsystems.TrackableType.Planes))
                {
                    Pose hitPose = hits[0].pose;
                    hitPlane = arPlaneManager.GetPlane(hits[0].trackableId);
                    selectedObjectPlane = hitPlane;
                    selectedObject.transform.position = hitPose.position;
                    //selectedObject.transform.rotation = hitPose.rotation;                                       
                }
            }
        }
    }

}
