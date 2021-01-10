using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameAI : Game
{
    List<Direction> directions = new List<Direction>();
    Dictionary<int, KeyValuePair<GameTool, List<Tile>>> AllPosibileActions;




    public GameAI(GameObject[] Tools, GameObject[] ToolsLoadingTitles, GameObject GameUI)
    {
        directions.Add(Direction.Right);
        directions.Add(Direction.Left);
        directions.Add(Direction.Up);
        directions.Add(Direction.Down);
        this.Tools = Tools;
        this.GameUI = GameUI;
        board = Board.GetBoardInstance();
        Player1Tools = new List<GameTool>();
        Player2Tools = new List<GameTool>();
        CanWalkToTiles = new List<Tile>();
        CanBeClickedTiles = new List<Tile>();
        TurnPlayerID = 2;
        RandomCountPlayer1 = 5;
        RandomCountPlayer2 = 5;
        GameOver = false;
        ClickedTile = null;
        Loader = null;
        ClickedLoadedTool = null;
        ToolsInit();
        cameraRotation = GameObject.FindGameObjectWithTag("CameraController").GetComponent<CameraRotation>();
        cameraRotation.ResetPlayerTool(1);
        cameraRotation.ResetPlayerTool(2);
        GameUI.gameObject.transform.Find("PlayerTurnText").gameObject.GetComponent<TMPro.TMP_Text>().text = "Randomize Tools Set Session";
    }
    protected override void GameOverCheck()
    {
        foreach (Tile tile in board.GetPlayer1Island())
        {
            if (tile.GetCurrentStepingGameTool() != null && tile.GetCurrentStepingGameTool().transform.parent.tag == "Flag" && tile.GetCurrentStepingGameTool().GetToolsPlayerId() == 2)
            {
                SceneManager.LoadScene("Main Menu");
            }
        }
        foreach (Tile tile in board.GetPlayer2Island())
        {
            if (tile.GetCurrentStepingGameTool() != null && tile.GetCurrentStepingGameTool().transform.parent.tag == "Flag" && tile.GetCurrentStepingGameTool().GetToolsPlayerId() == 1)
            {
                SceneManager.LoadScene("Main Menu");
            }
        }
        if (Player1Tools.Count == 1 || Player2Tools.Count == 1)
        {
            if (Player1Tools.Count == 1 && Player2Tools.Count > 1)
            {
                SceneManager.LoadScene("Main Menu");
            }
            else if (Player2Tools.Count == 1 && Player1Tools.Count > 1)
            {
                SceneManager.LoadScene("Main Menu");
            }
        }
    }

    public override void PassTurn()
    {
        if(TurnPlayerID == 1)
        {
            TurnPlayerID = 2;
            SetTurnDetails();
        }
        else if(TurnPlayerID == 2)
        {
            TurnPlayerID = 1;
            SetTurnDetails();

            
        }
    }


    protected override void SetTurnDetails()
    {
        if (TurnPlayerID == 1)
        {
            GameUI.gameObject.transform.Find("PlayerTurnText").gameObject.GetComponent<TMPro.TMP_Text>().text = "Player Turn";
            SetCanBeClickedTiles(1);
        }
        else if(TurnPlayerID == 2)
        {
            GameUI.gameObject.transform.Find("PlayerTurnText").gameObject.GetComponent<TMPro.TMP_Text>().text = "Com Turn";
            ResetClickedTile();
            ResetCanBeClickedTiles();
            AI_Play();
        }
        
    }


    private void AI_Play()
    {
        AllPosibileActions = GetAllPosibileActions();
        
     
        KeyValuePair<GameTool, List<Tile>> RandomToolActions = AllPosibileActions[Random.Range(0, AllPosibileActions.Count)];
        Debug.Log(RandomToolActions.Key.transform.parent.name + " ");
        foreach(Tile tile in RandomToolActions.Value)
        {
            Debug.Log(tile.GetRowNum() + "   " + tile.GetTileNum());
        }
        Direction direction = Direction.None;
        Tile currentTile = FindTile(RandomToolActions.Key);
        Tile tileToMove = RandomToolActions.Value[Random.Range(0, RandomToolActions.Value.Count)];
        ClickedTile = currentTile;
        TileToWalk = tileToMove;
        if (tileToMove.GetRowNum() > currentTile.GetRowNum())
        {
            direction = Direction.Right;
        }
        else if (tileToMove.GetRowNum() < currentTile.GetRowNum())
        {
            direction = Direction.Left;
        }
        else if (tileToMove.GetTileNum() > currentTile.GetTileNum())
        {
            direction = Direction.Up;
        }
        else if (tileToMove.GetTileNum() < currentTile.GetTileNum())
        {
            direction = Direction.Down;
        }

        int tilesToPass;
        if (direction == Direction.Right || direction == Direction.Left)
        {
            tilesToPass = Mathf.Abs(currentTile.GetRowNum() - tileToMove.GetRowNum());
        }
        else
        {
            tilesToPass = Mathf.Abs(currentTile.GetTileNum() - tileToMove.GetTileNum());
        }

        if (tileToMove.GetCurrentStepingGameTool() != null)
        {
            if (tileToMove.GetCurrentStepingGameTool().GetToolsPlayerId() != TurnPlayerID)
            {
                if (tileToMove.GetCurrentStepingGameTool().gameObject.transform.parent.tag != "Flag")
                {
                    Battle(currentTile.GetCurrentStepingGameTool(), tileToMove.GetCurrentStepingGameTool(), tileToMove, direction, tilesToPass);
                }
                else
                {
                    
                    currentTile.GetCurrentStepingGameTool().GetComponentInParent<Loading>().Load(tileToMove.GetCurrentStepingGameTool(), tileToMove);
                    currentTile.GetCurrentStepingGameTool().gameObject.GetComponent<ToolMovement>().MoveTo(direction, tilesToPass);
                    tileToMove.SetCurrentStepingGameTool(currentTile.GetCurrentStepingGameTool());
                    currentTile.SetCurrentStepingGameTool(null);
                }
            }
            else
            {
                Loading(tileToMove.GetCurrentStepingGameTool(), currentTile.GetCurrentStepingGameTool(), currentTile, direction, tilesToPass);
            }

        }
        else
        {




            
            currentTile.GetCurrentStepingGameTool().gameObject.GetComponent<ToolMovement>().MoveTo(direction, tilesToPass);
            tileToMove.SetCurrentStepingGameTool(currentTile.GetCurrentStepingGameTool());
            currentTile.SetCurrentStepingGameTool(null);

        }

    }
    
    
    private Dictionary<int ,KeyValuePair<GameTool, List<Tile>>> GetAllPosibileActions()
    {
        Dictionary<int, KeyValuePair<GameTool, List<Tile>>> AllPosibileActions = new Dictionary<int, KeyValuePair<GameTool, List<Tile>>>();
        int count = 1;
        GameTool tool;
        bool HasTilesLimit = true;
        List<Tile> tiles = new List<Tile>();
        foreach(Tile tile in board.GetAllTiles())
        {
            if (tile.GetCurrentStepingGameTool() != null)
            {
                if (tile.GetCurrentStepingGameTool().transform.parent.tag == "Flag")
                {
                    continue;
                }
                else if (tile.GetCurrentStepingGameTool().GetToolsPlayerId() != 2)
                {
                    continue;
                }
                else
                {
                    if (tile.GetCurrentStepingGameTool().transform.parent.tag == "Plane")
                    {
                        HasTilesLimit = false;
                    }
                    else
                    {
                        HasTilesLimit = true;
                    }
                    foreach (Direction direction in directions)
                    {
                            Tile temp = board.GetNeighbour(tile, direction);

                            if (temp != null)
                            {

                                do
                                {
                                    if (temp.GetType() == tile.GetType() || (tile.GetCurrentStepingGameTool().GetToolType() == Type.Both))
                                    {
                                        if (temp.GetCurrentStepingGameTool() == null)
                                        {
                                               tiles.Add(temp);
                                            
                                        }
                                        else
                                        {
                                            if (temp.GetCurrentStepingGameTool().GetToolsPlayerId() != tile.GetCurrentStepingGameTool().GetToolsPlayerId())
                                            {
                                                tiles.Add(temp);
                                                break;
                                            }
                                            else if (tile.GetCurrentStepingGameTool().GetComponentInParent<Loading>().CanBeLoadTo(temp.GetCurrentStepingGameTool()))
                                            {
                                                tiles.Add(temp);
                                                break;
                                            }
                                            else
                                            {
                                                break;
                                            }
                                        }

                                    }
                                    else if (temp.GetCurrentStepingGameTool() != null)
                                    {
                                        if (temp.GetCurrentStepingGameTool().GetToolsPlayerId() == tile.GetCurrentStepingGameTool().GetToolsPlayerId() && tile.GetCurrentStepingGameTool().GetComponentInParent<Loading>().CanBeLoadTo(temp.GetCurrentStepingGameTool()))
                                    {
                                        tiles.Add(temp);
                                        break;
                                    }
                                        
                                    }



                                    if (temp.GetRowNum() == 20 && direction == Direction.Right || temp.GetRowNum() == 1 && direction == Direction.Up || temp.GetRowNum() == 20 && direction == Direction.Left)
                                    {
                                        break;
                                    }


                                    temp = board.GetNeighbour(temp, direction);


                                } while (temp != null && !HasTilesLimit);




                            }






                     }
                    if(tiles.Count > 0)
                    {
                        AllPosibileActions.Add(count,new KeyValuePair<GameTool, List<Tile>>(tile.GetCurrentStepingGameTool(), new List<Tile>(tiles)));
                        count++;
                        tiles.Clear();
                    }


                }

            }
        }




        return AllPosibileActions;
    }













    private class State
    {
        Node[,] boardState;
        public class Tool
        {
            int id;
            int rank;
            Game.Type type;
            List<Tool> Loaded;
            bool CanBeLoad;
            bool isLoaded;
            Tool Loader;
            int LoadCapability;

            public Tool(GameTool gameTool,bool isLoaded,Tool Loader)
            {
                this.id = gameTool.GetToolsPlayerId();
                
                this.rank = gameTool.GetRank();
                this.isLoaded = isLoaded;
                this.Loader = Loader;
                
                this.type = gameTool.GetToolType();
                if (gameTool.GetComponentInParent<Loading>() != null)
                {
                    this.CanBeLoad = gameTool.GetComponentInParent<Loading>().CanBeLoaded();
                    this.LoadCapability = gameTool.GetComponentInParent<Loading>().GetLoadCapability();
                    this.Loaded = new List<Tool>();
                    foreach (GameTool gametool in gameTool.GetComponentInParent<Loading>().GetLoadedToolsList())
                    {
                        Tool tool = new Tool(gametool,true,this);
                        this.Loaded.Add(tool);
                    }
                }
            }
            public int GetID()
            {
                return id;
            }
            public int GetRank()
            {
                return rank;
            }
            public Game.Type GetToolType()
            {
                return type;
            }
            public bool IsLoaded()
            {
                return isLoaded;
            }
            
            public bool CanBeLoaded()
            {
                return CanBeLoad;
            }
            public List<Tool> GetLoadedTools()
            {
                return Loaded;
            }

        }
        public class Node
        {
            /*
             *Represent the tool in this tile.
             *If its Negative the tool is a Player's tool and the ABS of this tool is the rank of the tool.
             *If its Poitive the tool is a Computer's tool and the number is the rank of the tool.
             *If its Zero there is no tool in this tile.
             *If the tile's type is Land - the tool can be soldier or plane.
             *If the tile's type is Sea - the tool can be ship or plane.
             * The Player's flag represent as int.MinValue and the Computer's flag as int.MaxValue;
             */
            Tool tool;
            Game.Type type;
            string fieldType;

            public Node(GameTool tool, Game.Type type, string fieldType)
            {
                this.tool = new Tool(tool,false,null);
                this.type = type;
                this.fieldType = fieldType;
            }

            public void SetTool(Tool tool)
            {
                this.tool = tool;
            }
            public Tool GetTool()
            {
                return tool;
            }
            public Game.Type GetType()
            {
                return type;
            }
            public string GetFieldType()
            {
                return fieldType;
            }



        }
        public enum Turn
        {
            Player1 , Player2
        }


        public State(Board board)
        {
            
            boardState = new Node[21, 21];
            int row, tile, rank;
            string fieldType;
            Game.Type type;
            foreach (KeyValuePair<Tile, Dictionary<Game.Direction, Tile>> pair in board.GetBoard())
            {
                row = pair.Key.GetRowNum();
                tile = pair.Key.GetTileNum();
               
                fieldType = pair.Key.GetFieldType();
                type = pair.Key.GetType();
                boardState[row, tile] = new Node(pair.Key.GetCurrentStepingGameTool(), type, fieldType);
            }

        }
        public State(State state)
        {
            boardState = new Node[21, 21];
        }

        public void ExcuteAction(Action action)
        {
            if(boardState[action.GetNewRow(),action.GetNewTile()].GetTool() == null)
            {
                SetToolInIndex(action.GetNewRow(), action.GetNewTile(), boardState[action.GetRow(), action.GetTile()].GetTool());
                SetToolInIndex(action.GetRow(), action.GetTile(), null);
            }
            else
            {
                if(boardState[action.GetNewRow(), action.GetNewTile()].GetTool().GetID() != boardState[action.GetRow(), action.GetTile()].GetTool().GetID())
                {
                    if(boardState[action.GetNewRow(), action.GetNewTile()].GetTool().GetRank() == boardState[action.GetRow(), action.GetTile()].GetTool().GetRank())
                    {
                        SetToolInIndex(action.GetNewRow(), action.GetNewTile(),null);
                        SetToolInIndex(action.GetRow(), action.GetTile(), null);
                    }
                    else
                    {
                        if(boardState[action.GetNewRow(), action.GetNewTile()].GetTool().GetRank() > boardState[action.GetRow(), action.GetTile()].GetTool().GetRank())
                        {
                            SetToolInIndex(action.GetNewRow(), action.GetNewTile(), boardState[action.GetRow(), action.GetTile()].GetTool());
                            SetToolInIndex(action.GetRow(), action.GetTile(), null);
                        }
                        else
                        {
                            
                            SetToolInIndex(action.GetRow(), action.GetTile(), null);
                        }
                    }

                    
                }
                else
                {
                    
                }
            }
        }

        public Tool GetToolInIndex(int row, int tile)
        {
            return boardState[row, tile].GetTool();
        }
        public void SetToolInIndex(int row, int tile, Tool tool)
        {
            boardState[row, tile].SetTool(tool);
        }
    }

    public class Action
    {
      
        Direction direction;
        int Row, Tile;
        int newRow, newTile;

        public Action(int Row, int Tile, Direction direction, int numOfTiles)
        {
            this.direction = direction;
            this.Row = Row;
            this.Tile = Tile;

            if (direction == Direction.Right)
            {
                newRow = Row + numOfTiles;
                newTile = Tile;
            }
            else if (direction == Direction.Left)
            {
                newRow = Row - numOfTiles;
                newTile = Tile;
            }
            else if (direction == Direction.Up)
            {
                newRow = Row;
                newTile = Tile + numOfTiles;
            }
            else if (direction == Direction.Down)
            {
                newRow = Row;
                newTile = Tile - numOfTiles;
            }


        }

        public int GetRow()
        {
            return Row;
        }
        public int GetTile()
        {
            return Tile;
        }
        public int GetNewRow()
        {
            return newRow;
        }
        public int GetNewTile()
        {
            return newTile;
        }

      /*  public static List<Action> GetAllPosibileActions(State state)
        {
            List<Action> AllPosibileActions = new List<Action>();

            for (int i = 1; i < 21; i++)
            {
                for(int j = 1; j < 21; j++)
                {
                    int tool;
                }
            }





            return AllPosibileActions;
        }*/




    }
}