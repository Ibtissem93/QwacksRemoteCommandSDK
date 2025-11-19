using System;
using UnityEngine;

namespace QwacksSDK.Models
{
    /// <summary>
    /// Represents a remote command received from the server
    /// Supports both simple and complex parameter structures
    /// </summary>
    [Serializable]
    public class RemoteCommand
    {
        public string command;

        // For single parameters (can be primitive or object)
        [SerializeField]
        private string parameters;

        // For named two-parameter commands
        public string param1;
        public string param2;

        public string Parameters
        {
            get { return parameters; }
            set { parameters = value; }
        }

        public RemoteCommand(string command, string parameters = null)
        {
            this.command = command;
            this.parameters = parameters;
        }

        /// <summary>
        /// Check if this command has parameters
        /// </summary>
        public bool HasParameters()
        {
            return !string.IsNullOrEmpty(parameters) ||
                   !string.IsNullOrEmpty(param1) ||
                   !string.IsNullOrEmpty(param2);
        }
    }
}