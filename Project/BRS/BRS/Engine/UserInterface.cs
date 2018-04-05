// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System;
using BRS.Menu;

namespace BRS.Scripts {

    [Flags]
    public enum Align { Center = 0, Left = 1, Right = 2, Top = 4, Bottom = 8, TopLeft = 5, TopRight = 6, BotLeft = 9, BotRight = 10 }
    //public enum Align2 { M=0, L=1, R=2, T=4, B=8, TL=5, TR=6, BL=9, BR=10 }

    class UserInterface {
        ////////// acts as HUB to draw everything related to the UI, either in splitscreen (each window) or global (just once) //////////


        // --------------------- VARIABLES ---------------------

        //public enum VerticalAnchor { Top, Middle, Bottom}; // how to align the UI
        //public enum HorizontAnchor { Left, Middle, Right};
        //public enum Align { TL, RM, TR, ML, MM, MR, BL, BM, BR} // top, middle, bottom x left, middle, right
        //public



        //private
        public SpriteFont smallFont;
        public SpriteFont comicFont;
        SpriteFont bigFont;
        Texture2D bar;
        Texture2D barBig;
        public const int BARWIDTH = 128;
        public const int BARBIGWIDTH = 256;
        public const int BARHEIGHT = 16;

        Rectangle barRect, bigRect, smallRect;
        Texture2D white;

        //reference
        public static UserInterface instance;
        public SpriteBatch sB;

        Color White = new Color(1, 1, 1);


        // --------------------- BASE METHODS ------------------
        public void Start() {
            instance = this;
            smallFont = File.Load<SpriteFont>("Other/font/font1");
            comicFont = File.Load<SpriteFont>("Other/font/comicFont");
            bigFont   = File.Load<SpriteFont>("Other/font/font2");

            bar       = File.Load<Texture2D>("Images/UI/progress_bar_small");
            barBig    = File.Load<Texture2D>("Images/UI/progress_bar");
            white     = File.Load<Texture2D>("Images/UI/white");

            barRect = new Rectangle(0, 0, BARWIDTH, BARHEIGHT);
            bigRect = new Rectangle(0, 0, BARBIGWIDTH, BARHEIGHT);
            smallRect = new Rectangle(0, 0, BARBIGWIDTH/4, BARHEIGHT/4);
            //fgRect = new Rectangle(0, BARHEIGHT, BARWIDTH, BARHEIGHT);
        }

        public void DrawMenu(SpriteBatch spriteBatch) {
            sB = spriteBatch;
            MenuManager.instance.Draw();
        }

        public void DrawGlobal(SpriteBatch spriteBatch) {
            sB = spriteBatch;
            //callbacks
            //Minimap.instance.Draw(sB);
            GameUI.instance.Draw();
        }

        public void DrawSplitscreen(SpriteBatch spriteBatch, int index) { // call all subcomponents that are drawn on each split screen
            sB = spriteBatch;
            //callbacks
            
            /*
            BaseUI.instance.Draw(index%2);
            PlayerUI.instance.Draw(index);
            PowerupUI.instance.Draw(index);
            Suggestions.instance.Draw(index);
            Minimap.instance.DrawSmall(spriteBatch, index);
            */

            //test draw
            
            DrawPictureAlign(white, new Rectangle(53, 53, 100, 100), null, Align.TopLeft, Align.Center, Color.Gray, false);
            //DrawPictureAlign(white, new Rectangle(100, 100, 150, 100), null, Align.TopLeft, Align.TopLeft, Color.LightGray, true);

            DrawStringAlign("Text one", new Rectangle(10, 10, 300, 100), Align.TopLeft, Align.TopLeft, Align.BotRight, Color.Black);
            DrawStringAlign("Text two", new Rectangle(10, 10, 300, 100), Align.TopLeft, Align.TopLeft, Align.BotRight, Color.Black, true);
        }





        // --------------------- CUSTOM METHODS ----------------

        //DRAW CALLBACKS

        //BARS
        public void DrawBar(Vector2 position, float percent, Color color) {
            barRect.Width = BARWIDTH;
            sB.Draw(bar, position, barRect, Color.LightGray);
            barRect.Width = (int)(BARWIDTH * percent);
            sB.Draw(bar, position, barRect, color);
        }
        public void DrawBarVertical(Vector2 position, float percent, Color color) {
            barRect.Width = BARWIDTH;
            //sB.Draw(bar, position, barRect, Color.LightGray);
            sB.Draw(bar, position, barRect, Color.LightGray, MathHelper.ToRadians(-90), Vector2.Zero, 1f, SpriteEffects.None, 1);
            barRect.Width = (int)(BARWIDTH * percent);
            //sB.Draw(bar, position, barRect, color);
            sB.Draw(bar, position, barRect, color, MathHelper.ToRadians(-90), Vector2.Zero, 1f, SpriteEffects.None, 1);
        }
        public void DrawBarBig(Vector2 position, float percent, Color color) {
            bigRect.Width = BARBIGWIDTH;
            sB.Draw(barBig, position, bigRect, Color.LightGray);
            bigRect.Width = (int)(BARBIGWIDTH * percent);
            sB.Draw(barBig, position, bigRect, color);
        }
        public void DrawBarSmall(Vector2 position, float percent, Color color) {
            bigRect.Width = BARBIGWIDTH;
            sB.Draw(barBig, position, bigRect, Color.LightGray, 0, Vector2.Zero, .25f, SpriteEffects.None, 1);
            bigRect.Width = (int)(BARBIGWIDTH * percent);
            sB.Draw(barBig, position, bigRect, color, 0, Vector2.Zero, .25f, SpriteEffects.None, 1);
        }


        public void DrawString(Vector2 position, string text, Color colour = default(Color)) {
            sB.DrawString(smallFont, text, position, colour == default(Color)? Color.White : colour);
        }
        public void DrawStringBig(Vector2 position, string text, Color colour = default(Color)) {
            sB.DrawString(bigFont, text, position, colour == default(Color) ? Color.White : colour);
        }

        public void DrawPicture(Rectangle destination, Texture2D pic, Color colour = default(Color)) {
            sB.Draw(pic, destination, colour == default(Color) ? Color.White : colour);
        }

        public void DrawPicture(Vector2 position, Texture2D pic, Vector2 origin, float scale) {
            sB.Draw(pic, position, null, Color.White, 0, origin, scale, SpriteEffects.None, 1);
        }
        public void DrawPicture(Rectangle destination, Texture2D pic, Rectangle source) {
            sB.Draw(pic, destination, source, Color.White);
        }
        public void DrawPicture(Rectangle dest, Texture2D pic, Rectangle src, float rotation) {
            Vector2 origin = new Vector2(src.Width / 2, src.Height / 2);
            sB.Draw(pic, dest, src, Color.White, MathHelper.ToRadians(rotation), origin, SpriteEffects.None, 1);
        }



        //queries
        public int GetOffset(int index) { return index % 2 == 0 ? 0 : +680; }



        

        //pivot  = where is the center of the rectangle
        //anchor = which corner of the screen to follow
        //paragraph = how to align the paragraph
        public void DrawStringAlign(string text, Rectangle bounds, Align anchor, Align pivot, Align paragraph, Color? col, bool flip = false) { // bounds includes position offset and rectangle size
            if (flip) {
                bounds.X *= -1;
                pivot = Flip(pivot); anchor = Flip(anchor); paragraph = Flip(paragraph);
            }
            bounds = AlignRect(bounds, anchor, pivot);
            
            Color color = (col == null) ? Color.White : (Color)col;

            Vector2 size = smallFont.MeasureString(text);
            Vector2 pos = bounds.GetCenter();
            Vector2 origin = size * 0.5f;

            if (paragraph.HasFlag(Align.Left))   origin.X += bounds.Width / 2  - size.X / 2;
            if (paragraph.HasFlag(Align.Right))  origin.X -= bounds.Width / 2  - size.X / 2;
            if (paragraph.HasFlag(Align.Top))    origin.Y += bounds.Height / 2 - size.Y / 2;
            if (paragraph.HasFlag(Align.Bottom)) origin.Y -= bounds.Height / 2 - size.Y / 2;
            
            sB.DrawString(smallFont, text, pos, color, 0, origin, 1, SpriteEffects.None, 0);
        }

        public void DrawPictureAlign(Texture2D tex, Rectangle bounds, Rectangle? source, Align anchor, Align pivot, Color? col, bool flip = false, float rot = 0) {
            if (flip) {
                bounds.X *= -1;
                pivot = Flip(pivot); anchor = Flip(anchor);
            }
            Vector2 origin = PivotPoint(pivot, tex.Bounds).ToVector2();//is relative to the texture
            bounds.Location += AnchorPos(anchor) - PivotPoint(pivot, bounds) + origin.ToPoint();

            Color color = (col == null) ? Color.White : (Color)col;
            sB.Draw(tex, bounds, source, color, MathHelper.ToRadians(rot), origin, flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 1);
        }



        //OLD CODE
        //horiz
        /*
        if (anchor.HasFlag(Alignment.Left)) bounds.X = (int)position.X;
        else if (anchor.HasFlag(Alignment.Right)) bounds.X = Screen.WIDTH - bounds.Width - (int)position.X;
        else bounds.X = Screen.WIDTH / 2 - bounds.Width / 2 + (int)position.X;

        //vert
        if (anchor.HasFlag(Alignment.Top)) bounds.Y = (int)position.Y;
        else if (anchor.HasFlag(Alignment.Bottom)) bounds.Y = Screen.HEIGHT - bounds.Height - (int)position.Y;
        else bounds.Y = Screen.HEIGHT / 2 - bounds.Height / 2 + (int)position.Y;

        //--
        Vector2 size = bounds.Size.ToVector2();
        Vector2 pos = bounds.GetCenter();
        Vector2 origin = size * 0.5f;

        //probably unuseful
        if (anchor.HasFlag(Alignment.Left)) origin.X += bounds.Width / 2 - size.X / 2;
        if (anchor.HasFlag(Alignment.Right)) origin.X -= bounds.Width / 2 - size.X / 2;
        if (anchor.HasFlag(Alignment.Top)) origin.Y += bounds.Height / 2 - size.Y / 2;
        if (anchor.HasFlag(Alignment.Bottom)) origin.Y -= bounds.Height / 2 - size.Y / 2;
        */
        //sB.Draw(tex, pos, color, 0, origin, 1, SpriteEffects.None, 0);

        Rectangle AlignRect(Rectangle rect, Align anchor, Align pivot) {
            rect.Location += AnchorPos(anchor)-PivotPoint(pivot, rect);
            return rect;
        }

        Point AnchorPos(Align anchor) {
            int anchorPosX = anchor.HasFlag(Align.Left) ? 0 : anchor.HasFlag(Align.Right) ? Screen.SPLITWIDTH : Screen.SPLITWIDTH / 2;
            int anchorPosY = anchor.HasFlag(Align.Top) ? 0 : anchor.HasFlag(Align.Bottom) ? Screen.SPLITHEIGHT : Screen.SPLITHEIGHT / 2;
            return new Point(anchorPosX, anchorPosY);
        }

        Point PivotPoint(Align pivot, Rectangle rect) {
            int pivotPosX = pivot.HasFlag(Align.Left) ? 0 : pivot.HasFlag(Align.Right) ? rect.Width : rect.Width / 2;
            int pivotPosY = pivot.HasFlag(Align.Top) ? 0 : pivot.HasFlag(Align.Bottom) ? rect.Height : rect.Height / 2;
            return new Point(pivotPosX, pivotPosY);
        }

        Align Flip(Align al) {
            if      (al.HasFlag(Align.Left))  al += Align.Right - Align.Left;
            else if (al.HasFlag(Align.Right)) al -= Align.Right - Align.Left;
            return al;
        }

    }


}