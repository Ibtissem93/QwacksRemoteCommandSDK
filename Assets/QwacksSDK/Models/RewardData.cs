using System;

namespace QwacksSDK.Models
{
    /// <summary>
    /// Represents reward data that can be sent from server
    /// Used for player rewards, achievements, etc.
    /// </summary>
    [Serializable]
    public class RewardData
    {
        public int points;
        public string rewardType;
        public string reason;

        public RewardData()
        {
            points = 0;
            rewardType = "default";
            reason = "No reason specified";
        }

        public override string ToString()
        {
            return $"RewardData [Points: {points}, Type: {rewardType}, Reason: {reason}]";
        }
    }
}