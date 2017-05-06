using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FusionEngine {

    public abstract class Level {
        private String name;
        private List<Entity> layers;
        private int xmin, xmax, zmin, zmax;
        private List<Enemy> enemies;
        private List<Boss> bosses;
        private List<Obstacle> obstacles;
        private List<Collectable> collectables;
        private List<Wall> walls;


        public Level(String name) {
            layers = new List<Entity>();
            enemies = new List<Enemy>();
            bosses = new List<Boss>();
            obstacles = new List<Obstacle>();
            collectables = new List<Collectable>();
            walls = new List<Wall>();

            this.name = name;
            xmin = xmax = zmin = zmax = 0;

            Load();
        }

        public abstract void Load();

        public void SetName(string name) {
            this.name = name;
        }

        public void SetBoundries(int xmin, int xmax, int zmin, int zmax) {
            this.xmin = xmin;
            this.xmax = xmax;
            this.zmin = zmin;
            this.zmax = zmax;
        }

        public void AddLayer(Entity layer) {
            layers.Add(layer);
        }

        public void AddEnemy(Enemy enemy) {
            enemies.Add(enemy);
        }

        public void AddBoss(Boss boss) {
            bosses.Add(boss);
        }

        public void AddObstacle(Obstacle obstacle) {
            obstacles.Add(obstacle);
        }

        public void AddCollectable(Collectable collectable) {
            collectables.Add(collectable);
        }

        public void AddWall(Wall wall) {
            walls.Add(wall);
        }

        public String GetName() {
            return name;
        }

        public List<Entity> Layers {
            get { return layers; }
        }

        public List<Enemy> Enemies {
            get{ return enemies; }
        }

        public List<Boss> Bosses {
            get{ return bosses; }
        }

        public List<Obstacle> Obstacles {
            get { return obstacles; }
        }

        public List<Collectable> Collectables {
            get { return collectables; }
        }

        public List<Wall> Walls {
            get { return walls; }
        }

        public int X_MIN {
            get { return xmin; }
        }

        public int X_MAX {
            get { return xmax; }
        }

        public int Z_MIN {
            get { return zmin; }
        }

        public int Z_MAX {
            get { return zmax; }
        }
    }
}
