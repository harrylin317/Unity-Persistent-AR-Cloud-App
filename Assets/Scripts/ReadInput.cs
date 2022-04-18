using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class ReadInput : MonoBehaviour
{
    
    private float timer = 0f;
    private bool countTimer = true;
    private anchorPlaneLocation testComponent;
    private ARCloudAnchorHistory hostedCloudAnchor;
    private string cloudAnchorsStorageKey;
    private int storageLimit = 10;
    [SerializeField]
    private GameObject group;
    [SerializeField]
    private TMP_Dropdown dropDown;
    [SerializeField]
    private Button selectAnchor;
    [SerializeField]
    private TextMeshProUGUI sampleText;



    List<string> itemsList = new List<string>();


    //private ARCloudAnchorHistory cloudData1 = new ARCloudAnchorHistory("TestAnchor1", 1, "Cylinder", null, Vector3.zero);
    //private ARCloudAnchorHistory cloudData2 = new ARCloudAnchorHistory("TestAnchor2", 2, "Box", null, Vector3.zero);
    //private ARCloudAnchorHistoryCollection dataList = new ARCloudAnchorHistoryCollection();


    [SerializeField]
    private GameObject editedGameObject;
    [SerializeField]
    private TMP_InputField inputField;
    private string currentInputText = null;
    [SerializeField]
    private Button editTitleButton;
    [SerializeField]
    private Button editBodyButton;
    TMPro.TextMeshPro selectedText;
    TMPro.TextMeshPro titleText;
    TMPro.TextMeshPro bodyText;
    // Start is called before the first frame update
    void Start()
    {
        //inputField.gameObject.SetActive(false);
        EditText();

        //testComponent = editedGameObject.GetComponent<anchorPlaneLocation>();
        //Debug.Log($"test value is: {testComponent.testVariable}");

        /*if (PlayerPrefs.HasKey("SavedInputText"))
        {
            Debug.Log("has key " + PlayerPrefs.GetString("SavedInputText"));
            selectedText.text = PlayerPrefs.GetString("SavedInputText");
        }*/

        //dataList.Collection.Add(cloudData1);
        //dataList.Collection.Add(cloudData2);
        //DropDownList();

    }

    // Update is called once per frame
    void Update()
    {
        if (countTimer)
        {
            timer += 1 * Time.deltaTime;
        }
        
    }

    //public void ReadStringInput()
    //{
    //    //testComponent.testVariable = true;
    //    currentInputText = inputField.text;
    //    selectedText.text = currentInputText;
    //}

    public void SavedText()
    {
        //inputField.text = "";
        Debug.Log($"saving text: {currentInputText}");
        PlayerPrefs.SetString("SavedInputText", currentInputText);
        PlayerPrefs.Save();
        Debug.Log($"text saved: {PlayerPrefs.GetString("SavedInputText")}");
        countTimer = false;
        Debug.Log("time took: " + timer.ToString("F2"));

        //bool checkValue = editedGameObject.GetComponent<anchorPlaneLocation>().testVariable;
        //Debug.Log($"test result is: {checkValue}");

        group.gameObject.SetActive(true);

        sampleText.gameObject.SetActive(true);
    }
    
    //public void DropDownList()
    //{
    //    dropDown.ClearOptions();
    //    itemsList = new List<string>();
    //    foreach (var data in dataList.Collection)
    //    {
    //        itemsList.Add(data.AnchorName);
    //    }
    //    dropDown.AddOptions(itemsList);
    //}

    public void SelectAnchorButton()
    {
        Debug.Log(dropDown.value);
        int selectedIndex = dropDown.value;
        dropDown.options.RemoveAt(selectedIndex);
        itemsList.RemoveAt(selectedIndex);
        dropDown.RefreshShownValue();

    }

    private void EditText()
    {
        titleText = editedGameObject.transform.GetChild(0).gameObject.GetComponentInChildren<TextMeshPro>();
        bodyText = editedGameObject.transform.GetChild(1).gameObject.GetComponentInChildren<TextMeshPro>();
        //var title = getTitle.;
        Debug.Log($"get title: {titleText.text}");
        Debug.Log($"get title: {bodyText.text}");
    }

    //public void EditTitleButtonPressed()
    //{
    //    selectedText = titleText;
    //    inputField.gameObject.SetActive(true);
    //}
    //public void EditBodyButtonPressed()
    //{
    //    selectedText = bodyText;
    //    inputField.gameObject.SetActive(true);
    //}

    //public void FontSizeIncrease()
    //{
    //    selectedText.fontSize += 0.01f;
    //}
    //public void FontSizeDecrease()
    //{
        
    //    selectedText.fontSize -= 0.01f;
    //    if(selectedText.fontSize < 0)
    //    {
    //        selectedText.fontSize = 0;
    //    }
    //    Debug.Log($"Size decreased to {selectedText.fontSize}");
    //}
}
