using UnityEngine;

namespace Rotatris.Creation
{
    /// <summary>Builds the single generated white square sprite every cell view shares.</summary>
    public static class SquareSpriteFactory
    {
        public static Sprite Build()
        {
            var tex = new Texture2D(4, 4, TextureFormat.RGBA32, false) { filterMode = FilterMode.Point };
            var pixels = new Color[16];
            for (int i = 0; i < pixels.Length; i++) pixels[i] = Color.white;
            tex.SetPixels(pixels);
            tex.Apply();
            return Sprite.Create(tex, new Rect(0, 0, 4, 4), new Vector2(0.5f, 0.5f), 4f);
        }
    }
}