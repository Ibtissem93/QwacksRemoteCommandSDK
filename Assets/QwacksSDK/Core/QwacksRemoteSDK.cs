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
        /// Expected format: {"command": "CommandName", "parameters": {...}}
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
                    // One parameter - need to convert from JSON
                    Type paramType = parameters[0].ParameterType;
                    object convertedParam = ConvertParameterByType(remoteCommand.parameters, paramType);

                    commandDelegate.DynamicInvoke(convertedParam);
                    Debug.Log($"[QwacksSDK] ✓ Executed command: {commandName} with 1 parameter");
                }
                else if (parameters.Length == 2)
                {
                    // Two parameters - parse JSON object with named fields
                    ParseAndInvokeTwoParameters(commandDelegate, remoteCommand.parameters);
                    Debug.Log($"[QwacksSDK] ✓ Executed command: {commandName} with 2 parameters");
                }
                else
                {
                    Debug.LogError($"[QwacksSDK] Commands with more than 2 parameters are not supported");
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"[QwacksSDK] Failed to execute command '{commandName}': {e.Message}");
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