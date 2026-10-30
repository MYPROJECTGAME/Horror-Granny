using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

    [Header("Settings")]
    [Tooltip("Slider to controll volume")]
    public Slider volumeSlider;
    [Tooltip("Slider to controll sensitivity")]
    public Slider sensitivitySlider;
    public Toggle fullscreenToggle;
    public GameObject m_lockImageGameObject;
    public Button m_playButton;
    public Text m_playButtonText;
    public Font m_khmerFont;
    public GameObject m_startGameWindow;
    public GameObject m_setupGameWindow;
    public GameObject m_returnedFormGameWindow;
    public Text m_plus_Xp_text;
    public Text m_collectedPicturesCountText;

    [Header("Game Setup")]
    public string m_loadingSceneName;
    public LevelSetup[] m_levels;
    public EnemySetup[] m_enemys;
    public DifficultSetup[] m_difficults;
    int levelID = 0;
    int enemyID = 0;
    int difficultID = 0;

    [Header("XP Settings")]
    public Text m_xp_text;
    private static int m_XP;

    [Header("Level Settings")]
    public Text m_levelNameText;
    public Text m_levelDescriptionText;
    public GameObject m_buyLevelIconGO;
    public Text m_levelPriceText;
    public GameObject m_level_NotEnoughXP;
    bool m_selectedLevelIsLocked;


    [Header("Enemy Settings")]
    public Text m_enemyNameText;
    public Text m_enemyDescriptionText;
    public GameObject m_buyEnemyIconGO;
    public Text m_enemyPriceText;
    public GameObject m_enemy_NotEnoughXP;
    bool m_selectedEnemyIsLocked;


    [Header("Difficult Settings")]
    public Text m_difficultNameText;
    public Text m_difficultDescriptionText;

    private void OnValidate()
    {
        ApplyKhmerFont();
    }

    private void Awake()
    {
        ApplyKhmerFont();
        SetupFullscreenSetting();

        if (PlayerPrefs.HasKey("ReturnFromGame"))
        {
            int m_RFG = PlayerPrefs.GetInt("ReturnFromGame");
            if (m_RFG == 1)
            {

                m_returnedFormGameWindow.SetActive(true);
                m_startGameWindow.SetActive(false);

                if (PlayerPrefs.HasKey("PLUS_XP"))
                {
                 
                    int m_plus_XP = PlayerPrefs.GetInt("PLUS_XP");
                    m_XP += m_plus_XP;
                    PlayerPrefs.SetInt("XP", m_XP);
                    PlayerPrefs.Save();
                    m_plus_Xp_text.text = "+ " + m_plus_XP.ToString();

                }else
                {
                    m_XP += 0;
                    PlayerPrefs.SetInt("XP", m_XP);
                    PlayerPrefs.Save();
                    m_plus_Xp_text.text = "+ " + 0;
                }

                if(PlayerPrefs.HasKey("CollectedPictures"))
                {
                   m_collectedPicturesCountText.text = "ឯងទទួល";
                }

                PlayerPrefs.SetInt("ReturnFromGame", 0);
                PlayerPrefs.SetInt("CollectedPictures", 0);
            }
        }


        if (PlayerPrefs.HasKey("XP"))
        {
            m_XP = PlayerPrefs.GetInt("XP");
        }else
        {
            m_XP = 0;
        }
        m_xp_text.text = m_XP.ToString() + " XP";


        if (PlayerPrefs.HasKey("Volume"))
        {
            AudioListener.volume = PlayerPrefs.GetFloat("Volume");
            volumeSlider.value = PlayerPrefs.GetFloat("Volume");
        }

        if (PlayerPrefs.HasKey("Fullscreen"))
        {
            Screen.fullScreen = PlayerPrefs.GetInt("Fullscreen") == 1;
        }
        else
        {
            Screen.fullScreen = true;
        }

        if (fullscreenToggle != null)
        {
            fullscreenToggle.SetIsOnWithoutNotify(Screen.fullScreen);
        }
        else
        {
            AudioListener.volume = 1f;
            volumeSlider.value = 1f;
        }

        if (PlayerPrefs.HasKey("Sensitivity"))
        {         
            sensitivitySlider.value = PlayerPrefs.GetFloat("Sensitivity");
        }
        else
        {
            sensitivitySlider.value = sensitivitySlider.maxValue / 2f;
        }

        GameSetup();
    }

    private void ApplyKhmerFont()
    {
        if (m_khmerFont == null)
        {
            return;
        }

        Text[] menuTexts = GetComponentsInChildren<Text>(true);
        for (int i = 0; i < menuTexts.Length; i++)
        {
            menuTexts[i].font = m_khmerFont;
        }
    }

    private void SetupFullscreenSetting()
    {
        if (fullscreenToggle != null)
        {
            fullscreenToggle.onValueChanged.AddListener(SetFullscreen);
            return;
        }

        Transform settings = null;
        Transform[] children = GetComponentsInChildren<Transform>(true);
        for (int i = 0; i < children.Length; i++)
        {
            if (children[i].name == "Settings")
            {
                settings = children[i];
                break;
            }
        }

        if (settings == null)
        {
            return;
        }

        GameObject toggleObject = new GameObject("FullscreenToggle", typeof(RectTransform), typeof(Toggle), typeof(Image));
        toggleObject.transform.SetParent(settings, false);
        fullscreenToggle = toggleObject.GetComponent<Toggle>();
        Image toggleBackground = toggleObject.GetComponent<Image>();
        toggleBackground.color = new Color(0f, 0f, 0f, 0.35f);
        fullscreenToggle.targetGraphic = toggleBackground;

        GameObject checkmarkObject = new GameObject("Checkmark", typeof(RectTransform), typeof(Image));
        checkmarkObject.transform.SetParent(toggleObject.transform, false);
        RectTransform checkmarkRect = checkmarkObject.GetComponent<RectTransform>();
        checkmarkRect.anchorMin = new Vector2(0f, 0.5f);
        checkmarkRect.anchorMax = new Vector2(0f, 0.5f);
        checkmarkRect.pivot = new Vector2(0f, 0.5f);
        checkmarkRect.anchoredPosition = new Vector2(8f, 0f);
        checkmarkRect.sizeDelta = new Vector2(22f, 22f);
        Image checkmark = checkmarkObject.GetComponent<Image>();
        checkmark.color = Color.white;
        fullscreenToggle.graphic = checkmark;

        RectTransform toggleRect = toggleObject.GetComponent<RectTransform>();
        toggleRect.anchorMin = new Vector2(0.5f, 0f);
        toggleRect.anchorMax = new Vector2(0.5f, 0f);
        toggleRect.pivot = new Vector2(0.5f, 0f);
        toggleRect.anchoredPosition = new Vector2(0f, 72f);
        toggleRect.sizeDelta = new Vector2(360f, 44f);

        GameObject labelObject = new GameObject("Label", typeof(RectTransform), typeof(Text));
        labelObject.transform.SetParent(toggleObject.transform, false);
        RectTransform labelRect = labelObject.GetComponent<RectTransform>();
        labelRect.anchorMin = Vector2.zero;
        labelRect.anchorMax = Vector2.one;
        labelRect.offsetMin = new Vector2(36f, 0f);
        labelRect.offsetMax = Vector2.zero;

        Text label = labelObject.GetComponent<Text>();
        label.font = m_khmerFont != null ? m_khmerFont : Resources.GetBuiltinResource<Font>("Arial.ttf");
        label.fontSize = 22;
        label.color = Color.white;
        label.alignment = TextAnchor.MiddleLeft;
        label.text = "Fullscreen";

        fullscreenToggle.onValueChanged.AddListener(SetFullscreen);
    }

    public void SetFullscreen(bool enabled)
    {
        Screen.fullScreen = enabled;
        PlayerPrefs.SetInt("Fullscreen", enabled ? 1 : 0);
    }

    private void GameSetup()
    {
        m_levelNameText.text = m_levels[levelID].name;
        m_levelDescriptionText.text = m_levels[levelID].m_description;

        m_enemyNameText.text = m_enemys[enemyID].name;
        m_enemyDescriptionText.text = m_enemys[enemyID].m_description;

        m_difficultNameText.text = m_difficults[difficultID].name;
        m_difficultDescriptionText.text = m_difficults[difficultID].m_description;

        m_levels[levelID].m_levelShowGameobject.SetActive(true);

        for (int i = 0; i < m_enemys[enemyID].m_enemyShowGameobject.Length; i++)
        {
            m_enemys[enemyID].m_enemyShowGameobject[i].SetActive(true);
        }


        UpdateGameInfo();
    }

    public void ChangeLevel(int value)
    {
        

        if (value > 0)
        {
            if(levelID < m_levels.Length -1)
            {
                m_levels[levelID].m_levelShowGameobject.SetActive(false);
                levelID += 1;
            }
        }

        if (value < 0)
        {
            if (levelID > 0)
            {
                m_levels[levelID].m_levelShowGameobject.SetActive(false);
                levelID -= 1;
            }
        }


        m_levelNameText.text = m_levels[levelID].name;
        m_levelDescriptionText.text = m_levels[levelID].m_description;

        m_levels[levelID].m_levelShowGameobject.SetActive(true);

        UpdateGameInfo();
    }

    public void ChangeEnemy(int value)
    {

        if (value > 0)
        {
            if (enemyID < m_enemys.Length - 1)
            {
                for (int i = 0; i < m_enemys[enemyID].m_enemyShowGameobject.Length; i++)
                {
                    m_enemys[enemyID].m_enemyShowGameobject[i].SetActive(false);
                }
                enemyID += 1;
            }
        }

        if (value < 0)
        {
            if (enemyID > 0)
            {
                for (int i = 0; i < m_enemys[enemyID].m_enemyShowGameobject.Length; i++)
                {
                    m_enemys[enemyID].m_enemyShowGameobject[i].SetActive(false);
                }
                enemyID -= 1;
            }
        }

        for (int i = 0; i < m_enemys[enemyID].m_enemyShowGameobject.Length; i++)
        {
            m_enemys[enemyID].m_enemyShowGameobject[i].SetActive(true);
        }
        m_enemyNameText.text = m_enemys[enemyID].name;
        m_enemyDescriptionText.text = m_enemys[enemyID].m_description;
        UpdateGameInfo();
    }

    public void ChangeDifficulty(int value)
    {


        if (value > 0)
        {
            if (difficultID < m_difficults.Length - 1)
            {
                difficultID += 1;
            }
        }

        if (value < 0)
        {
            if (difficultID > 0)
            {
                difficultID -= 1;
            }
        }

        m_difficultNameText.text = m_difficults[difficultID].name;
        m_difficultDescriptionText.text = m_difficults[difficultID].m_description;

    }

    public void BuyMode(int modeID) /// buy 1 - level, 2 - enemy
    {
        if(modeID == 1)
        {
            if(m_selectedLevelIsLocked)
            {
                if(m_XP >= m_levels[levelID].m_price)
                {
                    m_XP -= m_levels[levelID].m_price;
                    m_levels[levelID].m_lockState = 1;
                    PlayerPrefs.SetInt(m_levels[levelID].m_sceneName + "LockState", 1);
                    PlayerPrefs.SetInt("XP",m_XP);
                    m_setupGameWindow.SetActive(true);
                    UpdateGameInfo();
                }else
                {
                    m_level_NotEnoughXP.SetActive(true);
                }
            }
        }

        if (modeID == 2)
        {
            if (m_selectedEnemyIsLocked)
            {
                if (m_XP >= m_enemys[enemyID].m_price)
                {
                    m_XP -= m_enemys[enemyID].m_price;
                    m_enemys[enemyID].m_lockState = 1;
                    PlayerPrefs.SetInt("Enemy" + m_enemys[enemyID].m_enemyModeId + "LockState", 1);
                    PlayerPrefs.SetInt("XP", m_XP);
                    m_setupGameWindow.SetActive(true);
                    UpdateGameInfo();
                }
                else
                {
                    m_enemy_NotEnoughXP.SetActive(true);
                }
            }
        }
    }

    private void UpdateGameInfo()
    {
        int m_levelLockState = 0;
        int m_enemyLockState = 0;

        m_xp_text.text = m_XP.ToString() + " XP";

        if (PlayerPrefs.HasKey(m_levels[levelID].m_sceneName + "LockState"))
        {
            m_levelLockState = PlayerPrefs.GetInt(m_levels[levelID].m_sceneName + "LockState");
            m_levels[levelID].m_lockState = m_levelLockState;

            switch (m_levelLockState)
            {
                case 0:
                    m_buyLevelIconGO.SetActive(true);
                    m_selectedLevelIsLocked = true;
                    break;
                case 1:
                    m_buyLevelIconGO.SetActive(false);
                    m_selectedLevelIsLocked = false;
                    break;
            }
        }
        else
        {
            if (m_levels[levelID].m_lockByDefault)
            {
                m_selectedLevelIsLocked = true;
                m_buyLevelIconGO.SetActive(true);
            }
            else
            {
                m_buyLevelIconGO.SetActive(false);
                m_selectedLevelIsLocked = false;
            }
        }



        if (PlayerPrefs.HasKey("Enemy" + m_enemys[enemyID].m_enemyModeId + "LockState"))
        {
            m_enemyLockState = PlayerPrefs.GetInt("Enemy" + m_enemys[enemyID].m_enemyModeId + "LockState");
            m_enemys[enemyID].m_lockState = m_enemyLockState;
            switch (m_enemyLockState)
            {
                case 0:
                    m_buyEnemyIconGO.SetActive(true);
                    m_selectedEnemyIsLocked = true;
                    break;
                case 1:
                    m_buyEnemyIconGO.SetActive(false);
                    m_selectedEnemyIsLocked = false;
                    break;
            }
        }
        else
        {
            if (m_enemys[enemyID].m_lockByDefault)
            {
                m_buyEnemyIconGO.SetActive(true);
                m_selectedEnemyIsLocked = true;
            }
            else
            {
                m_selectedEnemyIsLocked = false;
                m_buyEnemyIconGO.SetActive(false);
            }
        }

        if (m_selectedLevelIsLocked || m_selectedEnemyIsLocked)
        {
            m_lockImageGameObject.SetActive(true);
            m_playButton.interactable = false;
        }else
        {
            m_lockImageGameObject.SetActive(false);
            m_playButton.interactable = true;
        }

        m_levelPriceText.text = m_levels[levelID].m_price.ToString();
        m_enemyPriceText.text = m_enemys[enemyID].m_price.ToString();
    }

    public void ApplyConfig()
    {
        PlayerPrefs.SetFloat("Volume", volumeSlider.value);
        PlayerPrefs.SetFloat("Sensitivity", sensitivitySlider.value);
        PlayerPrefs.SetInt("Fullscreen", Screen.fullScreen ? 1 : 0);
        AudioListener.volume = PlayerPrefs.GetFloat("Volume");
        PlayerPrefs.Save();
     
    }

    public void ClearAllSaves()
    {
        m_XP = 0;
        PlayerPrefs.DeleteAll();
        m_xp_text.text = m_XP.ToString() + " XP";
        UpdateGameInfo();
        PlayerPrefs.Save();
    }

    public void StartGame()
    {
        if (!m_selectedEnemyIsLocked && !m_selectedLevelIsLocked)
        {
            PlayerPrefs.SetInt("EnemyMode", m_enemys[enemyID].m_enemyModeId);
            PlayerPrefs.SetString("GameLevel", m_levels[levelID].m_sceneName);
            PlayerPrefs.SetInt("GameDifficulty", m_difficults[difficultID].m_difficultId);
            PlayerPrefs.Save();
            SceneManager.LoadScene(m_loadingSceneName, LoadSceneMode.Single);
        }
    }

    public void StartGameRandomly()
    {
        if (m_enemys == null || m_enemys.Length == 0 ||
            m_levels == null || m_levels.Length == 0 ||
            m_difficults == null || m_difficults.Length == 0)
        {
            Debug.LogError("Cannot start a random game because the level, enemy, or difficulty setup is empty.");
            return;
        }

        EnemySetup enemySetup = m_enemys[Random.Range(0, m_enemys.Length)];
        PlayerPrefs.SetInt("EnemyMode", enemySetup.m_enemyModeId);
        PlayerPrefs.SetString("GameLevel", m_levels[0].m_sceneName);
        PlayerPrefs.SetInt("GameDifficulty", m_difficults[0].m_difficultId);
        PlayerPrefs.Save();
        SceneManager.LoadScene(m_loadingSceneName, LoadSceneMode.Single);
    }

    public void rate()
    {
        Application.OpenURL("https://play.google.com/store/apps/details?id=com.Lordly.Granny3");
    }

    public void QuitGame()
    {      
        Application.Quit();
    }

    

   
}
