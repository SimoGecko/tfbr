// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using System;
using BRS.Engine.Utilities;
using BRS.Menu;
using BRS.Scripts;
using BRS.Scripts.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BRS.Engine {

    [Flags]
    public enum Align { Center = 0, Left = 1, Right = 2, Top = 4, Bottom = 8, TopLeft = 5, TopRight = 6, BotLeft = 9, BotRight = 10 }
    //public enum Align2 { M=0, L=1, R=2, T=4, B=8, TL=5, TR=6, BL=9, BR=10 }
    //public enum Align3 { TL, RM, TR, ML, MM, MR, BL, BM, BR} // (top, middle, bottom) x (left, middle, right)

    class UserInterface {
        ////////// acts as HUB to draw everything related to the UI, either in splitscreen (each window) or global (just once) //////////

        // --------------------- VARIABLES ---------------------

        //TODO make it agnostic to this game

        //public
        public const int BarWidth = 128;
        public const int BarBigWidth = 256;
        public const int BarHeight = 16;


        //private
        public SpriteFont SmallFont { get; private set; }
        public SpriteFont ComicFont { get; private set; }
        public SpriteFont BigFont   { get; private set; }

        private Texture2D _bar, _barBig;
        private Texture2D _white;
        private Texture2D test_grid;

        private Rectangle _barRect, _bigRect, _smallRect;

        //reference
        public static UserInterface Instance;
        private SpriteBatch _sb;


        // --------------------- BASE METHODS ------------------
        public UserInterface() { Instance = this; }

        public void Start() {
            Instance = this;
            SmallFont = File.Load<SpriteFont>("Other/font/font1");
            ComicFont = File.Load<SpriteFont>("Other/font/comicFont");
            BigFont   = File.Load<SpriteFont>("Other/font/font2");

            _bar       = File.Load<Texture2D>("Images/UI/progress_bar_small");
            _barBig    = File.Load<Texture2D>("Images/UI/progress_bar");
            _white     = File.Load<Texture2D>("Images/UI/white");
            test_grid = File.Load<Texture2D>("Images/UI/test_grid");

            _barRect = new Rectangle(0, 0, BarWidth, BarHeight);
            _bigRect = new Rectangle(0, 0, BarBigWidth, BarHeight);
            _smallRect = new Rectangle(0, 0, BarBigWidth/4, BarHeight/4);
            //fgRect = new Rectangle(0, BARHEIGHT, BARWIDTH, BARHEIGHT);
        }



        // ---------- CALLBACKS ----------

        public void DrawMenu(SpriteBatch spriteBatch) {
            _sb = spriteBatch;
            MenuManager.Instance.Draw();
        }

        public void DrawGlobal(SpriteBatch spriteBatch) {
            _sb = spriteBatch;
            //callbacks
            //Minimap.Instance.Draw(_sb);
            GameUI.Instance.Draw();
            //Heatmap.instance.Draw();
            
        }

        public void DrawSplitscreen(SpriteBatch spriteBatch, int index) { // call all subcomponents that are drawn on each split screen
            _sb = spriteBatch;
            //callbacks

            
            BaseUI.Instance.Draw(index%2);
            PlayerUI.Instance.Draw(index);
            PowerupUI.Instance.Draw(index);
            Suggestions.Instance.Draw(index);
            Minimap.Instance.DrawSmall(spriteBatch, index);
            MoneyUI.Instance.Draw(index);

            //test draw
            /*
            Rectangle src = new Rectangle(0, 0, 64, 64);
            Rectangle dst = new Rectangle(100, 100, 128, 128);
            Rectangle dst2 = new Rectangle(-100, -100, 128, 128);
            
            DrawPictureAlign(test_grid, dst, src, Align.TopLeft, Align.Center, rot: Time.CurrentTime * 0, flip: false);
            DrawPictureAlign(test_grid, dst2, src, Align.BotRight, Align.Center, rot: Time.CurrentTime * 0, flip: false);

            DrawStringAlign("BR", dst, Align.TopLeft, Align.Center, Align.BotRight, scale: .5f);
            DrawStringAlign("BL", dst, Align.TopLeft, Align.Center, Align.BotLeft);
            DrawStringAlign("TR", dst, Align.TopLeft, Align.Center, Align.TopRight, scale:2f);
            DrawStringAlign("TL", dst, Align.TopLeft, Align.Center, Align.TopLeft);

            DrawStringAlign("BR", dst2, Align.BotRight, Align.Center, Align.BotRight, scale: .5f, flip:false);
            DrawStringAlign("BL", dst2, Align.BotRight, Align.Center, Align.BotLeft, flip: false);
            DrawStringAlign("TR", dst2, Align.BotRight, Align.Center, Align.TopRight, scale: 2f, flip: false);
            DrawStringAlign("TL", dst2, Align.BotRight, Align.Center, Align.TopLeft, flip: false);*/
        }





        // --------------------- CUSTOM METHODS ----------------

        //DRAW CALLBACKS

        //BARS
        public void DrawBar(Vector2 position, float percent, Color color) {
            _barRect.Width = BarWidth;
            _sb.Draw(_bar, position, _barRect, Color.LightGray);
            _barRect.Width = (int)(BarWidth * percent);
            _sb.Draw(_bar, position, _barRect, color);
        }
        public void DrawBarVertical(Vector2 position, float percent, Color color) {
            _barRect.Width = BarWidth;
            //sB.Draw(bar, position, barRect, Color.LightGray);
            _sb.Draw(_bar, position, _barRect, Color.LightGray, MathHelper.ToRadians(-90), Vector2.Zero, 1f, SpriteEffects.None, 1);
            _barRect.Width = (int)(BarWidth * percent);
            //sB.Draw(bar, position, barRect, color);
            _sb.Draw(_bar, position, _barRect, color, MathHelper.ToRadians(-90), Vector2.Zero, 1f, SpriteEffects.None, 1);
        }
        public void DrawBarBig(Vector2 position, float percent, Color color) {
            _bigRect.Width = BarBigWidth;
            _sb.Draw(_barBig, position, _bigRect, Color.LightGray);
            _bigRect.Width = (int)(BarBigWidth * percent);
            _sb.Draw(_barBig, position, _bigRect, color);
        }
        public void DrawBarSmall(Vector2 position, float percent, Color color) {
            _bigRect.Width = BarBigWidth;
            _sb.Draw(_barBig, position, _bigRect, Color.LightGray, 0, Vector2.Zero, .25f, SpriteEffects.None, 1);
            _bigRect.Width = (int)(BarBigWidth * percent);
            _sb.Draw(_barBig, position, _bigRect, color, 0, Vector2.Zero, .25f, SpriteEffects.None, 1);
        }


        public void DrawString(Vector2 position, string text, Color colour = default(Color)) {
            _sb.DrawString(SmallFont, text, position, colour == default(Color)? Color.White : colour);
        }
        public void DrawStringBig(Vector2 position, string text, Color colour = default(Color)) {
            _sb.DrawString(BigFont, text, position, colour == default(Color) ? Color.White : colour);
        }

        public void DrawPicture(Rectangle destination, Texture2D pic, Color colour = default(Color)) {
            _sb.Draw(pic, destination, colour == default(Color) ? Color.White : colour);
        }

        public void DrawPicture(Vector2 position, Texture2D pic, Vector2 origin, float scale) {
            _sb.Draw(pic, position, null, Color.White, 0, origin, scale, SpriteEffects.None, 1);
        }
        public void DrawPicture(Rectangle destination, Texture2D pic, Rectangle source) {
            _sb.Draw(pic, destination, source, Color.White);
        }
        public void DrawPicture(Rectangle dest, Texture2D pic, Rectangle src, float rotation) {
            Vector2 origin = new Vector2(src.Width / 2, src.Height / 2);
            _sb.Draw(pic, dest, src, Color.White, MathHelper.ToRadians(rotation), origin, SpriteEffects.None, 1);
        }



        //queries
        public int GetOffset(int index) { return index % 2 == 0 ? 0 : +680; }




        // --------------------- ALIGN ----------------

        //pivot  = where is the center of the rectangle
        //anchor = which corner of the screen to follow
        //paragraph = how to align the paragraph

        public void DrawPictureAlign(Texture2D tex, Rectangle dst, Rectangle? source, Align anchor, Align pivot, Color? col = null, bool flip = false, float rot = 0) {
            if (flip) {
                dst.X *= -1; rot *= -1;
                pivot = Flip(pivot); anchor = Flip(anchor);
            }
            Rectangle src = source ?? tex.Bounds;
            Vector2 origin = PivotPoint(pivot, src).ToVector2();//is relative to the texture
            dst.Location += AnchorPos(anchor);// - PivotPoint(pivot, dst); // not needed as the origin takes care of that

            _sb.Draw(tex, dst, src, (col ?? Color.White), MathHelper.ToRadians(rot), origin, (flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None), 1);
        }

        //Rotation not supported
        public void DrawStringAlign(string text, Rectangle dst, Align anchor, Align pivot, Align paragraph, Color? col = null, bool flip = false, float scale=1) { // bounds includes position offset and rectangle size
            if (flip) {
                dst.X *= -1;
                pivot = Flip(pivot); anchor = Flip(anchor); paragraph = Flip(paragraph);
            }
            Rectangle src = new Rectangle(Point.Zero, (SmallFont.MeasureString(text)*scale).ToPoint());
            Rectangle diff = new Rectangle(Point.Zero,  dst.Size - src.Size);

            Vector2 origin = PivotPoint(pivot, src).ToVector2();
            dst.Location += AnchorPos(anchor) - PivotPoint(pivot, dst) + PivotPoint(paragraph, diff);//required bc pivot isn't used in call code

            _sb.DrawString(SmallFont, text, dst.Location.ToVector2(), (col ?? Color.White), 0, Vector2.Zero, scale, (flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None), 1);
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

        Point AnchorPos(Align anchor, bool splitScreen = true) { // computes the position on the screen where to align the rectangle
            return splitScreen ? PivotPoint(anchor, Screen.Split) : PivotPoint(anchor, Screen.Full);
        }

        Point PivotPoint(Align align, Rectangle rect) { // computes the pivot of a given rectangle
            int pivotPosX = align.HasFlag(Align.Left) ? 0 : align.HasFlag(Align.Right) ? rect.Width : rect.Width / 2;
            int pivotPosY = align.HasFlag(Align.Top) ? 0 : align.HasFlag(Align.Bottom) ? rect.Height : rect.Height / 2;
            return new Point(pivotPosX, pivotPosY);
        }

        Align Flip(Align al) { // switches left/right flag if present
            if      (al.HasFlag(Align.Left))  al += Align.Right - Align.Left;
            else if (al.HasFlag(Align.Right)) al -= Align.Right - Align.Left;
            return al;
        }

    }


}