﻿// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using System;
using BRS.Engine.Utilities;
using BRS.Menu;
using BRS.Scripts;
using BRS.Scripts.UI;
using BRS.Scripts.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BRS.Engine {

    [Flags]
    public enum Align { Center = 0, Left = 1, Right = 2, Top = 4, Bottom = 8, TopLeft = 5, TopRight = 6, BotLeft = 9, BotRight = 10, Undef = 16 }
    //public enum Align2 { M=0, L=1, R=2, T=4, B=8, TL=5, TR=6, BL=9, BR=10 }
    //public enum Align3 { TL, RM, TR, ML, MM, MR, BL, BM, BR} // (top, middle, bottom) x (left, middle, right)

    class UserInterface {
        ////////// acts as HUB to draw everything related to the UI, either in splitscreen (each window) or global (just once) //////////

        // --------------------- VARIABLES ---------------------


        //public
        public const int BarWidth = 128;
        public const int BigBarWidth = 256;
        public const int BarHeight = 16;


        //private
        //public SpriteFont arialFont { get; private set; }
        public static SpriteFont comicFont { get; private set; }
        public static SpriteFont archerFont   { get; private set; }

        private static Texture2D _bar, _barBig;
        private static Texture2D barStriped;

        private static Rectangle _barRect, _bigRect, _smallRect;

        //reference
        public static SpriteBatch sB;

        static MenuManager _menuManager;


        // --------------------- BASE METHODS ------------------

        public static void Start() {
            //arialFont = File.Load<SpriteFont>("Other/font/debugFont");
            comicFont = File.Load<SpriteFont>("Other/font/comic");
            archerFont   = File.Load<SpriteFont>("Other/font/archer");

            _bar       = File.Load<Texture2D>("Images/UI/progress_bar_small");
            _barBig    = File.Load<Texture2D>("Images/UI/progress_bar");
            barStriped = File.Load<Texture2D>("Images/UI/bar_striped");

            _barRect = new Rectangle(0, 0, BarWidth, BarHeight);
            _bigRect = new Rectangle(0, 0, BigBarWidth, BarHeight);
            _smallRect = new Rectangle(0, 0, BigBarWidth/4, BarHeight/4);
            //fgRect = new Rectangle(0, BARHEIGHT, BARWIDTH, BARHEIGHT);

            _menuManager = new MenuManager();
            _menuManager.LoadContent();
        }



        // ---------- CALLBACKS ----------


        // --------------------- CUSTOM METHODS ----------------

        //DRAW CALLBACKS

        //BARS
        /*
        public static void DrawBar(float percent, Vector2 position, Color color, Align anchor) {
            DrawPicture(_bar, position, anchor: anchor, col: Color.LightGray);
            DrawPicture(_bar, position, new Rectangle(0,0, (int)(BarWidth * percent), BarHeight), anchor: anchor, col: color);
        }
        public static void DrawBarBig(float percent, Vector2 position, Color color, Align anchor) {
            DrawPicture(_barBig, position, anchor: anchor, pivot:Align.TopLeft, col: Color.LightGray);
            DrawPicture(_barBig, position, new Rectangle(0, 0, (int)(BigBarWidth * percent), BarHeight), anchor: anchor, pivot: Align.TopLeft, col: color);
        }
        public static void DrawBarSmall(float percent, Vector2 position, Color color, Align anchor = Align.TopLeft) {
            DrawPicture(_barBig, position, anchor: anchor, col: Color.LightGray, scale:.25f);
            DrawPicture(_barBig, position, new Rectangle(0, 0, (int)(BigBarWidth * percent), BarHeight), anchor: anchor, col: color, scale: .25f);
        }*/

        public static void DrawBarStriped(float percent, Rectangle dest, Color color, Align anchor = Align.TopLeft, bool flip = false) {
            Rectangle source = new Rectangle(0, 0, 185, 40);
            if (flip) {
                dest.X = -dest.X- dest.Width;
                anchor = Flip(anchor);
            }
            DrawPicture(barStriped, dest, source, anchor: anchor, pivot: Align.TopLeft, col: Color.White);
            source = new Rectangle(0, 40, (int)Math.Round(185 * percent), 40);
            dest.Width = (int)Math.Round(dest.Width * percent);
            DrawPicture(barStriped, dest, source, anchor: anchor, pivot: Align.TopLeft, col: color);
        }

        //OLD SIGNATURES --> CHANGE!!
        public static void DrawPictureOLD(Rectangle destination, Texture2D pic, Color colour = default(Color)) {
            DrawPicture(pic, destination, col: colour);
        }
        public static void DrawStringOLD(Vector2 position, string text, Color colour = default(Color)) {
            DrawString(text, position, col: colour);
        }



        // --------------------- ALIGN ----------------

        //pivot  = where is the center of the rectangle
        //anchor = which corner of the screen to follow
        //paragraph = how to align the paragraph

        public static void DrawPicture(Texture2D tex, Vector2 pos, Rectangle? source = null, Align anchor = Align.TopLeft, Align pivot = Align.Undef, Color? col = null, bool flip = false, float rot = 0, float scale = 1) {
            Rectangle src = source ?? tex.Bounds;
            Rectangle dest = new Rectangle(pos.ToPoint(), (src.Size.ToVector2()*scale).ToPoint());
            DrawPicture(tex, dest, source, anchor, pivot, col, flip, rot);
        }

        public static void DrawPicture(Texture2D tex, Rectangle dst, Rectangle? source = null, Align anchor = Align.TopLeft, Align pivot = Align.Undef, Color? col = null, bool flip = false, float rot = 0) {
            if (pivot == Align.Undef) pivot = anchor;
            if (flip) {
                dst.X *= -1; rot *= -1;
                pivot = Flip(pivot); anchor = Flip(anchor);
            }
            Rectangle src = source ?? tex.Bounds;
            Vector2 origin = PivotPoint(pivot, src).ToVector2();//is relative to the texture
            dst.Location += AnchorPos(anchor);// - PivotPoint(pivot, dst); // not needed as the origin takes care of that
            //dst.Location += AnchorPos(anchor) - PivotPoint(pivot, dst) + origin.ToPoint();

            sB.Draw(tex, dst, src, (col ?? Color.White), MathHelper.ToRadians(rot), origin, (flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None), 1);

        }

        //Rotation not supported
        public static void DrawString(string text, Vector2 pos, Align anchor = Align.TopLeft, Align pivot = Align.Undef, Align paragraph = Align.Undef, Color? col = null, bool flip = false, float scale = 1, bool bold = false) { // bounds includes position offset and rectangle size
            SpriteFont font = bold ? archerFont : comicFont;
            Rectangle dest = new Rectangle(pos.ToPoint(), (font.MeasureString(text) * scale).ToPoint());
            DrawString(text, dest, anchor, pivot, paragraph, col, flip, scale, bold);
        }

        public static void DrawString(string text, Rectangle dst, Align anchor = Align.TopLeft, Align pivot = Align.Undef, Align paragraph = Align.Undef, Color? col = null, bool flip = false, float scale=1, bool bold = false) { // bounds includes position offset and rectangle size
            if (pivot == Align.Undef) pivot = anchor;
            if (paragraph == Align.Undef) paragraph = anchor;
            if (flip) {
                dst.X *= -1;
                pivot = Flip(pivot); anchor = Flip(anchor); paragraph = Flip(paragraph);
            }
            SpriteFont font = bold ? archerFont : comicFont;
            Rectangle src = new Rectangle(Point.Zero, (font.MeasureString(text)*scale).ToPoint());
            Rectangle diff = new Rectangle(Point.Zero,  dst.Size - src.Size);

            Vector2 origin = PivotPoint(pivot, src).ToVector2();
            dst.Location += AnchorPos(anchor) - PivotPoint(pivot, dst) + PivotPoint(paragraph, diff);//required bc pivot isn't used in call code

            sB.DrawString(font, text, dst.Location.ToVector2(), (col ?? Color.White), 0, Vector2.Zero, scale, SpriteEffects.None, 1);
        }

        static Point AnchorPos(Align anchor, bool splitScreen = true) { // computes the position on the screen where to align the rectangle
            return splitScreen ? PivotPoint(anchor, Screen.Split) : PivotPoint(anchor, Screen.Full);
        }

        static Point PivotPoint(Align align, Rectangle rect) { // computes the pivot of a given rectangle
            int pivotPosX = align.HasFlag(Align.Left) ? 0 : align.HasFlag(Align.Right) ? rect.Width : rect.Width / 2;
            int pivotPosY = align.HasFlag(Align.Top) ? 0 : align.HasFlag(Align.Bottom) ? rect.Height : rect.Height / 2;
            return new Point(pivotPosX, pivotPosY);
        }

        public static Align Flip(Align al) { // switches left/right flag if present
            if (al.HasFlag(Align.Left)) al += Align.Right - Align.Left;
            else if (al.HasFlag(Align.Right)) al -= Align.Right - Align.Left;
            return al;
        }



        //OLD CODE
        /*
           Vector2 origin = size * 0.5f;
           Vector2 pos = dst.GetCenter();
           //Vector2 pos = dst.Location.ToVector2();
           if (paragraph.HasFlag(Align.Left)) pos.X += dst.Width / 2  - size.X / 2;
           if (paragraph.HasFlag(Align.Right)) pos.X -= dst.Width / 2  - size.X / 2;
           if (paragraph.HasFlag(Align.Top)) pos.Y += dst.Height / 2 - size.Y / 2;
           if (paragraph.HasFlag(Align.Bottom)) pos.Y -= dst.Height / 2 - size.Y / 2;
           */


        //int pivotPosX = paragraph.HasFlag(Align.Left) ? 0 : paragraph.HasFlag(Align.Right) ? diff.X : diff.X / 2;
        //int pivotPosY = paragraph.HasFlag(Align.Top) ? 0 : paragraph.HasFlag(Align.Bottom) ? diff.Y : diff.Y / 2;


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

        /*
        Rectangle AlignRect(Rectangle rect, Align anchor, Align pivot) {
            rect.Location += AnchorPos(anchor)-PivotPoint(pivot, rect);
            return rect;
        }*/



    }


}