using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class GameManager : MonoBehaviour

{
    Game game;
    [SerializeField]
    GameObject[] Tools;
    [SerializeField]
    GameObject[] ToolsLoadingTitles;
    [SerializeField]
    GameObject GameUI;

    private void Start()
    {
        //Game.SetGameType(Game.GameType.GameOnSameComputer); 
        game = Game.GetGame(Tools,ToolsLoadingTitles,GameUI);
    }
    private void Update()
    {
        print(game.GetTurnPlayerID());
    }

    public void PassTurn()
    {
        game.PassTurn();
    }

    public void MoveTo(Tile tile)
    {
        game.MoveTo(tile);
    }

    public void SetClickedLoadedTool(GameTool Loader, GameTool ReadyToUnloadTool)
    {
        game.SetClickedLoadedTool(Loader,ReadyToUnloadTool);
    }
    public void SetClickedTile(Tile clickedTile)
    {
        game.SetClickedTile(clickedTile);
    }
    public void ResetClickedTile()
    {
        game.ResetClickedTile();
    }
    public Tile GetClickedTile()
    {
        return game.GetClickedTile();
    }
    public Tile GetTileToWalk()
    {
        return game.GetTileToWalk();
    }
    public GameTool GetLoader()
    {
        return game.GetLoader();
    }
    public int GetTurnPlayerID()
    {
        return game.GetTurnPlayerID();
    }
    public GameObject GetToolByID(int ToolID)
    {
        return game.GetToolByID(ToolID);
    }
    public void SetToolUI(GameTool gameTool, Loading gameToolLoading,bool TurnOn)
    {
        game.SetToolUI(gameTool, gameToolLoading,TurnOn);
    }
    public void RandomizePlayer1Tools()
    {
        game.RandomizePlayer1Tools();
    }
    public void RandomizePlayer2Tools()
    {
        game.RandomizePlayer2Tools();
    }
    public int GetRandomizeLeftPlayer1()
    {
        return game.GetRandomizeLeftPlayer1();
    }

    public int GetRandomizeLeftPlayer2()
    {
        return game.GetRandomizeLeftPlayer1();
    }
    public void EndRandom(int playerID)
    {
        game.EndRandom(playerID);
    }

}