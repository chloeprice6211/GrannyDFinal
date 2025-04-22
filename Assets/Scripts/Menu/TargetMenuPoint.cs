using static MenuManager;
using UnityEngine;

public class TargetMenuPoint
{
    public MenuPoint menuPoint;
    public GameObject uiHolder;
    public Transform cameraPosition;

    public TargetMenuPoint(MenuPoint _point, GameObject _uiHolder, Transform _cameraPosition)
    {
        menuPoint = _point;
        uiHolder = _uiHolder;
        cameraPosition = _cameraPosition;
    }

    public static TargetMenuPoint _menuPoint;
    public static TargetMenuPoint _setupPoint;
    public static TargetMenuPoint _coopPoint;
    public static TargetMenuPoint _endingPoint;

    public static TargetMenuPoint Menu
    {
        get
        {
            return _menuPoint;
        }
    }
    public static TargetMenuPoint Coop
    {
        get
        {
            return _coopPoint;
        }
    }
    public static TargetMenuPoint Setup
    {
        get
        {
            return _setupPoint;
        }
    }

}
