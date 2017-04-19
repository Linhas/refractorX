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
        //Replay beginning timestamp
        private static long _replayBegTime;
        //Different commands needed
        private Command _cmdMoveForward, _cmdMoveBackward, _cmdMoveLeft, _cmdMoveRight, _cmdReplay, _cmdJump;
        //Key bindings map
        private Dictionary<KeyCode, Command> _keyBinds;
        //Stores all commands for replay
        public static List<Command> OldCommands = new List<Command>();
        //Stores remaining commands during replay
        private static List<Command> _replayCommands = new List<Command>();
        //Box start position to know where replay begins
        private static Vector3 _boxStartPos;
        //So we cant press keys while replaying
        public static bool IsReplaying;

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

            _boxStartPos = gameObject.transform.position;
        }

        /*[UsedImplicitly]
        private void Update()
        {
        }*/

        [UsedImplicitly]
        private void FixedUpdate()
        {
            if(IsReplaying)
                ReplayCommands();
            else
                HandleInput();
        }

        //Check if we press a key, if so do what the key is binded to 
        public void HandleInput()
        {
            foreach(var entry in _keyBinds)
            {
                if(!Input.GetKey(entry.Key)) continue;
                var newCommand = (Command)Activator.CreateInstance(entry.Value.GetType());
                newCommand.Execute(gameObject, newCommand);
            }
        }

        public void StartReplay()
        {
            IsReplaying = true;
            _replayCommands = new List<Command>(OldCommands);
            _replayBegTime = Utils.GetTimeinMilliseconds();
            //Move the box to the start position
            gameObject.transform.position = _boxStartPos;
        }

        private void ReplayCommands()
        {
            for(var i=0; i < _replayCommands.Count; i++)
            {
                Command cmd = _replayCommands[i];
                //Don't execute command unless it's at least past its time
                if(cmd.TimeStamp >= Utils.GetTimeinMilliseconds() - _replayBegTime) break;

                cmd.Execute(gameObject, cmd);
                _replayCommands.RemoveAt(i);
                i--;
            }

            if (_replayCommands.Count == 0)
            {
                //We can move the box again
                IsReplaying = false;
            }
        }
    }
}
