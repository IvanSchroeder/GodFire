using UnityEngine;
using System;
using UnityUtilities;

namespace UnityUtilities {
    /// <summary>
    /// Extensions for Colors.
    /// </summary>
    public static class ColorExtensions {
        public static Color SetRGBA(this Color c, float? r = null, float? g = null, float? b = null, float? a = null) => new Color(r ?? c.r, g ?? c.g, b ?? c.b, a ?? c.a);
		public static Color SetR(this Color c, float r) => new Color(r, c.g, c.b, c.a);
		public static Color SetG(this Color c, float g) => new Color(c.r, g, c.b, c.a);
		public static Color SetB(this Color c, float b) => new Color(c.r, c.g, b, c.a);
		public static Color SetA(this Color c, float a) => new Color(c.r, c.g, c.b, a);

		public static Color SetRGBA(this Color c, Color c2, float? a = null) => new Color(c2.r, c2.g, c2.b, a ?? c2.a);
		public static Color SetR(this Color c, Color c2) => new Color(c2.r, c.g, c.b, c.a);
		public static Color SetG(this Color c, Color c2) => new Color(c.r, c2.g, c.b, c.a);
		public static Color SetB(this Color c, Color c2) => new Color(c.r, c.g, c2.b, c.a);
		public static Color SetA(this Color c, Color c2) => new Color(c.r, c.g, c.b, c2.a);

		public static Color GetRGBA(this Color c, float? a = null) => new Color(c.r, c.g, c.b, a ?? c.a);
		public static Color GetR(this Color c, float? a = null) => new Color(c.r, 0f, 0f, a ?? c.a);
		public static Color GetG(this Color c, float? a = null) => new Color(0f, c.g, 0f, a ?? c.a);
		public static Color GetB(this Color c, float? a = null) => new Color(0f, 0f, c.b, a ?? c.a);
		public static Color GetA(this Color c) => new Color(0f, 0f, 0f, c.a);
		public static float GetRValue(this Color c) => c.r;
		public static float GetGValue(this Color c) => c.g;
		public static float GetBValue(this Color c) => c.b;
		public static float GetAValue(this Color c) => c.a;

        // /// <summary>
        // /// Sets the alpha component of the color.
        // /// </summary>
        // /// <param name="color">The original color.</param>
        // /// <param name="alpha">The new alpha value.</param>
        // /// <returns>A new color with the specified alpha value.</returns>
        // public static Color SetAlpha(this Color color, float alpha)
        //     => new(color.r, color.g, color.b, alpha);

        /// <summary>
        /// Adds the RGBA components of two colors and clamps the result between 0 and 1.
        /// </summary>
        /// <param name="thisColor">The first color.</param>
        /// <param name="otherColor">The second color.</param>
        /// <returns>A new color that is the sum of the two colors, clamped between 0 and 1.</returns>
        public static Color Add(this Color thisColor, Color otherColor)
            => (thisColor + otherColor).Clamp01();

        /// <summary>
        /// Subtracts the RGBA components of one color from another and clamps the result between 0 and 1.
        /// </summary>
        /// <param name="thisColor">The first color.</param>
        /// <param name="otherColor">The second color.</param>
        /// <returns>A new color that is the difference of the two colors, clamped between 0 and 1.</returns>
        public static Color Subtract(this Color thisColor, Color otherColor)
            => (thisColor - otherColor).Clamp01();

        /// <summary>
        /// Clamps the RGBA components of the color between 0 and 1.
        /// </summary>
        /// <param name="color">The original color.</param>
        /// <returns>A new color with each component clamped between 0 and 1.</returns>
        static Color Clamp01(this Color color) {
            return new Color {
                r = Mathf.Clamp01(color.r),
                g = Mathf.Clamp01(color.g),
                b = Mathf.Clamp01(color.b),
                a = Mathf.Clamp01(color.a)
            };
        }

        /// <summary>
        /// Converts a Color to a hexadecimal string.
        /// </summary>
        /// <param name="color">The color to convert.</param>
        /// <returns>A hexadecimal string representation of the color.</returns>
        public static string ToHex(this Color color)
            => $"#{ColorUtility.ToHtmlStringRGBA(color)}";

        /// <summary>
        /// Converts a hexadecimal string to a Color.
        /// </summary>
        /// <param name="hex">The hexadecimal string to convert.</param>
        /// <returns>The Color represented by the hexadecimal string.</returns>
        public static Color FromHex(this string hex) {
            if (ColorUtility.TryParseHtmlString(hex, out Color color)) {
                return color;
            }

            throw new ArgumentException("Invalid hex string", nameof(hex));
        }

        /// <summary>
        /// Blends two colors with a specified ratio.
        /// </summary>
        /// <param name="color1">The first color.</param>
        /// <param name="color2">The second color.</param>
        /// <param name="ratio">The blend ratio (0 to 1).</param>
        /// <returns>The blended color.</returns>
        public static Color Blend(this Color color1, Color color2, float ratio) {
            ratio = Mathf.Clamp01(ratio);
            return new Color(
                color1.r * (1 - ratio) + color2.r * ratio,
                color1.g * (1 - ratio) + color2.g * ratio,
                color1.b * (1 - ratio) + color2.b * ratio,
                color1.a * (1 - ratio) + color2.a * ratio
            );
        }

        /// <summary>
        /// Inverts the color.
        /// </summary>
        /// <param name="color">The color to invert.</param>
        /// <returns>The inverted color.</returns>
        public static Color Invert(this Color color)
            => new(1 - color.r, 1 - color.g, 1 - color.b, color.a);

        /// <summary>
        /// Hue is returned as an angle in degrees around the standard hue colour wheel.
        /// See: http://en.wikipedia.org/wiki/HSL_and_HSV
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static float ExtractHue(this Color c) {
            var max = Mathf.Max(c.r, Mathf.Max(c.g, c.b));
            var min = Mathf.Min(c.r, Mathf.Min(c.g, c.b));
            var delta = max - min;
            if (Mathf.Abs(delta) < 0.0001f) return 0f;
            else if(c.r >= c.g && c.r >= c.b) return 60f * (((c.g - c.b) / delta) % 6f);
            else if(c.g >= c.b) return 60f * ((c.b - c.r) / delta + 2f);
            else return 60f * ((c.r - c.g) / delta + 4f);
        }

		/// <summary>
        /// Returns the value of an RGB color. This can be used in an HSV representation of a colour.
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static float ExtractValue(this Color c) => Mathf.Max(c.r, Mathf.Max(c.g, c.b));

		public static float ExtractSaturation(this Color c, bool hsv = true) {
			var max = Mathf.Max(c.r, Mathf.Max(c.g, c.b));
			var min = Mathf.Min(c.r, Mathf.Min(c.g, c.b));

			if (hsv) {
				//ala HSV formula
				if (Mathf.Abs(max) < 0.0001f) return 0f;
				return (max - min) / max;
			}
			
			////ala HSL formula
			var delta = max - min;
			if (Mathf.Abs(delta) < 0.0001f)
				return 0f;
			return delta / (1f - Mathf.Abs(max + min - 1f));
        }

		/// <summary>
        /// Unity's Color.Lerp clamps between 0->1, this allows a true lerp of all ranges.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static Color Lerp(Color a, Color b, float t) => new Color(a.r + (b.r - a.r) * t, a.g + (b.g - a.g) * t, a.b + (b.b - a.b) * t, a.a + (b.a - a.a) * t);

        /// <summary>
        /// Unity's Color32.Lerp clamps between 0->1, this allows a true lerp of all ranges.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static Color32 Lerp(Color32 a, Color32 b, float t) {
            return new Color32((byte)Utils.Clamp((float)a.r + (float)((int)b.r - (int)a.r) * t, 0, 255), 
                               (byte)Utils.Clamp((float)a.g + (float)((int)b.g - (int)a.g) * t, 0, 255), 
                               (byte)Utils.Clamp((float)a.b + (float)((int)b.b - (int)a.b) * t, 0, 255), 
                               (byte)Utils.Clamp((float)a.a + (float)((int)b.a - (int)a.a) * t, 0, 255));
        }

		public static Color Lerp(float t, params Color[] colors) {
            if (colors == null || colors.Length == 0) return Color.black;
            if (colors.Length == 1) return colors[0];

            int i = Mathf.FloorToInt(colors.Length * t);
            if (i < 0) i = 0;
            if (i >= colors.Length - 1) return colors[colors.Length - 1];
            
            t %= 1f / (float)(colors.Length - 1);
            return Color.Lerp(colors[i], colors[i + 1], t);
        }
    }
}