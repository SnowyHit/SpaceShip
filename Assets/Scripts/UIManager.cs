using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public GameObject InGameScreen;
    public GameObject LoadingScreen;
    public GameObject WinScreen;
    public GameObject LoseScreen;
    public Text WinNftText;
    public Text WinScoreText;
    public Text time;
    public float TimeFloat;
    public float ScoreFloat;
    ShipController shipController;
    APIManager apiManager;
    public float RemainingTime;
    public bool EndGame = false;
    // Start is called before the first frame update
    void Start()
    {
        shipController = FindObjectOfType<ShipController>();
        //apiManager = FindObjectOfType<APIManager>();
        InGameScreen.SetActive(false);
        LoadingScreen.SetActive(true);
        WinScreen.SetActive(false);
        LoseScreen.SetActive(false);
        StartCoroutine(LoadingScreenEnumarator());
        TimeFloat = RemainingTime;
    }
    IEnumerator LoadingScreenEnumarator()
    {
        for (int i = 0; i < 100; i++)
        {
            yield return new WaitForSeconds(0.01f);
            LoadingScreen.GetComponentInChildren<Slider>().value += (float)(i / 100f);
        }

        InGameScreen.SetActive(true);
        InGameScreen.GetComponent<Image>().DOFade(0 , 0.3f);
        LoadingScreen.transform.DOScale(0 , 0.2f);
        yield return new WaitForSeconds(0.3f);
        LoadingScreen.SetActive(false);
        StartCoroutine(StartTimer());
    }
    IEnumerator StartTimer()
    {
        while(true)
        {
            yield return new WaitForSeconds(Time.deltaTime);
            TimeFloat -= Time.deltaTime;
            if(TimeFloat <= 0)
            {
                shipController.IsDead = true ;
                TimeFloat = 0;
                break;
            }
            if(EndGame)
            { 
                break;
            }
        }
    }
    private void FixedUpdate()
    {
        if(!shipController.IsDead)
        {
            time.text = TimeFloat.ToString("F1");
        }
    }
    public void AddScore(float score)
    {
        ScoreFloat += score;
    }

    public void AddTime(float time)
    {
        TimeFloat += time;
    }
    public void GetReward(Button rewardButton)
    {
        rewardButton.interactable = false;
        //apiManager.SendTokensToPlayer(PlayerPrefs.GetString("gamePlayerID"), ScoreFloat, (param) => { Debug.Log(ScoreFloat.ToString()+" Coin Has Sent To Player"); });
        if(shipController.RewardCount >= 1)
        {
            //apiManager.Mint(PlayerPrefs.GetString("gamePlayerID"), 56 , (param) => { Debug.Log(" NFT Has Sent To Player"); });
        }
    }
    public void LoadScene(int Scene)
    {
        SceneManager.LoadScene(Scene);
    }
    public void NextScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    public void ReLoadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void Win()
    {
        //Set Win Screen
        Cursor.visible = true;
        shipController.Lock(true);
        StopCoroutine(StartTimer());
        StartCoroutine(WinScreenCoroutine());
        WinScoreText.text = ScoreFloat.ToString();
        if(shipController.RewardCount >=1)
        {
            int NFTcount = Mathf.FloorToInt(shipController.RewardCount / 1);
            WinNftText.text = "You Have "+ NFTcount.ToString() + "NFT's Available to mint !";
            WinNftText.color = Color.green;
        }
        else
        {
            WinNftText.text = "No Reward ! ";
            WinNftText.color = Color.red;
        }
        EndGame = true;
        PlayerPrefs.SetInt("Level" + (SceneManager.GetActiveScene().buildIndex + 1) + "Unocked" , 1);
    }
    IEnumerator WinScreenCoroutine()
    {
        InGameScreen.transform.DOScale(0, 0.3f);
        yield return new WaitForSeconds(0.35f);
        InGameScreen.SetActive(false);
        WinScreen.transform.DOScale(0, 0.00001f);
        WinScreen.SetActive(true);
        WinScreen.transform.DOScale(1, 0.3f);
    }
    IEnumerator LoseScreenCoroutine()
    {
        InGameScreen.transform.DOScale(0, 0.3f);
        yield return new WaitForSeconds(0.35f);
        InGameScreen.SetActive(false);
        LoseScreen.transform.DOScale(0, 0.00001f);
        LoseScreen.SetActive(true);
        LoseScreen.transform.DOScale(1, 0.3f);
    }
    public void Lose()
    {
        Cursor.visible = true;
        shipController.Lock(true);
        StartCoroutine(LoseScreenCoroutine());
        EndGame = true;
    }
}
