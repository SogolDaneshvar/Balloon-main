using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuSceneLoader : MonoBehaviour
{
    private SerialPort serialPort;

    public void LoadSceneByIndex(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }

    public void RestartGameFromStart()
    {
        SceneManager.LoadScene(0);
        Time.timeScale = 1f;
    }

    /*public void RestartGameFromStartSerial()
    {
        if (serialPort != null && serialPort.IsOpen)
        {
            serialPort.Close();
            Debug.Log("Serial port closed before restarting the game.");
        }

        SceneManager.LoadScene(0);
    }*/
}
