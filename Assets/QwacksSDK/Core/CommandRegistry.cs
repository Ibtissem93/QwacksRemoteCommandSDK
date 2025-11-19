using System;
using System.Collections.Generic;
using UnityEngine;

namespace QwacksSDK.Core
{
    /// <summary>
    /// Manages registration and storage of remote commands
    /// </summary>
    public class CommandRegistry
    {
        private Dictionary<string, Delegate> registeredCommands;

        public CommandRegistry()
        {
            registeredCommands = new Dictionary<string, Delegate>();
        }

        /// <summary>
        /// Register a command with no parameters
        /// </summary>
        public void RegisterCommand(string commandName, Action callback)
        {
            if (string.IsNullOrEmpty(commandName))
            {
                Debug.LogError("[QwacksSDK] Command name cannot be null or empty");
                return;
            }

            if (registeredCommands.ContainsKey(commandName))
            {
                Debug.LogWarning($"[QwacksSDK] Command '{commandName}' is already registered. Overwriting...");
            }

            registeredCommands[commandName] = callback;
            Debug.Log($"[QwacksSDK] ✓ Registered command: {commandName}");
        }

        /// <summary>
        /// Register a command with one parameter
        /// </summary>
        public void RegisterCommand<T>(string commandName, Action<T> callback)
        {
            if (string.IsNullOrEmpty(commandName))
            {
                Debug.LogError("[QwacksSDK] Command name cannot be null or empty");
                return;
            }

            if (registeredCommands.ContainsKey(commandName))
            {
                Debug.LogWarning($"[QwacksSDK] Command '{commandName}' is already registered. Overwriting...");
            }

            registeredCommands[commandName] = callback;
            Debug.Log($"[QwacksSDK] ✓ Registered command: {commandName} with parameter type {typeof(T).Name}");
        }

        /// <summary>
        /// Register a command with two parameters
        /// </summary>
        public void RegisterCommand<T1, T2>(string commandName, Action<T1, T2> callback)
        {
            if (string.IsNullOrEmpty(commandName))
            {
                Debug.LogError("[QwacksSDK] Command name cannot be null or empty");
                return;
            }

            if (registeredCommands.ContainsKey(commandName))
            {
                Debug.LogWarning($"[QwacksSDK] Command '{commandName}' is already registered. Overwriting...");
            }

            registeredCommands[commandName] = callback;
            Debug.Log($"[QwacksSDK] ✓ Registered command: {commandName} with parameters ({typeof(T1).Name}, {typeof(T2).Name})");
        }

        /// <summary>
        /// Get a registered command by name
        /// </summary>
        public Delegate GetCommand(string commandName)
        {
            if (registeredCommands.TryGetValue(commandName, out Delegate command))
            {
                return command;
            }

            Debug.LogError($"[QwacksSDK] Command '{commandName}' not found. Make sure it's registered.");
            return null;
        }

        /// <summary>
        /// Check if a command is registered
        /// </summary>
        public bool IsCommandRegistered(string commandName)
        {
            return registeredCommands.ContainsKey(commandName);
        }

        /// <summary>
        /// Unregister a command
        /// </summary>
        public void UnregisterCommand(string commandName)
        {
            if (registeredCommands.Remove(commandName))
            {
                Debug.Log($"[QwacksSDK] ✓ Unregistered command: {commandName}");
            }
            else
            {
                Debug.LogWarning($"[QwacksSDK] Command '{commandName}' was not registered");
            }
        }

        /// <summary>
        /// Clear all registered commands
        /// </summary>
        public void ClearAll()
        {
            registeredCommands.Clear();
            Debug.Log("[QwacksSDK] All commands cleared");
        }

        /// <summary>
        /// Get count of registered commands
        /// </summary>
        public int GetCommandCount()
        {
            return registeredCommands.Count;
        }
    }
}