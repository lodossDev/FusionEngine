using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FusionEngine {

    public class AfterImage {
        private LinkedList<Entity> imageData;
        private Dictionary<int, float> countDown;

        private int frameLength;
        private int timeGap;
        private int frameGap;
        private int timeGapCount;
      
        private float time;
        private bool active;
        private bool isFirstCreated;
        private Entity entity;


        public AfterImage(Entity entity, int length = 20, int timeGap = 1, int frameGap = 4, float time = 1) {
            this.entity = entity;
            imageData = new LinkedList<Entity>();
            countDown = new Dictionary<int, float>();
            active = false;

            Reset();

            this.frameLength = length;
            this.timeGap = timeGap;
            this.frameGap = frameGap;
            this.time = time;
        }

        public void Reset() {
            countDown.Clear();
            timeGapCount = 0;
            isFirstCreated = false;
        }

        public void SetTime(float t) {
            time = t;
            countDown.Clear();
        }

        public float GetTime() {
            return time;
        }

        public bool Active() {
            return active;
        }

        public void SetActive(bool status) {
            active = status;
        }

        public void Update(GameTime gameTime) {
            if (active == false) return;

            ++timeGapCount;

            if (timeGapCount >= timeGap) {
                isFirstCreated = true;
                timeGapCount = 0;

                if (entity.IsEntity(Entity.ObjectType.PLAYER)) {
                    Entity afterImage = SpriteClone.CreateAfterImage(entity);
                    //afterImage.SetColor(255, 255, 255);
                    //afterImage.SetFade(100);

                    afterImage.SetAliveTime(time);
                    imageData.AddLast(afterImage);

                    while (imageData.Count > frameLength) imageData.RemoveFirst();
                }

                int index = 0; 
                Boolean isactive = false;

                foreach (Entity entity in imageData) {
                    if (TimeCheck_Update(index) == true) isactive = true;
                    ++index;
                }

                if (isactive == false && isFirstCreated == true) {
                    Reset();
                }
            }
        }

        public void Draw() {
            if (active == false) return;

            int index = imageData.Count;

            foreach(Entity entity in imageData) {
                --index;

                if (FrameGapCheck(index) == false) continue;
                Globals._renderManager.AddEntity(entity);
            }
        }

        private bool TimeCheck_Update(int index) {
			if (time == -1) return true;

			if (countDown.ContainsKey(index) == true) {
				if (countDown[index] <= 0) return false;
				--countDown[index];
			} else {
				if (time <= 0) return false;
				countDown.Add(index, time);
			}

			return true;
		}

        private bool FrameGapCheck(int index) {
			if (index <= 0) return false;

			return (index % frameGap == 0);
		}
    }
}
