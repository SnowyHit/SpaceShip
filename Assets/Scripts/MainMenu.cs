using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MainMenu : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Moving());
    }

    IEnumerator Moving()
    {
        while(true)
        {
            yield return new WaitForSeconds(5f);
            transform.DOLocalMoveZ(transform.position.z + 100, 5f);
        }
    }
}
