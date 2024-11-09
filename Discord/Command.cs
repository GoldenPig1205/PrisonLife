using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Text;
using Exiled.API.Features;
using System.Linq;

using MultiBroadcast.API;
using System.Text.RegularExpressions;
using UnityEngine.Windows;

using static PrisonLife.Variables.Protocol;

namespace PrisonLife.Discord
{
    class Command
    {
        public void OnEnabled()
        {
            Task.WhenAll(
                Main()
            );
        }

        static async Task Main()
        {
            try
            {
                var listener = new HttpListener();
                listener.Prefixes.Add(BotAPIServer);
                listener.Start();
                Log.Info($"Listening on {BotAPIServer}");

                while (true)
                {
                    var context = await listener.GetContextAsync();
                    _ = Task.Run(() => HandleRequest(context));
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }
        }

        public static async void Request(string requestBody, HttpListenerResponse response)
        { 
            string Content = WebUtility.UrlDecode(requestBody.Replace("data=", ""));
            string[] Value = Content.Split('/');
            string result = "null";

            if (Content.StartsWith("print"))
            {
                result = Value[1];
            }
            else if (Content.StartsWith("consolecommand"))
            {
                result = Server.ExecuteCommand($"{Value[1]}");
            }
            else if (Content.StartsWith("servercommand"))
            {
                result = Server.ExecuteCommand($"/{Value[1]}");
            }
            else if (Content.StartsWith("status"))
            {
                if (Value[1] == "players")
                    result = $"{Server.PlayerCount} / {Server.MaxPlayerCount}";
            }
            else if (Content.StartsWith("update"))
            {
                string pattern = @"\)\s(.*?)\s-";

                List<string> matches = new List<string>();
                foreach (Match match in Regex.Matches(Value[1], pattern))
                    matches.Add(match.Value);

                result = string.Join("\n", matches);

                foreach (var player in Player.List)
                {
                    player.AddBroadcast(10, $"<b><size=25><color=#FAAC58>프리즌 라이프</color>의 새 릴리즈가 업데이트되었습니다!</size></b>\n<size=20>이 패치는 다음 라운드부터 적용되며, <color=#7289da>Discord</color>에서 관련 내용을 확인할 수 있습니다.</size>");
                }
            }

            response.ContentLength64 = result.Length;
            response.ContentType = "application/json";
            await response.OutputStream.WriteAsync(Encoding.UTF8.GetBytes(result), 0, result.Length);

        }

        private static async Task HandleRequest(HttpListenerContext context)
        {
            var request = context.Request;
            var response = context.Response;

            if (request.HttpMethod == "POST")
            {
                using (var reader = new StreamReader(request.InputStream, request.ContentEncoding))
                {
                    string requestBody = await reader.ReadToEndAsync();

                    Request(requestBody, response);
                }
            }
            else if (request.HttpMethod == "GET")
            {
                var responseString = "<html><body><h1>Server is running</h1></body></html>";
                var buffer = Encoding.UTF8.GetBytes(responseString);
                response.ContentLength64 = buffer.Length;
                response.ContentType = "text/html";
                await response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
            }
            else
            {
                response.StatusCode = (int)HttpStatusCode.MethodNotAllowed;
            }

            response.Close();
        }

        private static string SimpleJsonSerializer(object obj)
        {
            if (obj == null) return "null";

            var type = obj.GetType();
            if (type == typeof(string))
            {
                return $"\"{obj}\"";
            }
            else if (type.IsPrimitive)
            {
                return obj.ToString();
            }
            else if (type.IsArray)
            {
                var array = (Array)obj;
                var elements = new List<string>();
                foreach (var element in array)
                {
                    elements.Add(SimpleJsonSerializer(element));
                }
                return $"[{string.Join(",", elements)}]";
            }
            else
            {
                var properties = type.GetProperties();
                var jsonElements = new List<string>();
                foreach (var property in properties)
                {
                    var value = property.GetValue(obj);
                    jsonElements.Add($"\"{property.Name}\":{SimpleJsonSerializer(value)}");
                }
                return $"{{{string.Join(",", jsonElements)}}}";
            }
        }

        private static Dictionary<string, object> DeserializeJson(string jsonData)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonData);
        }
    }
}
