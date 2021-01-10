using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolMovement : MonoBehaviour
{
   
    GameManager game;
    Animator anim;
    [SerializeField]
    Game.Direction prevoius;
    Game.Direction current;
    readonly static  float tileSize = 5.0f;
    Vector3 PosToWalk;
    bool DefaultDirectionSeted = false;
    [SerializeField]
    float speed;
    int tilesToPass;
   

    // Start is called before the first frame update
    void Start()
    {

        game = GameObject.FindGameObjectWithTag("Game").GetComponent<GameManager>();
        tilesToPass = 1;
        anim = GetComponent<Animator>();
        current = Game.Direction.None;
        Vector3 PosToWalk = Vector3.zero;

    }

    // Update is called once per frame
    void Update()
    {
        Movement();
    }
    public void SetDefaultDirection(int i)
    {
        if (!DefaultDirectionSeted)
        {
            if(i == 1)
            {
                prevoius = Game.Direction.Up;
            }
            else if(i == 2)
            {
                prevoius = Game.Direction.Down;
            }
        }
    }

    private void Movement()
    {
        if (current == Game.Direction.Right)
        {
            TurnRight();
        }
        else if (current == Game.Direction.Left)
        {
            TurnLeft();
        }
        else if (current == Game.Direction.Up)
        {
            TurnUp();
        }
        else if (current == Game.Direction.Down)
        {
            TurnDown();
        }
    }

    private void TurnRight()

    {

        if (transform.parent.transform.position.x < PosToWalk.x)
        {

            Debug.Log("Turning Right");
            Debug.Log("" + PosToWalk + "   " + transform.parent.position);
            transform.parent.Translate(Vector3.forward * Time.deltaTime * speed);

        }
        else
        {
            if(anim != null)
            {
                anim.SetFloat("Walk", 0.0f);
            }
            PosToWalk = Vector3.zero;
            current = Game.Direction.None;
            game.GetClickedTile().SetCurrentStepingGameTool(null);
            game.GetTileToWalk().SetCurrentStepingGameTool(GetComponent<GameTool>());
            game.PassTurn();
        }
    }
    private void TurnLeft()
    {
        if (transform.parent.transform.position.x > PosToWalk.x)
        {

            

            transform.parent.Translate(Vector3.forward * Time.deltaTime * speed);
        }
        else
        {
            if (anim != null)
            {
                anim.SetFloat("Walk", 0.0f);
            }
            PosToWalk = Vector3.zero;
            current = Game.Direction.None;
            game.GetClickedTile().SetCurrentStepingGameTool(null);
            game.GetTileToWalk().SetCurrentStepingGameTool(GetComponent<GameTool>());
            game.PassTurn();
        }
    }
    private void TurnUp()
    {
        if (transform.parent.transform.position.z < PosToWalk.z)
        {



            transform.parent.Translate(Vector3.forward * Time.deltaTime * speed);

        }
        else
        {
            if (anim != null)
            {
                anim.SetFloat("Walk", 0.0f);
            }
            PosToWalk = Vector3.zero;
            current = Game.Direction.None;
            game.GetClickedTile().SetCurrentStepingGameTool(null);
            game.GetTileToWalk().SetCurrentStepingGameTool(GetComponent<GameTool>());
            game.PassTurn();
        }
    }
    private void TurnDown()
    {
        if (transform.parent.transform.position.z > PosToWalk.z)
        {

            Debug.Log("" + transform.parent.transform.position.x + " " + PosToWalk.x);

            transform.parent.Translate(Vector3.forward * Time.deltaTime * speed);

        }
        else
        {
            if (anim != null)
            {
                anim.SetFloat("Walk", 0.0f);
            }
            PosToWalk = Vector3.zero;
            current = Game.Direction.None;
            game.GetClickedTile().SetCurrentStepingGameTool(null);
            game.GetTileToWalk().SetCurrentStepingGameTool(GetComponent<GameTool>());
            game.PassTurn();
        }
    }

    public void MoveTo(Game.Direction direction , int tilesToPass)
    {
        this.tilesToPass = tilesToPass;

        if (direction == Game.Direction.Right)
        {
            PosToWalk = transform.parent.transform.position + tileSize * tilesToPass * Vector3.right;
            current = Game.Direction.Right;
            if (prevoius == Game.Direction.Down)
            {
                this.transform.parent.transform.Rotate(0, -90, 0);
                prevoius = Game.Direction.Right;


            }
            else if (prevoius == Game.Direction.Left)
            {
                this.transform.parent.transform.Rotate(0, 180, 0);
                prevoius = Game.Direction.Right;


            }
            else if (prevoius == Game.Direction.Up)
            {
                this.transform.parent.transform.Rotate(0, 90, 0);
                prevoius = Game.Direction.Right;
            }
            if (anim != null)
            {
                anim.SetFloat("Walk", 1.0f);
            }
        }
        else if (direction == Game.Direction.Left)
        {
            PosToWalk = transform.parent.transform.position + tileSize * tilesToPass * Vector3.left;
            current = Game.Direction.Left;
            if (prevoius == Game.Direction.Down)
            {
                this.transform.parent.transform.Rotate(0, 90, 0);
                prevoius = Game.Direction.Left;

            }
            else if (prevoius == Game.Direction.Up)
            {
                this.transform.parent.transform.Rotate(0, -90, 0);
                prevoius = Game.Direction.Left;

            }
            else if (prevoius == Game.Direction.Right)
            {
                this.transform.parent.transform.Rotate(0, 180, 0);
                prevoius = Game.Direction.Left;

            }
            if (anim != null)
            {
                anim.SetFloat("Walk", 1.0f);
            }
        }
        else if (direction == Game.Direction.Up)
        {
            PosToWalk = transform.parent.transform.position + tileSize * tilesToPass * Vector3.forward;
            current = Game.Direction.Up;
            if (prevoius == Game.Direction.Down)
            {

                this.transform.parent.transform.Rotate(0, 180, 0);
                prevoius = Game.Direction.Up;
                ;



            }
            else if (prevoius == Game.Direction.Left)
            {
                this.transform.parent.transform.Rotate(0, 90, 0);
                prevoius = Game.Direction.Up;


            }
            else if (prevoius == Game.Direction.Right)
            {
                this.transform.parent.transform.Rotate(0, -90, 0);
                prevoius = Game.Direction.Up;

            }
            if (anim != null)
            {
                anim.SetFloat("Walk", 1.0f);
            }
        }
        else if (direction == Game.Direction.Down)
        {

            PosToWalk = transform.parent.transform.position + tileSize * tilesToPass * Vector3.back;
            current = Game.Direction.Down;
            if (prevoius == Game.Direction.Up)
            {
                this.transform.parent.transform.Rotate(0, 180, 0);
                prevoius = Game.Direction.Down;


            }
            else if (prevoius == Game.Direction.Left)
            {
                this.transform.parent.transform.Rotate(0, -90, 0);
                prevoius = Game.Direction.Down;


            }
            else if (prevoius == Game.Direction.Right)
            {
                this.transform.parent.transform.Rotate(0, 90, 0);
                prevoius = Game.Direction.Down;


            }
            if (anim != null)
            {
                anim.SetFloat("Walk", 1.0f);
            }
        }
        else
        {
            Debug.Log("Invalid direction");
        }
    }
}

