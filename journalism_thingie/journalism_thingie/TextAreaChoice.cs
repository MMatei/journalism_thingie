using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace journalism_thingie
{
    class TextAreaChoice
    {
        private SpriteBatch spriteBatch;
        private SpriteFont font;
        private List<String> description = new List<String>();
        private List<String>[] choice;
        private int height, width;
        private Vector2 start;

        public TextAreaChoice(SpriteBatch sb, SpriteFont font, int startX, int startY, int width, int height)
        {
            spriteBatch = sb;
            this.font = font;
            this.height = height;
            this.width = width;
            start = new Vector2(startX, startY);
        }

        public void setText(String descr, String[] choice)
        {
            int spaceSize = (int)font.MeasureString(" ").X;
            //trebuie sa impartim descrierea pe linii, pt ca: primim 1 sir si DrawString deseneaza doar pe o linie continua
            String[] word = descr.Split(' ');
            String line = "    ";
            int lineLength = 4*spaceSize;
            foreach (String w in word)
            {
                int wordLength = (int)font.MeasureString(w).X + spaceSize;
                if (lineLength + wordLength < width)
                {
                    lineLength += wordLength;
                    line += w + " ";
                }
                else
                {
                    description.Add(line);
                    line = w + " ";
                    lineLength = wordLength;
                }
            }
            description.Add(line);
            this.choice = new List<String>[choice.Length];
            for (int i = 0; i < choice.Length; i++)
            {
                word = choice[i].Split(' ');
                line = "    ";
                lineLength = 4*spaceSize;
                this.choice[i] = new List<String>();
                foreach (String w in word)
                {
                    int wordLength = (int)font.MeasureString(w).X + spaceSize;
                    if (lineLength + wordLength < width)
                    {
                        lineLength += wordLength;
                        line += w + " ";
                    }
                    else
                    {
                        this.choice[i].Add(line);
                        line = w + " ";
                        lineLength = wordLength;
                    }
                }
                this.choice[i].Add(line);
            }
        }

        public void draw()
        {
            Vector2 deplasament = start;
            foreach (String word in description)
            {
                spriteBatch.DrawString(font, word, deplasament, Color.Black, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 1.0f);
                deplasament.Y += font.LineSpacing;
            }
            deplasament.Y += font.LineSpacing;
            foreach (List<String> ch in choice)
            {
                foreach (String word in ch)
                {
                    spriteBatch.DrawString(font, word, deplasament, Color.Black, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 1.0f);
                    deplasament.Y += font.LineSpacing;
                }
            }
        }

        /// <summary>
        /// If the user clicked on a choice, returns choice nr. Otherwise, returns -1;
        /// </summary>
        public int update(MouseState mouseStateCrrt)
        {
            int mouseY = mouseStateCrrt.Y, mouseX = mouseStateCrrt.X;
            if (mouseStateCrrt.LeftButton == ButtonState.Pressed && mouseX > start.X && mouseX < (start.X + width)
                && mouseY > description.Count*font.LineSpacing + start.Y)
            {//we check what option is being clicked
                int dist = (description.Count+1)*font.LineSpacing + (int)start.Y;
                for (int i = 0; i < choice.Length; i++)
                {//for each choice
                    dist += choice[i].Count*font.LineSpacing;
                    if (mouseY < dist)
                    {
                        Console.WriteLine("YOU SELECTED #" + i);
                        return i;
                    }
                }
            }
            return -1;
        }
    }
}
