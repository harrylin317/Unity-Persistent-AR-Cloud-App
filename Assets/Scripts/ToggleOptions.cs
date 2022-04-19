using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;   
using UnityEngine.XR.ARFoundation;

public class ToggleOptions : MonoBehaviour
{
    [SerializeField]
    private Button togglePlaneDetectionButton;
    [SerializeField]
    private Button optionButton;

    public void ClearObjects()
    {
        Debug.Log("clearing scene");

        for (int i = 0; i < ARPlacementManager.Instance.objectPlacedList.Count; i++)
        {
            Destroy(ARPlacementManager.Instance.objectPlacedList[i].gameObject);
        }
        ARPlacementManager.Instance.objectPlacedList = new List<GameObject>();
        ARPlacementManager.Instance.selectedObject = null;
        ARPlacementManager.Instance.placedObjectCount = 0;

    }
    public void TogglePlaneDetection()
    {
        ARPlacementManager.Instance.arPlaneManager.enabled = !ARPlacementManager.Instance.arPlaneManager.enabled;

        foreach (ARPlane plane in ARPlacementManager.Instance.arPlaneManager.trackables)
        {
            plane.gameObject.SetActive(ARPlacementManager.Instance.arPlaneManager.enabled);
        }
        togglePlaneDetectionButton.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = ARPlacementManager.Instance.arPlaneManager.enabled ? "Hide Planes" : "Show Planes";
        Debug.Log("toggling button to" + ARPlacementManager.Instance.arPlaneManager.enabled.ToString());
    }
    public void ResetScene()
    {
        Debug.Log("resetting scene");
        ClearObjects();
        ARPlacementManager.Instance.session.Reset();
    }
    public void ClearPlayerPrefsHistory()
    {
        PlayerPrefs.DeleteKey(ARCloudAnchorManager.Instance.cloudAnchorsStorageKey);
        Debug.Log("Cleared PlayerPrefs");
    }
    public void OptionButtonPressed()
    {
        Debug.Log("option button pressed");

        ARPlacementManager.Instance.updateUI = false;
        gameObject.SetActive(true);
        optionButton.gameObject.SetActive(false);
        ARPlacementManager.Instance.selectObjectButtons.SetActive(false);
        ARPlacementManager.Instance.scanningText.gameObject.SetActive(false);
        ARPlacementManager.Instance.placeObjectButton.gameObject.SetActive(false);

    }
    public void OptionMenuExitButtonPressed()
    {
        Debug.Log("option menu exit button pressed");

        ARPlacementManager.Instance.updateUI = true;
        gameObject.SetActive(false);
        optionButton.gameObject.SetActive(true);
        ARPlacementManager.Instance.selectObjectButtons.SetActive(true);
        ARPlacementManager.Instance.scanningText.gameObject.SetActive(true);
        ARPlacementManager.Instance.placeObjectButton.gameObject.SetActive(true);

    }
}
