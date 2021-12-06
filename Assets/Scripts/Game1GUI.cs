using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;
using Newtonsoft.Json.Linq;

public class Game1GUI : MonoBehaviour
{
    public GameObject MainMenu;
    public GameObject Title;
    public GameObject WinConfirm;
    public GameObject WinDialog;
    public GameObject WinLoading;
    public GameObject WinLevelSelect;
    public GameObject WinHangar;
    public GameObject LevelItemOpen;
    public GameObject LevelItemLocked;
    public GameObject LevelItemNew;
    public UserPrefsGame1 User;
    public Text TxtCoins;
    public GameObject PnlLoading;
    public GameObject PnlLogin;
    public GameObject PnlRegister;
    public GameObject PnlRegistrationResult;
    public GameObject PnlOptions;
    public GameObject BtnLogout;
    public GameObject ShipCard;
    public GameObject CardPanel;

    public GameObject[] NFTShips;
    public Sprite[] NFTShipThumbnails;

    public class LevelItem
    {
        int number = 1;
        bool locked = true;
        bool completed = false;
        int stars = 0;

        public int Number
        {
            get
            {
                return number;
            }
        }
        public bool Completed
        {
            get
            {
                return completed;
            }
        }
        public bool Locked
        {
            get
            {
                return locked;
            }
        }
        public int Stars
        {
            get
            {
                return stars;
            }
        }

        public LevelItem(int number, bool locked)
        {
            this.number = number;
            this.locked = locked;
        }

        public LevelItem(int number, bool locked, int stars)
        {
            this.number = number;
            this.locked = locked;
            this.stars = stars;

            completed = true;
        }

        public void SetLocked(bool locked)
        {
            this.locked = locked;
        }
    }

    public LevelItem[] LevelItems;
    public SpaceShip[] Ships;

    public APIManager API;

    public delegate void OnEvent();

    int currentShip = 0;

    void Start()
    {
        PlayStartSequence();
        WinConfirm.SetActive(false);
        WinDialog.SetActive(false);
        WinHangar.SetActive(false);
        WinHangar.GetComponent<CanvasGroup>().alpha = 0;

        PnlLogin.SetActive(false);
        PnlRegister.SetActive(false);

        LevelItems = new LevelItem[21];
        for (int i = 0; i < LevelItems.Length; i++)
        {
            LevelItems[i] = new LevelItem(i + 1, i != 0);
            LevelItems[i].SetLocked(i == 0 ? false : (!PlayerPrefs.HasKey("Level" + (i + 1) + "Unocked") || (PlayerPrefs.GetInt("Level" + (i + 1) + "Unocked") != 1)));
        }


        PnlRegistrationResult.SetActive(false);

        TxtCoins.text = User.GetGold().ToString("N0");

        PnlOptions.SetActive(false);
        PnlLoading.SetActive(true);
        PnlLoading.GetComponent<CanvasGroup>().alpha = 0;
        PnlLoading.GetComponent<CanvasGroup>().DOFade(1, 0.33f);

        AttemptAutoLogin();
        TestIfSoundIsOn();
    }

    private void AttemptAutoLogin()
    {
        StartCoroutine(AttemptAutoLoginNow());
    }

    IEnumerator AttemptAutoLoginNow()
    {
        yield return new WaitForSeconds(0.33f);

        if (PlayerPrefs.HasKey("gamePlayerID") && !string.IsNullOrEmpty(PlayerPrefs.GetString("gamePlayerID")))
        {
            // login
            string userName = PlayerPrefs.GetString("gamePlayerID");
            string password = PlayerPrefs.GetString("password");
            DoLogin(userName, password);
        }
        else
        {
            // register
            PnlLogin.transform.SetAsLastSibling();
            PnlLogin.SetActive(true);
            PnlLogin.GetComponent<CanvasGroup>().alpha = 0;
            PnlLogin.GetComponent<CanvasGroup>().DOFade(1, 0.33f);

            yield return new WaitForSeconds(0.33f);

            PnlLoading.SetActive(false);
        }
    }

    public void OnClickSignin()
    {
        StartCoroutine(OnClickSigninNow());
    }

    IEnumerator OnClickSigninNow()
    {
        InputField inpUserName = PnlLogin.transform.Find("SignIn/InputField").GetComponent<InputField>();
        InputField inpPassword = PnlLogin.transform.Find("SignIn/InputField (1)").GetComponent<InputField>();

        if (string.IsNullOrEmpty(inpUserName.text.Trim()) || string.IsNullOrEmpty(inpPassword.text.Trim()))
        {
            // show error
            Dialog("Please enter user name and password", null);
        }
        else
        {
            PnlLoading.transform.SetAsLastSibling();
            PnlLoading.SetActive(true);
            PnlLoading.GetComponent<CanvasGroup>().alpha = 0;
            PnlLoading.GetComponent<CanvasGroup>().DOFade(1, 0.33f);

            yield return new WaitForSeconds(0.33f);

            PnlLogin.SetActive(false);

            DoLogin(inpUserName.text.Trim(), inpPassword.text.Trim());
        }
    }

    public void OnClickSignup()
    {
        StartCoroutine(OnClickSignupNow());
    }

    IEnumerator OnClickSignupNow()
    {
        InputField inpEmail = PnlRegister.transform.Find("SignUp/InputField (2)").GetComponent<InputField>();
        InputField inpUserName = PnlRegister.transform.Find("SignUp/InputField").GetComponent<InputField>();
        InputField inpPassword = PnlRegister.transform.Find("SignUp/InputField (1)").GetComponent<InputField>();

        if (string.IsNullOrEmpty(inpEmail.text.Trim()) || string.IsNullOrEmpty(inpUserName.text.Trim()) || string.IsNullOrEmpty(inpPassword.text.Trim()))
        {
            // show error
            Dialog("Please fill all fields", null);
        }
        else
        {
            PnlLoading.transform.SetAsLastSibling();
            PnlLoading.SetActive(true);
            PnlLoading.GetComponent<CanvasGroup>().alpha = 0;
            PnlLoading.GetComponent<CanvasGroup>().DOFade(1, 0.33f);

            yield return new WaitForSeconds(0.33f);

            PnlRegister.SetActive(false);

            DoRegister(inpEmail.text.Trim(), inpUserName.text.Trim(), inpPassword.text.Trim());
        }
    }

    public void SwitchToLogin()
    {
        StartCoroutine(SwitchToLoginNow());
    }

    IEnumerator SwitchToLoginNow()
    {
        PnlLogin.transform.SetAsLastSibling();
        PnlLogin.SetActive(true);
        PnlLogin.GetComponent<CanvasGroup>().alpha = 0;
        PnlLogin.GetComponent<CanvasGroup>().DOFade(1, 0.33f);

        yield return new WaitForSeconds(0.33f);

        PnlRegister.SetActive(false);
    }

    public void SwitchToRegister()
    {
        StartCoroutine(SwitchToRegisterNow());
    }

    IEnumerator SwitchToRegisterNow()
    {
        PnlRegister.transform.SetAsLastSibling();
        PnlRegister.SetActive(true);
        PnlRegister.GetComponent<CanvasGroup>().alpha = 0;
        PnlRegister.GetComponent<CanvasGroup>().DOFade(1, 0.33f);

        yield return new WaitForSeconds(0.33f);

        PnlLogin.SetActive(false);
    }

    private void DoLogin(string userName, string password)
    {
        StartCoroutine(DoLoginNow(userName, password));
    }

    
    IEnumerator DoLoginNow(string userName, string password)
    {
        yield return new WaitForEndOfFrame();

        API.Login(userName, password, (param) =>
        {
            // login succeeded
            Debug.Log("Login succeeded");

            JObject o = (JObject)param;
            PlayerPrefs.SetString("gamePlayerID", userName);
            PlayerPrefs.SetString("user_name", o["data"]["getPlayer"]["player"]["username"].ToString());
            PlayerPrefs.SetString("password", o["data"]["getPlayer"]["player"]["password"].ToString());
            PlayerPrefs.SetString("c_chain_private_key", o["data"]["getPlayer"]["player"]["c_chain_private_key"].ToString());
            PlayerPrefs.SetString("c_chain_private_key_hex", o["data"]["getPlayer"]["player"]["c_chain_private_key_hex"].ToString());

            Debug.Log("Fetching user tokens...");
            API.GetPlayerTokens(userName, (result) =>
            {
                PlayerPrefs.SetFloat("tokens", (float)result);
                Debug.Log("User's tokens: " + result.ToString());

                WinHangar.transform.Find("Top/Coins (1)/Text").GetComponent<Text>().text = result.ToString();
            });

            Debug.Log("Fetching user NFTs...");
            foreach (Transform child in CardPanel.transform)
            {
                Destroy(child);
            }
            API.GetNFTs(userName, (result) =>
            {
                try
                {
                    JObject o = JObject.Parse(result.ToString());

                    if (o["data"]["getPlayerOwners"]["info"]["status"].ToString() != "true")
                    {
                        Debug.LogError("Can't parse user NFTs: " + o["data"]["getPlayerOwners"]["info"]["message"].ToString());
                    }
                    else
                    {
                        List<SpaceShip> ships = new List<SpaceShip>();
                        ships.Add(Ships[0]);
                        AddCard(Ships[0]);

                        JArray arr = (JArray)o["data"]["getPlayerOwners"]["playerOwners"];
                        JObject metadata;
                        for (int i = 0; i < arr.Count; i++)
                        {
                            metadata = JObject.Parse(arr[i]["metadata_json"].ToString());
                            string itemName = metadata["name"].ToString();

                            JArray marr = (JArray)metadata["attributes"];
                            for (int j = 0; j < marr.Count; j++)
                            {
                                if (marr[j]["trait_type"].ToString() == "Type" && marr[j]["value"].ToString() == "ship")
                                {
                                    // ship
                                    SpaceShip ship = new SpaceShip();
                                    ship.Name = itemName;
                                    GameObject prefab = null;
                                    Sprite prefabImage = null;

                                    for (int k = j + 1; k < marr.Count; k++)
                                    {
                                        if (marr[k]["trait_type"].ToString() == "Internal Id")
                                        {
                                            switch (marr[k]["value"].ToString())
                                            {
                                                case "1003":
                                                    prefab = NFTShips[0];
                                                    prefabImage = NFTShipThumbnails[0];
                                                    break;
                                                case "1004":
                                                    prefab = NFTShips[1];
                                                    prefabImage = NFTShipThumbnails[1];
                                                    break;
                                                case "1005":
                                                    prefab = NFTShips[2];
                                                    prefabImage = NFTShipThumbnails[2];
                                                    break;
                                                default:
                                                    Debug.LogWarning("Unknown ship ID: " + arr[i]["id"].ToString());
                                                    continue;
                                            }

                                            ship.UniqID = marr[k]["value"].ToString();
                                            ship.Prefab = prefab;
                                            ship.PrefabImage = prefabImage;
                                        }
                                        else if (marr[k]["trait_type"].ToString() == "Life")
                                        {
                                            ship.Life = int.Parse(marr[k]["value"].ToString());
                                        }
                                        else if (marr[k]["trait_type"].ToString() == "Fire Power")
                                        {
                                            ship.FirePower = int.Parse(marr[k]["value"].ToString());
                                        }
                                        else if (marr[k]["trait_type"].ToString() == "Armor")
                                        {
                                            ship.Armor = int.Parse(marr[k]["value"].ToString());
                                        }
                                        else if (marr[k]["trait_type"].ToString() == "Speed")
                                        {
                                            ship.Speed = int.Parse(marr[k]["value"].ToString());
                                        }
                                    }

                                    ships.Add(ship);
                                    AddCard(ship);
                                    break;
                                }
                            }
                        }
                        Ships = ships.ToArray();
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError("Can't parse user NFTs: " + ex.Message);
                }
            });

            Debug.Log("Displaying main menu...");
            PlayerPrefs.SetString("selectedShip", Ships[currentShip].UniqID.ToString());
            FromLoginToMainMenu();
        }, (param) =>
        {
            // login failed
            Debug.Log("Login failed");

            Dialog("Login failed", () =>
            {
                SwitchToLogin();
            });
        });
    }

    public void AddCard(SpaceShip ship)
    {
        Debug.Log("Started Cards" + ship.Name.ToString());
        GameObject cardGO = Instantiate(ShipCard, Vector3.zero, Quaternion.identity , CardPanel.transform);
        cardGO.GetComponentInChildren<Image>().sprite = ship.PrefabImage;
        cardGO.transform.GetChild(0).GetComponent<Text>().text = ship.Name;
        cardGO.transform.GetChild(2).GetChild(1).GetComponent<Text>().text = ship.Speed.ToString();
        cardGO.GetComponent<ShipController>().UniqID = ship.UniqID;
        Debug.Log("Ended Cards");
    }
    private void FromLoginToMainMenu()
    {
        StartCoroutine(FromLoginToMainMenuNow());
    }

    IEnumerator FromLoginToMainMenuNow()
    {
        PnlLoading.GetComponent<CanvasGroup>().alpha = 1;
        PnlLoading.GetComponent<CanvasGroup>().DOFade(0, 0.33f);

        yield return new WaitForSeconds(0.33f);

        PnlLoading.SetActive(false);

        PlayStartSequence();
    }

    private void DoRegister(string email, string userName, string password)
    {
        API.Register(email, userName, password, (param) =>
        {
            // succeeded
            JObject o = (JObject)param;
            PlayerPrefs.SetString("user_name", o["data"]["createPlayer"]["player"]["username"].ToString());
            PlayerPrefs.SetString("password", o["data"]["createPlayer"]["player"]["password"].ToString());
            PlayerPrefs.SetString("email", email);
            PlayerPrefs.SetString("id", o["data"]["createPlayer"]["player"]["id"].ToString());
            PlayerPrefs.SetString("gamePlayerID", o["data"]["createPlayer"]["player"]["gamePlayerID"].ToString());
            PlayerPrefs.SetString("x_chain_public_key", o["data"]["createPlayer"]["player"]["x_chain_public_key"].ToString());
            PlayerPrefs.SetString("x_chain_private_key", o["data"]["createPlayer"]["player"]["x_chain_private_key"].ToString());
            PlayerPrefs.SetString("c_chain_public_key", o["data"]["createPlayer"]["player"]["c_chain_public_key"].ToString());
            PlayerPrefs.SetString("c_chain_private_key", o["data"]["createPlayer"]["player"]["c_chain_private_key"].ToString());
            PlayerPrefs.SetString("c_chain_private_key_hex", o["data"]["createPlayer"]["player"]["c_chain_private_key_hex"].ToString());

            DisplayRegistrationResult();

            Debug.Log("User creation succeeded");
        }, (param) =>
        {
            // failed
            Dialog("Registration failed for the following reason: " + param.ToString(), null);
        });
    }

    public void DisplayRegistrationResult()
    {
        StartCoroutine(DisplayRegistrationResultNow());
    }
    
    IEnumerator DisplayRegistrationResultNow()
    {
        PnlRegistrationResult.transform.SetAsLastSibling();
        PnlRegistrationResult.SetActive(true);
        PnlRegistrationResult.GetComponent<CanvasGroup>().alpha = 0;
        PnlRegistrationResult.GetComponent<CanvasGroup>().DOFade(1, 0.33f);

        //PnlRegistrationResult.transform.Find("Result/InputFieldXChainPublic").GetComponent<InputField>().text = PlayerPrefs.GetString("x_chain_public_key");
        //PnlRegistrationResult.transform.Find("Result/InputFieldXChainPrivate").GetComponent<InputField>().text = PlayerPrefs.GetString("x_chain_private_key");
        //PnlRegistrationResult.transform.Find("Result/InputFieldCChainPublic").GetComponent<InputField>().text = PlayerPrefs.GetString("c_chain_public_key");
        PnlRegistrationResult.transform.Find("Result/InputFieldUserName").GetComponent<InputField>().text = PlayerPrefs.GetString("user_name");
        PnlRegistrationResult.transform.Find("Result/InputFieldPassword").GetComponent<InputField>().text = PlayerPrefs.GetString("password");
        PnlRegistrationResult.transform.Find("Result/InputFieldCChainPrivate").GetComponent<InputField>().text = PlayerPrefs.GetString("c_chain_private_key");
        PnlRegistrationResult.transform.Find("Result/InputFieldCChainPrivateHex").GetComponent<InputField>().text = PlayerPrefs.GetString("c_chain_private_key_hex");

        yield return new WaitForSeconds(0.33f);

        PnlRegister.SetActive(false);
    }

    public void CloseRegistrationResult()
    {
        StartCoroutine(CloseRegistrationResultNow());
    }

    IEnumerator CloseRegistrationResultNow()
    {
        PnlRegistrationResult.GetComponent<CanvasGroup>().alpha = 1;
        PnlRegistrationResult.GetComponent<CanvasGroup>().DOFade(0, 0.33f);

        yield return new WaitForSeconds(0.33f);

        PnlRegistrationResult.SetActive(false);

        if (!PnlOptions.activeSelf)
        {
            PnlLoading.SetActive(false);
            PlayStartSequence();
        }
    }


    void PlayStartSequence()
    {
        MainMenu.GetComponent<CanvasGroup>().alpha = 0;
        MainMenu.GetComponent<RectTransform>().anchoredPosition = new Vector2(600, -50);

        MainMenu.GetComponent<CanvasGroup>().DOFade(1, 1);
        MainMenu.GetComponent<RectTransform>().DOAnchorPosX(-600, 0.5f);

        Title.GetComponent<CanvasGroup>().alpha = 0;
        Title.GetComponent<CanvasGroup>().DOFade(1, 1);

        Title.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 90);
        Title.GetComponent<RectTransform>().DOAnchorPosY(-100, 0.5f);

        WinLevelSelect.SetActive(false);
        BtnLogout.SetActive(true);
    }

    public void BtnPlayClick()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void BtnQuitClick()
    {
        Confirm("Are you sure you want to quit?", () => { Application.Quit(); }, null);
    }

    public void Confirm(string message, OnEvent onYes, OnEvent onNo)
    {
        WinConfirm.SetActive(true);
        WinConfirm.GetComponent<CanvasGroup>().alpha = 0;
        WinConfirm.GetComponent<CanvasGroup>().DOFade(1, 1);
        WinConfirm.transform.Find("Window/Text").GetComponent<Text>().text = message;

        var btnYes = WinConfirm.transform.Find("Window/BtnYes").GetComponent<Button>();
        var btnNo = WinConfirm.transform.Find("Window/BtnNo").GetComponent<Button>();
        var btnClose = WinConfirm.transform.Find("Window/BtnClose").GetComponent<Button>();

        btnYes.onClick.RemoveAllListeners();
        btnYes.onClick.AddListener(() =>
        {
            onYes?.Invoke();
            WinConfirm.SetActive(false);
        });

        btnNo.onClick.RemoveAllListeners();
        btnNo.onClick.AddListener(() =>
        {
            onNo?.Invoke();
            WinConfirm.SetActive(false);
        });

        btnClose.onClick.RemoveAllListeners();
        btnClose.onClick.AddListener(() =>
        {
            onNo?.Invoke();
            WinConfirm.SetActive(false);
        });
    }

    public void Dialog(string message, OnEvent onOK)
    {
        WinDialog.transform.SetAsLastSibling();
        WinDialog.SetActive(true);
        WinDialog.GetComponent<CanvasGroup>().alpha = 0;
        WinDialog.GetComponent<CanvasGroup>().DOFade(1, 1);
        WinDialog.transform.Find("Window/Text").GetComponent<Text>().text = message;

        var btnYes = WinDialog.transform.Find("Window/BtnYes").GetComponent<Button>();
        var btnClose = WinDialog.transform.Find("Window/BtnClose").GetComponent<Button>();

        btnYes.onClick.RemoveAllListeners();
        btnYes.onClick.AddListener(() =>
        {
            onOK?.Invoke();
            WinDialog.SetActive(false);
        });

        btnClose.onClick.RemoveAllListeners();
        btnClose.onClick.AddListener(() =>
        {
            onOK?.Invoke();
            WinDialog.SetActive(false);
        });
    }

    IEnumerator PlayGameSequence()
    {
        MainMenu.GetComponent<CanvasGroup>().DOFade(0, 0.5f);
        MainMenu.GetComponent<RectTransform>().DOAnchorPosX(600, 0.5f);
        Title.GetComponent<RectTransform>().DOAnchorPosY(90, 0.5f);
        Title.GetComponent<CanvasGroup>().DOFade(0, 0.5f);
        BtnLogout.SetActive(false);

        yield return new WaitForSeconds(0.5f);

        DisplayLevelWindow();
    }

    public void DisplayLevelWindow()
    {
        WinLevelSelect.SetActive(true);

        DisplayLevelButtons();

        WinLevelSelect.GetComponent<CanvasGroup>().DOFade(0, 0);
        WinLevelSelect.GetComponent<RectTransform>().DOAnchorPosX(600, 0);

        WinLevelSelect.GetComponent<CanvasGroup>().DOFade(1, 1);
        WinLevelSelect.GetComponent<RectTransform>().DOAnchorPosX(0, 0.5f);
    }

    private void DisplayLevelButtons()
    {
        GameObject element;
        for (int y = 0; y < 3; y++)
        {
            for (int x = 0; x < 7; x++)
            {
                if(LevelItems[y * 3 + x].Locked)
                {
                    element = Instantiate(LevelItemLocked, WinLevelSelect.transform);
                } else if(x != 6 && y != 2 && LevelItems[y * 3 + x + 1].Locked && !LevelItems[y * 3 + x].Locked)
                {
                    element = Instantiate(LevelItemNew, WinLevelSelect.transform);
                    element.GetComponent<Button>().onClick.RemoveAllListeners();
                    int currentLevel = (y * 3 + x + 1);
                    element.GetComponent<Button>().onClick.AddListener(() =>
                    {
                        PnlLoading.SetActive(true);
                        PlayerPrefs.SetInt("CurrentLevel", currentLevel);
                        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
                    });
                }
                else
                {
                    element = Instantiate(LevelItemOpen, WinLevelSelect.transform);
                    element.GetComponent<Button>().onClick.RemoveAllListeners();
                    element.GetComponent<Button>().onClick.AddListener(() =>
                    {
                        PnlLoading.SetActive(true);
                        int currentLevel = (y * 3 + x + 1);
                        PlayerPrefs.SetInt("CurrentLevel", currentLevel);
                        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
                    });

                    var star1 = element.transform.Find("Level Star/Background/Checkmark");
                    var star2 = element.transform.Find("Level Star (1)/Background/Checkmark");
                    var star3 = element.transform.Find("Level Star (2)/Background/Checkmark");

                    star1.gameObject.SetActive(LevelItems[y * 3 + x].Stars >= 1);
                    star2.gameObject.SetActive(LevelItems[y * 3 + x].Stars >= 2);
                    star3.gameObject.SetActive(LevelItems[y * 3 + x].Stars >= 3);
                }

                element.GetComponent<RectTransform>().anchoredPosition = new Vector2(-900 + 300 * x, 300 - 300 * y);
                element.transform.Find("Level number").GetComponent<Text>().text = (y * 3 + x + 1).ToString();
            }
        }
    }

    public void HideLevelWindow()
    {
        StartCoroutine(HideLevelWindowNow());
    }

    IEnumerator HideLevelWindowNow()
    {
        WinLevelSelect.GetComponent<CanvasGroup>().DOFade(0, 0.5f);
        WinLevelSelect.GetComponent<RectTransform>().DOAnchorPosX(600, 0.5f);

        yield return new WaitForSeconds(0.25f);

        PlayStartSequence();

        yield return new WaitForSeconds(0.5f);

        WinLevelSelect.SetActive(false);
    }

    public void PlayHangarSequence()
    {
        StartCoroutine(PlayHangarSequenceNow());
    }

    IEnumerator PlayHangarSequenceNow()
    {
        MainMenu.GetComponent<CanvasGroup>().DOFade(0, 0.5f);
        MainMenu.GetComponent<RectTransform>().DOAnchorPosX(600, 0.5f);
        Title.GetComponent<RectTransform>().DOAnchorPosY(90, 0.5f);
        Title.GetComponent<CanvasGroup>().DOFade(0, 0.5f);
        SelectShipSTR(PlayerPrefs.HasKey("selectedShip") ? PlayerPrefs.GetString("selectedShip"): "1001");
        yield return new WaitForSeconds(0.5f);

        SwitchToHangar();
    }

    public void SwitchToHangar()
    {
        WinHangar.SetActive(true);

        WinHangar.GetComponent<CanvasGroup>().DOFade(0, 0);
        WinHangar.GetComponent<RectTransform>().DOAnchorPosX(600, 0);

        WinHangar.GetComponent<CanvasGroup>().DOFade(1, 1);
        WinHangar.GetComponent<RectTransform>().DOAnchorPosX(0, 0.5f);

        DisplayShip(0);
    }

    public void HideHangarWindow()
    {
        StartCoroutine(HideHangarWindowNow());
    }

    IEnumerator HideHangarWindowNow()
    {
        WinHangar.GetComponent<CanvasGroup>().DOFade(0, 0.5f);
        WinHangar.GetComponent<RectTransform>().DOAnchorPosX(600, 0.5f);
        DisplayShip(-1);

        yield return new WaitForSeconds(0.25f);

        PlayStartSequence();

        yield return new WaitForSeconds(0.5f);

        WinHangar.SetActive(false);
    }

    void DisplayShip(int index)
    {
        for (int i = 0; i < Ships.Length; i++)
        {
            Debug.Log("Ship name :" + Ships[i].Name.ToString());
        }
    }

    public void DisplayOptions()
    {
        Transform toggleSound = PnlOptions.transform.Find("Window/TglSound");
        Toggle toggle = toggleSound.GetComponent<Toggle>();
        toggle.isOn = !PlayerPrefs.HasKey("Sound") || (PlayerPrefs.GetInt("Sound") == 1);

        PnlOptions.GetComponent<CanvasGroup>().alpha = 0;
        PnlOptions.SetActive(true);

        PnlOptions.GetComponent<CanvasGroup>().DOFade(1, 0.33f);
    }

    public void CloseOptions()
    {
        StartCoroutine(CloseOptionsNow());
    }

    IEnumerator CloseOptionsNow()
    {
        PnlOptions.GetComponent<CanvasGroup>().DOFade(0, 0.33f);

        yield return new WaitForSeconds(0.33f);

        PnlOptions.SetActive(false);
    }

    public void ToggleSound()
    {
        Transform toggleSound = PnlOptions.transform.Find("Window/TglSound");
        Toggle toggle = toggleSound.GetComponent<Toggle>();

        PlayerPrefs.SetInt("Sound", toggle.isOn ? 1 : 0);
        TestIfSoundIsOn();
    }

    private void TestIfSoundIsOn()
    {
        transform.Find("/Main Camera").GetComponent<AudioListener>().enabled = !PlayerPrefs.HasKey("Sound") || (PlayerPrefs.GetInt("Sound") == 1);
    }

    public void Logout()
    {
        Confirm("Are you sure you want to logout?", () => {
            PlayerPrefs.DeleteKey("gamePlayerID");
            
            MainMenu.GetComponent<CanvasGroup>().DOFade(0, 0.5f);
            MainMenu.GetComponent<RectTransform>().DOAnchorPosX(600, 0.5f);
            Title.GetComponent<RectTransform>().DOAnchorPosY(90, 0.5f);
            Title.GetComponent<CanvasGroup>().DOFade(0, 0.5f);
            BtnLogout.SetActive(false);

            SwitchToLogin();
        }, null);
    }

    public void DisplayLoading(string message)
    {
        WinLoading.transform.SetAsLastSibling();
        WinLoading.SetActive(true);
        WinLoading.GetComponent<CanvasGroup>().alpha = 0;
        WinLoading.GetComponent<CanvasGroup>().DOFade(1, 1);
        WinLoading.transform.Find("Window/Text").GetComponent<Text>().text = message;
    }

    public void HideLoading()
    {
        StartCoroutine(HideLoadingNow());
    }

    IEnumerator HideLoadingNow()
    {
        WinLoading.GetComponent<CanvasGroup>().DOFade(0, 1);

        yield return new WaitForSeconds(1);

        WinLoading.SetActive(false);
    }
    public void SelectShip(ShipController shipController)
    {
        CardPanel.GetComponent<CardManager>().AdjustSelected(shipController.UniqID);
        PlayerPrefs.SetString("selectedShip", shipController.UniqID);
        foreach (var ship in Ships)
        {
            if(ship.UniqID == shipController.UniqID)
            {
                PlayerPrefs.SetFloat("selectedShipSpeed", ship.Speed);
                PlayerPrefs.SetFloat("selectedShipLife", ship.Life);
            }
        }
        Debug.Log("Ship has been selected");
    }
    public void SelectShipSTR(string uniqID)
    {
        CardPanel.GetComponent<CardManager>().AdjustSelected(uniqID);
        PlayerPrefs.SetString("selectedShip", uniqID);
        foreach (var ship in Ships)
        {
            if (ship.UniqID == uniqID)
            {
                PlayerPrefs.SetFloat("selectedShipSpeed", ship.Speed);
                PlayerPrefs.SetFloat("selectedShipLife", ship.Life);
            }
        }
        Debug.Log("Ship has been selected");
    }
}