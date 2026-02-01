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
using Photon.Voice;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace iiMenu.Managers
{
    public class VoiceManager : IAudioReader<float>
    {
        private int samplingRate = 48000;
        private int outputRate = 48000;
        private float gain = 1;
        private float pitch = 1f;

        private readonly int loopLength;
        private string currentDevice;
        public AudioClip microphoneClip;
        private int lastSamplePosition;
        private float step;

        private string error;

        private float[] tempBuffer;
        private float resample;
        public sealed class Clip
        {
            public Guid Id { get; set; }
            public AudioClip Source { get; set; }
            public float[] Samples;
            public float Position;
            public float Step;
            public bool MuteMicrophone;
        }

        private readonly List<Clip> audioClips = new List<Clip>();

        private bool muteMicrophone;

        public VoiceManager(int loopLength = 1, string device = null)
        {
            this.loopLength = loopLength;
            StartRecording(device);
        }

        /// <summary>
        /// A read-only list of AudioClips currently playing
        /// </summary>
        public IReadOnlyList<Clip> AudioClips => audioClips.AsReadOnly();

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
        /// Gets or sets the output rate used for AudioClip samples.
        /// </summary>
        public int OutputRate
        {
            get { return outputRate; }
            set { outputRate = value; RestartMicrophone(); }
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
        /// A list of post processers that can be used to edit the buffer after all the audio data is compiled.
        /// </summary>
        public readonly Dictionary<string, Action<float[]>> PostProcessors = new Dictionary<string, Action<float[]>>();

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
        /// <param name="loopLength">Length (in seconds) of the looping mic buffer, handled by Unity when the microphone is started, only used if the instance is null.</param>
        /// <param name="device">The microphone device to be used in recording, if the instance is null.</param>
        /// <returns></returns>
        public static VoiceManager Get(int loopLength = 1, string device = null)
        {
            return Instance ??= new VoiceManager(loopLength, device);
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
            step = samplingRate / (float)OutputRate;
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
        /// Pushes an <see cref="UnityEngine.AudioClip"/> into the output stream.
        /// </summary>
        /// <param name="clip"><see cref="UnityEngine.AudioClip"/> to play.</param>
        /// <param name="disableMicrophone">Whether to mute the microphone while the clip plays.</param>
        /// <returns><see cref="System.Guid"/></returns>
        public Guid AudioClip(AudioClip clip, bool disableMicrophone = false)
        {
            if (clip == null)
                return Guid.Empty;

            if (clip.frequency != OutputRate)
                clip = Resample(clip, OutputRate);

            int channels = clip.channels;
            float[] raw = new float[clip.samples * channels];
            clip.GetData(raw, 0);

            float[] mono = new float[clip.samples];
            if (channels == 1)
            {
                for (int i = 0; i < clip.samples; i++)
                    mono[i] = raw[i];
            }
            else
            {
                for (int i = 0; i < clip.samples; i++)
                {
                    float sum = 0f;
                    int baseIndex = i * channels;
                    for (int c = 0; c < channels; c++)
                        sum += raw[baseIndex + c];
                    mono[i] = sum / channels;
                }
            }

            var id = Guid.NewGuid();
            audioClips.Add(new Clip
            {
                Id = id,
                Source = clip,
                Samples = mono,
                Position = 0f,
                Step = clip.frequency / (float)OutputRate,
                MuteMicrophone = disableMicrophone
            });

            return id;
        }

        /// <summary>
        /// Resamples the given <see cref="UnityEngine.AudioClip"/> to the specified sample rate.
        /// </summary>
        /// <param name="source">The <see cref="UnityEngine.AudioClip"/> to be resampled.</param>
        /// <param name="targetSampleRate">The desired sample rate for the resulting audio clip.</param>
        /// <returns>A new <see cref="UnityEngine.AudioClip"/> containing the resampled audio data.</returns>

        // this is pretty heavy, but the only fix I could think of for making clip length consistent.
        public static AudioClip Resample(AudioClip source, int sampleRate)
        {
            if (source == null || source.frequency == sampleRate)
                return source;

            int channels = source.channels;
            int sourceSamples = source.samples;

            float[] sourceData = new float[sourceSamples * channels];
            source.GetData(sourceData, 0);

            int targetSamples = Mathf.CeilToInt(source.length * sampleRate);
            float[] targetData = new float[targetSamples * channels];

            float ratio = (float)(sourceSamples - 1) / (targetSamples - 1);

            for (int i = 0; i < targetSamples; i++)
            {
                float srcIndex = i * ratio;
                int index = Mathf.FloorToInt(srcIndex);
                int next = Mathf.Min(index + 1, sourceSamples - 1);
                float t = srcIndex - index;

                for (int c = 0; c < channels; c++)
                    targetData[i * channels + c] = Mathf.Lerp(sourceData[index * channels + c], sourceData[next * channels + c], t);
            }

            var resampled = UnityEngine.AudioClip.Create(
                source.name,
                targetSamples,
                channels,
                sampleRate,
                false
            );

            resampled.SetData(targetData, 0);
            return resampled;
        }

        /// <summary>
        /// Stops the specified <see cref="UnityEngine.AudioClip"/> from playing.
        /// </summary>
        /// <param name="id">The GUID of the <see cref="UnityEngine.AudioClip"/> to stop.</param>
        public bool StopAudioClip(Guid id)
        {
            int index = audioClips.FindIndex(c => c.Id == id);
            if (index == -1) return false;

            audioClips.RemoveAt(index);
            return true;
        }


        /// <summary>
        /// Stops all the currently playing audio clips.
        /// </summary>
        public void StopAudioClips() =>
            audioClips.Clear();

        /// <summary>
        /// Used to pull the next chunk of audio samples.
        /// </summary>

        // this is automatically called by photon
        public bool Read(float[] buffer)
        {
            if (microphoneClip == null || string.IsNullOrEmpty(currentDevice)) return false;

            int samples = Mathf.CeilToInt(buffer.Length * step);
            int pos = Microphone.GetPosition(currentDevice);
            int available = (pos < lastSamplePosition) ? microphoneClip.samples - lastSamplePosition + pos : pos - lastSamplePosition;
            if (available < samples) return false;

            if (tempBuffer == null || tempBuffer.Length != samples)
                tempBuffer = new float[samples];

            int remaining = microphoneClip.samples - lastSamplePosition;
            if (remaining >= samples)
                microphoneClip.GetData(tempBuffer, lastSamplePosition);
            else
            {
                microphoneClip.GetData(tempBuffer, lastSamplePosition);
                int wrap = samples - remaining;
                float[] wrapBuffer = new float[wrap];
                microphoneClip.GetData(wrapBuffer, 0);
                Array.Copy(wrapBuffer, 0, tempBuffer, remaining, wrap);
            }

            float[] microphoneBuffer = new float[buffer.Length];
            for (int i = 0; i < buffer.Length; i++)
            {
                float microphoneSample = 0;
                if (!muteMicrophone && !audioClips.Any(c => c.MuteMicrophone))
                {
                    int index = (int)resample;
                    int nextIndex = index + 1;
                    if (index >= tempBuffer.Length) { resample = 0f; index = 0; nextIndex = 1; }
                    if (nextIndex >= tempBuffer.Length) nextIndex = 0;

                    microphoneSample = Mathf.Lerp(tempBuffer[index], tempBuffer[nextIndex], resample - index);

                    resample += step * pitch;
                    if (resample >= tempBuffer.Length) resample = 0f;
                }

                microphoneBuffer[i] = microphoneSample * gain;
            }

            if (!PostProcessClip)
            {
                foreach (var postProcess in PostProcessors.Values)
                    postProcess?.Invoke(microphoneBuffer);
            }

            for (int i = 0; i < buffer.Length; i++)
            {
                float pushed = NextAudioClipSample();
                buffer[i] = Mathf.Clamp(microphoneBuffer[i] + pushed, -1f, 1f);
            }

            if (PostProcessClip)
            {
                foreach (var postProcess in PostProcessors.Values)
                    postProcess?.Invoke(buffer);
            }

            lastSamplePosition = (lastSamplePosition + samples) % microphoneClip.samples;
            return true;
        }


        /// <summary>
        /// Returns the next sample from the pushed audio buffer each time the Read() function is called.
        /// </summary>
        private float NextAudioClipSample()
        {
            if (audioClips.Count == 0)
                return 0f;

            float mixed = 0f;

            for (int i = audioClips.Count - 1; i >= 0; i--)
            {
                var clip = audioClips[i];

                int index = (int)clip.Position;

                if (index >= clip.Samples.Length)
                {
                    audioClips.RemoveAt(i);
                    continue;
                }

                int nextIndex = index + 1;
                if (nextIndex >= clip.Samples.Length)
                {
                    mixed += clip.Samples[index];
                    audioClips.RemoveAt(i);
                    continue;
                }

                float frac = clip.Position - index;
                mixed += Mathf.Lerp(clip.Samples[index], clip.Samples[nextIndex], frac);

                clip.Position += clip.Step;
            }

            return mixed;
        }

        public void Dispose()
        {
            StopRecording();
            Instance = null;
        }  
    }
}