namespace MasonScarbroOnline.Services.SynthLib
{
    public static class ChordLibrary
    {
        public static readonly Dictionary<string, string[]> Chords = new()
        {
            ["F"] = ["F3", "C4", "F4", "A4", "C5", "F5"],
            ["C"] = ["C3", "E3", "G3", "C4", "E4", "G4"],
            ["G"] = ["G3", "B3", "D4", "G4", "B4", "G5"],
        };
    }
}
