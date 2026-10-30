using System.Collections;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Globalization;

public class GameControll : MonoBehaviour
{

    [Header("General parameters")]
    [Tooltip("Player controller script here")]
    public bool m_mobileTouchInput;
    [HideInInspector]
    public PlayerController player;
    [HideInInspector]
    public ItemsSpawner m_spawner;
    [HideInInspector]
    public Inventory inventory;
    private bool pause;
    public string m_loadingSceneName;
    public string m_mainMenuSceneName;
    public List<DifficultyModes> m_difficultyModes = new List<DifficultyModes>();

    [Header("Gameplay Audio")]
    public AudioClip m_gameplayMusic;
    public AudioClip m_babyCryingSound;
    public AudioClip m_dogHowlingSound;
    [Range(0f, 1f)]
    public float m_gameplayMusicVolume = 0.5f;
    [Range(0f, 1f)]
    public float m_ambientSoundsVolume = 1f;
    private AudioSource m_gameplayMusicSource;
    private AudioSource m_ambientSoundsSource;
    private Coroutine m_tipCoroutine;
    private bool m_terminalState;
    private Text m_objectivePromptText;
    private Text m_interactionPromptText;

    [Header("Enemy Settings")]
    public Text m_eyesPillsCountText;
    [HideInInspector]
    public List<Enemy> enemy = new List<Enemy>();
    public Animation m_eyesPillsAnimationUI;
    public string m_addEyesPillsAnimation;
    public int m_eyesCount;
    public float m_maxWatchTime;
    float m_currentWatchTime;
    bool m_isOnEnemyEyesView;
    public List<EnemyModes> m_enemyModes = new List<EnemyModes>();

    [Header("Security Cameras System")]
    public AudioClip m_videcamChangeSound;
    public AudioClip m_videcamOnSound;
    public AudioClip m_videcamOffSound;
    bool m_isOnSecurityCamerasView;
    [HideInInspector]
    public Videcam[] m_videcams;
    public GameObject videcamControllPanel;
    public Text m_videcamRoomNameText;
    int m_currentVidecam;


    [Header("UI Settings")]
    public Animation fadeScreen;
    public string fadeOutAnimName;
    public string fadeInAnimName;
    public GameObject enemyEyesControllPanel;
    public GameObject gameControllPanel;
    public GameObject pausePanel;
    public GameObject gameOverPanel;
    public GameObject gameWinPanel;
    public Slider volumeSlider;
    public Slider sensitivitySlider;
    public Font m_uiFont;

    [Header("Tips Settings")]
    [Tooltip("Tips")]
    public Text m_TipText;
    public float m_TipShowTime;

    [Header("Pictures Settings")]
    public Image m_currentPicture;
    [HideInInspector]
    public Sprite m_nextPicture;
    public Text m_currentPapersCountText;
    public int m_needPicturesCount;
    [HideInInspector]
    public int m_currenPicturestCount;
    public Animation m_paperPicturesAnimationUI;
    public string m_addPaperPictureAnimation;
    PicturePaper.effectType m_currentEffect = PicturePaper.effectType.Null;

    [Header("Start Cutscene Text Printing")]
    [TextArea(10,100)]
    public string m_cutsceneText;
    public GameObject m_cutscenePanel;
    public float m_printSpeed;
    public Text m_cutsceneTextArea;
    public AudioClip[] m_printSound;
    public AudioClip m_printSpaceSound;
    float m_gameHadrnesMultip;
    bool m_cutsceneEnded = false;
    string[] m_cutsceneTextElements;
    int m_textElementID;
    int m_textElementCount;
    bool m_cantPrint = false;


    private void Awake()
    {
        enemy.Clear();
        Enemy[] enm = FindObjectsOfType<Enemy>();
        enemy.AddRange(enm);
        player = FindObjectOfType<PlayerController>();
        m_videcams = FindObjectsOfType<Videcam>();
        m_spawner = GetComponent<ItemsSpawner>();    
        inventory = GetComponent<Inventory>();
        m_cutsceneTextElements = GetTextElements(m_cutsceneText);
        m_textElementCount = m_cutsceneTextElements.Length;
        player.locked = true;
        m_TipText.text = "";
        m_TipText.gameObject.SetActive(false);
        if (enemy.Count > 0)
        {
            for (int i = 0; i < enemy.Count; i++)
            {
                enemy[i].gameObject.SetActive(false);
            }
        }
        enemy.Clear();
        PrepareGame();
        m_cutsceneEnded = true;
        m_cutscenePanel.SetActive(false);
        player.locked = false;
        for (int i = 0; i < enemy.Count; i++)
        {
            enemy[i].gameObject.SetActive(true);
        }

    }

    private void Start()
    {
        SetupPlatformControls();
        SetupGameplayPrompts();
        StartGameplayAudio();

        m_spawner.SpawnPictures(m_needPicturesCount);
        m_spawner.SpawnPills();
        Time.timeScale = 1.0f;
        pause = false;
        m_terminalState = false;
        SetPanelActive(pausePanel, false);
        SetPanelActive(gameOverPanel, false);
        SetPanelActive(gameWinPanel, false);
        SetPanelActive(enemyEyesControllPanel, false);
        SetPanelActive(gameControllPanel, true);
        m_currentPapersCountText.text = m_currenPicturestCount.ToString() + "/" + m_needPicturesCount.ToString();
        UpdateObjectivePrompt();
    }

    private void SetupGameplayPrompts()
    {
        Canvas canvas = GetComponentInChildren<Canvas>(true);
        if (canvas == null)
        {
            Debug.LogError("Cannot create gameplay prompts because the GameManager has no Canvas.", this);
            return;
        }

        m_objectivePromptText = CreatePromptText(canvas.transform, "ObjectivePrompt",
            new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -34f),
            new Vector2(700f, 50f), 22);
        m_interactionPromptText = CreatePromptText(canvas.transform, "InteractionPrompt",
            new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 42f),
            new Vector2(700f, 42f), 20);
        m_interactionPromptText.text = Application.isMobilePlatform
            ? "Tap an object to interact"
            : "Left click to interact";
    }

    private Text CreatePromptText(Transform parent, string objectName, Vector2 anchorMin, Vector2 anchorMax,
        Vector2 anchoredPosition, Vector2 sizeDelta, int fontSize)
    {
        GameObject promptObject = new GameObject(objectName, typeof(RectTransform), typeof(Text), typeof(Shadow));
        promptObject.transform.SetParent(parent, false);
        RectTransform promptRect = promptObject.GetComponent<RectTransform>();
        promptRect.anchorMin = anchorMin;
        promptRect.anchorMax = anchorMax;
        promptRect.pivot = new Vector2(0.5f, 0.5f);
        promptRect.anchoredPosition = anchoredPosition;
        promptRect.sizeDelta = sizeDelta;

        Text prompt = promptObject.GetComponent<Text>();
        prompt.font = m_uiFont != null
            ? m_uiFont
            : m_TipText != null && m_TipText.font != null
                ? m_TipText.font
                : Resources.GetBuiltinResource<Font>("Arial.ttf");
        prompt.fontSize = fontSize;
        prompt.fontStyle = FontStyle.Bold;
        prompt.alignment = TextAnchor.MiddleCenter;
        prompt.horizontalOverflow = HorizontalWrapMode.Wrap;
        prompt.verticalOverflow = VerticalWrapMode.Truncate;
        prompt.color = Color.white;
        prompt.raycastTarget = false;

        Shadow shadow = promptObject.GetComponent<Shadow>();
        shadow.effectColor = new Color(0f, 0f, 0f, 0.85f);
        shadow.effectDistance = new Vector2(2f, -2f);
        return prompt;
    }

    private void UpdateObjectivePrompt()
    {
        if (m_objectivePromptText != null)
        {
            m_objectivePromptText.text = "Objective: Collect pictures " +
                m_currenPicturestCount.ToString() + "/" + m_needPicturesCount.ToString();
        }
    }

    public void SetInteractionPromptVisible(bool visible)
    {
        if (m_interactionPromptText != null)
        {
            m_interactionPromptText.gameObject.SetActive(visible && !m_terminalState && !pause);
        }
    }

    private void SetupPlatformControls()
    {
        bool useTouchControls = Application.isMobilePlatform;
        m_mobileTouchInput = useTouchControls;

        Transform mobileControls = gameControllPanel != null
            ? gameControllPanel.transform.Find("MobileControll")
            : null;

        if (mobileControls != null)
        {
            mobileControls.gameObject.SetActive(useTouchControls);
            Transform lookJoystick = mobileControls.Find("JoystickLook");
            if (lookJoystick != null)
            {
                lookJoystick.gameObject.SetActive(false);
            }
        }

        if (useTouchControls)
        {
            return;
        }

        Canvas canvas = GetComponentInChildren<Canvas>(true);
        if (canvas == null)
        {
            Debug.LogError("Cannot show PC controls because the GameManager has no Canvas.", this);
            return;
        }

        Transform existingGuide = canvas.transform.Find("PCControlsGuide");
        if (existingGuide != null)
        {
            existingGuide.gameObject.SetActive(true);
            return;
        }

        GameObject guide = new GameObject("PCControlsGuide", typeof(RectTransform), typeof(Image));
        guide.transform.SetParent(canvas.transform, false);
        guide.transform.SetAsLastSibling();

        RectTransform guideRect = guide.GetComponent<RectTransform>();
        guideRect.anchorMin = Vector2.zero;
        guideRect.anchorMax = Vector2.zero;
        guideRect.pivot = Vector2.zero;
        guideRect.anchoredPosition = new Vector2(16f, 16f);
        guideRect.sizeDelta = new Vector2(300f, 154f);

        Image background = guide.GetComponent<Image>();
        background.color = new Color(0f, 0f, 0f, 0.72f);
        background.raycastTarget = false;

        GameObject labelObject = new GameObject("Labels", typeof(RectTransform), typeof(Text));
        labelObject.transform.SetParent(guide.transform, false);
        RectTransform labelRect = labelObject.GetComponent<RectTransform>();
        labelRect.anchorMin = Vector2.zero;
        labelRect.anchorMax = Vector2.one;
        labelRect.offsetMin = new Vector2(12f, 8f);
        labelRect.offsetMax = new Vector2(-12f, -8f);

        Text label = labelObject.GetComponent<Text>();
        label.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        label.fontSize = 15;
        label.color = Color.white;
        label.alignment = TextAnchor.UpperLeft;
        label.horizontalOverflow = HorizontalWrapMode.Wrap;
        label.verticalOverflow = VerticalWrapMode.Truncate;
        label.raycastTarget = false;
        label.text = "PC CONTROLS\nWASD / Arrow keys - Move\nMouse - Look around\nLeft Shift - Toggle run\nC - Toggle crouch\nLeft mouse button - Interact\nEsc - Pause";
    }

    private void StartGameplayAudio()
    {
        if (m_gameplayMusic != null)
        {
            m_gameplayMusicSource = gameObject.AddComponent<AudioSource>();
            m_gameplayMusicSource.playOnAwake = false;
            m_gameplayMusicSource.spatialBlend = 0f;
            m_gameplayMusicSource.volume = m_gameplayMusicVolume;
            m_gameplayMusicSource.loop = true;
            m_gameplayMusicSource.clip = m_gameplayMusic;
            m_gameplayMusicSource.Play();
        }
        else
        {
            Debug.LogError("Gameplay music is not assigned.", this);
        }

        m_ambientSoundsSource = gameObject.AddComponent<AudioSource>();
        m_ambientSoundsSource.playOnAwake = false;
        m_ambientSoundsSource.spatialBlend = 0f;
        m_ambientSoundsSource.volume = m_ambientSoundsVolume;

        if (m_babyCryingSound != null)
        {
            StartCoroutine(PlayAmbientSoundEvery(m_babyCryingSound, 15f));
        }
        else
        {
            Debug.LogError("Baby crying ambient sound is not assigned.", this);
        }

        if (m_dogHowlingSound != null)
        {
            StartCoroutine(PlayAmbientSoundEvery(m_dogHowlingSound, 8f));
        }
        else
        {
            Debug.LogError("Dog howling ambient sound is not assigned.", this);
        }
    }

    private IEnumerator PlayAmbientSoundEvery(AudioClip clip, float interval)
    {
        while (true)
        {
            yield return new WaitForSeconds(interval);
            m_ambientSoundsSource.PlayOneShot(clip);
        }
    }

    private void PrepareGame()
    {

        if (PlayerPrefs.HasKey("Volume"))
        {
            AudioListener.volume = PlayerPrefs.GetFloat("Volume");
            volumeSlider.value = PlayerPrefs.GetFloat("Volume");
        }
        else
        {
            AudioListener.volume = 1f;
            volumeSlider.value = 1f;
        }

        if (PlayerPrefs.HasKey("Sensitivity"))
        {
            player.mouseSensetivity = PlayerPrefs.GetFloat("Sensitivity");
            sensitivitySlider.value = PlayerPrefs.GetFloat("Sensitivity");
        }
        else
        {
            sensitivitySlider.value = sensitivitySlider.maxValue / 2f;
            player.mouseSensetivity = sensitivitySlider.value;
        }

        if (PlayerPrefs.HasKey("EnemyMode"))
        {
            int EM = PlayerPrefs.GetInt("EnemyMode");

            int i = GetEnemyModeById(EM);

            if(i != -1)
            {
                enemy.AddRange(m_enemyModes[i].m_enemys);
           
            }else
            {
                Debug.Log("Wrond enemy mode id, cant find mode in Enemy Modes list");
            }
            
        }else
        {
            int EM = 0;

            int i = GetEnemyModeById(EM);

            if (i != -1)
            {
                enemy.AddRange(m_enemyModes[i].m_enemys);

            }
            else
            {
                Debug.Log("Wrond enemy mode id, cant find mode in Enemy Modes list");
            }
        }


        if (PlayerPrefs.HasKey("GameDifficulty"))
        {
            int GD = PlayerPrefs.GetInt("GameDifficulty");

            int diffId = GetDifficultyModeById(GD);

            if (diffId != -1)
            {
                for (int i = 0; i < enemy.Count; i++)
                {
                    enemy[i].seeDistance = m_difficultyModes[diffId].m_enemySeeDistance;
                    enemy[i].enemyFOV = m_difficultyModes[diffId].m_enemyFOV;
                    enemy[i].lostTime = m_difficultyModes[diffId].m_enemyLostTime;
                    enemy[i].walkSpeed = m_difficultyModes[diffId].m_enemyMoveSpeed;
                    m_gameHadrnesMultip = m_difficultyModes[diffId].m_XP_Multiplayer;
                }
            }else
            {
                Debug.Log("Wrond difficulty mode id, cant find mode in difficulty Modes list");
            }
        }else
        {
            int GD = 0;

            int diffId = GetDifficultyModeById(GD);

            if (diffId != -1)
            {
                for (int i = 0; i < enemy.Count; i++)
                {
                    enemy[i].seeDistance = m_difficultyModes[diffId].m_enemySeeDistance;
                    enemy[i].enemyFOV = m_difficultyModes[diffId].m_enemyFOV;
                    enemy[i].lostTime = m_difficultyModes[diffId].m_enemyLostTime;
                    enemy[i].walkSpeed = m_difficultyModes[diffId].m_enemyMoveSpeed;
                    m_gameHadrnesMultip = m_difficultyModes[diffId].m_XP_Multiplayer;
                }
            }
         
        }
    }

    private void Update()
    {
        ControllGame();
        UpdateStats();
        Cutscene();
    }

    private int GetEnemyModeById(int id)
    {
        for (int i = 0; i < m_enemyModes.Count; i++)
        {
            if(m_enemyModes[i].m_enemyModeId == id)
            {
                return i;
            }
        }

        return -1;
    }

    private int GetDifficultyModeById(int id)
    {
        for (int i = 0; i < m_difficultyModes.Count; i++)
        {
            if (m_difficultyModes[i].m_DifficultyModeId == id)
            {
                return i;
            }
        }

        return -1;
    }

    private void Cutscene()
    {
        if(!m_cutsceneEnded && !m_cantPrint)
        {
            string textElement = m_cutsceneTextElements[m_textElementID];
            float printDelay = m_printSpeed * textElement.Length;
            m_cutsceneTextArea.text += textElement;
           
                if (!char.IsWhiteSpace(textElement[0]))
                {
                    int r = Random.Range(0, m_printSound.Length);
                    player.AS.PlayOneShot(m_printSound[r]);
                }else
                {
                    player.AS.PlayOneShot(m_printSpaceSound);
                }
            
            m_textElementID += 1;
            m_cantPrint = true;
            if(m_textElementID >= m_textElementCount)
            {
                m_cutsceneEnded = true;
               
            }else
            {
                if (m_mobileTouchInput)
                {


                    for (var i = 0; i < Input.touchCount; ++i)
                    {
                        if (Input.GetTouch(i).phase == TouchPhase.Began)
                        {
                            StartCoroutine(WaitCharPrintTime(0.0f));

                        }
                    }

                    StartCoroutine(WaitCharPrintTime(printDelay));


                }
                else
                {

                    if (CrossPlatformInputManager.GetButton("Interact"))
                    {
                        StartCoroutine(WaitCharPrintTime(0.0f));
                    }
                    else
                    {
                        StartCoroutine(WaitCharPrintTime(printDelay));
                    }

                }


            }
        }
    }

    private string[] GetTextElements(string text)
    {
        List<string> textElements = new List<string>();
        TextElementEnumerator enumerator = StringInfo.GetTextElementEnumerator(text);
        bool mergeNextTextElement = false;

        while (enumerator.MoveNext())
        {
            string textElement = (string)enumerator.Current;
            int lastElementIndex = textElements.Count - 1;

            if (mergeNextTextElement && lastElementIndex >= 0)
            {
                textElements[lastElementIndex] += textElement;
            }
            else
            {
                textElements.Add(textElement);
            }

            mergeNextTextElement = textElement.EndsWith("\u17D2", System.StringComparison.Ordinal);
        }

        return textElements.ToArray();
    }

    public void SkipCutscene()
    {
        StopAllCoroutines();
        m_cutsceneEnded = true;
        m_cutsceneTextArea.text = m_cutsceneText;
        StartCoroutine(WaitCutscene());
    }

    public void SetEffect(PicturePaper.effectType effectType)
    {
        if (m_currentEffect != PicturePaper.effectType.Null)
        {
            DisableCurrentEffect(m_currentEffect);
        }

        m_currentEffect = effectType;

            switch (effectType)
            {
              case PicturePaper.effectType.Null:
                break;
              case PicturePaper.effectType.AddEyePills:
                AddEyePills(1);
                    break;
                case PicturePaper.effectType.EnemyAlwaysSeePlayer:
             
                for (int i = 0; i < enemy.Count; i++)
                {
                    enemy[i].alwaysSeePlayer = true;
                }
                break;
                case PicturePaper.effectType.EnemyCall:

                for (int i = 0; i < enemy.Count; i++)
                {
                    enemy[i].CallEnemy(player.transform.position);
                }
           
                    break;
                case PicturePaper.effectType.UnlimitedStamina:
                player.m_unlimitedStamina = true;
                    break;
            case PicturePaper.effectType.Random:

                int r = Random.Range(0,3); /// random from 0 to number of effects
                switch (r)
                {
                    case 0:
                        AddEyePills(1);
                        m_currentEffect = PicturePaper.effectType.AddEyePills;
                        break;
                    case 1:
                        for (int i = 0; i < enemy.Count; i++)
                        {
                            enemy[i].alwaysSeePlayer = true;
                        }
                        m_currentEffect = PicturePaper.effectType.EnemyAlwaysSeePlayer;
                        break;
                    case 2:
                        for (int i = 0; i < enemy.Count; i++)
                        {
                            enemy[i].CallEnemy(player.transform.position);
                        }
                        m_currentEffect = PicturePaper.effectType.EnemyCall;
                        break;
                    case 3:
                        player.m_unlimitedStamina = true;
                        m_currentEffect = PicturePaper.effectType.UnlimitedStamina;
                        break;

                }
                break;

            case PicturePaper.effectType.RandomBad:

                int rb = Random.Range(0, 1); /// random from 0 to number of effects
                switch (rb)
                {
                   
                    case 0:
                        for (int i = 0; i < enemy.Count; i++)
                        {
                            enemy[i].alwaysSeePlayer = true;
                        }
                        m_currentEffect = PicturePaper.effectType.EnemyAlwaysSeePlayer;
                        break;
                    case 1:
                        for (int i = 0; i < enemy.Count; i++)
                        {
                            enemy[i].CallEnemy(player.transform.position);
                        }
                        m_currentEffect = PicturePaper.effectType.EnemyCall;
                        break;
         

                }
                break;

            }   
        
    }

    private void DisableCurrentEffect(PicturePaper.effectType effectType)
    {
        switch (effectType)
        {
            case PicturePaper.effectType.Null:
                break;

            case PicturePaper.effectType.AddEyePills:             
                break;
            case PicturePaper.effectType.EnemyAlwaysSeePlayer:
                for (int i = 0; i < enemy.Count; i++)
                {
                    enemy[i].alwaysSeePlayer = false;
                    enemy[i].ResetPatrol();
                }
                break;
            case PicturePaper.effectType.EnemyCall:
                
                break;
            case PicturePaper.effectType.UnlimitedStamina:
                player.m_unlimitedStamina = false;
                break;
            case PicturePaper.effectType.Random:
                break;
            case PicturePaper.effectType.RandomBad:
                break;

        }
    }

    private void UpdateStats()
    {
        m_eyesPillsCountText.text = m_eyesCount.ToString();
        player.m_enemySeePlayer = SomeEnemySeePlayer();
    }

    private bool SomeEnemySeePlayer()
    {
        for (int i = 0; i < enemy.Count; i++)
        {
            if (enemy[i].seePlayer)
            {
                return true;
            }
        }

        return false;
    }

    public void AddPaperPicture()
    {
        m_currenPicturestCount += 1;
        m_paperPicturesAnimationUI.Play(m_addPaperPictureAnimation);
        m_currentPapersCountText.text = m_currenPicturestCount.ToString() + "/" + m_needPicturesCount.ToString();
        UpdateObjectivePrompt();
    }

    public void AddEyePills(int cnt)
    {
        m_eyesCount += cnt;
        m_eyesPillsAnimationUI.Play(m_addEyesPillsAnimation);
    }

    public void ChangePaperPictureIcon()
    {
        m_currentPicture.sprite = m_nextPicture;
    }

    private void ControllGame()
    {
        if(CrossPlatformInputManager.GetButtonDown("Pause"))
        {
            PauseGame();
        }


        if (m_isOnEnemyEyesView)
        {
           if(m_currentWatchTime >= m_maxWatchTime)
            {
                SetEnemyEyes();
            }else
            {
                m_currentWatchTime += Time.deltaTime;
            }
        }
    }

    public void SetSecurityCamerasChange(int id)
    {
        if(m_videcams.Length > 1)
        {
            if (id > 0)
            {
                if (m_currentVidecam + 1 < m_videcams.Length)
                {
                    m_videcams[m_currentVidecam].m_camera.SetActive(false);
                    m_currentVidecam += 1;
                    m_videcams[m_currentVidecam].m_camera.SetActive(true);
                    m_videcamRoomNameText.text = "Camera " + m_videcams[m_currentVidecam].m_cameraRoonName;
                }
                else
                {
                    m_videcams[m_currentVidecam].m_camera.SetActive(false);
                    m_currentVidecam = 0;
                    m_videcams[m_currentVidecam].m_camera.SetActive(true);
                    m_videcamRoomNameText.text = "Camera " + m_videcams[m_currentVidecam].m_cameraRoonName;
                }
            }
            else

            {
                if (m_currentVidecam > 0)
                {
                    m_videcams[m_currentVidecam].m_camera.SetActive(false);
                    m_currentVidecam -= 1;
                    m_videcams[m_currentVidecam].m_camera.SetActive(true);
                    m_videcamRoomNameText.text = "Camera " + m_videcams[m_currentVidecam].m_cameraRoonName;
                }
                else
                {
                    m_videcams[m_currentVidecam].m_camera.SetActive(false);
                    m_currentVidecam = m_videcams.Length - 1;
                    m_videcams[m_currentVidecam].m_camera.SetActive(true);
                    m_videcamRoomNameText.text = "Camera " + m_videcams[m_currentVidecam].m_cameraRoonName;
                }
            }
        }

        AudioSource.PlayClipAtPoint(m_videcamChangeSound,m_videcams[m_currentVidecam].transform.position);
    }

    public void SetSecurityCameras()
    {
        
            if (!m_isOnSecurityCamerasView)
            {
               if (m_videcams.Length > 0 && !player.locked)
               {
                gameControllPanel.SetActive(false);
                videcamControllPanel.SetActive(true);
                m_currentVidecam = 0;
                m_isOnSecurityCamerasView = true;
                player.locked = true;
                player.cameraTransform.gameObject.SetActive(false);
                m_videcams[m_currentVidecam].m_camera.SetActive(true);
                AudioSource.PlayClipAtPoint(m_videcamOnSound, m_videcams[m_currentVidecam].transform.position);
                m_videcamRoomNameText.text = "Camera " + m_videcams[m_currentVidecam].m_cameraRoonName;
               }
            }
            else
            {
                AudioSource.PlayClipAtPoint(m_videcamOffSound, player.transform.position);
                gameControllPanel.SetActive(true);
                videcamControllPanel.SetActive(false);
                m_isOnSecurityCamerasView = false;
                player.locked = false;
                player.cameraTransform.gameObject.SetActive(true);
                m_videcams[m_currentVidecam].m_camera.SetActive(false);
            }
        
    }

    public void SetEnemyEyes()
    {
        if(!m_isOnEnemyEyesView && !player.locked)
        {
            if (m_eyesCount > 0)
            {
                m_eyesCount -= 1;
                gameControllPanel.SetActive(false);
                enemyEyesControllPanel.SetActive(true);
                m_currentWatchTime = 0;
                m_isOnEnemyEyesView = true;
                player.locked = true;
                player.cameraTransform.gameObject.SetActive(false);
                int eID = GetNearestEnemy();
                enemy[eID].m_enemyHeadCamera.SetActive(true);
            }
        }else
        {
            gameControllPanel.SetActive(true);
            enemyEyesControllPanel.SetActive(false);
            m_currentWatchTime = 0;
            m_isOnEnemyEyesView = false;
            player.locked = false;
            player.cameraTransform.gameObject.SetActive(true);
            for (int i = 0; i < enemy.Count; i++)
            {
                enemy[i].m_enemyHeadCamera.SetActive(false);
            }
           
        }
    }

    int GetNearestEnemy()
    {
        float dist = 90000f;
        int enemyId = 0;

        for (int i = 0; i < enemy.Count; i++)
        {
            float dist2 = Vector3.Distance(enemy[i].transform.position ,player.transform.position);
            if(dist2 < dist)
            {
                dist = dist2;
                enemyId = i;
            }
        }

        return enemyId;
    }

    public void PauseGame()
    {
        if (m_terminalState || player == null || player.lockedByDying)
        {
            return;
        }

        pause = !pause;
        SetPanelActive(pausePanel, pause);
        Time.timeScale = pause ? 0.0f : 1.0f;
        player.locked = pause;
        SetInteractionPromptVisible(!pause);
    }

    public void GameOver()
    {
        if (m_terminalState)
        {
            return;
        }

        m_terminalState = true;
        pause = false;
        Time.timeScale = 0.0f;
        player.locked = true;
        player.lockedByDying = true;
        SetPanelActive(gameControllPanel, false);
        SetPanelActive(pausePanel, false);
        SetPanelActive(gameOverPanel, true);
        if (m_objectivePromptText != null)
        {
            m_objectivePromptText.gameObject.SetActive(false);
        }
        SetInteractionPromptVisible(false);
    }

    public void GameWin()
    {
        if (m_terminalState)
        {
            return;
        }

        m_terminalState = true;
        pause = false;
        Time.timeScale = 0.0f;
        SetPanelActive(gameControllPanel, false);
        SetPanelActive(pausePanel, false);
        SetPanelActive(gameWinPanel, true);
        player.locked = true;
        player.lockedByDying = true;
        if (m_objectivePromptText != null)
        {
            m_objectivePromptText.gameObject.SetActive(false);
        }
        SetInteractionPromptVisible(false);
        for (int i = 0; i < enemy.Count; i++)
        {
            enemy[i].gameObject.SetActive(false);
        }
     
    }

    private void SetPanelActive(GameObject panel, bool active)
    {
        if (panel != null)
        {
            panel.SetActive(active);
        }
    }

    public void ShowTip(int itemId, int type)
    {
        if (m_tipCoroutine != null)
        {
            StopCoroutine(m_tipCoroutine);
        }

        if (type == 0)
        {
            m_TipText.gameObject.SetActive(true);
            m_TipText.text = "You need a " + inventory.m_itemsDatabase.Items[itemId].name;
            m_tipCoroutine = StartCoroutine(WaitForTip());
        }

        if (type == 1)
        {
            m_TipText.gameObject.SetActive(true);
            m_TipText.text = "Door is jamned!";
            m_tipCoroutine = StartCoroutine(WaitForTip());
        }

        if (type == 3)
        {
            m_TipText.gameObject.SetActive(true);
            m_TipText.text = "I need to find all pictures before leave this place!";
            m_tipCoroutine = StartCoroutine(WaitForTip());
        }

        if (type == 4)
        {
            m_TipText.gameObject.SetActive(true);
            m_TipText.text = "Used a " + inventory.m_itemsDatabase.Items[itemId].name;
            m_tipCoroutine = StartCoroutine(WaitForTip());
        }

        if (type == 5)
        {
            m_TipText.gameObject.SetActive(true);
            m_TipText.text = "No Power!";
            m_tipCoroutine = StartCoroutine(WaitForTip());
        }

        if (type == 6)
        {
            m_TipText.gameObject.SetActive(true);
            m_TipText.text = "You take a " + inventory.m_itemsDatabase.Items[itemId].name;
            m_tipCoroutine = StartCoroutine(WaitForTip());
        }
    }

    public void ResetPlayerStates()
    {
        if (m_isOnEnemyEyesView)     
        {
            gameControllPanel.SetActive(true);
            enemyEyesControllPanel.SetActive(false);
            m_currentWatchTime = 0;
            m_isOnEnemyEyesView = false;
            player.locked = false;
            player.cameraTransform.gameObject.SetActive(true);
            for (int i = 0; i < enemy.Count; i++)
            {
                enemy[i].m_enemyHeadCamera.SetActive(false);
            }

        }


        if(m_isOnSecurityCamerasView)
        {
            AudioSource.PlayClipAtPoint(m_videcamOffSound, player.transform.position);
            gameControllPanel.SetActive(true);
            videcamControllPanel.SetActive(false);
            m_isOnSecurityCamerasView = false;
            player.locked = false;
            player.cameraTransform.gameObject.SetActive(true);
            m_videcams[m_currentVidecam].m_camera.SetActive(false);
        }









    }

    public void MainMenuExit(int type)
    {
        if (type == 0)
        {
            PlayerPrefs.SetString("GameLevel", m_mainMenuSceneName);
            SceneManager.LoadScene(m_loadingSceneName);
        }

        if(type == 1) //// Game Over
        {
            PlayerPrefs.SetInt("ReturnFromGame", 1);
            PlayerPrefs.SetInt("CollectedPictures", m_currenPicturestCount);
            float plusXP = m_currenPicturestCount * 100;
            plusXP = plusXP * m_gameHadrnesMultip;
            int xp = Mathf.RoundToInt(plusXP);
            PlayerPrefs.SetInt("PLUS_XP", xp);
            PlayerPrefs.SetString("GameLevel", m_mainMenuSceneName);
            SceneManager.LoadScene(m_loadingSceneName);
        }


        if (type == 2) //// Game Win
        {
            PlayerPrefs.SetInt("ReturnFromGame", 1);
            PlayerPrefs.SetInt("CollectedPictures", m_currenPicturestCount);
            float plusXP = m_currenPicturestCount * 250;
            plusXP = plusXP * m_gameHadrnesMultip;
            int xp = Mathf.RoundToInt(plusXP);
            PlayerPrefs.SetInt("PLUS_XP", xp);
            PlayerPrefs.SetString("GameLevel", m_mainMenuSceneName);
            SceneManager.LoadScene(m_loadingSceneName);
        }
    }

    public void ScreenFade(int state)
    {
        if(state == 0)
        {
            fadeScreen.Play(fadeOutAnimName);
        }

        if(state == 1)
        {
            fadeScreen.Play(fadeInAnimName);
        }

        if(state == 2)
        {
           
            StartCoroutine(WaitKillAnim(1.2f));
        }

    }

    IEnumerator WaitForTip()
    {

        yield return new WaitForSeconds(m_TipShowTime);
        m_TipText.text = "";
        m_TipText.gameObject.SetActive(false);
        m_tipCoroutine = null;

    }

    private IEnumerator WaitKillAnim(float killTime)
    {
        yield return new WaitForSeconds(killTime);
        fadeScreen.Play(fadeInAnimName);
        StartCoroutine(WaitFadeAnim(fadeInAnimName));
    }

    private IEnumerator WaitCharPrintTime(float speed)
    {
        yield return new WaitForSeconds(speed);
        m_cantPrint = false;

    }

    private IEnumerator WaitCutscene()
    {
        yield return new WaitForSeconds(1f);
        fadeScreen.Play(fadeOutAnimName);
        m_cutscenePanel.SetActive(false);
        player.locked = false;
        for (int i = 0; i < enemy.Count; i++)
        {
            enemy[i].gameObject.SetActive(true);
        }


    }

    private IEnumerator WaitFadeAnim(string name)
    {
        yield return new WaitForSeconds(fadeScreen[name].length);
        GameOver();
        
    }

    public void ConfigureApply()
    {
        PlayerPrefs.SetFloat("Volume", volumeSlider.value);
        PlayerPrefs.SetFloat("Sensitivity", sensitivitySlider.value);
        AudioListener.volume = PlayerPrefs.GetFloat("Volume");
        player.mouseSensetivity = PlayerPrefs.GetFloat("Sensitivity");
        PlayerPrefs.Save();

    }
}
