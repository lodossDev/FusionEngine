using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace FusionEngine {

    public abstract class AiBehaviour : AiState.IState {
        protected AiState.StateMachine stateMachine;
        protected Entity entity;
        protected Vector2 velocity;
        protected Vector2 direction;
        protected float distanceX, distanceZ;
        protected Random rnd;
        protected int activityTime;


        public AiBehaviour(Entity entity) {
            this.entity = entity;
            stateMachine = this.entity.GetAiStateMachine();
            distanceX = distanceZ = 0;
            activityTime = 0;

            velocity = new Vector2(2.5f, 2.0f);
            direction = new Vector2(1, -1);
            rnd = new Random();
        }

        public virtual void OnEnter() {
            activityTime = 0;
        }

        public virtual void OnExit() {

        }

        public virtual void Update(GameTime gameTime) {
            Entity target = entity.GetCurrentTarget();

            if (target != null){
                distanceX = Vector2.Distance(entity.GetProxyX(), target.GetProxyX());
                distanceZ = Vector2.Distance(entity.GetProxyZ(), target.GetProxyZ());

                stateMachine.SetLastDirection(direction.X, direction.Y);
            }
        }
    }
}
