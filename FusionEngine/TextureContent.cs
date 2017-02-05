using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace FusionEngine {

    public static class TextureContent {

        public static IEnumerable<string> CustomSort(this IEnumerable<string> list) {
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

            foreach (string file in Directory.EnumerateFiles(Setup.contentManager.RootDirectory + "/" + contentFolder).CustomSort().ToList()) {
                Debug.WriteLine(file);
                string key = Path.GetFileNameWithoutExtension(file);
                result.Add(Setup.contentManager.Load<Texture2D>(contentFolder + "/" + key));
            }

            return result;
        }
    }
}
