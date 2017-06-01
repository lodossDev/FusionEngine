using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Input;
using static FusionEngine.InputHelper;

namespace FusionEngine {

    public class Entity : IComparable<Entity> {
        private static int id = 0;

        public enum ObjectType {
            PLAYER, ENEMY, OBSTACLE, PLATFORM, ITEM, WEAPON, LEVEL,
            LIFE_BAR, OTHER, HIT_FLASH, AFTER_IMAGE, COLLECTABLE, LAYER,
            WALL, PORTRAIT, SYSTEM,
            PROJECTILE
        }

        [Flags]
        public enum DeathType {DEFAULT = 1, FLASH = 2, IMMEDIATE_DIE = 4}

        private Dictionary<Animation.State?, Sprite> spriteMap;
        private Sprite currentSprite;
        private Attributes.ColourInfo colorInfo;
        
        private Animation.State? currentAnimationState;
        private Animation.State? lastAnimationState;
        private Dictionary<Animation.State?, SoundEffect> animationSounds;
        private Dictionary<Animation.State?, SoundAction> soundActionMap;

        private Dictionary<Animation.State?, int> moveFrames;
        private Dictionary<Animation.State?, int> tossFrames;

        private Dictionary<String, Effect> specialArts;
        private Dictionary<Effect.State, Effect> hitSparks;
        private Dictionary<Effect.State, Effect> blockSparks;

        private CLNS.BoundingBox bodyBox;
        private CLNS.BoundingBox depthBox;
        private CLNS.BoundsBox boundsBox;
        
        private AfterImage afterImage;
        private List<Animation.Link> animationLinks;
        private ComboAttack.Chain defaultAttackChain;
        private List<InputHelper.CommandMove> commandMoves;

        private string name;
        private ObjectType type;

        private Vector3 position;
        private Vector3 offset;
        private Vector2 convertedPosition;
        private int layerPos;

        private Vector3 acceleration;
        private Vector3 direction;
        private Vector3 velocity;
        private Vector3 maxVelocity;
        private Vector3 absoluteVel;

        private Vector2 origin;
        private Vector2 scale;
        private Vector2 currentScale;
       
        private Vector2 tempScale;
        private Vector2 stanceOrigin;

        private float ground;
        private float groundBase;

        private Sprite baseSprite;
        private Vector2 baseScale;
        private Vector2 baseOffset;
        private Vector2 basePosition;

        private Attributes.CollisionInfo collisionInfo;
        private Attributes.AttackInfo attackInfo;
        private Attributes.TossInfo tossInfo;

        private AiState.StateMachine aiStateMachine;
        private Entity currentTarget;

        private int entityId;
        private int mp;
        private int currentMpLevel;
        private int maxMpLevel;
        private int maxLives;
        private int lives;
        private long oldPoints;
        private long points;
        private int oldHealth;
        private int health;
        private int maxHealth;
        private bool alive;
        private Attributes.Portrait portrait;
        private MugenFont comboFont;
        private MugenFont nameFont;
        private SFIII_SimpleLifebar lifeBar;

        private Dictionary<InputHelper.KeyPress, Keys> keyboardSettings;
        private Dictionary<InputHelper.KeyPress, Keys> keyboardBtnsOnly;

        private Dictionary<InputHelper.KeyPress, Buttons> gamepadSettings;
        private Dictionary<InputHelper.KeyPress, Buttons> gamepadBtnsOnly;

        private Attributes.GrabInfo grabInfo;
        private Entity link;
        private float aliveTime;
        private BlendState blendState;
        private Color baseColor;
        private Attributes.Rumble rumble;
        private int painTime;
        private Attributes.AnimationConfig animationConfig;
        private List<FrameAction> frameActions;
        private Attributes.FrameInfo grabItemFrame;
        private Animation.State? grabItemState;

        private bool isRise;
        private int riseTime;
        private int maxRiseTime;
        private DeathType deathMode;
        private int deathStep;
        private int dieTime;

        private bool isPlatform;
        private Vector2 scrollMin;
        private Vector2 scrollMax;
        private Vector2 scrollOffset;
        private bool boundToLevel;
        private bool isEdgeX, isEdgeZ;
        private bool drawShadow;

        private Dictionary<string, bool> actionStates;
        private Entity owner;


        public Entity(ObjectType type, string name) {
            this.type = type;
            this.name = name;

            spriteMap = new Dictionary<Animation.State?, Sprite>();
            moveFrames = new Dictionary<Animation.State?, int>();
            tossFrames = new Dictionary<Animation.State?, int>();

            specialArts = new Dictionary<string, Effect>();
            hitSparks = new Dictionary<Effect.State, Effect>();
            blockSparks = new Dictionary<Effect.State, Effect>();

            animationLinks = new List<Animation.Link>();
            animationSounds = new Dictionary<Animation.State?, SoundEffect>();
            soundActionMap = new Dictionary<Animation.State?, SoundAction>();

            actionStates = new Dictionary<string, bool>();

            scale = new Vector2(1f, 1f);
            tempScale = new Vector2(1f, 1f); 
            baseScale = new Vector2(1f, 1f);
            currentScale = new Vector2(1f, 1f);
            stanceOrigin = Vector2.Zero;

            currentAnimationState = Animation.State.NONE;
            colorInfo = new Attributes.ColourInfo();

            position = Vector3.Zero;
            convertedPosition = Vector2.Zero;
            offset = Vector3.Zero;

            acceleration = Vector3.Zero;
            direction = Vector3.Zero;
            velocity = Vector3.Zero;
            absoluteVel = Vector3.Zero;
            maxVelocity = new Vector3(15, 5, 5);
            layerPos = 0;

            origin = Vector2.Zero;
            ground = groundBase = 0;

            baseSprite = new Sprite("Sprites/Misc/Marker");
            baseOffset = Vector2.Zero;
            basePosition = Vector2.Zero;

            collisionInfo = new Attributes.CollisionInfo();
            attackInfo = new Attributes.AttackInfo();
            tossInfo = new Attributes.TossInfo();
            grabInfo = new Attributes.GrabInfo();

            aiStateMachine = new AiState.StateMachine();
            commandMoves = new List<InputHelper.CommandMove>();

            keyboardSettings = new Dictionary<InputHelper.KeyPress, Keys>();
            gamepadSettings = new Dictionary<InputHelper.KeyPress, Buttons>();

            isPlatform = false;
            scrollMin = Vector2.Zero;
            scrollMax = Vector2.Zero;
            scrollOffset = Vector2.Zero;
            boundToLevel = false;
            isEdgeX = isEdgeZ = false;

            blendState = BlendState.NonPremultiplied;
            aliveTime = -1;
            afterImage = new AfterImage(this);

            frameActions = new List<FrameAction>();
            animationConfig = new Attributes.AnimationConfig();
            rumble = new Attributes.Rumble();
            painTime = -1;

            isRise = false;
            riseTime = maxRiseTime = 50;
            dieTime = 50;
            deathStep = -1;
            
            comboFont = null;
            nameFont = new MugenFont("Fonts/sfiii_name.xFont", 4, 0, 2.8f);
            portrait = null;
            lifeBar = null;

            health = oldHealth = maxHealth = 100;
            points = oldPoints = 0;
            currentMpLevel = 0;
            maxMpLevel = 3;
            mp = 0;
            lives = maxLives = 3;

            deathMode = DeathType.IMMEDIATE_DIE;
            alive = true;
            drawShadow = false;

            owner = null;

            id++;
            entityId = id;
        }

        public void AddSprite(Animation.State? state, Sprite sprite) {
            spriteMap.Add(state, sprite);
            soundActionMap.Add(state, new SoundAction(state));
        }

        public void AddSprite(Animation.State? state, Sprite sprite, bool setAsDefaultState) {
            AddSprite(state, sprite);

            if (setAsDefaultState) {
                SetAnimationState(state);
            }
        }

        public void AddSprite(Animation.State? state, String location, bool setAsDefaultState) {
            AddSprite(state, new Sprite(location), setAsDefaultState);
        }

        public void AddAnimationLink(Animation.Link link) {
            animationLinks.Add(link);
        }

        public void AddSpecialArt(Effect art) {
            specialArts.Add(art.GetName(), art);
        }

        public void AddHitSpark(Effect hitSpark) {
            hitSparks.Add(hitSpark.GetState(), hitSpark);
        }

        public void AddBlockSpark(Effect blockSpark) {
            blockSparks.Add(blockSpark.GetState(), blockSpark);
        }

        public void AddActionState(string action, bool state = false) {
            actionStates.Add(action, state);
        }

        public void DisbaleActionState(string action) {
            actionStates[action] = false;
        }

        public void EnableActionState(string action) {
            actionStates[action] = true;
        }

        public bool InActionState(string action) {
            if (actionStates.ContainsKey(action)) {
                return actionStates[action];
            }

            return false;
        }

        public void SetAnimationLink(Animation.State? onState, Animation.State? toState, int frameOnStart, bool onFrameComplete = true) {
            Animation.Link link = animationLinks.Find(item => item.GetOnState() == onState);

            if (link != null) { 
                link.SetLink(onState, toState, frameOnStart, onFrameComplete);
            }
        }

        public void SetJumpLink(Animation.State? toState) {
            if (HasSprite(Animation.State.JUMP_START)) {
                Sprite jumpStart = GetSprite(Animation.State.JUMP_START);

                int frames = jumpStart.GetFrames();
                SetAnimationLink(Animation.State.JUMP_START, toState, frames);
            }
        }

        public void SetAnimationState(Animation.State? state) {
            if (!IsInAnimationState(state)) {
                SoundAction soundAction = GetSoundAction(state);

                if (soundAction != null) {
                    soundAction.SetStep(-1);
                }

                attackInfo.lastAttackFrame = -1;
                attackInfo.lastAttackState = Animation.State.NONE;
                Sprite newSprite = GetSprite(state);

                if (newSprite != null) {
                    lastAnimationState = currentAnimationState;
                    currentAnimationState = state;

                    newSprite.ResetAnimation();
                    currentSprite = newSprite;
                }
            }
        }

        public void AddBox(Animation.State? state, int frame, CLNS.BoundingBox box) {
            if (state != null) { 
                GetSprite(state).AddBox(frame, box);
            }
        }

        public void AddBodyBox(int w, int h, int x, int y) {
            if (bodyBox != null) {
                bodyBox = null;
            }

            bodyBox = new CLNS.BoundingBox(CLNS.BoxType.BODY_BOX, w, h, x, y);
        }

        public void AddBoundsBox(int w, int h, int x, int y, int depth) {
            if (boundsBox != null) {
                boundsBox = null;
            }

            boundsBox = new CLNS.BoundsBox(w, h, x, y, depth);

            AddBodyBox(w, h, x, y);
            AddDepthBox(depth);
        }

        public void AddDepthBox(int h, int x = 0, int y = 0) {
            if (depthBox != null) {
                depthBox = null;
            }

            if (boundsBox != null) {
                int x1 = (int)boundsBox.GetOffset().X + x;
                int y1 = (int)(boundsBox.GetOffset().Y + boundsBox.GetRect().Height + y) - h;
                
                depthBox = new CLNS.BoundingBox(CLNS.BoxType.DEPTH_BOX, boundsBox.GetRect().Width, h, x1, y1);
                depthBox.SetZdepth(h);
            }
        }

        public void AddDepthBox(int w, int h, int x, int y) {
            if (depthBox != null) {
                depthBox = null;
            }

            depthBox = new CLNS.BoundingBox(CLNS.BoxType.DEPTH_BOX, w, h, x, y);
            depthBox.SetZdepth(h);
        }

        public void AddFrameAction(Animation.State state, int startFrame, int endFrame, float moveX = 0, float moveY = 0, float tossHeight = 0) {
            FrameAction action = new FrameAction(state, startFrame, endFrame, moveX, moveY, tossHeight);
            frameActions.Add(action);
        }

        public void SetGrabItemFrameInfo(Animation.State state, int startFrame, int endFrame) {
            if (grabItemFrame != null) {
                grabItemFrame = null;
            }

            grabItemFrame = new Attributes.FrameInfo(startFrame, endFrame);
            grabItemState = state;
        }

        public void AddAnimationSound(Animation.State state, String location) {
            animationSounds.Add(state, GameManager.ContentManager.Load<SoundEffect>(location));
        }

        public void AddAnimationSound(Animation.State state, SoundEffect effect) {
            animationSounds.Add(state, effect);
        }

        public SoundEffect GetAnimationSound(Animation.State? state) {
            if (state != null && animationSounds.ContainsKey(state)) { 
                return animationSounds[state];
            }

            return null;
        }

        public SoundAction GetSoundAction(Animation.State? state) {
            if (state != null && soundActionMap.ContainsKey(state)) { 
                return soundActionMap[state];
            }

            return null;
        }

        public void SetLifeBar(SFIII_SimpleLifebar lifeBar) {
            this.lifeBar = lifeBar;
        }

        public void SetNameFont(MugenFont nameFont) {
            this.nameFont = nameFont;
        }

        public MugenFont GetNameFont() {
            return nameFont;
        }

        public virtual Projectile GetProjectille() {
            return null;
        }

        public void AddCommandMove(InputHelper.CommandMove commandMove) {
            commandMoves.Add(commandMove);
        }

        public void SetKeyboard(InputHelper.KeyPress press, Keys key) {
            keyboardSettings.Add(press, key);
            keyboardBtnsOnly = keyboardSettings.Where(i => i.Key == KeyPress.ATTACK1 || i.Key == KeyPress.ATTACK2 || i.Key == KeyPress.ATTACK3 
                                                        || i.Key == KeyPress.ATTACK4 || i.Key == KeyPress.SPECIAL || i.Key == KeyPress.JUMP
                                                        || i.Key == KeyPress.START || i.Key == KeyPress.PAUSE).ToDictionary(i => i.Key, i => i.Value);
        }

        public void SetGamepad(InputHelper.KeyPress press, Buttons key) {
            gamepadSettings.Add(press, key);
            gamepadBtnsOnly = gamepadSettings.Where(i => i.Key == KeyPress.ATTACK1 || i.Key == KeyPress.ATTACK2 || i.Key == KeyPress.ATTACK3 
                                                        || i.Key == KeyPress.ATTACK4 || i.Key == KeyPress.SPECIAL || i.Key == KeyPress.JUMP
                                                        || i.Key == KeyPress.START || i.Key == KeyPress.PAUSE).ToDictionary(i => i.Key, i => i.Value);
        }

        public List<InputHelper.CommandMove> GetCommandMoves() {
            return commandMoves;
        }

        public void UpdateCommandMoves(GameTime gameTime) {
            foreach (InputHelper.CommandMove command in commandMoves) {
                command.Update(gameTime);
            }
        }

        public void SetFrameScale(Animation.State? state, int frame, float x, float y) {
            GetSprite(state).SetFrameScale(frame, x, y);
        }

        public void SetFrameScale(Animation.State? state, float x, float y) {
            GetSprite(state).SetFrameScale(x, y);
        }

        public void SetFrameOffset(Animation.State? state, int frame, float x, float y) {
            GetSprite(state).SetFrameOffset(frame, x, y);
        }

        public void SetOffset(Animation.State? state, float x, float y) {   
            GetSprite(state).SetFrameOffset(x, y);    
        }

        public void SetSpriteOffSet(Animation.State? state, float x, float y) {
            GetSprite(state).SetSpriteOffset(x, y);
        }

        public void SetFrameDelay(Animation.State? state, int frame, int ticks) {
            GetSprite(state).SetFrameTime(frame, ticks);
        }

        public void SetFrameDelay(Animation.State? state, int ticks) {
            GetSprite(state).SetFrameTime(ticks);
        }

        public void SetResetFrame(Animation.State? state, int frame) {
            GetSprite(state).SetResetFrame(frame);
        }

        public void SetMoveFrame(Animation.State? state, int frame) {
            moveFrames.Add(state, frame - 1);
        }

        public void SetTossFrame(Animation.State? state, int frame) {
            tossFrames.Add(state, frame - 1);
        }

        public void SetDefaultAttackChain(ComboAttack.Chain attackChain) {
            defaultAttackChain = attackChain;
        }

        public void SetAttackBox(Animation.State state, int frame, float zDepth = 30, float hitPauseTime = 0, float painTime = 40, int hitDamage = 5,
                                                            int hitPoints = 5, float hitStrength = 0.4f, int comboStep = 1, int juggleCost = 0, 
                                                            CLNS.AttackBox.AttackType attackType = CLNS.AttackBox.AttackType.LIGHT, CLNS.AttackBox.State attackPosiiton = CLNS.AttackBox.State.NONE, 
                                                            CLNS.AttackBox.State blockPosition = CLNS.AttackBox.State.NONE, CLNS.AttackBox.HitType hitType = CLNS.AttackBox.HitType.ALL, 
                                                            Effect.State sparkState = Effect.State.NONE, float sparkX = 0, float sparkY = 0, float moveX = 0, float tossHeight = 0, 
                                                            bool isKnock = false, bool applyToAttacker = false) {

            foreach (CLNS.AttackBox attackBox in GetAttackBoxes(state, frame)) { 

                attackBox.SetAttack(zDepth, hitPauseTime, painTime, hitDamage, hitPoints, hitStrength, comboStep, juggleCost, attackType, 
                                        attackPosiiton, blockPosition, hitType, sparkState, sparkX, sparkY, moveX, tossHeight, isKnock, applyToAttacker);
            }
        }

        public void SetAttackBox(Animation.State state, float zDepth = 30, float hitPauseTime = 1 / 60, float painTime = 20 / 60, int hitDamage = 5,
                                                            int hitPoints = 5, float hitStrength = 0.4f, int comboStep = 1, int juggleCost = 0, 
                                                            CLNS.AttackBox.AttackType attackType = CLNS.AttackBox.AttackType.LIGHT, CLNS.AttackBox.State attackPosiiton = CLNS.AttackBox.State.NONE, 
                                                            CLNS.AttackBox.State blockPosition = CLNS.AttackBox.State.NONE, CLNS.AttackBox.HitType hitType = CLNS.AttackBox.HitType.ALL, 
                                                            Effect.State sparkState = Effect.State.NONE, float sparkX = 0, float sparkY = 0, float moveX = 0, float tossHeight = 0, 
                                                            bool isKnock = false, bool applyToAttacker = false) {

            foreach (CLNS.AttackBox attackBox in GetAttackBox(state)) { 

                attackBox.SetAttack(zDepth, hitPauseTime, painTime, hitDamage, hitPoints, hitStrength, comboStep, juggleCost, attackType, 
                                        attackPosiiton, blockPosition, hitType, sparkState, sparkX, sparkY, moveX, tossHeight, isKnock, applyToAttacker);
            }
        }
        
        public void SetPostion(float x, float y, float z) {
            position.X = x;
            position.Y = y;
            position.Z = z;
        }

        public void SetPostion(float x, float y) {
            position.X = x;
            position.Y = y;
        }

        public void SetPosX(float x) {
            position.X = x;
        }

        public void SetPosY(float y) {
            position.Y = y;
        }

        public void SetPosZ(float z) {
            position.Z = z;
        }

        public void SetOffset(float x, float y, float z) {
            offset.X = x;
            offset.Y = y;
            offset.Z = z;
        }

        public void SetOffsetX(float x) {
            offset.X = x;
        }

        public void SetOffsetY(float y) {
            offset.Y = y;
        }

        public void SetOffsetZ(float z) {
            offset.Z = z;
        }

        public void SetCurrentTarget(Entity target) {
            currentTarget = target;
        }

        public void SetEntityLink(Entity link) {
            this.link = link;
        }

        public void SetDirectionX(float dir) {
            direction.X = dir;
        }

        public void SetDirectionY(float dir) {
            direction.Y = dir;
        }

        public void SetDirectionZ(float dir) {
            direction.Z = dir;
        }

        public void SetMaxVelocityX(float x) {
            maxVelocity.X = x;
        }

        public void SetMaxVelocityY(float y) {
            maxVelocity.Y = y;
        }

        public void SetMaxVelocityZ(float z) {
            maxVelocity.Z = z;
        }

        public void SetAbsoluteVelX(float x) {
            absoluteVel.X = x;
        }

        public void SetAbsoluteVelY(float y) {
            absoluteVel.Y = y;
        }

        public void SetAbsoluteVelZ(float z) {
            absoluteVel.Z = z;
        }

        public void SetShadowOffsetX(float x) {
            foreach (Sprite sprite in spriteMap.Values) {
                sprite.SetShadowOffsetX(x);
            }
        }

        public void SetShadowOffsetY(float y) {
            foreach (Sprite sprite in spriteMap.Values) {
                sprite.SetShadowOffsetY(y);
            }
        }

        public void SetShadowOffset(float x, float y) {
            foreach (Sprite sprite in spriteMap.Values) {
                sprite.SetShadowOffset(x, y);
            }
        }

        public void SetShadowOffsetX(Animation.State state, float x) {
            GetSprite(state).SetShadowOffsetX(x);
        }

        public void SetShadowOffsetY(Animation.State state, float y) {
            GetSprite(state).SetShadowOffsetY(y);
        }

        public void SetShadowOffset(Animation.State state, float x, float y) {
            GetSprite(state).SetShadowOffset(x, y);
        }

        public void SetLives(int amount) {
            lives = amount;
        }

        public void SetMaxLives(int amount) {
            maxLives = amount;
            lives = amount;
        }

        public void SetMP(int amount) {
            mp = amount;
        }

        public void SetPoints(int amount) {
            points = amount;
        }

        public void SetOldPoints(int amount) {
            oldPoints = amount;
        }

        public void SyncOldPoints(int amount) {
            if (oldPoints < points) {
                oldPoints += amount;
            }

            if (oldPoints > points) {
                oldPoints = points;
            }
        }

        public void SetDieTime(int time) {
            dieTime = time;
        }

        public int GetDieTime() {
            return dieTime;
        }

        public void SetGrabbable(bool status) {
            grabInfo.grabbable = status;
        }

        public bool IsGrabbable() {
            return grabInfo.grabbable;
        }

        public void MoveX(float acc, float dir) {
            this.acceleration.X = acc * GameManager.GAME_VELOCITY;
            this.maxVelocity.X = this.acceleration.X;
            this.direction.X = dir;
        }

        public void StopMovement() {
            ResetX();
            ResetZ();
        }

        public void ResetX() {
            this.acceleration.X = 0;
            this.maxVelocity.X = 0;
            this.velocity.X = 0;
        }

        public void MoveY(float acc, float dir) {
            this.acceleration.Y = acc;
            this.direction.Y = dir;
        }

        public void MoveZ(float acc, float dir) {
            this.acceleration.Z = acc * GameManager.GAME_VELOCITY;
            this.maxVelocity.Z = this.acceleration.Z;
            this.direction.Z = dir;
        }

        public void ResetZ() {
            this.acceleration.Z = 0;
            this.maxVelocity.Z = 0;
            this.velocity.Z = 0;
        }

        public void MoveX(float velX) {
            if ((double)velX != 0.0) {
                absoluteVel.X = velX;
            }

            if (IsInMoveFrame() && !HasCollidedX()) {
                position.X += velX;
            }
        }

        public void TransformX(float velX) {
            if ((double)velX != 0.0) {
                absoluteVel.X = velX;
            }

            position.X += velX;
        }

        public void MoveY(float velY) {
            if ((double)velY != 0.0) {
                absoluteVel.Y = velY;
            }

            position.Y += velY;
        }

        public void MoveZ(float velZ) {
            if ((double)velZ != 0.0) {
                absoluteVel.Z = velZ;
            }

            if (IsInMoveFrame() && !HasCollidedZ()) {
                position.Z += velZ;
            }
        }

        public void VelX(float velX) {
            velocity.X = velX;
        }

        public void VelY(float velY) {
            velocity.Y = velY;
        }

        public void VelZ(float velZ) {
            velocity.Z = velZ;
        }

        public void SetScale(float x=1f, float y=1f) {
            scale.X = x;
            scale.Y = y;
        }

        public void SetBaseScale(float x = 1f, float y = 1f) {
            baseScale.X = x;
            baseScale.Y = y;
        }

        public void SetOnLoadScale(float x = 1f, float y = 1f) {
            SetScale(x, y);
            SetBaseScale(x, y);
        }

        public void SetScaleX(float x) {
            scale.X = x;
        }

        public void SetScaleY(float y) {
            scale.Y = y;
        }

        public void SetBlockMode(int mode) {
            GetAttackInfo().blockMode = mode;
        }

        public int GetBlockMode() {
            return GetAttackInfo().blockMode;
        }

        public void SetCanBeKnockedFromKnockedEntity(int state) {
            GetAttackInfo().knockedFromKnockedEntityState = state;
        }

        public void SetHeightKnockedFromKnockedEntity(int height) {
            GetAttackInfo().knockedFromKnockedEntityHeight = height;
        }

        public int GetKnockedFromKnockedEntityState() {
            return GetAttackInfo().knockedFromKnockedEntityState;
        }

        public int GetKnockedFromKnockedEntityHeight() {
            return GetAttackInfo().knockedFromKnockedEntityHeight;
        }

        public bool CanKnockOtherEntity(){
            return (IsKnocked()
                        && GetTossInfo().hitGroundCount < 1 
                        && (GetTossInfo().velocity.Y / GameManager.GAME_VELOCITY) > GetKnockedFromKnockedEntityHeight()
                        && InAir());
        }

        public void SetCurrentKnockedState(Attributes.KnockedState state) {
            GetAttackInfo().currentKnockedState = state;
        }

        public Attributes.KnockedState GetCurrentKnockedState() {
            return GetAttackInfo().currentKnockedState;
        }

        public bool InAllowedKnockedState(Attributes.KnockedState state) {
            return GetAttackInfo().allowedKnockedState.HasFlag(state);
        }

        public void SetHealth(int health) {
            this.health = health;
        }

        public void SetOldHealth(int health) {
            this.oldHealth = health;
        }

        public void SetMaxHealth(int health) {
            this.maxHealth = health;
            this.health = health;
            this.oldHealth = health;
        }

        public void SetMaxRiseTime(int riseTime) {
            maxRiseTime = riseTime;
        }

        public void SetIsRise(bool status) {
            isRise = status;
        }

        public void DecreaseHealth(int amount) {
            oldHealth -= amount;

            if (oldHealth < 0) {
                DecreaseLives(1);

                if (lives > 0) {
                    oldHealth = maxHealth;
                } else {
                    oldHealth = 0;
                }
            }
        }

        public void IncreaseHealth(int amount) {
            oldHealth += amount;

            if (oldHealth > 100) {
                oldHealth = 100;
            }
        }

        public void DecreaseMP(int amount) {
            mp -= amount;

            if (mp < 0) {
                mp = 0;
            }
        }

        public void IncreaseMP(int amount) {
            mp += amount;

            if (mp > 100) {
                IncreaseMpLevel(1);

                if (currentMpLevel > maxMpLevel) {
                    currentMpLevel = 0;
                }

                mp = 0;
            }
        }

        public void DecreaseLives(int amount) {
            lives -= amount;

            if (lives < 0) {
                lives = 0;
            }
        }

        public void IncreaseLives(int amount) {
            lives += amount;
        }

        public void IncreasePoints(int amount) {
            points += amount;
        }

        public bool InBlockAction() {
            return (IsInAnimationAction(Animation.Action.BLOCKING) 
                        && GetAttackInfo().blockResistance > 0);
        }

        public void DecreaseBlockResistance(int amount = 1) {
            GetAttackInfo().blockResistance -= amount;

            if (GetAttackInfo().blockResistance < 0) {
                GetAttackInfo().blockResistance = 0;
            }
        }

        public void SetAlive(bool alive) {
            this.alive = alive;
        }

        public void SetIsLeft(bool isLeft) {
            foreach (Sprite sprite in spriteMap.Values) {
                sprite.SetIsLeft(isLeft);
            }
        }
        
        public void SetGround(float ground) {
            this.ground = ground;
        }

        public void SetGroundBase(float groundBase) {
            this.groundBase = groundBase;
            SetGround(groundBase);
        }
        
        public void SetBaseOffset(float x, float y) {
            baseOffset.X = x;
            baseOffset.Y = y;
        }

        public void SetCurrentSpriteFrame(int frame) {
            currentSprite.SetCurrentFrame(frame);
        }

        public void SetCurrentSpriteAndFrame(Animation.State state, int frame) {
            SetAnimationState(state);
            currentSprite.SetCurrentFrame(frame);
        }

        public void SetAnimationType(Animation.State state, Animation.Type type) {
            Sprite sprite = GetSprite(state);

            if (sprite != null) {
                sprite.SetAnimationType(type);
            }
        }

        public void SetAnimationType(Animation.Type type) {
            foreach (Sprite sprite in spriteMap.Values) {
                sprite.SetAnimationType(type);
            }
        }

        public void SetMaxGrabbedHits(int hits) {
            grabInfo.maxGrabHits = hits;
        }

        public void SetMaxGrabbedTime(int time) {
            grabInfo.maxGrabbedTime = time;
        }

        public void SetGrabHeight(int height) {
            grabInfo.grabHeight = height;
        }

        public void SetThrowHeight(int height) {
            grabInfo.throwHeight = height;
        }

        public string GetName() {
            return name;
        }

        public void SetName(String name) {
            this.name = name;
        }

        public void SetIsPlatform(bool isPlatform) {
            this.isPlatform = isPlatform;
        }

        public bool IsPlatform() {
            return isPlatform;
        }

        public void SetMpLevel(int level) {
            maxMpLevel = level;
        }

        public void SetCurrentMpLevel(int level) {
            currentMpLevel = level;
        }

        public int GetMaxMpLevel() {
            return maxMpLevel;
        }

        public int GetCurrentMpLevel() {
            return currentMpLevel;
        }

        public void IncreaseMpLevel(int amount) {
            currentMpLevel += amount;
        }

        public void DecreaseMpLevel(int amount) {
            currentMpLevel -= amount;

            if (currentMpLevel < 0) {
                currentMpLevel = 0;
            }
        }

        public void SetPortrait(string location, int posx, int posy, int offx, int offy, float sx, float sy) {
            portrait = new Attributes.Portrait();
            portrait.location = location;
            portrait.posx = posx;
            portrait.posy = posy;
            portrait.offx = offx;
            portrait.offy = offy;
            portrait.sx = sx;
            portrait.sy = sy;
        }

        public Attributes.Portrait GetPortrait() {
            return portrait;
        }

        public int GetHealth() {
            return health;
        }

        public int GetOldHealth() {
            return oldHealth;
        }

        public int GetMaxHealth() {
            return maxHealth;
        }

        public int GetMaxLives() {
            return maxLives;
        }

        public int GetMP() {
            return mp;
        }

        public long GetPoints() {
            return points;
        }

        public long GetOldPoints() {
            return points;
        }

        public int GetLives() {
            return lives;
        }

        public bool Alive() {
            return alive;
        }

        public bool IsLeft() {
            return currentSprite.IsLeft();
        }

        public Vector3 GetPosition() {
            return position;
        }

        public float GetPosX() {
            return position.X;
        }

        public float GetPosY() {
            return position.Y;
        }

        public float GetPosZ() {
            return position.Z;
        }

        public float GetOffsetX() {
            return offset.X;
        }

        public float GetOffsetY() {
            return offset.Y;
        }

        public float GetOffsetZ() {
            return offset.Z;
        }

        public float GetVelX() {
            return velocity.X;
        }

        public float GetVelY() {
            return velocity.Y;
        }

        public float GetVelZ() {
            return velocity.Z;
        }

        public Vector2 GetScale() {
            return scale;
        }

        public Vector2 GetBaseScale(){
            return baseScale;
        }

        public float GetScaleX() {
            return scale.X;
        }

        public float GetScaleY(){
            return scale.Y;
        }

        public float GetBaseOffsetX() {
            return baseOffset.X;
        }

        public float GetBaseOffsetY() {
            return baseOffset.Y;
        }

        public float GetBaseScaleX() {
            return baseScale.X;
        }

        public float GetBaseScaleY(){
            return baseScale.Y;
        }

        public float GetAliveTime() {
            return aliveTime;
        }

        public bool InHitPauseTime() {
            return attackInfo.hitPauseTime > 0;
        }

        public bool IsRise() {
            return isRise;
        }

        public bool InPainTime() {
            return painTime > 0;
        }

        public int GetPainTime() {
            return painTime;
        }

        public void SetPainTime(int time) {
            painTime = time;
        }

        public void SetHitPauseTime(int time) {
            attackInfo.hitPauseTime = time;
        }

        public void SetBoundToLevel(bool bound) {
            boundToLevel = bound;
        }

        public bool IsBoundToLevel() {
            return boundToLevel;
        }

        public Animation.State? GetGrabItemAnimationState() {
            return grabItemState;
        }

        public Attributes.FrameInfo GetGrabItemFrameInfo() {
            return grabItemFrame;
        }

        public bool HasCollidedX() {
             return (direction.X < 0 && collisionInfo.IsLeft())
                        || (direction.X > 0 && collisionInfo.IsRight());
        }

        public bool HasCollidedZ() {
             return (direction.Z > 0.0 && collisionInfo.IsTop()) 
                        || (absoluteVel.Z < 0.0 && collisionInfo.IsBottom());
        }

        public Vector3 GetDirection() {
            return direction;
        }

        public int GetDirX() {
            return (int)direction.X;
        }

        public int GetDirY() {
            return (int)direction.Y;
        }

        public int GetDirZ() {
            return (int)direction.Z;
        }

        public float GetAccelX() {
            return acceleration.X;
        }

        public float GetAccelY() {
            return acceleration.Y;
        }

        public float GetAccelZ() {
            return acceleration.Z;
        }

        public float GetAbsoluteVelX() {
            return absoluteVel.X;
        }

        public float GetAbsoluteVelY() {
            return absoluteVel.Y;
        }

        public float GetAbsoluteVelZ() {
            return absoluteVel.Z;
        }

        public float GetGround() {
            return ground;
        }

        public float GetGroundBase() {
            return groundBase;
        }

        public ObjectType GetEntityType() {
            return type;
        }

        public bool IsEntity(ObjectType type) {
            return (this.type == type);
        }

        public void SetObjectType(ObjectType type) {
            this.type = type;
        }

        public Vector2 GetConvertedPosition() {
            convertedPosition.X = position.X;
            convertedPosition.Y = position.Y + position.Z;
            return convertedPosition;
        }

        public virtual Vector2 GetOrigin() {
            Sprite sprite = GetCurrentSprite();
            origin.X = (sprite.GetCurrentTexture().Width / 2);
            origin.Y = 0;

            return origin;
        }

        public Vector2 GetStanceOrigin() {
            Sprite sprite = GetSprite(Animation.State.STANCE);
            stanceOrigin.X = sprite.GetCurrentTexture().Width / 2;
            stanceOrigin.Y = 0;

            return stanceOrigin;
        }

        public Vector3 GetVelocity() {
            return velocity;
        }

        public Vector3 GetMaxVelocity() {
            return maxVelocity;
        }

        public Entity GetEntityLink() {
            return link;
        }

        public Attributes.GrabInfo GetGrabInfo() {
            return grabInfo;
        }

        public int GetSpriteCount() {
            return spriteMap.Count;
        }

        public Sprite GetSprite(Animation.State? state) {
            if (state != null && spriteMap.ContainsKey(state)) {
                return spriteMap[state];
            } else {
                return null;
            }
        }

        public bool HasSprite(Animation.State? state) {
            if (state != null) { 
                return spriteMap.ContainsKey(state);
            }

            return false;
        }

        public int GetSpriteFrames(Animation.State? state) {
            if (state != null) {
                return GetSprite(state).GetFrames();
            }

            return 0;
        }

        public Sprite GetCurrentSprite() {
            return currentSprite;
        }

        public int GetCurrentSpriteFrame() {
            return GetCurrentSprite().GetCurrentFrame();
        }

        public bool IsAnimationComplete() {
            return GetCurrentSprite().IsAnimationComplete();
        }

        public bool IsAnimationComplete(Animation.State? state) {
            if (state == null) {
                return false;
            }

            return GetSprite(state).IsAnimationComplete();
        }

        public Entity GetCurrentTarget() {
            return currentTarget;
        }

        public float GetSpriteWidth(Animation.State state) {
            return GetSprite(state).GetCurrentTexture().Width * GetScale().X;
        }

        public float GetSpriteHeight(Animation.State state) {
            return GetSprite(state).GetCurrentTexture().Height * GetScale().Y;
        }

        public float GetCurrentSpriteWidth() {
            return GetCurrentSprite().GetCurrentTexture().Width * GetScale().X;
        }

        public float GetCurrentSpriteHeight() {
            return GetCurrentSprite().GetCurrentTexture().Height * GetScale().Y;
        }

        public Sprite GetBaseSprite() {
            return baseSprite;
        }

        public Color GetSpriteColor() {
            return colorInfo.GetColor();
        }

        public Attributes.ColourInfo GetColourInfo() {
            return colorInfo;
        }

        public Color GetBaseColor() {
            return baseColor;
        }

        public void SetBaseColor(Color color) {
            baseColor = color;
        }

        public Vector2 GetBasePosition() {
            Vector2 diffScale = GetCurrentScale();
          
            basePosition.X = GetConvertedPosition().X + (baseOffset.X * diffScale.X);
            basePosition.Y = GetConvertedPosition().Y + boundsBox.GetHeight() + (baseOffset.Y + boundsBox.GetOffset().Y * diffScale.Y);
            return basePosition;
        }

        public void SetComboFont(MugenFont comboFont) {
            this.comboFont = comboFont;
        }

        public MugenFont GetComboFont() {
            return comboFont;
        }

        public virtual void ExpandComboFont() {
            if (comboFont == null) {
                if (this is Player) {
                    if (((Player)this).GetPlayerIndex() == 1) { 
                        comboFont = new MugenFont("Fonts/sfiii_combo.xFont", new Vector2(-200, 240), 4, 0, 2.8f);
                        comboFont.Translate(50, 0, 13, 0, 80, 1);
                    } else {
                        comboFont = new MugenFont("Fonts/sfiii_combo.xFont", new Vector2(1300, 240), 4, 0, 2.8f);
                        comboFont.Translate(1000, 0, 13, 0, 80, -1);
                    } 
                } 
            }

            ShowComboFont();
        }

        public void ShowComboFont() {
            if (comboFont != null) {
                comboFont.Translate();
            }
        }

        public CLNS.BoundingBox GetLastBoxFrame(Animation.State state, int frame) {
            return GetSprite(state).GetBoxes(frame).Last();
        }

        public CLNS.AttackBox GetAttackBox(Animation.State state, int frame) {
            return (CLNS.AttackBox)GetSprite(state).GetBoxes(frame).Last();
        }

        public List<CLNS.AttackBox> GetAttackBox(Animation.State state) {
            return GetSprite(state).GetAllBoxes(CLNS.BoxType.HIT_BOX).Cast<CLNS.AttackBox>().ToList();
        }

        public List<CLNS.AttackBox> GetAttackBoxes(Animation.State state, int frame) {
            return GetSprite(state).GetBoxes(frame).Cast<CLNS.AttackBox>().ToList();
        }

        public CLNS.AttackBox GetAttackBox(Animation.State state, int frame, int index) {
            return (CLNS.AttackBox)GetSprite(state).GetBoxes(frame)[index];
        }

        public List<CLNS.BoundingBox> GetAllFrameBoxes() {
            List<CLNS.BoundingBox> currentBoxes = new List<CLNS.BoundingBox>();

            foreach (Sprite sprite in spriteMap.Values) {
                currentBoxes.AddRange(sprite.GetAllBoxes());
            }

            return currentBoxes;
        }

        public CLNS.BoundingBox GetBodyBox() {
            return bodyBox;
        }

        public CLNS.BoundsBox GetBoundsBox() {
            return boundsBox;
        }

        public CLNS.BoundingBox GetDepthBox() {
            return depthBox;
        }

        public List<CLNS.BoundingBox> GetCurrentBoxes() {
            return GetCurrentSprite().GetCurrentBoxes();
        }

        public List<CLNS.BoundingBox> GetCurrentBoxes(CLNS.BoxType type) {
            return GetCurrentSprite().GetCurrentBoxes(type);
        }

        public SpriteEffects GetEffects() {
            return currentSprite.GetEffects();
        }

        public AiState.StateMachine GetAiStateMachine() {
            return aiStateMachine;
        }

        public BlendState GetBlendState() {
            return blendState;
        }

        public void SetBlendState(BlendState state) {
            blendState = state;
        }

        public Effect GetSpecialArt(String name) {
            if (specialArts.ContainsKey(name)) { 
                return specialArts[name];
            }

            return null;
        }

        public Effect GetHitSpark(Effect.State state) {
            if (hitSparks.ContainsKey(state)) { 
                return hitSparks[state];
            }

            return null;
        }

        public Effect GetBlockSpark(Effect.State state) {
            if (blockSparks.ContainsKey(state)) { 
                return blockSparks[state];
            }

            return null;
        }

        public bool IsInAnimationState(Animation.State? state) {
            return (currentSprite != null 
                        && state != null
                        && spriteMap.ContainsKey(state) 
                        && currentSprite == GetSprite(state)
                        && this.currentAnimationState == state);
        }

        public Animation.State? GetCurrentAnimationState() {
            return currentAnimationState;
        }

        public Animation.State? GetLastAnimationState() {
            return lastAnimationState;
        }

        public bool IsLastAnimationState(Animation.State? state) {
            return (lastAnimationState == state);
        }

        public bool IsInAnimationAction(Animation.Action animationAction) {
            Animation.Action currentAction = GetCurrentAnimationAction(GetCurrentAnimationState());
            return (currentAction == animationAction);
        }

        public Animation.Action GetCurrentAnimationAction() {
            return GetCurrentAnimationAction(GetCurrentAnimationState());
        }

        public bool IsLastAnimationAction(Animation.Action animationAction) {
            Animation.Action currentAction = GetCurrentAnimationAction(GetLastAnimationState());
            return (currentAction == animationAction);
        }

        public Animation.Action GetLastAnimationAction() {
            return GetCurrentAnimationAction(GetLastAnimationState());
        }

        public Animation.Action GetCurrentAnimationAction(Animation.State? currentState) {
            Animation.Action currentAction = Animation.Action.NONE;

            if (currentState.ToString().Contains("ATTACK") 
                    || currentState.ToString().Contains("SPECIAL")) {

                return Animation.Action.ATTACKING;

            } else {
                if (currentState.ToString().Contains("DIE")) {
                    return Animation.Action.DYING;

                } else if (currentState.ToString().Contains("RECOVER")) {
                    return Animation.Action.RECOVERY;

                } else if (currentState.ToString().Contains("JUMP")) {
                    return Animation.Action.JUMPING;

                } else if (currentState.ToString().Contains("FALLING")) {
                    return Animation.Action.FALLING;

                } else if (currentState.ToString().Contains("KNOCKED")
                            || currentState.ToString().Contains("BOUNCE")) {

                    return Animation.Action.KNOCKED;

                } else if (currentState.ToString().Contains("RUN") 
                            && !currentState.ToString().Contains("STOP")
                            && !currentState.ToString().Contains("ATTACK")) {

                    return Animation.Action.RUNNING;

                } else if (currentState.ToString().Contains("GRAB") 
                            && !currentState.ToString().Contains("IN")) {

                    return Animation.Action.GRABBING;

                } else if (currentState.ToString().StartsWith("INGRAB")) {
                    return Animation.Action.GRABBED;

                } else if (currentState.ToString().Contains("THROW") 
                            && !currentState.ToString().Contains("THROWN")) {

                    return Animation.Action.THROWING;

                } else if (currentState.ToString().Contains("THROWN")) {
                    return Animation.Action.KNOCKED;

                } else if (currentState.ToString().StartsWith("RUN_STOP")) {
                    return Animation.Action.STOPPING;

                } else if (currentState.ToString().Contains("PAIN")) {
                    return Animation.Action.INPAIN;

                } else if (currentState.ToString().Contains("RISE")) {
                    return Animation.Action.RISING;

                } else if (currentState.ToString().Contains("BLOCK")) {
                    return Animation.Action.BLOCKING;

                } else if (currentState.ToString().Contains("PICKUP")) {
                    return Animation.Action.PICKING_UP;

                } else if (currentState.ToString().Contains("NONE")) {
                    return Animation.Action.NONE;

                } else if (currentState.ToString().Contains("STANCE")) {
                    return Animation.Action.IDLE;

                } else if (currentState.ToString().Contains("WALK_TOWARDS")
                               || currentState.ToString().Contains("WALK_BACKWARDS")) {

                    return Animation.Action.WALKING;

                } else if (currentState.ToString().Contains("LAND")) {

                    return Animation.Action.LANDING;
                }
            }

            return currentAction;
        }

        public bool InGrabItemFrameState() {
            bool isValidGrabFrame = (IsInAnimationAction(Animation.Action.PICKING_UP) && IsAnimationComplete());

            if (GetGrabItemFrameInfo() != null) {
                isValidGrabFrame = (IsInAnimationState(GetGrabItemAnimationState()) && GetGrabItemFrameInfo().IsInFrame(GetCurrentSpriteFrame())); 
            }

            return isValidGrabFrame;
        }

        public bool InGrabAttackState() {
            return (IsInAnimationAction(Animation.Action.GRABBING) && GetGrabInfo().grabbed != null);
        }

        public bool InGrabState() {
            return (!IsInAnimationAction(Animation.Action.ATTACKING)
                            && !IsInAnimationAction(Animation.Action.KNOCKED)
                            && !IsInAnimationAction(Animation.Action.INPAIN)
                            && !InSpecialAttack()
                            && !IsDying()
                            && !IsKnocked()
                            && IsMoving()
                            && GetGrabInfo().grabbed == null);
        }

        public bool InvalidGrabbedState() {
            return IsInAnimationAction(Animation.Action.FALLING)
                        || IsInAnimationAction(Animation.Action.KNOCKED)
                        || IsInAnimationAction(Animation.Action.DYING)
                        || IsInAnimationAction(Animation.Action.RISING)
                        || IsInAnimationAction(Animation.Action.ATTACKING)
                        || IsInAnimationAction(Animation.Action.GRABBING)
                        || IsInAnimationAction(Animation.Action.JUMPING)
                        || IsInAnimationAction(Animation.Action.LANDING)
                        || !(GetGrabInfo().grabbedTime > 0)
                        || !GetGrabInfo().grabbable
                        || IsKnocked()
                        || IsDying()
                        || IsToss();
        }

        public bool ValidGrabItemState() {
            return !IsInAnimationAction(Animation.Action.FALLING)
                        && !IsInAnimationAction(Animation.Action.KNOCKED)
                        && !IsInAnimationAction(Animation.Action.DYING)
                        && !IsInAnimationAction(Animation.Action.RISING)
                        && !IsInAnimationAction(Animation.Action.KNOCKED)
                        && !IsInAnimationAction(Animation.Action.GRABBING)
                        && !IsInAnimationAction(Animation.Action.JUMPING)
                        && !IsInAnimationAction(Animation.Action.LANDING)
                        && !IsInAnimationAction(Animation.Action.INPAIN)
                        && !IsInAnimationAction(Animation.Action.ATTACKING)
                        && GetCollisionInfo().GetItem() != null
                        && !InPainTime()
                        && !HasGrabbed()
                        && !HasHit()
                        && !IsDying()
                        && !IsToss();
        }

        public bool IsInMoveFrame() {
            return ((moveFrames.ContainsKey(GetCurrentAnimationState()) 
                        && IsInAnimationState(GetCurrentAnimationState())
                            && currentSprite.GetCurrentFrame() >= moveFrames[GetCurrentAnimationState()]) 
                    || (!moveFrames.ContainsKey(GetCurrentAnimationState()) && IsInAnimationState(GetCurrentAnimationState()))
                    || moveFrames.Count == 0);
        }
        
        public int GetMoveFrame() {
            return (moveFrames.ContainsKey(GetCurrentAnimationState()) ? moveFrames[GetCurrentAnimationState()] : 0);
        }

        public bool IsInTossFrame() {
            return ((tossFrames.ContainsKey(GetCurrentAnimationState())
                        && IsInAnimationState(GetCurrentAnimationState())
                            && currentSprite.GetCurrentFrame() >= tossFrames[GetCurrentAnimationState()]
                            && IsFrameComplete(GetCurrentAnimationState(), tossFrames[GetCurrentAnimationState()] + 1))
                    || (!tossFrames.ContainsKey(GetCurrentAnimationState()) 
                            && IsInAnimationState(GetCurrentAnimationState()) && IsToss()) 
                    || tossFrames.Count == 0);
        }

        public int GetTossFrame() {
            return (tossFrames.ContainsKey(GetCurrentAnimationState()) ? tossFrames[GetCurrentAnimationState()] : 0);
        }

        public bool IsFrameComplete(Animation.State? state, int frame) {
            Sprite sprite = GetSprite(state);
            return sprite.IsFrameComplete(frame);
        }

        public int GetEntityId() {
            return entityId;
        }

        public bool IsToss() {
            return tossInfo.isToss;
        }

        public bool IsMoving() {
            return (IsMovingX() || IsMovingZ());
        }

        public bool IsMovingX() {
            return ((double)velocity.X != 0.0);
        }

        public bool IsMovingY() {
            return ((double)velocity.Y != 0.0 || (double)Math.Abs(GetPosY()) > 0.0);
        }

        public bool IsMovingZ() {
            return ((double)velocity.Z != 0.0);
        }

        public bool HasLanded() {
            return ((double)GetPosY() >= (double)GetGround() 
                        && (double)GetGroundBase() > (double)GetGround());
        }

        public bool IsOnGround() {
            return ((double)GetPosY() == (double)GetGroundBase());
        }

        public bool InAir() {
            return ((double)GetGround() > (double)GetPosY());
        }

        public Attributes.TossInfo GetTossInfo() {
            return tossInfo;
        }

        public ComboAttack.Chain GetDefaultAttackChain() {
            return defaultAttackChain;
        }

        public Animation.State GetCurrentAttackChainState() {
            return defaultAttackChain.GetCurrentAttackState();
        }

        public Animation.State GetPreviousAttackChainState() {
            return defaultAttackChain.GetPreviousAttackState();
        }

        public AfterImage GetAfterImageData() {
            return afterImage;
        }

        public Attributes.Rumble GetRumble() {
            return rumble;
        }

        public bool InCurrentAttackCancelState() {
            if (defaultAttackChain == null) return false;

            List<ComboAttack.Move> attackStates = defaultAttackChain.GetMoves().FindAll(item => item.GetState().Equals(GetCurrentAnimationState()));

            return IsInAnimationAction(Animation.Action.ATTACKING)
                        && attackStates.Count > 0
                        && IsInAnimationState(attackStates[0].GetState())
                        && GetCurrentSpriteFrame() >= attackStates[0].GetCancelFrame()
                        && IsFrameComplete(attackStates[0].GetState(), attackStates[0].GetCancelFrame() + 1);
        }

        public void ResetCurrentAnimation() {
            GetAttackInfo().Reset();
            GetCurrentSprite().ResetAnimation();
            SoundAction soundAction = GetSoundAction(GetCurrentAttackChainState());

            if (soundAction != null) {
                soundAction.SetStep(-1);
            }
        }

         public void ResetAnimation(Animation.State state) {
            SetAnimationState(state);
            GetAttackInfo().Reset();
            GetCurrentSprite().ResetAnimation();

            SoundAction soundAction = GetSoundAction(state);

            if (soundAction != null) {
                soundAction.SetStep(-1);
            }
        }

        public void ProcessAttackChainStep() {
            if (defaultAttackChain == null) {
                ResetAnimation(Animation.State.ATTACK1);

            } else { 
                if (!IsInAnimationAction(Animation.Action.ATTACKING) 
                        || InCurrentAttackCancelState()) {

                    SetAnimationState(GetCurrentAttackChainState());
                }

                if (InCurrentAttackCancelState()) {
                    ResetCurrentAnimation();
                }
            }
        }

        public bool InAttackFrame() {
            List<CLNS.BoundingBox> attackBoxes = GetCurrentSprite().GetCurrentBoxes(CLNS.BoxType.HIT_BOX);
            return IsInAnimationAction(Animation.Action.ATTACKING) && attackBoxes != null && attackBoxes.Count > 0;
        }

        public void RefreshComboHits(){
            if (GetComboFont() != null && GetComboFont().IsNotShowing()) { 
                GetAttackInfo().comboHits = 0;
            }
        }

        public Attributes.CollisionInfo GetCollisionInfo() {
            return collisionInfo;
        }

        public Attributes.AttackInfo GetAttackInfo() {
            return attackInfo;
        }

        public bool InSpecialAttack() {
            return GetCurrentAnimationState().ToString().Contains("SPECIAL");
        }

        public bool IsJumpingOrInAir() {
            return (IsToss() || IsInAnimationAction(Animation.Action.JUMPING));
        }

        public Dictionary<InputHelper.KeyPress, Keys> GetKeyboardSettings() {
            return keyboardSettings;
        }

        public Dictionary<InputHelper.KeyPress, Buttons> GetGamepadSettings() {
            return gamepadSettings;
        }

        public Dictionary<InputHelper.KeyPress, Keys> GetKeyboardButtonsOnly() {
            return keyboardBtnsOnly;
        }

        public Dictionary<InputHelper.KeyPress, Buttons> GetGamepadButtonsOnly() {
            return gamepadBtnsOnly;
        }

        public Keys GetKeyboardKey(InputHelper.KeyPress press) {
            return keyboardSettings[press];
        }

        public Buttons GetGamepadKey(InputHelper.KeyPress press) {
            return gamepadSettings[press];
        }

        public int GetLayerPos() {
            return layerPos;
        }

        public void SetLayerPos(int pos) {
            layerPos = pos;
        }

        public void SetJump(float height = -25f, float velX = 0f)  {
            if (tossInfo.tossCount < tossInfo.maxTossCount && !tossInfo.isToss) { 
                Toss(height, velX);
 
                if (HasSprite(Animation.State.JUMP_START)) {
                    SetAnimationState(Animation.State.JUMP_START);
                } else {
                    SetAnimationState(Animation.State.JUMP);
                }

                if ((double)velX != 0.0) {
                    if (HasSprite(Animation.State.JUMP_TOWARDS)) {
                        SetJumpLink(Animation.State.JUMP_TOWARDS);
                    } else {
                        SetJumpLink(Animation.State.JUMP);
                    }
                } else {
                    SetJumpLink(Animation.State.JUMP);
                }

            } else if (tossInfo.tossCount < tossInfo.maxTossCount && tossInfo.isToss) {
                Toss(height, tossInfo.velocity.X);
                tossInfo.inTossFrame = true;

                if ((double)tossInfo.velocity.X != 0.0) {
                    SetAnimationState(Animation.State.JUMP_TOWARDS);
                } else {
                    SetAnimationState(Animation.State.JUMP);
                }

                GetCurrentSprite().ResetAnimation();
            }
        }

        public bool IsAliveTime() {
            return aliveTime > 0.0;
        }

        public void SetAliveTime(float t) {
            aliveTime = t;
        }

        public void Toss(float height = -20, float velX = 0f, int maxToss = 1, int maxHitGround = 1, bool isKnock = false) {
            if (tossInfo.tossCount < maxToss) { 
                if ((double)velX < 0.0) {
                    direction.X = -1;
                } else if ((double)velX > 0.0) {
                    direction.X = 1;
                }

                if ((tossInfo.velocity.Y / GameManager.GAME_VELOCITY) >= -10 && tossInfo.tossCount > 0) { 
                    tossInfo.tempHeight += ((height / 2) * tossInfo.gravity);
                    tossInfo.height = tossInfo.tempHeight;
                    tossInfo.gravity = 0.45f * Math.Abs(tossInfo.height / 15);
                } else {
                    tossInfo.tempHeight = height * GameManager.GAME_VELOCITY;
                    tossInfo.height = tossInfo.tempHeight;
                }

                tossInfo.velocity.Y = tossInfo.height;
                tossInfo.velocity.X = velX;

                tossInfo.inTossFrame = false;
                tossInfo.isToss = true;

                tossInfo.hitGroundCount = 0;
                tossInfo.maxTossCount = maxToss;
                tossInfo.maxHitGround = maxHitGround;
                tossInfo.isKnock = isKnock;
                tossInfo.tossCount ++;
            }
        }

        public void TossFast(float height = -20, float velX = 0f, int maxToss = 1, int maxHitGround = 1, bool isKnock = false) {
            tossInfo.tossCount = 0;
            Toss(height, velX, maxToss, maxHitGround, isKnock);
        }

        public void SetTossGravity(float gravity) {
            tossInfo.gravity = gravity * GameManager.GAME_VELOCITY;
        }

        public void ResetToss() {
            velocity.X = 0f;
            velocity.Y = 0f;

            tossInfo.height = tossInfo.tempHeight = 0;
            tossInfo.velocity.X = 0f;
            tossInfo.velocity.Y = 0f;

            tossInfo.hitGroundCount = 0;
            tossInfo.tossCount = 0;
            tossInfo.gravity = 0.48f * GameManager.GAME_VELOCITY;

            tossInfo.inTossFrame = false;
            tossInfo.isToss = false;
        }

        public void UpdateToss(GameTime gameTime) {

            if (tossInfo.isToss) {

                if ((double)tossInfo.velocity.X < 0.0) {
                    direction.X = -1;

                } else if ((double)tossInfo.velocity.X > 0.0) {
                    direction.X = 1;
                }

                if (IsInTossFrame()) {

                    if (!tossInfo.inTossFrame) { 

                        if (IsInAnimationAction(Animation.Action.JUMPING)) {
                            GameManager.GetInstance().PlaySFX(this, GetCurrentAnimationState(), "jump");
                        }

                        if (IsInAnimationAction(Animation.Action.KNOCKED)) {
                            grabInfo.Reset();
                            attackInfo.Reset();
                        }

                        tossInfo.inTossFrame = true;
                    }
                }

                if (tossInfo.inTossFrame) {
                    MoveX(tossInfo.velocity.X);
                    VelY(tossInfo.velocity.Y);

                    tossInfo.velocity.Y += tossInfo.gravity;
                    
                    if (tossInfo.velocity.Y >= tossInfo.maxVelocity.Y) {
                        tossInfo.velocity.Y = tossInfo.maxVelocity.Y;
                    }
                }

                if ((int)GetPosY() > (int)GetGround()) {
                   
                    if (tossInfo.hitGroundCount < tossInfo.maxHitGround - 1) {

                        if (IsInAnimationAction(Animation.Action.KNOCKED)) {

                            SetAnimationState(Animation.State.BOUNCE1);
                            currentSprite.ResetAnimation();
                            GameManager.GetInstance().PlaySFX(this, Animation.State.BOUNCE1, "fall");
                        }
                    }

                    if (tossInfo.maxHitGround > 1) {
                        tossInfo.height += ((Math.Abs(tossInfo.tempHeight) / tossInfo.maxHitGround));
                        
                        if (tossInfo.height >= 0) {
                            tossInfo.height = 0;
                        }

                        SetPosY(GetGround());
                        tossInfo.velocity.Y = tossInfo.height;
                        MoveY(tossInfo.height / GameManager.GAME_VELOCITY);
                    }

                    if (tossInfo.hitGroundCount >= tossInfo.maxHitGround) {
                        SetPosY(GetGround());
                        
                        if (IsKnocked()) {
                            ResetJuggleHits();
                            attackInfo.attacker = null;
                            attackInfo.currentKnockedState = Attributes.KnockedState.NONE;
                            attackInfo.currentAttackTime = 0;
                            attackInfo.isHit = false;
                            SetIsRise(true);

                        } else { 
                            if (!IsDying()) { 
                                if (HasSprite(Animation.State.LAND1)) { 
                                    SetAnimationState(Animation.State.LAND1);
                                } else {
                                    SetAnimationState(Animation.State.STANCE);
                                }
                            } else {
                                if (IsDeathMode(Entity.DeathType.IMMEDIATE_DIE)) {
                                    if (InvalidDeathState()) {
                                        SetAnimationState(Animation.State.DIE1);
                                    }
                                }

                                SetDeathStep(1);
                            }
                            
                            if (IsInAnimationAction(Animation.Action.LANDING)) {
                                GameManager.GetInstance().PlaySFX(this, Animation.State.LAND1, "land");
                            }
                        }

                        GetGrabInfo().inGrabHeight = false;
                        GetTossInfo().isKnock = false;
                        ResetToss();
                    }

                    tossInfo.hitGroundCount += 1;
                }
            }
        }

        public void UpdateAnimationLinks(GameTime gameTime) {
            List<Animation.Link> links = animationLinks.FindAll(item => item.GetOnState() == GetCurrentAnimationState());

            foreach (Animation.Link link in links) {
                if (link.OnFrameComplete() && currentSprite.GetCurrentFrame() >= link.GetOnFrameStart()) {
                    if (IsFrameComplete(link.GetOnState(), link.GetOnFrameStart() + 1)) {
                        SetAnimationState(link.GetToState());
                    }
                } else {
                    if (link.GetOnFrameStart() == currentSprite.GetCurrentFrame()) {
                        SetAnimationState(link.GetToState());
                    }
                }
            }
        }

        public bool IsExpired() {
             return (IsEntity(Entity.ObjectType.HIT_FLASH) && GetCurrentSprite().IsAnimationComplete())
                            || (GetAliveTime() != -1 && GetAliveTime() <= 0);
        }

        public Entity GetOwner() {
            return owner;
        }

        public void SetOwner(Entity owner) {
            this.owner = owner;
        }

        public bool IsNonActionState() { 
            return (!IsToss() && !IsInAnimationAction(Animation.Action.ATTACKING) 
                              && !IsInAnimationAction(Animation.Action.GRABBING)
                              && !IsInAnimationAction(Animation.Action.THROWING)
                              && !IsInAnimationAction(Animation.Action.PICKING_UP)
                              && !IsInAnimationAction(Animation.Action.THROWING)
                              && !HasGrabbed()
                              && !IsGrabbed()
                              && !InPainTime()
                              && !IsKnocked()
                              && !IsDying());
        }

        public bool CanRunAction() {
            return (!IsToss() && !IsInAnimationAction(Animation.Action.ATTACKING) 
                              && !IsInAnimationAction(Animation.Action.GRABBING)
                              && !IsInAnimationAction(Animation.Action.PICKING_UP)
                              && !IsInAnimationAction(Animation.Action.THROWING)
                              && !HasGrabbed()
                              && !IsGrabbed()
                              && !InPainTime()
                              && !IsKnocked()
                              && !IsDying());
        }

        public bool InNegativeState() {
            return (IsInAnimationAction(Animation.Action.GRABBING)
                        || IsInAnimationAction(Animation.Action.THROWING)
                        || IsInAnimationAction(Animation.Action.INPAIN)
                        || IsInAnimationAction(Animation.Action.RECOVERY)
                        || HasGrabbed()
                        || IsGrabbed()
                        || InPainTime()
                        || IsKnocked()
                        || IsDying());
        }

        public void SetWalkState() {
             if (!IsInAnimationAction(Animation.Action.RUNNING) 
                    && !IsInAnimationAction(Animation.Action.GRABBING)) {

                SetAnimationState(Animation.State.WALK_TOWARDS);
             } 
        }

        public bool IsActionComplete(Animation.Action action) {
            return IsInAnimationAction(action) && IsAnimationComplete();
        }

        public bool InResetState() {
            return (!IsDying() && !InAir() && !InPainTime()
                        &&  (IsInAnimationAction(Animation.Action.WALKING)
                                || IsInAnimationAction(Animation.Action.RUNNING)
                                || IsInAnimationState(Animation.State.NONE)
                                || IsActionComplete(Animation.Action.JUMPING)
                                || IsActionComplete(Animation.Action.LANDING)
                                || IsActionComplete(Animation.Action.ATTACKING)
                                || IsActionComplete(Animation.Action.STOPPING)   
                                || IsActionComplete(Animation.Action.THROWING)      
                                || IsActionComplete(Animation.Action.INPAIN)     
                                || IsActionComplete(Animation.Action.RISING)
                                || IsActionComplete(Animation.Action.PICKING_UP)
                                || IsActionComplete(Animation.Action.FALLING)
                                /*|| IsActionComplete(Animation.Action.BLOCKING)*/));
        }

        public bool IsKnocked() {
            return (IsInAnimationAction(Animation.Action.KNOCKED) || GetTossInfo().isKnock);
        }

        public virtual void ResetToIdle(GameTime gameTime) {
            if (InResetState()) {
                int frame = (IsEntity(ObjectType.PLAYER) ? GetCurrentSpriteFrame() : GetCurrentSprite().GetFrames());

                bool isFrameComplete = (IsEntity(ObjectType.PLAYER)
                                            ? IsFrameComplete(GetCurrentAnimationState(), frame)
                                                 : IsFrameComplete(GetCurrentAnimationState(), frame)
                                                        && !IsInAnimationAction(Animation.Action.WALKING));

                if (isFrameComplete && !IsJumpingOrInAir()) {
                    if (IsInAnimationAction(Animation.Action.RUNNING) && HasSprite(Animation.State.RUN_STOP1)) {
                        SetAnimationState(Animation.State.RUN_STOP1);
                    } else { 
                        SetAnimationState(Animation.State.STANCE);
                    }
                }
            }
        }

        public void UpdateAnimation(GameTime gameTime) {
            currentSprite.UpdateAnimation(gameTime);
            UpdateAnimationLinks(gameTime);
        }

        public void UpdateNextAttackTime(GameTime gameTime) {
            if (attackInfo.currentAttackTime > 0) {
                attackInfo.currentAttackTime --;
            }

            if (attackInfo.currentAttackTime < 0) {
                attackInfo.currentAttackTime = 0;
            }
        }

        public void UpdateDefaultAttackChain(GameTime gameTime) {
            if (defaultAttackChain != null) {
                defaultAttackChain.UpdateCombo(gameTime);
            }
        }

        public void SetFade(int alpha) {
            colorInfo.alpha = (float)alpha;
        }

        public void SetColor(int r, int g, int b) {
            colorInfo.r = r;
            colorInfo.g = g;
            colorInfo.b = b;
        }

        public void Flash(float time = 5, float speed = 80f) {
            colorInfo.isFlash = true;
            colorInfo.expired = false;
            colorInfo.alpha = 255;
            colorInfo.fadeFrequency = speed;
            colorInfo.originalFreq = speed;
            colorInfo.currentFadeTime = 0f;
            colorInfo.maxFadeTime = time;
        }

        public bool IsFlash() {
            return (colorInfo.isFlash || colorInfo.alpha != 255);
        }

        public void UpdateFade(GameTime gameTime) {
            if (colorInfo.isFlash) {
                if (!colorInfo.expired) {
                    colorInfo.currentFadeTime += (float)gameTime.ElapsedGameTime.TotalSeconds;

                    if (colorInfo.alpha >= 255 || colorInfo.alpha <= 0) {
                        colorInfo.fadeFrequency *= -1;
                    }

                    colorInfo.alpha += colorInfo.fadeFrequency;
                }

                if (colorInfo.currentFadeTime > colorInfo.maxFadeTime) {
                    colorInfo.expired = true;
                }
                
                if (colorInfo.expired) {
                    colorInfo.currentFadeTime = 0f;

                    if (colorInfo.alpha != 255) {
                        float freq = colorInfo.originalFreq * 1f;

                        colorInfo.fadeFrequency = 1 * Math.Abs(freq);
                        colorInfo.alpha += colorInfo.fadeFrequency;
                    }

                    if (colorInfo.alpha >= 255) {
                        colorInfo.alpha = 255;
                        colorInfo.isFlash = false;
                        colorInfo.expired = false;
                    }
                }
            }
        }

        public void UpdateHealth(GameTime gameTime) { 
            if (health < oldHealth) {
                health++;
            }

            health = (int)MathHelper.Lerp((float)health, (float)oldHealth, 8.0f * (float)gameTime.ElapsedGameTime.TotalSeconds);
            SetLifebarPercent(health);
        }

        public void SetRumble(float dir = 1f, float force = 2.8f, float time = 50f, float maxForce = 20f) {
            if (rumble.count == 0) { 
                rumble.lastDir = dir;
                rumble.dir = dir;
            }

            rumble.force = force;
            rumble.isRumble = true;
            rumble.time = 0;
            rumble.maxTime = time;
            rumble.maxForceTime = maxForce;
            rumble.forceTime = rumble.maxForceTime;
        }

        public void UpdateRumble(GameTime gameTime) {
            if (!rumble.isRumble) {
                rumble.lx = GetPosX();
            }

            if (rumble.isRumble == true) {
                rumble.forceTime -= 0.5f * (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                rumble.time += 0.1f * (float)gameTime.ElapsedGameTime.TotalMilliseconds;

                if (rumble.forceTime <= 0) {
                    if (rumble.count == 0) {
                        rumble.dir = rumble.lastDir;
                    } else {
                        rumble.dir = -rumble.dir;
                    }
                    
                    TransformX(rumble.force * rumble.dir);
                    rumble.count ++;
                    rumble.forceTime = rumble.maxForceTime;
                }

                if (rumble.time >= rumble.maxTime) {
                    rumble.dir = rumble.lastDir;
                    rumble.count = 0;
                    rumble.isRumble = false;
                    rumble.time = 0;
                    rumble.forceTime = rumble.maxForceTime;
                }
            }       
        }

        public void UpdatePauseHit(GameTime gameTime) {
            if (attackInfo.hitPauseTime > 0) {
                attackInfo.hitPauseTime --;
            }

            if (attackInfo.hitPauseTime < 0) {
                attackInfo.hitPauseTime = 0;
            }
        }

        public void UpdateAliveTime(GameTime gameTime) {
            if (aliveTime != -1) {
                aliveTime --;

                if (aliveTime <= 0) {
                    aliveTime = 0;
                }
            }
        }

        public void UpdateRiseTime(GameTime gameTime) {
            if (isRise) {
                riseTime --;

                if (riseTime < 0) {
                    SetAnimationState(Animation.State.RISE1);
                    riseTime = maxRiseTime;
                    isRise = false;
                }
            }
        }

        public void UpdatePainTime(GameTime gameTime) {
            if (painTime > 0) {
                painTime --;
            }

            if (painTime <= 0) {
                if (IsDying()) {
                    attackInfo.isHit = false;
                    painTime = -1;

                } else { 
                    if (painTime != -1 
                            && IsInAnimationAction(Animation.Action.INPAIN) 
                            && !IsGrabbed()) {

                        if (!currentSprite.IsAnimationComplete()) {
                            painTime = 0;
                        } else {
                            SetAnimationState(Animation.State.STANCE);
                            attackInfo.currentAttackTime = 0;
                            attackInfo.isHit = false;
                            painTime = -1;
                        }
                    } else {
                        attackInfo.isHit = false;
                        painTime = -1;
                    }
                }
            }
        }

        public void UpdateFrameActions(GameTime gameTime) {
            if (frameActions.Count > 0) {
                
                foreach (FrameAction action in frameActions) {
                    
                    if (action.GetAnimationState() == GetCurrentAnimationState()) {

                        if (action.IsInFrame(GetCurrentSpriteFrame())) { 

                            if (IsEdgeX() == false && GetCollisionInfo().GetCollideX() == Attributes.CollisionState.NO_COLLISION) { 

                                if (action.GetMoveX() != 0.0) {
                                    TransformX((action.GetMoveX() * GetDirX()));
                                }
                            }

                            if (action.GetMoveY() != 0.0) {
                                MoveY(action.GetMoveY());
                            }

                            if (action.GetTossHeight() != 0.0) {
                                TossFast(action.GetTossHeight());
                            }
                        } 
                    }
                }
            }
        }

        public void SetDeathMode(DeathType deathMode) {
            this.deathMode = deathMode;
        }

        public DeathType GetDeathMode() {
            return deathMode;
        }

        public bool IsHittable() {
            return attackInfo.isHittable;
        }

        public void SetIsHittable(bool status) {
            attackInfo.isHittable = status;
        }

        public bool IsCollidable() {
            return collisionInfo.IsCollidable();
        }

        public void SetIsCollidable(bool status) {
            collisionInfo.SetIsCollidable(status);
        }

        public bool CanHurtOthers() {
            return attackInfo.canHurtOthers;
        }

        public void SetCanHurtOthers(bool status) {
            attackInfo.canHurtOthers = status;
        }

        public bool IsDrawShadow() {
            return drawShadow;
        }

        public void SetDrawShadow(bool status) {
            drawShadow = status;
        }

        public bool IsDeathMode(DeathType deathMode) {
            return this.deathMode.HasFlag(deathMode);
        }

        public bool IsDying() {
            return ((GetOldHealth() <= 0 && lives <= 0) || IsInAnimationAction(Animation.Action.DYING));
        }

        public bool InvalidHitState() {
            return (IsInAnimationAction(Animation.Action.KNOCKED)
                        || IsInAnimationAction(Animation.Action.RISING)
                        || IsDying()
                        || IsRise()
                        || IsKnocked());
        }

        public bool InvalidDeathState() {
            return (IsInAnimationAction(Animation.Action.KNOCKED)
                        || IsInAnimationAction(Animation.Action.RISING)
                        || IsKnocked()
                        || IsRise());
        }

        public int GetDeathStep() {
            return deathStep;
        }

        public void SetDeathStep(int step) {
            deathStep = step;
        }

        public virtual void OnDeath() {
        }

        public virtual void OnRun() {
        }

        public virtual void Actions(GameTime gameTime) {
            if (this is Projectile) {
                if (!InAttackFrame()) {
                    GetAttackInfo().Reset();
                }
            }
        }

        public void Update(GameTime gameTime) {
            UpdatePosition(gameTime);
        }

        public SFIII_SimpleLifebar GetLifeBar() {
            return lifeBar;
        }

        public void RenderLifebar() {
            if (lifeBar != null) {
                lifeBar.Render();
            }
        }

        public void UpdateLifebar(GameTime gameTime) {
            if (lifeBar != null) {
                if (IsDying()) {
                    if (!lifeBar.GetPortrait().IsFlash()) {
                        lifeBar.GetPortrait().Flash(GameManager.DEATH_FLASH_TIME);
                    }
                }

                lifeBar.Update(gameTime);
            }
        }

        public void SetLifebarPercent(float percent) {
            if (lifeBar != null) {
                lifeBar.SetPercent(percent);
            }
        }

        public void IncreaseLifebar(float amount) {
            if (lifeBar != null) {
                lifeBar.Increase(amount);
            }
        }

        public void DecreaseLifebar(float amount) {
            if (lifeBar != null) {
                lifeBar.Decrease(amount);
            }
        }

        public void UpdateBoxes(GameTime gameTime) {
            //Update bounding boxes.
            foreach (CLNS.BoundingBox box in GetAllFrameBoxes()) {
                box.Update(gameTime, this);
            }
            
            if (boundsBox != null) {
                boundsBox.Update(gameTime, this);
            }

            if (bodyBox != null) {
                bodyBox.Update(gameTime, this);
            }

            if (depthBox != null) {
                depthBox.Update(gameTime, this);
            }
        }

        public void UpdatePosition(GameTime gameTime) {
            Vector2 drawScale = scale;

            if (IsEntity(ObjectType.LIFE_BAR)) {
                drawScale = tempScale;
            }

            foreach (Sprite sprite in spriteMap.Values) {
                sprite.Update(gameTime, position, drawScale);
            }

            //Update movement.
            velocity.X += acceleration.X * direction.X;
            velocity.Y += acceleration.Y * direction.Y;
            velocity.Z += acceleration.Z * direction.Z;

            velocity.X = MathHelper.Clamp(velocity.X, -maxVelocity.X, maxVelocity.X);
            velocity.Z = MathHelper.Clamp(velocity.Z, -maxVelocity.Z, maxVelocity.Z);

            if ((double)velocity.X != 0.0) { 
                absoluteVel.X = (velocity.X / GameManager.GAME_VELOCITY);
            }

            if ((double)velocity.Y != 0.0) { 
                absoluteVel.Y = (velocity.Y / GameManager.GAME_VELOCITY);
            }

            if ((double)velocity.Z != 0.0) { 
                absoluteVel.Z = (velocity.Z / GameManager.GAME_VELOCITY);
            }

            if (IsInMoveFrame() && InHitPauseTime() == false) { 
                position.X += velocity.X * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            if(InHitPauseTime() == false) {
                position.Y += velocity.Y * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            if (IsInMoveFrame() && InHitPauseTime() == false) { 
                position.Z += velocity.Z * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            BoundEntityToScreen();
        }

        public Vector2 GetCurrentScale() {
            currentScale.X = ((GetScaleX() - GetBaseScaleX()) / GetBaseScaleX()) + 1;
            currentScale.Y = ((GetScaleY() - GetBaseScaleY()) / GetBaseScaleY()) + 1;
            return currentScale;
        }

        public virtual void BoundEntityToScreen() {
            if ((this is Player || IsEntity(ObjectType.PLAYER) || IsBoundToLevel()) 
                    && boundsBox != null && depthBox != null
                    && GameManager.GetInstance().CurrentLevel != null) {

                Vector2 diffScale = GetCurrentScale();
                
                scrollOffset.X = (float)((boundsBox.GetWidth() / 2) * (double)((double)GameManager.Camera.ViewPort.Width / (double)GameManager.RESOLUTION_X));
                scrollMax.X = GameManager.Camera.ViewPort.Width - scrollOffset.X;
                scrollMin.X = GameManager.GetInstance().CurrentLevel.X_MIN + scrollOffset.X;

                float sx1 =  scrollMax.X - 25;
                float sx2 =  scrollMin.X + 25;

                Vector2 pos = GameManager.Camera.WorldToScreen(GetConvertedPosition());
                Vector2 max = GameManager.Camera.ScreenToWorld(scrollMax);
                Vector2 min = GameManager.Camera.ScreenToWorld(scrollMin);

                if (!HasGrabbed() && !IsGrabbed()) {

                    if (GetCollisionInfo().GetCollideX() == Attributes.CollisionState.NO_COLLISION) {

                        if ((double)pos.X > (double)sx1) { 
                            isEdgeX = true;  
                            
                        } else if ((double)pos.X < (double)sx2) { 
                            isEdgeX = true;

                        } else {
                            isEdgeX = false;
                        }

                        if ((double)pos.X > (double)scrollMax.X) { 
                            SetPosX(max.X);            
                        } else if ((double)pos.X < (double)scrollMin.X) { 
                            SetPosX(min.X);
                        }
                    }

                    if (GetCollisionInfo().GetCollideZ() == Attributes.CollisionState.NO_COLLISION) {

                        if ((GetPosZ() + GetOffsetZ()) < GameManager.GetInstance().CurrentLevel.Z_MIN + 40) {
                            isEdgeZ = true;

                        } else if ((GetPosZ() + GetOffsetZ()) > (GameManager.GetInstance().CurrentLevel.Z_MAX - (depthBox.GetRect().Height)) - 60) {
                            isEdgeZ = true;

                        } else {
                            isEdgeZ = false;
                        }

                        if ((GetPosZ() + GetOffsetZ()) < GameManager.GetInstance().CurrentLevel.Z_MIN) {
                            SetPosZ(GameManager.GetInstance().CurrentLevel.Z_MIN - GetOffsetZ());

                        } else if ((GetPosZ() + GetOffsetZ()) > GameManager.GetInstance().CurrentLevel.Z_MAX - (depthBox.GetRect().Height)) {
                            SetPosZ((GameManager.GetInstance().CurrentLevel.Z_MAX - (depthBox.GetRect().Height)) - GetOffsetZ());
                        }
                    }
                }
            }
        }

        public bool IsHit() {
            return attackInfo.isHit;
        }

        public bool HasHit() {
            return attackInfo.hasHit;
        }

        public bool IsGrabbed() {
            return (grabInfo.isGrabbed || IsInAnimationAction(Animation.Action.GRABBED));
        }

        public bool HasGrabbed() {
            return (GetGrabInfo().grabbed != null || IsInAnimationAction(Animation.Action.GRABBING));
        }

        public virtual void OnAttackHit(Entity target, CLNS.AttackBox attackBox) {
            if (this != target) {
            }
        }

        public virtual void OnTargetHit(Entity attacker, CLNS.AttackBox attackBox) {
            if (this != attacker) {
            }
        }

        public virtual void OnCommandMoveComplete(InputHelper.CommandMove command) {
        }

        public Animation.State? GetLowPainState() {
            return animationConfig.lowPainState;
        }

        public Animation.State? GetMediumPainState() {
            return animationConfig.mediumPainState;
        }

        public Animation.State? GetHeavyPainState() {
            return animationConfig.heavyPainState;
        }

        public Animation.State? GetLightPainGrabbedState() {
            return animationConfig.lowPainGrabbedState;
        }

        public Animation.State? GetMediumPainGrabbedState() {
            return animationConfig.mediumPainGrabbedState;
        }

        public Animation.State? GetHeavyPainGrabbedState() {
            return animationConfig.heavyPainGrabbedState;
        }

        public Animation.State? GetGrabbedState() {
            return animationConfig.grabbedState;
        }

        public Animation.State? GetGrabHoldState() {
            return animationConfig.grabHoldState;
        }

        public Animation.State? GetThrowState() {
            return animationConfig.throwState;
        }

        public void SetLowPainState(Animation.State state) {
            animationConfig.lowPainState = state;
        }

        public void SetMediumPainState(Animation.State state) {
            animationConfig.mediumPainState = state;
        }

        public void SetHeavyPainState(Animation.State state) {
            animationConfig.heavyPainState = state;
        }

        public void SetLowPainGrabbedState(Animation.State state) {
            animationConfig.lowPainGrabbedState = state;
        }

        public void SetMediumPainGrabbedState(Animation.State state) {
            animationConfig.mediumPainGrabbedState = state;
        }

        public void SetHeavyPainGrabbedState(Animation.State state) {
            animationConfig.heavyPainGrabbedState = state;
        }

        public void SetGrabbedState(Animation.State state) {
            animationConfig.grabbedState = state;
        }

        public void SetGrabHoldState(Animation.State state) {
            animationConfig.grabHoldState = state;
        }

        public void SetBlockResistance(int resistance) {
            attackInfo.maxBlockResistance = attackInfo.blockResistance = resistance;
        }

        public int GetBlockResistance() {
            return attackInfo.blockResistance;
        }

        public void SetGrabResistance(int resistance) {
            grabInfo.maxGrabResistance = grabInfo.grabResistance = resistance;
        }

        public int GetGrabResistance() {
            return grabInfo.grabResistance;
        }

        public void DecreaseGrabResistance(int amount = 1) {
            grabInfo.grabResistance -= amount;

            if (grabInfo.grabResistance < 0) {
                grabInfo.grabResistance = 0;
            }
        }

        public void SetThrowState(Animation.State state) {
            animationConfig.throwState = state;
        }

        public bool InJuggleState() {
            float y1 = Math.Abs(GetPosY());
            float y2 =  Math.Abs(GetGround());
            bool inJuggleHitHeight = y1 > y2 + GetJuggleHitHeight();

            return (GetAttackInfo().juggleHits >= 0 
                        && GetAttackInfo().lastJuggleState != -1
                        && IsInAnimationAction(Animation.Action.KNOCKED) 
                        && GetTossInfo().hitGroundCount < 1
                        && inJuggleHitHeight
                        && InAir()
                        && !IsDying());
        }

        public void SetJuggleHits(int hits) {
            attackInfo.maxJuggleHits = attackInfo.juggleHits = hits;
        }

        public void TakeJuggleHit(int amount = 1) {
            attackInfo.juggleHits -= amount;

            if (attackInfo.juggleHits < 0) {
                attackInfo.juggleHits = 0;
            }
        }

        public void ResetJuggleHits() {
            attackInfo.juggleHits = attackInfo.maxJuggleHits;
        }

        public void SetJuggleHitHeight(int height) {
            GetAttackInfo().juggleHitHeight = height;
        }

        public int GetJuggleHitHeight() {
            return GetAttackInfo().juggleHitHeight;
        }

        public void SetAttackState(Animation.State attack) {
            if ((double)attackInfo.currentAttackTime == 0.0) {
                SetAnimationState(attack);
            }
        }

        public bool IsEdgeX() {
            return isEdgeX;
        }

        public bool IsEdgeZ() {
            return isEdgeZ;
        }

        public int CompareTo(Entity other) {
            if (other == null) {
                return 0;
            }

            int h1 = (GetDepthBox() != null ? GetDepthBox().GetRect().Bottom + GetLayerPos() : GetLayerPos());
            int h2 = (other.GetDepthBox() != null ? other.GetDepthBox().GetRect().Bottom + other.GetLayerPos() : other.GetLayerPos());
            int dist = -1;

            if (h1.Equals(h2)) {
                dist = GetEntityId().CompareTo(other.GetEntityId());
            } else {
                dist = h1.CompareTo(h2);
            }
            
            return dist;
        }
    }
}
