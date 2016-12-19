using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Kyrsach {
    class Character {

        protected Texture2D texture;
        protected Rectangle rectangle;
        protected Animation animation;
        // protected Rectangle sourceRectangle;
        protected int screenWidth;
        protected int screenHeight;
        

        public virtual void Update(GameTime gameTime) { }

        public virtual void Draw(SpriteBatch spriteBatch) {
            spriteBatch.Draw(texture, rectangle, Color.White);
        }

        public Rectangle getRectangle() {
            return this.rectangle;
        }
    }
    class Hero : Character {
        private float speed;
        private int maxHitPoints;
        private int currentHitPoints;
        private float time = 0;

        public Hero(Texture2D texture, Rectangle rectangle, 
            float speed, int screenWidth, int screenHeight,int HP)
        {
            this.texture = texture;
            this.rectangle = rectangle;
            this.speed = speed;
            this.screenWidth = screenWidth;
            this.screenHeight = screenHeight;
            maxHitPoints = HP;
            currentHitPoints = maxHitPoints;
            animation = new HeroAnimation(texture, 64, 70);
        }

        public bool IntersectionCheck(Hero hero, Enemy enemy) {
            if (hero.rectangle.Intersects(enemy.getRectangle()))
                return true;
            return false;
        }

        public int getHp() {
            return currentHitPoints;
        }
        public void OnDamage(int value){
            int hit=(int)((float)(value*maxHitPoints)/100);
            currentHitPoints-=(hit>0)?hit:1;
        }

        public void StartPlace() {
            rectangle.X = 0;
            rectangle.Y = 297;
        }

        public override void Update(GameTime gameTime) {

            time += (float)gameTime.ElapsedGameTime.TotalSeconds;
            //Движение песонажа нажатием клавиш
            if (Keyboard.GetState().IsKeyDown(Keys.Right)) {
                rectangle.X += Convert.ToInt32(speed);
                while (time * 100 > 10) {
                    ((HeroAnimation) animation).PlayRightAnimation();
                    time = 0f;
                }
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Left)) {
                rectangle.X -= Convert.ToInt32(speed);
                while (time * 100 > 10) {
                    ((HeroAnimation) animation).PlayLeftAnimation();
                    time = 0f;
                }
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Up)) {
                rectangle.Y -= Convert.ToInt32(speed);
                ((HeroAnimation)animation).PlayRightAnimation();
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Down)) {
                rectangle.Y += Convert.ToInt32(speed);
                ((HeroAnimation)animation).PlayRightAnimation();
            }
            

            //запрет на выход за пределы экрана
            if (rectangle.X <= 0)
                rectangle.X = 0;
            if (rectangle.Y <= 0)
                rectangle.Y = 0;
            if (rectangle.X + rectangle.Width >= screenWidth)
                rectangle.X = screenWidth - rectangle.Width;
            if (rectangle.Y + rectangle.Height >= screenHeight)
                rectangle.Y = screenHeight - rectangle.Height;
        }

        public override void Draw(SpriteBatch spriteBatch) {
            (animation).Draw(spriteBatch, rectangle);
        }

    }
    class Enemy : Character {
        private Vector2 velocity;
        private float speedX;
        private float speedY;
        

        public Enemy(Texture2D texture, Rectangle rectangle, 
            int screenWidth, int screenHeight, float SpeedX, float SpeedY)
        {
            this.texture = texture;
            this.rectangle = rectangle;
            this.screenWidth = screenWidth;
            this.screenHeight = screenHeight;
            this.speedX = SpeedX;
            this.speedY = SpeedY;
        }

        public override void Update(GameTime gameTime) {
            //самостоятельное движение врагов
            velocity.X = speedX;
            velocity.Y = speedY;
            rectangle.X += Convert.ToInt32(velocity.X);
            rectangle.Y += Convert.ToInt32(velocity.Y);

            if ((rectangle.X <= 0) || (rectangle.X + rectangle.Width >= screenWidth)) {
                speedX = -speedX;
            }
            if ((rectangle.Y <= 0) || (rectangle.Y + rectangle.Height >= screenHeight)) {
                speedY = -speedY;
            }
            if (this.rectangle.Intersects(new Rectangle(0, 235, 85, 145))) {
                speedX = -speedX;
                speedY = -speedY;
            }
        }       

    }
    class Princess : Character {
        private float time;
        public Princess(Texture2D texture, Rectangle rectangle) {
            this.texture = texture;
            this.rectangle = rectangle;
            animation = new PrincessAnimation(texture,300,200);
        }
        public override void Update(GameTime gameTime) {
            time += (float)gameTime.ElapsedGameTime.TotalSeconds;
            while (time * 100 > 20) {
                ((PrincessAnimation)animation).playIdleAnimation();
                time = 0f;
            }
        }
        public override void Draw(SpriteBatch spriteBatch) {
            animation.Draw(spriteBatch,rectangle);
        }

    }
    abstract class Animation {
        public Texture2D texture { get; set; }
        protected int frameWidth;
        protected int frameHeight;
        protected int frameX;
        protected int frameY;
        protected int currentFrame;
        
        public Animation(Texture2D texture, int frameWidth, int frameHeight) {
            this.texture = texture;
            this.frameHeight = frameHeight;
            this.frameWidth = frameWidth;
        }
      

        public void Draw(SpriteBatch spriteBatch, Rectangle destination) {

            Rectangle sourceRectangle = new Rectangle(currentFrame * frameWidth,
                frameY, frameWidth, frameHeight);

            spriteBatch.Draw(texture, destination, sourceRectangle, Color.White);
        }

    }
    class HeroAnimation : Animation {
        public HeroAnimation(Texture2D texture, int frameWidth, int frameHeight)
            : base(texture, frameWidth, frameHeight) {
            currentFrame = 0;
            frameX = 0;
            frameY = 0;

        }
        public void PlayRightAnimation() {
            if (currentFrame < 3) {
                currentFrame++;
            } else {
                currentFrame = 0;
            }
      

        }
        public void PlayLeftAnimation() {
            if (currentFrame >= 4 && currentFrame < 7) {
                currentFrame++;
            } else {
                currentFrame = 4;
            }
        }   
    }

    class PrincessAnimation : Animation {
        public PrincessAnimation(Texture2D texture, int frameWidth, int frameHeight)
            : base(texture, frameWidth, frameHeight) {
            currentFrame = 0;
            frameX = 0;
            frameY = 0;

        }
        public void playIdleAnimation() {
            frameY = 0;
            frameWidth=200;
            frameHeight = 300;
            if (currentFrame < 10) {
                currentFrame++;
            } else {
                currentFrame = 0;
            }
        }
                      

    }

}
