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

        //Move and maybe save command
        public virtual void Execute(GameObject go, Command command)
        {
            if (!InputHandler.ReplayOnly)
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
                go.transform.position = Vector3.Lerp(StartPosition, EndPosition, t);
                yield return null;
            }
            InputHandler.IsMoving = false;
            yield return 0;
        }

        protected RaycastHit? CheckTile(Vector3 position)
        {
            RaycastHit[] hits = Physics.RaycastAll(position, Vector3.up, 2, LayerMask.GetMask("Ground", "Interactive"));
            if (hits.Length > 0)
            {
                bool foundInteractive = false;
                foreach (var hit in hits)
                {
                    if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Interactive"))
                        foundInteractive = true;
                }

                if (!foundInteractive) return hits[0];
            }

            return null;
        }
    }

    public class MoveForward : MoveCommand
    {
        public override void Execute(GameObject go, Command command)
        {
            base.Execute(go,command);

            if (InputHandler.CurrDirection == Direction.Forward)
            {
                RaycastHit? hit = CheckTile(go.transform.position + new Vector3(0, -1, 1));
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
                RaycastHit? hit = CheckTile(go.transform.position + new Vector3(0, -1, -1));
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
                RaycastHit? hit = CheckTile(go.transform.position + new Vector3(-1, -1, 0));
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
                RaycastHit? hit = CheckTile(go.transform.position + new Vector3(1, -1, 0));
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
        public override void Execute(GameObject go, Command command)
        {
            base.Execute(go, command);

            Move(go);
        }

        public void Move(GameObject go)
        {
            Jumpable jumpable = go.GetComponent<Jumpable>();
            if (jumpable.IsGrounded)
            {
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
            base.Execute(go, command);

            //interact with closest object
            float radius = Toolbox.InteractDistance;
            Collider[] interactives = Physics.OverlapSphere(go.transform.position, radius, LayerMask.GetMask("Interactive"));
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
            
            base.Execute(go, command);

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