using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
public class MainMenu : MonoBehaviour
{
    public string MainGameScene = "Game";
    public TMP_Dropdown resolutionDropdown;
    public Toggle fullscreenToggle;
    private Resolution[] resolutions;
    private bool isLoading = true;
    void Start()
    {
        if (PlayerPrefs.HasKey("fullscreen"))
        {
            fullscreenToggle.isOn = PlayerPrefs.GetInt("fullscreen") == 1 ? true : false;
        } else
        {
            fullscreenToggle.isOn = Screen.fullScreen;
        }
        bool fromPrefs = false;
        int res_w = 0, res_h = 0;
        if (PlayerPrefs.HasKey("resolution_w") && PlayerPrefs.HasKey("resolution_h") && PlayerPrefs.HasKey("resolution_r"))
        {
            fromPrefs = true;
            res_w = PlayerPrefs.GetInt("resolution_w");
            res_h = PlayerPrefs.GetInt("resolution_h");
            Screen.SetResolution(res_w, res_h, Screen.fullScreen, PlayerPrefs.GetInt("resolution_r"));
        }
        resolutions = Screen.resolutions; //Get resolutions by unity
        resolutionDropdown.ClearOptions(); //Clear existing options in dropdown
        List<string> options = new List<string>(); //Create a list of new options
        int currentResolutionIndex = 0;
        for (int i = 0; i < resolutions.Length; i++) //Loop through every resolution by unity
        {
            options.Add(resolutions[i].width + " x " + resolutions[i].height + " @ " + resolutions[i].refreshRate + " Hz"); //Add resolution as a string option
            if (fromPrefs)
            {
                if (resolutions[i].width == res_w && resolutions[i].height == res_h) //Check if the resolution is the current resolution
                {
                    currentResolutionIndex = i; //Set the current resolution index
                }
            } else
            {
                if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height) //Check if the resolution is the current resolution
                {
                    currentResolutionIndex = i; //Set the current resolution index
                }
            }
        }
        resolutionDropdown.AddOptions(options); //Add the options to the dropdown
        resolutionDropdown.value = currentResolutionIndex; //Select the current resolution
        resolutionDropdown.RefreshShownValue(); //Refresh the selected value
        isLoading = false;
    }
    public void Play()
    {
        SceneManager.LoadScene(MainGameScene);
    }
    public void Quit()
    {
        Application.Quit();
    }
    public void SetFullscreen(bool fullscreen)
    {
        if (isLoading)
        {
            return; //Stops fullscreen from "resetting"
        }
        Screen.fullScreen = fullscreen;
        PlayerPrefs.SetInt("fullscreen", fullscreen ? 1 : 0);
    }
    public void SetResolution(int resolutionIndex)
    {
        if (isLoading)
        {
            return; //Stops resolution from "resetting"
        }
        Resolution resolution = resolutions[resolutionIndex]; //Get resolution which is to be changed to
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen, resolution.refreshRate); //Set resolution
        PlayerPrefs.SetInt("resolution_w", resolution.width);
        PlayerPrefs.SetInt("resolution_h", resolution.height);
        PlayerPrefs.SetInt("resolution_r", resolution.refreshRate);
    }
}