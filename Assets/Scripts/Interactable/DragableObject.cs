using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragableObject : MonoBehaviour
{
    public List<GameObject> objects;
    float multiplier;
    Vector2 mouseVector = new();
    int _currentDisplayedItemIndex = 0;
    [SerializeField] Transform objectToRotate;

    PlayerControls _controls;

    private void Awake()
    {
        _controls = new();
        _controls.Player.Look.Enable();
    }

    public void Drag()
    {
        mouseVector = _controls.Player.Look.ReadValue<Vector2>();
        objectToRotate.Rotate(0, mouseVector.x * 0.7f, mouseVector.y * 0.7f, Space.World);
    }

    public void DisplayItem(int index)
    {
        //Debug.Log(index);
        objectToRotate.localRotation = Quaternion.Euler(new Vector3(0,0,-9));

        objects[_currentDisplayedItemIndex].SetActive(false);
        objects[index].SetActive(true);
        _currentDisplayedItemIndex = index;
    }

    public void HideItem()
    {
        objects[_currentDisplayedItemIndex].SetActive(false);
    }
}
