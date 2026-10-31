using System;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Common.Input;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace FlappyBird
{
    class Program
    {
        static void Main()
        {
            var icon = GameTextures.LoadImage("Textures/icon.png");

            GameWindowSettings gSettings = new GameWindowSettings() 
            { 
                UpdateFrequency = 60.0,
            };
            NativeWindowSettings nSettings = new NativeWindowSettings()
            {
                Title = "Flappy Bird",
                ClientSize = (800, 600),
                Flags = ContextFlags.Default,
                Profile = ContextProfile.Compatability,
            };

            // macOS exposes fixed-function drawing through its legacy 2.1 context.
            // Its 3.2+ core contexts do not support the immediate-mode renderer.
            if (OperatingSystem.IsMacOS())
            {
                nSettings.APIVersion = new Version(2, 1);
                nSettings.Profile = ContextProfile.Any;
            }

            // Wayland has no API for setting an individual window's icon.
            if (GLFW.GetPlatform() != Platform.Wayland)
                nSettings.Icon = new WindowIcon(icon);

            Game game = new Game(gSettings, nSettings);
            game.Run();
        }

    }
}
