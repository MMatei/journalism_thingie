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
    public partial class Game : Microsoft.Xna.Framework.Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private int screenW, screenH;
        private Citizen[] population = new Citizen[100];
        private FocusGroupView focusGroupView;
        private News crrtNews;
        public static MouseState mousePrevious;
        private KeyboardState keyPrevious;

        //Textures and their rectangles
        private RenderTarget2D focusGroupViewSnapshot, notepadSnapshot;
        private Texture2D background, notepad, tvBackground, room, splashScreen, menuScreen;
        private Rectangle backgroundRect, notepadRect, tvRect, snapshotRect, notepadSnapshotRect,
            newGameRect, loadGameRect, quitRect,
            _newGameRect, _saveGameRect, _loadGameRect, _quitRect;//the menu accessed during a game has an extra option: save game
            //therefore, we need to place the next two options lower
        private SpriteFont notepadFont, subtitleFont;
        private TextAreaChoice notepadChoice;
        private SubtitleArea tvSpeech;

        // Game State - we need to keep track of where we are to know what to draw and what input to receive
        public byte gameState;
        public const byte ROOM = 0;//choice between notepad, focus group view and watch tv
        public const byte NOTEPAD = 1;//the part where you make your choice
        public const byte FOCUS_GROUP = 2;//view info about the focus group
        public const byte WATCH_NEWS = 3;//watch your decision unfold on tv
        public const byte TRANSITION = 4;//transition animation; controls disabled
        public const byte ENDGAME = 5;
        public const byte SPLASH_SCREEN = 6;
        public const byte MAIN_MENU = 7;
        public const byte INGAME_MENU = 8;
        private int day;//contorizam la ce zi am ajuns ca sa stim ce date sa accesam

        /// <summary>
        /// now here we have a bit of cultural shock ;))
        /// you see, C# Convert functions take into consideration cultural aspects when converting
        /// thus we must ensure a standard; this is important for floats, where there are varying notations, such as 1,0 and 1.0
        /// IMPORTANT: we don't use cultureInfo for config.ini, because some pinhead at M$ decided it'd be a good ideea
        /// if WriteLine took into consideration culture by default, while at the same time not providing a way to change this moronic behaviour
        /// as a bottom line, use culturalInfo wherever we read files shipped with the game - 'cause they're written using a global standard
        /// don't use it when reading/writing local files - the local standard will suffice
        /// If you have further questions on the subject, I suggest burning down your nearest M$ office.
        /// </summary>
        public static System.Globalization.CultureInfo cultureInfo = new System.Globalization.CultureInfo("en-US");

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
            gameState = SPLASH_SCREEN;
            keyPrevious = Keyboard.GetState();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            splashScreen = Content.Load<Texture2D>("splash_screen");
            menuScreen = Content.Load<Texture2D>("menuScreen");
            notepad = Content.Load<Texture2D>("blank-notepad");
            notepadRect = new Rectangle((int)(screenW * 0.2), (int)(screenH * 0.1), (int)(screenW * 0.6), (int)(screenH * 0.8));
            background = Content.Load<Texture2D>("background");
            backgroundRect = new Rectangle(0, 0, screenW, screenH);
            room = Content.Load<Texture2D>("theroom");
            tvBackground = Content.Load<Texture2D>("tv-pdf");
            tvRect = new Rectangle(0, 0, screenW, screenH);
            notepadFont = Content.Load<SpriteFont>("notepadFont");
            subtitleFont = Content.Load<SpriteFont>("subtitleFont");
            newGameRect = new Rectangle((int)(screenW * 0.45), (int)(screenH * 0.4), (int)(screenW * 0.1), (int)(screenH * 0.05));
            loadGameRect = new Rectangle((int)(screenW * 0.45), (int)(screenH * 0.5), (int)(screenW * 0.1), (int)(screenH * 0.05));
            quitRect = new Rectangle((int)(screenW * 0.45), (int)(screenH * 0.6), (int)(screenW * 0.1), (int)(screenH * 0.05));
            _newGameRect = new Rectangle((int)(screenW * 0.45), (int)(screenH * 0.3), (int)(screenW * 0.1), (int)(screenH * 0.05));
            _saveGameRect = new Rectangle((int)(screenW * 0.45), (int)(screenH * 0.4), (int)(screenW * 0.1), (int)(screenH * 0.05));
            _loadGameRect = new Rectangle((int)(screenW * 0.45), (int)(screenH * 0.5), (int)(screenW * 0.1), (int)(screenH * 0.05));
            _quitRect = new Rectangle((int)(screenW * 0.45), (int)(screenH * 0.6), (int)(screenW * 0.1), (int)(screenH * 0.05));

            notepadChoice = new TextAreaChoice(spriteBatch, notepadFont, (int)(screenW * 0.3), (int)(screenH * 0.2), (int)(screenW * 0.4), (int)(screenH * 0.6));
            tvSpeech = new SubtitleArea(spriteBatch, subtitleFont, (int)(screenW * 0.1), (int)(screenH * 0.6), (int)(screenW * 0.8), (int)(screenH * 0.1));
            focusGroupView = new FocusGroupView(GraphicsDevice, spriteBatch, Content, notepadFont, background, screenW, screenH);
            focusGroupViewSnapshot = new RenderTarget2D(GraphicsDevice, screenW, screenH);
            notepadSnapshot = new RenderTarget2D(GraphicsDevice, screenW, screenH);
            // la dimensiunile originale, ar trebui sa inceapa de la 380x105 (imaginea orig e de 1920x1080)
            // fractiile de width si height recomand sa fie egale ca sa se pastreze proportiile ecranului pe care se joaca
            snapshotRect = new Rectangle((int)(screenW * 0.2), (int)(screenH * 0.098), (int)(screenW * 0.25), (int)(screenH * 0.25));
            notepadSnapshotRect = new Rectangle((int)(screenW * 0.48), (int)(screenH * 0.63), (int)(screenW * 0.15), (int)(screenH * 0.2));
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            switch(gameState)
            {
                case NOTEPAD:
                {
                    spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.NonPremultiplied, null, null, null);
                    spriteBatch.Draw(background, backgroundRect, null, Color.Navy, 0.0f, Vector2.Zero, SpriteEffects.None, 0.0f);
                    spriteBatch.Draw(notepad, notepadRect, null, Color.White, 0.0f, Vector2.Zero, SpriteEffects.None, 0.5f);
                    notepadChoice.draw();
                    spriteBatch.End();
                }
                break;
                case FOCUS_GROUP:
                {
                    focusGroupView.draw(null);
                }
                break;
                case WATCH_NEWS:
                case ENDGAME: 
                {
                    spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.NonPremultiplied, null, null, null);
                    spriteBatch.Draw(tvBackground, tvRect, null, Color.White, 0.0f, Vector2.Zero, SpriteEffects.None, 0.0f);
                    tvSpeech.draw();
                    spriteBatch.End();
                }
                break;
                case ROOM:
                {
                    focusGroupView.draw(focusGroupViewSnapshot);
                    GraphicsDevice.SetRenderTarget(notepadSnapshot);
                    spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.NonPremultiplied, null, null, null);
                    spriteBatch.Draw(background, backgroundRect, null, Color.Navy, 0.0f, Vector2.Zero, SpriteEffects.None, 0.0f);
                    spriteBatch.Draw(notepad, notepadRect, null, Color.White, 0.0f, Vector2.Zero, SpriteEffects.None, 0.5f);
                    notepadChoice.draw();
                    spriteBatch.End();
                    GraphicsDevice.SetRenderTarget(null);
                    spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.NonPremultiplied, null, null, null);
                    spriteBatch.Draw(room, backgroundRect, null, Color.White, 0.0f, Vector2.Zero, SpriteEffects.None, 0.0f);
                    spriteBatch.Draw(focusGroupViewSnapshot, snapshotRect, null, Color.White, 0.0f, Vector2.Zero, SpriteEffects.None, 0.5f);
                    spriteBatch.Draw(notepadSnapshot, notepadSnapshotRect, null, Color.White, 0.0f, Vector2.Zero, SpriteEffects.None, 0.5f);
                    spriteBatch.End();
                }
                break;
                case SPLASH_SCREEN:
                {
                    spriteBatch.Begin();
                    spriteBatch.Draw(splashScreen, backgroundRect, Color.White);
                    spriteBatch.End();
                }
                break;
                case MAIN_MENU:
                {
                    spriteBatch.Begin();
                    spriteBatch.Draw(menuScreen, backgroundRect, Color.White);
                    spriteBatch.DrawString(subtitleFont, "New Game", new Vector2(newGameRect.X, newGameRect.Y), Color.White, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 1.0f);
                    spriteBatch.DrawString(subtitleFont, "Load Game", new Vector2(loadGameRect.X, loadGameRect.Y), Color.White, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 1.0f);
                    spriteBatch.DrawString(subtitleFont, "Quit", new Vector2(quitRect.X, quitRect.Y), Color.White, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 1.0f);
                    spriteBatch.DrawString(subtitleFont, "v0.5", new Vector2((int)(screenW * 0.9), (int)(screenH * 0.9)), Color.White, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 1.0f);
                    spriteBatch.End();
                }
                break;
                case INGAME_MENU:
                {
                    spriteBatch.Begin();
                    spriteBatch.Draw(menuScreen, backgroundRect, Color.White);
                    spriteBatch.DrawString(subtitleFont, "New Game", new Vector2(_newGameRect.X, _newGameRect.Y), Color.White, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 1.0f);
                    spriteBatch.DrawString(subtitleFont, "Save Game", new Vector2(_saveGameRect.X, _saveGameRect.Y), Color.White, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 1.0f);
                    spriteBatch.DrawString(subtitleFont, "Load Game", new Vector2(_loadGameRect.X, _loadGameRect.Y), Color.White, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 1.0f);
                    spriteBatch.DrawString(subtitleFont, "Quit", new Vector2(_quitRect.X, _quitRect.Y), Color.White, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 1.0f);
                    spriteBatch.DrawString(subtitleFont, "v0.5", new Vector2((int)(screenW * 0.9), (int)(screenH * 0.9)), Color.White, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 1.0f);
                    spriteBatch.End();
                }
                break;
            }
        }

        /// <summary>
        /// Initializes all variables at their new game state; this means generating a random focus group
        /// </summary>
        private void prepareNewGame()
        {
            day = 1;
            population = Citizen.generatePopulation();
            focusGroupView.prepare(population);
            crrtNews = new News("news/1.txt");
            String[] choices = new String[crrtNews.options.Length];
            int j = 0;
            foreach (Option o in crrtNews.options)
            {
                choices[j++] = o.description;
            }
            notepadChoice.setText(crrtNews.situationDescription, choices);
        }
    }
}
