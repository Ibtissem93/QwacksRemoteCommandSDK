using System;
using UnityEngine;

namespace QwacksSDK.Utilities
{
    /// <summary>
    /// Handles conversion from JSON strings to various data types
    /// Supports Unity types (Vector3, Color, etc.) and custom types
    /// </summary>
    public static class TypeConverter
    {
        /// <summary>
        /// Convert JSON parameter string to specified type
        /// </summary>
        public static T ConvertParameter<T>(string parameterJson)
        {
            if (string.IsNullOrEmpty(parameterJson))
            {
                Debug.LogError("[QwacksSDK] Parameter JSON is null or empty");
                return default(T);
            }

            Type targetType = typeof(T);

            try
            {
                // Clean the JSON string (remove extra quotes for primitives)
                parameterJson = parameterJson.Trim();

                // Handle primitive types
                if (targetType == typeof(int))
                {
                    // Remove quotes if wrapped
                    string cleaned = parameterJson.Trim('"');
                    return (T)(object)int.Parse(cleaned);
                }
                else if (targetType == typeof(float))
                {
                    string cleaned = parameterJson.Trim('"');
                    return (T)(object)float.Parse(cleaned);
                }
                else if (targetType == typeof(string))
                {
                    // Remove surrounding quotes if present
                    if (parameterJson.StartsWith("\"") && parameterJson.EndsWith("\""))
                    {
                        return (T)(object)parameterJson.Substring(1, parameterJson.Length - 2);
                    }
                    return (T)(object)parameterJson;
                }
                else if (targetType == typeof(bool))
                {
                    string cleaned = parameterJson.Trim('"').ToLower();
                    return (T)(object)bool.Parse(cleaned);
                }
                // Handle Unity types (these need full JSON objects)
                else if (targetType == typeof(Vector3))
                {
                    return (T)(object)JsonUtility.FromJson<Vector3>(parameterJson);
                }
                else if (targetType == typeof(Vector2))
                {
                    return (T)(object)JsonUtility.FromJson<Vector2>(parameterJson);
                }
                else if (targetType == typeof(Color))
                {
                    return (T)(object)JsonUtility.FromJson<Color>(parameterJson);
                }
                else if (targetType == typeof(Quaternion))
                {
                    return (T)(object)JsonUtility.FromJson<Quaternion>(parameterJson);
                }
                // Handle custom types (any class with [Serializable])
                else
                {
                    return JsonUtility.FromJson<T>(parameterJson);
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"[QwacksSDK] Failed to convert parameter to type {targetType.Name}: {e.Message}\nInput: {parameterJson}");
                return default(T);
            }
        }

        /// <summary>
        /// Convert a Vector3 from JSON object notation
        /// Handles both Unity's format and standard JSON format
        /// </summary>
        public static Vector3 ParseVector3(string json)
        {
            try
            {
                // Try Unity's JsonUtility first
                return JsonUtility.FromJson<Vector3>(json);
            }
            catch
            {
                Debug.LogError($"[QwacksSDK] Failed to parse Vector3 from: {json}");
                return Vector3.zero;
            }
        }

        /// <summary>
        /// Convert a Color from JSON object notation
        /// </summary>
        public static Color ParseColor(string json)
        {
            try
            {
                return JsonUtility.FromJson<Color>(json);
            }
            catch
            {
                Debug.LogError($"[QwacksSDK] Failed to parse Color from: {json}");
                return Color.white;
            }
        }

        /// <summary>
        /// Validate if a string is valid JSON
        /// </summary>
        public static bool IsValidJson(string jsonString)
        {
            if (string.IsNullOrWhiteSpace(jsonString))
                return false;

            jsonString = jsonString.Trim();
            return (jsonString.StartsWith("{") && jsonString.EndsWith("}")) ||
                   (jsonString.StartsWith("[") && jsonString.EndsWith("]"));
        }
    }
}