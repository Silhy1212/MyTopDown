using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MyTopDown;

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
        cannonRotation = (float)Math.Atan2(direction.Y, direction.X) - Single.Pi/2;

        
        if (mouseState.LeftButton == ButtonState.Pressed && timeSinceLastShot >= shootCooldown)
        {
            Vector2 projectileDirection = Vector2.Normalize(direction); 
            projectiles.Add(new Projectile(spritePosition, projectileDirection, _projectileTexture));
            timeSinceLastShot = 0f;
        }

       
        foreach (var projectile in projectiles)
        {
            projectile.Update();
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

        _spriteBatch.End();
        base.Draw(gameTime);
    }
}

public class Projectile
{
    public Vector2 Position;
    public Vector2 Direction;
    public float Speed = 5f;
    public Texture2D Texture;
    public float Rotation;

    public Projectile(Vector2 position, Vector2 direction, Texture2D texture)
    {
        Position = position;
        Direction = direction;
        Texture = texture;
        Rotation = (float)Math.Atan2(direction.Y, direction.X);
    }

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


