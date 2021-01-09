using UnityEngine;

public class GameTool : MonoBehaviour
{
   

    [SerializeField]
    int playerID;
    [SerializeField]
    int rank;
    [SerializeField]
    Game.Type type;
    [SerializeField]
    int ToolID;
    Vector3 resizeFactor;
    bool IDseted = false;
    



    // Start is called before the first frame update
    private void Start()
    {
        resizeFactor = new Vector3(this.transform.localScale.x * 0.07f, this.transform.localScale.y * 0.07f, this.transform.localScale.z * 0.07f);
    }

    public int GetToolsPlayerId()
    {
        
            return playerID;
        
    }
    public void SetToolsPlayerId(int playerID)
    {

        if (!IDseted)
        {
            this.playerID = playerID;
            if (gameObject.GetComponentInParent<ToolMovement>() != null)
            { 
                gameObject.GetComponentInParent<ToolMovement>().SetDefaultDirection(this.playerID);
            }
        }
    }
    
    public void SetToolID(int ToolID)
    {
        this.ToolID = ToolID;
    }
    public int GetToolID()
    {
        return ToolID;
    }
    
    public Game.Type GetToolType()
    {
        return type; 
    }
    public int GetRank()
    {
        return rank;
    }
    public Vector3 GetResizeFactor()
    {
        return resizeFactor;
    }



}


 
   


