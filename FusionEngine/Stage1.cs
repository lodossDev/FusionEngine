using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FusionEngine
{
    public class Stage1 : Level{
        public Stage1() :base("Stage 1") {

        }

        public override void Load() {
            Entity layer1 = new Entity(Entity.ObjectType.LEVEL, "LAYER 1");
            layer1.AddSprite(Animation.State.NONE, new Sprite(Setup.contentManager.Load<Texture2D>("Sprites/Levels/Stage1/normal01")), true);

            layer1.SetPostion(4000, 0, -130);
            layer1.SetScale(3.1f, 3.4f);
            AddLayer(1, layer1);
            SetMainLayer(layer1);

            Entity layer3 = new Entity(Entity.ObjectType.LEVEL, "LAYER 3");
            layer3.AddSprite(Animation.State.NONE, new Sprite(Setup.contentManager.Load<Texture2D>("Sprites/Levels/Stage1/front")), true);

            layer3.SetPostion(4000, 0, -130);
            layer3.SetScale(3.1f, 3.4f);
            AddLayer(3, layer3);

            Obstacle phoneBooth = new PhoneBooth1();
            phoneBooth.SetPostion(200, 0, 100);
            AddMisc(phoneBooth);

            SetXScrollBoundry(4000, -2744);
        }
    }
}
