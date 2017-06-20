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

                if (entity is Player) {
                    ((Player)entity).UpdateHitLifebarTimer(gameTime);
                }

                entity.UpdateHitPause(gameTime);
                
                if (entity.InHitPauseTime() == false) {
                    entity.UpdateAliveTime(gameTime);

                    entity.UpdateLifebar(gameTime);
                    entity.UpdateToss(gameTime);
                
                    entity.UpdatePainTime(gameTime);
                    entity.UpdateRiseTime(gameTime);
                    entity.UpdateFade(gameTime);

                    //Update animation.
                    entity.UpdateAnimation(gameTime);           
                    entity.UpdateFrameActions(gameTime);
                    entity.UpdateDefaultAttackChain(gameTime);
                    entity.UpdateNextAttackTime(gameTime);
                }

                entity.GetAfterImageData().Update(gameTime);
                entity.UpdateRumble(gameTime);
                entity.UpdateBoxes(gameTime);
                entity.UpdateIsMoveInFrameComplete();
            }
        }

        public void AfterUpdate(GameTime gameTime) {
            for (int i = 0; i < entities.Count; i++) {
                Entity entity = entities[i];
                entity.UpdateHealth(gameTime);
                entity.Actions(gameTime);
                entity.Update(gameTime);

                if (entity is Character || entity is Obstacle || entity is Projectile) { 
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
