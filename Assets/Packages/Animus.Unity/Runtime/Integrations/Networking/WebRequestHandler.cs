using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Packages.Animus.Unity.Runtime.Infrastructure.Serialization;
using UnityEngine.Networking;

namespace Packages.Animus.Unity.Runtime.Integrations.Networking
{
    public static class WebRequestHandler
    {
        public static async Task<string> Get(string url, Dictionary<string, string> headers = null, CancellationToken token = default)
        {
            using var request = UnityWebRequest.Get(url);
            ApplyHeaders(request, headers);
            return await SendRequest(request, token);
        }

        public static async Task<T> Get<T>(string url, Dictionary<string, string> headers = null, CancellationToken token = default)
        {
            var response = await Get(url, headers, token);
            return JsonUtility.Deserialize<T>(response);
        }

        public static async Task<string> Post(string url, string jsonBody = null,
            Dictionary<string, string> headers = null, CancellationToken token = default)
        {
            using var request = new UnityWebRequest(url, UnityWebRequest.kHttpVerbPOST);
            if (!string.IsNullOrEmpty(jsonBody))
            {
                var bodyRaw = Encoding.UTF8.GetBytes(jsonBody);
                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                request.SetRequestHeader("Content-Type", "application/json");
            }

            request.downloadHandler = new DownloadHandlerBuffer();
            ApplyHeaders(request, headers);
            return await SendRequest(request, token);
        }

        public static async Task<TResponse> Post<TRequest, TResponse>(string url, TRequest payload,
            Dictionary<string, string> headers = null, CancellationToken token = default)
        {
            var jsonBody = JsonUtility.Serialize(payload);
            var response = await Post(url, jsonBody, headers, token);
            return JsonUtility.Deserialize<TResponse>(response);
        }

        private static void ApplyHeaders(UnityWebRequest request, Dictionary<string, string> headers)
        {
            if (headers == null) return;

            foreach (var header in headers)
            {
                request.SetRequestHeader(header.Key, header.Value);
            }
        }

        private static async Task<string> SendRequest(UnityWebRequest request, CancellationToken token)
        {
            var operation = request.SendWebRequest();

            while (!operation.isDone)
            {
                if (token.IsCancellationRequested)
                {
                    request.Abort(); 
                    token.ThrowIfCancellationRequested(); 
                }
                await Task.Yield();
            }

            if (request.responseCode is 204 or 404)
            {
                return null;
            }
            
            if (request.result is not (UnityWebRequest.Result.ConnectionError or UnityWebRequest.Result.ProtocolError))
            {
                return request.downloadHandler.text;
            }

            throw new Exception(request.error);
        }
    }
}