using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartPlayerVSPlayerGame()
    {
        Game.SetGameType(Game.GameType.GameOnSameComputer);
        SceneManager.LoadScene("Tactico");
    }
    public void StartPlayerVSComGame()
    {
        Game.SetGameType(Game.GameType.GameAI);
        SceneManager.LoadScene("Tactico");
    }

   
}
