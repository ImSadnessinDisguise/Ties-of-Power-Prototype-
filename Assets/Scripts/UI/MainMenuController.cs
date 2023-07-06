using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [Header("Levels to Load")]
    public string _newGameLevel;
    public string levelToLoad;

    [SerializeField] private GameObject noSavedgameDialog = null;


    public void newGameDialogYes()
    {
        SceneManager.LoadScene("CutScene");
    }

    public void LoadGameDialogYes()
    {
        if (PlayerPrefs.HasKey("SavedLevel"))
        {
            levelToLoad = PlayerPrefs.GetString("SavedLevel1");
            SceneManager.LoadScene(levelToLoad);
        }
        else
        {
            noSavedgameDialog.SetActive(true);
        }
    }

    public void ExitButton()
    {
        Application.Quit(); 
    }

}
