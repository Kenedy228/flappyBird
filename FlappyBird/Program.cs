using System;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.Common;

namespace FlappyBird
{
    class Program
    {
        static void Main()
        {
            GameWindowSettings gSettings = new GameWindowSettings();
            NativeWindowSettings nSettings = new NativeWindowSettings()
            {
                Title = "Flappy Bird",
                Size = (800, 600),
                Flags = ContextFlags.Default,
                Profile = ContextProfile.Compatability,
            };

            Game game = new Game(gSettings, nSettings);
            game.Run();
        }
    }
}