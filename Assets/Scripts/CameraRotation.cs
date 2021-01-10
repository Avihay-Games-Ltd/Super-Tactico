using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotation : MonoBehaviour
{
    [SerializeField]
    Canvas GameUI;
    [SerializeField]
    Camera Camera_3D;
    [SerializeField]
    Camera Camera_2D;
    [SerializeField]
    GameObject Tool1;
    [SerializeField]
    GameObject Tool2;
    [SerializeField]
    GameObject PlayerTool1;
    [SerializeField]
    GameObject PlayerTool2;
    GameManager game;
    bool On2DMode;
    private void Start()
    {
        game = GameObject.FindGameObjectWithTag("Game").GetComponent<GameManager>();
        On2DMode = false;
    }
    private void Update()
    {
        Tool1.gameObject.transform.Rotate(Vector3.up, 20 * Time.deltaTime, Space.Self);
        Tool2.gameObject.transform.Rotate(Vector3.up, 20 * Time.deltaTime, Space.Self);
    }
    public void Rotate90DegreesRight()
    {
        transform.Rotate(Vector3.up, -90, Space.Self);
    }
    public void Rotate90DegreesLeft()
    {
        transform.Rotate(Vector3.up, 90, Space.Self);
    }
    public bool isOn2DMode()
    {
        return On2DMode;
    }
    public void Set2Dview()
    {
        On2DMode = true;
        Camera_2D.enabled = true;
        GameUI.GetComponent<Canvas>().worldCamera = Camera_2D;
        
        Camera_3D.enabled = false;
    }
    public void Set3Dview()
    {
        On2DMode = false;
        Camera_3D.enabled = true;
        GameUI.GetComponent<Canvas>().worldCamera = Camera_3D;
        
        Camera_2D.enabled = false;
    }
    public void SetPlayerTool(GameTool gameTool,Loading gameToolLoading)
    {
        if (gameTool.GetToolsPlayerId() == 1)
        {
            if (Tool1.transform.childCount == 0)
            {
                GameObject tool1 = GameObject.Instantiate(game.GetToolByID(gameTool.GetToolID()), Tool1.transform.position, Quaternion.identity);
                tool1.transform.parent = Tool1.transform;
                tool1.transform.position = Tool1.transform.position;
                tool1.transform.localRotation = Tool1.transform.localRotation;
                tool1.transform.localScale = new Vector3(30f, 30f, 30f);
            }
            PlayerTool1.gameObject.transform.Find("ToolName").gameObject.GetComponent<TMPro.TMP_Text>().text = "Name : "+gameTool.GetName();
            PlayerTool1.gameObject.transform.Find("Rank").gameObject.GetComponent<TMPro.TMP_Text>().text = "Rank : " + gameTool.GetRank();
            PlayerTool1.gameObject.transform.Find("Army").gameObject.GetComponent<TMPro.TMP_Text>().text = "Army : " + gameTool.GetArmy();


            if (gameToolLoading != null)
            {
                PlayerTool1.gameObject.transform.Find("Load").gameObject.GetComponent<TMPro.TMP_Text>().text = "Load Capability : " + gameToolLoading.GetLoadCapability();
                List<GameTool> loaded = gameToolLoading.GetLoadedToolsList();
                for (int i = 1; i <= loaded.Count; i++)
                {
                    PlayerTool1.gameObject.transform.Find("LoadedTool" + i).gameObject.GetComponent<TMPro.TMP_Text>().text = loaded[i - 1].GetName();
                    PlayerTool1.gameObject.transform.Find("LoadedTool" + i).GetComponent<LoadedToolsText>().SetLoaded(loaded[i - 1]);
                    PlayerTool1.gameObject.transform.Find("LoadedTool" + i).GetComponent<LoadedToolsText>().SetLoader(gameTool);
                }
            }
      

        }
        else if (gameTool.GetToolsPlayerId() == 2)
        {
            if (Tool2.transform.childCount == 0)
            {
                GameObject tool2 = GameObject.Instantiate(game.GetToolByID(gameTool.GetToolID()), Tool2.transform.position, Quaternion.identity);
                tool2.transform.parent = Tool2.transform;
                tool2.transform.position = Tool2.transform.position;
                tool2.transform.localRotation = Tool2.transform.localRotation;
                tool2.transform.localScale = new Vector3(30f, 30f, 30f);
                // tool2.transform.trv
            }
            PlayerTool2.gameObject.transform.Find("ToolName").gameObject.GetComponent<TMPro.TMP_Text>().text = "Name : " + gameTool.GetName();
            PlayerTool2.gameObject.transform.Find("Rank").gameObject.GetComponent<TMPro.TMP_Text>().text = "Rank : " + gameTool.GetRank();
            PlayerTool2.gameObject.transform.Find("Army").gameObject.GetComponent<TMPro.TMP_Text>().text = "Army : " + gameTool.GetArmy();

            if (gameToolLoading != null)
            {
                PlayerTool2.gameObject.transform.Find("Load").gameObject.GetComponent<TMPro.TMP_Text>().text = "Load Capability : " + gameToolLoading.GetLoadCapability();
                List<GameTool> loaded = gameToolLoading.GetLoadedToolsList();
                for (int i = 1; i <= loaded.Count; i++)
                {
                    PlayerTool2.gameObject.transform.Find("LoadedTool" + i).gameObject.GetComponent<TMPro.TMP_Text>().text = loaded[i - 1].GetName();
                    PlayerTool2.gameObject.transform.Find("LoadedTool" + i).GetComponent<LoadedToolsText>().SetLoaded(loaded[i - 1]);
                    PlayerTool2.gameObject.transform.Find("LoadedTool" + i).GetComponent<LoadedToolsText>().SetLoader(gameTool);
                }
            }
     
        }
    }
    public void ResetPlayerTool(int playerID)
    {
        if (playerID == 1)
        {
            foreach(Transform chiled in Tool1.transform)
            {
                Destroy(chiled.gameObject);
            }
            PlayerTool1.gameObject.transform.Find("ToolName").gameObject.GetComponent<TMPro.TMP_Text>().text = "";
            PlayerTool1.gameObject.transform.Find("Rank").gameObject.GetComponent<TMPro.TMP_Text>().text = "";
            PlayerTool1.gameObject.transform.Find("Army").gameObject.GetComponent<TMPro.TMP_Text>().text = "";
            PlayerTool1.gameObject.transform.Find("Load").gameObject.GetComponent<TMPro.TMP_Text>().text = "";
            
            for (int i=1; i <= 6; i++)
            {
                PlayerTool1.gameObject.transform.Find("LoadedTool"+i).gameObject.GetComponent<TMPro.TMP_Text>().text = "";
                PlayerTool1.gameObject.transform.Find("LoadedTool" + i).GetComponent<LoadedToolsText>().SetLoaded(null);
                PlayerTool1.gameObject.transform.Find("LoadedTool" + i).GetComponent<LoadedToolsText>().SetLoader(null);
            }
            /*PlayerTool1.gameObject.transform.Find("LoadedTool1").gameObject.GetComponent<TMPro.TMP_Text>().text = "";
            PlayerTool1.gameObject.transform.Find("LoadedTool2").gameObject.GetComponent<TMPro.TMP_Text>().text = "";
            PlayerTool1.gameObject.transform.Find("LoadedTool3").gameObject.GetComponent<TMPro.TMP_Text>().text = "";
            PlayerTool1.gameObject.transform.Find("LoadedTool4").gameObject.GetComponent<TMPro.TMP_Text>().text = "";
            PlayerTool1.gameObject.transform.Find("LoadedTool5").gameObject.GetComponent<TMPro.TMP_Text>().text = "";
            PlayerTool1.gameObject.transform.Find("LoadedTool6").gameObject.GetComponent<TMPro.TMP_Text>().text = "";*/

        }
        else if(playerID == 2)
        {
            foreach (Transform chiled in Tool2.transform)
            {
                Destroy(chiled.gameObject);
            }
            PlayerTool2.gameObject.transform.Find("ToolName").gameObject.GetComponent<TMPro.TMP_Text>().text = "";
            PlayerTool2.gameObject.transform.Find("Rank").gameObject.GetComponent<TMPro.TMP_Text>().text = "";
            PlayerTool2.gameObject.transform.Find("Load").gameObject.GetComponent<TMPro.TMP_Text>().text = "";
            PlayerTool2.gameObject.transform.Find("Army").gameObject.GetComponent<TMPro.TMP_Text>().text = "";
            for (int i = 1; i <= 6; i++)
            {
                PlayerTool2.gameObject.transform.Find("LoadedTool"+i).gameObject.GetComponent<TMPro.TMP_Text>().text = "";
                PlayerTool2.gameObject.transform.Find("LoadedTool" + i).GetComponent<LoadedToolsText>().SetLoaded(null);
                PlayerTool2.gameObject.transform.Find("LoadedTool" + i).GetComponent<LoadedToolsText>().SetLoader(null);
            }
           /* PlayerTool2.gameObject.transform.Find("ToolName").gameObject.GetComponent<TMPro.TMP_Text>().text = "";
            PlayerTool2.gameObject.transform.Find("Rank").gameObject.GetComponent<TMPro.TMP_Text>().text = "";
            PlayerTool2.gameObject.transform.Find("LoadedTool1").gameObject.GetComponent<TMPro.TMP_Text>().text = "";
            PlayerTool2.gameObject.transform.Find("LoadedTool2").gameObject.GetComponent<TMPro.TMP_Text>().text = "";
            PlayerTool2.gameObject.transform.Find("LoadedTool3").gameObject.GetComponent<TMPro.TMP_Text>().text = "";
            PlayerTool2.gameObject.transform.Find("LoadedTool4").gameObject.GetComponent<TMPro.TMP_Text>().text = "";
            PlayerTool2.gameObject.transform.Find("LoadedTool5").gameObject.GetComponent<TMPro.TMP_Text>().text = "";
            PlayerTool2.gameObject.transform.Find("LoadedTool6").gameObject.GetComponent<TMPro.TMP_Text>().text = "";*/
        }
    }

}
