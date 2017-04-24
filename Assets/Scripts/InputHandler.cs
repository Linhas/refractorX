using System;
using UnityEngine;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace CommandPattern
{
    [RequireComponent(typeof(Rigidbody))]
    public class InputHandler : MonoBehaviour
    {
        //Beginning timestamp
        public static long BeginningTime;
        //Stores commands for replay
        public List<Command> Commands = new List<Command>();
        //GO start position
        private Vector3 _goStartPos;
        //GO start rotation
        private Quaternion _goStartRot;
        //Different commands needed
        private Command _cmdMoveForward, _cmdMoveBackward, _cmdMoveLeft, _cmdMoveRight, _cmdReplay, _cmdJump;
        //Key bindings map
        private Dictionary<KeyCode, Command> _keyBinds;
        
        [UsedImplicitly]
        private void Start()
        {
            BeginningTime = Utils.GetTimeinMilliseconds();

            //Initiate commands
            _cmdMoveForward = new MoveForward();
            _cmdMoveBackward = new MoveBackward();
            _cmdMoveLeft = new MoveLeft();
            _cmdMoveRight = new MoveRight();
            _cmdReplay = new Replay();
            _cmdJump = new Jump();

            //Bind keys with commands
            _keyBinds = new Dictionary<KeyCode, Command>
            {
                {KeyCode.A, _cmdMoveLeft},
                {KeyCode.D, _cmdMoveRight},
                {KeyCode.R, _cmdReplay},
                {KeyCode.S, _cmdMoveBackward},
                {KeyCode.W, _cmdMoveForward},
                {KeyCode.Space, _cmdJump}
            };

            _goStartPos = gameObject.transform.position;
            _goStartRot = gameObject.transform.rotation;
        }

        /*[UsedImplicitly]
        private void Update()
        {
        }*/

        [UsedImplicitly]
        private void FixedUpdate()
        {
            HandleInput();
        }

        //Check if we press a key, if so do what the key is binded to 
        public void HandleInput()
        {
            foreach(var entry in _keyBinds)
            {
                if(!Input.GetKey(entry.Key)) continue;
                var newCommand = (Command)Activator.CreateInstance(entry.Value.GetType());
                newCommand.InputHandler = this;
                newCommand.Execute(gameObject, newCommand);
            }
        }

        public void StartReplay()
        {
            if (Commands.Count == 0) return;
            GameObject clone = Instantiate(gameObject);
            clone.layer = LayerMask.NameToLayer("ClonesGO");
            Destroy(clone.GetComponent<InputHandler>());
            CommandReplayer replayer = clone.AddComponent<CommandReplayer>();
            replayer.Setup(Commands, _goStartPos, _goStartRot);

            Commands = new List<Command>();
            BeginningTime = Utils.GetTimeinMilliseconds();
            _goStartPos = gameObject.transform.position;
            _goStartRot = gameObject.transform.rotation;
        }

    }
}
