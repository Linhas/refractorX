using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CommandPattern;

public class LevelChanger : MonoBehaviour {

	public int NextLevel;
	public InputHandler InputHandler;

	public void ChangeLevel()
	{
		InputHandler.CurrLevel = NextLevel;
	}
}
