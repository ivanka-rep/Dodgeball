using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Configs.GameConfig;
using Items;

public class GameData : MonoBehaviour
{
    public static GameData s_instance;
    
    [SerializeField] private List<Item> m_items;
    
    public List<Item> Items => m_items;

    public float MusicVolume { get; set; } = 1.0f;
    public float GameVolume { get; set; } = 1.0f;
    public int RoundNumber { get; set; } = -1;
    public int[] TotalScore { get; set; }
    public bool NextRoundAvailable { get; set ; } = false;
    public bool IsPractice { get; set; } = false;
    public bool IsRestart { get; set; } = false;
    public int CurrentDiffNum { get; set; } = 0;

    public UserDataModel UserData => m_userData; 
    public string[] PlayerItems { get => m_playerItems; set => m_playerItems = value; }

    private UserDataModel m_userData;
    private string[] m_playerItems;
    
    private void Awake()
    {
        if (s_instance == null) { DontDestroyOnLoad(gameObject); s_instance = this; }
        else if (s_instance != this) { Destroy(gameObject); }

        if (PlayerPrefs.GetInt("CLEAR_PLAYER_PREFS", 0) == 0)
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.SetInt("CLEAR_PLAYER_PREFS", 1);
        }
        
        MusicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.75f);
        GameVolume = PlayerPrefs.GetFloat("GameVolume", 0.75f);
        
        TotalScore = new int [2];
        // AudioManager.s_instance.PlayMusic(AudioManager.AudioType.MenuMusic);
        
        GetUserData();
    }
    
    void GetUserData()
    {
        string defaultUserData = "{\"UserName\":\"User\",\"CurrentBallId\":\"ball_0\",\"UserLevel\":\"0\",\"PlayedGames\":\"0\"," + 
                                 "\"Wins\":\"0\",\"Cash\":\"100\",\"DonateCash\":\"0\",\"CurrentSkinId\":\"skin_0\"}";
        m_userData = JsonUtility.FromJson<UserDataModel>(PlayerPrefs.GetString("userDataLocal", defaultUserData));
        m_playerItems = PlayerPrefs.GetString("PlayerItems", "ball_0,skin_0").Split(',');
        SaveUserData();
        SceneManager.LoadScene("PlayScene1");
    }

    public void SaveUserData()
    {
        PlayerPrefs.SetString("userDataLocal", JsonUtility.ToJson(m_userData));
        PlayerPrefs.SetString("PlayerItems", string.Join(",", m_playerItems));
        
        PlayerPrefs.Save();
    }

    public void SaveSettings()
    {
        PlayerPrefs.SetFloat("MusicVolume", MusicVolume);
        PlayerPrefs.SetFloat("GameVolume", GameVolume);
    }
    
    public class UserDataModel
    {
        public string UserName;
        public string CurrentBallId;
        public int UserLevel;
        public int PlayedGames;
        public int Wins;
        public int Cash;
        public int DonateCash;
        public string CurrentSkinId;
    }
}
