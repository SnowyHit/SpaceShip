using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardManager : MonoBehaviour
{
    
    public void AdjustSelected(string uniqID)
    {
        foreach (Transform child in transform)
        {
            if(uniqID == child.GetComponent<ShipController>().UniqID)
            {
                child.transform.GetChild(3).GetComponentInChildren<Button>().interactable = false;
                child.transform.GetChild(3).GetComponentInChildren<Text>().text = "Current";
            }
            else
            {
                child.transform.GetChild(3).GetComponentInChildren<Button>().interactable = true;
                child.transform.GetChild(3).GetComponentInChildren<Text>().text = "Select";
            }
        }
    }
}
