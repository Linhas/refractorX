using System;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;

/// <summary>
/// Handles interaction with ELIAS. Plays ELIAS' output through the attached AudioSource.
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class EliasPlayer : MonoBehaviour
{
	[HideInInspector]
	public string file;
	[HideInInspector]
	public int actionPreset = -1;
	[HideInInspector]
	public EliasSetLevel customPreset;
	[HideInInspector]
	public int eliasFramesPerBuffer = 4096;

	public bool playOnStart = true;
	public int eliasChannelCount = 2;

	private volatile EliasHelper elias;
	private volatile EliasAudioReader audioReader;
    
    private volatile bool isEliasStarted = false;

    //Change this if you want to use the high latency mode, it will allow unity to handle channel and sample rate conversions in exchange for a higher latency with ELIAS.
    private volatile bool useHighLatencyMode = false;
    
    public EliasHelper Elias
	{
		get
		{
			return elias;
		}
    }

    /// <summary>
    /// Stop the rendering of the theme. This function must not be called while any other Elias calls are in progress with the same handle in another thread.
    /// The exception is if the engine has its own background threads; these will automatically be shut down.
    /// IMPORTANT: This function must never use thread unsafe calls, as it can be called from either the audio thread in case of failure to render, or the main thread, (game).
    /// </summary>
    public void Stop()
	{
        isEliasStarted = false;
        if (elias != null)
        {
            EliasWrapper.elias_result_codes r = EliasWrapper.elias_stop(elias.Handle);
            EliasHelper.LogResult(r, "Problems stopping");
        }
	}

	/// <summary>
	/// Lock and unlock the mixer in order to allow many changes to be made without the risk of running one or more buffers ahead in between your changes.
	/// Note that multiple calls to elias_lock_mixer may be made in succession, as long as the same number of calls to elias_unlock_mixer are made (a recursive mutex, in other words).
	/// If you forget to unlock the mixer, the music will stop and you will get deadlocks.
	/// </summary>
	public void LockMixer()
	{
		EliasWrapper.elias_result_codes r = EliasWrapper.elias_lock_mixer(elias.Handle);
		EliasHelper.LogResult(r, "Problems locking the mixer");
	}

	/// <summary>
	/// Lock and unlock the mixer in order to allow many changes to be made without the risk of running one or more buffers ahead in between your changes.
	/// Note that multiple calls to elias_lock_mixer may be made in succession, as long as the same number of calls to elias_unlock_mixer are made (a recursive mutex, in other words).
	/// If you forget to unlock the mixer, the music will stop and you will get deadlocks.
	/// </summary>
	public void UnlockMixer()
	{
		EliasWrapper.elias_result_codes r = EliasWrapper.elias_unlock_mixer(elias.Handle);
		EliasHelper.LogResult(r, "Problems unlocking the mixer");
	}

	/// <summary>
	/// Clear all queued events.
	/// </summary>
	public void ClearEvents()
	{
		EliasWrapper.elias_result_codes r = EliasWrapper.elias_clear_events(elias.Handle);
		EliasHelper.LogResult(r, "Problems clearing events");
	}
		
	/// <summary>
	/// Run an action preset.
	/// </summary>
	public void RunActionPreset(string preset, bool ignoreRequiredThemeMissmatch = false)
	{
		EliasWrapper.elias_result_codes r = EliasWrapper.elias_run_action_preset(elias.Handle, preset);
		if (ignoreRequiredThemeMissmatch == false || r != EliasWrapper.elias_result_codes.elias_error_requiredthememismatch)
		{
			EliasHelper.LogResult(r, "Problems running an action preset");
		}
	}

	/// <summary>
	/// Check whether the given action preset can be run at the current time.
	/// </summary>
	public bool CanRunActionPreset(string preset)
	{
		return EliasWrapper.elias_can_run_action_preset(elias.Handle, preset) == EliasWrapper.elias_result_codes.elias_result_success;
	}

	/// <summary>
	/// Get the current theme.
	/// </summary>
	public string GetActiveTheme()
	{
        return elias.GetActiveTheme();

    }

	/// <summary>
	/// Get greatest Level in theme. Returns -1 on error.
	/// </summary>
	public int GetGreatestLevelInTheme(string themeName)
	{
		return elias.GetGreatestLevelInTheme(themeName);
	}

	/// <summary>
	/// Get greatest Level on track. Returns -1 on error.
	/// </summary>
	public int GetGreatesLeveltOnTrack(string themeName, string trackName)
	{
		return EliasWrapper.elias_get_greatest_level_on_track(elias.Handle, themeName, trackName);
	}

	/// <summary>
	/// Start ELIAS with an action preset. This function must not be called from more than one thread at the same time.
	/// </summary>
	public bool StartWithActionPreset(string preset)
	{
		EliasWrapper.elias_result_codes r = EliasWrapper.elias_start_with_action_preset(elias.Handle, preset);
		EliasHelper.LogResult(r, preset);
		return r == EliasWrapper.elias_result_codes.elias_result_success;
	}

	/// <summary>
	/// Start ELIAS with a elias_event_set_level event. This function must not be called from more than one thread at the same time.
	/// </summary>
	public bool StartTheme(elias_event_set_level setLevel)
	{
		EliasWrapper.elias_result_codes r = EliasWrapper.elias_start_wrapped(elias.Handle, setLevel);
		EliasHelper.LogResult(r, setLevel.ToString());
		return r == EliasWrapper.elias_result_codes.elias_result_success;
	}

	/// <summary>
	/// Queue an elias_event.
	/// </summary>
	public void QueueEvent(elias_event @event)
	{
		EliasWrapper.elias_result_codes r = EliasWrapper.elias_queue_event_wrapped(elias.Handle, @event);
		EliasHelper.LogResult(r, "Problems queing an event");
	}
		
	private void Start()
    {
		elias = new EliasHelper(file, eliasChannelCount, eliasFramesPerBuffer);
		if (playOnStart) 
		{
			StartElias ();
		}
	}

	private void OnDestroy()
    {
        Stop();
        if (elias != null)
        {
            elias.Dispose();
        }
        if (audioReader != null)
        {
            audioReader.Dispose();
        }
	}

	public bool StartElias()
	{
		bool startedSucessfully;
		if (actionPreset >= 0)
		{
			startedSucessfully = StartWithActionPreset();
		}
		else
		{
			startedSucessfully = StartTheme(customPreset.CreateSetLevelEvent(elias));
		}
		if (startedSucessfully)
        {
            isEliasStarted = true;
            if (audioReader == null)
            {
                audioReader = new EliasAudioReader(elias, GetComponent<AudioSource>(), useHighLatencyMode);
            }
			GetComponent<AudioSource> ().Play ();
        }
		return startedSucessfully;
	}

	private bool StartWithActionPreset()
	{
		bool success;
		string name;
		EliasWrapper.elias_result_codes r = EliasWrapper.elias_get_action_preset_name_wrapped(elias.Handle, (uint)actionPreset, out name);
		EliasHelper.LogResult(r, "Problems getting an action preset name");
		string theme;
		r = EliasWrapper.elias_get_action_preset_required_initial_theme_wrapped(elias.Handle, name, out theme);
		EliasHelper.LogResult(r, "Problems with getting the required initial theme for an action preset");
		success = StartWithActionPreset(name);
		return success;
    }

    //Due to some problems in Unity where they always pre-buffer 400ms of procedural audio, we use the OnAudioFilterRead to get close to no latency.
    void OnAudioFilterRead(float[] data, int channels)
	{
        if (isEliasStarted && useHighLatencyMode == false)
        {
            if (audioReader.ReadCallback(data) == false)
            {
                Stop();
            }
        }
    }
}