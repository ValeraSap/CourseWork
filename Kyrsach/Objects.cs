using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Kyrsach
{
    class Object
    {
        protected Texture2D texture2d;
        protected Rectangle rectangle;
        public void Draw(SpriteBatch spritebatch)
        {
            spritebatch.Draw(texture2d, rectangle, Color.White);
        }
        public Rectangle getRectangle()
        {
            return this.rectangle;
        }
        
    }
    class GameButton : Object
    {
        
        protected Texture2D texture;
        protected Texture2D textureOnFocus;
        private bool isFocused;
        public GameButton(Texture2D texture,Texture2D focusedTexture,Rectangle rectangle){
            this.texture = texture;
            this.textureOnFocus = focusedTexture;
            this.rectangle = rectangle;
            isFocused = false;
        }
        public void switchState()
        {
            isFocused = !isFocused;
        }
        public  void  Draw(SpriteBatch spritebatch)
        {
            if (isFocused)
            {
                spritebatch.Draw(textureOnFocus, rectangle, Color.White);
            }
            else
            {
                spritebatch.Draw(texture, rectangle, Color.White);
            }
        }

    }
    class GameMenu
    {
        public int focusedId{get;set;}
        List<GameButton> buttons;
        public GameMenu( ){
            focusedId = -1;
            buttons = null; 
        }
        public GameMenu(params GameButton[] buttons){
            this.buttons = new List<GameButton>(0);
            this.buttons.AddRange(buttons);
            focusedId = 0;
            this.buttons[0].switchState();
            

        }
        public void NextButton(){
            if (focusedId >= 0)
            {
                buttons[focusedId].switchState();
                if (focusedId + 1 >= buttons.Count)
                {
                    focusedId = 0;
                    buttons[focusedId].switchState();
                }
                else
                {
                    buttons[focusedId + 1].switchState();
                    focusedId++;
                }
            }
        }
        public void PreviousButton(){
            if (focusedId >= 0)
            {
                buttons[focusedId].switchState();
                if (focusedId - 1 < 0)
                {
                    focusedId = buttons.Count-1;
                    buttons[focusedId].switchState();
                }
                else
                {
                    focusedId--;
                    buttons[focusedId].switchState();
                    
                }
            }
        }
        public void Draw(SpriteBatch spritebatch){
            foreach (GameButton item in buttons){
                item.Draw(spritebatch);
            }
        }

    }
    class Door : Object
    {
        public Door(Texture2D newtexture2d, Rectangle newrectangle)
        {
            texture2d = newtexture2d;
            rectangle = newrectangle;
        }
        public bool IntersectionCheck(Hero hero, Door door)
        {
            if (door.rectangle.Intersects(hero.getRectangle()))
                return true;
            return false;
        }
    }
    
    class Heal : Object
    {
        public Heal(Texture2D newtexture2d, Rectangle newrectangle)
        {
            texture2d = newtexture2d;
            rectangle = newrectangle;
        }
    }
    class Background : Object
    {
        public Background(Texture2D newtexture2d, Rectangle newrectangle)
        {
            texture2d = newtexture2d;
            rectangle = newrectangle;
        }
    }
   
    class LifeBar : Object
    {
        private int hitPoints;
        public LifeBar(Texture2D texture, Rectangle rectangle,int hitPoints){
            this.texture2d = texture;
            this.rectangle = rectangle;
            this.hitPoints = hitPoints;
        }
        public void Update(int newHealth)
        {
            rectangle.Width=(newHealth * rectangle.Width) / hitPoints;
            hitPoints = newHealth;                     
        }
    }
}
