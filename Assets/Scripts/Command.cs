using System;
using UnityEngine;

namespace CommandPattern
{
    //The parent class
    public abstract class Command
    {
        public long TimeStamp;

        protected static readonly Toolbox Toolbox = Toolbox.Instance;
        protected static InputHandler InputHandler = Toolbox.InputHandler;
        protected float Speed = Toolbox.Speed;

        //Move and maybe save command
        public abstract void Execute(GameObject go, Command command);
    }

    public class MoveForward : Command
    {
        //Called when we press a key
        public override void Execute(GameObject go, Command command)
        {
            if (!InputHandler.IsReplaying)
            {
                TimeStamp = Utils.GetTimeinMilliseconds() - InputHandler.BeginningTime;
                //Save the command
                InputHandler.OldCommands.Add(command);
            }

            //Move the box
            Move(go);
        }

        //Move the box
        public void Move(GameObject go)
        {
            go.GetComponent<Rigidbody>().AddForce(new Vector3(0,0,Speed), ForceMode.Impulse);
        }
    }

    public class MoveBackward : Command
    {
        //Called when we press a key
        public override void Execute(GameObject go, Command command)
        {
            if (!InputHandler.IsReplaying)
            {
                TimeStamp = Utils.GetTimeinMilliseconds() - InputHandler.BeginningTime;
                //Save the command
                InputHandler.OldCommands.Add(command);
            }

            //Move the box
            Move(go);
        }

        //Move the box
        public void Move(GameObject go)
        {
            go.GetComponent<Rigidbody>().AddForce(new Vector3(0, 0, -Speed), ForceMode.Impulse);
        }
    }

    public class MoveLeft : Command
    {
        //Called when we press a key
        public override void Execute(GameObject go, Command command)
        {
            if (!InputHandler.IsReplaying)
            {
                TimeStamp = Utils.GetTimeinMilliseconds() - InputHandler.BeginningTime;
                //Save the command
                InputHandler.OldCommands.Add(command);
            }

            //Move the box
            Move(go);
        }

        //Move the box
        public void Move(GameObject go)
        {
            go.GetComponent<Rigidbody>().AddForce(new Vector3(-Speed, 0, 0), ForceMode.Impulse);
        }
    }

    public class MoveRight : Command
    {
        //Called when we press a key
        public override void Execute(GameObject go, Command command)
        {
            if (!InputHandler.IsReplaying)
            {
                TimeStamp = Utils.GetTimeinMilliseconds() - InputHandler.BeginningTime;
                //Save the command
                InputHandler.OldCommands.Add(command);
            }

            //Move the box
            Move(go);
        }

        //Move the box
        public void Move(GameObject go)
        {
            go.GetComponent<Rigidbody>().AddForce(new Vector3(Speed, 0, 0), ForceMode.Impulse);
        }
    }

    public class Jump : Command
    {

        //Called when we press a key
        public override void Execute(GameObject go, Command command)
        {
            if (!InputHandler.IsReplaying)
            {
                TimeStamp = Utils.GetTimeinMilliseconds() - InputHandler.BeginningTime;
                //Save the command
                InputHandler.OldCommands.Add(command);
            }

            //Move the box
            Move(go);
        }

        //Move the box
        public void Move(GameObject go)
        {
            Jumpable jumpable = go.GetComponent<Jumpable>();
            if (jumpable.IsGrounded)
            {
                Debug.Log("jumping!");
                go.GetComponent<Rigidbody>().AddForce(new Vector3(0, jumpable.Force, 0), ForceMode.Impulse);
                jumpable.IsGrounded = false;
            }
        }
    }

    //Replay all commands
    public class Replay : Command
    {
        public override void Execute(GameObject go, Command command)
        {
            InputHandler.StartReplay();
        }
    }

}