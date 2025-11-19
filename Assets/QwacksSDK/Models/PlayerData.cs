using System;
using UnityEngine;

namespace QwacksSDK.Models
{
    /// <summary>
    /// Example of a custom type that can be used with QwacksSDK
    /// Demonstrates how complex data structures can be sent from server
    /// </summary>
    [Serializable]
    public class PlayerData
    {
        public string playerName;
        public int level;
        public int experience;
        public Vector3 lastPosition;
        public float health;
        public float maxHealth;

        public PlayerData()
        {
            playerName = "Player";
            level = 1;
            experience = 0;
            lastPosition = Vector3.zero;
            health = 100f;
            maxHealth = 100f;
        }

        public override string ToString()
        {
            return $"PlayerData [Name: {playerName}, Level: {level}, XP: {experience}, " +
                   $"Health: {health}/{maxHealth}, Position: {lastPosition}]";
        }
    }
}