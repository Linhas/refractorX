using System;
using System.Collections;
using UnityEngine;

namespace CommandPattern
{
    public enum Direction
    {
        Forward, Right, Backward, Left
    }

    //The parent class
    public abstract class Command
    {
        public long TimeStamp;

        protected static readonly Toolbox Toolbox = Toolbox.Instance;
        public InputHandler InputHandler = null;
        protected float Speed = Toolbox.Speed;

        //Save command if recording
        public virtual void Execute(GameObject go, Command command)
        {
            if (!InputHandler.ReplayOnly && InputHandler.IsRecording)
            {
                TimeStamp = Utils.GetTimeinMilliseconds() - InputHandler.BeginningTime;
                //Save the command
                InputHandler.Commands.Add(command);
            }
        }
    }

    public class MoveCommand : Command
    {
        protected Vector3 StartPosition;
        protected Vector3 EndPosition;

        public override void Execute(GameObject go, Command command)
        {
            base.Execute(go, command);
            StartPosition = go.transform.position;
        }

        public IEnumerator Move(GameObject go, RaycastHit tile)
        {
            InputHandler.IsMoving = true;
            float t = 0;
            while (t < 1f)
            {
                t += Time.deltaTime * Speed;
                InputHandler.myAnimator.SetFloat("speed", Speed);
                go.transform.position = Vector3.Lerp(StartPosition, EndPosition, t);
                yield return null;
            }
            InputHandler.IsMoving = false;
            InputHandler.myAnimator.SetFloat("speed", 0.0f);
            yield return 0;
        }

    }

    public class MoveForward : MoveCommand
    {
        public override void Execute(GameObject go, Command command)
        {
            base.Execute(go,command);

            if (InputHandler.CurrDirection == Direction.Forward)
            {
                RaycastHit? hit = Utils.CheckTile(go.transform.position + new Vector3(0, -2, 1), 3);
                if (hit != null)
                {
                    EndPosition = hit.Value.transform.position + Vector3.up * go.transform.position.y;
                    new Task(Move(go, hit.Value));
                }
            }
            else
            {
                go.GetComponent<Rigidbody>().MoveRotation(Quaternion.Euler(0, 0, 0));
                InputHandler.CurrDirection = Direction.Forward;
            }
        }
    }

    public class MoveBackward : MoveCommand
    {
        public override void Execute(GameObject go, Command command)
        {
            base.Execute(go, command);

            if (InputHandler.CurrDirection == Direction.Backward)
            {
                RaycastHit? hit = Utils.CheckTile(go.transform.position + new Vector3(0, -2, -1), 3);
                if (hit != null)
                {
                    EndPosition = hit.Value.transform.position + Vector3.up * go.transform.position.y;
                    new Task(Move(go, hit.Value));
                }
            }
            else
            {
                go.GetComponent<Rigidbody>().MoveRotation(Quaternion.Euler(0, 180, 0));
                InputHandler.CurrDirection = Direction.Backward;
            }
        }
    }

    public class MoveLeft : MoveCommand
    {
        public override void Execute(GameObject go, Command command)
        {
            base.Execute(go, command);

            if (InputHandler.CurrDirection == Direction.Left)
            {
                RaycastHit? hit = Utils.CheckTile(go.transform.position + new Vector3(-1, -2, 0), 3);
                if (hit != null)
                {
                    EndPosition = hit.Value.transform.position + Vector3.up * go.transform.position.y;
                    new Task(Move(go, hit.Value));
                }
            }
            else
            {
                go.GetComponent<Rigidbody>().MoveRotation(Quaternion.Euler(0, -90, 0));
                InputHandler.CurrDirection = Direction.Left;
            }
        }
    }

    public class MoveRight : MoveCommand
    {
        public override void Execute(GameObject go, Command command)
        {
            base.Execute(go, command);

            if (InputHandler.CurrDirection == Direction.Right)
            {
                RaycastHit? hit = Utils.CheckTile(go.transform.position + new Vector3(1, -2, 0), 3);
                if (hit != null)
                {
                    EndPosition = hit.Value.transform.position + Vector3.up * go.transform.position.y;
                    new Task(Move(go, hit.Value));
                }
            }
            else
            {
                go.GetComponent<Rigidbody>().MoveRotation(Quaternion.Euler(0, 90, 0));
                InputHandler.CurrDirection = Direction.Right;
            }
        }
    }

    public class Jump : Command
    {

        protected Vector3 StartPosition;
        protected Vector3 EndPosition;

        public override void Execute(GameObject go, Command command)
        {
            base.Execute(go, command);

            StartPosition = go.transform.position;

            Jumpable jumpable = go.GetComponent<Jumpable>();

            if (jumpable.IsGrounded)
            {
                InputHandler.myAnimator.SetFloat("speed", -1.0f);
                jumpable.IsGrounded = false;

                RaycastHit[] climbables = Physics.RaycastAll(go.transform.position, go.transform.rotation * Vector3.forward, 1, LayerMask.GetMask("Climbable"));
                if (climbables.Length > 0)
                {
                    EndPosition = climbables[0].transform.position + Vector3.up * 1.3f;
                    new Task(Move(go, climbables[0]));
                }
            }

        }

        public IEnumerator Move(GameObject go, RaycastHit tile)
        {
            InputHandler.IsMoving = true;
            float t = 0;
            while (t < 1f)
            {
                t += Time.deltaTime * Speed;
                go.transform.position = Vector3.Lerp(StartPosition, EndPosition, t);
                yield return null;
            }
            InputHandler.IsMoving = false;
            yield return 0;
        }
    }

    //Interact with objects
    public class Interact : Command
    {
        public override void Execute(GameObject go, Command command)
        {
            base.Execute(go, command);
            
            RaycastHit[] interactives = Physics.RaycastAll(go.transform.position + Vector3.up * 0.1f, go.transform.rotation * Vector3.forward, 1, LayerMask.GetMask("Interactive"));
            if (interactives.Length > 0)
            {
                interactives[0].collider.GetComponent<Interactive.Interactive>().Interact(go);
            }
        }
    }

    public class DeInteract : Command
    {
        public override void Execute(GameObject go, Command command)
        {
            
            base.Execute(go, command);

            RaycastHit[] interactives = Physics.RaycastAll(go.transform.position, go.transform.rotation * Vector3.forward, 1, LayerMask.GetMask("Interactive"));
            if (interactives.Length > 0)
            {
                var interactive = interactives[0].collider.GetComponent<Interactive.Interactive>();
                if (interactive.IsInteracting)
                    interactive.GetComponent<Interactive.Interactive>().DeInteract(go);
            }
        }
    }

}