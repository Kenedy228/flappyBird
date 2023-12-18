using System;
using OpenTK.Mathematics;

namespace FlappyBird
{
    internal class Menu : TextureDrawing
    {
        private float[,] playCoordinates = { 
            { -0.12f, 0.12f, 0.12f, -0.12f }, 
            { 0.05f, 0.05f, -0.05f, -0.05f } 
        };

        private float[,] settingsCoordinates = { 
            { -0.12f, 0.12f, 0.12f, -0.12f},
            {-0.07f, -0.07f, -0.17f, -0.17f }
        };

        private float[,] exitCoordinates = { 
            {-0.12f, 0.12f, 0.12f, -0.12f },
            { -0.19f, -0.19f, -0.29f, -0.29f}
        };

        private float[,] birdColorCoordinates =
        {
            {-0.20f, 0.20f, 0.20f, -0.20f },
            { 0.05f, 0.05f, -0.05f, -0.05f }
        };

        private float[,] pipeColorCoordinates =
        {
            {-0.20f, 0.20f, 0.20f, -0.20f },
            { -0.07f, -0.07f, -0.17f, -0.17f }
        };

        private float[,] backCoordinates =
        {
            {-0.20f, 0.20f, 0.20f, -0.20f },
            { -0.19f, -0.19f, -0.29f, -0.29f }
        };

        private float[,] restartCoordinates =
        {
            {-0.20f, 0.20f, 0.20f, -0.20f },
            { 0.05f, 0.05f, -0.05f, -0.05f }
        };

        private float[,] menuCoordinates =
        {
            {-0.20f, 0.20f, 0.20f, -0.20f },
            { -0.07f, -0.07f, -0.17f, -0.17f }
        };

        public int gameStatus = 0;

        public int birdColorCounter = 0, pipeColorCounter = 0;
        public bool restart = false;

        public void DrawMenu(int[] textureIds, string[] buttonNames)
        {
            for (int i = 0; i < textureIds.Length; i++)
            {
                Bind(textureIds[i]);

                float[,] coordinates = new float[,] { };

                switch (buttonNames[i])
                {
                    case "play":
                        coordinates = playCoordinates;
                        break;
                    case "settings":
                        coordinates = settingsCoordinates;
                        break;
                    case "exit":
                        coordinates = exitCoordinates;
                        break;
                    case "birdColor":
                        coordinates = birdColorCoordinates;
                        break;
                    case "pipeColor":
                        coordinates = pipeColorCoordinates;
                        break;
                    case "back":
                        coordinates = backCoordinates;
                        break;
                    case "restart":
                        coordinates = restartCoordinates;
                        break;
                    case "menu":
                        coordinates = menuCoordinates;
                        break;
                }

                base.Draw(
                    new float[,] { { 0f, 1f, 1f, 0f }, { 0f, 0f, 1f, 1f } },
                    coordinates
                );
            }
        }

        public void MouseClickHandler(Vector2 cursorPosition)
        {
            if (gameStatus == 0)
            {
                if (IsButton(cursorPosition, playCoordinates)) gameStatus = 2;
                else if (IsButton(cursorPosition, settingsCoordinates)) gameStatus = 1;
                else if (IsButton(cursorPosition, exitCoordinates)) gameStatus = -1;
            } else if (gameStatus == 1)
            {
                if (IsButton(cursorPosition, birdColorCoordinates))
                {
                    birdColorCounter++;
                    if (birdColorCounter == 3) birdColorCounter = 0;
                }
                else if (IsButton(cursorPosition, pipeColorCoordinates))
                {
                    pipeColorCounter++;
                    if (pipeColorCounter == 2) pipeColorCounter = 0;
                }
                else if (IsButton(cursorPosition, backCoordinates)) gameStatus = 0;
            }else if (gameStatus == 3)
            {
                if (IsButton(cursorPosition, restartCoordinates))
                {
                    gameStatus = 2;
                    restart = true;
                }
                else if (IsButton(cursorPosition, menuCoordinates))
                {
                    gameStatus = 0;
                    restart = true;
                }
            }
        }
    }
}