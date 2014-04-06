using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace journalism_thingie
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game : Microsoft.Xna.Framework.Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private int screenW, screenH;
        private Citizen[] population = new Citizen[100];
        private News crrtNews;
        private KeyboardState keyPrevious;

        //Textures and their rectangles
        private Texture2D background, notepad, tvBackground;
        private Rectangle backgroundRect, notepadRect, tvRect;
        private SpriteFont font;
        private TextAreaChoice notepadChoice;
        private SubtitleArea tvSpeech;

        // Game State - we need to keep track of where we are to know what to draw and what input to receive
        public byte gameState;
        public const byte NOTEPAD = 1;//the part where you make your choice
        public const byte FOCUS_GROUP = 2;//view info about the focus group
        public const byte WATCH_NEWS = 3;//watch your decision unfold on tv

        public Game()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            screenW = this.graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            screenH = this.graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            this.graphics.IsFullScreen = true;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            this.IsMouseVisible = true;
            spriteBatch = new SpriteBatch(GraphicsDevice);
            base.Initialize();
            gameState = NOTEPAD;
            keyPrevious = Keyboard.GetState();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            notepad = Content.Load<Texture2D>("blank-notepad");
            notepadRect = new Rectangle((int)(screenW * 0.2), (int)(screenH * 0.1), (int)(screenW * 0.6), (int)(screenH * 0.8));
            background = Content.Load<Texture2D>("background");
            backgroundRect = new Rectangle(0, 0, screenW, screenH);
            tvBackground = Content.Load<Texture2D>("tv-pdf");
            tvRect = new Rectangle(0, 0, screenW, screenH);
            font = Content.Load<SpriteFont>("SpriteFont1");
            
            int i;
            uint nrmin = 0, nrmed = 0, nrmax = 0;
            Console.WriteLine("#POOR#");
            for (i = 0; i < 40; i++)
            {
                population[i] = new Citizen(Citizen.POOR, 0);
                Console.WriteLine(population[i].ideology + " " + population[i].fanaticism);
                if (population[i].fanaticism < 0.1) nrmin++;
                if (population[i].fanaticism > 0.45 && population[i].fanaticism < 0.55) nrmed++;
                if (population[i].fanaticism > 0.9) nrmax++;
            }
            Console.WriteLine("#MIDDLE#");
            for (i = 40; i < 90; i++)
            {
                population[i] = new Citizen(Citizen.MIDDLE, 0);
                Console.WriteLine(population[i].ideology + " " + population[i].fanaticism);
                if (population[i].fanaticism < 0.1) nrmin++;
                if (population[i].fanaticism > 0.45 && population[i].fanaticism < 0.55) nrmed++;
                if (population[i].fanaticism > 0.9) nrmax++;
            }
            Console.WriteLine("#RICH#");
            for (i = 90; i < 100; i++)
            {
                population[i] = new Citizen(Citizen.RICH, 0);
                Console.WriteLine(population[i].ideology + " " + population[i].fanaticism);
                if (population[i].fanaticism < 0.1) nrmin++;
                if (population[i].fanaticism > 0.45 && population[i].fanaticism < 0.55) nrmed++;
                if (population[i].fanaticism > 0.9) nrmax++;
            }
            Console.WriteLine("###\nOverly apathetic: " + nrmin);
            Console.WriteLine("Middle ground: " + nrmed);
            Console.WriteLine("Overly fanatic: " + nrmax);

            crrtNews = new News("news.txt");
            String[] choices = new String[crrtNews.options.Length];
            int j = 0;
            foreach (Option o in crrtNews.options)
            {
                choices[j++] = o.description;
            }
            notepadChoice = new TextAreaChoice(GraphicsDevice, spriteBatch, font, (int)(screenW * 0.3), (int)(screenH * 0.2), (int)(screenW * 0.4), (int)(screenH * 0.6), crrtNews.situationDescription, choices);
            tvSpeech = new SubtitleArea(GraphicsDevice, spriteBatch, font, (int)(screenW * 0.1), (int)(screenH * 0.6), (int)(screenW * 0.8), (int)(screenH * 0.1));
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            KeyboardState keyCurrent = Keyboard.GetState();
            MouseState mouseStateCurrent = Mouse.GetState();
            // Allows the game to exit
            if (keyCurrent.IsKeyDown(Keys.Escape))
                this.Exit();

            switch (gameState)
            {
                case NOTEPAD:
                {
                    int ret = notepadChoice.update(mouseStateCurrent);
                    if (ret != -1)
                    {
                        gameState = WATCH_NEWS;
                        tvSpeech.setText(crrtNews.options[ret].newsArticle);
                    }
                }
                break;
                case WATCH_NEWS:
                {
                    if (tvSpeech.update(keyCurrent, keyPrevious) == 1)
                        gameState = NOTEPAD;
                }
                break;
            }
            keyPrevious = keyCurrent;
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            switch(gameState){
            case NOTEPAD: {
                notepadChoice.draw();
                spriteBatch.Begin();
                spriteBatch.Draw(background, backgroundRect, Color.White);
                spriteBatch.Draw(notepad, notepadRect, Color.White);
                spriteBatch.Draw(notepadChoice.textArea, notepadChoice.textAreaRect, Color.White);
                spriteBatch.End();
            }
            break;
            case WATCH_NEWS:
            {
                tvSpeech.draw();
                spriteBatch.Begin();
                spriteBatch.Draw(tvBackground, tvRect, Color.White);
                spriteBatch.Draw(tvSpeech.textArea, tvSpeech.textAreaRect, Color.White);
                spriteBatch.End();
            }
            break;
            }
        }
    }
}
