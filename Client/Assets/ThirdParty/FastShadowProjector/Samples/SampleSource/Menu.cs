using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{

    void Start()
    {
        Application.targetFrameRate = 60;
    }

    void OnGUI()
    {

        float y = Screen.height - 50;
        if (GUI.Button(new Rect(10, y, 100, 40), "General"))
        {
            SceneManager.LoadScene("GeneralScene");
        }

        if (GUI.Button(new Rect(130, y, 100, 40), "Many shadows"))
        {
            SceneManager.LoadScene("ManyShadowsScene");
        }

        if (GUI.Button(new Rect(250, y, 100, 40), "Terrain"))
        {
            SceneManager.LoadScene("TerrainScene");
        }

        if (GUI.Button(new Rect(370, y, 110, 40), "Shadow triggers"))
        {
            SceneManager.LoadScene("ShadowTriggerScene");
        }
    }
}