using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FusionEngine
{
    public class ComboAttack
    {
        public class Chain
        {
            private List<Move> moves;
            private int currentMoveIndex;
            public float currentMoveTime;

            public Chain(List<Move> moves)
            {
                this.moves = moves;
                currentMoveIndex = 0;
                currentMoveTime = 0f;
            }

            public List<Move> GetMoves()
            {
                return moves;
            }

            public int GetCurrentMoveIndex()
            {
                return currentMoveIndex;
            }

            public bool InLastAttackState()
            {
                return currentMoveIndex == moves.Count - 1;
            }

            public Animation.State GetCurrentAttackState()
            {
                return moves[currentMoveIndex].GetState();
            }

            public Animation.State GetPreviousAttackState()
            {
                int step = currentMoveIndex - 1;
                if (step < 0) step = 0;

                return moves[step].GetState();
            }

            public Animation.State GetLastAttackState()
            {
                return moves[moves.Count - 1].GetState();
            }

            public void IncrementMoveIndex(int step)
            {
                currentMoveIndex += step;
                currentMoveTime = 0f;

                if (currentMoveIndex > moves.Count - 1)
                {
                    ResetMove();
                }
            }

            public void IncrementMoveIndex()
            {
                IncrementMoveIndex(1);
            }

            public void ResetMove()
            {
                currentMoveIndex = 0;
                currentMoveTime = 0f;
            }

            public Move GetCurrentMove()
            {
                return moves[currentMoveIndex];
            }

            public void UpdateCombo(GameTime gameTime)
            {
                if (moves.Count > 0 && currentMoveIndex > 0)
                {
                    currentMoveTime += (float)gameTime.ElapsedGameTime.TotalMilliseconds;

                    if (currentMoveIndex > moves.Count - 1)
                    {
                        ResetMove();
                    }

                    if (currentMoveTime > moves[currentMoveIndex].GetExpireTime())
                    {
                        ResetMove();
                    }
                }
            }
        }

        public class Move
        {
            private Animation.State state;
            private float expireTime;
            private int cancelFrame;

            public Move(Animation.State state, float expireTime, int cancelFrame)
            {
                this.state = state;
                this.expireTime = expireTime;
                this.cancelFrame = cancelFrame - 1;
            }

            public Animation.State GetState()
            {
                return state;
            }

            public float GetExpireTime()
            {
                return expireTime;
            }

            public int GetCancelFrame()
            {
                return cancelFrame;
            }
        }
    }
}
