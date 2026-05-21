using UnityEngine;

namespace RicochetBrigade
{
    public static class GreyboxSprites
    {
        private static Sprite square;
        private static Sprite circle;
        private static Sprite triangle;

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

        public static Sprite Triangle
        {
            get
            {
                if (triangle == null)
                {
                    const int size = 64;
                    Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
                    texture.filterMode = FilterMode.Bilinear;

                    Vector2 top = new Vector2(size * 0.5f, size - 3f);
                    Vector2 left = new Vector2(4f, 4f);
                    Vector2 right = new Vector2(size - 4f, 4f);

                    for (int y = 0; y < size; y++)
                    {
                        for (int x = 0; x < size; x++)
                        {
                            Vector2 point = new Vector2(x, y);
                            float alpha = IsInsideTriangle(point, top, left, right) ? 1f : 0f;
                            texture.SetPixel(x, y, new Color(1f, 1f, 1f, alpha));
                        }
                    }

                    texture.Apply();
                    triangle = Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), size);
                }

                return triangle;
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

        private static bool IsInsideTriangle(Vector2 point, Vector2 a, Vector2 b, Vector2 c)
        {
            float area = 0.5f * (-b.y * c.x + a.y * (-b.x + c.x) + a.x * (b.y - c.y) + b.x * c.y);
            float sign = area < 0f ? -1f : 1f;
            float s = (a.y * c.x - a.x * c.y + (c.y - a.y) * point.x + (a.x - c.x) * point.y) * sign;
            float t = (a.x * b.y - a.y * b.x + (a.y - b.y) * point.x + (b.x - a.x) * point.y) * sign;

            return s >= 0f && t >= 0f && s + t <= 2f * area * sign;
        }
    }
}
