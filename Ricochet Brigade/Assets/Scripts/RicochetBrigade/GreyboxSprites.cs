using UnityEngine;

namespace RicochetBrigade
{
    public static class GreyboxSprites
    {
        private static Sprite square;
        private static Sprite circle;

        public static Sprite Square
        {
            get
            {
                if (square == null)
                {
                    Texture2D texture = new Texture2D(8, 8, TextureFormat.RGBA32, false);
                    texture.filterMode = FilterMode.Point;
                    Fill(texture, true);
                    square = Sprite.Create(texture, new Rect(0, 0, 8, 8), new Vector2(0.5f, 0.5f), 8f);
                }

                return square;
            }
        }

        public static Sprite Circle
        {
            get
            {
                if (circle == null)
                {
                    const int size = 64;
                    Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
                    texture.filterMode = FilterMode.Bilinear;
                    Vector2 center = new Vector2((size - 1) * 0.5f, (size - 1) * 0.5f);
                    float radius = (size - 2) * 0.5f;

                    for (int y = 0; y < size; y++)
                    {
                        for (int x = 0; x < size; x++)
                        {
                            float alpha = Vector2.Distance(new Vector2(x, y), center) <= radius ? 1f : 0f;
                            texture.SetPixel(x, y, new Color(1f, 1f, 1f, alpha));
                        }
                    }

                    texture.Apply();
                    circle = Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), size);
                }

                return circle;
            }
        }

        private static void Fill(Texture2D texture, bool opaque)
        {
            Color color = opaque ? Color.white : Color.clear;
            for (int y = 0; y < texture.height; y++)
            {
                for (int x = 0; x < texture.width; x++)
                {
                    texture.SetPixel(x, y, color);
                }
            }

            texture.Apply();
        }
    }
}
