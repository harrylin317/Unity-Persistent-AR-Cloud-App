using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class getInput : MonoBehaviour
{
    [SerializeField]
    private TMP_InputField inputField;
    [SerializeField]
    private Button optionButton;
    private string currentInputText = null;


    

    public void ReadStringInput()
    {
        currentInputText = inputField.text;
        //selectedText.text = currentInputText;
    }

    public void SaveName()
    {
        if(ARCloudAnchorManager.Instance.pendingHostAnchor.arAnchor != null)
        {
            Debug.Log("A object is currently being hosted, please wait till it finish");
            return;

        }
        Debug.Log($"Saving name: {currentInputText}");
        ARPlacementManager.Instance.savedAnchorName = currentInputText;
        inputField.text = "";
        ARPlacementManager.Instance.updateUI = true;
        gameObject.SetActive(false);
        optionButton.gameObject.SetActive(true);

        ARPlacementManager.Instance.PlaceAnchor();
    }
    public void SaveLocationButtonPress()
    {
        Debug.Log("save object location button pressed");

        ARPlacementManager.Instance.updateUI = false;
        gameObject.SetActive(true);
        optionButton.gameObject.SetActive(false);
        ARPlacementManager.Instance.selectObjectButtons.SetActive(false);
        ARPlacementManager.Instance.scanningText.gameObject.SetActive(false);
        ARPlacementManager.Instance.placeObjectButton.gameObject.SetActive(false);

    }
    public void SaveNameMenuExitButtonPressed()
    {
        Debug.Log("closing save name menu");
        ARPlacementManager.Instance.updateUI = true;
        gameObject.SetActive(false);
        optionButton.gameObject.SetActive(true);
        ARPlacementManager.Instance.selectObjectButtons.SetActive(true);
        ARPlacementManager.Instance.scanningText.gameObject.SetActive(true);
        ARPlacementManager.Instance.placeObjectButton.gameObject.SetActive(true);

    }
}
