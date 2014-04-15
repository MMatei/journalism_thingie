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
        private Piechart2D[] piechart = new Piechart2D[6];
        private RenderTarget2D[] piechartTexture = new RenderTarget2D[6];
        private Rectangle[] piechartRect = new Rectangle[6];

        public FocusGroupView(GraphicsDevice gdi, SpriteBatch sb, ContentManager content, SpriteFont font, Texture2D background, int screenW, int screenH)
        {
            this.gdi = gdi;
            spriteBatch = sb;
            this.font = font;
            width = screenW;
            height = screenH;
            this.background = background;
            backgroundRect = new Rectangle(0, 0, width, height);
            circle = content.Load<Texture2D>("circle_small");
            circleRect = new Rectangle(0, 0, 32, 32);
            Texture2D temp = content.Load<Texture2D>("circle");
            for (int i = 0; i < 6; i++)
            {
                piechart[i] = new Piechart2D(gdi, content, temp, 600, 600);
                piechartTexture[i] = new RenderTarget2D(gdi, 600, 600, false, gdi.PresentationParameters.BackBufferFormat, DepthFormat.Depth24);
            }
            piechartRect[0] = new Rectangle(0, 0, (int)(screenW * 0.22), (int)(screenW * 0.22));
            piechartRect[1] = new Rectangle((int)(screenW * 0.33), 0, (int)(screenW * 0.22), (int)(screenW * 0.22));
            piechartRect[2] = new Rectangle((int)(screenW * 0.66), 0, (int)(screenW * 0.22), (int)(screenW * 0.22));
            piechartRect[3] = new Rectangle(0, (int)(screenH * 0.5), (int)(screenW * 0.22), (int)(screenW * 0.22));
            piechartRect[4] = new Rectangle((int)(screenW * 0.33), (int)(screenH * 0.5), (int)(screenW * 0.22), (int)(screenW * 0.22));
            piechartRect[5] = new Rectangle((int)(screenW * 0.66), (int)(screenH * 0.5), (int)(screenW * 0.22), (int)(screenW * 0.22));
        }

        /// <summary>
        /// Call this function each time we enter the focus grou view, in order to update the piechart with relevant information
        /// </summary>
        public void prepare(Citizen[] pop)
        {
            #region Ideology
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
                if (kane.ideology < -0.7)
                    eright++;
                else if (kane.ideology < -0.4)
                    right++;
                else if (kane.ideology < 0.4)
                    center++;
                else if (kane.ideology < 0.7)
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
            descr.Add("Nationalist " + eright + "%");
            descr.Add("Conservative " + right + "%");
            descr.Add("Liberal " + center + "%");
            descr.Add("Social-democrat " + left + "%");
            descr.Add("Socialist " + eleft + "%");
            piechart[0].setPiechart(percentages, colors, descr);
            #endregion
            //common colors for the issues piecharts
            colors = new List<Color>();
            colors.Add(Color.Blue);
            colors.Add(Color.Yellow);
            colors.Add(Color.Red);
            #region Nationalism
            percentages = new List<int>();
            int pro = 0, meh = 0, anti = 0;
            foreach (Citizen kane in pop)
            {
                if (kane.nationalist < -0.4)
                    anti++;
                else if (kane.nationalist < 0.4)
                    meh++;
                else
                    pro++;
            }
            percentages.Add(pro);
            percentages.Add(meh);
            percentages.Add(anti);
            descr = new List<String>();
            descr.Add("No " + pro + "%");
            descr.Add("I don't know " + meh + "%");
            descr.Add("Yes " + anti + "%");
            piechart[1].setPiechart(percentages, colors, descr);
            #endregion
            #region Minority rights
            percentages = new List<int>();
            pro = 0; meh = 0; anti = 0;
            foreach (Citizen kane in pop)
            {
                if (kane.minorityRights < -0.4)
                    anti++;
                else if (kane.minorityRights < 0.4)
                    meh++;
                else
                    pro++;
            }
            percentages.Add(pro);
            percentages.Add(meh);
            percentages.Add(anti);
            descr = new List<String>();
            descr.Add("Expand them " + pro + "%");
            descr.Add("I don't know " + meh + "%");
            descr.Add("Restrict them " + anti + "%");
            piechart[2].setPiechart(percentages, colors, descr);
            #endregion
            #region Isolationism
            percentages = new List<int>();
            pro = 0; meh = 0; anti = 0;
            foreach (Citizen kane in pop)
            {
                if (kane.isolationism < -0.4)
                    anti++;
                else if (kane.isolationism < 0.4)
                    meh++;
                else
                    pro++;
            }
            percentages.Add(pro);
            percentages.Add(meh);
            percentages.Add(anti);
            descr = new List<String>();
            descr.Add("I'm for it " + pro + "%");
            descr.Add("I don't know " + meh + "%");
            descr.Add("I'm against it " + anti + "%");
            piechart[3].setPiechart(percentages, colors, descr);
            #endregion
            #region Social justice
            percentages = new List<int>();
            pro = 0; meh = 0; anti = 0;
            foreach (Citizen kane in pop)
            {
                if (kane.socialJustice < -0.4)
                    anti++;
                else if (kane.socialJustice < 0.4)
                    meh++;
                else
                    pro++;
            }
            percentages.Add(pro);
            percentages.Add(meh);
            percentages.Add(anti);
            descr = new List<String>();
            descr.Add("No " + pro + "%");
            descr.Add("I don't know " + meh + "%");
            descr.Add("Yes " + anti + "%");
            piechart[4].setPiechart(percentages, colors, descr);
            #endregion
            #region Media trust
            percentages = new List<int>();
            pro = 0; meh = 0; anti = 0;
            foreach (Citizen kane in pop)
            {
                if (kane.mediaTrust < 0.3)
                    anti++;
                else if (kane.mediaTrust < 0.7)
                    meh++;
                else
                    pro++;
            }
            percentages.Add(pro);
            percentages.Add(meh);
            percentages.Add(anti);
            descr = new List<String>();
            descr.Add("I trust it " + pro + "%");
            descr.Add("I don't know " + meh + "%");
            descr.Add("I don't trust it " + anti + "%");
            piechart[5].setPiechart(percentages, colors, descr);
            #endregion
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
            for (int i = 0; i < 6; i++)
            {
                gdi.SetRenderTarget(piechartTexture[i]);
                piechart[i].draw();
            }
            gdi.SetRenderTarget(null);
            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.NonPremultiplied, null, null, null);
            spriteBatch.Draw(background, backgroundRect, null, Color.White, 0.0f, Vector2.Zero, SpriteEffects.None, 0.0f);
            for (int i = 0; i < 6; i++)
            {
                spriteBatch.Draw(piechartTexture[i], piechartRect[i], null, Color.White, 0.0f, Vector2.Zero, SpriteEffects.None, 1.0f);
                circleRect.X = piechartRect[i].Right - 40;
                circleRect.Y = piechartRect[i].Top + 40;
                Vector2 txtPos = new Vector2(circleRect.X + 40, piechartRect[i].Top + 45);
                foreach (PieSlice slice in piechart[i].slices)
                {
                    spriteBatch.Draw(circle, circleRect, null, slice.color, 0.0f, Vector2.Zero, SpriteEffects.None, 1.0f);
                    spriteBatch.DrawString(font, slice.description, txtPos, Color.Black, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 1.0f);
                    circleRect.Y += 40;
                    txtPos.Y += 40;
                }
            }
            spriteBatch.DrawString(font, "What ideology do you identify yourself with?", new Vector2(piechartRect[0].Left + 50, piechartRect[0].Bottom - 50), Color.Black, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 1.0f);
            spriteBatch.DrawString(font, "Should we reclaim former Tolozian lands?", new Vector2(piechartRect[1].Left + 50, piechartRect[1].Bottom - 50), Color.Black, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 1.0f);
            spriteBatch.DrawString(font, "What should be done about minority rights?", new Vector2(piechartRect[2].Left + 50, piechartRect[2].Bottom - 50), Color.Black, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 1.0f);
            spriteBatch.DrawString(font, "How do you feel about globalization?", new Vector2(piechartRect[3].Left + 50, piechartRect[3].Bottom - 50), Color.Black, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 1.0f);
            spriteBatch.DrawString(font, "Do you think the government is corrupt?", new Vector2(piechartRect[4].Left + 50, piechartRect[4].Bottom - 50), Color.Black, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 1.0f);
            spriteBatch.DrawString(font, "How much confidence do you have in our network?", new Vector2(piechartRect[5].Left + 50, piechartRect[5].Bottom - 50), Color.Black, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 1.0f);
            spriteBatch.End();
        }

        /// <summary>
        /// Check if mouse hovers over person more than 2 seconds => pop-up displaying relevant info.
        /// Returns 1 if user wants to return to notepad
        /// </summary>
        public int update(MouseState mouseStateCrrt, MouseState mouseStatePrev, KeyboardState keyCurrent, GameTime gameTime)
        {
            if (keyCurrent.IsKeyDown(Keys.Enter))
                return 1;
            /*bool mouseNotMoved = mouseStateCrrt.X == mouseStatePrev.X && mouseStateCrrt.Y == mouseStatePrev.Y;
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
