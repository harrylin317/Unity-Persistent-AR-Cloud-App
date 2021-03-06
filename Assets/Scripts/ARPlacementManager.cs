using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;
using TMPro;

public class ARPlacementManager : MonoBehaviour
{
    public static ARPlacementManager Instance;

    private bool start = false;
    [Header("AR Foundation")]
    public ARSession session;
    public ARPlaneManager arPlaneManager;
    public ARAnchorManager arAnchorManager;
    public ARRaycastManager raycastManager;
    
        
    //ray casting
    private List<ARRaycastHit> hits = new List<ARRaycastHit>();
    private Pose placementIndicatorPose;
    private ARPlane hitPlane;



    //placement indicators
    private bool placementPoseIsValid = false;

    //screen touching
    private bool onTouchHold = false;
    private Vector2 touchPosition = default;

    //placing and selecting object
    [Header("Object Placement")]
    [SerializeField]
    private GameObject placementIndicator;
    [SerializeField]
    private List<GameObject> objectToPlaceList = new List<GameObject>();
    public int objectToPlaceIndex = 0;
    [HideInInspector]
    public GameObject selectedObject = null;
    private anchorPlaneLocation selectedObjectPlane = null;
    private GameObject newObjectPlaced = null;
    [HideInInspector]
    public List<GameObject> objectPlacedList = new List<GameObject>();
    [SerializeField]
    private int maxObjectPlacedCount = 5;
    [HideInInspector]
    public int placedObjectCount = 0;
    //private int currentNameNum = -1;
    [HideInInspector]
    public string savedAnchorName = null;
    [HideInInspector]
    public bool editingText = false;

    [Header("UI")]
    //UI
    [SerializeField]
    private GameObject startGroup;
    public Button optionButton;
    public GameObject selectObjectButtons;
    public GameObject rotateObjectButtons;
    public GameObject scaleObjectButtons;
    public Button placeObjectButton;

    [SerializeField]
    private Image objectLocationTextBox;
    [SerializeField]
    private TMPro.TextMeshProUGUI objectLocationText;
    

    public Image scanningPlaneTextBox;
    
    public Image mappingQualityTextBox;
    public TMPro.TextMeshProUGUI mappingQualityText;
    public bool updateUI = true; 
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

    private void Start()
    {
        arPlaneManager.enabled = false;
    }
    // Update is called once per frame
    void Update()
    {
        if (start)
        {
            //Debug.Log("started");
            //Debug.Log($"value is = {updateUI}");


            DragObject();
            UpdatePlacementIndicator();

            if (updateUI)
            {
                //Debug.Log("updating ui");
                UpdateUI();
            }
        }
        
    }

    void UpdateUI()
    {

        if (selectedObject != null)
        {
            placeObjectButton.gameObject.SetActive(false);
            selectObjectButtons.SetActive(true);
            scanningPlaneTextBox.gameObject.SetActive(false);

            //changeObjectButtons.SetActive(false);
        }
        else
        {
            if (placementPoseIsValid)
            {
                scanningPlaneTextBox.gameObject.SetActive(false);
                placeObjectButton.gameObject.SetActive(true);
                //changeObjectButtons.SetActive(true);

            }
            else
            {
                scanningPlaneTextBox.gameObject.SetActive(true);
                placeObjectButton.gameObject.SetActive(false);
                //changeObjectButtons.SetActive(false);
            }
            selectObjectButtons.SetActive(false);
        }
    }

    void UpdatePlacementIndicator()
    {
        Vector2 screenPosition = Camera.main.ViewportToScreenPoint(new Vector2(0.5f, 0.5f));
        raycastManager.Raycast(screenPosition, hits, UnityEngine.XR.ARSubsystems.TrackableType.PlaneWithinPolygon);

        placementPoseIsValid = hits.Count > 0;
        if (placementPoseIsValid && selectedObject == null && updateUI)
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

    public void StartScene()
    {
        
        startGroup.gameObject.SetActive(false);
        optionButton.gameObject.SetActive(true);
        scanningPlaneTextBox.gameObject.SetActive(true);
        mappingQualityTextBox.gameObject.SetActive(true);
        arPlaneManager.enabled = true;
        start = true;

    }
    public void PlaceObject()
    {
        placedObjectCount = objectPlacedList.Count;

        if (placementPoseIsValid && placedObjectCount < maxObjectPlacedCount && selectedObject == null) {
            Debug.Log("placing object " + objectToPlaceList[objectToPlaceIndex].name);

            newObjectPlaced = Instantiate(objectToPlaceList[objectToPlaceIndex], placementIndicator.transform.position, placementIndicator.transform.rotation);
            newObjectPlaced.GetComponent<Outline>().enabled = false;
            //set initialized hit plane to the object
            selectedObjectPlane = newObjectPlaced.GetComponent<anchorPlaneLocation>();
            selectedObjectPlane.anchorPlane = hitPlane;
            
            newObjectPlaced.name = "PrefabIndex-" + objectToPlaceIndex + "-" + objectToPlaceList[objectToPlaceIndex].name + "-" + placedObjectCount;
            objectPlacedList.Add(newObjectPlaced);

            /*objectLocationText.gameObject.SetActive(true);
            objectLocationText.text = $"Object placed at:\n {placementIndicatorPose.position} , {placementIndicatorPose.rotation}";*/

            Debug.Log("placed object " + newObjectPlaced.name);
        }
        else
        {
            Debug.Log("maximum object reached: " + placedObjectCount);
            return;
        }

    }    

    public void PlaceAnchor()
    {
        if (selectedObject != null && selectedObjectPlane != null)
        {
            ARPlane anchorPlane = selectedObject.GetComponent<anchorPlaneLocation>().anchorPlane;
            if (anchorPlane == null)
            {
                Debug.Log("no planes attached to object");
            }
            Pose objectPose = new Pose(selectedObject.transform.position, selectedObject.transform.rotation);
            ARAnchor anchor = arAnchorManager.AttachAnchor(anchorPlane, objectPose);

            //get the prefab index value from selected object name
            string objectName = selectedObject.name;
            Debug.Log("object name is: " + objectName);

            string[] split = objectName.Split('-');
            string prefabIndex = split[1];
            Debug.Log("object prefab index extracted: " + prefabIndex);

            int prefabIndexInt = Int32.Parse(prefabIndex);
            Debug.Log("pased success, value is: " + prefabIndexInt);

            Vector3 scale = selectedObject.transform.localScale;

            TextMeshPro titleText = selectedObject.transform.GetChild(0).gameObject.GetComponentInChildren<TextMeshPro>();
            TextMeshPro bodyText = selectedObject.transform.GetChild(1).gameObject.GetComponentInChildren<TextMeshPro>();

            objectLocationTextBox.gameObject.SetActive(true);
            objectLocationText.text = $"Saving object with Pose of:\n {selectedObject.transform.position} , {selectedObject.transform.rotation}";

            ARCloudAnchorHistory queueARAnchor = new ARCloudAnchorHistory(savedAnchorName, prefabIndexInt, objectName, anchor, 
                titleText.text, bodyText.text, titleText.fontSize, bodyText.fontSize, scale);
            ARCloudAnchorManager.Instance.QueueAnchor(queueARAnchor);
            ARCloudAnchorManager.Instance.HostAnchor();
            savedAnchorName = null;
        }
    }

    public void ReCreatePlacement(Transform resolvedAnchorTransform, ARCloudAnchorHistory anchorData)
    {
        placedObjectCount = objectPlacedList.Count;

        string objectName = anchorData.ObjectName;
        string[] split = objectName.Split('-');
        objectName = split[0] + "-" + split[1] + "-" + split[2] + "-" + placedObjectCount;

        newObjectPlaced = Instantiate(objectToPlaceList[anchorData.PrefabIndex], resolvedAnchorTransform.position, resolvedAnchorTransform.rotation);
        newObjectPlaced.transform.localScale = anchorData.ObjectScale;
        newObjectPlaced.GetComponent<Outline>().enabled = false;

        TextMeshPro titleText = newObjectPlaced.transform.GetChild(0).gameObject.GetComponentInChildren<TextMeshPro>();
        TextMeshPro bodyText = newObjectPlaced.transform.GetChild(1).gameObject.GetComponentInChildren<TextMeshPro>();

        titleText.text = anchorData.TitleText;
        bodyText.text = anchorData.BodyText;
        titleText.fontSize = anchorData.TitleTextFontSize;
        bodyText.fontSize = anchorData.BodyTextFontSize;

        if (raycastManager.Raycast(resolvedAnchorTransform.position, hits, UnityEngine.XR.ARSubsystems.TrackableType.PlaneWithinPolygon))
        {
            hitPlane = arPlaneManager.GetPlane(hits[0].trackableId);
        }
        selectedObjectPlane = newObjectPlaced.GetComponent<anchorPlaneLocation>();
        selectedObjectPlane.anchorPlane = hitPlane;
        newObjectPlaced.name = objectName;
        objectPlacedList.Add(newObjectPlaced);


        Debug.Log($"instantiate at {resolvedAnchorTransform.position} , {resolvedAnchorTransform.rotation}");
        objectLocationTextBox.gameObject.SetActive(true);
        objectLocationText.text = $"Object re-created at at:\n {resolvedAnchorTransform.position} , {resolvedAnchorTransform.rotation}";
        //objectPlaced.transform.parent = transform;
    }

    public void DeleteObject()
    {
        if (selectedObject != null)
        {
            int dashIndex = selectedObject.name.IndexOf('-');
            //currentNameNum = int.Parse(selectedObject.name.Substring(0, dashIndex));
            Debug.Log("removing: " + selectedObject.name);
            //Debug.Log("currentNameNum: " + currentNameNum.ToString());

            objectPlacedList.Remove(selectedObject);
            Destroy(selectedObject.gameObject);
            selectedObject = null;
            placedObjectCount= objectPlacedList.Count;
            //if (placedObjectCount == 0)
            //{
            //    currentNameNum = -1;
            //}
        }

        foreach (var x in objectPlacedList)
        {
            Debug.Log("currently in list: " + x.name);
        }
    }

    public void ScaleButtonPressed()
    {
        if (rotateObjectButtons.activeSelf)
        {
            rotateObjectButtons.SetActive(false);
        }
        scaleObjectButtons.SetActive(!scaleObjectButtons.activeSelf);

    }

    public void RotateButtonPressed()
    {
        if (scaleObjectButtons.activeSelf)
        {
            scaleObjectButtons.SetActive(false);
        }
        rotateObjectButtons.SetActive(!rotateObjectButtons.activeSelf);

    }
    void DragObject()
    {
        if (editingText)
        {
            return;
        }

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            touchPosition = touch.position;

            //prevent touch object under the UI
            if (EventSystem.current.IsPointerOverGameObject(touch.fingerId))
            {
                //Debug.Log("touching UI");
                return;
            }
                


            if (touch.phase == TouchPhase.Began)
            {

                Ray ray = Camera.main.ScreenPointToRay(touch.position);
                RaycastHit hitObject;
                //Debug.Log("shooting ray");

                if (Physics.Raycast(ray, out hitObject))
                {
                    //Debug.Log($"hit object{hitObject.collider.gameObject.name}");

                    if (hitObject.collider.gameObject.tag == "Spawnable")
                    {
                        //reset the buttons if it was previously on

                        rotateObjectButtons.SetActive(false);
                        scaleObjectButtons.SetActive(false);
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
                                selectedObjectPlane = selectedObject.GetComponent<anchorPlaneLocation>();
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
                if (raycastManager.Raycast(touchPosition, hits, UnityEngine.XR.ARSubsystems.TrackableType.PlaneWithinPolygon))
                {
                    Pose hitPose = hits[0].pose;
                    hitPlane = arPlaneManager.GetPlane(hits[0].trackableId);
                    selectedObjectPlane.anchorPlane = hitPlane;
                    selectedObject.transform.position = hitPose.position;
                    //selectedObject.transform.rotation = hitPose.rotation;
                    /*objectLocationText.gameObject.SetActive(true);
                    objectLocationText.text = $"Object moved to:\n {selectedObject.transform.position} , {selectedObject.transform.rotation}";*/
                }
            }
        }
    }

}
