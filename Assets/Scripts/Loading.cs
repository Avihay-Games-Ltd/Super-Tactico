using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loading : MonoBehaviour
{
    GameManager game;
    [SerializeField]
    private int loadCapability;
    [SerializeField]
    private bool canBeLoaded;
    [SerializeField]
    List<GameTool> loadedGameTools;
    GameTool ReadyToUnloadTool;
    GameTool Loader;
    bool HasFlag;
    float HighestTextY;


    private void Start()
    {

        game = GameObject.FindGameObjectWithTag("Game").GetComponent<GameManager>();
        loadedGameTools = new List<GameTool>();
        ReadyToUnloadTool = null;
        Loader = null;
        HasFlag = false;
        HighestTextY = 6.0f;
        
    }
    public int GetLoadCapability()
    {
        return loadCapability;
    }
    public void ResetReadyToUnloadTool()
    {
        if (ReadyToUnloadTool != null)
        {

            ReadyToUnloadTool = null;
        }
    }
    public void SetReadyToUnloadTool(GameTool Loader, GameTool ReadyToUnloadTool)
    {
        if (ReadyToUnloadTool != null)
        {
            ResetReadyToUnloadTool();
            this.ReadyToUnloadTool = ReadyToUnloadTool;
        }
        game.SetClickedLoadedTool(Loader, ReadyToUnloadTool);
    }
    public List<GameTool> GetLoadedToolsList()
    {
        return loadedGameTools;
    }
    public GameTool GetReadyToUnloadTool()
    {

        return ReadyToUnloadTool;
    }
    
    public GameTool GetLoader()
    {
        return Loader;
    }
    public void SetLoader(GameTool Loader)
    {
        this.Loader = Loader;
    }
    public void LoadToolOnReadyToUnloadTool(GameTool gametool)
    {
        ReadyToUnloadTool.GetComponentInParent<Loading>().Load(gametool);
    }
    public bool HasEnemyFlag()
    {
        return HasFlag;
    }
    public bool CanBeLoaded()
    {
        return canBeLoaded;
    }
    public bool CanBeLoadTo(GameTool gameTool)
    {
        if(gameTool.GetName() == "Flag")
        {
            return false;
        }
        if (!canBeLoaded || gameTool.transform.parent.tag == "Soldier")

        {
            return false;
        }
        else
        {

            if (loadedGameTools.Count + 1 + gameTool.GetComponentInParent<Loading>().loadedGameTools.Count <= gameTool.GetComponentInParent<Loading>().loadCapability)
            {
                return true;
            }
            else
            {
                return false;
            }




        }
    }




     public void ResetLoadedToolsList()
      {
        loadedGameTools.Clear();
      }
   
     public void Load(GameTool gameTool , Tile loadFrom = null)
     {
        if (gameTool.GetName() != "Flag")
        {
            if (gameTool.GetComponentInParent<Loading>().GetLoader() != null)
            {
                gameTool.GetComponentInParent<Loading>().GetLoader().GetComponentInParent<Loading>().UnLoad(gameTool, game.GetClickedTile());
            }
            loadedGameTools.Add(gameTool);

            if (loadFrom != null)
            {
                loadFrom.SetCurrentStepingGameTool(null);
            }
            gameTool.gameObject.transform.parent.localScale = Vector3.zero;
            gameTool.gameObject.transform.parent.position = gameObject.transform.parent.position;
            gameTool.GetComponentInParent<Loading>().SetLoader(GetComponentInParent<GameTool>());
            game.PassTurn();
        }
        else
        {
            loadedGameTools.Add(gameTool);
            if (loadFrom != null)
            {
                loadFrom.SetCurrentStepingGameTool(null);
            }
            gameTool.gameObject.transform.parent.localScale = Vector3.zero;
            gameTool.gameObject.transform.parent.position = gameObject.transform.parent.position;
            HasFlag = true;
            
        }
     }

     public void UnLoad(GameTool gameTool , Tile unLoadTo)
     {
        loadedGameTools.Remove(gameTool);
        
        gameTool.gameObject.transform.parent.position = unLoadTo.GetPosition();
        gameTool.gameObject.transform.parent.localScale = Vector3.one;
        unLoadTo.SetCurrentStepingGameTool(gameTool);
        game.PassTurn();

    }

    private bool OnLoad(string gameToolName)
     {
            foreach (GameTool tool in loadedGameTools)
            {
                if (tool.transform.parent.name == gameToolName)
                {
                    return true;
                }
            }

            return false;
     }

    


}
