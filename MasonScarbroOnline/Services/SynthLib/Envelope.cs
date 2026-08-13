namespace MasonScarbroOnline.Services.SynthLib
{
    public struct Envelope
    {
        /// <summary>time for volume to ramp from silent (0) up to full (1). The note "turning on."</summary>
        public double Attack;

        /// <summary>Decay is the time it takes to fall from that initial full volume down to the sustain level</summary>
        public double Decay;

        /// <summary>How loud the note stays while you're holding it down 0-1</summary>
        public double Sustain;

        /// <summary>Time for volume to ramp from sustain level down to silent (0). The note "releasing."</summary>
        public double Release;

        /// <summary>
        /// Returns a default envelope with Attack=0.01, Decay=0.1, Sustain=0.7, Release=0.3
        /// </summary>
        public static Envelope Default => new()
        {
            Attack = 0.01,
            Decay = 0.1,
            Sustain = 0.7,
            Release = 0.3
        };

        /// <summary>
        /// Generates an array of floats representing the envelope over time, given the total number of samples and the sample rate.
        /// </summary>
        /// <param name="totalSamples"></param>
        /// <param name="sampleRate"></param>
        /// <returns></returns>
        public readonly float[] Generate(int totalSamples, int sampleRate)
        {
            // We * everyhing by sampleRate to convert seconds to samples 
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
                env[i++] = 0f; // saftey net

            return env;
        }
    }
}
