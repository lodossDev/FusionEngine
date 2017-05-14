using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FusionEngine {
    public interface IGameScreen : IDisposable {
        void LoadContent();
        void Update(GameTime gameTime);
        void Render(GameTime gameTime);
    }
}
