using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FusionEngine {

    public static class SoundContent {

        public static Dictionary<string, SoundEffect> LoadSounds(string contentFolder) {
            Dictionary<string, SoundEffect> result = new Dictionary<string, SoundEffect>();

            foreach (string file in Directory.EnumerateFiles(GameManager.ContentManager.RootDirectory + "/" + contentFolder)) {
                string key = Path.GetFileNameWithoutExtension(file);
                result.Add(key, GameManager.ContentManager.Load<SoundEffect>(contentFolder + "/" + key));
            }

            return result;
        }
    }
}
