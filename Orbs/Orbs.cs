using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


using Orbs.Tiled;
using Orbs.LootSystem;
using Orbs.Interface;
using Orbs.World;
using Orbs.Audio;
using Orbs.QuestSystem;
using System;

using Orbs.Interface.TextInput;
using Orbs.Effects;
using System.Globalization;
using System.Threading;
using System.Collections.Generic;

namespace Orbs
{
    public enum GameState { Menu, Playing, Paused, Dead, Settings, NewGame };

    public class Orbs : Game
    {
        public static GraphicsDeviceManager graphics;
        public static ContentManager content;
        public static SpriteBatch spriteBatch;
        public static SpriteFont spriteFontDefault;
        public static SpriteFont spriteFontHandwriting;
        public static SpriteFont spriteFontBig;
        public static SpriteFont spriteFontMenu;
        public static SoundManager soundEffects;
        public static MusicManager musicManager;
        public static GameState curGameState = GameState.Menu;
        public static Texture2D mouseCursorDefault;

        public static TileMap curMap;
        public static string curLevel;

        private Player player;
        public static Camera2D camera;
        private MouseState mouseState;
        private MouseState oldMouseState;
        private InputHandler inputHandler;

        private const string SOURCE_CHARANIMATIONS = @"Animations\Characters\";
        private const int LAYER_BACKGROUND = 0;
        private const int LAYER_FRONTLAYER = 1;
        private const int LAYER_TOPLAYER = 2;
        private const int LAYER_DETAILLAYER = 3;
        private Vector2 cameraCenter;
        private Vector2 mouseWorldPos;
        private Vector2 mouseWindowPos;

        public static Texture2D pixel;
        public static int selectedInventorySlot = -1;
        public static string selectedCharacterSlot = "";
        public static TimeSpan currentTimeSpan;

        public static bool respawnPlayer = false;
        private Vector2 playerMovement = Vector2.Zero;
        private KeyboardState oldKeyState;

        //demo stuff
        public static Random rand = new Random();
        private TextBox consoleBox;
        private Rectangle consoleBoxPosition = new Rectangle(100, 100, 500, 100);

        Matrix viewMatrix;
        private Vector2 shakeOffset = Vector2.Zero;
        private float shakeRadius = 30.0f;
        private int shakeAngle = rand.Next() % 360;
        public static bool shakeViewport = false;

        private RenderTarget2D renderTarget;
        private RenderTarget2D screenshot;
        Microsoft.Xna.Framework.Graphics.Effect grayScaleEffect;
        Microsoft.Xna.Framework.Graphics.Effect blurEffect;
        Microsoft.Xna.Framework.Graphics.Effect sepiaEffect;

        Helper.FPSCounter fpsCounter = new Helper.FPSCounter();

        //Animations.SpineObject tree1;
        //Animations.SpineObject tree2;

        //List<Animations.SpineObject> trees = new List<Animations.SpineObject>();

        public Orbs()
        {
            graphics = new GraphicsDeviceManager(this);

            //Fullscreen
            //graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            //graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            //graphics.IsFullScreen = true;

            //Windowed
            graphics.PreferredBackBufferWidth = 1920;
            graphics.PreferredBackBufferHeight = 1080;
            graphics.IsFullScreen = false;

            TargetElapsedTime = TimeSpan.FromSeconds(1d / 60d);
            IsFixedTimeStep = true;
            graphics.GraphicsProfile = GraphicsProfile.HiDef;
            graphics.ApplyChanges();

            Content.RootDirectory = "Content";
            content = Content;

            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("en-US");
        }

        protected override void Initialize()
        {
            IsMouseVisible = true;
            camera = new Camera2D(GraphicsDevice.Viewport);
            LootTable.ReadLootTable();
            EnemyTable.ReadEnemyTable();

            soundEffects = new SoundManager();
            musicManager = new MusicManager();

            //For Debug
            pixel = new Texture2D(GraphicsDevice, 1, 1);
            pixel.SetData(new[] { Color.White });

            inputHandler = new InputHandler();
            KeyboardInput.Initialize(this, 500f, 20);

            EffectManager.lightning = new Lightning(this);
            Components.Add(EffectManager.lightning.effectComponent);

            PresentationParameters pp = graphics.GraphicsDevice.PresentationParameters;
            renderTarget = new RenderTarget2D(graphics.GraphicsDevice, graphics.PreferredBackBufferWidth, 
                graphics.PreferredBackBufferHeight, false, SurfaceFormat.Color, DepthFormat.None, pp.MultiSampleCount, RenderTargetUsage.PreserveContents);
            screenshot = new RenderTarget2D(graphics.GraphicsDevice, graphics.PreferredBackBufferWidth, 
                graphics.PreferredBackBufferHeight, false, SurfaceFormat.Color, DepthFormat.None, pp.MultiSampleCount, RenderTargetUsage.DiscardContents);

            

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            spriteFontDefault = Content.Load<SpriteFont>("Fonts/FallingSkySeBd");
            spriteFontBig = Content.Load<SpriteFont>("Fonts/FallingSkySeBd_big");
            spriteFontHandwriting = Content.Load<SpriteFont>("Fonts/ManicMonday");
            spriteFontMenu = Content.Load<SpriteFont>("Fonts/Aladin");

            UIManager.GenerateUI();

            musicManager.ChangeBackgroundMusic("menu");
            musicManager.PlayBackgroundMusic();

            mouseCursorDefault = content.Load<Texture2D>(@"UserInterface\Icons\cursor_default");
            Mouse.SetCursor(MouseCursor.FromTexture2D(mouseCursorDefault, 0, 0));

            //CONSOLE TEXTBOX
            consoleBox = new TextBox(consoleBoxPosition, 200, "", GraphicsDevice, spriteFontDefault, Color.LightGray, Color.LightBlue, 30);

            float margin = 5;
            consoleBox.Area = new Rectangle((int)(consoleBoxPosition.X + margin), consoleBoxPosition.Y, (int)(consoleBoxPosition.Width - margin),
                consoleBoxPosition.Height);
            consoleBox.Renderer.Color = Color.White;
            consoleBox.IsActive = false;

            grayScaleEffect = Content.Load<Microsoft.Xna.Framework.Graphics.Effect>("Effects/GrayScaleShader");
            blurEffect = Content.Load<Microsoft.Xna.Framework.Graphics.Effect>("Effects/BlurShader");
            sepiaEffect = Content.Load<Microsoft.Xna.Framework.Graphics.Effect>("Effects/SepiaShader");

            //tree1 = new Animations.SpineObject("Objects/tree1/tree1", new Vector2(200, 2000));
            //tree2 = new Animations.SpineObject("Objects/tree2/tree2", new Vector2(800, 2000));

            //for(int i=0; i < 100; i++)
            //{
            //    trees.Add(new Animations.SpineObject("Objects/tree1/tree1", new Vector2(200 + i * 20, 2000)));
            //}
        }

        protected override void UnloadContent()
        {
            //Unload any non ContentManager content here
        }

        protected override void Update(GameTime gameTime)
        {
            mouseState = Mouse.GetState();
            mouseWorldPos = mouseState.Position.ToVector2() + camera.Position;
            mouseWindowPos = mouseState.Position.ToVector2();

            KeyboardInput.Update();
            KeyboardState curKeyState = Keyboard.GetState();

            //TOGGLE CONSOLE WINDOW
            if (inputHandler.gameplayButtons.Console == ButtonState.Pressed && oldKeyState.IsKeyUp(Keys.End))
            {
                consoleBox.IsActive = !consoleBox.IsActive;
                UIManager.textInputOpen = !UIManager.textInputOpen;
            }

            if (consoleBox.IsActive)
                consoleBox.Update();

            if (shakeViewport)
            {
                shakeOffset = new Vector2((float)(Math.Sin(shakeAngle) * shakeRadius), (float)(Math.Cos(shakeAngle) * shakeRadius));
                shakeRadius -= 0.25f;
                shakeAngle += (150 + rand.Next(60));

                if (shakeRadius <= 0)
                {
                    //shakeViewport = false;
                    shakeRadius = 30.0f;
                    shakeAngle = rand.Next() % 360;
                }
            }

            if (curGameState == GameState.Dead)
            {
                if (inputHandler.gameplayButtons.Escape == ButtonState.Pressed)
                {
                    curGameState = GameState.Paused;
                }
            }

            #region GameState.Menu
            if (curGameState == GameState.Menu || curGameState == GameState.NewGame)
            {
                if (mouseState.LeftButton == ButtonState.Pressed && oldMouseState.LeftButton == ButtonState.Released)
                {
                    string actionToBeTriggered = inputHandler.HandleLeftMouseClick(mouseWorldPos, mouseWindowPos);

                    if (actionToBeTriggered == "quit")
                    {
                        Exit();
                    }

                    if (actionToBeTriggered == "load")
                    {
                        //TODO Load
                    }

                    if (actionToBeTriggered == "settings")
                    {
                        curGameState = GameState.Settings;
                    }

                    if (actionToBeTriggered == "newGame")
                        curGameState = GameState.NewGame;

                    if (actionToBeTriggered == "selectMale")
                        UIManager.selectedHero = "male";

                    if (actionToBeTriggered == "selectFemale")
                        UIManager.selectedHero = "female";

                    if (actionToBeTriggered == "newGameMale")
                        StartNewGame(new string[] {"player_male", "hollowrock"});

                    if (actionToBeTriggered == "newGameFemale")
                        StartNewGame(new string[] { "player_male", "abandoned" });
                }

                oldMouseState = mouseState;
            }
            #endregion

            #region Settings
            if (curGameState == GameState.Settings)
            {
                if (mouseState.LeftButton == ButtonState.Pressed && oldMouseState.LeftButton == ButtonState.Released)
                {
                    string actionToBeTriggered = inputHandler.HandleLeftMouseClick(mouseWorldPos, mouseWindowPos);

                    if (actionToBeTriggered == "toggleLanguage")
                    {
                        ToggleLanguage();
                    }

                    if (actionToBeTriggered == "toggleMusic")
                    {
                        musicManager.ToggleMusic();
                    }

                    if (actionToBeTriggered == "toggleSound")
                    {
                        soundEffects.ToggleSound();
                    }

                    if (actionToBeTriggered == "back")
                    {
                        if (player != null)
                            curGameState = GameState.Paused;
                        else
                            curGameState = GameState.Menu;
                    }
                }
                oldMouseState = mouseState;
            }
            #endregion

            #region GameState.Paused
            if (curGameState == GameState.Paused)
            {
                if (inputHandler.gameplayButtons.Escape == ButtonState.Pressed && oldKeyState.IsKeyUp(Keys.Escape))
                {
                    curGameState = GameState.Playing;
                    oldKeyState = curKeyState;
                }


                if (mouseState.LeftButton == ButtonState.Pressed && oldMouseState.LeftButton == ButtonState.Released)
                {
                    string actionToBeTriggered = inputHandler.HandleLeftMouseClick(mouseWorldPos, mouseWindowPos);

                    if (actionToBeTriggered == "quit")
                        Exit();

                    if (actionToBeTriggered == "resume")
                        curGameState = GameState.Playing;

                    if (actionToBeTriggered == "newGame")
                        curGameState = GameState.NewGame;

                    if (actionToBeTriggered == "load")
                    {
                        //Load
                    }

                    if (actionToBeTriggered == "save")
                    {
                        //Save
                    }

                    if (actionToBeTriggered == "settings")
                        curGameState = GameState.Settings;
                }

                oldKeyState = curKeyState;
                oldMouseState = mouseState;
            }
            #endregion

            #region GameState.Playing
            if (curGameState == GameState.Playing)
            {
                string actionToBeTriggered = "";

                if (inputHandler.gameplayButtons.Escape == ButtonState.Pressed && oldKeyState.IsKeyUp(Keys.Escape))
                {
                    //TakeScreenshot();
                    inputHandler.HandleEscapePress();
                }
                if (mouseState.RightButton == ButtonState.Pressed && oldMouseState.RightButton == ButtonState.Released)
                {
                    inputHandler.HandleItemUse(mouseState.Position.ToVector2());
                    actionToBeTriggered = inputHandler.HandleRightMouseClick(mouseWorldPos, mouseWindowPos);
                }

                //Check if player clicked anything that triggers something
                if (mouseState.LeftButton == ButtonState.Pressed && oldMouseState.LeftButton == ButtonState.Released)
                    actionToBeTriggered = inputHandler.HandleLeftMouseClick(mouseWorldPos, mouseWindowPos);

                //If nothing is triggered by click, check for keystroke if anything is pressed
                if (string.IsNullOrEmpty(actionToBeTriggered) && curKeyState.GetPressedKeys().Length > 0)
                    actionToBeTriggered = inputHandler.HandleKeyStroke(curKeyState, oldKeyState);

                //If any action has been triggered, do it
                if (!string.IsNullOrEmpty(actionToBeTriggered))
                {
                    if (actionToBeTriggered.StartsWith("w"))
                        player.StartAttack(actionToBeTriggered);

                    if (actionToBeTriggered.StartsWith("c"))
                        player.ConsumeItem(actionToBeTriggered);

                    if (actionToBeTriggered.StartsWith("s"))
                        player.CastSpell(actionToBeTriggered);

                    if (actionToBeTriggered.StartsWith("r"))
                        player.CastGlyph(actionToBeTriggered);

                }

                if (mouseState.LeftButton == ButtonState.Pressed)
                    inputHandler.HandleDragAndDrop(mouseState.Position.ToVector2(), true);

                if (mouseState.LeftButton == ButtonState.Released && selectedInventorySlot >= 0 ||
                    mouseState.LeftButton == ButtonState.Released && selectedCharacterSlot != "")
                    inputHandler.HandleDragAndDrop(mouseState.Position.ToVector2(), false);

                if (respawnPlayer)
                {
                    player.SpawnAt(curMap.playerSpawnPosition);
                    respawnPlayer = false;
                }

                UIManager.Update(gameTime);
                UIManager.UpdateCombatText(gameTime);

                if (Player.isAlive && !consoleBox.IsActive)
                {
                    playerMovement = inputHandler.movementButtons.PlayerMove();
                    player.UpdatePlayer(gameTime, playerMovement);
                    camera.Update(player.X, player.Y);

                    if (inputHandler.gameplayButtons.Attack == ButtonState.Pressed && !Player.isAttacking)
                        player.StartAttack("attack_Melee"); //

                    //TEST STUFF
                    if (inputHandler.gameplayButtons.SelfDMG == ButtonState.Pressed)
                        player.DamagePlayer(rand.Next(4, 14));

                    if (Player.currentGlyph != null)
                        Player.currentGlyph.Update(gameTime, mouseWindowPos, mouseWorldPos);
                }

                WorldCharacters.UpdateWorldCharacters(gameTime);
                WorldObjects.UpdateWorldObjects(gameTime);
                QuestManager.Update(gameTime);
                curMap.UpdateAnimatedTiles(gameTime);
                currentTimeSpan = gameTime.TotalGameTime;

                //lightning.Update(player.Position, player.Width, player.Height);              

                EffectManager.UpdateEffects(gameTime);

                fpsCounter.Update(gameTime);
                base.Update(gameTime);
                oldMouseState = mouseState;
                oldKeyState = curKeyState;
            }
            #endregion

            inputHandler.HandleMouseOver(mouseWorldPos, mouseWindowPos);
        }

        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.SetRenderTarget(renderTarget);

            GraphicsDevice.Clear(Color.Black);

            if (viewMatrix != null)
            {
                viewMatrix = camera.GetViewMatrix();
            }

            if (curGameState == GameState.Menu || curGameState == GameState.NewGame)
            {
                spriteBatch.Begin();
                UIManager.DrawStartMenu(spriteBatch);
                spriteBatch.End();
            }

            else if (curGameState == GameState.Settings)
            {
                if (player != null)
                {
                    spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
                    grayScaleEffect.CurrentTechnique.Passes[0].Apply();
                    spriteBatch.Draw(screenshot, new Vector2(0, 0), Color.White);
                    spriteBatch.End();
                }

                spriteBatch.Begin();
                UIManager.DrawSettingsWindow(spriteBatch, player == null ? false : true);
                spriteBatch.End();
            }

            else if (curGameState == GameState.Paused)
            {
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

                //blurEffect.CurrentTechnique.Passes[0].Apply();
                //blurEffect.Parameters["weights"].SetValue(Helper.EffectConstants.BLUR_WEIGHTS);
                //blurEffect.Parameters["offsets"].SetValue(Helper.EffectConstants.BLUR_OFFSETS);

                grayScaleEffect.CurrentTechnique.Passes[0].Apply();
                //sepiaEffect.CurrentTechnique.Passes[0].Apply();

                spriteBatch.Draw(screenshot, new Vector2(0, 0), Color.White);
                spriteBatch.End();
                spriteBatch.Begin();
                UIManager.DrawPauseMenu(spriteBatch);
                spriteBatch.End();
            }

            else if (curGameState == GameState.Dead)
            {
                
                //Map and Characters
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, transformMatrix: viewMatrix);

                curMap.DrawLayer(LAYER_BACKGROUND);
                curMap.DrawLayer(LAYER_FRONTLAYER);
                WorldCharacters.DrawWorldCharacters();
                WorldObjects.DrawEnemyDrops(spriteBatch);
                WorldObjects.DrawAnimatedObjects(spriteBatch);
                //spriteBatch.Draw(player.deadPlayer, player.Position, Color.White);

                curMap.DrawLayer(LAYER_TOPLAYER);
                curMap.DrawLayer(LAYER_DETAILLAYER);

                //WorldObjects.DrawObjectStatus(); //debug purpose
                spriteBatch.End();

                //Userinterface
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);
                UIManager.DrawUI(spriteBatch, mouseState.Position.ToVector2());
                UIManager.DrawUIElements(spriteBatch);

                spriteBatch.DrawString(
                    spriteFontBig, Localization.strings.player_dead,
                    new Vector2(graphics.PreferredBackBufferWidth / 2 - spriteFontBig.MeasureString(Localization.strings.player_dead).X / 2, graphics.PreferredBackBufferHeight / 2),
                    Color.Red);
                spriteBatch.End();

                //Position Data for debug purpose
                spriteBatch.Begin();
                spriteBatch.DrawString(spriteFontDefault, "PlayerPos: " + player.Position, new Vector2(10, 10), Color.Black);
                spriteBatch.DrawString(spriteFontDefault, "CameraPos: " + camera.Position, new Vector2(10, 25), Color.Black);
                spriteBatch.DrawString(spriteFontDefault, "MousePos: " + mouseState.Position, new Vector2(10, 40), Color.Black);
                spriteBatch.DrawString(spriteFontDefault, "TilePos: " + curMap.GetTilePosition(mouseState.Position, camera.Position), new Vector2(10, 55), Color.Black);

                spriteBatch.End();
            }

            else if(curGameState != GameState.Menu && player != null)
            {
                //Map and Characters
                EffectManager.lightning.BeginDraw();
                if (shakeViewport)
                    spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, null, null, null,
                        Matrix.CreateTranslation(-camera.Position.X + shakeOffset.X, -camera.Position.Y + shakeOffset.Y, 0));
                else
                    spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.None, null, null, viewMatrix);

                curMap.DrawLayer(LAYER_BACKGROUND);
                curMap.DrawLayer(LAYER_FRONTLAYER);

                

                EffectManager.DrawGroundEffects(spriteBatch);
                WorldObjects.DrawPlacedGlyphes();
                WorldCharacters.DrawWorldCharacters();
                WorldObjects.DrawEnemyDrops(spriteBatch);
                WorldObjects.DrawAnimatedObjects(spriteBatch);
                WorldObjects.DrawObjects(spriteBatch);
                UIManager.DrawCombatText(spriteBatch);

                Player.playerPet.Draw(spriteBatch);

                spriteBatch.End();

                //if (!Player.isAttacking && !Player.isCasting)
                //    player.Draw(spriteBatch, null);

                //player.DrawAttack(spriteBatch);

                //if (Player.playerDecoy != null)
                //    Player.playerDecoy.Draw(spriteBatch, Color.Gray);

                if (!Player.isAttacking && !Player.isCasting)
                    player.DrawPlayer();

                //PositionRectangle of Player, for debug
                //int bw = 2; // Border width
                //spriteBatch.Draw(pixel, new Rectangle(Player.PlayerPositionRectangle.Left, Player.PlayerPositionRectangle.Top, bw, Player.PlayerPositionRectangle.Height), Color.Black); // Left
                //spriteBatch.Draw(pixel, new Rectangle(Player.PlayerPositionRectangle.Right, Player.PlayerPositionRectangle.Top, bw, Player.PlayerPositionRectangle.Height), Color.Black); // Right
                //spriteBatch.Draw(pixel, new Rectangle(Player.PlayerPositionRectangle.Left, Player.PlayerPositionRectangle.Top, Player.PlayerPositionRectangle.Width, bw), Color.Black); // Top
                //spriteBatch.Draw(pixel, new Rectangle(Player.PlayerPositionRectangle.Left, Player.PlayerPositionRectangle.Bottom, Player.PlayerPositionRectangle.Width, bw), Color.Black); // Bottom


                if (shakeViewport)
                    spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, null, null, null,
                        Matrix.CreateTranslation(-camera.Position.X + shakeOffset.X, -camera.Position.Y + shakeOffset.Y, 0));
                else
                    spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.None, null, null, viewMatrix);

                player.DrawAttack(spriteBatch);

                curMap.DrawLayer(LAYER_TOPLAYER);
                curMap.DrawLayer(LAYER_DETAILLAYER);
                //spriteBatch.Draw(pixel, new Rectangle((int)tree2.skeleton.X - (int)tree2.skeleton.Data.Width / 2, (int)tree2.skeleton.Y, (int)tree2.skeleton.Data.Width, (int)tree2.skeleton.Data.Height / 3), Color.Black);

                //WorldObjects.DrawObjectStatus(); //debug purpose
                spriteBatch.End();
                EffectManager.lightning.Draw(gameTime);

                //tree1.Draw();
                
                //tree2.Draw();
                //foreach (Animations.SpineObject sp in trees)
                //    sp.Draw();

                spriteBatch.Begin(SpriteSortMode.Texture, BlendState.Additive, SamplerState.PointClamp, null, null, null, viewMatrix);
                EffectManager.DrawEffects(spriteBatch);
                spriteBatch.End();

                //Userinterface
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);

               

                if (Player.currentGlyph != null)
                    Player.currentGlyph.Draw(spriteBatch);

                UIManager.DrawUI(spriteBatch, mouseState.Position.ToVector2());
                UIManager.DrawUIElements(spriteBatch);

                if (consoleBox.IsActive)
                {
                    consoleBox.Draw(spriteBatch);
                }

                spriteBatch.End();

                //Position Data for debug purpose
                spriteBatch.Begin();
                spriteBatch.DrawString(spriteFontDefault, "PlayerPos: " + player.Position, new Vector2(10, 10), Color.White);
                spriteBatch.DrawString(spriteFontDefault, "CameraPos: " + camera.Position, new Vector2(10, 25), Color.White);
                spriteBatch.DrawString(spriteFontDefault, "MousePos: " + mouseState.Position, new Vector2(10, 40), Color.White);
                spriteBatch.DrawString(spriteFontDefault, "TilePos: " + curMap.GetTilePosition(mouseState.Position, camera.Position), new Vector2(10, 55), Color.White);
                fpsCounter.DrawFps(spriteBatch, spriteFontDefault, new Vector2(10, 70), Color.White);
                spriteBatch.End();

                graphics.GraphicsDevice.SetRenderTarget(screenshot);
                spriteBatch.Begin();
                spriteBatch.Draw(renderTarget, Vector2.Zero, Color.White);
                spriteBatch.End();

                
            } // End Of Draw Playing

            graphics.GraphicsDevice.SetRenderTarget(null);
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque);
            spriteBatch.Draw(renderTarget, Vector2.Zero, Color.White); //this works fine
            spriteBatch.End();
        }

        public void ToggleLanguage()
        {
            if (Thread.CurrentThread.CurrentUICulture == CultureInfo.GetCultureInfo("en-US"))
            {
                Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("de-DE");
            }
            else
            {
                Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("en-US");
            }

            UIManager.GenerateStartMenuButtons();
            UIManager.GenerateSettingsMenuButtons();
            UIManager.GeneratePauseMenuButtons();
        }

        /// <summary>
        /// Starts new Game with given args
        /// </summary>
        /// <param name="args">0 = Male/Female, 1 = Map</param>
        private void StartNewGame(string[] args)
        {
            ClearGameData();

            curMap = MapLoader.loadMap(args[1]);
            QuestManager.InitializeLevel();

            cameraCenter = new Vector2(graphics.PreferredBackBufferWidth / 2, graphics.PreferredBackBufferHeight / 2);
            camera.worldWidth = curMap.worldWidth;
            camera.worldHeight = curMap.worldHeight;

            player = new Player(Content.Load<Texture2D>(SOURCE_CHARANIMATIONS + args[0]), curMap.playerSpawnPosition);
            player.LoadEmptyInventoryGrid();
            player.LoadInventoryItems();

            musicManager.ChangeBackgroundMusic(curMap.GetBackgroundMusicFromMap());
            curGameState = GameState.Playing;
        }

        private static void ClearGameData()
        {
            WorldCharacters.enemyWorldCharacters.Clear();
            WorldCharacters.standardWorldCharacters.Clear();

            WorldObjects.animatedObjects.Clear();
            WorldObjects.enemyDrops.Clear();
            WorldObjects.interactionObjects.Clear();
            WorldObjects.lootableContainers.Clear();
            WorldObjects.placedGlyphs.Clear();
            WorldObjects.transits.Clear();

            EffectManager.ClearLights();

            TileMap.collisionObjects.Clear();
            TileMap.lootableObjects.Clear();

            UIManager.combatTexts.Clear();
            UIManager.enemyNameplates.Clear();
        }
    }
}
