using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Packages.Animus.Unity.Runtime.Integrations.Networking;
using Packages.Animus.Unity.Runtime.Integrations.Prompting;
using Packages.Animus.Unity.Runtime.Settings;
using UnityEngine;

namespace Packages.Animus.Unity.Runtime.Integrations.Service
{
    public static class AnimusService
    {
        public static async Task<ApiResponse> Chat(PromptContext promptContext, CancellationToken cancellationToken = default)
        {
            var apiServiceUrl = AnimusSettings.Instance.apiServiceUrl;

            if (string.IsNullOrEmpty(apiServiceUrl))
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
                    $"{apiServiceUrl}/chat",
                    requestPayload,
                    headers,
                    cancellationToken
                );

                return response;
            }
            catch (OperationCanceledException)
            {
                // Rethrow to let the ActionQueue know it was cancelled intentionally
                throw;
            }
            catch (Exception e)
            {
                Debug.LogError($"Error calling Animus-Backend: {e.Message}");
                return null;
            }
        }
    }
}