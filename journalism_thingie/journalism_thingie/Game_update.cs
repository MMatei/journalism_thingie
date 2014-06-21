using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace journalism_thingie
{
    public partial class Game
    {
        internal void SaveGame(String saveFileName)
        {
            //using binary format to discourage savegame editing
            BinaryWriter savefile = new BinaryWriter(new FileStream(saveFileName, FileMode.Create));
            savefile.Write(day);
            foreach (Citizen c in population)
            {
                savefile.Write(c.lifeQuality);
                savefile.Write(c.fanaticism);
                savefile.Write(c.ideology);
                savefile.Write(c.nationalist);
                savefile.Write(c.minorityRights);
                savefile.Write(c.isolationism);
                savefile.Write(c.socialJustice);
                savefile.Write(c.mediaTrust);
            }
        }
        internal void LoadGame(String saveFileName)
        {
            BinaryReader savefile = new BinaryReader(new FileStream(saveFileName, FileMode.Open));
            day = savefile.ReadInt32();
            foreach (Citizen c in population)
            {
                c.lifeQuality = savefile.ReadDouble();
                c.fanaticism = savefile.ReadDouble();
                c.ideology = savefile.ReadDouble();
                c.nationalist = savefile.ReadDouble();
                c.minorityRights = savefile.ReadDouble();
                c.isolationism = savefile.ReadDouble();
                c.socialJustice = savefile.ReadDouble();
                c.mediaTrust = savefile.ReadDouble();
            }
        }
        protected override void Update(GameTime gameTime)
        {
            KeyboardState keyCurrent = Keyboard.GetState();
            MouseState mouseStateCurrent = Mouse.GetState();

            switch (gameState)
            {
                case NOTEPAD:
                {
                    int ret = notepadChoice.update(mouseStateCurrent, keyCurrent);
                    if (ret != -1)
                    {
                        gameState = WATCH_NEWS;
                        tvSpeech.setText(crrtNews.options[ret].newsArticle);
                    }
                    else
                    {
                        if (keyCurrent.IsKeyDown(Keys.Escape) || keyCurrent.IsKeyDown(Keys.Enter))
                        {
                            gameState = ROOM;
                        }
                    }
                }
                break;
                case FOCUS_GROUP:
                {
                    if (keyCurrent.IsKeyDown(Keys.Escape) || keyCurrent.IsKeyDown(Keys.Enter))
                    {
                        gameState = ROOM;
                    }
                }
                break;
                case WATCH_NEWS:
                {
                    if (tvSpeech.update(keyCurrent, keyPrevious) == 1)
                    {
                        day++;//the choice has been made, we move on to the next day
                        if (!File.Exists("news/" + day + ".txt"))
                        {//endgame
                            tvSpeech.setText(endgame());
                            gameState = ENDGAME;
                        }
                        else
                        {
                            crrtNews = new News("news/" + day + ".txt");
                            String[] choices = new String[crrtNews.options.Length];
                            int j = 0;
                            foreach (Option o in crrtNews.options)
                            {
                                choices[j++] = o.description;
                            }
                            notepadChoice.setText(crrtNews.situationDescription, choices);
                            focusGroupView.prepare(population);//a day has passed => piecharts need updating
                            gameState = ROOM;
                        }
                    }
                }
                break;
                case ROOM:
                {
                    if (keyCurrent.IsKeyDown(Keys.Escape))
                        gameState = INGAME_MENU;
                    if (mouseStateCurrent.LeftButton == ButtonState.Pressed)
                    {
                        if (snapshotRect.Contains(mouseStateCurrent.X, mouseStateCurrent.Y))
                        {//am dat click pe tabla cu focus view
                            gameState = FOCUS_GROUP;
                        }
                        if (notepadSnapshotRect.Contains(mouseStateCurrent.X, mouseStateCurrent.Y))
                        {//am dat click pe notepad
                            gameState = NOTEPAD;
                        }
                    }
                }
                break;
                case ENDGAME:
                {
                    if (keyCurrent.IsKeyDown(Keys.Escape))
                        gameState = MAIN_MENU;
                    if (tvSpeech.update(keyCurrent, keyPrevious) == 1)
                        gameState = MAIN_MENU;
                }
                break;
                case SPLASH_SCREEN:
                {
                    day++;
                    if (day == 100)
                    {
                        day = 1;
                        gameState = MAIN_MENU;
                    }
                }
                break;
                case MAIN_MENU:
                {
                    if(mouseStateCurrent.LeftButton == ButtonState.Pressed)
                    {
                        if (newGameRect.Contains(mouseStateCurrent.X, mouseStateCurrent.Y))
                        {
                            prepareNewGame();
                            gameState = ROOM;
                        }
                        if(quitRect.Contains(mouseStateCurrent.X, mouseStateCurrent.Y))
                            this.Exit();
                    }
                }
                break;
                case INGAME_MENU:
                {
                    if (keyCurrent.IsKeyDown(Keys.Escape))
                        gameState = ROOM;//go back
                    if (mouseStateCurrent.LeftButton == ButtonState.Pressed)
                    {
                        if (newGameRect.Contains(mouseStateCurrent.X, mouseStateCurrent.Y))
                        {
                            prepareNewGame();
                            gameState = ROOM;
                        }
                        if (quitRect.Contains(mouseStateCurrent.X, mouseStateCurrent.Y))
                            this.Exit();
                    }
                }
                break;
            }
            keyPrevious = keyCurrent;
            mousePrevious = mouseStateCurrent;
        }

        /// <summary>
        /// Called in order to provide the ending's text crawl.
        /// </summary>
        private String endgame()
        {
            int[] tally = new int[24];
            Array.Clear(tally, 0, 24);
            foreach (Citizen c in population)
            {
                tally[c.vote()]++;
            }
            int max = 0, result = 0;
            for (int i = 0; i < 24; i++)
            {
                if (tally[i] > max)
                {
                    max = tally[i];
                    result = i;
                }
            }

            StreamReader input = new StreamReader("endings.txt");
            for (int i = 0; i < result; i++)
            {
                input.ReadLine();
            }
            String rez = input.ReadLine();
            input.Close();
            return rez;
        }
    }
}
