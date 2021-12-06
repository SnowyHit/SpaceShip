using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserPrefsGame1 : MonoBehaviour
{
    private int gold = 0;
    private int selectedShip = 0;
    private int selectedWeapon = 0;
    private int selectedAttachment = 0;

    public int GetGold()
    {
        return gold;
    }

    public int GetSelectedWeapon()
    {
        return selectedWeapon;
    }

    public int GetSelectedShip()
    {
        return selectedShip;
    }

    public int GetSelectedAttachment()
    {
        return selectedAttachment;
    }

    void Start()
    {
        ReadPrefs();
    }

    private void ReadPrefs()
    {
        gold = PlayerPrefs.GetInt("gold");
        selectedShip = PlayerPrefs.GetInt("selectedShip");
        selectedWeapon = PlayerPrefs.GetInt("selectedWeapon");
        selectedAttachment = PlayerPrefs.GetInt("selectedAttachment");
    }
}
