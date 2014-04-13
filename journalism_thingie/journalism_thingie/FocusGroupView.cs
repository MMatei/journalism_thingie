using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace journalism_thingie
{
    class FocusGroupView
    {
        private GraphicsDevice gdi;
        private SpriteBatch spriteBatch;
        private SpriteFont font;
        private Texture2D background, circle;
        private Rectangle backgroundRect, circleRect;
        private int width, height;
        private Piechart2D piechart;
        private RenderTarget2D piechartTexture;

        public FocusGroupView(GraphicsDevice gdi, SpriteBatch sb, ContentManager content, SpriteFont font, Texture2D background, int screenW, int screenH)
        {
            this.gdi = gdi;
            spriteBatch = sb;
            this.font = font;
            width = screenW;
            height = screenH;
            piechartTexture = new RenderTarget2D(gdi, 300, 300, false, gdi.PresentationParameters.BackBufferFormat, DepthFormat.Depth24);
            this.background = background;
            backgroundRect = new Rectangle(0, 0, width, height);
            piechart = new Piechart2D(gdi, content, content.Load<Texture2D>("circle"), 300, 300);
            circle = content.Load<Texture2D>("circle_small");
            circleRect = new Rectangle(0, 0, 32, 32);
        }

        /// <summary>
        /// Call this function each time we enter the focus grou view, in order to update the piechart with relevant information
        /// </summary>
        public void prepare(Citizen[] pop)
        {
            List<Color> colors = new List<Color>();
            colors.Add(Color.Black);
            colors.Add(new Color(64, 64, 64));
            colors.Add(Color.Blue);
            colors.Add(new Color(255, 50, 50));
            colors.Add(Color.Red);
            List<int> percentages = new List<int>();
            int eright = 0, right = 0, center = 0, left = 0, eleft = 0;
            foreach (Citizen kane in pop)
            {
                if (kane.ideology < 0.15)
                    eright++;
                else if (kane.ideology < 0.3)
                    right++;
                else if (kane.ideology < 0.7)
                    center++;
                else if (kane.ideology < 0.85)
                    left++;
                else
                    eleft++;
            }
            percentages.Add(eright);
            percentages.Add(right);
            percentages.Add(center);
            percentages.Add(left);
            percentages.Add(eleft);
            List<String> descr = new List<String>();
            descr.Add("Extreme right " + eright + "%");
            descr.Add("Rightist " + right + "%");
            descr.Add("Centrist " + center + "%");
            descr.Add("Leftist " + left + "%");
            descr.Add("Extreme left " + eleft + "%");
            piechart.setPiechart(percentages, colors, descr);
        }

        public void draw()
        {
            /*personRect.X = -personRect.Width;
            personRect.Y = 0;
            for (int i = 0; i < population.Length; i++)
            {
                personRect.X += personRect.Width;
                if (personRect.X + personRect.Width > width)
                {
                    personRect.Y += personRect.Height;
                    personRect.X = 0;
                }
                //color will depend on ideology: extreme right is black, center is blue and extreme left is red
                float ideology = (float)population[i].ideology;
                Color color;
                if (ideology <= 0.5f)
                {
                    color = new Color(0.0f, 0.0f, ideology * 2);
                }
                else
                    color = new Color((ideology - 0.5f) * 2, 0.0f, (1.0f - ideology) * 2);
                spriteBatch.Draw(person, personRect, null, color, 0.0f, Vector2.Zero, SpriteEffects.None, 0.9f);
            }
            if (displayingTooltip)
            {
                spriteBatch.Draw(tooltipBackground, tooltipRect, null, Color.White, 0.0f, Vector2.Zero, SpriteEffects.None, 0.95f);
                spriteBatch.DrawString(font, tooltip, tooltipStart, Color.Black, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 1.0f);
            }*/
            gdi.SetRenderTarget(piechartTexture);
            piechart.draw();
            gdi.SetRenderTarget(null);
            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.NonPremultiplied, null, null, null);
            spriteBatch.Draw(background, backgroundRect, null, Color.White, 0.0f, Vector2.Zero, SpriteEffects.None, 0.0f);
            spriteBatch.Draw(piechartTexture, new Rectangle(0,0,300,300), null, Color.White, 0.0f, Vector2.Zero, SpriteEffects.None, 1.0f);
            circleRect.X = 300;
            circleRect.Y = 40;
            Vector2 txtPos = new Vector2(340, 45);
            foreach (PieSlice slice in piechart.slices)
            {
                spriteBatch.Draw(circle, circleRect, null, slice.color, 0.0f, Vector2.Zero, SpriteEffects.None, 1.0f);
                spriteBatch.DrawString(font, slice.description, txtPos, Color.Black, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 1.0f);
                circleRect.Y += 40;
                txtPos.Y += 40;
            }
            spriteBatch.DrawString(font, "Citizen's ideology", new Vector2(80, 250), Color.Black, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 1.0f);
            spriteBatch.End();
        }

        /// <summary>
        /// Check if mouse hovers over person more than 2 seconds => pop-up displaying relevant info.
        /// Returns 1 if user wants to return to notepad
        /// </summary>
        public int update(MouseState mouseStateCrrt, MouseState mouseStatePrev, KeyboardState keyCurrent, GameTime gameTime)
        {/*
            if (keyCurrent.IsKeyDown(Keys.Enter))
                return 1;
            bool mouseNotMoved = mouseStateCrrt.X == mouseStatePrev.X && mouseStateCrrt.Y == mouseStatePrev.Y;
            if(displayingTooltip)
            {
                if (mouseNotMoved)
                    return 0;
                else
                {
                    displayingTooltip = false;
                    return 0;
                }
            }
            if (mouseNotMoved)
            {
                if (alreadyCounting)
                {
                    counter += gameTime.ElapsedGameTime.Milliseconds;
                    //Console.WriteLine(counter);
                    if (counter >= 2000)
                    {
                        displayingTooltip = true;
                        alreadyCounting = false;
                        int x = mouseStateCrrt.X / personRect.Width;
                        int y = mouseStateCrrt.Y / personRect.Height;
                        tooltip = "Ideology: " + population[y * personsPerRow + x].ideology.ToString("0.00");
                        tooltipStart.X = mouseStateCrrt.X + 20;
                        tooltipStart.Y = mouseStateCrrt.Y;
                        Vector2 stringSize = font.MeasureString(tooltip);
                        tooltipRect = new Rectangle((int)tooltipStart.X - 2, (int)tooltipStart.Y - 2, (int)stringSize.X + 4, (int)stringSize.Y + 4);
                    }
                }
                else
                {
                    alreadyCounting = true;
                    counter = 0;
                }
            }*/
            return 0;
        }
    }
}
