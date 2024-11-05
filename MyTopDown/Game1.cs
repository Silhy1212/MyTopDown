using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MyTopDown
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Texture2D _texture;
        private Texture2D _projectileTexture;
        private Vector2 spritePosition;
        private float rotationAngle;
        private List<Projectile> projectiles = new List<Projectile>();
        public float _playerSpeed = 3f;
        public float cannonRotation;
        private float shootCooldown = 0.5f;
        private float timeSinceLastShot = 0f;

        private EnemyTank enemyTank;  

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

           
            _texture = new Texture2D(GraphicsDevice, 1, 1);
            _texture.SetData(new Color[] { Color.White });

           
            _projectileTexture = new Texture2D(GraphicsDevice, 5, 5);
            Color[] data = new Color[5 * 5];
            for (int i = 0; i < data.Length; ++i) data[i] = Color.Red;
            _projectileTexture.SetData(data);

            spritePosition = new Vector2(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2);

           
            enemyTank = new EnemyTank(new Vector2(100, 100), 2f, _texture, _projectileTexture);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            timeSinceLastShot += (float)gameTime.ElapsedGameTime.TotalSeconds;
            KeyboardState state = Keyboard.GetState();
            if (state.IsKeyDown(Keys.D))
            {
                rotationAngle += 0.06f;
            }
            if (state.IsKeyDown(Keys.A))
            {
                rotationAngle -= 0.06f;
            }
            if (state.IsKeyDown(Keys.W))
            {
                spritePosition.X += (float)Math.Sin(rotationAngle) * _playerSpeed;
                spritePosition.Y -= (float)Math.Cos(rotationAngle) * _playerSpeed;
            }
            if (state.IsKeyDown(Keys.S))
            {
                spritePosition.X -= (float)Math.Sin(rotationAngle) * _playerSpeed;
                spritePosition.Y += (float)Math.Cos(rotationAngle) * _playerSpeed;
            }

          
            MouseState mouseState = Mouse.GetState();
            Vector2 mousePosition = new Vector2(mouseState.X, mouseState.Y);
            Vector2 direction = mousePosition - spritePosition;
            cannonRotation = (float)Math.Atan2(direction.Y, direction.X) - MathHelper.Pi / 2;

            var windowBounds = Window.ClientBounds;
            spritePosition.X = MathHelper.Clamp(spritePosition.X, 60 / 2, windowBounds.Width - 60 / 2);
            spritePosition.Y = MathHelper.Clamp(spritePosition.Y, 35 / 2, windowBounds.Height - 35 / 2);

            if (mouseState.LeftButton == ButtonState.Pressed && timeSinceLastShot >= shootCooldown)
            {
                Vector2 projectileDirection = Vector2.Normalize(direction);
                projectiles.Add(new Projectile(spritePosition, projectileDirection, _projectileTexture, "Player")); // Set owner as "Player"
                timeSinceLastShot = 0f;
            }

            foreach (var projectile in projectiles)
            {
                projectile.Update();
            }

            for (int i = 0; i < projectiles.Count; i++)
            {
                if (enemyTank != null && projectiles[i].Owner == "Player" && projectiles[i].BoundingBox.Intersects(enemyTank.BoundingBox))
                {
                    enemyTank = null;
                    break;
                }
            }

            if (enemyTank != null)
            {
                enemyTank.Update(spritePosition, projectiles, gameTime);
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();

            var tankRectangle = new Rectangle(0, 0, 35, 60);
            var cannonRectangle = new Rectangle(0, 0, 10, 40);

            _spriteBatch.Draw(_texture, spritePosition, tankRectangle, Color.White, rotationAngle,
                              new Vector2(tankRectangle.Width / 2f, tankRectangle.Height / 2f),
                              Vector2.One, SpriteEffects.None, 0);
            _spriteBatch.Draw(_texture, spritePosition, cannonRectangle, Color.Black, cannonRotation,
                              new Vector2(cannonRectangle.Width / 2f, 0),
                              Vector2.One, SpriteEffects.None, 0);

            foreach (var projectile in projectiles)
            {
                projectile.Draw(_spriteBatch);
            }

            if (enemyTank != null)
            {
                enemyTank.Draw(_spriteBatch);
            }

            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }

    public class EnemyTank
    {
        public Vector2 Position;
        public float Speed;
        public Texture2D Texture;
        public float Rotation;
        private Texture2D projectileTexture;
        private float shootCooldown = 5f; 
        private float timeSinceLastShot = 0f;

        public EnemyTank(Vector2 position, float speed, Texture2D texture, Texture2D projectileTexture)
        {
            Position = position;
            Speed = speed;
            Texture = texture;
            this.projectileTexture = projectileTexture;
        }

        public Rectangle BoundingBox => new Rectangle((int)(Position.X - 17.5f), (int)(Position.Y - 30f), 35, 60);

        public void Update(Vector2 playerPosition, List<Projectile> projectiles, GameTime gameTime)
        {
            Vector2 direction = playerPosition - Position;
            direction.Normalize();
            Position += direction * Speed;
            Rotation = (float)Math.Atan2(direction.Y, direction.X) + MathHelper.PiOver2;

            timeSinceLastShot += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (timeSinceLastShot >= shootCooldown)
            {
                ShootAtPlayer(playerPosition, projectiles);
                timeSinceLastShot = 0f; 
            }
        }

        private void ShootAtPlayer(Vector2 playerPosition, List<Projectile> projectiles)
        {
            Vector2 direction = Vector2.Normalize(playerPosition - Position);
            projectiles.Add(new Projectile(Position, direction, projectileTexture, "Enemy")); 
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            var tankRectangle = new Rectangle(0, 0, 35, 60);
            spriteBatch.Draw(Texture, Position, tankRectangle, Color.Red, Rotation,
                             new Vector2(tankRectangle.Width / 2f, tankRectangle.Height / 2f),
                             Vector2.One, SpriteEffects.None, 0);
        }
    }

    public class Projectile
    {
        public Vector2 Position;
        public Vector2 Direction;
        public float Speed = 5f;
        public Texture2D Texture;
        public float Rotation;
        public string Owner; 

        public Projectile(Vector2 position, Vector2 direction, Texture2D texture, string owner)
        {
            Position = position;
            Direction = direction;
            Texture = texture;
            Rotation = (float)Math.Atan2(direction.Y, direction.X);
            Owner = owner;
        }

        public Rectangle BoundingBox => new Rectangle((int)(Position.X - Texture.Width / 2), (int)(Position.Y - Texture.Height / 2), Texture.Width, Texture.Height);

        public void Update()
        {
            Position += Direction * Speed;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            var origin = new Vector2(Texture.Width / 2, Texture.Height / 2);
            spriteBatch.Draw(Texture, Position, null, Color.Black, Rotation, origin, 1f, SpriteEffects.None, 0f);
        }
    }
}
