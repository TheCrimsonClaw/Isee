using UnityEngine;
using System.Collections;

using System.IO;
using System.Text.RegularExpressions;

public class EndScene : MonoBehaviour
{
    public Scores[] scores = new Scores[0];
    public string fileName = "score.dat";

    private string username = "";
    private bool Stored = false;

    private Vector2 scrollView = Vector2.zero;

    private void Start()
    {
        if (File.Exists(fileName))
        {
            string data;
            try
            {
                StreamReader read = new StreamReader(fileName);
                data = read.ReadToEnd();
                read.Close();

                MatchCollection matches = Regex.Matches(data, @"([a-zA-Z0-9]+),\s*([0-9]+);");
                scores = new Scores[matches.Count];
                int count = 0;
                foreach (Match m in matches)
                {
                    Debug.Log(m.Groups[1] + ": " + m.Groups[2]);
                    scores[count].name = m.Groups[1].ToString();
                    scores[count].score = int.Parse(m.Groups[2].ToString());
                    count++;
                }
            }
            catch (System.Exception e)
            {
                Debug.Log("ERROR: " + e);
            }
        }
        else
            Debug.Log("error");


    }


    private void OnGUI()
    {
        if (GUILayout.Button("Back to main menu"))
        {
            Application.LoadLevel("Menu");
        }
        if (!Stored)
        {
            username = GUILayout.TextField(username);
            if (GUILayout.Button("Finish"))
            {
                Stored = true;
                StreamWriter write = new StreamWriter(fileName);

                string data = "";

                for (int x = 0; x < scores.Length; x++)
                {
                    data += scores[x].name + ", " + scores[x].score + ";";
                }

                data = username + ", " + Player.playerScore + "; " + data;

                write.Write(data);

                write.Close();
            }
        }
        else
        {
            scrollView = GUILayout.BeginScrollView(scrollView);

            GUILayout.Label(username + ": " + Player.playerScore);

            for (int x = 0; x < scores.Length; x++ )
            {
                GUILayout.Label(scores[x].name + ": " + scores[x].score);
            }

            GUILayout.EndScrollView();
        }
    }
}

public class Scores
{
    public string name;
    public int score;
}