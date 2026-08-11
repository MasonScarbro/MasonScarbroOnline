namespace MasonScarbroOnline.Services.SynthLib
{
    public class NoteUtils
    {
        static readonly Dictionary<string, int> semitoneFromC = new()
        {
            { "C", 0 }, { "C#", 1 }, { "Db", 1 }, { "D", 2 }, { "D#", 3 }, { "Eb", 3 },
            { "E", 4 }, { "F", 5 }, { "F#", 6 }, { "Gb", 6 }, { "G", 7 }, { "G#", 8 },
            { "Ab", 8 }, { "A", 9 }, { "A#", 10 }, { "Bb", 10 }, { "B", 11 }
        };

        public static double Frequency(string note)
        {
            int splitIndex = note.Length - 1;
            while (splitIndex > 0 && char.IsDigit(note[splitIndex - 1]))
                splitIndex--;

            string name = note.Substring(0, splitIndex);
            int octave = int.Parse(note.Substring(splitIndex));

            if (!semitoneFromC.TryGetValue(name, out int semitone))
                throw new ArgumentException($"Unrecognized note name: {name}");

            int midi = (octave + 1) * 12 + semitone; // A4 = midi 69
            return 440.0 * Math.Pow(2.0, (midi - 69) / 12.0);
        }
    }
}
