using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FusionEngine {

    public class Player : Character {
        private int playerIndex;


        public Player(String name) : base(ObjectType.PLAYER, name) {
        }

        public void SetPlayerIndex(int index) {
            playerIndex = index;
        }

        public int GetPlayerIndex() {
            return playerIndex;
        }
    }
}
