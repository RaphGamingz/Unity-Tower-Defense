using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
public class MainMenu : MonoBehaviour
{
    public string MainGameScene = "Game";
    public TMP_Dropdown resolutionDropdown;
    private Resolution[] resolutions;
    void Start()
    {
        resolutions = Screen.resolutions; //Get resolutions by unity
        resolutionDropdown.ClearOptions(); //Clear existing options in dropdown
        List<string> options = new List<string>(); //Create a list of new options
        int currentResolutionIndex = 0;
        for (int i = 0; i < resolutions.Length; i++) //Loop through every resolution by unity
        {
            options.Add(resolutions[i].width + " x " + resolutions[i].height + " @ " + resolutions[i].refreshRate + " Hz"); //Add resolution as a string option
            if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height) //Check if the resolution is the current resolution
            {
                currentResolutionIndex = i; //Set the current resolution index
            }
        }
        resolutionDropdown.AddOptions(options); //Add the options to the dropdown
        resolutionDropdown.value = currentResolutionIndex; //Select the current resolution
        resolutionDropdown.RefreshShownValue(); //Refresh the selected value
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
        Screen.fullScreen = fullscreen;
    }
    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex]; //Get resolution which is to be changed to
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen, resolution.refreshRate); //Set resolution
    }
}