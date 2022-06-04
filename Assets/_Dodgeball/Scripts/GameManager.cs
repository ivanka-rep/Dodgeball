using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Configs.GameConfig;
using DataSakura.JoyStick.Scripts;
using Feautures;
using Items;
using TMPro;
using UnityEditor.UIElements;
using Utilities;

public class GameManager : MonoBehaviour
{
    public static GameManager s_instance;

    [Header("Params")] 
    [SerializeField] private GameConfig m_gameConfig;
    [SerializeField] private TutorialManager m_tutorialManager;
    
    [Header("Game Objects")]
    [SerializeField] private Camera m_camera;
    [SerializeField] private GameObject m_playerBallPrefab;
    [SerializeField] private GameObject m_enemyBallPrefab;
    [SerializeField] private GameObject m_hitParticleEffectPrefab;
    [SerializeField] private ObjectPool m_spheresPool;
    [SerializeField] private List<GameObject> m_players;
    [SerializeField] private List<GameObject> m_limitWalls;
    [SerializeField] private List<Transform> m_playerPositions;
    [SerializeField] private List<Transform> m_enemy3Positions;
    
    
    [Header("GUI")]
    [SerializeField] private ParticleSystem m_confettiEffect;
    [SerializeField] private Text m_PlayerTeamText;
    [SerializeField] private Text m_PlayerScoreText;
    [SerializeField] private Text m_EnemyScoreText;
    [SerializeField] private Text m_timerText;
    [SerializeField] private Text m_roundText;
    [SerializeField] private Text m_resultText;
    [SerializeField] private Text m_playerResultText;
    [SerializeField] private Text m_enemyResultText;
    [SerializeField] private Text m_difficultyText;
    [SerializeField] private Text m_delayText;
    [SerializeField] private Text m_wonCashText;
    [SerializeField] private GameObject m_topPanel;
    [SerializeField] private GameObject m_gameOverPanel;
    [SerializeField] private GameObject m_menuPanel;
    [SerializeField] private GameObject m_pauseMenu;
    [SerializeField] private GameObject m_controls;
    [SerializeField] private GameObject m_panelsMask;
    [SerializeField] private Transform m_settingsPanel;
    [SerializeField] private Transform m_shopPanel;
    [SerializeField] private Button m_playButton;
    [SerializeField] private Button m_settingsButton;
    [SerializeField] private Button m_shopButton;

    public DifficultyParams DifficultyParams => m_difficultyParams;
    public List<GameObject> Players => m_players;
    public List<GameObject> LimitWalls => m_limitWalls;
    public GameObject PlayerBallPrefab => m_playerBallPrefab;
    public GameObject EnemyBallPrefab => m_enemyBallPrefab;
    public GameObject HitParticleEffectPrefab => m_hitParticleEffectPrefab;
    public GameObject PanelsMask => m_panelsMask;
    public ParticleSystem ConfettiEffect => m_confettiEffect;
    public ObjectPool SpheresPool => m_spheresPool;

    [HideInInspector] public bool gameStarted = false;
    
    //private GameObject m_currentPlayer;
    private int m_playerScore = 0;
    private int m_enemyScore = 0;
    private float m_timeLeft;
    private DifficultyParams m_difficultyParams;
    private GameData m_gameData;

    private SkinnedMeshRenderer playerSkinMeshRenderer;
    private MeshRenderer[] playerOtherMeshRenderers;
    
    private void Awake()
   {
       s_instance = this;
       m_gameData = GameData.s_instance;
       m_difficultyParams = m_gameConfig.DifficultyParams[m_gameData.CurrentDiffNum];
       playerSkinMeshRenderer = m_players[0].GetComponentInChildren<SkinnedMeshRenderer>();
       playerOtherMeshRenderers = m_players[0].GetComponentsInChildren<MeshRenderer>();
       
       m_playButton.onClick.AddListener(StartGame);
       m_settingsButton.onClick.AddListener(() =>
       {
           m_panelsMask.SetActive(true);
           m_settingsPanel.gameObject.SetActive(true);
           TweenAnimation.PlayAnimation(m_settingsPanel, AnimationType.OpeningTransition, 0.5f);
       });
       m_shopButton.onClick.AddListener(() =>
       {
           m_panelsMask.SetActive(true);
           m_shopPanel.gameObject.SetActive(true);
           TweenAnimation.PlayAnimation(m_shopPanel, AnimationType.OpeningTransition, 0.5f);
       });
       
       DisableControls();
       m_tutorialManager.Init();
       if (m_tutorialManager.TutorialActive || m_gameData.IsPractice)
       {
           m_players.ForEach(p  => { if (LayerMask.LayerToName(p.layer) != "Player_1") p.SetActive(false); });
           m_tutorialManager.StartTutorial(); return;
       }
       
       ChangeLevelStructure();
       ChangePlayerSkin();
       
       // if is start or we are started by PlayAgain button
       if (m_gameData.RoundNumber == -1 || m_gameData.RoundNumber == m_difficultyParams.countOfRounds - 1)
       {
           m_gameData.RoundNumber = -1;
           m_gameData.TotalScore = new int[2];
       }
       if (m_gameData.RoundNumber < m_difficultyParams.countOfRounds && m_gameData.NextRoundAvailable)
       {
           m_gameData.RoundNumber++; // set number of current round
           StartGame();
       }
       else if (!m_gameData.NextRoundAvailable) // Default menu loading.
       {
           m_gameData.RoundNumber = 0;
           
           // GUI Changes
           m_difficultyText.text = m_difficultyParams.difficulty.ToString();
           m_difficultyText.color = Color.green;
           if (m_difficultyParams.difficulty == m_gameConfig.DifficultyParams[1].difficulty) m_difficultyText.color = Color.cyan;
           else if (m_difficultyParams.difficulty == m_gameConfig.DifficultyParams[2].difficulty) m_difficultyText.color = Color.red;
           m_menuPanel.SetActive(true);
       }
   }

    private void ChangeLevelStructure()
    {
        // Difficulty EASY
        if (m_difficultyParams.difficulty == DifficultyType.Easy)
        {
            m_limitWalls[0].SetActive(true);
            
            m_players[1].SetActive(false);
            m_players[2].SetActive(false);

            m_players[0].transform.position = m_playerPositions[0].position; // set 1 x 1 user position
            m_players[3].transform.position = m_enemy3Positions[0].position; // set 1 x 1 enemy3 position
            RotatePlayer(m_players[3], 40f); 
        }
        // Difficulty HARD & NORMAL
        if (m_difficultyParams.difficulty == DifficultyType.Easy) return;
        
        RotatePlayer(m_players[2], 90f); 
        RotatePlayer(m_players[1], 180f);
        
        m_players[3].transform.position = m_enemy3Positions[1].position;  // set 1 x 3 or 1 x 2 enemy3 position // NEED TO TEST
        m_players[0].transform.position = m_playerPositions[1].position;  // set 1 x 3 or 1 x 2 user position
        
        if (m_difficultyParams.difficulty == DifficultyType.Normal)
        {
            m_limitWalls[1].SetActive(true);
            
            m_players[1].SetActive(false);
            m_players[0].transform.position = m_playerPositions[0].position; // set 1 x 1 user position
        }
        else if (m_difficultyParams.difficulty == DifficultyType.Hard)
        {
            RotatePlayer(m_players[0], 45f);
            m_limitWalls[2].SetActive(true);
        }
    }

    public void DisableControls()
    {
        foreach (GameObject player in m_players) 
        { 
            if (!player.activeSelf) continue;
            APRController playerAPR = player.GetComponent<APRController>();
            playerAPR.useControls = false;
            playerAPR.isBot = false;
            m_controls.SetActive(false);
        } 
    }

    void RotatePlayer(GameObject player, float reqRotation)
    {
        APRController playerAPR = player.GetComponent<APRController>();
        Quaternion rot = playerAPR.Root.GetComponent<ConfigurableJoint>().targetRotation;

        playerAPR.Root.GetComponent<ConfigurableJoint>().targetRotation = 
            Quaternion.Euler(rot.eulerAngles.x, rot.eulerAngles.y + reqRotation, rot.eulerAngles.z);
    }

    private void Update()
    {
        #if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.A)) AddScore(true);
        if (Input.GetKeyDown(KeyCode.M)) m_gameData.UserData.Cash += 1000;
        #endif
        
        if(Input.GetKey(KeyCode.Escape)) m_pauseMenu.SetActive(true);
    }

    public void StartGame() 
   {
       Debug.Log("Round count: " + m_gameData.RoundNumber);
       m_menuPanel.SetActive(false);
       
       if (m_gameData.RoundNumber > 0f || m_gameData.IsRestart)
       {
           m_gameData.IsRestart = false;
           var camPos = m_camera.transform.position;
           m_camera.transform.position = new Vector3(m_players[0].GetComponent<APRController>().Root.transform.position.x, camPos.y, camPos.z);
       }
       StartCoroutine(GamePrepare());
   }

    private IEnumerator GamePrepare()
    {
        // 3..2..1..GO!!!
        if (!m_tutorialManager.TutorialActive)
        {
            float t = m_gameConfig.startDelay;
            while (t > 0)
            {
                m_delayText.text = t.ToString(CultureInfo.InvariantCulture);
                yield return new WaitForSeconds(1f);
                t--;
            }

            m_delayText.text = "GO!";
            m_delayText.GetComponent<Animator>().enabled = true; // start disappearing anim
        }

        foreach (GameObject player in m_players)
        {
            if (player.activeSelf == false) continue;
            APRController playerApr = player.GetComponent<APRController>();
            ThrowingController playerThrowingCtrl = player.GetComponent<ThrowingController>();
           
            if (player.layer == LayerMask.NameToLayer("Player_1"))
            {
                playerApr.useControls = true;
                playerApr.isBot = false;
                m_controls.SetActive(true);
                playerApr.Joystick.gameObject.SetActive(true);
            }
            else
            {
                playerApr.isBot = true;
                playerApr.useControls = false;
            }
           
            // return collisions if in last frame ball was thrown
            Physics.IgnoreLayerCollision(LayerMask.NameToLayer(playerApr.thisPlayerLayer),
                LayerMask.NameToLayer(playerThrowingCtrl.ThisPlayerBallLayer), false); 
            Physics.IgnoreLayerCollision(LayerMask.NameToLayer("LimitWall"),
                LayerMask.NameToLayer(playerThrowingCtrl.ThisPlayerBallLayer), false);
           
            playerThrowingCtrl.SetGameVars();
            playerThrowingCtrl.CreateNewBall();
        }

        // GUI changes
        if (!m_tutorialManager.TutorialActive)
        {
            m_EnemyScoreText.text = "0";
            m_PlayerScoreText.text = "0";
            m_roundText.text = "Round " + (m_gameData.RoundNumber + 1) + "/" + m_difficultyParams.countOfRounds;
            StartCoroutine(StartTimer());
            m_topPanel.SetActive(true);
        }
        
        m_camera.GetComponent<CameraController>().enabled = true;
        gameStarted = true;
    }

    private IEnumerator StartTimer()
    {
        m_timeLeft = m_difficultyParams.roundTime;
        
        while (m_timeLeft > -1f)
        {
            m_timerText.text = (m_timeLeft + " S" ).ToString(CultureInfo.InvariantCulture);
            yield return new WaitForSeconds(1f);
            m_timeLeft--;
        }

        NextRoundOrEnd(); // Go to next round, or end of game
    }

    private void NextRoundOrEnd()
    {
        int currentScoreDelta = m_playerScore - m_enemyScore;
        if (currentScoreDelta == 0) // Draw situation
        { for (int i = 0; i < 2; i++) { m_gameData.TotalScore[i]++; } }
        else
        {
            m_gameData.TotalScore[0] += currentScoreDelta > 0 ? 1 : 0; // player
            m_gameData.TotalScore[1] += currentScoreDelta > 0 ? 0 : 1; // enemy
        }

        if (m_gameData.RoundNumber == m_difficultyParams.countOfRounds - 1)
        {
            DisableControls();

            int totalScoreDelta = m_gameData.TotalScore[0] - m_gameData.TotalScore[1];
            
            if (totalScoreDelta == 0) { m_resultText.text = "DRAW";}
            else if (totalScoreDelta > 0)
            {
                int wonCash = m_difficultyParams.difficulty switch
                {
                    DifficultyType.Hard => 500,
                    DifficultyType.Normal => 200,
                    _ => 100 // Easy
                };

                m_resultText.text = "YOU WON";
                m_gameData.UserData.Cash += wonCash;
                m_wonCashText.text = "+" + wonCash + "$";
                m_wonCashText.gameObject.SetActive(true);
            }
            else { m_resultText.text = "YOU LOSE"; }
            
            m_playerResultText.text = m_gameData.TotalScore[0].ToString();
            m_enemyResultText.text = m_gameData.TotalScore[1].ToString();
            
            m_topPanel.SetActive(false);
            m_gameOverPanel.SetActive(true);
            if (totalScoreDelta > 0) m_confettiEffect.Play();
        }
        else
        {
            PlayAgain();
        }
    }

    public void AddScore(bool isPlayer)
    {
        if (isPlayer)
        { m_playerScore++; m_PlayerScoreText.text = m_playerScore.ToString();}
        else
        { m_enemyScore++; m_EnemyScoreText.text = m_enemyScore.ToString();}
    }

    public void GoToMenu() // method is used by Button
   {
       m_gameData.RoundNumber = -1;
       m_gameData.NextRoundAvailable = false;
       SetDefaultJoyPositions();
       SceneManager.LoadScene(SceneManager.GetActiveScene().name);
   }

   public void PlayAgain() // method is used by Button
   {
       m_gameData.NextRoundAvailable = true;
       m_gameData.IsRestart = true;
       SetDefaultJoyPositions();
       SceneManager.LoadScene(SceneManager.GetActiveScene().name);
   }

   public void Restart() { m_gameData.RoundNumber = -1;  PlayAgain(); }

   public void StartPractice() { m_gameData.IsPractice = true;
       m_gameData.CurrentDiffNum = 0;  Restart(); }
   
   public void ChangeDifficulty(bool next)
   {
       if (next && m_gameData.CurrentDiffNum < 2)
           m_gameData.CurrentDiffNum++; 
       else if (!next && m_gameData.CurrentDiffNum > 0)
           m_gameData.CurrentDiffNum--; 
       else return; 
       
       GoToMenu();
   }

   private void SetDefaultJoyPositions()
   {
       JoyStick movementJoy = m_players[0].GetComponent<APRController>().Joystick;
       JoyStick controlJoy = m_players[0].GetComponent<ThrowingController>().ControlJoystick;
       movementJoy.Settings.IsTouched = false;
       movementJoy.Settings.DragPos = Vector3.zero;
       controlJoy.Settings.IsTouched = false;
       controlJoy.Settings.DragPos = Vector3.zero;
   }

   public void ChangePlayerSkin()
   {
       ItemSkin itemSkin = (ItemSkin) m_gameData.Items.Find(skin =>
           skin.ItemId == m_gameData.UserData.CurrentSkinId);
       Material mat = itemSkin.GetMaterial();

       Color col = new Color(mat.color.r, mat.color.g, mat.color.b, 1f);
       m_PlayerTeamText.color = col; m_PlayerScoreText.color = col;
       playerSkinMeshRenderer.material = mat;
       foreach (var meshRenderer in playerOtherMeshRenderers) { meshRenderer.material = mat; }

   }
   
}
