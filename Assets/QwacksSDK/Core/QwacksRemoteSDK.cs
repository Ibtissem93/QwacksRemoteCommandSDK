using System;
using UnityEngine;
using QwacksSDK.Models;
using QwacksSDK.Utilities;

namespace QwacksSDK.Core
{
    /// <summary>
    /// Main SDK class for Qwacks Remote Command Execution
    /// Allows game servers to trigger predefined functions in the Unity client
    /// </summary>
    public class QwacksRemoteSDK
    {
        private static QwacksRemoteSDK instance;
        private CommandRegistry commandRegistry;

        // Singleton pattern
        public static QwacksRemoteSDK Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new QwacksRemoteSDK();
                }
                return instance;
            }
        }

        private QwacksRemoteSDK()
        {
            commandRegistry = new CommandRegistry();
            Debug.Log("[QwacksSDK] SDK Initialized");
        }

        #region Command Registration

        /// <summary>
        /// Register a command with no parameters
        /// </summary>
        public void RegisterCommand(string commandName, Action callback)
        {
            commandRegistry.RegisterCommand(commandName, callback);
        }

        /// <summary>
        /// Register a command with one parameter
        /// </summary>
        public void RegisterCommand<T>(string commandName, Action<T> callback)
        {
            commandRegistry.RegisterCommand(commandName, callback);
        }

        /// <summary>
        /// Register a command with two parameters
        /// </summary>
        public void RegisterCommand<T1, T2>(string commandName, Action<T1, T2> callback)
        {
            commandRegistry.RegisterCommand(commandName, callback);
        }

        /// <summary>
        /// Unregister a command
        /// </summary>
        public void UnregisterCommand(string commandName)
        {
            commandRegistry.UnregisterCommand(commandName);
        }

        #endregion

        #region Command Execution

        /// <summary>
        /// Process and execute a command from JSON message
        /// 
        /// Supported JSON formats:
        /// 
        /// No parameters:
        ///   {"command": "CommandName"}
        /// 
        /// Single parameter (primitive):
        ///   {"command": "CommandName", "parameters": "value"}
        ///   Example: {"command": "AwardPoints", "parameters": "100"}
        /// 
        /// Single parameter (object - Vector3, Color, custom types):
        ///   {"command": "CommandName", "parameters": "{\"field\": value}"}
        ///   Example: {"command": "MovePlayer", "parameters": "{\"x\": 5, \"y\": 0, \"z\": 3}"}
        /// 
        /// Two parameters:
        ///   {"command": "CommandName", "param1": "value1", "param2": "value2"}
        ///   Example: {"command": "TriggerEffect", "param1": "{\"x\": 8, \"y\": 0, \"z\": 8}", "param2": "explosion"}
        /// 
        /// Note: For object parameters (Vector3, Color, custom types), the value should be a JSON string containing the serialized object.
        /// </summary>

        public void ProcessCommand(string jsonMessage)
        {
            if (string.IsNullOrEmpty(jsonMessage))
            {
                Debug.LogError("[QwacksSDK] JSON message is null or empty");
                return;
            }

            if (!TypeConverter.IsValidJson(jsonMessage))
            {
                Debug.LogError($"[QwacksSDK] Invalid JSON format: {jsonMessage}");
                return;
            }

            try
            {
                // Parse the command
                RemoteCommand remoteCommand = JsonUtility.FromJson<RemoteCommand>(jsonMessage);

                if (string.IsNullOrEmpty(remoteCommand.command))
                {
                    Debug.LogError("[QwacksSDK] Command name is missing in JSON");
                    return;
                }

                ExecuteCommand(remoteCommand);
            }
            catch (Exception e)
            {
                Debug.LogError($"[QwacksSDK] Failed to process command: {e.Message}");
            }
        }

        /// <summary>
        /// Execute a registered command
        /// </summary>
        private void ExecuteCommand(RemoteCommand remoteCommand)
        {
            string commandName = remoteCommand.command;

            if (!commandRegistry.IsCommandRegistered(commandName))
            {
                Debug.LogError($"[QwacksSDK] Command '{commandName}' is not registered");
                return;
            }

            Delegate commandDelegate = commandRegistry.GetCommand(commandName);

            if (commandDelegate == null)
            {
                Debug.LogError($"[QwacksSDK] Failed to retrieve command '{commandName}'");
                return;
            }

            try
            {
                // Execute command based on parameter count
                var parameters = commandDelegate.Method.GetParameters();

                if (parameters.Length == 0)
                {
                    // No parameters
                    ((Action)commandDelegate).Invoke();
                    Debug.Log($"[QwacksSDK] ✓ Executed command: {commandName}");
                }
                else if (parameters.Length == 1)
                {
                    // One parameter - convert from JSON
                    Type paramType = parameters[0].ParameterType;

                    // Get the parameters field value
                    string paramJson = remoteCommand.Parameters;

                    if (string.IsNullOrEmpty(paramJson))
                    {
                        Debug.LogError($"[QwacksSDK] Command '{commandName}' expects a parameter but none provided");
                        return;
                    }

                    object convertedParam = ConvertParameterByType(paramJson, paramType);

                    commandDelegate.DynamicInvoke(convertedParam);
                    Debug.Log($"[QwacksSDK] ✓ Executed command: {commandName} with 1 parameter");
                }
                else if (parameters.Length == 2)
                {
                    // Two parameters - use param1 and param2 fields
                    Type param1Type = parameters[0].ParameterType;
                    Type param2Type = parameters[1].ParameterType;

                    string param1Json = remoteCommand.param1;
                    string param2Json = remoteCommand.param2;

                    if (string.IsNullOrEmpty(param1Json) || string.IsNullOrEmpty(param2Json))
                    {
                        Debug.LogError($"[QwacksSDK] Command '{commandName}' expects 2 parameters. Use 'param1' and 'param2' fields in JSON");
                        return;
                    }

                    object convertedParam1 = ConvertParameterByType(param1Json, param1Type);
                    object convertedParam2 = ConvertParameterByType(param2Json, param2Type);

                    commandDelegate.DynamicInvoke(convertedParam1, convertedParam2);
                    Debug.Log($"[QwacksSDK] ✓ Executed command: {commandName} with 2 parameters");
                }
                else
                {
                    Debug.LogError($"[QwacksSDK] Commands with more than 2 parameters are not supported");
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"[QwacksSDK] Failed to execute command '{commandName}': {e.Message}\n{e.StackTrace}");
            }
        }

        /// <summary>
        /// Convert parameter JSON to specific type
        /// </summary>
        private object ConvertParameterByType(string parameterJson, Type targetType)
        {
            var method = typeof(TypeConverter).GetMethod("ConvertParameter");
            var genericMethod = method.MakeGenericMethod(targetType);
            return genericMethod.Invoke(null, new object[] { parameterJson });
        }

        /// <summary>
        /// Parse and invoke command with two parameters
        /// </summary>
        private void ParseAndInvokeTwoParameters(Delegate commandDelegate, string parametersJson)
        {
            var parameters = commandDelegate.Method.GetParameters();
            Type param1Type = parameters[0].ParameterType;
            Type param2Type = parameters[1].ParameterType;

            // For simplicity, assume parameters JSON is a JSON object with two fields
            // You can extend this to handle more complex scenarios
            Debug.LogWarning("[QwacksSDK] Two-parameter commands require custom JSON parsing");
        }
        #endregion

        #region Utility Methods

        /// <summary>
        /// Get count of registered commands
        /// </summary>
        public int GetRegisteredCommandCount()
        {
            return commandRegistry.GetCommandCount();
        }

        /// <summary>
        /// Clear all registered commands
        /// </summary>
        public void ClearAllCommands()
        {
            commandRegistry.ClearAll();
        }

        #endregion
    }
}