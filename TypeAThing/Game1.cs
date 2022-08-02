using System.Diagnostics;
using System.Text;
namespace TypeAThing
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private SpriteFont arial;
        private SpriteFont arialBold;
        private TextControl tts;
        private MemoryStream ms = new MemoryStream();
        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            this.arial = Content.Load<SpriteFont>("Arial");
            this.arialBold = Content.Load<SpriteFont>("ArialBold");
            // TODO: use this.Content to load your game content here
            this.tts = new TextControl(this, this._spriteBatch,  arialBold, ms);
            this.Components.Add(tts);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                Exit();
            CheckTheMs();
            // TODO: Add your update logic here
            base.Update(gameTime);
        }

        private void CheckTheMs()
        {
            if (ms.Length > 0)
            {
                ms.Position = 0;
                StreamReader sr = new StreamReader(ms, Encoding.UTF8, bufferSize: (int)ms.Length, leaveOpen:true);
                var thetext = sr.ReadLine();
                Debug.WriteLine(thetext);
                ms.SetLength(0);
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            this._spriteBatch.Begin();
            this.tts.Draw(gameTime);
            this._spriteBatch.End();
        }
    }
}