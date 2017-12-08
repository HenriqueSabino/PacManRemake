using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIScript : MonoBehaviour
{
    public void StartGame()
    {
        PlayerPrefs.SetInt("Lifes",-1);
        PlayerPrefs.SetInt("Score", 0);
        PlayerPrefs.SetInt("LifeIndex", 0);
        PlayerPrefs.SetInt("Level", 1);
        if (!PlayerPrefs.HasKey("HighScore"))
            PlayerPrefs.SetInt("HighScore", 0);
        SceneManager.LoadScene("Map - Copia");
    }
}
