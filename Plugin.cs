using BepInEx;
using UnityEngine;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

[BepInPlugin("com.walkslow.gtag.discordwebhook", "GorillaTag Discord Webhook Mod", "1.2.0")]
public class SendToDiscordWebhookMod : BaseUnityPlugin
{
    private string webhookUrl = "https://discord.com/api/webhooks/your_webhook_id/your_webhook_token";
    private static readonly HttpClient httpClient = new HttpClient();

    private bool showInput = false;
    private string inputMessage = "";

    private static readonly HashSet<string> badWords = new HashSet<string>
    {
        "fuck",
        "shit",
        "bitch",
        "cunt",
        "nigger",
        "faggot",
        "asshole",
        "dick",
        "pussy",
        "bastard",
        "slut",
        "whore",
        "retard",
        "nigga",
        "cock",
        "twat",
        "spic",
        "chink",
        "kike",
        "cum",
        "fag",
        "dyke",
        "tranny",
        "rape",
        "rapist",
        "motherfucker",
        "bollocks",
        "wanker",
        "tosser",
        "bugger",
        "bollock",
        "arsehole",
        "prick",
        "shithead",
        "shitface"
        // Add/remove as needed for your use case
    };

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F9))
        {
            showInput = !showInput;
            if (showInput)
            {
                inputMessage = "";
            }
        }
    }

    void OnGUI()
    {
        if (showInput)
        {
            GUI.Window(12345, new Rect(Screen.width / 2 - 150, Screen.height / 2 - 50, 300, 120), DrawInputWindow, "Send Discord Message");
        }
    }

    void DrawInputWindow(int windowID)
    {
        GUI.Label(new Rect(10, 25, 280, 20), "Enter message to send to Discord:");
        inputMessage = GUI.TextField(new Rect(10, 45, 280, 20), inputMessage, 200);

        if (GUI.Button(new Rect(10, 70, 135, 20), "Send"))
        {
            if (!string.IsNullOrWhiteSpace(inputMessage))
            {
                if (ContainsBadWord(inputMessage))
                {
                    Logger.LogWarning("Message contains inappropriate language.");
                }
                else
                {
                    SendDiscordMessage(inputMessage);
                }
            }
            showInput = false;
        }
        if (GUI.Button(new Rect(155, 70, 135, 20), "Cancel"))
        {
            showInput = false;
        }
    }

    private bool ContainsBadWord(string message)
    {
        string lowerMsg = message.ToLower();
        foreach (string badWord in badWords)
        {
            if (lowerMsg.Contains(badWord))
            {
                return true;
            }
        }
        return false;
    }

    private async void SendDiscordMessage(string message)
    {
        var payload = new { content = message };
        string jsonPayload = JsonUtility.ToJson(payload);

        var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

        try
        {
            HttpResponseMessage response = await httpClient.PostAsync(webhookUrl, content);
            if (response.IsSuccessStatusCode)
            {
                Logger.LogInfo("Message sent to Discord webhook!");
            }
            else
            {
                Logger.LogWarning($"Failed to send message: {response.StatusCode}");
            }
        }
        catch (System.Exception ex)
        {
            Logger.LogError($"Error sending Discord webhook: {ex.Message}");
        }
    }
}
