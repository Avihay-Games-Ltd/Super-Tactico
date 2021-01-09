using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class RotationButton : MonoBehaviour , IPointerDownHandler,IPointerUpHandler
{
    [SerializeField]
    GameObject RotationObject;
    [SerializeField]
    float RotationSpeed;
    int RotationFactor;
    bool isPressed;
    // Start is called before the first frame update
    void Start()
    {
        RotationSpeed = 10.0f;
        isPressed = false;
        if (name.Contains("Left"))
        {
            RotationFactor = 1;
        }
        else
        {
            RotationFactor = -1;
        }
    }
    private void Update()
    {
        if (isPressed)
        {
            RotationObject.gameObject.transform.Rotate(Vector3.up, RotationFactor*RotationSpeed* Time.deltaTime, Space.Self);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isPressed = true;
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        isPressed = false;
    }

}
