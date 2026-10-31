using System;
using OpenTK.Graphics.OpenGL;
using OpenTK.Windowing.Common.Input;
using StbImageNext;

namespace FlappyBird
{
    internal class GameTextures
    {
        public static Image LoadImage(string path)
        {
            path = Path.Combine(AppContext.BaseDirectory, path);
            if (!File.Exists(path))
            {
                throw new FileNotFoundException($"Загружаемый файл не найден по пути {path}");
            }

            using var stream = File.OpenRead(path);
            var image = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);
            return new Image(image.Width, image.Height, image.Data);
        }

        static public int LoadTexture(string path)
        {
            var image = LoadImage(path);
            int id = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, id);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, image.Width, image.Height, 0,
                PixelFormat.Rgba, PixelType.UnsignedByte, image.Data);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, 
                (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, 
                (int)TextureMagFilter.Linear);

            return id;
        }
    }
}
