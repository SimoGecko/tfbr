// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using BRS.Menu;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

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
        public const int BarWidth = 128;
        public const int BarBigWidth = 256;
        public const int BarHeight = 16;



        //private
        public SpriteFont SmallFont { get; private set; }
        public SpriteFont ComicFont { get; private set; }
        public SpriteFont BigFont { get; private set; }
        private Texture2D _bar;
        private Texture2D _barBig;

        private Rectangle _barRect, _bigRect, _smallRect;
        private Texture2D _white;

        //reference
        public static UserInterface Instance;
        private SpriteBatch _sb;


        // --------------------- BASE METHODS ------------------
        public void Start() {
            Instance = this;
            SmallFont = File.Load<SpriteFont>("Other/font/font1");
            ComicFont = File.Load<SpriteFont>("Other/font/comicFont");
            BigFont   = File.Load<SpriteFont>("Other/font/font2");

            _bar       = File.Load<Texture2D>("Images/UI/progress_bar_small");
            _barBig    = File.Load<Texture2D>("Images/UI/progress_bar");
            _white     = File.Load<Texture2D>("Images/UI/white");

            _barRect = new Rectangle(0, 0, BarWidth, BarHeight);
            _bigRect = new Rectangle(0, 0, BarBigWidth, BarHeight);
            _smallRect = new Rectangle(0, 0, BarBigWidth/4, BarHeight/4);
            //fgRect = new Rectangle(0, BARHEIGHT, BARWIDTH, BARHEIGHT);
        }

        public void DrawMenu(SpriteBatch spriteBatch) {
            _sb = spriteBatch;
            MenuManager.Instance.Draw();
        }

        public void DrawGlobal(SpriteBatch spriteBatch) {
            _sb = spriteBatch;
            //callbacks
            //Minimap.instance.Draw(sB);
            GameUI.Instance.Draw();
        }

        public void DrawSplitscreen(SpriteBatch spriteBatch, int index) { // call all subcomponents that are drawn on each split screen
            _sb = spriteBatch;
            //callbacks
            
            /*
            BaseUI.instance.Draw(index%2);
            PlayerUI.instance.Draw(index);
            PowerupUI.instance.Draw(index);
            Suggestions.instance.Draw(index);
            Minimap.instance.DrawSmall(spriteBatch, index);
            */

            //test draw
            
            DrawPictureAlign(_white, new Rectangle(53, 53, 100, 100), null, Align.TopLeft, Align.Center, Color.Gray, false);
            //DrawPictureAlign(white, new Rectangle(100, 100, 150, 100), null, Align.TopLeft, Align.TopLeft, Color.LightGray, true);

            DrawStringAlign("Text one", new Rectangle(10, 10, 300, 100), Align.TopLeft, Align.TopLeft, Align.BotRight, Color.Black);
            DrawStringAlign("Text two", new Rectangle(10, 10, 300, 100), Align.TopLeft, Align.TopLeft, Align.BotRight, Color.Black, true);
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



        

        //pivot  = where is the center of the rectangle
        //anchor = which corner of the screen to follow
        //paragraph = how to align the paragraph
        public void DrawStringAlign(string text, Rectangle bounds, Align anchor, Align pivot, Align paragraph, Color? col, bool flip = false) { // bounds includes position offset and rectangle size
            if (flip) {
                bounds.X *= -1;
                pivot = Flip(pivot); anchor = Flip(anchor); paragraph = Flip(paragraph);
            }
            bounds = AlignRect(bounds, anchor, pivot);
            
            Color color = col ?? Color.White;

            Vector2 size = SmallFont.MeasureString(text);
            Vector2 pos = bounds.GetCenter();
            Vector2 origin = size * 0.5f;

            if (paragraph.HasFlag(Align.Left))   origin.X += bounds.Width / 2  - size.X / 2;
            if (paragraph.HasFlag(Align.Right))  origin.X -= bounds.Width / 2  - size.X / 2;
            if (paragraph.HasFlag(Align.Top))    origin.Y += bounds.Height / 2 - size.Y / 2;
            if (paragraph.HasFlag(Align.Bottom)) origin.Y -= bounds.Height / 2 - size.Y / 2;
            
            _sb.DrawString(SmallFont, text, pos, color, 0, origin, 1, SpriteEffects.None, 0);
        }

        public void DrawPictureAlign(Texture2D tex, Rectangle bounds, Rectangle? source, Align anchor, Align pivot, Color? col, bool flip = false, float rot = 0) {
            if (flip) {
                bounds.X *= -1;
                pivot = Flip(pivot); anchor = Flip(anchor);
            }
            Vector2 origin = PivotPoint(pivot, tex.Bounds).ToVector2();//is relative to the texture
            bounds.Location += AnchorPos(anchor) - PivotPoint(pivot, bounds) + origin.ToPoint();

            Color color = col ?? Color.White;
            _sb.Draw(tex, bounds, source, color, MathHelper.ToRadians(rot), origin, flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 1);
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
            int anchorPosX = anchor.HasFlag(Align.Left) ? 0 : anchor.HasFlag(Align.Right) ? Screen.SplitWidth : Screen.SplitWidth / 2;
            int anchorPosY = anchor.HasFlag(Align.Top) ? 0 : anchor.HasFlag(Align.Bottom) ? Screen.SplitHeight : Screen.SplitHeight / 2;
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