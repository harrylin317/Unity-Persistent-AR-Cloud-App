using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class getInput : MonoBehaviour
{
    [SerializeField]
    private TMP_InputField inputField;

    private string currentInputText = null;


    public void ReadStringInput()
    {
        currentInputText = inputField.text;
        //selectedText.text = currentInputText;
    }

    public void SaveName()
    {
        Debug.Log($"Saving name: {currentInputText}");
        ARPlacementManager.Instance.savedAnchorName = currentInputText;
        inputField.text = "";
        ARPlacementManager.Instance.nameInputGroup.gameObject.SetActive(false);
        ARPlacementManager.Instance.PlaceAnchor();
    }
}
