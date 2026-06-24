using System;
using System.Media;
using System.IO;
using System.Speech.Synthesis;

namespace CybersecurityBot
{
    public static class VoiceGreeting
    {
        public static void PlayGreeting()
        {
            string greetingMessage = "Hello! Welcome to Thabiso's Cybersecurity Awareness Bot. I'm here to help you stay safe online.";

            try
            {

                using (SpeechSynthesizer synthesizer = new SpeechSynthesizer())
                {
                    synthesizer.SetOutputToDefaultAudioDevice();
                    synthesizer.Speak(greetingMessage);
                }
            }
            catch (Exception ex)
            {
                // Method 2: Fallback to WAV file if TTS fails
                try
                {
                    string audioPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "greeting.wav");

                    if (File.Exists(audioPath))
                    {
                        using (SoundPlayer player = new SoundPlayer(audioPath))
                        {
                            player.PlaySync();
                        }
                    }
                    else
                    {
                        // Method 3: Text fallback if no audio
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("\n🔊 [Voice Greeting]: " + greetingMessage);
                        Console.ResetColor();
                        System.Threading.Thread.Sleep(3010);
                    }
                }
                catch
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("\n🔊 [Voice Greeting]: " + greetingMessage);
                    Console.ResetColor();
                    System.Threading.Thread.Sleep(3000);
                }
            }
        }
    }
}
