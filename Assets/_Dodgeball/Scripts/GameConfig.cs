using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Configs.GameConfig
{
    [CreateAssetMenu(fileName = "GameConfig", menuName = "ScriptableObjects/GameConfig", order = 1)]
    public class GameConfig : ScriptableObject
    {
        [Header("Settings of each difficulty")]
        public List<DifficultyParams> DifficultyParams;

        [Header("Other Parameters")] public float startDelay = 3f;
    }

    [Serializable]
    public class DifficultyParams
    {
        public DifficultyType difficulty;
        public int countOfRounds;
        public float roundTime;

        [Header("Player Params")] 
        public float playerThrowForce;

        [Header("Bot Params")] 
        public float botThrowForce;
        [Range(0f, 1f)] public float botMoveSpeed;
    }

    public enum DifficultyType
    {
        Easy,
        Normal,
        Hard
    }
}