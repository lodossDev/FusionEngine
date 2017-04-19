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

        private void CheckActions(GameTime gameTime) {
            for (int i = 0; i < entities.Count; i++) {
                Entity entity = entities[i];
                SoundAction soundAction = entity.GetSoundAction(entity.GetCurrentAnimationState());

                EntityActions.OnAttacking(entity, soundAction);
                EntityActions.OnRun(entity);
                EntityActions.OnDeath(entity);

                if (entity.IsExpired()) {
                    GameManager.GetInstance().RemoveEntity(entity);
                }
            }
        }

        public void Update(GameTime gameTime) {
            CheckActions(gameTime);
        }
    }
}
