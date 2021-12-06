using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipsManager : MonoBehaviour
{
    public List<GameObject> SpaceShips;
    public string SelectedShip;
    public int FirePower;
    public int Armor;
    public int Life;
    public int Speed;
    // Start is called before the first frame update
    private void Awake()
    {
        //Set Player Selection Prefab
        GameObject playerShip = FindSpaceShip(PlayerPrefs.GetString("selectedShip"));
        //Setship controller accordingly to stats
        playerShip.SetActive(true);
        playerShip.transform.position = Vector3.zero;
        playerShip.tag = "Player";
        ShipController playerShipController = playerShip.GetComponent<ShipController>();
        float shipLife = PlayerPrefs.GetFloat("selectedShipLife");
        float shipSpeed = PlayerPrefs.GetFloat("selectedShipSpeed");
        playerShipController.maxThrottle = shipLife * shipSpeed * 10;
        playerShipController.throttleAmount = shipSpeed * 10;
        playerShipController.minThrottle = shipSpeed * 5;
        playerShipController.yawStrength = 50 + shipSpeed * 10;
        playerShipController.pitchStrength = 50 + shipSpeed * 10;
        playerShipController.rollStrength = shipSpeed * 5;

    }
    public GameObject FindSpaceShip(string uniqID)
    {
        foreach(GameObject spaceShip in SpaceShips)
        {
            if(spaceShip.GetComponent<ShipController>().UniqID == uniqID)
            {
                return spaceShip;
            }
        }
        Debug.LogError("Cant Found Ship");
        return null ;
    }
}
