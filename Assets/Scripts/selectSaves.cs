using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class selectSaves : MonoBehaviour
{

    [SerializeField]
    private TMP_Dropdown dropDown;
    [SerializeField]
    private TextMeshProUGUI noSaveWarningText;
    [SerializeField]
    private TextMeshProUGUI recreatingWarningText;
    [SerializeField]
    private GameObject optionGroup;
    [SerializeField]
    private Button optionButton;

    private List<string> itemsList = new List<string>();
    private ARCloudAnchorHistoryCollection dataList = new ARCloudAnchorHistoryCollection();
    private bool noSavedObject = true;

    public void ShowSelectSaveMenu()
    {
        Debug.Log("Opening save menu");
        gameObject.SetActive(true);
        optionGroup.SetActive(false);
        ARPlacementManager.Instance.updateUI = false;
        ARPlacementManager.Instance.selectObjectButtons.SetActive(false);
        ARPlacementManager.Instance.scanningText.gameObject.SetActive(false);
        ARPlacementManager.Instance.placeObjectButton.gameObject.SetActive(false);

        dataList = ARCloudAnchorManager.Instance.LoadCloudAnchorHistory();
        Debug.Log($"got history: {dataList}");

        dropDown.ClearOptions();
        itemsList = new List<string>();

        if (dataList.Collection.Count > 0)
        {
            Debug.Log("data exist, loading history");

            noSaveWarningText.gameObject.SetActive(false);

            foreach (var data in dataList.Collection)
            {
                itemsList.Add(data.AnchorName);
            }
            dropDown.AddOptions(itemsList);
            noSavedObject = false;

        }
        else
        {
            Debug.Log("no history found, empty dropdown");
            noSavedObject = true;
            noSaveWarningText.gameObject.SetActive(true);
        }
        dropDown.RefreshShownValue();


    }

    // Update is called once per frame
    public void RecreateObjectButtonPressed()
    {
        if(!string.IsNullOrEmpty(ARCloudAnchorManager.Instance.currentResolvingAnchor.AnchorID))
        {
            Debug.Log("A object is currently being resolved, please wait till it finish");
            Debug.Log($"currentResolvingAnchor: {ARCloudAnchorManager.Instance.currentResolvingAnchor}");
            recreatingWarningText.gameObject.SetActive(true);
            Invoke("SetFalse", 3.0f);
            return;
        }
        else if (noSavedObject)
        {
            Debug.Log("no save object selected");
            return;
        }
        else
        {
            Debug.Log("loading saved object to resolve");

            Debug.Log(dropDown.value);
            int selectedIndex = dropDown.value;
            ARCloudAnchorHistory anchorToResolve = dataList.Collection[selectedIndex];
            ARCloudAnchorManager.Instance.ResolveAnchor(anchorToResolve);
            /*itemsList = new List<string>();
            dataList = new ARCloudAnchorHistoryCollection();*/
            gameObject.SetActive(false);
            optionButton.gameObject.SetActive(true);
            ARPlacementManager.Instance.updateUI = true;
            ARPlacementManager.Instance.selectObjectButtons.SetActive(true);
            ARPlacementManager.Instance.scanningText.gameObject.SetActive(true);
            ARPlacementManager.Instance.placeObjectButton.gameObject.SetActive(true);

        }
        


    }
    public void DeleteSelectSaveButtonPressed()
    {

        if (noSavedObject)
        {
            Debug.Log("no save object selected");

            return;
        }
        else
        {

            int selectedIndex = dropDown.value;
            Debug.Log($"deleting save: {dataList.Collection[selectedIndex].AnchorName}");

            dropDown.options.RemoveAt(selectedIndex);
            //itemsList.RemoveAt(selectedIndex);
            dataList.Collection.RemoveAt(selectedIndex);
            dropDown.RefreshShownValue();
            PlayerPrefs.SetString(ARCloudAnchorManager.Instance.cloudAnchorsStorageKey, JsonUtility.ToJson(dataList));

            if(dataList.Collection.Count > 0)
            {               
                noSavedObject = false;
            }
            else
            {
                Debug.Log("removed last save, no more save in history");
                noSavedObject = true;
                noSaveWarningText.gameObject.SetActive(true);
            }

        }


    }
    public void SelectSaveMenuExitButtonPressed()
    {
        Debug.Log("closing select save menu");
        itemsList = new List<string>();
        dataList = new ARCloudAnchorHistoryCollection();
        //ARPlacementManager.Instance.updateUI = true;
        gameObject.SetActive(false);
        optionGroup.SetActive(true);
        /*ARPlacementManager.Instance.selectObjectButtons.SetActive(true);
        ARPlacementManager.Instance.scanningText.gameObject.SetActive(true);
        ARPlacementManager.Instance.placeObjectButton.gameObject.SetActive(true);*/

    }
    private void SetFalse()
    {

        recreatingWarningText.gameObject.SetActive(false);

    }
}
