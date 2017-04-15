using Microsoft.Xna.Framework.Audio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FusionEngine {

    public class SoundAction {
        private int step;
        private Animation.State? state;
        private SoundEffectInstance soundInstance;


        public SoundAction(Animation.State? state) {
            step = 0;
            this.state = state;
        }

        public void Reset() {
            if (soundInstance != null) {
                soundInstance.Stop();
            }

            step = 0;
        }

        public SoundEffectInstance SetSoundInstance(SoundEffectInstance soundInstance) {
            this.soundInstance = soundInstance;
            step = 1;
            return this.soundInstance;
        }

        public void SetStep(int step) {
            this.step = step;
        }

        public bool IsActive() {
            return (step == 1);
        }

        public int GetStep() {
            return step;
        }

        public void StopSoundInstance() {
            if (this.soundInstance != null) { 
                this.soundInstance.Stop();
            }
        }

        public Animation.State? GetState() {
            return state;
        }
    }
}
