namespace GameLibrary.Extensions
{
    public static class SpriteBatchExtensions
    {
        private static Lazy<Texture2D> OnePixel;

        private static Texture2D GetOnePixelTexture(GraphicsDevice device)
        {
            if (OnePixel == null)
            {
                var textureData = new Color[] { Color.White };
                OnePixel = new Lazy<Texture2D>(() => new Texture2D(device, 1, 1));
                OnePixel.Value.SetData<Color>(textureData);
            }
            return OnePixel.Value;
        }

        public static void DrawFilledRect(this SpriteBatch @this, Vector2 start, int width, int height, Color Colour)
        {
            var texture = SpriteBatchExtensions.GetOnePixelTexture(@this.GraphicsDevice);

            @this.Draw(texture, new Rectangle(start.ToPoint(), new Point(width, height)), Colour);

        }

        public static Texture2D CreateFilledRectTexture(this SpriteBatch @this, Rectangle dimensions, Color colour)
        {
            var filledColour = Enumerable.Range(0, dimensions.Width * dimensions.Height).Select(o=>colour);
            var texture = new Texture2D(@this.GraphicsDevice, dimensions.Width, dimensions.Height);
            texture.SetData<Color>(filledColour.ToArray());
            return texture;
        }

    }
}
