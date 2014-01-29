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
                GUILayout.Label("How To Play this game: \n \n Controls: Move - Arrow Keys Activate Sonar Sensor - Left Click" +
                    " Activate Red Sonar to scare enemies - Right Click Aim - Use Mouse Scroll Power-Ups - Mouse Wheel" +
                    "\n \n Collect: \n Blood Droplets - 100 pts Coffins - 500 pts Gather as many points as you can!" +
                    "\n \n Avoid: \n Crosses Holy Water Droplets Suns" +
                    "\n \n Utilise Power-Ups: Infinite Sonar Stun Wave Invincibility");
                if (GUILayout.Button("Back"))
                {
                    audio.clip = retreat;
                    audio.Play();
                    menu = 0;
                }
                break;
            case 2: // About
                GUILayout.Label("Bats see the world in a different way from us humans. They don't see the world because they are blind, they 'see' the world through their other senses, hearing. You As the player, are embarked on a journey to guide the vampire bat back to the cave before the sun rises, using her Sonar Vision to illuminate the path ahead and avoid the dangers that are out to get you.");
                if (GUILayout.Button("Back"))
                {
                    menu = 0;
                }
                break;
            default:
            case 0: // Main Menu
                if (GUILayout.Button("Solo"))
                {
                    Application.LoadLevel(1);
                }
                if (GUILayout.Button("About"))
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
}