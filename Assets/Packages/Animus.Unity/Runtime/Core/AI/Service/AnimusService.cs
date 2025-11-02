using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Packages.Animus.Unity.Runtime.Networking;
using Packages.Animus.Unity.Runtime.Settings;
using UnityEngine;

namespace Packages.Animus.Unity.Runtime.Core.AI.Service
{
    public static class AnimusService
    {
        private static readonly AnimusSettings Settings;

        static AnimusService()
        {
            Settings = Resources.Load<AnimusSettings>("AnimusSettings");
        }
        
        public static async Task<ApiResponse> Chat(PromptContext promptContext)
        {
            if (string.IsNullOrEmpty(Settings.apiServiceUrl))
            {
                Debug.LogError("Backend URL is not set in AnimusSettings!");
                return null;
            }

            var requestPayload = new ApiRequest<PromptContext>
            {
                Payload = promptContext
            };

            var headers = new Dictionary<string, string>();

            try
            {
                var response = await WebRequestHandler.Post<ApiRequest<PromptContext>, ApiResponse>(
                    $"{Settings.apiServiceUrl}/chat",
                    requestPayload,
                    headers
                );

                return response;
            }
            catch (Exception e)
            {
                Debug.LogError($"Error calling Animus-Backend: {e.Message}");
                return null;
            }
        }
    }
}