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
        private GraphicsDevice gdi;
        private SpriteBatch spriteBatch;
        private SpriteFont font;
        private List<String> description = new List<String>();
        private List<String>[] choice;
        internal RenderTarget2D textArea;
        internal Rectangle textAreaRect;

        public TextAreaChoice(GraphicsDevice gdi, SpriteBatch sb, SpriteFont font, int startX, int startY, int width, int height,
            String descr, String[] choice)
        {
            this.gdi = gdi;
            spriteBatch = sb;
            this.font = font;
            //trebuie sa impartim descrierea pe linii, pt ca: primim 1 sir si DrawString deseneaza doar pe o linie continua
            String[] word = descr.Split(' ');
            String line = "    ";
            int lineLength = 20;
            foreach (String w in word)
            {
                int wordLength = (int)font.MeasureString(w).X+5;
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
                lineLength = 20;
                this.choice[i] = new List<String>();
                foreach (String w in word)
                {
                    int wordLength = (int)font.MeasureString(w).X+5;
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
            textArea = new RenderTarget2D(gdi, width, height, false, gdi.PresentationParameters.BackBufferFormat, DepthFormat.Depth24);
            textAreaRect = new Rectangle(startX, startY, width, height);
        }

        public void draw()
        {
            gdi.SetRenderTarget(textArea);
            gdi.Clear(Color.Transparent);
            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.NonPremultiplied, null, null, null);
            Vector2 deplasament = Vector2.Zero;
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
            spriteBatch.End();
            gdi.SetRenderTarget(null);
        }

        /// <summary>
        /// If the user clicked on a choice, returns choice nr. Otherwise, returns -1;
        /// </summary>
        public int update(MouseState mouseStateCrrt)
        {
            if (mouseStateCrrt.LeftButton == ButtonState.Pressed && textAreaRect.Contains(mouseStateCrrt.X, mouseStateCrrt.Y)
                && mouseStateCrrt.Y > description.Count*font.LineSpacing + textAreaRect.Y)
            {//we check what option is being clicked
                int mouseY = mouseStateCrrt.Y - textAreaRect.Y;
                //Console.WriteLine(mouseY);
                int dist = (description.Count+1)*font.LineSpacing;
                //Console.WriteLine(dist);
                for (int i = 0; i < choice.Length; i++)
                {//for each choice
                    dist += choice[i].Count*font.LineSpacing;
                    //Console.WriteLine(dist);
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
