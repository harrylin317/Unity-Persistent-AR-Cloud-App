using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ButtonHeldEvent : MonoBehaviour
{
    private bool buttonHeldDown = false;
    private string currentButtonName = null;
    [SerializeField]
    private int rotationValue = 5;
    [SerializeField]
    private Vector3 scaleValue = new Vector3(0.01f, 0.01f, 0.01f);

    // Update is called once per frame
    void Update()
    {
        if (buttonHeldDown && ARPlacementManager.Instance.selectedObject != null)
        {
            Debug.Log("object is selected");

            if (currentButtonName == "ScaleArrowUp")
            {
                Debug.Log("scaling up");
                ARPlacementManager.Instance.selectedObject.transform.localScale += scaleValue;
            }
            else if (currentButtonName == "ScaleArrowDown" && ARPlacementManager.Instance.selectedObject.transform.localScale != new Vector3(0.01f, 0.01f, 0.01f))
            {
                Debug.Log("scaling down");
                ARPlacementManager.Instance.selectedObject.transform.localScale -= scaleValue;
            }
            else if (currentButtonName == "RotateArrowLeft")
            {
                Debug.Log("rotating right");
                ARPlacementManager.Instance.selectedObject.transform.Rotate(new Vector3(0, rotationValue, 0), Space.Self);
            }
            else if (currentButtonName == "RotateArrowRight")
            {
                Debug.Log("rotating left");
                ARPlacementManager.Instance.selectedObject.transform.Rotate(new Vector3(0, -rotationValue, 0), Space.Self);
            }
        }
    }
    public void HoldButton(Button button)
    {
        buttonHeldDown = true;
        currentButtonName = button.name;
        //Debug.Log($"Holding button: {currentButtonName}");
    }

    public void ReleaseButton(Button button)
    {
        buttonHeldDown = false;

    }
}
