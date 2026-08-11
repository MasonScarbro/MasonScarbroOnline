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
            var samples = RenderOscillator(freq, totalSamples);
            var env = Envelope.Generate(totalSamples, SampleRate);
            for (int i = 0; i < totalSamples; i++)
                samples[i] *= env[i];
            return samples;
        }

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
