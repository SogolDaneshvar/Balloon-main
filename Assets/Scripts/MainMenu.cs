using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEngine.Rendering.VirtualTexturing.Debugging;
using UnityEngine.XR;
using TMPro;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;

public class MainMenu : MonoBehaviour
{
    public Slider controlSlider;
    public TMP_Dropdown portDropdown;
    public static string selectedPort;
    
    public static MainMenu Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        string[] availablePorts = SerialPort.GetPortNames();

        // ports sort
        List<string> sortedPorts = availablePorts
            .OrderBy(port => int.Parse(new string(port.Where(char.IsDigit).ToArray())))
            .ToList();

        int numberOfActivePorts = sortedPorts.Count;

        portDropdown.ClearOptions();
        portDropdown.AddOptions(sortedPorts);

        if (numberOfActivePorts == 1)
        {
            selectedPort = sortedPorts[0];
            Debug.Log("Only one port available: " + selectedPort);
        }
        else if (numberOfActivePorts > 1)
        {
            selectedPort = sortedPorts[0];
            Debug.Log("Multiple ports available. Default selected: " + selectedPort);

            portDropdown.onValueChanged.AddListener(delegate {
                OnPortSelected(portDropdown);
            });
        }
        else
        {
            Debug.LogError("No active ports found. Exiting application.");
            Time.timeScale = 0;
            Application.Quit();
        }
    }

    void OnPortSelected(TMP_Dropdown dropdown)
    {
        selectedPort = dropdown.options[dropdown.value].text;
        Debug.Log("Selected Port: " + selectedPort);
    }

    // Method to start the game
    public void PlayGame()
    {
        // Load the main game scene, replace "MainScene" with your game scene name
        SceneManager.LoadScene("MainScene");
    }

    // Method to exit the game
    public void ExitGame()
    {
        Application.Quit();  // Will only work in a built game, not in the editor
    }

   // Method to handle the slider value change
    public void HandleControlSelection()
    {
        int selectedOption = (int)controlSlider.value;  // 0 for Hand, 1 for Feet
        string controlChoice = selectedOption == 0 ? "Hand" : "Feet";

        // Store the selected option if needed (e.g., PlayerPrefs or other variable)
        Debug.Log("Control Selected: " + controlChoice);
    }

}
