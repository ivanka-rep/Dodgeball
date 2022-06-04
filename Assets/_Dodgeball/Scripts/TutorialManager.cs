using System;
using System.Collections;
using System.Collections.Generic;
using DataSakura.JoyStick.Scripts;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager s_instance;

    [Header("Objects")]
    [SerializeField] private GameObject m_player;
    [SerializeField] private List<GameObject> m_targets;
    [SerializeField] private Transform m_enemyPos;
    [SerializeField] private Transform m_playerPos;

    [Header("GUI")]
    [SerializeField] private GameObject m_tutorialOverlayPanel;
    [SerializeField] private GameObject m_congratulationsPanel;
    [SerializeField] private GameObject m_tutorialGuide1;
    [SerializeField] private GameObject m_tutorialGuide2;
    [SerializeField] private GameObject m_hand;
    [SerializeField] private GameObject m_secondHand;
    [SerializeField] private GameObject m_pauseButton;
    [SerializeField] private JoyStick m_joyStick;
    [SerializeField] private JoyStick m_secondJoyStick;
    [SerializeField] private Text m_stageText;
    [SerializeField] private Text m_scoreText;
    public bool TutorialActive => m_tutorialActive;
    public bool GuideActive => m_guideActive;

    private bool m_tutorialActive = false;
    private bool m_guideActive = false;
    
    private int m_stage = 0;
    private int m_activeTargetsCount = 0;
    private int m_playerScore = 0;
    
    public void Init()
    {
        s_instance = this;
        if (PlayerPrefs.GetInt("TutorialActive", 1) == 1 || GameData.s_instance.IsPractice)
        {m_tutorialActive = true;}
    }

    public void StartTutorial()
    {
        if (GameData.s_instance.RoundNumber == -1)
        { GameData.s_instance.RoundNumber = 1; m_stage = 1; }
        else { m_stage = GameData.s_instance.RoundNumber; }
        
        ChangeTutorialLevelStructure();
        GameManager.s_instance.StartGame();
        GameManager.s_instance.ChangePlayerSkin();
        if (m_stage == 1 && !GameData.s_instance.IsPractice) StartGuide();
    }

    private void ChangeTutorialLevelStructure()
    {
        switch (m_stage)
        {
            case 1:
                m_targets[0].SetActive(true); RotateObject(m_targets[0], -30f);
                m_targets[0].transform.position = m_enemyPos.position;
                m_player.transform.position = m_playerPos.position;
                GameManager.s_instance.LimitWalls[0].SetActive(true);
                break;
            case 2:
                m_targets[0].SetActive(true); m_targets[1].SetActive(true);
                m_player.transform.position = m_playerPos.position;
                GameManager.s_instance.LimitWalls[1].SetActive(true);
                break;
            case 3:
                m_targets.ForEach(target => target.SetActive(true));
                RotateObject(m_player, 45f );
                GameManager.s_instance.LimitWalls[2].SetActive(true);
                break;
        }

        m_targets.ForEach(target => { if (target.activeSelf) m_activeTargetsCount++; });
        
        //GUI changes
        m_stageText.text = "Tutorial stage " + m_stage;
        m_scoreText.text = "Hit all targets: " + m_playerScore + "/" + m_activeTargetsCount;
        m_tutorialOverlayPanel.SetActive(true);
        m_pauseButton.SetActive(false);
        
        void RotateObject(GameObject obj, float rotation)
        {
            Vector3 rot = obj.transform.rotation.eulerAngles;
            obj.transform.rotation = Quaternion.Euler(rot.x, rot.y + rotation, rot.z);
        }
    }

    private void StartGuide()
    {
        m_guideActive = true;
        
        m_tutorialGuide1.SetActive(true);
        m_hand.SetActive(true);
        
        m_joyStick.ONPointerDownCustomAction.AddListener(OnPlayerTouch);
        
        void OnPlayerTouch()
        {
            m_hand.SetActive(false);
            m_tutorialGuide1.SetActive(false);
            m_joyStick.ONPointerDownCustomAction.RemoveListener(OnPlayerTouch);
        }
    }

    public void StartGuide2()
    {
        m_player.GetComponent<APRController>().useControls = false;
        m_joyStick.gameObject.SetActive(false);
        m_tutorialGuide2.SetActive(true);
        m_secondHand.SetActive(true);

        m_secondJoyStick.ONPointerDownCustomAction.AddListener(OnPlayerTouch);
        
        void OnPlayerTouch()
        {
            m_player.GetComponent<APRController>().useControls = true;
            m_joyStick.gameObject.SetActive(true);
            m_tutorialGuide2.SetActive(false);
            m_secondHand.SetActive(false);
            m_guideActive = false;
            
            m_secondJoyStick.ONPointerDownCustomAction.RemoveListener(OnPlayerTouch);
        }
    }
    
    public void AddTutorialScore()
    {
        m_playerScore++;
        m_scoreText.text = "Hit all targets: " + m_playerScore + "/" + m_activeTargetsCount;
        
        if (m_playerScore == m_activeTargetsCount)
        { StartCoroutine(EndTutorialOrGoNextStage()); }
    }

    IEnumerator EndTutorialOrGoNextStage()
    {
        yield return new WaitForSeconds(1f);

        if (m_stage < 3)
        {
            GameData.s_instance.RoundNumber++;
            GameData.s_instance.CurrentDiffNum++;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        else
        {
            GameData.s_instance.CurrentDiffNum = 0;

            if (!GameData.s_instance.IsPractice)
            {
                PlayerPrefs.SetInt("TutorialActive", 0);
                
                GameManager.s_instance.DisableControls();
                m_congratulationsPanel.SetActive(true);
                GameManager.s_instance.ConfettiEffect.Play();
            }
            else
            {
                GameData.s_instance.IsPractice = false;
                GameManager.s_instance.GoToMenu();
            }
        }
    }
}
