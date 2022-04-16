using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class ReadInput : MonoBehaviour
{
    [SerializeField]
    private GameObject editedGameObject;
    [SerializeField]
    private TMP_InputField inputField;
    private string currentInputText = null;
    TextMeshProUGUI selectedText;
    private float timer = 0f;
    private bool countTimer = true;
    private anchorPlaneLocation testComponent;
    private ARCloudAnchorHistory hostedCloudAnchor;
    private string cloudAnchorsStorageKey;
    private int storageLimit = 10;
    [SerializeField]
    private GameObject group;
    // Start is called before the first frame update
    void Start()
    {
        //inputField.gameObject.SetActive(false);
        GameObject getCanvas = editedGameObject.transform.GetChild(1).gameObject;
        selectedText = getCanvas.GetComponentInChildren<TextMeshProUGUI>();

        testComponent = editedGameObject.GetComponent<anchorPlaneLocation>();
        Debug.Log($"test value is: {testComponent.testVariable}");

        if (PlayerPrefs.HasKey("SavedInputText"))
        {
            Debug.Log("has key " + PlayerPrefs.GetString("SavedInputText"));
            selectedText.text = PlayerPrefs.GetString("SavedInputText");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (countTimer)
        {
            timer += 1 * Time.deltaTime;
        }
        
    }

    public void ReadStringInput()
    {
        testComponent.testVariable = true;
        currentInputText = inputField.text;
        //selectedText.text = currentInputText;
    }

    public void SavedText()
    {
        //inputField.text = "";
        Debug.Log($"saving text: {currentInputText}");
        PlayerPrefs.SetString("SavedInputText", currentInputText);
        PlayerPrefs.Save();
        Debug.Log($"text saved: {PlayerPrefs.GetString("SavedInputText")}");
        countTimer = false;
        Debug.Log("time took: " + timer.ToString("F2"));

        bool checkValue = editedGameObject.GetComponent<anchorPlaneLocation>().testVariable;
        Debug.Log($"test result is: {checkValue}");

        group.gameObject.SetActive(true);


    }
    public ARCloudAnchorHistoryCollection LoadCloudAnchorHistory()
    {
        if (PlayerPrefs.HasKey(cloudAnchorsStorageKey))
        {
            var history = JsonUtility.FromJson<ARCloudAnchorHistoryCollection>(
                PlayerPrefs.GetString(cloudAnchorsStorageKey));

            // Remove all records created more than 24 hours and update stored history.
            DateTime current = DateTime.Now;
            history.Collection.RemoveAll(
                data => current.Subtract(data.CreatedTime).Days > 0);
            PlayerPrefs.SetString(cloudAnchorsStorageKey,
                JsonUtility.ToJson(history));
            return history;
        }

        return new ARCloudAnchorHistoryCollection();
    }
    public void SaveCloudAnchorHistory(ARCloudAnchorHistory data)
    {
        var history = LoadCloudAnchorHistory();

        // Sort the data from latest record to oldest record which affects the option order in
        // multiselection dropdown.
        history.Collection.Add(data);
        history.Collection.Sort((left, right) => right.CreatedTime.CompareTo(left.CreatedTime));

        // Remove the oldest data if the capacity exceeds storage limit.
        if (history.Collection.Count > storageLimit)
        {
            history.Collection.RemoveRange(
                storageLimit, history.Collection.Count - storageLimit);
        }

        PlayerPrefs.SetString(cloudAnchorsStorageKey, JsonUtility.ToJson(history));
    }


}
