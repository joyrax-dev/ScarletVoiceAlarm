using KeyboardHookLite;
using NAudio.Wave;
using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Windows.Input;

namespace ScarletVoiceAlarm.Server
{
    internal class Program
    {
        private static Key? PressedKey = null;

        static void Main(string[] args)
        {
            Console.WriteLine("Load Configuration...");
            Configuration.Load();

            KeyBindings[]? bindings = Configuration.Config?.KeyBindings;
            for (int i = 0; i < bindings?.Length; i++)
            {
                KeyBindings binding = bindings[i];
                Console.WriteLine("Key: " + binding.SystemKey);
            }
            Console.WriteLine("+++");

            WaveInEvent waveIn = new WaveInEvent();
            waveIn.DeviceNumber = 0;
            waveIn.DataAvailable += OnRecordData;
            waveIn.StartRecording();

            KeyboardHook kbh = new KeyboardHook();
            kbh.KeyboardPressed += OnKeyPress;
            System.Windows.Threading.Dispatcher.Run();

            Console.ReadLine();
        }

        private static void OnRecordData(object? sender, WaveInEventArgs e)
        {
            KeyBindings[]? bindings = Configuration.Config?.KeyBindings;
            for (int i = 0; i < bindings?.Length; i++)
            {
                KeyBindings binding = bindings[i];
                
                if (binding.SystemKey == PressedKey)
                {
                    Console.Write("Sended voice to [");
                    foreach (string loc in binding.Locations)
                    {
                        Console.Write(" " + loc + " ");
                    }
                    Console.Write("]\n");
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