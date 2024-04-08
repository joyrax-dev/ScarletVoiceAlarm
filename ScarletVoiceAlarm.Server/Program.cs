using KeyboardHookLite;
using NAudio.Wave;
using System.Net.Sockets;
using System.Windows.Input;

namespace ScarletVoiceAlarm.Server
{
    internal class Program
    {
        // Клавиша которая нажата в любой момент работы
        private static Key? PressedKey = null;

        // UDP Client
        private static UdpClient udpClient = new UdpClient();

        static void Main(string[] args)
        {
            // Загрузка конфигурации
            Console.WriteLine("Load Configuration...");
            Configuration.Load();

            // Выводим какие клавиши подгрузились
            KeyBindings[]? bindings = Configuration.Config?.KeyBindings;
            for (int i = 0; i < bindings?.Length; i++)
            {
                KeyBindings binding = bindings[i];
                Console.WriteLine("Key: " + binding.SystemKey);
            }
            Console.WriteLine("+++");

            // Объект для записи звука с микрофона
            WaveInEvent waveIn = new WaveInEvent();
            waveIn.DeviceNumber = 0;
            waveIn.DataAvailable += OnRecordData;
            waveIn.WaveFormat = new WaveFormat(44100, 16, 1);
            waveIn.StartRecording();
            
            // Объект для захвата событий с клавиатуры
            KeyboardHook keyHook = new KeyboardHook();
            keyHook.KeyboardPressed += OnKeyPress;
            System.Windows.Threading.Dispatcher.Run();
        }

        // Обработчик события записи звука с микрофона
        private static void OnRecordData(object? sender, WaveInEventArgs e)
        {
            // Берем все клавиши с конфигурации
            KeyBindings[]? bindings = Configuration.Config?.KeyBindings;
            for (int i = 0; i < bindings?.Length; i++)
            {
                KeyBindings binding = bindings[i];
                
                // Проверяем с текущей нажатой клавишей
                if (binding.SystemKey == PressedKey)
                {
                    // Берем локации с конфигурации
                    List<Locations> accessLocations = new List<Locations>();
                    Locations[]? locations = Configuration.Config?.Locations;

                    // Проходим каждую локацию и если какаято совпадает добавляем ее в список локаций для отправки
                    foreach (string loc in binding.Locations)
                    {
                        for (int i1 = 0; i1 < locations?.Length; i1++)
                        {
                            Locations location = locations[i1];

                            if (location.Name == loc)
                            {
                                accessLocations.Add(location);
                            }
                        }
                    }

                    // Берем список локаций готовых к отправке и отправляем записаный буфер аудио
                    foreach (Locations location in accessLocations)
                    {
                        int bytes = udpClient.Send(e.Buffer, location.EndPoint);
                    }
                }
            }
        }  

        private static void OnKeyPress(object? sender, KeyboardHookEventArgs e)
        {
            if (e.KeyPressType == KeyboardHook.KeyPressType.KeyDown)
            {
                PressedKey = e.InputEvent.Key;
            }
            else
            {
                PressedKey = null;
            }
            
        }
    }
}