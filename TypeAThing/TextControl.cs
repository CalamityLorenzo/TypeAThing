using GameLibrary.Extensions;
using System.Text;

namespace TypeAThing
{
    public class TextControl : DrawableGameComponent
    {
        private readonly SpriteBatch sb;
        private readonly SpriteFont fnt;
        private TypedTextSource textSource;
        private Texture2D Cursor { get; }
        private int _cursorCharPosition;
        private Stream MemStream;
        private List<char> currentLine = new();
        private List<string> AllLines = new();

        private bool isEnterPressed;
        private bool isLeftArrowPressed;
        private bool isRightArrowPressed;

        public TextControl(Game game, SpriteBatch sb, SpriteFont fnt) : base(game)
        {
            this.textSource = new TypedTextSource(game.Window);
            this.sb = sb;
            this.fnt = fnt;
            var fntWidth = fnt.MeasureString("0");
            this.Cursor = sb.CreateFilledRectTexture(new Rectangle(0, 0, (int)fntWidth.X, (int)fntWidth.Y), Color.White);
            this._cursorCharPosition = 0;
            this.MemStream = new MemoryStream();
        }

        public TextControl(Game game, SpriteBatch sb, SpriteFont fnt, Stream writeableStream) : this(game, sb, fnt)
        {
            this.MemStream = writeableStream;
        }

        public override void Update(GameTime gameTime)
        {
            CheckTextBuffer();
            var kboard = Keyboard.GetState();
            CheckNonCharacterKeys(kboard);
        }

        private void CheckNonCharacterKeys(KeyboardState kboard)
        {
            if (kboard.IsKeyDown(Keys.Enter) && isEnterPressed == false)
            {
                isEnterPressed = true;
                this.AllLines.Add(String.Join("", currentLine.ToArray()) + "\n");
                this.WriteToOutputStream();
                this.currentLine.Clear();
                _cursorCharPosition = 0;
            }

            if (kboard.IsKeyDown(Keys.Left) && isLeftArrowPressed == false)
            {
                isLeftArrowPressed = true;
                if (_cursorCharPosition == 0) return;

                _cursorCharPosition -= 1;
            }

            if (kboard.IsKeyDown(Keys.Right) && isRightArrowPressed == false)
            {
                isRightArrowPressed = true;
                if (_cursorCharPosition == currentLine.Count) return;
                _cursorCharPosition += 1;
            }

            if (kboard.IsKeyUp(Keys.Enter) && isEnterPressed) isEnterPressed = false;
            if (kboard.IsKeyUp(Keys.Left) && isLeftArrowPressed) isLeftArrowPressed = false;
            if (kboard.IsKeyUp(Keys.Right) && isRightArrowPressed) isRightArrowPressed = false;
        }

        private void CheckTextBuffer()
        {
            if (this.textSource.TextStreamLength > 0)
            {
                var chars = this.textSource.GetCurrentCharacters();
                for (var x = 0; x < chars.Length; x++)
                {
                    var charA = (char)chars[x];
                    if (charA == '\b')
                    { // backspace
                        if (_cursorCharPosition != 0)
                        {
                            this.currentLine.RemoveAt(_cursorCharPosition - 1);
                            _cursorCharPosition -= 1;
                        }
                        else
                        {
                            if (this.currentLine.Count > 0)
                                this.currentLine.RemoveAt(_cursorCharPosition);
                        }

                    }
                    else if (charA == '\u007f')  // forward del
                    {
                        if (this.currentLine.Count > 0 && _cursorCharPosition < currentLine.Count)
                        {
                            this.currentLine.RemoveAt(_cursorCharPosition);
                        }
                    }
                    else if (charA == 27) // escape
                    {
                        this.currentLine = new List<char>();
                        _cursorCharPosition = 0;
                    }
                    else // Append character and move the cursor along.
                    {
                        this.currentLine.Insert(_cursorCharPosition, charA);
                        _cursorCharPosition += 1;
                    }
                }
            }
        }

        private void WriteToOutputStream()
        {
            // Nothing is known about the stream as the outside can manipulate it.
            // so just write, and move on.
            using StreamWriter sw = new StreamWriter(MemStream, Encoding.UTF8, bufferSize: (int)this.currentLine.Count, leaveOpen: true);
            sw.WriteLine(this.currentLine.ToArray(),0, this.currentLine.Count);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            var cLine = String.Join("", currentLine);
            var cursorPoint = fnt.MeasureString(cLine.Substring(0, _cursorCharPosition));
            sb.DrawString(fnt, cLine, new Vector2(20, 20), Color.White);
            sb.Draw(this.Cursor, new Vector2(20 + cursorPoint.X, 20), Color.White);
            // If the cursor is over a character, we want to paint the 'reverese' colour of the character
            if (_cursorCharPosition != currentLine.Count)
            {
                sb.DrawString(fnt, cLine.Substring(_cursorCharPosition, 1), new Vector2(20 + cursorPoint.X, 20), Color.Black);
            }
        }
    }
}
