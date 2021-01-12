using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameAI : Game
{
     List<Direction> directions = new List<Direction>();
    Dictionary<int, KeyValuePair<GameTool, List<Tile>>> AllPosibileActions;
    List<Tile> WalkedTiles = new List<Tile>();




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
            foreach(Tile tile in WalkedTiles)
            {
                tile.UnSetWalkedTile();
            }
            WalkedTiles.Clear();
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
        List<State.Action> Actions = Minimax();
        State.Action Action = Actions[Random.Range(0, Actions.Count)];
        Debug.Log(" ( " + Action.GetNewRow() + " , " + Action.GetNewTile() + ")  ( " + Action.GetNewRow() + " , " + Action.GetNewTile() + " )");
        Direction direction = Direction.None;
        Tile currentTile = FindTile(Action.GetRow(),Action.GetTile());
        Tile tileToMove = FindTile(Action.GetNewRow(), Action.GetNewTile());
        ClickedTile = currentTile;
        Loader = ClickedTile.GetCurrentStepingGameTool();
        TileToWalk = tileToMove;
        if (Action.GetLoadedTool() != null)
        {
            ClickedLoadedTool = Loader.GetComponentInParent<Loading>().GetLoadedToolsList()[Action.GetLoadedToolIndex()];
        }
        else
        {
            Loader = null;
            ClickedLoadedTool = null;
        }
        ClickedTile.SetWalkedTile();
        TileToWalk.SetWalkedTile();
        WalkedTiles.Add(ClickedTile);
        WalkedTiles.Add(TileToWalk);

        if (ClickedLoadedTool == null)
        {
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
                    tileToMove.GetCurrentStepingGameTool().GetComponentInParent<Loading>().Load(currentTile.GetCurrentStepingGameTool(), currentTile);
                }

            }
            else
            {





                currentTile.GetCurrentStepingGameTool().gameObject.GetComponent<ToolMovement>().MoveTo(direction, tilesToPass);
                tileToMove.SetCurrentStepingGameTool(currentTile.GetCurrentStepingGameTool());
                currentTile.SetCurrentStepingGameTool(null);

            }
        }

        else
        {








            if (tileToMove.GetCurrentStepingGameTool() == null)
            {
                DeactivateTileMovementOptions();
                ResetCanBeClickedTiles();
                Loader.GetComponentInParent<Loading>().UnLoad(ClickedLoadedTool, tileToMove);
                PassTurn();
            }
            else if (tileToMove.GetCurrentStepingGameTool().GetToolsPlayerId() == ClickedLoadedTool.GetToolsPlayerId())
            {
                DeactivateTileMovementOptions();
                ResetCanBeClickedTiles();
                Loader.GetComponentInParent<Loading>().UnLoad(ClickedLoadedTool);
                tileToMove.GetCurrentStepingGameTool().GetComponentInParent<Loading>().Load(ClickedLoadedTool);
            }
            else if (tileToMove.GetCurrentStepingGameTool().GetToolsPlayerId() != ClickedLoadedTool.GetToolsPlayerId())
            {
                DeactivateTileMovementOptions();
                ResetCanBeClickedTiles();

                if (ClickedLoadedTool.GetRank() == tileToMove.GetCurrentStepingGameTool().GetRank())
                {
                    Loader.GetComponentInParent<Loading>().GetLoadedToolsList().Remove(ClickedLoadedTool);
                    if (ClickedLoadedTool.GetToolsPlayerId() == 1)
                    {
                        Player1Tools.Remove(ClickedLoadedTool);
                        Player2Tools.Remove(tileToMove.GetCurrentStepingGameTool());
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

                        foreach (GameTool tool in tileToMove.GetCurrentStepingGameTool().GetComponentInParent<Loading>().GetLoadedToolsList())
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
                        Player1Tools.Remove(tileToMove.GetCurrentStepingGameTool());


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

                        foreach (GameTool tool in tileToMove.GetCurrentStepingGameTool().GetComponentInParent<Loading>().GetLoadedToolsList())
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
                    GameObject.Destroy(tileToMove.GetCurrentStepingGameTool().gameObject.transform.parent.gameObject);
                    tileToMove.SetCurrentStepingGameTool(null);
                    PassTurn();


                }
                else if (tileToMove.GetCurrentStepingGameTool().GetName() == "Flag")
                {
                    ClickedLoadedTool.GetComponentInParent<Loading>().Load(tileToMove.GetCurrentStepingGameTool(), tileToMove);
                    Loader.GetComponentInParent<Loading>().UnLoad(ClickedLoadedTool, tileToMove);
                    PassTurn();
                }
                else if (ClickedLoadedTool.GetRank() <= tileToMove.GetCurrentStepingGameTool().GetRank())
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
                    PassTurn();


                }

                else if (ClickedLoadedTool.GetRank() >= tileToMove.GetCurrentStepingGameTool().GetRank())
                {
                    Debug.Log("ClickedLoadedTool stronger");
                    if (ClickedLoadedTool.GetToolsPlayerId() == 1)
                    {
                        Player2Tools.Remove(tileToMove.GetCurrentStepingGameTool());
                        foreach (GameTool tool in tileToMove.GetCurrentStepingGameTool().GetComponentInParent<Loading>().GetLoadedToolsList())

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
                        GameObject.Destroy(tileToMove.GetCurrentStepingGameTool().gameObject.transform.parent.gameObject);
                        tileToMove.SetCurrentStepingGameTool(null);
                        Loader.GetComponentInParent<Loading>().UnLoad(ClickedLoadedTool, tileToMove);
                        PassTurn();
                    }

                    else
                    {
                        Player1Tools.Remove(tileToMove.GetCurrentStepingGameTool());

                        foreach (GameTool tool in tileToMove.GetCurrentStepingGameTool().GetComponentInParent<Loading>().GetLoadedToolsList())

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

                        GameObject.Destroy(tileToMove.GetCurrentStepingGameTool().gameObject.transform.parent.gameObject);
                        tileToMove.SetCurrentStepingGameTool(null);
                        Loader.GetComponentInParent<Loading>().UnLoad(ClickedLoadedTool, tileToMove);
                        PassTurn();
                    }
                }
            }







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







    public  List<State.Action> Minimax()
    {
        int depth = 1;
        State state = new State(board);
        int MaxAction = -Mathf.RoundToInt(Mathf.Infinity);
        State.Action Action = null;
        int alpha = -Mathf.RoundToInt(Mathf.Infinity);
        int beta = Mathf.RoundToInt(Mathf.Infinity);
        List<State.Action> BestActions = new List<State.Action>();
        List<State.Action> PosibleActions = state.GetAllPosibileActions(2);
        foreach(State.Action action in PosibleActions)
        {
            int val = Eval(new State(state, action),alpha,beta, depth + 1,1);
            if (val > MaxAction)
            {
                MaxAction = val;
                BestActions.Clear();
                BestActions.Add(action);
            }
            else if(val == MaxAction)
            {
                BestActions.Add(action);
            }
        }

        return BestActions;
    }

    public int Eval(State state, int alpha, int beta, int depth, int PlayerID)
    {
        if (state.isEndState())
        {
            return state.GetendStateValue();
        }

        if (depth == 2)
        {
            return state.GetHeurisitic();
        }



        if (PlayerID == 1)
        {
            int current = beta;
            foreach (State.Action action in state.GetAllPosibileActions(1))
            {
                int val = Eval(new State(state, action), alpha, current, depth + 1, 2);
                current = Mathf.Min(current, val);
                if (current <= alpha)
                {
                    return alpha;
                }

            }
            return current;
        }
        else
        {
            int current = alpha;
            foreach (State.Action action in state.GetAllPosibileActions(2))
            {
                int val = Eval(new State(state, action), current, beta, depth + 1, 1);
                current = Mathf.Max(current, val);
                if (current >= beta)
                {
                    return beta;
                }

            }
            return current;
        }
    }
   /* public int Max(State state, int depth)
    {
        Debug.Log("Min");
        if (state.isEndState())
        {
            return state.GetendStateValue();
        }

        if (depth == 3)
        {
            return state.GetHeurisitic();
        }
        int MaxAction = -Mathf.RoundToInt(Mathf.Infinity);

        foreach(State.Action action in state.GetAllPosibileActions(2))
        {
            int val = Min(new State(state, action), depth + 1);

            if(val > MaxAction)
            {
                MaxAction = val;
            }
        }
        return MaxAction;
    }
    public int Min(State state, int depth)
    {
        Debug.Log("Min");
        if (state.isEndState())
        {
            return state.GetendStateValue();
        }

        if(depth == 3)
        {
            return state.GetHeurisitic();
        }
        int MinAction = Mathf.RoundToInt(Mathf.Infinity);

        foreach (State.Action action in state.GetAllPosibileActions(1))
        {
            int val = Max(new State(state, action), depth + 1);

            if (val < MinAction)
            {
                MinAction = val;
            }
        }

        return MinAction;
    }*/










    public class State
    {

        static List<Direction> directions = new List<Direction>(new Direction[] {Direction.Right, Direction.Left, Direction.Up, Direction.Down});
        Node[,] boardState;
        int heuristic = 0;
        int endStateValue = 0;

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
                            Tool tool = new Tool(gametool, true, this);
                            this.Loaded.Add(tool);
                        }
                    }
                    else
                    {
                        CanBeLoad = true;
                        this.LoadCapability = 0;
                        this.Loaded = new List<Tool>();

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
            public int GetLoadCapability()
            {
                return LoadCapability;
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
            public void LoadTool(Tool tool)
            {
                Loaded.Add(tool);
            }
            public void UnLoadTool(Tool tool)
            {
                Loaded.Remove(tool);
            }
            public bool HasFlag()
            {
                foreach(Tool tool in Loaded)
                {
                    if(tool.GetRank() == 0)
                    {
                        return true;
                    }
                }
                return false;
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
            int row,tile;

            public Node(GameTool tool, Game.Type type, string fieldType , int row ,int tile)
            {
                if (tool != null)
                {
                    this.tool = new Tool(tool, false, null);
                }
                else this.tool = null;
                this.type = type;
                this.fieldType = fieldType;
                this.row = row;
                this.tile = tile;
            }
            public Node(Tool tool, Game.Type type, string fieldType , int row , int tile)
            {
                this.tool = tool;
                this.type = type;
                this.fieldType = fieldType;
                this.row = row;
                this.tile = tile;
            }

            public void SetTool(Tool tool)
            {
                this.tool = tool;
            }
            public int GetNodeRow()
            {
                return row;
            }
            public int GetNodeNum()
            {
                return tile;
            }
            public Tool GetTool()
            {
                return tool;
            }
            public Game.Type GetTileType()
            {
                return type;
            }
            public string GetFieldType()
            {
                return fieldType;
            }



        }





        public class Action
        {

            Direction direction;
            int Row, Tile;
            int newRow, newTile;
            Tool LoadedTool;
            int index;

            public Action(int Row, int Tile, Direction direction, int numOfTiles,Tool LoadedTool,int index = -1)
            {
                this.direction = direction;
                this.Row = Row;
                this.Tile = Tile;
                this.LoadedTool = LoadedTool;
                this.index = index;

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
            public Tool GetLoadedTool()
            {
                return LoadedTool;
            }
            public int GetLoadedToolIndex()
            {
                return index;
            }




        }
        /*  public enum Turn
          {
              Player1 , Player2
          }*/


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
                boardState[row, tile] = new Node(pair.Key.GetCurrentStepingGameTool(), type, fieldType,row,tile);
            }

        }
        public State(State state)
        {
            this.boardState = new Node[21, 21];
            for(int i = 1; i < 21; i++)
            {
                for(int j = 1; j < 21; j++)
                {
                    this.boardState[i, j] = new Node(state.boardState[i, j].GetTool(), state.boardState[i, j].GetTileType(), state.boardState[i, j].GetFieldType(),i,j);
                }
            }


            for (int i = 1; i < 21; i++)
            {

                for (int j = 1; j < 21; j++)
                {
                    Tool tool = boardState[i, j].GetTool();
                    if (tool != null)
                    {


                        if (tool.GetRank() == 1)
                        {
                            heuristic += tool.GetID() == 2 ? 600 : -600;
                        }
                        else if (tool.GetRank() == 2)
                        {
                            heuristic += tool.GetID() == 2 ? 100 : -100;
                        }
                        else if (tool.GetToolType() == Type.Land && tool.GetRank() == 3)
                        {
                            heuristic += tool.GetID() == 2 ? 30 : -30;
                        }
                        else if (tool.GetToolType() == Type.Land && tool.GetRank() == 4)
                        {
                            heuristic += tool.GetID() == 2 ? 40 : -40;
                        }
                        else if (tool.GetToolType() == Type.Land && tool.GetRank() == 5)
                        {
                            heuristic += tool.GetID() == 2 ? 60 : -60;
                        }
                        else if (tool.GetToolType() == Type.Land && tool.GetRank() == 6)
                        {
                            heuristic += tool.GetID() == 2 ? 80 : -80;
                        }
                        else if (tool.GetToolType() == Type.Sea && tool.GetRank() == 3)
                        {
                            heuristic += tool.GetID() == 2 ? 30 : -30;
                        }
                        else if (tool.GetToolType() == Type.Sea && tool.GetRank() == 4)
                        {
                            heuristic += tool.GetID() == 2 ? 80 : -80;
                        }
                        else if (tool.GetToolType() == Type.Sea && tool.GetRank() == 5)
                        {
                            heuristic += tool.GetID() == 2 ? 120 : -120;
                        }
                        else if (tool.GetToolType() == Type.Sea && tool.GetRank() == 6)
                        {
                            heuristic += tool.GetID() == 2 ? 180 : -180;
                        }
                        else if (tool.GetRank() == 7)
                        {
                            heuristic += tool.GetID() == 2 ? 200 : -200;
                        }
                        else if (tool.GetRank() == 8)
                        {
                            heuristic += tool.GetID() == 2 ? 300 : -300;
                        }
                        else if (tool.GetRank() == 9)
                        {
                            heuristic += tool.GetID() == 2 ? 400 : -400;
                        }
                        else if (tool.GetRank() == 10)
                        {
                            heuristic += tool.GetID() == 2 ? 600 : -600;
                        }




                        if (tool.HasFlag())
                        {
                            heuristic += tool.GetID() == 2 ? 1500 : -1500;
                        }
                    }
                }

            }
        }
        public State(State state, Action action)
        {
            this.boardState = new Node[21, 21];
            for (int i = 1; i < 21; i++)
            {
                string str = "";
                for (int j = 1; j < 21; j++)
                {
                    this.boardState[i, j] = new Node(state.boardState[i, j].GetTool(), state.boardState[i, j].GetTileType(), state.boardState[i, j].GetFieldType(),i,j);
                    if(boardState[i, j].GetTool() == null)
                    {
                        str += "null ";
                    }
                    else str += boardState[i, j].GetTool() + " ";
                }
                Debug.Log(str);
            }

            this.ExcuteAction(action);

            for (int i = 1; i < 21; i++)
            {
                string str = "";
                for (int j = 1; j < 21; j++)
                {
                    if (boardState[i, j].GetTool() == null)
                    {
                        str += "null ";
                    }
                    else str += boardState[i, j].GetTool() + " ";
                }
                Debug.Log(str);
            }
            for (int i = 1; i < 21; i++)
            {

                for (int j = 1; j < 21; j++)
                {
                    Tool tool = boardState[i,j].GetTool();
                    if (tool != null)
                    {


                        if (tool.GetRank() == 1)
                        {
                            heuristic += tool.GetID() == 2 ? 70 : -70;
                        }
                        else if (tool.GetRank() == 2)
                        {
                            heuristic += tool.GetID() == 2 ? 100 : -100;
                        }
                        else if (tool.GetToolType() == Type.Land && tool.GetRank() == 3)
                        {
                            heuristic += tool.GetID() == 2 ? 30 : -30;
                        }
                        else if (tool.GetToolType() == Type.Land && tool.GetRank() == 4)
                        {
                            heuristic += tool.GetID() == 2 ? 40 : -40;
                        }
                        else if (tool.GetToolType() == Type.Land && tool.GetRank() == 5)
                        {
                            heuristic += tool.GetID() == 2 ? 60 : -60;
                        }
                        else if (tool.GetToolType() == Type.Land && tool.GetRank() == 6)
                        {
                            heuristic += tool.GetID() == 2 ? 80 : -80;
                        }
                        else if (tool.GetToolType() == Type.Sea && tool.GetRank() == 3)
                        {
                            heuristic += tool.GetID() == 2 ? 30 : -30;
                        }
                        else if (tool.GetToolType() == Type.Sea && tool.GetRank() == 4)
                        {
                            heuristic += tool.GetID() == 2 ? 80 : -80;
                        }
                        else if (tool.GetToolType() == Type.Sea && tool.GetRank() == 5)
                        {
                            heuristic += tool.GetID() == 2 ? 120 : -120;
                        }
                        else if (tool.GetToolType() == Type.Sea && tool.GetRank() == 6)
                        {
                            heuristic += tool.GetID() == 2 ? 180 : -180;
                        }
                        else if (tool.GetRank() == 7)
                        {
                            heuristic += tool.GetID() == 2 ? 200 : -200;
                        }
                        else if (tool.GetRank() == 8)
                        {
                            heuristic += tool.GetID() == 2 ? 300 : -300;
                        }
                        else if (tool.GetRank() == 9)
                        {
                            heuristic += tool.GetID() == 2 ? 400 : -400;
                        }
                        else if (tool.GetRank() == 10)
                        {
                            heuristic += tool.GetID() == 2 ? 600 : -600;
                        }




                        if (tool.HasFlag())
                        {
                            heuristic += tool.GetID() == 2 ? 1500 : -1500;
                        }
                    }
                }

            }
        }


        public void ExcuteAction(Action action)
        {
            if (action.GetLoadedTool() == null)
            {
                if (boardState[action.GetNewRow(), action.GetNewTile()].GetTool() == null)
                {
                    SetToolInIndex(action.GetNewRow(), action.GetNewTile(), boardState[action.GetRow(), action.GetTile()].GetTool());
                    SetToolInIndex(action.GetRow(), action.GetTile(), null);
                }
                else
                {
                    if (boardState[action.GetNewRow(), action.GetNewTile()].GetTool().GetID() != boardState[action.GetRow(), action.GetTile()].GetTool().GetID())
                    {
                        if (boardState[action.GetNewRow(), action.GetNewTile()].GetTool().GetRank() == 0)
                        {
                            boardState[action.GetRow(), action.GetTile()].GetTool().LoadTool(boardState[action.GetNewRow(), action.GetNewTile()].GetTool());
                            SetToolInIndex(action.GetNewRow(), action.GetNewTile(), boardState[action.GetRow(), action.GetTile()].GetTool());
                            SetToolInIndex(action.GetRow(), action.GetTile(), null);
                        }
                        else if (boardState[action.GetNewRow(), action.GetNewTile()].GetTool().GetRank() == boardState[action.GetRow(), action.GetTile()].GetTool().GetRank())
                        {
                            SetToolInIndex(action.GetNewRow(), action.GetNewTile(), null);
                            SetToolInIndex(action.GetRow(), action.GetTile(), null);
                        }
                        else
                        {
                            if (boardState[action.GetNewRow(), action.GetNewTile()].GetTool().GetRank() < boardState[action.GetRow(), action.GetTile()].GetTool().GetRank())
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
                        boardState[action.GetNewRow(), action.GetNewTile()].GetTool().LoadTool(boardState[action.GetRow(), action.GetTile()].GetTool());
                        SetToolInIndex(action.GetRow(), action.GetTile(), null);

                    }
                }
            }

            



            else
            {
                if (boardState[action.GetNewRow(), action.GetNewTile()].GetTool() == null)
                {
                    SetToolInIndex(action.GetNewRow(), action.GetNewTile(), action.GetLoadedTool());
                    boardState[action.GetRow(), action.GetTile()].GetTool().UnLoadTool(action.GetLoadedTool());
                }
                else
                {
                    if (boardState[action.GetNewRow(), action.GetNewTile()].GetTool().GetID() != action.GetLoadedTool().GetID())
                    {
                        if (boardState[action.GetNewRow(), action.GetNewTile()].GetTool().GetRank() == 0)
                        {
                            action.GetLoadedTool().LoadTool(boardState[action.GetNewRow(), action.GetNewTile()].GetTool());
                            SetToolInIndex(action.GetNewRow(), action.GetNewTile(), action.GetLoadedTool());
                            boardState[action.GetRow(), action.GetTile()].GetTool().UnLoadTool(action.GetLoadedTool());
                        }
                        else if (boardState[action.GetNewRow(), action.GetNewTile()].GetTool().GetRank() == action.GetLoadedTool().GetRank())
                        {
                            SetToolInIndex(action.GetNewRow(), action.GetNewTile(), null);
                            boardState[action.GetRow(), action.GetTile()].GetTool().UnLoadTool(action.GetLoadedTool());
                        }
                        else
                        {
                            if (boardState[action.GetNewRow(), action.GetNewTile()].GetTool().GetRank() < action.GetLoadedTool().GetRank())
                            {
                                SetToolInIndex(action.GetNewRow(), action.GetNewTile(), action.GetLoadedTool());
                                boardState[action.GetRow(), action.GetTile()].GetTool().UnLoadTool(action.GetLoadedTool());
                            }
                            else
                            {

                                boardState[action.GetRow(), action.GetTile()].GetTool().UnLoadTool(action.GetLoadedTool());
                            }
                        }


                    }
                    else
                    {
                        boardState[action.GetNewRow(), action.GetNewTile()].GetTool().LoadTool(boardState[action.GetRow(), action.GetTile()].GetTool());
                        SetToolInIndex(action.GetRow(), action.GetTile(), null);

                    }
                }






            }



        }


        public List<Action> GetAllPosibileActions(int PlayerTurnID)
        {
            List<Action> AllPosibileActions = new List<Action>();
            bool HasTilesLimit = true;

            for (int i = 1; i < 21; i++)
            {
                for (int j = 1; j < 21; j++)
                {
                    Node node = boardState[i, j];
                    if (node.GetTool() != null)
                    {





                        if (node.GetTool().GetID() != PlayerTurnID || node.GetTool().GetRank() == 0)
                        {
                            continue;
                        }

                        else
                        {
                            if(node.GetTool().GetRank() == 1 || node.GetTool().GetRank() == 2)
                            {
                                HasTilesLimit = false;
                            }
                            else
                            {
                                HasTilesLimit = true;
                            }


                           foreach(Direction direction in directions)
                            {
                                Node toMoveNode = GetNodeInDirection(i, j, direction);
                                if (toMoveNode != null)
                                {
                                    int count = 0;
                                    do
                                    {
                                        count++;
                                        

                                        if(toMoveNode.GetTool() == null)
                                        {
                                            if(toMoveNode.GetTileType() == node.GetTileType() || node.GetTool().GetToolType() == Type.Both)
                                            {
                                                AllPosibileActions.Add(new Action(node.GetNodeRow(), node.GetNodeNum(), direction, count, null));
                                            }
                                            else
                                            {
                                                break;
                                            }
                                        }
                                        else
                                        {

                                            if(toMoveNode.GetTool().GetID() != node.GetTool().GetID())
                                            {

                                                if(toMoveNode.GetTileType() == node.GetTileType())
                                                {
                                                    AllPosibileActions.Add(new Action(node.GetNodeRow(), node.GetNodeNum(), direction, count, null));
                                                    break;
                                                }
                                                else
                                                {
                                                    break;
                                                }


                                            }

                                            else
                                            {

                                                if(node.GetTool().CanBeLoaded() && toMoveNode.GetTool().GetToolType() != Type.Land && toMoveNode.GetTool().GetLoadedTools().Count +1 <= toMoveNode.GetTool().GetLoadCapability())
                                                {
                                                    AllPosibileActions.Add(new Action(node.GetNodeRow(), node.GetNodeNum(), direction, count, null));
                                                    break;
                                                }
                                                else
                                                {
                                                    break;
                                                }



                                            }

                                        }
                                        toMoveNode = GetNodeInDirection(toMoveNode.GetNodeRow(),toMoveNode.GetNodeNum(), direction);
                                    } while (toMoveNode != null && !HasTilesLimit);


                                }
                            }
                           






                        }




                        if(node.GetTool().GetLoadedTools().Count > 0)
                        {

                            foreach(Tool loadedTool in node.GetTool().GetLoadedTools())
                            {

                                foreach(Direction direction in directions)
                                {
                                    Node toMoveNode = GetNodeInDirection(node.GetNodeRow(),node.GetNodeNum(), direction);

                                    if(toMoveNode != null)
                                    {

                                        if(toMoveNode.GetTool() == null)
                                        {
                                            if(toMoveNode.GetTileType() == loadedTool.GetToolType())
                                            {
                                                AllPosibileActions.Add(new Action(node.GetNodeRow(), node.GetNodeNum(), direction, 1, loadedTool, node.GetTool().GetLoadedTools().IndexOf(loadedTool)));
                                            }
                                        }

                                        else
                                        {

                                            if(toMoveNode.GetTool().GetID() == loadedTool.GetID())
                                            {
                                                
                                                if(loadedTool.CanBeLoaded() && toMoveNode.GetTool().GetToolType() != Type.Land && toMoveNode.GetTool().GetLoadedTools().Count+1 <= toMoveNode.GetTool().GetLoadCapability())
                                                {
                                                    AllPosibileActions.Add(new Action(node.GetNodeRow(), node.GetNodeNum(), direction, 1, loadedTool, node.GetTool().GetLoadedTools().IndexOf(loadedTool)));
                                                }

                                            }


                                            else
                                            {

                                                if(toMoveNode.GetTileType() == loadedTool.GetToolType())
                                                {
                                                    AllPosibileActions.Add(new Action(node.GetNodeRow(), node.GetNodeNum(), direction, 1, loadedTool, node.GetTool().GetLoadedTools().IndexOf(loadedTool)));
                                                }




                                            }

                                        }

                                    }
                                        
                                }


                            }


                        }


                    }
                }
            }
            

        
            return AllPosibileActions;

        }





        public Node[,] GetStateNodes()
        {
            return boardState;
        }
        public Tool GetToolInIndex(int row, int tile)
        {
            return boardState[row, tile].GetTool();
        }
        public void SetToolInIndex(int row, int tile, Tool tool)
        {
            boardState[row, tile].SetTool(tool);
        }
        public Node GetNodeInDirection(int i,int j ,Direction direction)
        {
            if (direction == Direction.Right)
            {
                if (i + 1 <= 20)
                {
                    return boardState[i + 1, j];
                }
                else return null;
            }
            else if (direction == Direction.Left)
            {
                if (i - 1 >= 1)
                {
                    return boardState[i - 1, j];
                }
                else return null;
            }
            else if (direction == Direction.Up)
            {
                if (j + 1 <= 20)
                {
                    return boardState[i , j +1];
                }
                else return null;
            }
            else if (direction == Direction.Down)
            {
                if (j - 1 >= 1)
                {
                    return boardState[i , j -1];
                }
                else return null;
            }

            else return null;
        }

        public bool isEndState()
        {
            int countPlayer1Tools = 0;
            int countPlayer2Tools = 0;
            for(int i=1;i<21;i++)
            {
                for (int j = 1; j < 21; j++)
                {
                    Node node = boardState[i, j];
                    if (node.GetTool() != null)
                    {
                        if (node.GetTool().GetRank() == 0)
                        {
                            continue;
                        }
                        else if (node.GetTool().GetID() == 2)
                        {
                            countPlayer2Tools++;
                            if (node.GetFieldType() == "Player_B_Island" && node.GetTool().HasFlag())
                            {
                                endStateValue = 1000000;
                                return true;
                            }
                        }
                        else if (node.GetTool().GetID() == 1)
                        {
                            countPlayer1Tools++;
                            if (node.GetFieldType() == "Player_A_Island" && node.GetTool().HasFlag())
                            {
                                endStateValue = -1000000;
                                return true;
                            }
                        }


                    }
                }
            }

            if(countPlayer1Tools == 0 || countPlayer2Tools == 0)
            {
                endStateValue = 0;
                return true;
            }
            if (countPlayer1Tools == 0)
            {
                endStateValue = 1000000;
                return true;
            }
            else if (countPlayer2Tools == 0)
            {
                endStateValue = -1000000;
                return true;
            }

            else return false;
        }
        
        

        public int GetendStateValue()
        {
            return endStateValue;
        }
        public int GetHeurisitic()
        {
            return heuristic;
        }



    }

   
}