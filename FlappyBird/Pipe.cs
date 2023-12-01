using System;
using OpenTK.Graphics.OpenGL;

namespace FlappyBird
{
    internal class Pipe : Object
    {
        private Random rnd = new Random();

        public float[] xCoordinates;
        public float[] yCoordinatesUp = new float[] {-10f, -10f, -1f, -1f};
        public float[] yCoordinatesDown = new float[] { 10f, 10f, 1f, 1f };

        private bool flag = false;
        public bool gameFinish = false;

        public Pipe(float[] xCoordinates)
        {
            this.xCoordinates = xCoordinates;
            ChangeY();
        }

        public void Draw(int textureId, float[] birdX, float[] birdY, Score score, bool startGame)
        {
            Bind(textureId);

            Draw(
                new float[,] { { 0f, 1f, 1f, 0f }, { 0f, 0f, 1f, 1f } },
                new float[,] { { xCoordinates[0], xCoordinates[1], xCoordinates[2], xCoordinates[3], },
                    {yCoordinatesUp[0], yCoordinatesUp[1], yCoordinatesUp[2], yCoordinatesUp[3] } }
                );

            Draw(
                new float[,] { { 0f, 1f, 1f, 0f }, { 0f, 0f, 1f, 1f } },
                new float[,] { { xCoordinates[0], xCoordinates[1], xCoordinates[2], xCoordinates[3], },
                    {yCoordinatesDown[0], yCoordinatesDown[1], yCoordinatesDown[2], yCoordinatesDown[3] } }
                );

            if (startGame) Move(birdX, birdY, score);
        }

        public void Move(float[] birdX, float[] birdY, Score score)
        {
            if (xCoordinates[1] < -1f)
            {
                ChangeX();
                ChangeY();
                flag = false;
            }

            float distance = 0.03f;

            if (!flag)
            {
                if (birdX[0] > xCoordinates[1] && yCoordinatesUp[0] < birdY[2] && yCoordinatesDown[0] > birdY[0])
                {
                    flag = true;
                    score.IncrementCounter();
                }
                else
                {
                    if (birdX[0] + distance > xCoordinates[0] && birdX[1] - distance < xCoordinates[1])
                    {
                        if (birdY[0] - distance > yCoordinatesDown[0] || birdY[2] + distance < yCoordinatesUp[0])
                        {
                            gameFinish = true;
                        }
                    } else if (birdX[0] + distance > xCoordinates[0] && birdX[0] + distance < xCoordinates[1] && birdX[1] - distance > xCoordinates[1])
                    {
                        if (birdY[0] - distance > yCoordinatesDown[0] || birdY[2] + distance < yCoordinatesUp[0])
                        {
                            gameFinish = true;
                        }
                    } else if (birdX[1] - distance > xCoordinates[0] && birdX[1] - distance < xCoordinates[1] && birdX[0] + distance < xCoordinates[0])
                    {
                        if (birdY[0] - distance > yCoordinatesDown[0] || birdY[2] + distance < yCoordinatesUp[0])
                        {
                            gameFinish = true;
                        }
                    }
                }
            }

            for (int i = 0; i < xCoordinates.Length; i++)
            {
                xCoordinates[i] -= 0.0015f;
            }
        }

        public void ChangeX()
        {
            xCoordinates[0] = 1.0f;
            xCoordinates[1] = 1.2f;
            xCoordinates[2] = 1.2f;
            xCoordinates[3] = 1.0f;
        }

        public void ChangeY()
        {
            int number = rnd.Next(-5, 4);
            float y = (-1 * number) % 10 * -0.1f;

            yCoordinatesUp[2] = (2f - y) * -1;
            yCoordinatesUp[3] = (2f - y) * -1;

            yCoordinatesDown[2] = 2f + (y + 0.5f);
            yCoordinatesDown[3] = 2f + (y + 0.5f);

            yCoordinatesUp[0] = y;
            yCoordinatesUp[1] = y;

            yCoordinatesDown[0] = y + 0.5f;
            yCoordinatesDown[1] = y + 0.5f;
        }
    }
}
