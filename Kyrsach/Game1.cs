using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using System.Threading;

namespace Kyrsach {

    public class Game1 : Game {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Hero hero;//персонаж
        Princess princess;//принцесса

        SoundEffect soundEffect;//звуковые эффекты
        Song backgroundMusic;//фоновая музыка

        GameMenu mainMenu;//основное меню
        GameMenu restartMenu;//меню перезапуска
        GameButton textbox; 

        List<Background> background = new List<Background>();
        List<Enemy> gardenEnemy = new List<Enemy>();
        List<Enemy> hallEnemy = new List<Enemy>();
        List<Enemy> libraryEnemy = new List<Enemy>();
        List<Door> door = new List<Door>();       

        private int ScreenWidth;//параметры окна
        private int ScrenHeight;

        private int enemyDamage;//урон 

        float heroSpeed;//скорость движения персонажа
        float time = 0;

        bool temp = false;
        GameEnum CurrentScreen;
        GameState gameState;

        LifeBar lifeBar;
        Background lifeFrame;

        enum GameState{//перечисление вариантов меню
            MainMenu,
            Gameplay,
            RestartMenu,
            GamePause
        }
        
        enum GameEnum {//перечисление уровней
            Garden = 0,
            CastleHoll,
            Library,
            Dungeon
        }

        private Random rand = new Random();
        public int Random() {//рекурсивная функция для получения случайного числа
            int r;
            r = rand.Next(-5, 5);
            if (r == 0)
                return Random();
            else
                return r;
        }

        bool StartMov() {
            if (Keyboard.GetState().IsKeyDown(Keys.Right))
                temp = true;
            return temp;
        }


        public Game1() {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferHeight = 620;
            graphics.PreferredBackBufferWidth = 1080;           
            gameState = GameState.MainMenu;
        }

        protected override void Initialize() {
            
            base.Initialize();
        }


        protected override void LoadContent() {

            spriteBatch = new SpriteBatch(GraphicsDevice);
            ScreenWidth = GraphicsDevice.Viewport.Width;
            ScrenHeight = GraphicsDevice.Viewport.Height;

            enemyDamage = 10;

            gameState = GameState.MainMenu;
            CurrentScreen = GameEnum.Garden;

            backgroundMusic = Content.Load<Song>("backgroundMusic");
            MediaPlayer.Volume = 0.1f;
            MediaPlayer.Play(backgroundMusic);
            soundEffect = Content.Load<SoundEffect>("krik");
            SoundEffect.MasterVolume = 0.1f;

            mainMenu = new GameMenu(
                new GameButton(Content.Load<Texture2D>("start"),
                    Content.Load<Texture2D>("startChosen"),new Rectangle(200,50,400,200)),
                new GameButton(Content.Load<Texture2D>("Exit"), 
                    Content.Load<Texture2D>("ExitChosen"), new Rectangle(200,250, 400, 200))
                );
            textbox = new GameButton(Content.Load<Texture2D>("again"), 
                Content.Load<Texture2D>("again"), new Rectangle(ScreenWidth / 2 - 350, 20, 700, 200)); 
            restartMenu = new GameMenu(
                new GameButton(Content.Load<Texture2D>("yes"),
                    Content.Load<Texture2D>("yesChosen"),new Rectangle(ScreenWidth/2-200,220,400,150)),
            new GameButton(Content.Load<Texture2D>("no"),
                Content.Load<Texture2D>("no_focused"),new Rectangle(ScreenWidth/2-200,370,400,150))
            );

            background.Add(new Background(Content.Load<Texture2D>("Garden3"), new Rectangle(0, 0, 1080, 620)));
            background.Add(new Background(Content.Load<Texture2D>("Holl2"), new Rectangle(0, 0, 1080, 620)));
            background.Add(new Background(Content.Load<Texture2D>("Library2"), new Rectangle(0, 0, 1080, 620)));
            background.Add(new Background(Content.Load<Texture2D>("Dungeon"), new Rectangle(0, 0, 1080, 620)));            

            door.Add(new Door(Content.Load<Texture2D>("Door1"), new Rectangle(997, 520, 80, 100)));
            door.Add(new Door(Content.Load<Texture2D>("Door2"), new Rectangle(1005, 265, 70, 100)));
            door.Add(new Door(Content.Load<Texture2D>("Door2"), new Rectangle(1005, 265, 70, 100)));

            heroSpeed = 7;
            hero = new Hero(Content.Load<Texture2D>("Knight"),
                new Rectangle(0, 265, 55, 55), heroSpeed, ScreenWidth, ScrenHeight,300);
            princess = new Princess(Content.Load<Texture2D>("princess"), new Rectangle(600, 400, 55, 55));

            lifeBar = new LifeBar(Content.Load<Texture2D>("Life"), new Rectangle(88, 569, 125, 28),hero.getHp());
            lifeFrame = new Background(Content.Load<Texture2D>("LifeFrame"), new Rectangle(-34, 380, 300, 250));

            for (int i = 0; i < 10; i++) {
                if (i < 5) {
                    gardenEnemy.Add(new Enemy(Content.Load<Texture2D>("Angel"), 
                        new Rectangle(i * 225, 10, 30, 55), ScreenWidth, ScrenHeight, Random(), Random()));
                    hallEnemy.Add(new Enemy(Content.Load<Texture2D>("Armor"),
                        new Rectangle(i * 225, 10, 30, 55), ScreenWidth, ScrenHeight, Random(), Random()));
                    libraryEnemy.Add(new Enemy(Content.Load<Texture2D>("Book"), 
                        new Rectangle(i * 225, 10, 55, 55), ScreenWidth, ScrenHeight, Random(), Random()));
                } else {
                    gardenEnemy.Add(new Enemy(Content.Load<Texture2D>("Angel"), 
                        new Rectangle((i - 5) * 225, 550, 30, 55), ScreenWidth, ScrenHeight, Random(), Random()));
                    hallEnemy.Add(new Enemy(Content.Load<Texture2D>("Armor"), 
                        new Rectangle((i - 5) * 225, 550, 30, 55), ScreenWidth, ScrenHeight, Random(), Random()));
                    libraryEnemy.Add(new Enemy(Content.Load<Texture2D>("Book"), 
                        new Rectangle((i - 5) * 225, 550, 55, 55), ScreenWidth, ScrenHeight, Random(), Random()));
                }
            }
        }

        protected override void UnloadContent() {
           
        }

        protected override void Update(GameTime gameTime) {
           switch(gameState){
               case GameState.MainMenu:
                   UpdateMenu(gameTime);
                   break;

               case GameState.Gameplay:
                   UpdateGameplay(gameTime);
                   break;

               case GameState.RestartMenu:
                   UpdateRestartMenu(gameTime);
                   break;

               case GameState.GamePause:
                   UpdatePaused(gameTime);
                   break;
               
           }
        }
        protected void UpdatePaused(GameTime gameTime) {
            time += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (Keyboard.GetState().IsKeyDown(Keys.Escape)) {
                while (time * 100 > 50) {
                    gameState = GameState.Gameplay;
                    time =0f;
                }
            }
        }
        protected  void UpdateMenu(GameTime gameTime){

            time += (float)gameTime.ElapsedGameTime.TotalSeconds;
            
            if (Keyboard.GetState().IsKeyDown(Keys.Down)){
                
                while (time*100  > 30) {
                    mainMenu.NextButton();
                    time = 0f;
                }
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Up)) {

                while (time * 100 > 30) {
                    mainMenu.PreviousButton();
                    time = 0f;
                }
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Enter)) {
                switch (mainMenu.focusedId) {
                    case 0:
                        gameState = GameState.Gameplay;
                        break;
                    case 1:
                        this.Exit();
                        break;
                }
            }

        }

        protected  void UpdateRestartMenu(GameTime gameTime){

            time += (float)gameTime.ElapsedGameTime.TotalSeconds;
            
            if (Keyboard.GetState().IsKeyDown(Keys.Down)){
                
                while (time*100  > 30) {
                    restartMenu.NextButton();
                    time = 0f;
                }
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Up)) {

                while (time * 100 > 30) {
                    restartMenu.PreviousButton();
                    time = 0f;
                }
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Enter)) {
                switch (restartMenu.focusedId) {
                    case 0:
                        gameState = GameState.MainMenu;
                        this.EndRun();
                        this.EndDraw();
                        this.UnloadContent();
                        this.Initialize();
                        //this.LoadContent();
                        this.BeginRun();
                        this.BeginDraw();
                        break;
                    case 1:
                        this.Exit();
                        break;
                }
            }

        }

        protected  void UpdateGameplay(GameTime gameTime)
        {
            
            time += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (Keyboard.GetState().IsKeyDown(Keys.Escape)) {
                while (time * 100 > 50) {
                    gameState = GameState.GamePause;
                    time = 0f;
                }
            }
            hero.Update(gameTime);
            princess.Update(gameTime);

            switch (CurrentScreen)
            {
                /////////////////////////////////////////////////////////////////////////////
                case GameEnum.Garden:
                    //начало движения врагов
                    if (StartMov())
                        foreach (Enemy element in gardenEnemy)
                        {
                            element.Update(gameTime);
                        }
                    //Столкновение игровых объектов
                    foreach (Enemy element in gardenEnemy)
                    {
                        if (hero.IntersectionCheck(hero, element))
                        {
                            soundEffect.Play();
                            hero.OnDamage(enemyDamage);
                            Console.WriteLine(hero.getHp());
                            lifeBar.Update(hero.getHp());
                            hero.StartPlace();
                        }
                    }
                    //переход на следующую локацию
                    if (door[0].IntersectionCheck(hero, door[0]))
                    {
                        CurrentScreen = GameEnum.CastleHoll;
                        temp = false;
                        hero.StartPlace();

                    }
                    break;
                ////////////////////////////////////////////////////////////////
                case GameEnum.CastleHoll:

                    if (StartMov())
                        foreach (Enemy element in hallEnemy)
                        {
                            element.Update(gameTime);
                        }
                    //Столкновение игровых объектов
                    foreach (Enemy element in hallEnemy)
                    {
                        if (hero.IntersectionCheck(hero, element))
                        {
                            soundEffect.Play();
                            hero.OnDamage(enemyDamage);
                            lifeBar.Update(hero.getHp());
                            hero.StartPlace();
                        }
                    }
                    if (door[1].IntersectionCheck(hero, door[1]))
                    {
                        CurrentScreen = GameEnum.Library;
                        temp = false;
                        hero.StartPlace();
                    }
                    break;
                //////////////////////////////////////////////////////////////////////////////
                case GameEnum.Library:

                    if (StartMov())
                        foreach (Enemy element in libraryEnemy)
                        {
                            element.Update(gameTime);
                        }
                    //Столкновение игровых объектов
                    foreach (Enemy element in libraryEnemy)
                    {
                        if (hero.IntersectionCheck(hero, element))
                        {
                            soundEffect.Play();
                            hero.OnDamage(enemyDamage);
                            lifeBar.Update(hero.getHp());
                            hero.StartPlace();
                        }
                    }
                    if (door[1].IntersectionCheck(hero, door[1]))
                    {
                        CurrentScreen = GameEnum.Dungeon;
                        temp = false;
                        hero.StartPlace();
                    }
                    break;
                /////////////////////////////////////////////////////////////////////////
                case GameEnum.Dungeon:
                    if (princess.getRectangle().Intersects(hero.getRectangle()))
                    {
                        gameState = GameState.RestartMenu;
                    }
                    break;
            }
            if (hero.getHp() <= 0) gameState = GameState.RestartMenu;
            base.Update(gameTime);
        }


        protected override void Draw(GameTime gameTime) {
            
            switch (gameState){
                case GameState.Gameplay:
                    DrawGame(gameTime);
                    break;
                case GameState.MainMenu:
                    DrawMenu(gameTime);
                    break;
                    case GameState.RestartMenu:
                    DrawRestartMenu(gameTime);
                    break;
                    case GameState.GamePause:
                    DrawPausedGame(gameTime);
                    break;
            }

        }
        protected void DrawPausedGame(GameTime gameTime) {
            spriteBatch.Begin();
            spriteBatch.End();
        }
         protected  void DrawMenu(GameTime gameTime) {
             GraphicsDevice.Clear(Color.Silver);
             spriteBatch.Begin();
             mainMenu.Draw(spriteBatch);
             spriteBatch.End();

         }

        protected  void DrawRestartMenu(GameTime gameTime) {
             GraphicsDevice.Clear(Color.Silver);
             spriteBatch.Begin();
             textbox.Draw(spriteBatch);
             restartMenu.Draw(spriteBatch);
             spriteBatch.End();

         }

         protected void DrawGame(GameTime gameTime) {
            GraphicsDevice.Clear(Color.White);
            spriteBatch.Begin();
            switch (CurrentScreen) {
                case GameEnum.Garden:
                   
                    background[0].Draw(spriteBatch);
                    
                    door[0].Draw(spriteBatch);
                    foreach (Enemy element in gardenEnemy)
                        element.Draw(spriteBatch);
                    
                    break;

                case GameEnum.CastleHoll:
                    background[1].Draw(spriteBatch);
                    door[1].Draw(spriteBatch);
                    foreach (Enemy element in hallEnemy)
                        element.Draw(spriteBatch);
                    
                    break;


                case GameEnum.Library:
                    
                    background[2].Draw(spriteBatch);
                    door[2].Draw(spriteBatch);
                    foreach (Enemy element in libraryEnemy)
                        element.Draw(spriteBatch);
                    
                    break;


                case GameEnum.Dungeon:
                    
                    background[3].Draw(spriteBatch);
                    princess.Draw(spriteBatch);
                    
                    break;
            }

            
            hero.Draw(spriteBatch);
            lifeBar.Draw(spriteBatch);
            lifeFrame.Draw(spriteBatch);
            spriteBatch.End();


            base.Draw(gameTime);
        }
    }
    
}
