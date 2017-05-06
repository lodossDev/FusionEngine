using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FusionEngine {

    public class UpdateManager : Manager {

        public UpdateManager() : base() {

        }

        public void BeforeUpdate(GameTime gameTime) {
            for (int i = 0; i < entities.Count; i++) {
                Entity entity = entities[i];

                entity.UpdateToss(gameTime);
                entity.UpdatePauseHit(gameTime);
                entity.UpdateAliveTime(gameTime);
                entity.UpdatePainTime(gameTime);
                entity.UpdateRiseTime(gameTime);

                entity.GetAfterImageData().Update(gameTime);
                entity.UpdateFade(gameTime);

                //Update animation.
                entity.UpdateAnimation(gameTime);           
                entity.UpdateFrameActions(gameTime);
                entity.UpdateDefaultAttackChain(gameTime);
                entity.UpdateRumble(gameTime);
                entity.UpdateBoxes(gameTime);
            }
        }

        public void AfterUpdate(GameTime gameTime) {
            for (int i = 0; i < entities.Count; i++) {
                Entity entity = entities[i];

                if (entity is Character || entity is Obstacle) { 
                    SoundAction soundAction = entity.GetSoundAction(entity.GetCurrentAnimationState());

                    EntityActions.OnAttacking(entity, soundAction);
                    EntityActions.OnRun(entity);
                    EntityActions.OnDeath(entity);
                }

                if (entity.IsExpired()) {
                    GameManager.GetInstance().RemoveEntity(entity);
                }
            }
        }
    }
}
