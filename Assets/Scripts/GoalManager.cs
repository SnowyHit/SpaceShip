using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class GoalManager : MonoBehaviour
{
    public float DistanceThreshold; 
    public float Distance;
    public List<Vector3> GoalPositions;
    public List<Vector3> GoalPositionsPath;
    private int hitCount; 
    public int HitGoalAmount; 
    private bool isMoving  = false;
    UIManager uiManager;
    public GameObject BlowUpPrefab; 
    public GameObject BulletPrefab; 
    GameObject player; 
    // Start is called before the first frame update
    void Start()
    {
        uiManager = GameObject.FindObjectOfType<UIManager>();
        GoalPositionsPath = new List<Vector3>(GoalPositions);
        player = GameObject.FindGameObjectWithTag("Player");
        transform.DOMove(GoalPositions[hitCount], 0.01f);
    }

    // Update is called once per frame
    void Update()
    {
        Distance = Vector3.Distance(player.transform.position, transform.position);
        if (Distance > DistanceThreshold)
        {
            transform.DOLookAt(player.transform.position, 0.1f);
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && !isMoving)
        {
            isMoving = true;
            StartCoroutine(HitGoal());
        }
    }
    
    IEnumerator HitGoal()
    {
        hitCount += 1;
        GoalPositionsPath.Clear();
        for (int i = hitCount; i < GoalPositions.Count; i++)
        {
            Debug.Log("Debug " + i);
            GoalPositionsPath.Add(GoalPositions[i]);
        }
        GameObject blowUp = Instantiate(BlowUpPrefab, transform.position, Quaternion.identity);
        blowUp.transform.DOScale(10, 0.1f);
        Destroy(blowUp ,2f);
        transform.GetComponent<MeshRenderer>().enabled = false;
        if(hitCount != HitGoalAmount)
        {
            transform.DOMove(GoalPositions[hitCount], 0.01f);
        }
        uiManager.AddScore(uiManager.TimeFloat / 2 + hitCount*2);
        uiManager.AddTime(10f);
        yield return new WaitForSeconds(0.1f);
        if (HitGoalAmount == hitCount)
        {
            uiManager.Win();
        }
        else
        {
            isMoving = false;
            transform.GetComponent<MeshRenderer>().enabled = true;
        }
    }
}
