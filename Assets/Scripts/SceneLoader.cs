using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public GameObject AboutScreen;

    public void PlayGame()
    {
        Debug.Log("Playing game");
        SceneManager.LoadScene("Stage");
    }

    public void QuitGame()
    {
        Debug.Log("Quitting game");
        Application.Quit();
    }

    public void About()
    {
        Debug.Log("Returning...");
        AboutScreen.SetActive(true);
    }

    public void MainMenu()
    {
        Debug.Log("Returning...");
        AboutScreen.SetActive(false);
    }
}