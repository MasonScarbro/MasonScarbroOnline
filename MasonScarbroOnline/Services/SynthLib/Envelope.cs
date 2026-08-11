namespace MasonScarbroOnline.Services.SynthLib
{
    public struct Envelope
    {
        public double Attack;   // seconds
        public double Decay;    // seconds
        public double Sustain;  // sustain level, 0-1
        public double Release;  // seconds
        public static Envelope Default => new()
        {
            Attack = 0.01,
            Decay = 0.1,
            Sustain = 0.7,
            Release = 0.3
        };
        
        public float[] Generate(int totalSamples, int sampleRate)
        {
            float[] env = new float[totalSamples];
            int attackSamples = (int)(Attack * sampleRate);
            int decaySamples = (int)(Decay * sampleRate);
            int releaseSamples = (int)(Release * sampleRate);
            int sustainSamples = Math.Max(0, totalSamples - attackSamples - decaySamples - releaseSamples);

            int i = 0;
            for (int a = 0; a < attackSamples && i < totalSamples; a++, i++)
                env[i] = (float)a / attackSamples;
            for (int d = 0; d < decaySamples && i < totalSamples; d++, i++)
                env[i] = (float)(1.0 - (1.0 - Sustain) * d / decaySamples);
            for (int s = 0; s < sustainSamples && i < totalSamples; s++, i++)
                env[i] = (float)Sustain;
            for (int r = 0; r < releaseSamples && i < totalSamples; r++, i++)
                env[i] = (float)(Sustain * (1.0 - (double)r / releaseSamples));
            while (i < totalSamples)
                env[i++] = 0f;

            return env;
        }
    }
}
