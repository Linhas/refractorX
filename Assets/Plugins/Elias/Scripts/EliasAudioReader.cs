using System;
using System.Runtime.InteropServices;
using UnityEngine;
using AOT;

/// <summary>
/// Reads ELIAS' output and plays it via an AudioSource.
/// </summary>
public class EliasAudioReader
{
	private EliasHelper elias;
	private AudioSource audioSource;
	private GCHandle gcHandle;
	private float[] dataBuffer;
	private int currentDataIndex;
	private int bufferedLength;

	public EliasAudioReader(EliasHelper eliasHelper, AudioSource audioSourceTarget, bool useProceduralClip)
	{
		elias = eliasHelper;
		audioSource = audioSourceTarget;
		dataBuffer = new float[elias.FramesPerBuffer * elias.ChannelCount];
		if (useProceduralClip)
        {
            AudioClip clip = AudioClip.Create("elias clip", elias.FramesPerBuffer * elias.ChannelCount, elias.ChannelCount, elias.SampleRate, true, PCMReadCallback);
            audioSource.clip = clip;
        }
        // By not having any audio clip, and making sure ELIAS is the first effect on the Audio Source, ELIAS is treated as the "source".
		audioSource.loop = true;
		gcHandle = GCHandle.Alloc(dataBuffer, GCHandleType.Pinned);
	}

	/// <summary>
	/// Stops the AudioSource and disposes references. DOES NOT stop ELIAS!
	/// </summary>
	public void Dispose()
	{
		audioSource.Stop();
		audioSource = null;
		elias = null;
		gcHandle.Free ();
    }

    private void PCMReadCallback(float[] data)
    {
        ReadCallback(data);
    }


    public bool ReadCallback(float[] data)
	{
		currentDataIndex = 0;
		while (currentDataIndex < data.Length)
		{
			if (bufferedLength > 0)
			{
				int length = Math.Min(data.Length - currentDataIndex, bufferedLength);
				Array.Copy(dataBuffer, 0, data, currentDataIndex, length);
				currentDataIndex += length;
				bufferedLength -= length;
				if (bufferedLength > 0)
				{
					Array.Copy(dataBuffer, length, dataBuffer, 0, bufferedLength);
				}
			}
			else
			{
				EliasWrapper.elias_result_codes r = EliasWrapper.elias_read_samples(elias.Handle, Marshal.UnsafeAddrOfPinnedArrayElement(dataBuffer, 0));
				bufferedLength = elias.FramesPerBuffer * elias.ChannelCount;
				EliasHelper.LogResult(r, "Failed to render");
                if (r != EliasWrapper.elias_result_codes.elias_result_success)
                {
                    return false;
                }
			}
		}
        return true;
	}
}