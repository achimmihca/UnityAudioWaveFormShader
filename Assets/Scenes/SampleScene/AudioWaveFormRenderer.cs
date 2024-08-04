using System;
using UnityEngine;

public class AudioWaveFormRenderer : MonoBehaviour
{
    public int audioWaveformResolution = 1024;
    public float fromTimeInSeconds;
    public float untilTimeInSeconds = 30;

    public Material material;
    public AudioClip audioClip;

    private float[] audioSamples;

    private float[] minValues;
    private float[] maxValues;

    private ComputeBuffer minValuesComputeBuffer;
    private ComputeBuffer maxValuesComputeBuffer;

    private float lastFromTimeInSeconds;
    private float lastUntilTimeInSeconds;

    void Start()
    {
        using (new DisposableStopwatch($"Initialize arrays and ComputeBuffer"))
        {
            // Initialize the ComputeBuffer with the float array
            minValues = new float[audioWaveformResolution];
            maxValues = new float[audioWaveformResolution];

            minValuesComputeBuffer = new ComputeBuffer(audioWaveformResolution, sizeof(float));
            minValuesComputeBuffer.SetData(minValues);

            maxValuesComputeBuffer = new ComputeBuffer(audioWaveformResolution, sizeof(float));
            maxValuesComputeBuffer.SetData(maxValues);

            // Pass the ComputeBuffer to the shader via the material
            material.SetBuffer("_MinAmplitude", minValuesComputeBuffer);
            material.SetBuffer("_MaxAmplitude", maxValuesComputeBuffer);
            material.SetInt("_ArraySize", audioWaveformResolution);
        }

        using (new DisposableStopwatch($"Get audio samples"))
        {
            audioSamples = AudioUtil.GetAudioSamples(audioClip, 0);
        }
    }

    private void Update()
    {
        if (Math.Abs(lastFromTimeInSeconds - fromTimeInSeconds) > 0.1f
            || Math.Abs(lastUntilTimeInSeconds - untilTimeInSeconds) > 0.1f)
        {
            UpdateWaveForm();
        }

        lastFromTimeInSeconds = fromTimeInSeconds;
        lastUntilTimeInSeconds = untilTimeInSeconds;
    }

    void UpdateWaveForm()
    {
        if (fromTimeInSeconds < 0 || untilTimeInSeconds < 0 || fromTimeInSeconds >= untilTimeInSeconds)
        {
            Debug.LogError("Invalid start and end time");
            return;
        }

        Debug.Log($"Updating waveform from {fromTimeInSeconds} to {untilTimeInSeconds} seconds.");

        MinMax[] minAndMaxValues;
        using (new DisposableStopwatch($"Calculate min and max values"))
        {
            int audioWaveFormSampleStart = (int)(audioClip.frequency * fromTimeInSeconds);
            int audioWaveFormSampleEnd = (int)(audioClip.frequency * untilTimeInSeconds);

            minAndMaxValues = AudioUtil.CalculateMinAndMaxValues(audioSamples, audioWaveFormSampleStart, audioWaveFormSampleEnd, audioWaveformResolution);
        }

        using (new DisposableStopwatch($"Copy min and max values"))
        {
            for (int i = 0; i < audioWaveformResolution; i++)
            {
                minValues[i] = minAndMaxValues[i].min;
                maxValues[i] = minAndMaxValues[i].max;
            }
        }

        using (new DisposableStopwatch($"Set ComputeBuffer data"))
        {
            minValuesComputeBuffer.SetData(minValues);
            maxValuesComputeBuffer.SetData(maxValues);
        }
    }

    void OnDestroy()
    {
        minValuesComputeBuffer?.Dispose();
        maxValuesComputeBuffer?.Dispose();
    }
}
