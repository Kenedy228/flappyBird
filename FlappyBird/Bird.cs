using System;
using System.Diagnostics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace FlappyBird
{
    internal class Bird : Object
    {
        public float[] xCoordinates = new float[] { -0.05f, 0.05f, 0.05f, -0.05f };
        public float[] yCoordinates = new float[] { 0.7f, 0.7f, 0.55f, 0.55f };
        public float[] yJumpCoordinates = new float[] { 0.7f, 0.7f, 0.55f, 0.55f };
        public bool gameFinish = false;

        private int rows = 1,
            columns = 3, 
            choosenAnimationFrameCount = 3,
            choosenAnimationFrameNumber = 0,
            choosenAnimationRowNumber = 0,
            frameDelay = 0;

        private float frameWidth, frameHeight;

        public Bird()
        {
            frameWidth = 1.0f / columns;
            frameHeight = 1.0f / rows;
        }

        public void DrawBird(int birdId, bool startGame)
        {
            base.Bind(birdId);

            float x = choosenAnimationFrameNumber * frameWidth;
            float y = choosenAnimationRowNumber * frameHeight;

            base.Draw(
                new float[,] { { x, x + frameWidth, x + frameWidth, x }, { y, y, y + frameHeight, y + frameHeight } },
                new float[,]
                {
                    { xCoordinates[0], xCoordinates[1], xCoordinates[2], xCoordinates[3] },
                    { yCoordinates[0], yCoordinates[1], yCoordinates[2], yCoordinates[3]}
                }
            );

            frameDelay++;

            if (frameDelay > 30 && startGame)
            {
                frameDelay = 0;
                choosenAnimationFrameNumber++;
                choosenAnimationFrameNumber %= choosenAnimationFrameCount;
            }
            
            if (startGame)
            {
                if (yJumpCoordinates[0] > yCoordinates[0]) Jump();
                else
                {
                    Fall();
                }
            }
        }

        public void Jump()
        {
            for (int i = 0; i < yCoordinates.Length; i++) yCoordinates[i] += 0.01f;
        }

        public void JumpHandler()
        {
            for (int i = 0; i < yJumpCoordinates.Length; i++) yJumpCoordinates[i] += 0.3f;

            if (yJumpCoordinates[0] > 1)
            {
                yJumpCoordinates[0] = 1.0f;
                yJumpCoordinates[1] = 1.0f;
                yJumpCoordinates[2] = 0.85f;
                yJumpCoordinates[3] = 0.85f;
            }
        }

        public void Fall()
        {
            for (int i = 0; i < yCoordinates.Length; i++)
            {
                yJumpCoordinates[i] -= 0.0015f;
                yCoordinates[i] -= 0.0015f;
            }

            if (yCoordinates[2] < -1.0f) gameFinish = true;
        }
    }
}
