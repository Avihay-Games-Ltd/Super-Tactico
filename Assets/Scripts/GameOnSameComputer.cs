using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOnSameComputer : Game
{


    public GameOnSameComputer(GameObject[] Tools , GameObject[] ToolsLoadingTitles, GameObject GameUI)
    {
        this.Tools = Tools;
        this.GameUI = GameUI;
        board = Board.GetBoardInstance();
        Player1Tools = new List<GameTool>();
        Player2Tools = new List<GameTool>();
        CanWalkToTiles = new List<Tile>();
        CanBeClickedTiles = new List<Tile>();
        TurnPlayerID = 0;
        RandomCountPlayer1 = 5;
        RandomCountPlayer2 = 5;
        GameOver = false;
        ClickedTile = null;
        Loader = null;
        ClickedLoadedTool = null;
        ToolsInit();
        //SetTurnDetails();
        cameraRotation = GameObject.FindGameObjectWithTag("CameraController").GetComponent<CameraRotation>();
        cameraRotation.ResetPlayerTool(1);
        cameraRotation.ResetPlayerTool(2);
        GameUI.gameObject.transform.Find("PlayerTurnText").gameObject.GetComponent<TMPro.TMP_Text>().text = "Randomize Tools Set Session";
    }

    
    protected override void SetTurnDetails()
    {
        if (TurnPlayerID == 1)
        {
            GameUI.gameObject.transform.Find("PlayerTurnText").gameObject.GetComponent<TMPro.TMP_Text>().text = "Turn of US army";
            SetCanBeClickedTiles(1);
        }
        else
        {
            GameUI.gameObject.transform.Find("PlayerTurnText").gameObject.GetComponent<TMPro.TMP_Text>().text = "Turn of Germany army";
            SetCanBeClickedTiles(2);
        }
        
        // PlayerTurnText.gameObject.GetComponent<TMPro.TMP_Text>().text = "Player " + TurnPlayerID + " Turn";
    }
    public override void PassTurn()
    {
        GameOverCheck();
        if (TurnPlayerID == 1)
        {
            TurnPlayerID = 2;
            ResetClickedTile();
            
            SetTurnDetails();


        }
        else 
        {
            TurnPlayerID = 1;
            ResetClickedTile();
            
            SetTurnDetails();


        }
    }

    

    protected override void GameOverCheck()
    {
        foreach(Tile tile in board.GetPlayer1Island())
        {
            if(tile.GetCurrentStepingGameTool() != null && tile.GetCurrentStepingGameTool().GetToolsPlayerId() == 1 && tile.GetCurrentStepingGameTool().GetComponentInParent<Loading>().HasEnemyFlag())
            {
                SceneManager.LoadScene("Main Menu");
            }
        }
        foreach (Tile tile in board.GetPlayer2Island())
        {
            if (tile.GetCurrentStepingGameTool() != null  && tile.GetCurrentStepingGameTool().GetToolsPlayerId() == 2 && tile.GetCurrentStepingGameTool().GetComponentInParent<Loading>().HasEnemyFlag())
            {
                SceneManager.LoadScene("Main Menu");
            }
        }
        if (Player1Tools.Count == 1 || Player2Tools.Count == 1)
        {
            if(Player1Tools.Count == 1 && Player2Tools.Count > 1)
            {
                SceneManager.LoadScene("Main Menu");
            }
            else if (Player2Tools.Count == 1 && Player1Tools.Count > 1)
            {
                SceneManager.LoadScene("Main Menu");
            }
        }
    }


}
