using System;
using UnityEngine;

public class AudioUtil
{
    public static float[] GetAudioSamples(AudioClip audioClip, int channel)
    {
        float[] singleChannelSamples = new float[audioClip.samples];
        float[] allSamples = new float[audioClip.samples * audioClip.channels];
        audioClip.GetData(allSamples, 0);

        // Fill the single channel array with the samples of the selected channel
        for (int i = 0; i < singleChannelSamples.Length; i++)
        {
            singleChannelSamples[i] = allSamples[i * audioClip.channels + channel];
        }

        return singleChannelSamples;
    }

    public static MinMax[] CalculateMinAndMaxValues(float[] samples, int minSample, int maxSample, int count)
    {
        MinMax[] minMaxValues = new MinMax[count];

        // calculate window size to fit all samples in the texture
        int lengthInSamples = maxSample - minSample;
        if (lengthInSamples <= count)
        {
            throw new Exception("Too few samples to calculate min and max values");
        }

        int windowSize = lengthInSamples / count;

        // move the window over all the samples. For each position, find the min and max value.
        for (int i = 0; i < count; i++)
        {
            int offset = minSample + i * windowSize;
            FindMinAndMaxValues(samples, offset, windowSize, out float min, out float max);
            minMaxValues[i].min = min;
            minMaxValues[i].max = max;
        }

        return minMaxValues;
    }

    private static void FindMinAndMaxValues(float[] samples, int offset, int length, out float min, out float max)
    {
        min = float.MaxValue;
        max = float.MinValue;

        for (int i = 0; i < length; i++)
        {
            int index = offset + i;
            if (index >= samples.Length)
            {
                min = 0;
                max = 0;
                break;
            }

            float f = samples[index];
            if (f < min)
            {
                min = f;
            }
            if (f > max)
            {
                max = f;
            }
        }
    }
}
