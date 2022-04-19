using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectSelectionMenu : MonoBehaviour
{
    [SerializeField]
    private GameObject optionGroup;
    [SerializeField]
    private Button optionButton;
    public void ObjectSelectionMenuButtonPressed()
    {
        Debug.Log("object selection menu pressed");

        gameObject.SetActive(true);
        optionGroup.SetActive(false);
        ARPlacementManager.Instance.updateUI = false;
        ARPlacementManager.Instance.selectObjectButtons.SetActive(false);
        ARPlacementManager.Instance.scanningText.gameObject.SetActive(false);
        ARPlacementManager.Instance.placeObjectButton.gameObject.SetActive(false);
    }

    public void ObjectIconPressed(int value)
    {
        Debug.Log($"selected new object: {value}");
        ARPlacementManager.Instance.objectToPlaceIndex = value;
        gameObject.SetActive(false);
        optionButton.gameObject.SetActive(true);
        ARPlacementManager.Instance.updateUI = true;
        ARPlacementManager.Instance.selectObjectButtons.SetActive(true);
        ARPlacementManager.Instance.scanningText.gameObject.SetActive(true);
        ARPlacementManager.Instance.placeObjectButton.gameObject.SetActive(true);
    }

    public void ObjectSelectionMenuExitButtonPressed()
    {
        Debug.Log("Quiting object selection menu");

        gameObject.SetActive(false);
        optionGroup.SetActive(true);
    }
}
