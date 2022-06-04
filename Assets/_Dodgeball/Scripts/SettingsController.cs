using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

public class SettingsController : MonoBehaviour
{

    [SerializeField] private Slider m_musicVolumeSlider;
    [SerializeField] private Slider m_gameVolumeSlider;

    [SerializeField] private Button m_backButton;

    private GameManager m_gameManager;
    
    void Start()
    {
        m_musicVolumeSlider.value = GameData.s_instance.MusicVolume;
        m_gameVolumeSlider.value = GameData.s_instance.GameVolume;

        m_gameManager = GameManager.s_instance;
        
        m_musicVolumeSlider.onValueChanged.AddListener ( value =>
        {
            GameData.s_instance.MusicVolume = m_musicVolumeSlider.value;
            AudioManager.s_instance.MusicAudioSource.volume = value;
        });

        m_gameVolumeSlider.onValueChanged.AddListener (value => 
        {
            GameData.s_instance.GameVolume = m_gameVolumeSlider.value;
            AudioManager.s_instance.GameAudioSource.volume = value;
        });
        
        m_backButton.onClick.AddListener(() =>
        {
            m_gameManager.PanelsMask.SetActive(false);
            TweenAnimation.PlayAnimation(transform, AnimationType.ClosingTransition, 0.5f,
                () => gameObject.SetActive(false));
            
            GameData.s_instance.SaveSettings();
        });
    }

    void OnApplicationQuit()
    {
        GameData.s_instance.SaveSettings();
    }
}