using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// Encapsulates one ELIAS project and provides utility functions.
/// </summary>
public class EliasHelper
{
	public const int ABI_VERSION = 1;

	public IntPtr Handle
	{
		get;
		private set;
	}

	public int ChannelCount
	{
		get;
		private set;
	}

	public int SampleRate
	{
		get;
		private set;
	}

	public int FramesPerBuffer
	{
		get;
		private set;
	}

	/// <summary>
	/// Initialize ELIAS and deserializes "file" with default values. Expects the audio files in the same folder.
	/// </summary>
	private static string GetPath(string file)
	{
		return Path.GetDirectoryName(file);
	}

	/// <summary>
	/// Initializes ELIAS and deserializes "file" with all provided parameters.
	/// </summary>
	public EliasHelper(string file) : this(Path.Combine(Application.streamingAssetsPath, file), null, GetPath(Path.Combine(Application.streamingAssetsPath, file)), (uint)AudioSettings.outputSampleRate, 2, 2048, null)
	{
	}

	/// <summary>
	/// Initializes ELIAS and deserializes "file" with all provided parameters. This overload also takes channels and framesPerBuffer.
	/// </summary>
	public EliasHelper(string file, int channels, int framesPerBuffer) : this(Path.Combine(Application.streamingAssetsPath, file), null, GetPath(Path.Combine(Application.streamingAssetsPath, file)), (uint)AudioSettings.outputSampleRate, (byte)channels, (ushort)framesPerBuffer, null)
	{
	}

	public EliasHelper(string file, EliasWrapper.elias_data_reader_functions? readerFunctions, string basePath, uint sampleRate, byte channels, ushort framesPerBuffer, EliasWrapper.elias_memory_functions? memoryFunctions)
	{
		ChannelCount = channels;
		SampleRate = (int)sampleRate;
		FramesPerBuffer = (int)framesPerBuffer;
        Initialize(readerFunctions, basePath, sampleRate, channels, framesPerBuffer, memoryFunctions);
		Deserialize(file);
	}

	public static void LogResult(EliasWrapper.elias_result_codes result, string errorMessage = "")
	{
		if (result != EliasWrapper.elias_result_codes.elias_result_success)
		{
			Debug.LogError(result + "\n" + errorMessage);
		}
	}

	public IList<string> GetActionPresets()
	{
		string name;
		List<string> presets = new List<string>();
		for (uint i = 0; i < EliasWrapper.elias_get_action_preset_count(Handle); i++)
		{
			EliasWrapper.elias_result_codes result = EliasWrapper.elias_get_action_preset_name_wrapped(Handle, i, out name);
			LogResult(result, i.ToString());
			presets.Add(name);
		}
		return presets;
	}

	public IList<string> GetTransitionPresets()
	{
		List<string> presets = new List<string>();
		for (uint i = 0; i < EliasWrapper.elias_get_transition_preset_count(Handle); i++)
		{
			string name;
			EliasWrapper.elias_result_codes r = EliasWrapper.elias_get_transition_preset_name_wrapped(Handle, i, out name);
			LogResult(r, i.ToString());
			presets.Add(name);
		}
		return presets;
	}

	public IList<string> GetThemes()
	{
		List<string> themes = new List<string>();
		for (uint i = 0; i < EliasWrapper.elias_get_theme_count(Handle); i++)
		{
			string name;
			EliasWrapper.elias_result_codes r = EliasWrapper.elias_get_theme_name_wrapped(Handle, i, out name);
			LogResult(r, i.ToString());
			themes.Add(name);
		}
		return themes;
	}

	public IList<string> GetTracks(string theme)
	{
		List<string> tracks = new List<string>();
		for (uint i = 0; i < EliasWrapper.elias_get_track_count(Handle, theme); i++)
		{
			string name;
			EliasWrapper.elias_result_codes r = EliasWrapper.elias_get_track_name_wrapped(Handle, theme, i, out name);
			LogResult(r, theme);
			tracks.Add(name);
		}
		return tracks;
	}

	public IList<string> GetTracksGroups()
	{
		List<string> tracks = new List<string>();
		for (uint i = 0; i < EliasWrapper.elias_get_track_group_count(Handle); i++)
		{
			string name;
			EliasWrapper.elias_result_codes r = EliasWrapper.elias_get_track_group_name_wrapped(Handle, i, out name);
			LogResult(r, name + " " + i);
			tracks.Add(name);
		}
		return tracks;
	}

	public IList<string> GetStingers(string theme)
	{
		List<string> tracks = new List<string>();
		for (uint i = 0; i < EliasWrapper.elias_get_track_count(Handle, theme); i++)
		{
			string name;
			EliasWrapper.elias_result_codes r = EliasWrapper.elias_get_track_name_wrapped(Handle, theme, i, out name);
			LogResult(r, theme);
			uint type;
			r = EliasWrapper.elias_get_track_type(Handle, theme, name, out type);
			LogResult(r, theme);
			if (type == (int)elias_track_types.elias_track_audio_stinger)
			{
				tracks.Add(name);
			}
		}
		return tracks;
	}

	public int GetGreatestLevelInTheme(string theme)
	{
		return EliasWrapper.elias_get_greatest_level_in_theme(Handle, theme);
	}

	public EliasBasicInfo GetBasicInfo(string theme)
	{
		EliasBasicInfo info;
		EliasWrapper.elias_result_codes r = EliasWrapper.elias_get_theme_basic_info(Handle, theme, out info.initial_bpm, out info.initial_timesig_numerator, out info.initial_timesig_denominator, out info.bars);
		LogResult(r, theme);
		return info;
	}
	
	public uint GetBusIndex(string busName)
	{
		uint busIndex;
		EliasWrapper.elias_result_codes r = EliasWrapper.elias_get_bus_index(Handle, busName, out busIndex);
		LogResult(r, "Failed to get bus index for " + busName);
		return busIndex;
	}

	public uint GetTransitionPresetIndex(string transitionPresetName)
	{
		uint transitionPresetIndex;
		EliasWrapper.elias_result_codes r = EliasWrapper.elias_get_transition_preset_index(Handle, transitionPresetName, out transitionPresetIndex);
		LogResult(r, "Failed to get transition preset index for " + transitionPresetName);
		if (r != EliasWrapper.elias_result_codes.elias_result_success) 
		{
			transitionPresetIndex = 0;
		}
		return transitionPresetIndex;
	}

	public uint GetThemeIndex(string themeName)
	{
		uint themeIndex;
		EliasWrapper.elias_result_codes r = EliasWrapper.elias_get_theme_index(Handle, themeName, out themeIndex);
		LogResult(r, themeName);
		return themeIndex;
	}

	public int GetGroupIndex(string trackGroupName)
	{
		uint tmp;
		int affectedTracksGroupIndex;
		if (IsValidName(trackGroupName))
		{
			EliasWrapper.elias_result_codes r = EliasWrapper.elias_get_track_group_index(Handle, trackGroupName, out tmp);
			affectedTracksGroupIndex = (int)tmp;
			LogResult(r, trackGroupName);
		}
		else
		{
			affectedTracksGroupIndex = -1;
		}
		return affectedTracksGroupIndex;
    }

    /// <summary>
    /// Get the current theme.
    /// </summary>
    public string GetActiveTheme()
    {
        uint id = (uint)EliasWrapper.elias_get_active_theme_index(Handle, new double[0]);
        string name;
        EliasWrapper.elias_result_codes r = EliasWrapper.elias_get_theme_name_wrapped(Handle, id, out name);
        EliasHelper.LogResult(r, "Failed to get active theme");
        return name;
    }

    public int GetStingerIndex(string themeName, string stingerName)
	{
		int stingerIndex;
		uint tmp;
		if (IsValidName(stingerName))
		{
            if (themeName == "")
            {
                themeName = GetActiveTheme();
            }
			EliasWrapper.elias_result_codes r = EliasWrapper.elias_get_track_index(Handle, themeName, stingerName, out tmp);
			stingerIndex = (int)tmp;
			LogResult(r, "Failed to get stinger index");
		}
		else
		{
			stingerIndex = -1;
		}
		return stingerIndex;
	}

	/// <summary>
	/// Disposes the ELIAS reference. This instance of EliasHelper can't be used after this method is called.
	/// </summary>
	public void Dispose()
	{
		EliasWrapper.elias_free(Handle);
	}

	private void Initialize(EliasWrapper.elias_data_reader_functions? readerFunctions, string basePath, uint sampleRate, byte channels, ushort framesPerBuffer, EliasWrapper.elias_memory_functions? memoryFunctions)
    {
        string innerPath = basePath;
#if UNITY_ANDROID && !UNITY_EDITOR
        string pathToArchive;
        int splitIndex = basePath.IndexOf('!');
		innerPath = basePath.Substring(splitIndex + 2);
		pathToArchive = basePath.Remove(splitIndex);
		pathToArchive = pathToArchive.Replace("jar:file:/", "");
#endif
        EliasWrapper.elias_result_codes result;
		Handle = EliasWrapper.elias_initialize_wrapped(out result, ABI_VERSION, readerFunctions, innerPath, sampleRate, channels, framesPerBuffer, memoryFunctions);
		LogResult(result, basePath);

#if UNITY_ANDROID && !UNITY_EDITOR
        result = EliasWrapper.elias_set_archive(Handle, pathToArchive, 0);
		LogResult(result, pathToArchive);
#endif
    }

    private void Deserialize(string file)
	{

        // TODO: CAREFUL here, for safety reasons you shouldn't let this while loop unattended, 
        //		place a timer and error check or find a different better way of waiting for the file to be done reading!
#if UNITY_ANDROID && !UNITY_EDITOR
        WWW themeDataWWW = new WWW(file);
        while (!themeDataWWW.isDone) { }
        byte[] mepro = themeDataWWW.bytes;
#else
        byte[] mepro = File.ReadAllBytes(file);
#endif
        EliasWrapper.elias_result_codes r = EliasWrapper.elias_deserialize(Handle, mepro, (uint)mepro.Length + 1, 0);
        LogResult(r, file);
    }

	private bool IsValidName(string name)
	{
		return !string.IsNullOrEmpty(name) && !name.Equals("None");
	}
}