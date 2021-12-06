using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Ship", menuName = "Inventuna/Hackathon/Space Ships", order = 1)]
public class SpaceShip : ScriptableObject
{
    public string UniqID;
    public int Life = 3;
    public int FirePower = 1;
    public int Armor = 0;
    public int Speed = 3;
    public int Level = 1;
    public Vector3 ScaleFactor = Vector3.one;

    public bool Unlocked = true;
    public bool Owned = true;
    public int Price = 0;
    public string Name = "Inferno";
    public GameObject Prefab;
    public Sprite PrefabImage;
}
