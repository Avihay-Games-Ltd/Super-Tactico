
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class Game
{
    
    public enum GameType
    {
        GameOnSameComputer, GameAI, GameMultiplayer
    }
    public enum Armies
    {
        American_British , Russian_French , German_AustroHungarian 
    }

    public enum Type
    {
        Land, Sea, Both
    }

    public enum Direction
    {
        Right, Left, Up, Down, None
    }

    public enum ClickedGameObject
    {
        ClickedTile , ClickedLoadedTool
    }
    protected static class ToolsNames
    {
        public const int USMarshal = 0;
        public const int USGeneral = 1;
        public const int USColonel = 2;
        public const int USMajor = 3;
        public const int USSergeant = 4;
        public const int B3 = 5;
        public const int B2 = 6;
        public const int B1 = 7;
        public const int GMarshal = 8;
        public const int GGeneral = 9;
        public const int GColonel = 10;
        public const int GMajor = 11;
        public const int GSergeant = 12;
        public const int AU3 = 13;
        public const int AU2 = 14;
        public const int AU1 = 15;
        public const int RMarshal = 16;
        public const int RGeneral = 17;
        public const int RColonel = 18;
        public const int RMajor = 19;
        public const int RSergeant = 20;
        public const int F3 = 21;
        public const int F2 = 22;
        public const int F1 = 23;
        public const int USAircraft = 24;
        public const int GAircraft = 25;
        public const int RAircraft = 26;
        public const int USReconnaissance_Plane = 27;
        public const int GReconnaissance_Plane = 28;
        public const int RReconnaissance_Plane = 29;
        public const int USM7_Ship = 30;
        public const int GM7_Ship = 31;
        public const int RM7_Ship = 32;
        public const int USM4_Ship = 33;
        public const int GM4_Ship = 34;
        public const int RM4_Ship = 35;
        public const int USReconnaissance_Boat = 36;
        public const int GReconnaissance_Boat = 37;
        public const int RReconnaissance_Boat = 38;
        public const int USRescue_Boat = 39;
        public const int GRescue_Boat = 40;
        public const int RRescue_Boat = 41;
        public const int USFlag = 42;
        public const int GFlag = 43;
        public const int RFlag = 44;
    }
    protected static GameType gameType;
    protected static Game game;
    protected Board board;
    [SerializeField]
    protected GameObject[] Tools;
    [SerializeField]
    protected TMPro.TMP_Text PlayerTurn;
    protected TMPro.TMP_Text GameLog;
    protected GameObject GameUI;
    protected Tile ClickedTile;
    protected Tile TileToWalk;
    protected GameTool ClickedLoadedTool;
    protected GameTool Loader;
    protected List<GameTool> Player1Tools;
    protected List<GameTool> Player2Tools;
    protected List<Tile> CanWalkToTiles;
    protected List<Tile> CanBeClickedTiles;
    protected int TurnPlayerID;
    protected bool GameOver;
    protected int RandomCountPlayer1;
    protected int RandomCountPlayer2;
    protected bool RandomEndPlayer1;
    protected bool RandomEndPlayer2;
    protected CameraRotation cameraRotation;

    public static void SetGameType(GameType gameType)
    {
        Game.gameType = gameType;       
    }
    public static Game GetGame(GameObject[] Tools, GameObject[] ToolsLoadingTitles, GameObject PlayerTurnText)
    {
        if(game != null)
        {
            game = null;
        }

        if(gameType == GameType.GameOnSameComputer)
        {
            game = new GameOnSameComputer(Tools, ToolsLoadingTitles, PlayerTurnText);
        }
        else if(gameType == GameType.GameAI)
        {
            game = new GameAI(Tools, ToolsLoadingTitles, PlayerTurnText);
        }

        return game;
    }


    protected abstract void SetTurnDetails();

    protected abstract void GameOverCheck();
    public abstract void PassTurn();


    public int GetRandomizeLeftPlayer1()
    {
        return RandomCountPlayer1;
    }

    public int GetRandomizeLeftPlayer2()
    {
        return RandomCountPlayer2;
    }
    public void EndRandom(int playerID)
    {
        if(playerID == 1)
        {
            RandomEndPlayer1 = true; 
          
            GameObject.Destroy(GameUI.gameObject.transform.Find("P1Random").gameObject);
           GameObject.Destroy(GameUI.gameObject.transform.Find("ReadyButton1").gameObject);

            if (RandomEndPlayer2)
            {
                PassTurn();
            }
        }
        else if (playerID == 2)
        {
            RandomEndPlayer2 = true;
            GameObject.Destroy(GameUI.gameObject.transform.Find("P2Random").gameObject);
            GameObject.Destroy(GameUI.gameObject.transform.Find("ReadyButton2").gameObject);

            if (RandomEndPlayer1)
            {
                PassTurn();
            }
        }
    }


    protected void ToolsInit()
    {
        RandomizePlayer1Tools();
        RandomizePlayer2Tools();
    }
    public void RandomizePlayer1Tools()
    {
        foreach (Tile tile in board.GetPlayer1Land())
        {
            if (tile.GetCurrentStepingGameTool() != null)
            {
                GameObject.Destroy(tile.GetCurrentStepingGameTool().gameObject.transform.parent.gameObject);
                tile.SetCurrentStepingGameTool(null);
            }
        }
        foreach (Tile tile in board.GetPlayer1Sea())
        {
            if (tile.GetCurrentStepingGameTool() != null)
            {
                GameObject.Destroy(tile.GetCurrentStepingGameTool().gameObject.transform.parent.gameObject);
                tile.SetCurrentStepingGameTool(null);
            }
        }
        foreach (Tile tile in board.GetPlayer1Island())
        {
            if (tile.GetCurrentStepingGameTool() != null)
            {
                GameObject.Destroy(tile.GetCurrentStepingGameTool().gameObject.transform.parent.gameObject);
                tile.SetCurrentStepingGameTool(null);
            }
        }
        Player1Tools.Clear();
        List<Tile> Player1Land = new List<Tile>(board.GetPlayer1Land());
        List<Tile> Player1Sea = new List<Tile>(board.GetPlayer1Sea());
        List<Tile> Player1Island = new List<Tile>(board.GetPlayer1Island());
        List<int> Player1ToolsNumbers = new List<int>();
        FillToolsLists(Player1ToolsNumbers, Armies.American_British);
        Tile temp;
        Vector3 pos;
        GameObject tempTool;
        GameTool tempGameTool;
        while (Player1ToolsNumbers.Count > 0)
        {
            int tool = Player1ToolsNumbers[UnityEngine.Random.Range(0, Player1ToolsNumbers.Count)];

            if (tool >= 42 && tool <= 44)
            {
                temp = Player1Land[Random.Range(0, Player1Land.Count)];
                pos = new Vector3(temp.gameObject.transform.position.x, temp.gameObject.transform.position.y, temp.gameObject.transform.position.z);
                tempTool = Instantiate(Tools[tool], pos, Quaternion.identity);
                Player1Land.Remove(temp);


            }
            else if (tool >= 24 && tool <= 29)
            {
                temp = Random.Range(1, 3) == 1 ? Player1Land[Random.Range(0, Player1Land.Count)] : Player1Sea[Random.Range(0, Player1Sea.Count)];
                pos = new Vector3(temp.gameObject.transform.position.x, temp.gameObject.transform.position.y, temp.gameObject.transform.position.z);
                tempTool = Instantiate(Tools[tool], pos, Quaternion.identity);
                if (Player1Land.Contains(temp)) Player1Land.Remove(temp);
                else Player1Sea.Remove(temp);
            }
            else if (tool >= 30 && tool <= 41)
            {
                temp = Player1Sea[Random.Range(0, Player1Sea.Count)];
                pos = new Vector3(temp.gameObject.transform.position.x, temp.gameObject.transform.position.y, temp.gameObject.transform.position.z);
                tempTool = Instantiate(Tools[tool], pos, Quaternion.identity);
                Player1Sea.Remove(temp);
            }
            else
            {
                temp = board.IslandFullForSetup(1) ? Player1Land[Random.Range(0, Player1Land.Count)] : (Random.Range(1, 3) == 1 ? Player1Land[Random.Range(0, Player1Land.Count)] : Player1Island[Random.Range(0, Player1Island.Count)]);
                pos = new Vector3(temp.gameObject.transform.position.x, temp.gameObject.transform.position.y, temp.gameObject.transform.position.z);
                tempTool = Instantiate(Tools[tool], pos, Quaternion.identity);
                if (Player1Land.Contains(temp)) Player1Land.Remove(temp);
                else Player1Island.Remove(temp);
            }
            tempGameTool = tempTool.GetComponentInChildren<GameTool>();
            Player1Tools.Add(tempGameTool);
            tempGameTool.SetToolsPlayerId(1);
            tempGameTool.SetToolID(tool);
            temp.SetCurrentStepingGameTool(tempGameTool);
            

            Player1ToolsNumbers.Remove(tool);
        }


    }

    public int GetTurnPlayerID()
    {
        return TurnPlayerID;
    }
    
    
    protected GameObject Instantiate(GameObject gameObject, Vector3 pos, Quaternion identity)
    {
        return GameObject.Instantiate(gameObject, pos, identity);
    }
    public GameObject GetToolByID(int ToolID)
    {
        return Tools[ToolID];
    }
    public void SetToolUI(GameTool gameTool, Loading gameToolLoading,bool TurnOn)
    {
        if(!TurnOn)
        {
            
                cameraRotation.ResetPlayerTool(gameTool.GetToolsPlayerId());
            
           
            
        }
        else
        {
            cameraRotation.SetPlayerTool(gameTool, gameToolLoading);
        }
    }

    public void RandomizePlayer2Tools()
    {
        foreach (Tile tile in board.GetPlayer2Land())
        {
            if (tile.GetCurrentStepingGameTool() != null)
            {
                GameObject.Destroy(tile.GetCurrentStepingGameTool().gameObject.transform.parent.gameObject);
                tile.SetCurrentStepingGameTool(null);
            }
        }
        foreach (Tile tile in board.GetPlayer2Sea())
        {
            if (tile.GetCurrentStepingGameTool() != null)
            {
                GameObject.Destroy(tile.GetCurrentStepingGameTool().gameObject.transform.parent.gameObject);
                tile.SetCurrentStepingGameTool(null);
            }
        }
        foreach (Tile tile in board.GetPlayer2Island())
        {
            if (tile.GetCurrentStepingGameTool() != null)
            {
                GameObject.Destroy(tile.GetCurrentStepingGameTool().gameObject.transform.parent.gameObject);
                tile.SetCurrentStepingGameTool(null);
            }
        }
        Player1Tools.Clear();
        List<Tile> Player2Land = new List<Tile>(board.GetPlayer2Land());
        List<Tile> Player2Sea = new List<Tile>(board.GetPlayer2Sea());
        List<Tile> Player2Island = new List<Tile>(board.GetPlayer2Island());
        List<int> Player2ToolsNumbers = new List<int>();
        FillToolsLists(Player2ToolsNumbers, Armies.German_AustroHungarian);
        Tile temp;
        Vector3 pos;
        GameObject tempTool;
        GameTool tempGameTool;
        while (Player2ToolsNumbers.Count > 0)
        {
            int tool = Player2ToolsNumbers[Random.Range(0, Player2ToolsNumbers.Count)];

            if (tool >= 42 && tool <= 44)
            {
                temp = Player2Land[Random.Range(0, Player2Land.Count)];
                pos = new Vector3(temp.gameObject.transform.position.x, temp.gameObject.transform.position.y, temp.gameObject.transform.position.z);
                tempTool = Instantiate(Tools[tool], pos, Quaternion.identity);
                Player2Land.Remove(temp);


            }
            else if (tool >= 24 && tool <= 29)
            {
                temp = Random.Range(1, 3) == 1 ? Player2Land[Random.Range(0, Player2Land.Count)] : Player2Sea[Random.Range(0, Player2Sea.Count)];
                pos = new Vector3(temp.gameObject.transform.position.x, temp.gameObject.transform.position.y, temp.gameObject.transform.position.z);
                tempTool = Instantiate(Tools[tool], pos, Quaternion.Euler(0f, 180.0f, 0f));
                if (Player2Land.Contains(temp)) Player2Land.Remove(temp);
                else Player2Sea.Remove(temp);
            }
            else if (tool >= 30 && tool <= 41)
            {
                temp = Player2Sea[Random.Range(0, Player2Sea.Count)];
                pos = new Vector3(temp.gameObject.transform.position.x, temp.gameObject.transform.position.y, temp.gameObject.transform.position.z);
                tempTool = Instantiate(Tools[tool], pos, Quaternion.Euler(0f, 180.0f, 0f));
                Player2Sea.Remove(temp);
            }
            else
            {
                temp = board.IslandFullForSetup(2) ? Player2Land[Random.Range(0, Player2Land.Count)] : (Random.Range(1, 3) == 1 ? Player2Land[Random.Range(0, Player2Land.Count)] : Player2Island[Random.Range(0, Player2Island.Count)]);
                pos = new Vector3(temp.gameObject.transform.position.x, temp.gameObject.transform.position.y, temp.gameObject.transform.position.z);
                tempTool = Instantiate(Tools[tool], pos, Quaternion.Euler(0f, 180.0f, 0f));
                if (Player2Land.Contains(temp)) Player2Land.Remove(temp);
                else Player2Island.Remove(temp);
            }
            tempGameTool = tempTool.GetComponentInChildren<GameTool>();
            Player2Tools.Add(tempGameTool);
            tempGameTool.SetToolsPlayerId(2);
            tempGameTool.SetToolID(tool);
            temp.SetCurrentStepingGameTool(tempGameTool);

            Player2ToolsNumbers.Remove(tool);

        }
        
    }



    protected void FillToolsLists(List<int> PlayerToolsNumbers, Armies army)
    {
        if (army == Armies.American_British)
        {
            PlayerToolsNumbers.Add(ToolsNames.USMarshal);
            PlayerToolsNumbers.Add(ToolsNames.USFlag);
            for (int i = 1; i < 3; i++) PlayerToolsNumbers.Add(ToolsNames.USGeneral);
            for (int i = 1; i < 4; i++) PlayerToolsNumbers.Add(ToolsNames.USColonel);
            for (int i = 1; i < 5; i++) PlayerToolsNumbers.Add(ToolsNames.USMajor);
            for (int i = 1; i < 5; i++) PlayerToolsNumbers.Add(ToolsNames.USSergeant);
            for (int i = 1; i < 6; i++) PlayerToolsNumbers.Add(ToolsNames.B3);
            for (int i = 1; i < 6; i++) PlayerToolsNumbers.Add(ToolsNames.B2);
            for (int i = 1; i < 6; i++) PlayerToolsNumbers.Add(ToolsNames.B1);
            for (int i = 1; i < 3; i++) PlayerToolsNumbers.Add(ToolsNames.USM7_Ship);
            for (int i = 1; i < 3; i++) PlayerToolsNumbers.Add(ToolsNames.USM4_Ship);
            for (int i = 1; i < 5; i++) PlayerToolsNumbers.Add(ToolsNames.USReconnaissance_Boat);
            for (int i = 1; i < 4; i++) PlayerToolsNumbers.Add(ToolsNames.USRescue_Boat);
            for (int i = 1; i < 3; i++) PlayerToolsNumbers.Add(ToolsNames.USAircraft);
            for (int i = 1; i < 3; i++) PlayerToolsNumbers.Add(ToolsNames.USReconnaissance_Plane);
        }
        else if(army == Armies.Russian_French)
        {

            PlayerToolsNumbers.Add(ToolsNames.RMarshal);
            PlayerToolsNumbers.Add(ToolsNames.RFlag);
            for (int i = 1; i < 3; i++) PlayerToolsNumbers.Add(ToolsNames.RGeneral);
            for (int i = 1; i < 4; i++) PlayerToolsNumbers.Add(ToolsNames.RColonel);
            for (int i = 1; i < 5; i++) PlayerToolsNumbers.Add(ToolsNames.RMajor);
            for (int i = 1; i < 5; i++) PlayerToolsNumbers.Add(ToolsNames.RSergeant);
            for (int i = 1; i < 6; i++) PlayerToolsNumbers.Add(ToolsNames.F3);
            for (int i = 1; i < 6; i++) PlayerToolsNumbers.Add(ToolsNames.F2);
            for (int i = 1; i < 6; i++) PlayerToolsNumbers.Add(ToolsNames.F1);
            for (int i = 1; i < 3; i++) PlayerToolsNumbers.Add(ToolsNames.RM7_Ship);
            for (int i = 1; i < 3; i++) PlayerToolsNumbers.Add(ToolsNames.RM4_Ship);
            for (int i = 1; i < 5; i++) PlayerToolsNumbers.Add(ToolsNames.RReconnaissance_Boat);
            for (int i = 1; i < 4; i++) PlayerToolsNumbers.Add(ToolsNames.RRescue_Boat);
            for (int i = 1; i < 3; i++) PlayerToolsNumbers.Add(ToolsNames.RAircraft);
            for (int i = 1; i < 3; i++) PlayerToolsNumbers.Add(ToolsNames.RReconnaissance_Plane);
        }

        else
        {
            PlayerToolsNumbers.Add(ToolsNames.GMarshal);
            PlayerToolsNumbers.Add(ToolsNames.GFlag);
            for (int i = 1; i < 3; i++) PlayerToolsNumbers.Add(ToolsNames.GGeneral);
            for (int i = 1; i < 4; i++) PlayerToolsNumbers.Add(ToolsNames.GColonel);
            for (int i = 1; i < 5; i++) PlayerToolsNumbers.Add(ToolsNames.GMajor);
            for (int i = 1; i < 5; i++) PlayerToolsNumbers.Add(ToolsNames.GSergeant);
            for (int i = 1; i < 6; i++) PlayerToolsNumbers.Add(ToolsNames.AU3);
            for (int i = 1; i < 6; i++) PlayerToolsNumbers.Add(ToolsNames.AU2);
            for (int i = 1; i < 6; i++) PlayerToolsNumbers.Add(ToolsNames.AU1);
            for (int i = 1; i < 3; i++) PlayerToolsNumbers.Add(ToolsNames.GM7_Ship);
            for (int i = 1; i < 3; i++) PlayerToolsNumbers.Add(ToolsNames.GM4_Ship);
            for (int i = 1; i < 5; i++) PlayerToolsNumbers.Add(ToolsNames.GReconnaissance_Boat);
            for (int i = 1; i < 4; i++) PlayerToolsNumbers.Add(ToolsNames.GRescue_Boat);
            for (int i = 1; i < 3; i++) PlayerToolsNumbers.Add(ToolsNames.GAircraft);
            for (int i = 1; i < 3; i++) PlayerToolsNumbers.Add(ToolsNames.GReconnaissance_Plane);
        }
    }

    public void SetClickedTile(Tile clickedTile)
    {
     

            if (ClickedLoadedTool != null)
            {

                ResetClickedLoadedTool();
            }
            DeactivateTileMovementOptions();
            ClickedTile = clickedTile;
            ActivateTileMovementOptions(ClickedGameObject.ClickedTile);
        
    }
    public void SetClickedLoadedTool(GameTool Loader , GameTool clickedLoadedTool)
    {
        if (ClickedLoadedTool != null)
        {
            ResetClickedLoadedTool();
        }
        if (ClickedTile != null)
        {
            ClickedTile = null;
        }
        DeactivateTileMovementOptions();
        ClickedLoadedTool = clickedLoadedTool;
        this.Loader = Loader;
        ClickedTile = FindTile(Loader);
        ActivateTileMovementOptions(ClickedGameObject.ClickedLoadedTool);

    }
    
    public GameTool GetLoader()
    {
        return Loader;
    }
    public void ResetClickedTile()
    {
        if(ClickedTile != null)
        {
            DeactivateTileMovementOptions();
        }
        ClickedTile = null;
        
    }
    protected void ResetClickedLoadedTool()
    {
        ClickedLoadedTool.GetComponentInParent<Loading>().ResetLoadedToolsList();
        ClickedLoadedTool = null;
        Loader = null;
        ClickedTile = null;
    }



    protected void DeactivateTileMovementOptions()
    {
        foreach (Tile tile in CanWalkToTiles)
        {
            tile.UnSetCanWalkTo();
        }
        CanWalkToTiles.Clear();

    }

    protected void ActivateTileMovementOptions(ClickedGameObject clickedGameObject)
    {
        bool isNotPlane = true;
        if (ClickedTile.GetCurrentStepingGameTool().gameObject.tag == "Plane")
        {
            isNotPlane = false;
        }
        SetCanWalkIfPosibile(Direction.Right, isNotPlane,clickedGameObject);
        SetCanWalkIfPosibile(Direction.Left, isNotPlane, clickedGameObject);
        SetCanWalkIfPosibile(Direction.Up, isNotPlane, clickedGameObject);
        SetCanWalkIfPosibile(Direction.Down, isNotPlane, clickedGameObject);





    }
    protected void SetCanWalkIfPosibile(Direction direction, bool HasTilesLimit,ClickedGameObject  clickedGameObject)
    {
        if (clickedGameObject == ClickedGameObject.ClickedTile)
        {
            Tile temp = board.GetNeighbour(ClickedTile, direction);

            if (temp != null)
            {

                do
                {
                    if (temp.GetType() == ClickedTile.GetType() || (ClickedTile.GetCurrentStepingGameTool().GetToolType() == Type.Both))
                    {
                        if (temp.GetCurrentStepingGameTool() == null)
                        {
                            temp.SetCanWalkTo();
                            CanWalkToTiles.Add(temp);
                        }
                        else
                        {
                            if (temp.GetCurrentStepingGameTool().GetToolsPlayerId() != ClickedTile.GetCurrentStepingGameTool().GetToolsPlayerId())
                            {
                                if(temp.GetCurrentStepingGameTool().GetName() != "Flag")
                                {
                                    temp.SetCanWalkTo();
                                    CanWalkToTiles.Add(temp);
                                    break;
                                }
                                else
                                {
                                    if(ClickedTile.GetCurrentStepingGameTool().transform.parent.tag == "Soldier")
                                    {
                                        temp.SetCanWalkTo();
                                        CanWalkToTiles.Add(temp);
                                        break;
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                                
                            }
                            else if (ClickedTile.GetCurrentStepingGameTool().GetComponentInParent<Loading>().CanBeLoadTo(temp.GetCurrentStepingGameTool()))
                            {
                                temp.SetCanWalkTo();
                                CanWalkToTiles.Add(temp);
                                break;
                            }
                            else
                            {
                                break;
                            }
                        }

                    }
                    else if (temp.GetCurrentStepingGameTool() != null && temp.GetCurrentStepingGameTool().GetToolsPlayerId() == ClickedTile.GetCurrentStepingGameTool().GetToolsPlayerId() && ClickedTile.GetCurrentStepingGameTool().GetComponentInParent<Loading>().CanBeLoadTo(temp.GetCurrentStepingGameTool()))
                    {
                        temp.SetCanWalkTo();
                        CanWalkToTiles.Add(temp);
                        break;
                    }



                    if (temp.GetRowNum() == 20 && direction == Direction.Right || temp.GetRowNum() == 1 && direction == Direction.Up || temp.GetRowNum() == 20 && direction == Direction.Left)
                    {
                        break;
                    }


                    temp = board.GetNeighbour(temp, direction);
                    

                } while (temp != null && !HasTilesLimit);

            }
           
        }
        else if (clickedGameObject == ClickedGameObject.ClickedLoadedTool)
        {
            if (ClickedTile == null) Debug.Log("Clicked Tile is null");
            Tile temp = board.GetNeighbour(ClickedTile, direction);
            if (temp == null) Debug.Log("temp Tile is null");
            if (temp != null)
            {
                if (temp.GetCurrentStepingGameTool() == null)
                {
                    if (ClickedLoadedTool.GetToolType() == temp.GetType() || ClickedLoadedTool.GetToolType() == Type.Both)
                    {
                        temp.SetCanWalkTo();
                        CanWalkToTiles.Add(temp);
                    }
                }
                else
                {
                    if (ClickedLoadedTool.GetToolType() == temp.GetType() || ClickedLoadedTool.GetToolType() == Type.Both)
                    {
                        if (temp.GetCurrentStepingGameTool().GetToolsPlayerId() == ClickedLoadedTool.GetToolsPlayerId())
                        {
                            if (ClickedLoadedTool.GetComponentInParent<Loading>().CanBeLoadTo(temp.GetCurrentStepingGameTool()))
                            {
                                temp.SetCanWalkTo();
                                CanWalkToTiles.Add(temp);
                            }
                        }
                        else
                        {
                            temp.SetCanWalkTo();
                            CanWalkToTiles.Add(temp);
                        }
                    }
                    else if(ClickedLoadedTool.GetToolType() != temp.GetType() && temp.GetCurrentStepingGameTool().GetToolsPlayerId() == ClickedLoadedTool.GetToolsPlayerId() && ClickedLoadedTool.GetComponentInParent<Loading>().CanBeLoadTo(temp.GetCurrentStepingGameTool()))
                    {
                        temp.SetCanWalkTo();
                        CanWalkToTiles.Add(temp);
                    }

                }
            }
        }
    }

    protected void SetCanBeClickedTiles(int activePlayer)
    {
        ResetCanBeClickedTiles();
        foreach (Tile tile in board.GetAllTiles())
        {
            
            if (tile.GetCurrentStepingGameTool() != null && tile.GetCurrentStepingGameTool().GetComponentInParent<ToolMovement>() != null)
            {
                
                if (tile.GetCurrentStepingGameTool().GetToolsPlayerId() == activePlayer)
                {
                    
                    tile.setCanBeClicked(true);
                    CanBeClickedTiles.Add(tile);

                }
                else
                {
                    tile.setCanBeClicked(false);
                }

            }
            else
            {
                tile.setCanBeClicked(false);
            }
        }
    }
    public Tile GetClickedTile()
    {
        return ClickedTile;
    }
    public Tile GetTileToWalk()
    {
        return TileToWalk;
    }
    public Tile FindTile(GameTool gametool)
    {
        foreach (Tile tile in board.GetAllTiles())
        {
            if(tile.GetCurrentStepingGameTool() == gametool)
            {
                return tile;
            }
        }

        return null;
    }
    public Tile FindTile(int row , int tile)
    {
        foreach (Tile Tile in board.GetAllTiles())
        {
            if (Tile.GetRowNum() == row && Tile.GetTileNum() == tile)
            {
                return Tile;
            }
        }

        return null;
    }
    protected void ResetCanBeClickedTiles()
    {
        foreach(Tile tile in CanBeClickedTiles)
        {
            tile.setCanBeClicked(false);
        }

        CanBeClickedTiles.Clear();
    }

    public void MoveTo(Tile tile)
    {
        TileToWalk = tile;
        if (ClickedLoadedTool == null)
        {


            DeactivateTileMovementOptions();
            Direction direction = Direction.Up;
            if (tile.GetRowNum() > ClickedTile.GetRowNum())
            {
                direction = Direction.Right;
            }
            else if (tile.GetRowNum() < ClickedTile.GetRowNum())
            {
                direction = Direction.Left;
            }
            else if (tile.GetTileNum() > ClickedTile.GetTileNum())
            {
                direction = Direction.Up;
            }
            else if (tile.GetTileNum() < ClickedTile.GetTileNum())
            {
                direction = Direction.Down;
            }

            int tilesToPass;
            if (direction == Direction.Right || direction == Direction.Left)
            {
                tilesToPass = Mathf.Abs(ClickedTile.GetRowNum() - tile.GetRowNum());
            }
            else
            {
                tilesToPass = Mathf.Abs(ClickedTile.GetTileNum() - tile.GetTileNum());
            }



            if (tile.GetCurrentStepingGameTool() != null)
            {
                if (tile.GetCurrentStepingGameTool().GetToolsPlayerId() != TurnPlayerID)
                {
                    if (tile.GetCurrentStepingGameTool().gameObject.transform.parent.tag != "Flag")
                    {
                        Battle(ClickedTile.GetCurrentStepingGameTool(), tile.GetCurrentStepingGameTool(), tile, direction, tilesToPass);
                    }
                    else
                    {
                        DeactivateTileMovementOptions();
                        ResetCanBeClickedTiles();
                        GameLog.text = ClickedTile.GetCurrentStepingGameTool().GetName() + " of " + ClickedTile.GetCurrentStepingGameTool().GetArmy() + " army loaded the " + tile.GetCurrentStepingGameTool().GetArmy() + " army Flag";
                        ClickedTile.GetCurrentStepingGameTool().GetComponentInParent<Loading>().Load(tile.GetCurrentStepingGameTool(), tile);
                        ClickedTile.GetCurrentStepingGameTool().gameObject.GetComponent<ToolMovement>().MoveTo(direction, tilesToPass);
                 
                        


                    }
                }
                else
                {
                    GameLog.text = ClickedTile.GetCurrentStepingGameTool().GetName() + " of " + ClickedTile.GetCurrentStepingGameTool().GetArmy() + "army loaded to " + tile.GetCurrentStepingGameTool().GetName();
                    tile.GetCurrentStepingGameTool().GetComponentInParent<Loading>().Load(ClickedTile.GetCurrentStepingGameTool(), ClickedTile);
                    
                }

            }
            else
            {




                DeactivateTileMovementOptions();
                ResetCanBeClickedTiles();
                Tile tile1 = ClickedTile, tile2 = tile;
                ClickedTile.GetCurrentStepingGameTool().gameObject.GetComponent<ToolMovement>().MoveTo(direction, tilesToPass);
                GameLog.text = ClickedTile.GetCurrentStepingGameTool().GetName() + " of " + ClickedTile.GetCurrentStepingGameTool().GetArmy() + " army moved "+direction.ToString()+" to an empty tile";

            }
        }
        else
        {
            if (tile.GetCurrentStepingGameTool() ==  null)
            {
                DeactivateTileMovementOptions();
                ResetCanBeClickedTiles();
                GameLog.text =ClickedLoadedTool.GetName() + " of " + ClickedLoadedTool.GetArmy() + " army landed from "+  ClickedTile.GetCurrentStepingGameTool().GetName()+" on an empty tile";
                Loader.GetComponentInParent<Loading>().UnLoad(ClickedLoadedTool, tile);
                PassTurn();
            }
            else if (tile.GetCurrentStepingGameTool().GetToolsPlayerId() == ClickedLoadedTool.GetToolsPlayerId())
            {
                DeactivateTileMovementOptions();
                ResetCanBeClickedTiles();
                GameLog.text = ClickedLoadedTool.GetName() + " of " + ClickedLoadedTool.GetArmy() + " army moved from " + ClickedTile.GetCurrentStepingGameTool().GetName() + " to "+tile.GetCurrentStepingGameTool().GetName();
                Loader.GetComponentInParent<Loading>().UnLoad(ClickedLoadedTool);
                tile.GetCurrentStepingGameTool().GetComponentInParent<Loading>().Load(ClickedLoadedTool);
            }
            else if (tile.GetCurrentStepingGameTool().GetToolsPlayerId() != ClickedLoadedTool.GetToolsPlayerId())
            {
                DeactivateTileMovementOptions();
                ResetCanBeClickedTiles();
           
                if(ClickedLoadedTool.GetRank() == tile.GetCurrentStepingGameTool().GetRank())
                {
                    Loader.GetComponentInParent<Loading>().GetLoadedToolsList().Remove(ClickedLoadedTool);
                    if(ClickedLoadedTool.GetToolsPlayerId() == 1)
                    {
                        Player1Tools.Remove(ClickedLoadedTool);
                        Player2Tools.Remove(tile.GetCurrentStepingGameTool());
                        foreach (GameTool tool in ClickedLoadedTool.GetComponentInParent<Loading>().GetLoadedToolsList())
                        {
                            if (tool.GetName() == "Flag")
                            {
                                Player2Tools.Remove(tool);
                            }
                            else
                            {
                                if (tool.GetComponentInParent<Loading>().HasEnemyFlag())
                                {
                                    Player2Tools.Remove(tool.GetComponentInParent<Loading>().GetLoadedToolsList()[0]);
                                }
                                Player1Tools.Remove(tool);
                            }
                        }

                        foreach (GameTool tool in tile.GetCurrentStepingGameTool().GetComponentInParent<Loading>().GetLoadedToolsList())
                        {
                            if (tool.GetName() == "Flag")
                            {
                                Player1Tools.Remove(tool);
                            }
                            else
                            {
                                if (tool.GetComponentInParent<Loading>().HasEnemyFlag())
                                {
                                    Player1Tools.Remove(tool.GetComponentInParent<Loading>().GetLoadedToolsList()[0]);
                                }
                                Player2Tools.Remove(tool);
                            }
                        }
                    }
                    else
                    {
                        Player2Tools.Remove(ClickedLoadedTool);
                        Player1Tools.Remove(tile.GetCurrentStepingGameTool());


                        foreach (GameTool tool in ClickedLoadedTool.GetComponentInParent<Loading>().GetLoadedToolsList())
                        {
                            if (tool.GetName() == "Flag")
                            {
                                Player1Tools.Remove(tool);
                            }
                            else
                            {
                                if (tool.GetComponentInParent<Loading>().HasEnemyFlag())
                                {
                                    Player1Tools.Remove(tool.GetComponentInParent<Loading>().GetLoadedToolsList()[0]);
                                }
                                Player2Tools.Remove(tool);
                            }
                        }

                        foreach (GameTool tool in tile.GetCurrentStepingGameTool().GetComponentInParent<Loading>().GetLoadedToolsList())
                        {
                            if (tool.GetName() == "Flag")
                            {
                                Player2Tools.Remove(tool);
                            }
                            else
                            {
                                if (tool.GetComponentInParent<Loading>().HasEnemyFlag())
                                {
                                    Player2Tools.Remove(tool.GetComponentInParent<Loading>().GetLoadedToolsList()[0]);
                                }
                                Player1Tools.Remove(tool);
                            }
                        }
                    }

                    
                    GameObject.Destroy(ClickedLoadedTool.gameObject.transform.parent.gameObject);
                    GameObject.Destroy(tile.GetCurrentStepingGameTool().gameObject.transform.parent.gameObject);

                    GameLog.text = ClickedLoadedTool.GetName() + " of " + ClickedLoadedTool.GetArmy() + " army  from " + ClickedTile.GetCurrentStepingGameTool().GetName() + " attacked " + tile.GetCurrentStepingGameTool().GetName() +" from "+tile.GetCurrentStepingGameTool().GetArmy()+" army and they both die";

                    tile.SetCurrentStepingGameTool(null);
                    PassTurn();


                }
                else if(tile.GetCurrentStepingGameTool().GetName() == "Flag")
                {
                    GameLog.text = ClickedLoadedTool.GetName() + " of " + ClickedLoadedTool.GetArmy() + " army  from " + ClickedTile.GetCurrentStepingGameTool().GetName() + " loaded " +tile.GetCurrentStepingGameTool().GetArmy()+" army Flag";
                    ClickedLoadedTool.GetComponentInParent<Loading>().Load(tile.GetCurrentStepingGameTool(), tile);
                    Loader.GetComponentInParent<Loading>().UnLoad(ClickedLoadedTool, tile);
                    PassTurn();
                }
                else if(ClickedLoadedTool.GetRank() <= tile.GetCurrentStepingGameTool().GetRank())
                {
                    Loader.GetComponentInParent<Loading>().GetLoadedToolsList().Remove(ClickedLoadedTool);
                    if (ClickedLoadedTool.GetToolsPlayerId() == 1)
                    {
                        Player1Tools.Remove(ClickedLoadedTool);
                        foreach (GameTool tool in ClickedLoadedTool.GetComponentInParent<Loading>().GetLoadedToolsList())
                        {
                            if (tool.GetName() == "Flag")
                            {
                                Player2Tools.Remove(tool);
                            }
                            else
                            {
                                if (tool.GetComponentInParent<Loading>().HasEnemyFlag())
                                {
                                    Player2Tools.Remove(tool.GetComponentInParent<Loading>().GetLoadedToolsList()[0]);
                                }
                                Player1Tools.Remove(tool);
                            }
                        }
                    }
                    else
                    {
                        Player2Tools.Remove(ClickedLoadedTool);
                        foreach (GameTool tool in ClickedLoadedTool.GetComponentInParent<Loading>().GetLoadedToolsList())
                        {
                            if (tool.GetName() == "Flag")
                            {
                                Player1Tools.Remove(tool);
                            }
                            else
                            {
                                if (tool.GetComponentInParent<Loading>().HasEnemyFlag())
                                {
                                    Player1Tools.Remove(tool.GetComponentInParent<Loading>().GetLoadedToolsList()[0]);
                                }
                                Player2Tools.Remove(tool);
                            }
                        }
                    }
                    GameObject.Destroy(ClickedLoadedTool.gameObject.transform.parent.gameObject);
                    GameLog.text = ClickedLoadedTool.GetName() + " of " + ClickedLoadedTool.GetArmy() + " army  from " + ClickedTile.GetCurrentStepingGameTool().GetName() + " attacked " + tile.GetCurrentStepingGameTool().GetName() + " from " + tile.GetCurrentStepingGameTool().GetArmy() + " army and they has been defeated";
                    PassTurn();


                }

                else if (ClickedLoadedTool.GetRank() >= tile.GetCurrentStepingGameTool().GetRank())
                {
                    Debug.Log("ClickedLoadedTool stronger");
                    if (ClickedLoadedTool.GetToolsPlayerId() == 1)
                    {
                        Player2Tools.Remove(tile.GetCurrentStepingGameTool());
                        foreach (GameTool tool in tile.GetCurrentStepingGameTool().GetComponentInParent<Loading>().GetLoadedToolsList())

                        {
                            if (tool.GetName() == "Flag")
                            {
                                Player1Tools.Remove(tool);
                            }
                            else
                            {
                                if (tool.GetComponentInParent<Loading>().HasEnemyFlag())
                                {
                                    Player1Tools.Remove(tool.GetComponentInParent<Loading>().GetLoadedToolsList()[0]);
                                }
                                Player2Tools.Remove(tool);
                            }
                        }
                        GameObject.Destroy(tile.GetCurrentStepingGameTool().gameObject.transform.parent.gameObject);
                        tile.SetCurrentStepingGameTool(null);
                        Loader.GetComponentInParent<Loading>().UnLoad(ClickedLoadedTool,tile);
                        PassTurn();
                    }
                    
                    else
                    {
                        Player1Tools.Remove(tile.GetCurrentStepingGameTool());

                        foreach (GameTool tool in tile.GetCurrentStepingGameTool().GetComponentInParent<Loading>().GetLoadedToolsList())

                        {
                            if (tool.GetName() == "Flag")
                            {
                                Player2Tools.Remove(tool);
                            }
                            else
                            {
                                if (tool.GetComponentInParent<Loading>().HasEnemyFlag())
                                {
                                    Player2Tools.Remove(tool.GetComponentInParent<Loading>().GetLoadedToolsList()[0]);
                                }
                                Player1Tools.Remove(tool);
                            }
                        }

                        GameObject.Destroy(tile.GetCurrentStepingGameTool().gameObject.transform.parent.gameObject);
                        GameLog.text = ClickedLoadedTool.GetName() + " of " + ClickedLoadedTool.GetArmy() + " army  from " + ClickedTile.GetCurrentStepingGameTool().GetName() + " attacked " + tile.GetCurrentStepingGameTool().GetName() + " from " + tile.GetCurrentStepingGameTool().GetArmy() + " army and defeat it";
                        tile.SetCurrentStepingGameTool(null);
                        Loader.GetComponentInParent<Loading>().UnLoad(ClickedLoadedTool,tile);

                        PassTurn();
                    }
                }
            }
        }
    }
    protected void Battle(GameTool AttackTool, GameTool DefendTool, Tile DefendToolTile,Direction direction, int tilesToPass)
    {
        int p1 = AttackTool.GetToolsPlayerId();

        if (AttackTool.GetRank() == DefendTool.GetRank())
        {
            if (p1 == 1)
            {
                Player1Tools.Remove(AttackTool);
                Player2Tools.Remove(DefendTool);
                foreach(GameTool tool in AttackTool.GetComponentInParent<Loading>().GetLoadedToolsList())
                {
                    if(tool.GetName() == "Flag")
                    {
                        Player2Tools.Remove(tool);
                    }
                    else
                    {
                        if (tool.GetComponentInParent<Loading>().HasEnemyFlag())
                        {
                            Player2Tools.Remove(tool.GetComponentInParent<Loading>().GetLoadedToolsList()[0]);
                        }
                        Player1Tools.Remove(tool);
                    }
                }

                foreach (GameTool tool in DefendTool.GetComponentInParent<Loading>().GetLoadedToolsList())
                {
                    if (tool.GetName() == "Flag")
                    {
                        Player1Tools.Remove(tool);
                    }
                    else
                    {
                        if (tool.GetComponentInParent<Loading>().HasEnemyFlag())
                        {
                            Player1Tools.Remove(tool.GetComponentInParent<Loading>().GetLoadedToolsList()[0]);
                        }
                        Player2Tools.Remove(tool);
                    }
                }
            }
            else
            {
                Player2Tools.Remove(AttackTool);
                Player1Tools.Remove(DefendTool);

                
                foreach (GameTool tool in AttackTool.GetComponentInParent<Loading>().GetLoadedToolsList())
                {
                    if (tool.GetName() == "Flag")
                    {
                        Player1Tools.Remove(tool);
                    }
                    else
                    {
                        if (tool.GetComponentInParent<Loading>().HasEnemyFlag())
                        {
                            Player1Tools.Remove(tool.GetComponentInParent<Loading>().GetLoadedToolsList()[0]);
                        }
                        Player2Tools.Remove(tool);
                    }
                }

                foreach (GameTool tool in DefendTool.GetComponentInParent<Loading>().GetLoadedToolsList())
                {
                    if (tool.GetName() == "Flag")
                    {
                        Player2Tools.Remove(tool);
                    }
                    else
                    {
                        if (tool.GetComponentInParent<Loading>().HasEnemyFlag())
                        {
                            Player2Tools.Remove(tool.GetComponentInParent<Loading>().GetLoadedToolsList()[0]);
                        }
                        Player1Tools.Remove(tool);
                    }
                }
            }

            GameLog.text = AttackTool.GetName() + " of " + AttackTool.GetArmy() + " army  attacked " + DefendTool.GetName() + " from " + DefendTool.GetArmy() + " army and they both die";
            ClickedTile.SetCurrentStepingGameTool(null);
            DefendToolTile.SetCurrentStepingGameTool(null);

            GameObject.Destroy(AttackTool.gameObject.transform.parent.gameObject);
            GameObject.Destroy(DefendTool.gameObject.transform.parent.gameObject);
            PassTurn();
        }


        else if (AttackTool.GetRank() > DefendTool.GetRank())
        {
            if (p1 == 1)
            {
                foreach (GameTool tool in DefendTool.GetComponentInParent<Loading>().GetLoadedToolsList())
                {
                    if (tool.GetName() == "Flag")
                    {
                        Player1Tools.Remove(tool);
                    }
                    else
                    {
                        if (tool.GetComponentInParent<Loading>().HasEnemyFlag())
                        {
                            Player1Tools.Remove(tool.GetComponentInParent<Loading>().GetLoadedToolsList()[0]);
                        }
                        Player2Tools.Remove(tool);
                    }
                }
                Player2Tools.Remove(DefendTool);
            }
            else
            {
                foreach (GameTool tool in DefendTool.GetComponentInParent<Loading>().GetLoadedToolsList())
                {
                    if (tool.GetName() == "Flag")
                    {
                        Player2Tools.Remove(tool);
                    }
                    else
                    {
                        if (tool.GetComponentInParent<Loading>().HasEnemyFlag())
                        {
                            Player2Tools.Remove(tool.GetComponentInParent<Loading>().GetLoadedToolsList()[0]);
                        }
                        Player1Tools.Remove(tool);
                    }
                }
                Player1Tools.Remove(DefendTool);
            }

            GameLog.text = AttackTool.GetName() + " of " + AttackTool.GetArmy() + " army  attacked " + DefendTool.GetName() + " from " + DefendTool.GetArmy() + " army and defeat it";
            DefendToolTile.SetCurrentStepingGameTool(null);
            GameObject.Destroy(DefendTool.gameObject.transform.parent.gameObject);
            
            ResetCanBeClickedTiles();
            ClickedTile.GetCurrentStepingGameTool().gameObject.GetComponent<ToolMovement>().MoveTo(direction, tilesToPass);
            

        }
        else
        {
            if (p1 == 1)
            {
                foreach (GameTool tool in AttackTool.GetComponentInParent<Loading>().GetLoadedToolsList())
                {
                    if (tool.GetName() == "Flag")
                    {
                        Player2Tools.Remove(tool);
                    }
                    else
                    {
                        if (tool.GetComponentInParent<Loading>().HasEnemyFlag())
                        {
                            Player2Tools.Remove(tool.GetComponentInParent<Loading>().GetLoadedToolsList()[0]);
                        }
                        Player1Tools.Remove(tool);
                    }
                }
                Player1Tools.Remove(AttackTool);
            }
            else
            {
                foreach (GameTool tool in AttackTool.GetComponentInParent<Loading>().GetLoadedToolsList())
                {
                    if (tool.GetName() == "Flag")
                    {
                        Player1Tools.Remove(tool);
                    }
                    else
                    {
                        if (tool.GetComponentInParent<Loading>().HasEnemyFlag())
                        {
                            Player1Tools.Remove(tool.GetComponentInParent<Loading>().GetLoadedToolsList()[0]);
                        }
                        Player2Tools.Remove(tool);
                    }
                }
                Player2Tools.Remove(AttackTool);
            }

            ClickedTile.SetCurrentStepingGameTool(null);
            GameObject.Destroy(AttackTool.gameObject.transform.parent.gameObject);
            PassTurn();
        }
    }
    

    



}
