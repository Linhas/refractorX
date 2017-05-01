using UnityEngine;

namespace CommandPattern
{
    //The parent class
    public abstract class Command
    {
        public long TimeStamp;

        protected static readonly Toolbox Toolbox = Toolbox.Instance;
        public InputHandler InputHandler = null;
        protected float Speed = Toolbox.Speed;

        //Move and maybe save command
        public abstract void Execute(GameObject go, Command command);
    }

    public class MoveForward : Command
    {
        public override void Execute(GameObject go, Command command)
        {
            if (InputHandler!=null)
            {
                TimeStamp = Utils.GetTimeinMilliseconds() - InputHandler.BeginningTime;
                //Save the command
                InputHandler.Commands.Add(command);
            }

            Move(go);
        }

        public void Move(GameObject go)
        {
            go.GetComponent<Rigidbody>().AddForce(new Vector3(0,0,Speed), ForceMode.Impulse);
        }
    }

    public class MoveBackward : Command
    {
        public override void Execute(GameObject go, Command command)
        {
            if (InputHandler != null)
            {
                TimeStamp = Utils.GetTimeinMilliseconds() - InputHandler.BeginningTime;
                //Save the command
                InputHandler.Commands.Add(command);
            }
            
            Move(go);
        }
        
        public void Move(GameObject go)
        {
            go.GetComponent<Rigidbody>().AddForce(new Vector3(0, 0, -Speed), ForceMode.Impulse);
        }
    }

    public class MoveLeft : Command
    {
        public override void Execute(GameObject go, Command command)
        {
            if (InputHandler != null)
            {
                TimeStamp = Utils.GetTimeinMilliseconds() - InputHandler.BeginningTime;
                //Save the command
                InputHandler.Commands.Add(command);
            }
        
            Move(go);
        }

        public void Move(GameObject go)
        {
            go.GetComponent<Rigidbody>().AddForce(new Vector3(-Speed, 0, 0), ForceMode.Impulse);
        }
    }

    public class MoveRight : Command
    {
        public override void Execute(GameObject go, Command command)
        {
            if (InputHandler)
            {
                TimeStamp = Utils.GetTimeinMilliseconds() - InputHandler.BeginningTime;
                //Save the command
                InputHandler.Commands.Add(command);
            }
        
            Move(go);
        }

        public void Move(GameObject go)
        {
            go.GetComponent<Rigidbody>().AddForce(new Vector3(Speed, 0, 0), ForceMode.Impulse);
        }
    }

    public class Jump : Command
    {
        public override void Execute(GameObject go, Command command)
        {
            if (InputHandler != null)
            {
                TimeStamp = Utils.GetTimeinMilliseconds() - InputHandler.BeginningTime;
                //Save the command
                InputHandler.Commands.Add(command);
            }

            Move(go);
        }

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

    //Interact with objects
    public class Interact : Command
    {
        public override void Execute(GameObject go, Command command)
        {
            if (InputHandler != null)
            {
                TimeStamp = Utils.GetTimeinMilliseconds() - InputHandler.BeginningTime;
                //Save the command
                InputHandler.Commands.Add(command);
            }

            //interact with closest object
            Collider[] interactives = Physics.OverlapSphere(go.transform.position, 1f, LayerMask.GetMask("Interactive"));
            if (interactives.Length > 0)
            {
                interactives = Utils.DistanceSort(interactives, go.GetComponent<Collider>());
                interactives[0].GetComponent<Interactive.Interactive>().Interact(go);
            }

        }
    }

    public class DeInteract : Command
    {
        public override void Execute(GameObject go, Command command)
        {
            if (InputHandler != null)
            {
                TimeStamp = Utils.GetTimeinMilliseconds() - InputHandler.BeginningTime;
                //Save the command
                InputHandler.Commands.Add(command);
            }

            //interact with closest object
            Collider[] interactives = Physics.OverlapSphere(go.transform.position, 1f, LayerMask.GetMask("Interactive"));
            if (interactives.Length > 0)
            {
                interactives = Utils.DistanceSort(interactives, go.GetComponent<Collider>());
                var interactive = interactives[0].GetComponent<Interactive.Interactive>();
                if (interactive.IsInteracting)
                    interactive.GetComponent<Interactive.Interactive>().DeInteract(go);
            }
        }
    }

}