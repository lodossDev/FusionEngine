using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FusionEngine {

    public static class SpriteClone {

        public static Entity CreateAfterImage(Entity target) {
            Sprite sprite = target.GetSprite(target.GetCurrentAnimationState());

            Entity afterImage = new Entity(Entity.ObjectType.AFTER_IMAGE, target.GetName());
            afterImage.AddSprite(target.GetCurrentAnimationState(), sprite.Clone(target.GetCurrentSpriteFrame() + 1), true);
            afterImage.SetPostion(target.GetPosX(), target.GetPosY(), target.GetPosZ() - 5);

            afterImage.SetSpriteOffSet(target.GetCurrentAnimationState(), sprite.GetSpriteOffSet().X, sprite.GetSpriteOffSet().Y);
            afterImage.SetFrameOffset(target.GetCurrentAnimationState(), 1, sprite.GetCurrentFrameOffSet().X, sprite.GetCurrentFrameOffSet().Y);
            afterImage.SetFrameScale(target.GetCurrentAnimationState(), 1, sprite.GetCurrentScaleFrame().X, sprite.GetCurrentScaleFrame().Y);

            afterImage.SetOnLoadScale(target.GetScaleX(), target.GetScaleY());
            afterImage.SetIsLeft(target.IsLeft());
            afterImage.SetLayerPos(target.GetDepthBox().GetRect().Bottom - 5);
            
            return afterImage;
        }
    }
}
