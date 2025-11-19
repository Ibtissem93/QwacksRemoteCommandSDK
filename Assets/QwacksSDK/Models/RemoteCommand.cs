using System;
using UnityEngine;

namespace QwacksSDK.Models
{
    /// <summary>
    /// Represents a remote command received from the server
    /// </summary>
    [Serializable]
    public class RemoteCommand
    {
        public string command;
        public string parameters;

        public RemoteCommand(string command, string parameters = null)
        {
            this.command = command;
            this.parameters = parameters;
        }
    }
}