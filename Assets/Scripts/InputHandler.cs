using System;
using UnityEngine;
using System.Collections.Generic;
using cakeslice;
using JetBrains.Annotations;

namespace CommandPattern
{
    [RequireComponent(typeof(Rigidbody))]
    public class InputHandler : MonoBehaviour
    {
        protected static Toolbox Toolbox;

        private bool _replayOnly;
        public bool ReplayOnly
        {
            get { return _replayOnly; }
        }
        
        //Beginning timestamp
        public long BeginningTime;
        
        //Stores commands for replay
        public List<Command> Commands = new List<Command>();
        
        //GO start position
        private Vector3 _goStartPos;
        //GO start rotation
        private Quaternion _goStartRot;
        
        //Different commands needed
        private Command _cmdMoveForward, _cmdMoveBackward, _cmdMoveLeft, _cmdMoveRight, _cmdReplay, _cmdJump, _cmdInteract;
        //Key bindings map
        private Dictionary<KeyCode, Command> _movementKeyBinds;
        private Dictionary<KeyCode, Command> _othersKeyBinds;
        
        //closest interactive object within range (if any) 
        private Collider _interactiveObject;
        public Collider InteractiveObject
        {
            get { return _interactiveObject; }
        }

        public KeyCode ForwardKey;
        public KeyCode BackwardKey;
        public KeyCode LeftKey;
        public KeyCode RightKey;
        public KeyCode JumpKey;
        public KeyCode ReplayKey;
        public KeyCode InteractKey;

        public bool IsMoving = false;
        public Direction CurrDirection = Direction.Forward;

        public float InputDelay;
        private float _timePassed;

        [UsedImplicitly]
        private void Start()
        {
            Toolbox = Toolbox.Instance;
            BeginningTime = Utils.GetTimeinMilliseconds();

            //Initiate commands
            _cmdMoveForward = new MoveForward();
            _cmdMoveBackward = new MoveBackward();
            _cmdMoveLeft = new MoveLeft();
            _cmdMoveRight = new MoveRight();
            _cmdReplay = new Replay();
            _cmdJump = new Jump();
            _cmdInteract = new Interact();

            if (!_replayOnly)
            {
                //Bind keys with commands
                _movementKeyBinds = new Dictionary<KeyCode, Command>
                {
                    { LeftKey, _cmdMoveLeft},
                    { RightKey, _cmdMoveRight},
                    { BackwardKey, _cmdMoveBackward},
                    { ForwardKey, _cmdMoveForward},
                    { JumpKey, _cmdJump}
                };

                _othersKeyBinds = new Dictionary<KeyCode, Command>
                {
                    { ReplayKey, _cmdReplay},
                    { InteractKey, _cmdInteract}
                };
            }

            _goStartPos = gameObject.transform.position;
            _goStartRot = gameObject.transform.rotation;
        }

        [UsedImplicitly]
        private void Update()
        {
            _timePassed += Time.deltaTime;
            if (!_replayOnly)
                HandleOthersInput();

            CheckInteractiveObject();
        }

        [UsedImplicitly]
        private void FixedUpdate()
        {
            if (!_replayOnly)
                HandleMovementInput();
        }

        //Check if we press a key bound to movements, if so do what the key is binded to 
        private void HandleMovementInput()
        {
            if (IsMoving) return;
            foreach (var entry in _movementKeyBinds)
            {
                if(Input.GetKey(entry.Key) && _timePassed >= InputDelay)
                {
                    var newCommand = (Command) Activator.CreateInstance(entry.Value.GetType());
                    newCommand.InputHandler = this;
                    newCommand.Execute(gameObject, newCommand);
                    _timePassed = 0;
                }
            }
        }

        //Check if we press a key bound to others, if so do what the key is binded to 
        private void HandleOthersInput()
        {
            if (IsMoving) return;
            foreach (var entry in _othersKeyBinds)
            {
                if (Input.GetKeyDown(entry.Key))
                {
                    var newCommand = (Command)Activator.CreateInstance(entry.Value.GetType());
                    newCommand.InputHandler = this;
                    newCommand.Execute(gameObject, newCommand);
                }
                else if (entry.Key == InteractKey && Input.GetKeyUp(entry.Key))
                {
                    var newCommand = (Command)Activator.CreateInstance(typeof(DeInteract));
                    newCommand.InputHandler = this;
                    newCommand.Execute(gameObject, newCommand);
                }
            }
        }

        //Check if there's any interactive object within range to interact
        private void CheckInteractiveObject()
        {
            //interact with closest object
            float radius = Toolbox.InteractDistance;
            Collider[] interactives = Physics.OverlapSphere(gameObject.transform.position, radius, LayerMask.GetMask("Interactive"));
            if (interactives.Length > 0)
            {
                interactives = Utils.DistanceSort(interactives, gameObject.GetComponent<Collider>());
                if (interactives[0] != _interactiveObject)
                {
                    ClearInteractiveObject();

                    _interactiveObject = interactives[0];
                    if (!_replayOnly)
                    {
                        Outline outline = _interactiveObject.gameObject.AddComponent<Outline>();
                        outline.color = 0;
                        outline.eraseRenderer = false;
                    }
                }
            }
            else ClearInteractiveObject();
        }

        private void ClearInteractiveObject()
        {
            if(_interactiveObject != null)
            {
                Interactive.Interactive interactive = _interactiveObject.GetComponent<Interactive.Interactive>();
                if (interactive.IsInteracting)
                {
                    interactive.DeInteract(gameObject);
                }

                Outline outline = _interactiveObject.GetComponent<Outline>();
                if(outline != null && !_replayOnly)
                {
                    Destroy(outline);
                }
                _interactiveObject = null;
            }
        }

        public void StartReplay()
        {
            if (Commands.Count == 0) return;
            GameObject clone = Instantiate(gameObject);
            clone.layer = LayerMask.NameToLayer("ClonesGO");
            InputHandler cloneIH = clone.GetComponent<InputHandler>();
            cloneIH._replayOnly = true;
            CommandReplayer replayer = clone.AddComponent<CommandReplayer>();
            replayer.Setup(Commands, _goStartPos, _goStartRot);

            Commands = new List<Command>();
            BeginningTime = Utils.GetTimeinMilliseconds();
            _goStartPos = gameObject.transform.position;
            _goStartRot = gameObject.transform.rotation;
        }

    }
}
