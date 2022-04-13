using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

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

    private CloudAnchorHistory hostedCloudAnchor;

    // Start is called before the first frame update
    void Start()
    {
        GameObject getCanvas = editedGameObject.transform.GetChild(1).gameObject;
        selectedText = getCanvas.GetComponentInChildren<TextMeshProUGUI>();

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
        currentInputText = inputField.text;
        selectedText.text = currentInputText;
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


    }


}
