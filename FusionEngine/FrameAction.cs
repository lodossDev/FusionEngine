using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FusionEngine {

    public class FrameAction {
        private float moveX;
        private float moveY;
        private float tossHeight;
        private Attributes.FrameInfo frameInfo;
        private Animation.State animationState;

        public FrameAction(Animation.State animationState, int startFrame, int endFrame, float moveX, float moveY, float tossHeight) {
            this.animationState = animationState;
            frameInfo = new Attributes.FrameInfo(startFrame - 1, endFrame - 1);
            this.moveX = moveX;
            this.moveY = moveY;
            this.tossHeight = tossHeight;
        }

        public Animation.State GetAnimationState() {
            return animationState;
        }

        public float GetMoveX() {
            return moveX;
        }

        public float GetMoveY() {
            return moveY;
        }

        public float GetTossHeight() {
            return tossHeight;
        }

        public int GetStartFrame() {
            return frameInfo.GetStartFrame();
        }

        public int GetEndFrame() {
            return frameInfo.GetEndFrame();
        }

        public bool IsInFrame(int currentFrame) {
            return frameInfo.IsInFrame(currentFrame);
        }

        public void SetMoveX(float x) {
            moveX = x;
        }

        public void SetMoveY(float y) {
            moveY = y;
        }

        public void SetTossHeight(float height) {
            tossHeight = height;
        }

        public void SetAnimationState(Animation.State state) {
            animationState = state;
        }

        public void SetFrame(int startFrame, int endFrame) {
            frameInfo.SetStartFrame(startFrame);
            frameInfo.SetEndFrame(endFrame);
        }
    }
}
