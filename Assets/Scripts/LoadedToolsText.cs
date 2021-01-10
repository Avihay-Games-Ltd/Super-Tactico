using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadedToolsText : MonoBehaviour
{

    
    GameManager game;
    TMPro.TMP_Text Text;
    Color regColor , currentColor ,onMouseColor;
    Vector3 Size;
    GameTool Loaded , Loader;
    void Start()
    {
        
        game = GameObject.FindWithTag("Game").GetComponent<GameManager>();
        Text = GetComponent<TMPro.TMP_Text>();
        regColor = Text.color;
        onMouseColor = Color.green;
        Size = new Vector3(this.transform.localScale.x * 0.07f, this.transform.localScale.y * 0.07f, this.transform.localScale.z * 0.07f);
    }

    
    
    private void OnMouseDown()
    {
        if (Loaded != null && Loaded.GetToolsPlayerId() == game.GetTurnPlayerID())
        {


            if (Loader.GetComponentInParent<Loading>().GetReadyToUnloadTool() == null)
            {
                Loader.GetComponentInParent<Loading>().SetReadyToUnloadTool(Loader, Loaded);

            }
            else
            {
                if (Loader == game.GetLoader())
                {
                    if (Loaded.GetComponentInParent<Loading>().CanBeLoadTo(Loader.GetComponentInParent<Loading>().GetReadyToUnloadTool()))
                    {
                        Loader.GetComponentInParent<Loading>().LoadToolOnReadyToUnloadTool(Loaded);
                        game.SetClickedLoadedTool(Loader, Loader.GetComponentInParent<Loading>().GetReadyToUnloadTool());
                    }
                }
                else
                {
                    Loader.GetComponentInParent<Loading>().SetReadyToUnloadTool(Loader, Loaded);
                }
            }
        }
    }
    private void OnMouseOver()
    {
       if(Loaded !=null && Loaded.GetToolsPlayerId() == game.GetTurnPlayerID())
        {
            Text.color = onMouseColor;
        }
        
    }
    private void OnMouseExit()
    {
        if (Loaded != null && Loaded.GetToolsPlayerId() == game.GetTurnPlayerID())
        {
            Text.color = regColor;
        }
        
    }
    public void SetLoader(GameTool Loader)
    {
        this.Loader = Loader;
    }
    public void SetLoaded(GameTool Loaded)
    {
        this.Loaded = Loaded;
    }
}
