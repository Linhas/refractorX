using System.Collections.Generic;
using CommandPattern;
using JetBrains.Annotations;
using UnityEngine;
using System;

[RequireComponent(typeof(Rigidbody))]
public class CommandReplayer : MonoBehaviour {

    //Replay beginning timestamp
    private long _beginningTime;
    //Stores remaining commands during replay
    private List<Command> _commands;
    //Stores all commands for replay
    private List<Command> _commandsBackup = new List<Command>();
    //GO start position
    private Vector3 _goStartPos;
    //GO start direction
    private  Direction _goStartDir;

    private bool _isReplaying;

    // Use this for initialization
    [UsedImplicitly]
    public void Start()
    {
        gameObject.transform.position = _goStartPos;
        switch (_goStartDir)
        {
            case Direction.Forward:
                gameObject.GetComponent<Rigidbody>().MoveRotation(Quaternion.Euler(0, 0, 0));
                break;
            case Direction.Right:
                gameObject.GetComponent<Rigidbody>().MoveRotation(Quaternion.Euler(0, 90, 0));
                break;
            case Direction.Backward:
                gameObject.GetComponent<Rigidbody>().MoveRotation(Quaternion.Euler(0, 180, 0));
                break;
            case Direction.Left:
                gameObject.GetComponent<Rigidbody>().MoveRotation(Quaternion.Euler(0, -90, 0));
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        gameObject.GetComponent<InputHandler>().CurrDirection = _goStartDir;

        //gameObject.GetComponent<Renderer>().material.SetColor("_Color",
          //  new Color(gameObject.GetComponent<Renderer>().material.color.r, gameObject.GetComponent<Renderer>().material.color.g, gameObject.GetComponent<Renderer>().material.color.b, 0.5f));

        _commands = new List<Command>(_commandsBackup);

        _beginningTime = Utils.GetTimeinMilliseconds();
        _isReplaying = true;
    }

    // Update is called once per frame
    [UsedImplicitly]
	private void FixedUpdate()
    {
		if(_isReplaying)
            ReplayCommands();
	}

    public void Setup(List<Command> commands, Vector3 goStartPos, Direction goStartDir)
    {
        _commandsBackup = new List<Command>();
        foreach(var cmd in commands)
        {
            var newCmd = (Command)Activator.CreateInstance(cmd.GetType());
            newCmd.InputHandler = gameObject.GetComponent<InputHandler>();
            newCmd.TimeStamp = cmd.TimeStamp;
            _commandsBackup.Add(newCmd);
        }
        _goStartPos = new Vector3(goStartPos.x, goStartPos.y, goStartPos.z);
        _goStartDir = goStartDir;
    }

    private void ReplayCommands()
    {
        for (var i = 0; i < _commands.Count; i++)
        {
            Command cmd = _commands[i];
            //Don't execute command unless it's at least its time
            if (cmd.TimeStamp >= Utils.GetTimeinMilliseconds() - _beginningTime) break;

            cmd.Execute(gameObject, cmd);
            _commands.RemoveAt(i);
            i--;
        }

        if (_commands.Count == 0)
        {
            _isReplaying = false;
        }
    }
}
