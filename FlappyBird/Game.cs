using System;
using System.Collections.Generic;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace FlappyBird
{
    class Game : GameWindow
    {
        private int backgroundId, scoreId, playButtonId,
            settingsButtonId, exitButtonId, backId,
            menuId, restartId;

        private List<int> birds = new List<int>();
        private List<int> pipesTextures = new List<int>();
        private List<int> birdColors = new List<int>();
        private List<int> pipeColors = new List<int>();


        private Bird bird = new Bird();
        private Score score = new Score();
        private List<Pipe> pipes = new List<Pipe>{
            new Pipe(new float[] { 1f, 1.2f, 1.2f, 1f }),
            new Pipe(new float[] { 2f, 2.2f, 2.2f, 2f }),
        };
        private Background background = new Background();
        private Menu menu = new Menu();

        private Vector2 cursorPosition = new Vector2();

        public Game(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeSettings)
            : base(gameWindowSettings, nativeSettings)
        {
            birds.Add(GameTextures.LoadTexture("Textures/yellowbird.png"));
            birds.Add(GameTextures.LoadTexture("Textures/bluebird.png"));
            birds.Add(GameTextures.LoadTexture("Textures/redbird.png"));

            pipesTextures.Add(GameTextures.LoadTexture("Textures/pipe-green.png"));
            pipesTextures.Add(GameTextures.LoadTexture("Textures/pipe-red.png"));

            birdColors.Add(GameTextures.LoadTexture("Textures/yellowbirdcoloroption.png"));
            birdColors.Add(GameTextures.LoadTexture("Textures/bluebirdcoloroption.png"));
            birdColors.Add(GameTextures.LoadTexture("Textures/redbirdcoloroption.png"));

            pipeColors.Add(GameTextures.LoadTexture("Textures/greenpipeoption.png"));
            pipeColors.Add(GameTextures.LoadTexture("Textures/redpipeoption.png"));

            backgroundId = GameTextures.LoadTexture("Textures/background.jpeg");
            scoreId = GameTextures.LoadTexture("Textures/numbers.png");
            playButtonId = GameTextures.LoadTexture("Textures/playbutton.png");
            settingsButtonId = GameTextures.LoadTexture("Textures/settingsbutton.png");
            exitButtonId = GameTextures.LoadTexture("Textures/exitbutton.png");
            backId = GameTextures.LoadTexture("Textures/backoption.png");
            menuId = GameTextures.LoadTexture("Textures/menu.png");
            restartId = GameTextures.LoadTexture("Textures/restart.png");
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);

            if (KeyboardState.IsKeyDown(Keys.Escape))
            {
                Close();
            }
        }

        protected override void OnKeyUp(KeyboardKeyEventArgs e)
        {
            base.OnKeyUp(e);

            if (e.Key == Keys.Space)
            {
                bird.JumpHandler();
            }
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);

            if (e.Button == MouseButton.Button1)
            {
                menu.MouseClickHandler(cursorPosition);
            }

        }

        protected override void OnMouseMove(MouseMoveEventArgs e)
        {
            base.OnMouseMove(e);

            cursorPosition.X = 2 * e.Position.X / ClientSize.X - 1.0f;
            cursorPosition.Y = -(2 * e.Position.Y / ClientSize.Y - 1.0f);
        }

        protected override void OnLoad()
        {
            base.OnLoad();

            GL.Enable(EnableCap.Texture2D);
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);

            GL.Viewport(0, 0, e.Width, e.Height);
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);

            GL.Clear(ClearBufferMask.ColorBufferBit);

            background.DrawBackground(backgroundId);

            switch (menu.gameStatus)
            {
                case 0:
                    if (menu.restart) Clear();
                    menu.DrawMenu(
                        new int[] { playButtonId, settingsButtonId, exitButtonId },
                        new string[] { "play", "settings", "exit" }
                    );

                    bird.DrawBird(birds[menu.birdColorCounter], false);
                    break;
                case 1:
                    menu.DrawMenu(
                        new int[] { birdColors[menu.birdColorCounter], pipeColors[menu.pipeColorCounter], backId },
                        new string[] { "birdColor", "pipeColor", "back" }
                    );

                    bird.DrawBird(birds[menu.birdColorCounter], false);
                    break;
                case 2:
                    if (menu.restart) Clear();

                    for (int i = 0; i < pipes.Count; i++)
                    {
                        if (pipes[i].gameFinish == true) menu.gameStatus = 3;
                        pipes[i].Draw(pipesTextures[menu.pipeColorCounter], bird.xCoordinates, bird.yCoordinates, score, true);
                    }

                    if (bird.gameFinish == true) menu.gameStatus = 3;

                    bird.DrawBird(birds[menu.birdColorCounter], true);
                    score.DrawScore(scoreId);
                    break;
                case 3:

                    bird.DrawBird(birds[menu.birdColorCounter], false);

                    for (int i = 0; i < pipes.Count; i++)
                    {
                        pipes[i].Draw(pipesTextures[menu.pipeColorCounter], bird.xCoordinates, bird.yCoordinates, score, false);
                    }

                    score.DrawScore(scoreId);

                    menu.DrawMenu(
                        new int[] { restartId, menuId },
                        new string[] { "restart", "menu" }
                    );

                    break;
                case -1:
                    Close();
                    break;
            }

            SwapBuffers();

        }

        protected void Clear()
        {
            score = new Score();
            bird = new Bird();

            pipes = new List<Pipe>{
                new Pipe(new float[] { 1f, 1.2f, 1.2f, 1f }),
                new Pipe(new float[] { 2f, 2.2f, 2.2f, 2f }),
            };

            menu.restart = false;
        }
    }
}
