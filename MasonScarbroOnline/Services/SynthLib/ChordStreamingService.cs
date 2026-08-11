using System.Net.WebSockets;

namespace MasonScarbroOnline.Services.SynthLib
{
    public class ChordStreamingService
    {
        public async Task StreamToWebSocketAsync(
            WebSocket socket,
            Synthesizer synth,
            IEnumerable<double> frequencies,
            double durationSec,
            CancellationToken ct)
        {
            await foreach (var chunk in synth.StreamChordAsync(frequencies, durationSec, ct: ct))
            {
                var pcmBytes = ToPcm16(chunk);
                await socket.SendAsync(pcmBytes, WebSocketMessageType.Binary, endOfMessage: true, ct);
            }

            if (socket.State == WebSocketState.Open)
                await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "stream complete", ct);
        }

        static byte[] ToPcm16(float[] samples)
        {
            var bytes = new byte[samples.Length * 2];
            for (int i = 0; i < samples.Length; i++)
            {
                short val = (short)(Math.Clamp(samples[i], -1f, 1f) * short.MaxValue);
                bytes[i * 2] = (byte)(val & 0xFF);
                bytes[i * 2 + 1] = (byte)((val >> 8) & 0xFF);
            }
            return bytes;
        }
    }
}
