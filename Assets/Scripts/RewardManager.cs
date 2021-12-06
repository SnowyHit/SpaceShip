using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardManager : MonoBehaviour
{
    ShipController shipController;
    private void Start()
    {
        shipController = GameObject.FindObjectOfType<ShipController>();
    }
    // Start is called before the first frame update
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Destroy(gameObject, 0.1f);
            shipController.EarnReward();
        }
    }
}
