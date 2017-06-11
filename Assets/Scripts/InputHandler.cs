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
        private Direction _goStartDir;
        
        //Different commands needed
        private Command _cmdMoveForward, _cmdMoveBackward, _cmdMoveLeft, _cmdMoveRight, _cmdReplay, _cmdJump, _cmdInteract;
        //Key bindings map
        private Dictionary<KeyCode, Command> _movementKeyBinds;
        private Dictionary<KeyCode, Command> _othersKeyBinds;
        
        //interactive object in front (if any) 
        private Collider _interactiveObject;
        public Collider InteractiveObject
        {
            get { return _interactiveObject; }
        }

        //command keys
        public KeyCode ForwardKey;
        public KeyCode BackwardKey;
        public KeyCode LeftKey;
        public KeyCode RightKey;
        public KeyCode JumpKey;
        public KeyCode InteractKey;

        //non-command keys
        public KeyCode RecordKey;
        public KeyCode ReplayKey;

        public bool IsMoving = false;
        public Direction CurrDirection = Direction.Forward;

        public float InputDelay;
        private float _timePassed;

        public Animator myAnimator;

        private bool _isRecording;
        public bool IsRecording
        {
            get { return _isRecording; }
        }

        public int RecordLimit;

        private List<GameObject> _clones = new List<GameObject>();


        private int status;

        [UsedImplicitly]
        private void Start()
        {
            Toolbox = Toolbox.Instance;
            status = Toolbox.Status;
            BeginningTime = Utils.GetTimeinMilliseconds();
            

            //Initiate commands
            _cmdMoveForward = new MoveForward();
            _cmdMoveBackward = new MoveBackward();
            _cmdMoveLeft = new MoveLeft();
            _cmdMoveRight = new MoveRight();
            //_cmdReplay = new Replay();
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
                    { InteractKey, _cmdInteract}
                };
            }

            _goStartPos = gameObject.transform.position;
            _goStartDir = CurrDirection;


            myAnimator = GetComponent<Animator>();
        }

        [UsedImplicitly]
        private void Update()
        {
            if (!_replayOnly && !IsMoving)
            {
                _timePassed += Time.deltaTime;
                HandleNonMovementInput();
                HandleNonCommandInput();
                CheckInteractiveObject();
            }
        }

        [UsedImplicitly]
        private void FixedUpdate()
        {
            CheckRotations();

            if (!_replayOnly && !IsMoving)
                HandleMovementInput();
        }

        //Check if a key bound to movement commands is pressed, if so execute respective command
        private void HandleMovementInput()
        {
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

        //Check if a key bound to other commands is pressed, if so execute respective command
        private void HandleNonMovementInput()
        {

            foreach (var entry in _othersKeyBinds)
            {
                if (Input.GetKeyDown(entry.Key))
                {
                   // myAnimator.SetFloat("speed", -1.0f);
                    var newCommand = (Command)Activator.CreateInstance(entry.Value.GetType());
                    newCommand.InputHandler = this;
                    newCommand.Execute(gameObject, newCommand);
                }
                else if (entry.Key == InteractKey && Input.GetKeyUp(entry.Key))
                {
                   // myAnimator.SetFloat("speed", 0.0f);
                    var newCommand = (Command)Activator.CreateInstance(typeof(DeInteract));
                    newCommand.InputHandler = this;
                    newCommand.Execute(gameObject, newCommand);
                }
            }
        }

        //Check if a key not bound to commands is pressed
        private void HandleNonCommandInput()
        {
            if (Input.GetKeyDown(RecordKey))
            {
                _isRecording = !_isRecording;
                if (_isRecording)
                {
                    Commands = new List<Command>();
                    BeginningTime = Utils.GetTimeinMilliseconds();
                    _goStartPos = gameObject.transform.position;
                    _goStartDir = CurrDirection;
                    StartReplay();
                }
            }
                
            if (!_isRecording && Input.GetKeyDown(ReplayKey))
                StartReplay();
        }

        //Check if there's any interactive object at next tile to interact
        private void CheckInteractiveObject()
        {
            RaycastHit[] interactives = Physics.RaycastAll(gameObject.transform.position + Vector3.up * 0.1f, gameObject.transform.rotation * Vector3.forward, 1, LayerMask.GetMask("Interactive"));
            if (interactives.Length > 0)
            {
                if (interactives[0].collider != _interactiveObject)
                {
                    ClearInteractiveObject();

                    _interactiveObject = interactives[0].collider;
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

        private void StartReplay()
        {
            if (Commands.Count != 0)
            {
                GameObject clone = Instantiate(gameObject);
                if (_clones.Count == RecordLimit)
                {
                    GameObject oldClone = _clones[0];
                    _clones.RemoveAt(0);
                    Destroy(oldClone);
                }
                _clones.Add(clone);
                clone.layer = LayerMask.NameToLayer("ClonesGO");
                InputHandler cloneIH = clone.GetComponent<InputHandler>();
                cloneIH._replayOnly = true;
                cloneIH.CurrDirection = _goStartDir;
                CommandReplayer replayer = clone.AddComponent<CommandReplayer>();
                replayer.Setup(Commands, _goStartPos, _goStartDir);

                Commands = new List<Command>();
                BeginningTime = Utils.GetTimeinMilliseconds();
                _goStartPos = gameObject.transform.position;
                _goStartDir = CurrDirection;
            }

            foreach (var clone in _clones)
            {
                clone.GetComponent<CommandReplayer>().Start();
            }
        }

        private void CheckRotations()
        {
            if(status != Toolbox.Status)
            {
                status = Toolbox.Status;
                int statusRemain = Toolbox.Status % 4;
                _movementKeyBinds.Clear();
                switch (statusRemain)
                {
                    case 0: //w front
                        _movementKeyBinds = new Dictionary<KeyCode, Command>
                {
                    { LeftKey, _cmdMoveLeft},
                    { RightKey, _cmdMoveRight},
                    { BackwardKey, _cmdMoveBackward},
                    { ForwardKey, _cmdMoveForward},
                    { JumpKey, _cmdJump}
                };
                        break;
                    case 1: //a front
                        _movementKeyBinds = new Dictionary<KeyCode, Command>
                {
                    { LeftKey, _cmdMoveBackward},
                    { RightKey, _cmdMoveForward},
                    { BackwardKey, _cmdMoveRight},
                    { ForwardKey, _cmdMoveLeft},
                    { JumpKey, _cmdJump}
                };
                        
                        break;
                    case 2: //s front
                        _movementKeyBinds = new Dictionary<KeyCode, Command>
                {
                    { LeftKey, _cmdMoveRight},
                    { RightKey, _cmdMoveLeft},
                    { BackwardKey, _cmdMoveForward},
                    { ForwardKey, _cmdMoveBackward},
                    { JumpKey, _cmdJump}
                };
                        break;
                    case 3: //d front
                        _movementKeyBinds = new Dictionary<KeyCode, Command>
                {
                    { LeftKey, _cmdMoveForward},
                    { RightKey, _cmdMoveBackward},
                    { BackwardKey, _cmdMoveLeft},
                    { ForwardKey, _cmdMoveRight},
                    { JumpKey, _cmdJump}
                };
                        break;
                }
            }
        }
    }
}
