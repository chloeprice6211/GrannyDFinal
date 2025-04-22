using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ObjectiveType
{
    Basement,
    Bathroom,
    Childroom,
    Garage,
    Car,
    Locker,
    DocumentShelf,
    Fusebox,

    ______,

    LVL2_Child,
    LVL2_Locker,
    LVL2_Garage,
    LVL2_Main,
    LVL2_Backyard,
    LVL2_Office,
    LVL2_ChildBathroom,
    LVL2_BathroomExit,
    LVL2_Toilet,
    LVL2_ToiletBathroom,
    LVL2_Trunk,
    LVL2_SafeLocker,
    LVL2_Gate,

    _______,

    LVL3_WasherRoom,
    LVL3_Bathroom,
    LVL3_BedroomBathroom,
    LVL3_Bedroom,
    LVL3_Childroom,
    LVL3_Garage,
    LVL3_ChildBedroom,
    LVL3_Main,
    LVL3_Toilet,
    LVL3_Office,
    LVL3_Fusebox,
    LVL3_Truck,
    LVL3_Gearbox,
    LVL3_FileCabinet,
    LVL3_ArmoredShelf,
    ________,
    LVL4Bedroom,
    LVL4Exit,
    LVL4LivingToilet,
    LVL4WashingRoom,
    LVL4OfficeRoom,
    LVL4BackyardDoor,
    LVL4Generator,

    None
}

public class Key : Item
{
    public ObjectiveType objectiveType;
}
