using System;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.Common;
using System.Drawing;
using OpenTK.Windowing.Common.Input;

namespace FlappyBird
{
    class Program
    {
        static void Main()
        {
            Bitmap image = new Bitmap("Textures/icon.png");

            OpenTK.Windowing.Common.Input.Image icon = new OpenTK.Windowing.Common.Input.Image(image.Width, image.Height, GetPixels(image));

            GameWindowSettings gSettings = new GameWindowSettings() 
            { 
                UpdateFrequency = 60.0,
            };
            NativeWindowSettings nSettings = new NativeWindowSettings()
            {
                Title = "Flappy Bird",
                Size = (800, 600),
                Flags = ContextFlags.Default,
                Profile = ContextProfile.Compatability,
                Icon = new WindowIcon(icon),
            };

            Game game = new Game(gSettings, nSettings);
            game.Run();
        }

        static byte[] GetPixels(Bitmap image)
        {

            byte[] pixels = new byte[image.Width * image.Height * 4];

            int index = 0;

            for (int i = 0; i < image.Height; i++)
            {
                for (int j = 0; j < image.Width; j++)
                {
                    pixels[index] = image.GetPixel(j, i).R;
                    index++;
                    pixels[index] = image.GetPixel(j, i).G;
                    index++;
                    pixels[index] = image.GetPixel(j, i).B;
                    index++;
                    pixels[index] = image.GetPixel(j, i).A;
                    index++;
                }
            }

            return pixels;
        }
    }
}