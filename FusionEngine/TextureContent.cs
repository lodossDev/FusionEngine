using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Text.RegularExpressions;
using System.Drawing.Imaging;
using System.Drawing;
using System.Runtime.InteropServices;

namespace FusionEngine {

    public static class TextureContent {

        private static IEnumerable<string> CustomSort(this IEnumerable<string> list) {
            int maxLen = list.Select(s => s.Length).Max();

            return list.Select(s => new {
                OrgStr = s,
                SortStr = Regex.Replace(s, @"(\d+)|(\D+)", m => m.Value.PadLeft(maxLen, char.IsDigit(m.Value[0]) ? ' ' : '\xffff'))
            })
            .OrderBy(x => x.SortStr)
            .Select(x => x.OrgStr);
        }

        public static List<Texture2D> LoadTextures(string contentFolder) {
            List<Texture2D> result = new List<Texture2D>();

            foreach (string file in Directory.EnumerateFiles(Globals.contentManager.RootDirectory + "/" + contentFolder).CustomSort().ToList()) {
                string key = Path.GetFileNameWithoutExtension(file);
                result.Add(Globals.contentManager.Load<Texture2D>(contentFolder + "/" + key));
            }

            return result;
        }

        public static Texture2D TakeScreenshot(Game currentGame) {
            int w, h;
            w = Globals.graphicsDevice.PresentationParameters.BackBufferWidth;
            h = Globals.graphicsDevice.PresentationParameters.BackBufferHeight;
            RenderTarget2D screenshot;
            screenshot = new RenderTarget2D(Globals.graphicsDevice, w, h, false, SurfaceFormat.Bgra32, DepthFormat.None);
            Globals.graphicsDevice.SetRenderTarget(screenshot);

            currentGame.Render(new GameTime());

            Globals.graphicsDevice.Present();
            Globals.graphicsDevice.SetRenderTarget(null);
            return screenshot;
        }

        public static void Save(this Texture2D texture, ImageFormat imageFormat, Stream stream) {
            var width = texture.Width;
            var height = texture.Height;

            using (Bitmap bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb)) {
                IntPtr safePtr;
                BitmapData bitmapData;
                System.Drawing.Rectangle rect = new System.Drawing.Rectangle(0, 0, width, height);
                
                int[] textureData = new int[width * height];

                texture.GetData(textureData);
                bitmapData = bitmap.LockBits(rect, ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
                safePtr = bitmapData.Scan0;
                Marshal.Copy(textureData, 0, safePtr, textureData.Length);
                bitmap.UnlockBits(bitmapData);
                bitmap.Save(stream, imageFormat);

                textureData = null;
            }

            GC.Collect();
        }
    }
}
