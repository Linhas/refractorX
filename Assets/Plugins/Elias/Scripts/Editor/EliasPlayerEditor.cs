using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EliasPlayer))]
public class EliasPlayerEditor : Editor
{
	private const string ELIAS_SUFFIX = ".mepro";
	private static string[] projects;
	private static string[] empty = new string[] { "None" };
	private int selectedProject;
	private int oldProject;
	private string[] actionPresets;
	private IList<string> transitionPresets;
	private IList<string> themes;
	private IList<string> trackGroups;
	private IList<string> stingers;
	private EliasHelper elias;
	private EliasPlayer player;

	private string ProjectsPath
	{
		get
		{
			return Application.streamingAssetsPath;
		}
	}

	public override void OnInspectorGUI()
	{
		DrawDefaultInspector ();
		GUIContent[] framesPerBufferDisplayStrings = {new GUIContent("1024"), new GUIContent("2048"), new GUIContent("4096"), new GUIContent("8192")};
		int[] framesPerBufferOptions = {1024, 2048, 4096, 8192};
		player = (EliasPlayer)target;
		player.eliasFramesPerBuffer = EditorGUILayout.IntPopup (new GUIContent("Frames per buffer", "A higher value will generally save performance by doing less disk reads, in exchange for a (slightly) increased latency"), player.eliasFramesPerBuffer, framesPerBufferDisplayStrings, framesPerBufferOptions);
		EditorGUILayout.HelpBox ("Note that the sample rate of the Unity project must match the ELIAS Theme.", MessageType.Warning);
		LoadProjects();
		DrawProjectSelector();
		InitializeElias();
		DrawActionPresets();
		player.file = GetSelectedProjectPath();
		if (player.actionPreset == -1)
		{
			transitionPresets = elias.GetTransitionPresets();
			themes = elias.GetThemes().ToArray();
			EliasSetLevel setLevel = player.customPreset;
			if (setLevel == null)
			{
				setLevel = new EliasSetLevel();
				player.customPreset = setLevel;
			}
			setLevel.preWaitTimeMs = Mathf.Clamp(EditorGUILayout.IntField("Wait Time (ms)", setLevel.preWaitTimeMs), 0, int.MaxValue);
			DrawTransitionPreset(setLevel);
			DrawThemeName(setLevel);
			stingers = empty.Concat(elias.GetStingers(setLevel.themeName)).ToList();
			trackGroups = empty.Concat(elias.GetTracksGroups()).ToList();
			setLevel.jumpToBar = (ushort)Mathf.Clamp(EditorGUILayout.IntField("Jump To Bar", setLevel.jumpToBar), 0, (int)elias.GetBasicInfo(setLevel.themeName).bars);
			DrawTrackGroupName(setLevel);
			setLevel.level = Mathf.Clamp(EditorGUILayout.IntField("Level", setLevel.level), 0, elias.GetGreatestLevelInTheme(setLevel.themeName));
			setLevel.suggestedMaxTimeMs = EditorGUILayout.IntField("Suggested Max Time", setLevel.suggestedMaxTimeMs);
			DrawStinger(setLevel);
		}
		else
		{
			player.customPreset = null;
		}
	}

	private void DrawTransitionPreset(EliasSetLevel setLevel)
	{
		int id = transitionPresets.IndexOf(setLevel.transitionPresetName);
		id = Mathf.Clamp(id, 0, transitionPresets.Count - 1);
		id = EditorGUILayout.Popup("Transition Preset", id, transitionPresets.ToArray());
		setLevel.transitionPresetName = transitionPresets[id];
	}

	private void DrawThemeName(EliasSetLevel setLevel)
	{
		int id = themes.IndexOf(setLevel.themeName);
		id = Mathf.Clamp(id, 0, themes.Count - 1);
		id = EditorGUILayout.Popup("Theme", id, themes.ToArray());
		setLevel.themeName = themes[id];
	}

	private void DrawTrackGroupName(EliasSetLevel setLevel)
	{
		int id = trackGroups.IndexOf(setLevel.trackGroupName);
		id = Mathf.Clamp(id, 0, trackGroups.Count - 1);
		id = EditorGUILayout.Popup("Track Group", id, trackGroups.ToArray());
		setLevel.trackGroupName = trackGroups[id];
	}

	private void DrawStinger(EliasSetLevel setLevel)
	{
		int id = stingers.IndexOf(setLevel.stingerName);
		id = Mathf.Clamp(id, 0, stingers.Count - 1);
		id = EditorGUILayout.Popup("Stinger", id, stingers.ToArray());
		setLevel.stingerName = stingers[id];
	}

	private void DrawProjectSelector()
	{
		if (projects != null)
		{
			oldProject = selectedProject;
			selectedProject = EditorGUILayout.Popup("Project", selectedProject, projects);
		}
		else
		{
			EditorGUILayout.HelpBox("No Themes found.", MessageType.Error);
		}
	}

	private void DrawActionPresets()
	{
		if (elias != null)
		{
			actionPresets = new string[] { "None" }.Concat(elias.GetActionPresets()).ToArray();
			player.actionPreset = EditorGUILayout.Popup("Action Preset", player.actionPreset + 1, actionPresets) - 1;
		}
	}

	private void InitializeElias()
	{
		if ((elias == null || oldProject != selectedProject) && IsProjectSelected())
		{
			elias = new EliasHelper(GetSelectedProjectPath());
		}
	}

	private string GetSelectedProjectPath()
	{
		return projects[selectedProject] + ELIAS_SUFFIX;
	}

	private bool IsProjectSelected()
	{
		return selectedProject >= 0;
	}

	private void LoadProjects()
	{
		try
		{
			projects = Directory.GetFiles(ProjectsPath, "*" + ELIAS_SUFFIX, SearchOption.AllDirectories)
				.Select(s => s.Replace(ProjectsPath, "").Replace(ELIAS_SUFFIX, "").Replace("\\", "/").Remove(0, 1)).ToArray();
		}
		catch (Exception ex)
		{
			Debug.LogException(ex);
		}
		for (int i = 0; i < projects.Count(); i++) 
		{		
			if ((projects[i] + ELIAS_SUFFIX) == player.file) 
			{
				selectedProject = i;
				oldProject = i;
			}
		}
	}
}