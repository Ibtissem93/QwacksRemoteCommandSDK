using System.Collections;
using UnityEngine;
using QwacksSDK.Core;
using QwacksSDK.Models;

namespace QwacksSDK.Examples
{
    /// <summary>
    /// Demonstration of QwacksSDK usage
    /// Shows command registration and execution with various data types
    /// </summary>
    public class QwacksSDKDemo : MonoBehaviour
    {
        [Header("Demo Settings")]
        [SerializeField] private bool autoRunDemo = true;
        [SerializeField] private float delayBetweenCommands = 2f;

        [Header("Visual References")]
        [SerializeField] private GameObject playerAvatar;
        [SerializeField] private Material playerMaterial;

        private QwacksRemoteSDK sdk;
        private int totalPoints = 0;

        void Start()
        {
            // Get SDK instance
            sdk = QwacksRemoteSDK.Instance;

            // Register all commands
            RegisterCommands();

            // Auto-run demo
            if (autoRunDemo)
            {
                StartCoroutine(RunDemoSequence());
            }
        }

        /// <summary>
        /// Register all available commands with the SDK
        /// </summary>
        void RegisterCommands()
        {
            Debug.Log("=== Registering Commands ===");

            // Simple commands (no parameters)
            sdk.RegisterCommand("ResetPlayer", ResetPlayer);
            sdk.RegisterCommand("ShowStatus", ShowPlayerStatus);

            // Commands with single parameter (built-in types)
            sdk.RegisterCommand<int>("AwardPoints", AwardPoints);
            sdk.RegisterCommand<string>("UpdatePlayerStatus", UpdatePlayerStatus);
            sdk.RegisterCommand<Vector3>("MovePlayer", MovePlayer);
            sdk.RegisterCommand<Color>("ChangePlayerColor", ChangePlayerColor);
            sdk.RegisterCommand<float>("SetPlayerHealth", SetPlayerHealth);

            // Commands with custom types
            sdk.RegisterCommand<PlayerData>("SyncPlayerData", SyncPlayerData);
            sdk.RegisterCommand<RewardData>("ProcessReward", ProcessReward);

            // Commands with two parameters
            sdk.RegisterCommand<Vector3, string>("TriggerEffect", TriggerEffect);

            Debug.Log($"✓ Registered {sdk.GetRegisteredCommandCount()} commands\n");
        }

        /// <summary>
        /// Simulate server sending commands in sequence
        /// </summary>
        IEnumerator RunDemoSequence()
        {
            Debug.Log("=== Starting Server Simulation ===\n");

            yield return new WaitForSeconds(1f);

            // Command 1: Award Points (int parameter)
            Debug.Log("--- Server Command 1: AwardPoints (int) ---");
            string cmd1 = @"{""command"": ""AwardPoints"", ""parameters"": ""100""}";
            sdk.ProcessCommand(cmd1);

            yield return new WaitForSeconds(delayBetweenCommands);

            // Command 2: Update Status (string parameter)
            Debug.Log("--- Server Command 2: UpdatePlayerStatus (string) ---");
            string cmd2 = @"{""command"": ""UpdatePlayerStatus"", ""parameters"": ""VIP Member""}";
            sdk.ProcessCommand(cmd2);

            yield return new WaitForSeconds(delayBetweenCommands);

            // Command 3: Move Player (Vector3 parameter - JSON object)
            Debug.Log("--- Server Command 3: MovePlayer (Vector3) ---");
            string cmd3 = @"{""command"": ""MovePlayer"", ""parameters"": ""{\""x\"": 5, \""y\"": 0, \""z\"": 3}""}";
            sdk.ProcessCommand(cmd3);

            yield return new WaitForSeconds(delayBetweenCommands);

            // Command 4: Change Color (Color parameter - JSON object)
            Debug.Log("--- Server Command 4: ChangePlayerColor (Color) ---");
            string cmd4 = @"{""command"": ""ChangePlayerColor"", ""parameters"": ""{\""r\"": 0.2, \""g\"": 0.8, \""b\"": 0.2, \""a\"": 1}""}";
            sdk.ProcessCommand(cmd4);

            yield return new WaitForSeconds(delayBetweenCommands);

            // Command 5: Process Reward (Custom Type - RewardData)
            Debug.Log("--- Server Command 5: ProcessReward (Custom Type) ---");
            string cmd5 = @"{""command"": ""ProcessReward"", ""parameters"": ""{\""points\"": 250, \""rewardType\"": \""achievement\"", \""reason\"": \""Completed daily challenge\""}""}";
            sdk.ProcessCommand(cmd5);

            yield return new WaitForSeconds(delayBetweenCommands);

            // Command 6: Sync Player Data (Complex Custom Type)
            Debug.Log("--- Server Command 6: SyncPlayerData (Complex Custom Type) ---");
            string cmd6 = @"{""command"": ""SyncPlayerData"", ""parameters"": ""{\""playerName\"": \""ProGamer123\"", \""level\"": 25, \""experience\"": 8500, \""lastPosition\"": {\""x\"": 10, \""y\"": 0, \""z\"": 15}, \""health\"": 85.5, \""maxHealth\"": 120}""}";
            sdk.ProcessCommand(cmd6);

            yield return new WaitForSeconds(delayBetweenCommands);

            // Command 7: Set Health (float parameter)
            Debug.Log("--- Server Command 7: SetPlayerHealth (float) ---");
            string cmd7 = @"{""command"": ""SetPlayerHealth"", ""parameters"": ""100.0""}";
            sdk.ProcessCommand(cmd7);

            yield return new WaitForSeconds(delayBetweenCommands);

            // Command 8: Trigger Effect (TWO parameters: Vector3 + string)
            Debug.Log("--- Server Command 8: TriggerEffect (2 parameters) ---");
            string cmd8 = @"{""command"": ""TriggerEffect"", ""param1"": ""{\""x\"": 8, \""y\"": 0, \""z\"": 8}"", ""param2"": ""explosion""}";
            sdk.ProcessCommand(cmd8);

            yield return new WaitForSeconds(delayBetweenCommands);

            // Command 9: Show Status (no parameters)
            Debug.Log("--- Server Command 9: ShowStatus (no params) ---");
            string cmd9 = @"{""command"": ""ShowStatus""}";
            sdk.ProcessCommand(cmd9);

            Debug.Log("\n=== Demo Complete ===");
        }

        #region Command Implementations

        void ResetPlayer()
        {
            if (playerAvatar != null)
            {
                playerAvatar.transform.position = Vector3.zero;
                if (playerMaterial != null)
                {
                    playerMaterial.color = Color.white;
                }
            }
            totalPoints = 0;
            Debug.Log("► Player reset to default state");
        }

        void ShowPlayerStatus()
        {
            Debug.Log($"► PLAYER STATUS: Points={totalPoints}, Position={playerAvatar.transform.position}");
        }

        void AwardPoints(int points)
        {
            totalPoints += points;
            Debug.Log($"► Awarded {points} points! Total: {totalPoints}");
        }

        void UpdatePlayerStatus(string status)
        {
            Debug.Log($"► Player status updated to: {status}");
        }

        void MovePlayer(Vector3 position)
        {
            if (playerAvatar != null)
            {
                playerAvatar.transform.position = position;
                Debug.Log($"► Player moved to: {position}");
            }
            else
            {
                Debug.LogWarning("► Player avatar not assigned!");
            }
        }

        void ChangePlayerColor(Color color)
        {
            if (playerMaterial != null)
            {
                playerMaterial.color = color;
                Debug.Log($"► Player color changed to: {color}");
            }
            else
            {
                Debug.LogWarning("► Player material not assigned!");
            }
        }

        void SetPlayerHealth(float health)
        {
            Debug.Log($"► Player health set to: {health}");
        }

        void SyncPlayerData(PlayerData data)
        {
            Debug.Log($"► Syncing player data: {data}");

            if (playerAvatar != null)
            {
                playerAvatar.transform.position = data.lastPosition;
            }
        }

        void ProcessReward(RewardData reward)
        {
            Debug.Log($"► Processing reward: {reward}");
            totalPoints += reward.points;
            Debug.Log($"► Total points after reward: {totalPoints}");
        }

        void TriggerEffect(Vector3 position, string effectType)
        {
            Debug.Log($"► Triggering effect '{effectType}' at position: {position}");

            // Spawn a simple visual indicator
            GameObject effect = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            effect.transform.position = position;
            effect.transform.localScale = Vector3.one * 0.5f;
            effect.name = $"Effect_{effectType}";

            // Destroy after 2 seconds
            Destroy(effect, 2f);
        }

        #endregion
    }
}