﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FusionEngine
{
    public class Stage1 : Level{
        public Stage1() :base("STAGE 1") {

        }

        public override void Load() {
            Entity layer1 = new Entity(Entity.ObjectType.LEVEL, "LAYER 1");
            layer1.AddSprite(Animation.State.NONE, new Sprite(GameManager.ContentManager.Load<Texture2D>("Sprites/Levels/Stage1/normal01")), true);

            layer1.SetPostion(5850, 0, -280);
            layer1.SetScale(4.5f, 4.5f);
            AddLayer(layer1);

            Entity layer3 = new Entity(Entity.ObjectType.LEVEL, "LAYER 3");
            layer3.AddSprite(Animation.State.NONE, new Sprite(GameManager.ContentManager.Load<Texture2D>("Sprites/Levels/Stage1/front")), true);

            layer3.SetPostion(5850, 0, -280);
            layer3.SetScale(4.5f, 4.5f);
            layer3.SetLayerPos(5000);
            AddLayer(layer3);

            Obstacle phoneBooth = new PhoneBooth();
            phoneBooth.SetPostion(400, 0, -30);
            AddObstacle(phoneBooth);

            phoneBooth = new PhoneBooth();
            phoneBooth.SetPostion(3100, 0, 340);
            AddObstacle(phoneBooth);

            Obstacle drum = new Drum();
            drum.SetPostion(900, 0, 300);
            AddObstacle(drum);

            Meat meat = new Meat();
            AddCollectable(meat);

            Wall wall1 = new Wall("TEST", 200, 340, 10750, -5, 0, 50, 100);
            AddWall(wall1);

            basePosition = new Vector2(5850, -280);
            SetBoundries(0, 13000, 0, 500);
        }
    }
}
