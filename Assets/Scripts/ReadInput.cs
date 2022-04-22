using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using Firebase.Database;
using Firebase.Extensions;
using Newtonsoft.Json;
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

    public Image textbox;

    private ARCloudAnchorHistory cloudData1 = new ARCloudAnchorHistory("TestAnchor1", 1, "Cylinder1", null, "title1", "body1", 3, 3, Vector3.zero);
    private ARCloudAnchorHistory cloudData2 = new ARCloudAnchorHistory("TestAnchor2", 1, "Cylinder2", null, "title2", "body2", 3, 3, Vector3.zero);
    private ARCloudAnchorHistoryCollection newHistory = new ARCloudAnchorHistoryCollection();

    private string userID;
    private DatabaseReference dbReference;

    // Start is called before the first frame update
    void Start()
    {
        //inputField.gameObject.SetActive(false);
        EditText();
        var testText = textbox.GetComponentInChildren<TextMeshProUGUI>();
        print(testText.text);

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


        userID = SystemInfo.deviceUniqueIdentifier;
        dbReference = FirebaseDatabase.DefaultInstance.RootReference;
        Debug.Log("Start");
        //DatabaseManager.Instance.SaveHistory(cloudData1, DatabaseManager.Instance.Test);
        Debug.Log("Finish");

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

    public void EditTitleButtonPressed(Button button)
    {
        Debug.Log($"Button pressed: {button.name}");
        selectedText = titleText;
        inputField.gameObject.SetActive(true);
    }
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
    public IEnumerator TestCreateCloudAnchorHistory(ARCloudAnchorHistory history)
    {
        Debug.Log("creating user data");

        string historyJson = JsonUtility.ToJson(history);

        var dbTask = dbReference.Child("users").Child(userID).Child(history.AnchorName).SetRawJsonValueAsync(historyJson);
        yield return new WaitUntil(predicate: () => dbTask.IsCompleted);

        if (dbTask.Exception != null)
        {
            Debug.Log("Failed to save data");
        }
        else
        {
            Debug.Log("Save finished");

        }
    }
    public IEnumerator TestRemoveCloudAnchorHistory(string saveName)
    {
        Debug.Log("removing user data");

        var dbTask = dbReference.Child("users").Child(userID).Child("anchor1").RemoveValueAsync();
        yield return new WaitUntil(predicate: () => dbTask.IsCompleted);

        if (dbTask.Exception != null)
        {
            Debug.Log("Failed to remove data");
        }
        else
        {
            Debug.Log("Successfully removed");

        }
    }

    private IEnumerator TestLoadCloudAnchorHistory()
    {
        var dbTask = dbReference.Child("users").Child(userID).GetValueAsync();

        yield return new WaitUntil(predicate: () => dbTask.IsCompleted);

        if (dbTask.Exception != null)
        {
            Debug.Log($"Failed to register task with {dbTask.Exception}");
        }
        else if (dbTask.Result.Value == null)
        {
            Debug.Log("No data exist");

        }
        else
        {
            DataSnapshot snapshot = dbTask.Result;
            foreach(DataSnapshot s in snapshot.Children)
            {
                string getHistoryJson = s.GetRawJsonValue();
                Debug.Log($"get: {getHistoryJson}");
                ARCloudAnchorHistory getHistory = JsonUtility.FromJson<ARCloudAnchorHistory>(getHistoryJson);

                newHistory.Collection.Add(getHistory);

            }

            //var getHistoryJsonDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            /*foreach (KeyValuePair<string,string> history in getHistoryJsonDictionary)
            {
                Debug.Log(history.Key);
                Debug.Log(history.Value);
            }*/


            foreach (ARCloudAnchorHistory data in newHistory.Collection)
            {
                Debug.Log($"data: {data}");

            }


        }
    }

    public void GetAnchorHistory()
    {
        StartCoroutine(TestLoadCloudAnchorHistory());
        
    }
    public void SaveAnchorHistory()
    {
       StartCoroutine(TestCreateCloudAnchorHistory(cloudData1));
       StartCoroutine(TestCreateCloudAnchorHistory(cloudData2));
    }

    public void DeleteAnchorHistory()
    {
        StartCoroutine(TestRemoveCloudAnchorHistory(cloudData1.AnchorName));
    }


}
