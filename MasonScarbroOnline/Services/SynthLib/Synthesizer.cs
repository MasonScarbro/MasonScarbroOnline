namespace MasonScarbroOnline.Services.SynthLib
{
    public class Synthesizer
    {
        public int SampleRate { get; set; } = 44100;
        public WaveForm WaveForm { get; set; } = WaveForm.Pluck;
        public Envelope Envelope { get; set; } = Envelope.Default;

        public double Decay { get; set; } = 0.996;
        public double Brightness { get; set; } = 0.5;

        readonly Random rng = new();

        public float[] RenderNote(double freq, double duration)
        {
            int totalSamples = (int)(duration * SampleRate);
            
            if (WaveForm == WaveForm.Pluck)
            {
                return RenderPluck(freq, totalSamples);
            }
            // else, render oscillator and apply envelope
            var samples = RenderOscillator(freq, totalSamples);
            var env = Envelope.Generate(totalSamples, SampleRate);
            for (int i = 0; i < totalSamples; i++)
                samples[i] *= env[i];
            return samples;
        }

        /// <summary>
        /// Renders a plucked string sound using the Karplus-Strong algorithm.
        /// </summary>
        /// <param name="freq"></param>
        /// <param name="totalSamples"></param>
        /// <returns></returns>
        public float[] RenderPluck(double freq, int totalSamples)
        {
            int n = Math.Max(2, (int)Math.Round(SampleRate / freq));
            var buf = new float[n];
            for (int i = 0; i < n; i++)
                buf[i] = (float)(rng.NextDouble() * 2 - 1);

            var output = new float[totalSamples];
            int p = 0;
            for (int i = 0; i < totalSamples; i++)
            {
                output[i] = buf[p];
                int next = (p + 1) % n;
                buf[p] = (float)(Decay * (Brightness * buf[p] + (1 - Brightness) * buf[next]));
                p = next;
            }
            return output;
        }

        /// <summary>
        /// Renders a simple oscillator waveform (sine, square, sawtooth, triangle) at the given frequency and sample count.
        /// </summary>
        /// <param name="freq"></param>
        /// <param name="totalSamples"></param>
        /// <returns></returns>
        public float[] RenderOscillator(double freq, int totalSamples)
        {
            var output = new float[totalSamples];
            double phaseIncrement = freq / SampleRate;
            double phase = 0;

            for (int i = 0; i < totalSamples; i++)
            {
                output[i] = WaveForm switch
                {
                    WaveForm.Sine => (float)Math.Sin(2 * Math.PI * phase),
                    WaveForm.Square => phase < 0.5 ? 1f : -1f,
                    WaveForm.Sawtooth => (float)(2.0 * (phase - Math.Floor(phase + 0.5))),
                    WaveForm.Triangle => (float)(2.0 * Math.Abs(2.0 * (phase - Math.Floor(phase + 0.5))) - 1.0),
                    _ => 0f
                };
                phase += phaseIncrement;
                if (phase >= 1.0) phase -= 1.0;
            }
            return output;
        }

        /// <summary>
        /// Streams a chord of plucked string sounds using the Karplus-Strong algorithm, with optional strumming stagger.
        /// </summary>
        /// <param name="frequencies"></param>
        /// <param name="durationSec"></param>
        /// <param name="strumStaggerSec"></param>
        /// <param name="chunkSamples"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public async IAsyncEnumerable<float[]> StreamChordAsync(
            IEnumerable<double> frequencies,
            double durationSec,
            double strumStaggerSec = 0.015,
            int chunkSamples = 4096,
            [System.Runtime.CompilerServices.EnumeratorCancellation] System.Threading.CancellationToken ct = default)
        {
            var freqList = new List<double>(frequencies);
            int totalSamples = (int)(durationSec * SampleRate);

            var voices = new List<KarplusStrongVoice>();
            for (int i = 0; i < freqList.Count; i++)
            {
                int start = (int)(i * strumStaggerSec * SampleRate);
                int dur = Math.Max(0, totalSamples - start);
                voices.Add(new KarplusStrongVoice(freqList[i], SampleRate, Decay, Brightness, start, dur, rng));
            }

            for (int chunkStart = 0; chunkStart < totalSamples; chunkStart += chunkSamples)
            {
                ct.ThrowIfCancellationRequested();
                int len = Math.Min(chunkSamples, totalSamples - chunkStart);
                var chunk = new float[len];

                for (int i = 0; i < len; i++)
                {
                    int globalIdx = chunkStart + i;
                    float sum = 0f;
                    foreach (var v in voices)
                        sum += v.Next(globalIdx) * 0.5f;
                    chunk[i] = Math.Clamp(sum, -1f, 1f);
                }

                yield return chunk;
                await Task.Yield(); 
            }
        }

        public float[] RenderChord(IEnumerable<double> frequencies, double durationSec, double strumStaggerSec = 0.0)
        {
            var freqList = new List<double>(frequencies);
            int totalSamples = (int)(durationSec * SampleRate);
            var mix = new float[totalSamples];

            for (int i = 0; i < freqList.Count; i++)
            {
                int offset = (int)(i * strumStaggerSec * SampleRate);
                double dur = durationSec - i * strumStaggerSec;
                if (dur <= 0) continue;
                var note = RenderNote(freqList[i], dur);

                for (int n = 0; n < note.Length; n++)
                {
                    int idx = n + offset;
                    if (idx < totalSamples)
                        mix[idx] += note[n] * 0.5f;
                }
            }

            Normalize(mix, 0.85f);
            return mix;
        }
        static void Normalize(float[] samples, float targetPeak)
        {
            float maxAbs = 0f;
            foreach (var s in samples)
                maxAbs = Math.Max(maxAbs, Math.Abs(s));
            if (maxAbs <= 0f) return;
            float norm = targetPeak / maxAbs;
            for (int i = 0; i < samples.Length; i++)
                samples[i] *= norm;
        }
    }
}
