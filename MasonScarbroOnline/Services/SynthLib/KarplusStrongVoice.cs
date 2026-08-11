namespace MasonScarbroOnline.Services.SynthLib
{
    public class KarplusStrongVoice
    {
        readonly float[] buf;
        readonly int n;
        readonly double _decay;
        readonly double _brightness;
        int p = 0;
        int samplesEmitted = 0;

        public int StartSample { get; set; } = 0;
        public int TotalSamples { get; set; } = 0;

        public KarplusStrongVoice(double frequency, int sampleRate, double decay, double brightness,
                                   int startSample, int totalSamples, Random rng)
        {
            n = Math.Max(2, (int)Math.Round(sampleRate / frequency));
            buf = new float[n];
            rng ??= new Random();
            for (int i = 0; i < n; i++)
            {
                buf[i] = (float)(rng.NextDouble() * 2 - 1);
            }
            _decay = decay;
            _brightness = brightness;
            StartSample = startSample;
            TotalSamples = totalSamples;
        }

        public float Next(int globalSampleIndex)
        {
            if (globalSampleIndex < StartSample || samplesEmitted >= TotalSamples)
                return 0f;

            float output = buf[p];
            int next = (p + 1) % n;
            buf[p] = (float)(_decay * (_brightness * buf[p] + (1 - _brightness) * buf[next]));
            p = next;
            samplesEmitted++;
            return output;
        }

    }
}
