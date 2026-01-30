/*
 * ii's Stupid Menu  Managers/VoiceManager.cs
 * A mod menu for Gorilla Tag with over 1000+ mods
 *
 * Copyright (C) 2026  Goldentrophy Software
 * https://github.com/iiDk-the-actual/iis.Stupid.Menu
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <https://www.gnu.org/licenses/>.
 */

// Written with love by kingofnetflix </3
// For anyone else snooping in this class hoping to use it, you need to make sure that your recorder source type is a Factory and that the Factory is a new instance of this class.
// You may use VoiceManager.Get()
using iiMenu.Managers;
using Photon.Voice;
using System;
using UnityEngine;

public class VoiceManager : IAudioReader<float>
{
    private int samplingRate = 16000;
    private float gain = 1;
    private float pitch = 1f;

    private readonly int loopLength;
    private string currentDevice;
    public AudioClip microphoneClip;
    private int lastSamplePosition;

    private string error;

    private float[] tempBuffer;
    private float resample;
    private float[] audioClip;
    private float audioClipPosition;
    private float audioClipStep;

    private bool muteMicrophone;

    public VoiceManager(int micLoopLengthSeconds = 1, string device = null)
    {
        this.loopLength = micLoopLengthSeconds;
        StartRecording(device);
    }

    /// <summary>
    /// Gets or sets the microphone's recording status. This does not stop the pushed AudioClip from playing.
    /// </summary>
    public bool MuteMicrophone
    {
        get { return muteMicrophone; }
        set { muteMicrophone = value; }
    }

    /// <summary>
    /// Gets or sets the microphone sampling rate. Setting a value restarts the microphone.
    /// </summary>
    public int SamplingRate
    {
        get { return samplingRate; }
        set { samplingRate = value; RestartMicrophone(); }
    }

    /// <summary>
    /// Gets or sets the microphone gain multiplier.
    /// </summary>
    public float Gain
    {
        get { return gain; }
        set { gain = value; }
    }

    /// <summary>
    /// Gets or sets the pitch. Lowest possible value can be 0.1f.
    /// </summary>
    public float Pitch
    {
        get => pitch;
        set => pitch = Mathf.Max(0.1f, value); 
    }

    /// <summary>
    /// A post processer that can be used to edit the buffer after all the audio data is compiled.
    /// </summary>
    public Func<float[], bool> PostProcess { get; set; }

    /// <summary>
    /// Gets or sets the decision on if the post processing should affect the applied Audio Clip or not.
    /// </summary>
    public bool PostProcessClip { get; set; }


    public int Channels => 1;
    public string Error => error;
    public string CurrentDevice => currentDevice;

    public static VoiceManager Instance { get; private set; }

    /// <summary>
    /// Returns a valid VoiceManager instance. If the Instance variable is null, it will create a new VoiceManager.
    /// </summary>
    /// <param name="micLoopLengthSeconds">Length (in seconds) of the looping mic buffer, handled by Unity when the microphone is started, only used if the instance is null.</param>
    /// <param name="device">The microphone device to be used in recording, if the instance is null.</param>
    /// <returns></returns>
    public static VoiceManager Get(int micLoopLengthSeconds = 1, string device = null)
    {
        return Instance ??= new VoiceManager(micLoopLengthSeconds, device);
    }

    /// <summary>
    /// Starts the microphone recording.
    /// </summary>
    /// <param name="device"> Microphone device name to be used. If empty, the default microphone is selected.</param>
    public bool StartRecording(string device = null)
    {
        error = null;

        if (Microphone.devices.Length == 0)
        {
            error = "No microphone devices found";
            LogManager.LogWarning(error);
            return false;
        }

        currentDevice = string.IsNullOrEmpty(device) ? Microphone.devices[0] : device;

        if (Microphone.IsRecording(currentDevice))
            Microphone.End(currentDevice);

        microphoneClip = Microphone.Start(currentDevice, true, loopLength, samplingRate);
        lastSamplePosition = 0;
        return true;
    }

    /// <summary>
    /// Stops the microphone recording.
    /// </summary>
    public bool StopRecording()
    {
        if (!string.IsNullOrEmpty(currentDevice) && Microphone.IsRecording(currentDevice))
            Microphone.End(currentDevice);

        microphoneClip = null;
        lastSamplePosition = 0;
        return true;
    }

    /// <summary>
    /// Switches the microphone device and restarts recording.
    /// </summary>
    /// <param name="device">Microphone device name to be used.</param>
    public bool SwitchMicrophone(string device)
        => StopRecording() && StartRecording(device);

    /// <summary>
    /// Restarts the microphone using the current device, or the default if none is set.
    /// </summary>
    public bool RestartMicrophone() 
        => StopRecording() && StartRecording(currentDevice);

    /// <summary>
    /// Pushes an AudioClip into the output stream.
    /// </summary>
    /// <param name="clip">AudioClip to play.</param>
    /// <param name="disableMicrophone">Whether to mute the microphone while the clip plays.</param>
    public bool AudioClip(AudioClip clip, bool disableMicrophone)
    {
        if (clip == null)
            return false;

        muteMicrophone = disableMicrophone;

        int channels = clip.channels;
        float[] raw = new float[clip.samples * channels];
        clip.GetData(raw, 0);

        audioClip = new float[clip.samples];
        if (channels == 1)
        {
            for (int i = 0; i < clip.samples; i++)
                audioClip[i] = raw[i];
        }
        else
        {
            for (int i = 0; i < clip.samples; i++)
            {
                float sum = 0f;
                int baseIndex = i * channels;
                for (int c = 0; c < channels; c++)
                    sum += raw[baseIndex + c];
                audioClip[i] = sum / channels;
            }
        }

        audioClipPosition = 0f;
        audioClipStep = clip.frequency / (float)samplingRate;
        return true;
    }

    /// <summary>
    /// Stops the currently playing AudioClip.
    /// </summary>
    public void StopAudioClip()
    {
        audioClip = null;
        audioClipPosition = 0f;
        muteMicrophone = false; // maybe?
    }

    /// <summary>
    /// Used to pull the next chunk of audio samples.
    /// </summary>

    // this is automatically called by photon
    public bool Read(float[] buffer)
    {
        if (microphoneClip == null || string.IsNullOrEmpty(currentDevice)) return false;

        int pos = Microphone.GetPosition(currentDevice);
        int available = (pos < lastSamplePosition) ? microphoneClip.samples - lastSamplePosition + pos : pos - lastSamplePosition;
        if (available < buffer.Length) return false;

        if (tempBuffer == null || tempBuffer.Length != buffer.Length)
            tempBuffer = new float[buffer.Length];

        int remaining = microphoneClip.samples - lastSamplePosition;
        if (remaining >= buffer.Length)
            microphoneClip.GetData(tempBuffer, lastSamplePosition);
        else
        {
            microphoneClip.GetData(tempBuffer, lastSamplePosition);
            microphoneClip.GetData(tempBuffer, 0);
        }

        float[] microphoneBuffer = new float[buffer.Length];
        for (int i = 0; i < buffer.Length; i++)
        {
            float microphoneSample = 0f;
            if (!muteMicrophone)
            {
                int index = (int)resample;
                int nextIndex = index + 1;
                if (index >= tempBuffer.Length) { resample = 0f; index = 0; nextIndex = 1; }
                if (nextIndex >= tempBuffer.Length) nextIndex = 0;

                microphoneSample = Mathf.Lerp(tempBuffer[index], tempBuffer[nextIndex], resample - index);

                resample += pitch;
                if (resample >= tempBuffer.Length) resample = 0f;
            }

            microphoneBuffer[i] = microphoneSample * gain;
        }

        if (!PostProcessClip)
            PostProcess?.Invoke(microphoneBuffer);

        for (int i = 0; i < buffer.Length; i++)
        {
            float pushed = NextAudioClipSample();
            buffer[i] = Mathf.Clamp(microphoneBuffer[i] + pushed, -1f, 1f);
        }

        if (PostProcessClip)
            PostProcess?.Invoke(buffer);

        lastSamplePosition = (lastSamplePosition + buffer.Length) % microphoneClip.samples;
        return true;
    }


    /// <summary>
    /// Returns the next sample from the pushed audio buffer each time the Read() function is called.
    /// </summary>
    private float NextAudioClipSample()
    {
        if (audioClip == null || audioClip.Length == 0)
            return 0f;

        int index = (int)audioClipPosition;
        int nextIndex = index + 1;

        if (nextIndex >= audioClip.Length)
        {
            audioClip = null;
            return 0f;
        }

        float frac = audioClipPosition - index;
        float sample = Mathf.Lerp(audioClip[index], audioClip[nextIndex], frac);

        audioClipPosition += audioClipStep;
        return sample;
    }

    public void Dispose()
    {
        StopRecording();
        Instance = null;
    }
}