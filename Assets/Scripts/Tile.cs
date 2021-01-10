using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


public class Tile : MonoBehaviour
{
    private bool isSetuped = false;
    [SerializeField]
    private int rownum;
    [SerializeField]
    private int tilenum;
    [SerializeField]
    private string fieldType;
    [SerializeField]
    private Game.Type type;
    GameTool currentStepingGameTool;
    [SerializeField]
    GameObject GameManager;
    GameManager game;
    Material regMaterial;
    Material transparent1;
    Material transparent2;
    [SerializeField]
    bool canBeClicked;
    [SerializeField]
    bool canWalkTo;
    private bool gameToolResized;

   
    // Start is called before the first frame update
   

    /*[MenuItem("Tools/AssignGameManagerToTiles")]
    public static void AssignGameManagerToTiles()
    {
        GameObject GameManager = GameObject.FindGameObjectWithTag("Game");
        GameObject[] Tiles = GameObject.FindGameObjectsWithTag("Tile");
        foreach(GameObject Tile in Tiles)
        {
            Tile tile = Tile.GetComponent<Tile>();
            tile.GameManager = GameManager;
        }
    }*/
    void Start()
    {
        
        game = GameManager.GetComponent<GameManager>();
        regMaterial = GetComponent<MeshRenderer>().materials[0];
        transparent1 = (Material)Resources.Load("Materials\\GlassSolidStripe", typeof(Material));
        transparent2 = (Material)Resources.Load("Materials\\GlassSolidStripe Black", typeof(Material));
        canWalkTo = false;
        gameToolResized = false;

}

    // Update is called once per frame
    void Update()
    {
        
    }



    public void setCanBeClicked(bool canBeClicked)
    {
        this.canBeClicked = canBeClicked;
    }
    public GameTool GetCurrentStepingGameTool()
    {
        return currentStepingGameTool;
    }
    public void SetCurrentStepingGameTool(GameTool currentStepingGameTool)
    {
        this.currentStepingGameTool = currentStepingGameTool;
    }


    private void OnMouseDown()
    {
        if (canWalkTo)
        {
            if (currentStepingGameTool != null)
            {
                game.SetToolUI(currentStepingGameTool, currentStepingGameTool.GetComponentInParent<Loading>(), false);
            }
            game.SetToolUI(game.GetClickedTile().currentStepingGameTool, game.GetClickedTile().currentStepingGameTool.GetComponentInParent<Loading>(), false);
            game.MoveTo(GetComponent<Tile>());

        }
        else if (!canWalkTo && canBeClicked && game.GetClickedTile() != this)
        {
            if (gameToolResized)
            {

                currentStepingGameTool.gameObject.transform.localScale -= currentStepingGameTool.GetResizeFactor();
                gameToolResized = false;
            }

            game.SetClickedTile(GetComponent<Tile>());
            game.SetToolUI(currentStepingGameTool, currentStepingGameTool.GetComponentInParent<Loading>(),true);

        }
        else if (!canWalkTo && canBeClicked && game.GetClickedTile() == this)
        {
            game.ResetClickedTile();
            game.SetToolUI(currentStepingGameTool, currentStepingGameTool.GetComponentInParent<Loading>(), false);
        }
        

    }
    private void OnMouseOver()
    {
        
        

        if (canWalkTo)
        {
            Material[] transparent = { regMaterial, transparent2 };
            GetComponent<MeshRenderer>().materials = transparent;
            
        }
        else if(canBeClicked)
        {
            Material[] transparent = { regMaterial, transparent2 };
            GetComponent<MeshRenderer>().materials = transparent;
            if (currentStepingGameTool != null && !gameToolResized)
            {
                currentStepingGameTool.gameObject.transform.localScale += currentStepingGameTool.GetResizeFactor();
                gameToolResized = true;
            }
        }

        if(game.GetClickedTile() == null&& currentStepingGameTool != null || currentStepingGameTool != null && currentStepingGameTool.GetToolsPlayerId() != game.GetTurnPlayerID())
        {
            game.SetToolUI(currentStepingGameTool, currentStepingGameTool.GetComponentInParent<Loading>(),true);
        }

    }
    private void OnMouseExit()
    {

        if(game.GetClickedTile() == null && currentStepingGameTool != null || currentStepingGameTool != null && currentStepingGameTool.GetToolsPlayerId() != game.GetTurnPlayerID())
        {
            game.SetToolUI(currentStepingGameTool, currentStepingGameTool.GetComponentInParent<Loading>(), false);
        }
        if (canWalkTo)
        {
            Material[] transparent = { regMaterial, transparent1 };
            GetComponent<MeshRenderer>().materials = transparent;
           
        }
        else if (canBeClicked)
        {
            Material[] transparent = { regMaterial };
            GetComponent<MeshRenderer>().materials = transparent;
            if (currentStepingGameTool != null && gameToolResized)
            {
                currentStepingGameTool.gameObject.transform.localScale -= currentStepingGameTool.GetResizeFactor();
                gameToolResized = false;
            }
        }
    }
   
    public bool GetCanWalkTo()
    {
        return canWalkTo;
    }
    public void UnSetCanWalkTo()
    {
        canWalkTo = false;
        Material[] reg = { regMaterial };
        GetComponent<MeshRenderer>().materials = reg;
    }


    public void SetCanWalkTo()
    {
           this.canWalkTo = true;
           Material[] transparent = { regMaterial, transparent1 };
           GetComponent<MeshRenderer>().materials = transparent;
    }
    public int GetTileNum()
    {
        return tilenum;
    }
    public int GetRowNum()
    {
        return rownum;
    }
    public string GetFieldType()
    {
        return fieldType;
    }
    
    public Game.Type GetType()
    {
        return type;
    }
    public Vector3 GetPosition()
    {
        return this.gameObject.transform.position;
    }
    public void SetupReset()
    {
        isSetuped = false;
    }
    public void TileSetup()
    {
        if (!isSetuped)
        {
            isSetuped = true;
            rownum = int.Parse(transform.parent.name);
            tilenum = int.Parse(gameObject.name);
            currentStepingGameTool = null;
            MeshRenderer m = gameObject.GetComponentInParent<MeshRenderer>();
            Material M = m.sharedMaterial;


            int rn = rownum, tn = tilenum;
            if (M.mainTexture.name == "Vol_19_4_Base_Color")
            {
                type = Game.Type.Land;
                Debug.Log(rn + " " + tn);
                if (tn > 9 && tn < 12)
                {
                    fieldType = "Neutral_Land";

                }
                else
                {
                    if (tn >= 11)
                    {
                        if (rn <= 5)
                        {

                            fieldType = "Player_B_Island";


                        }
                        else
                        {
                            fieldType = "Player_B_Land";
                        }
                    }
                    else
                    {
                        if (rn <= 5)
                        {

                            fieldType = "Player_A_Island";

                        }
                        else
                        {
                            fieldType = "Player_A_Land";
                        }
                    }
                }
            }
            else
            {
                type = Game.Type.Sea;
                if (tn > 9 && tn < 12)
                {
                    fieldType = "Neutral_Sea";
                }
                else
                {
                    if (tn >= 11)
                    {


                        fieldType = "Player_B_Sea";


                    }
                    else
                    {


                        fieldType = "Player_A_Sea";

                    }
                }

            }
        }
    }

 }
