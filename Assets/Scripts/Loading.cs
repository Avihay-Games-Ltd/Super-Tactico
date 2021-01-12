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
        Debug.Log(gameTool.transform.position);
        if (gameTool.GetName() != "Flag")
        {

            loadedGameTools.Add(gameTool);

            if (loadFrom != null)
            {
                loadFrom.SetCurrentStepingGameTool(null);
            }
            Debug.Log(gameTool.transform.position);
            foreach (Transform child in gameTool.gameObject.transform.parent)
            { 

            child.gameObject.transform.localScale = Vector3.zero;

            }
            //gameTool.gameObject.transform.parent.position = transform.parent.position;
            //gameTool.gameObject.transform.parent.parent = transform.parent;
            gameTool.GetComponentInParent<Loading>().SetLoader(GetComponentInParent<GameTool>());
         Debug.Log(gameTool.transform.position);
            game.PassTurn();
        }
        else
        {
            loadedGameTools.Add(gameTool);
            if (loadFrom != null)
            {
                loadFrom.SetCurrentStepingGameTool(null);
            }

            foreach (Transform child in gameTool.gameObject.transform.parent)
            {

                child.gameObject.transform.localScale = Vector3.zero;

            }
            //gameTool.gameObject.transform.parent.position = transform.parent.position;
            //gameTool.gameObject.transform.parent.parent = transform.parent;
            HasFlag = true;
            
        }
        Debug.Log(gameTool.transform.position);
    }

     public void UnLoad(GameTool gameTool , Tile unLoadTo = null)
     {
        if (gameTool.GetName().Contains("Boat"))
        {
            foreach (Transform child in gameTool.gameObject.transform.parent)
            {
                child.gameObject.transform.localScale = Vector3.one * 1.5f;
            }
            gameTool.gameObject.transform.localScale = Vector3.one * 0.5f;
        }
        else
        {
            foreach (Transform child in gameTool.gameObject.transform.parent)
            {
                child.gameObject.transform.localScale = Vector3.one * 1.5f;
            }
            gameTool.gameObject.transform.localScale = Vector3.one * 1.4f;
        }
        //gameTool.gameObject.transform.parent.parent = null;
        loadedGameTools.Remove(gameTool);
        if (unLoadTo != null)
        {
            gameTool.gameObject.transform.parent.position = unLoadTo.GetPosition();
            unLoadTo.SetCurrentStepingGameTool(gameTool);
        }

        

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
