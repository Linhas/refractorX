using System;

/// <summary>
/// Editor utility class. Encapsulates elias_event_set_level in an easy to store way.
/// </summary>
[Serializable]
public class EliasSetLevel
{
	public elias_event_flags flags;
	public int preWaitTimeMs;
	public int level;
	public ushort jumpToBar;
	public int suggestedMaxTimeMs;
	public string transitionPresetName;
	public string themeName;
	public string trackGroupName;
	public string stingerName;

	public elias_event_set_level CreateSetLevelEvent(EliasHelper elias)
	{
		return new elias_event_set_level(
			(uint)flags,
			(uint)preWaitTimeMs,
			transitionPresetName == "" ? 0 : elias.GetTransitionPresetIndex(transitionPresetName),
			elias.GetThemeIndex(themeName),
			level,
			elias.GetGroupIndex(trackGroupName),
			jumpToBar,
			(uint)suggestedMaxTimeMs,
			elias.GetStingerIndex("", stingerName)); 
            //Note: By not passing in a theme name, we are getting the stinger index of the currently playing theme.
            //This is required when changing themes, as the stinger that plays is from the theme that is transitioned away from.
	}
}