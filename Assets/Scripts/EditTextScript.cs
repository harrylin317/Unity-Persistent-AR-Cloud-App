using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class EditTextScript : MonoBehaviour
{
    [SerializeField]
    private TMP_InputField inputField;
    [SerializeField]
    private GameObject optionGroup;
    [SerializeField]
    private Button optionButton;
    private TMPro.TextMeshPro selectedText = null;
    private TMPro.TextMeshPro titleText = null;
    private TMPro.TextMeshPro bodyText = null;
    private bool exitEditing = false;

    public void EditTextButtonPressed()
    {
        gameObject.SetActive(true);
        optionButton.gameObject.SetActive(false);
        ARPlacementManager.Instance.updateUI = false;
        ARPlacementManager.Instance.editingText = true;
        ARPlacementManager.Instance.selectObjectButtons.SetActive(false);
        ARPlacementManager.Instance.scanningText.gameObject.SetActive(false);
        ARPlacementManager.Instance.placeObjectButton.gameObject.SetActive(false);

        titleText = ARPlacementManager.Instance.selectedObject.transform.GetChild(0).gameObject.GetComponentInChildren<TextMeshPro>();
        bodyText = ARPlacementManager.Instance.selectedObject.transform.GetChild(1).gameObject.GetComponentInChildren<TextMeshPro>();
        Debug.Log($"get title: {titleText.text}");
        Debug.Log($"get title: {bodyText.text}");
    }

    public void UpdateText()
    {
        if (exitEditing)
        {
            return;
        }
        Debug.Log("updated text");
        selectedText.text = inputField.text;
    }
    public void EditTitleButtonPressed()
    {
        selectedText = titleText;
        inputField.gameObject.SetActive(true);
        inputField.text = selectedText.text;

    }
    public void EditBodyButtonPressed()
    {
        selectedText = bodyText;
        inputField.gameObject.SetActive(true);
        inputField.text = selectedText.text;
    }

    public void EditTextExitButtonPressed()
    {
        exitEditing = true;
        inputField.text = "";
        exitEditing = false;

        inputField.gameObject.SetActive(false);
        gameObject.SetActive(false);
        optionButton.gameObject.SetActive(true);
        ARPlacementManager.Instance.updateUI = true;
        ARPlacementManager.Instance.editingText = false;
        ARPlacementManager.Instance.selectObjectButtons.SetActive(true);
        ARPlacementManager.Instance.scanningText.gameObject.SetActive(true);
        ARPlacementManager.Instance.placeObjectButton.gameObject.SetActive(true);
    }

    public void FontSizeIncrease()
    {
        selectedText.fontSize += 0.01f;
    }
    public void FontSizeDecrease()
    {

        selectedText.fontSize -= 0.01f;
        if (selectedText.fontSize < 0)
        {
            selectedText.fontSize = 0;
        }
        Debug.Log($"Size decreased to {selectedText.fontSize}");
    }

    
}
