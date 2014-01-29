using UnityEngine;
using System.Collections;

public class Menu : MonoBehaviour
{
    #region Variables
    public GUISkin skin;
    public Texture2D background;
    public AudioClip advance;
    public AudioClip retreat;

    private int menu = 0;
    private string gameName = "";
    private string ip = "127.0.0.1";
    private int maxLimit = 2;
    private int port = 25565;
    #endregion

    private void OnGUI()
    {
        GUI.skin = skin;

        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), background);
        GUI.Window(menu, new Rect(Screen.width / 2 - 200, Screen.height - 400, 400, 300), menuFunc, "");
    }

    private void menuFunc(int id)
    {
        switch (id)
        {
            case 1: // How To Play
                GUILayout.Label("How To Play this game.");
                if (GUILayout.Button("Back"))
                {
                    audio.clip = retreat;
                    audio.Play();
                    menu = 0;
                }
                break;
            case 2: // Multiplayer
                if (GUILayout.Button("Create Servers"))
                {
                    menu = 3;
                }
                if (GUILayout.Button("Browse Servers"))
                {
                    audio.clip = advance;
                    audio.Play();
                    menu = 4;
                }
                if (GUILayout.Button("Back"))
                {
                    menu = 0;
                    audio.clip = retreat;
                    audio.Play();
                }
                break;
            case 3: // Create A Server
                GUILayout.Label("Name: ");
                gameName = GUILayout.TextField(gameName);
                GUILayout.Label("Max Players: ");
                try
                {
                    maxLimit = int.Parse(GUILayout.TextField(maxLimit.ToString()));
                }
                catch (System.Exception)
                {
                    maxLimit = 2;
                }
                if (GUILayout.Button("Create"))
                {
                    Network.InitializeSecurity();
                    Network.InitializeServer(maxLimit, port, true);
                }
                if (GUILayout.Button("Back"))
                {
                    menu = 2;
                    audio.clip = retreat;
                    audio.Play();
                }
                break;
            case 4: // Browse Servers
                GUILayout.Label("IP Address");
                ip = GUILayout.TextField(ip);
                if (GUILayout.Button("Join"))
                {
                    audio.clip = advance;
                    audio.Play();
                    Network.Connect(ip, port);
                }
                if (GUILayout.Button("Back"))
                {
                    menu = 2;
                    audio.clip = retreat;
                    audio.Play();
                }
                break;
            default:
            case 0: // Main Menu
                if (GUILayout.Button("Solo"))
                {
                    Application.LoadLevel(1);
                }
                if (GUILayout.Button("Multiplayer"))
                {
                    audio.clip = advance;
                    audio.Play();
                    menu = 2;
                }
                if (GUILayout.Button("How To Play"))
                {
                    audio.clip = advance;
                    audio.Play();
                    menu = 1;
                }
                if (GUILayout.Button("Quit"))
                {
                    Application.Quit();
                }
                break;
        }
    }

    #region Network Methods

    private void OnServerInitialized()
    {
        Application.LoadLevel(1);
    }

    private void OnConnectedToServer()
    {
        Application.LoadLevelAdditive(1);
        Destroy(gameObject);
    }

    #endregion
}