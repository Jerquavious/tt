using System;

namespace HandHeldTemplate
{
    internal class PluginInfo
    {
        public string Name { get; set; } = "NixxQuestClient";
        public string Version { get; set; } = "1.0.0";
        public string Author { get; set; } = "54nixx54";
        public string Description { get; set; } = "Made For Apk Method Games";

        public PluginInfo()
        {
        }

        public void DisplayInfo()
        {
            Console.WriteLine($"Name: {Name}");
            Console.WriteLine($"Version: {Version}");
            Console.WriteLine($"Author: {Author}");
            Console.WriteLine($"Description: {Description}");
        }
    }
}
