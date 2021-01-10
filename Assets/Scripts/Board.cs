using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class Board
{

    static Board board;
    [SerializeField]
    Dictionary<Tile, Dictionary<Game.Direction, Tile>> Tiles =  new Dictionary<Tile, Dictionary<Game.Direction, Tile>>();
    [SerializeField]
    List<Tile> NeutralTiles;
    [SerializeField]
    List<Tile> Player1Land;
    [SerializeField]
    List<Tile> Player1Sea;
    [SerializeField]
    List<Tile> Player1Island;
    [SerializeField]
    List<Tile> Player2Land;
    [SerializeField]
    List<Tile> Player2Sea;
    [SerializeField]
    List<Tile> Player2Island;
    [SerializeField]
    List<Tile> AllTiles;
    // Start is called before the first frame update

   
    public static Board GetBoardInstance()
    {
        if(board == null)
        {
            board = new Board();
            return board;
        }

        return board;
    }
    public static void ResetBoard()
    {
        board = null;
    }
   


    
    private Board()
    {
        NeutralTiles = new List<Tile>();
        Player1Land = new List<Tile>();
        Player1Sea = new List<Tile>();
        Player1Island = new List<Tile>();
        Player2Land = new List<Tile>();
        Player2Sea = new List<Tile>();
        Player2Island = new List<Tile>();
        AllTiles = new List<Tile>();

            GameObject[] Tiles = GameObject.FindGameObjectsWithTag("Tile");
            Tile[] tiles = new Tile[Tiles.Length];
            for (int i = 0; i < Tiles.Length; i++)
            {
                tiles[i] = Tiles[i].GetComponent<Tile>();
                tiles[i].TileSetup();
                AllTiles.Add(tiles[i]);
                if (tiles[i].GetFieldType().Contains("Neutral"))
                {
                    NeutralTiles.Add(tiles[i]);
                }
                else if (tiles[i].GetFieldType().Contains("Player_A"))
                {
                    if (tiles[i].GetFieldType().Contains("Sea"))
                    {
                        Player1Sea.Add(tiles[i]);
                    }
                    else if (tiles[i].GetFieldType().Contains("Land"))
                    {
                        Player1Land.Add(tiles[i]);
                    }
                    else Player1Island.Add(tiles[i]);
                }
                else 
                {
                    if (tiles[i].GetFieldType().Contains("Sea"))
                    {
                        Player2Sea.Add(tiles[i]);
                    }
                    else if (tiles[i].GetFieldType().Contains("Land"))
                    {
                        Player2Land.Add(tiles[i]);
                    }
                    else Player2Island.Add(tiles[i]);
                }
            }

        SetBoardDirections();

           
        
    }
    

    private void SetBoardDirections()
    {
        Tiles = new Dictionary<Tile, Dictionary<Game.Direction, Tile>>();
        foreach (Tile t in AllTiles)
        {
            Dictionary<Game.Direction, Tile> sides = new Dictionary<Game.Direction, Tile>();
            if (t.GetTileNum() > 1)
            {

                foreach (Tile ti in AllTiles)
                {
                    if (ti.GetRowNum() == t.GetRowNum() && ti.GetTileNum() == t.GetTileNum() - 1)
                    {
                        sides.Add(Game.Direction.Down, ti);
                        break;
                    }
                }

            }

            if (t.GetTileNum() < 20)
            {

                foreach (Tile ti in AllTiles)
                {
                    if (ti.GetRowNum() == t.GetRowNum() && ti.GetTileNum() == t.GetTileNum() + 1)
                    {
                        sides.Add(Game.Direction.Up, ti);
                        break;
                    }
                }

            }
            if (t.GetRowNum() > 1)
            {

                foreach (Tile ti in AllTiles)
                {
                    if (ti.GetRowNum() == t.GetRowNum() - 1 && ti.GetTileNum() == t.GetTileNum())
                    {
                        sides.Add(Game.Direction.Left, ti);
                        break;
                    }
                }

            }

            if (t.GetRowNum() < 20)
            {

                foreach (Tile ti in AllTiles)
                {
                    if (ti.GetRowNum() == t.GetRowNum() + 1 && ti.GetTileNum() == t.GetTileNum())
                    {
                        sides.Add(Game.Direction.Right, ti);
                        break;
                    }
                }

            }
            if(t.GetRowNum() == 20)
            {
                sides.Add(Game.Direction.Right, null);
            }
            if (t.GetRowNum() == 1)
            {
                sides.Add(Game.Direction.Left, null);
            }
            if (t.GetTileNum() == 20)
            {
                sides.Add(Game.Direction.Up, null);
            }
            if (t.GetTileNum() == 1)
            {
                sides.Add(Game.Direction.Down, null);
            }
            Tiles.Add(t, sides);

        }
    }
    public Dictionary<Tile, Dictionary<Game.Direction, Tile>> GetBoard()
    {
        return Tiles;
    }

    public List<Tile> GetNeutralTiles()
    {
        return NeutralTiles;
    }
    public List<Tile> GetPlayer1Land()
    {
        return Player1Land;
    }
    public List<Tile> GetPlayer2Land()
    {
        return Player2Land;
    }
    public List<Tile> GetPlayer1Sea()
    {
        return Player1Sea;
    }
    public List<Tile> GetPlayer2Sea()
    {
        return Player2Sea;
    }
    public List<Tile> GetPlayer1Island()
    {
        return Player1Island;
    }
    public List<Tile> GetPlayer2Island()
    {
        return Player2Island;
    }
    public List<Tile> GetAllTiles()
    {
        return AllTiles;
    }
    public Tile GetNeighbour(Tile tile , Game.Direction direction)
    {
        foreach(KeyValuePair<Game.Direction,Tile> pair in Tiles[tile])
        {
            if(pair.Key == direction)
            {
                return pair.Value;
            }
        }

        return null;
    }
    
    public bool IslandFullForSetup(int player)
    {
        int count = 0;
        if(player == 1)
        {
            foreach(Tile tile in Player1Island)
            {
                if (tile.GetCurrentStepingGameTool() != null) count++;

                if (count == 3) return true;
            }
            return false;
        }
        else
        {
            foreach (Tile tile in Player2Island)
            {
                if (tile.GetCurrentStepingGameTool() != null) count++;

                if (count == 3) return true;
            }
            return false;
        }
    }

    
  

}
