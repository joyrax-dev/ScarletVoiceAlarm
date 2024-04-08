using System.Net;
using System.Net.Sockets;
using System.Text;
using NAudio.Dsp;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace ScarletVoiceAlarm.Client
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using UdpClient udpServer = new UdpClient(5051);
            Console.WriteLine("UDP-сервер запущен...");
            WaveOutEvent outEvent = new WaveOutEvent();
            BufferedWaveProvider waveBuffer = new BufferedWaveProvider(new WaveFormat(44100, 16, 1));
            outEvent.Init(waveBuffer);
            outEvent.Play();

            while (true)
            {
                IPEndPoint? point = null;
                var buffer = udpServer.Receive(ref point);

                waveBuffer.AddSamples(buffer, 0, buffer.Length);

                Console.WriteLine($"Получено {buffer.Length} байт");
                Console.WriteLine($"Удаленный адрес: {point}");
            }
        }
    }
}