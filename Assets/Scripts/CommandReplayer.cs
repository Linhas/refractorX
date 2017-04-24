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
    //GO start position
    private Vector3 _goStartPos;
    //GO start rotation
    private  Quaternion _goStartRot;

    private bool _isReplaying;

    // Use this for initialization
    [UsedImplicitly]
    private void Start()
    {
        gameObject.transform.position = _goStartPos;
        gameObject.transform.rotation = _goStartRot;

        gameObject.GetComponent<Renderer>().material.SetColor("_Color",
            new Color(gameObject.GetComponent<Renderer>().material.color.r, gameObject.GetComponent<Renderer>().material.color.g, gameObject.GetComponent<Renderer>().material.color.b, 0.5f));

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

    public void Setup(List<Command> commands, Vector3 goStartPos, Quaternion goStartRot)
    {
        _commands = new List<Command>();
        foreach(var cmd in commands)
        {
            var newCmd = (Command)Activator.CreateInstance(cmd.GetType());
            newCmd.TimeStamp = cmd.TimeStamp;
            _commands.Add(newCmd);
        }
        _goStartPos = new Vector3(goStartPos.x, goStartPos.y, goStartPos.z);
        _goStartRot = new Quaternion(goStartRot.x, goStartRot.y, goStartRot.z, goStartRot.w);
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
            //We can move the box again
            _isReplaying = false;
        }
    }
}
