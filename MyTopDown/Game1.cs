﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MyTopDown;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private Texture2D _texture;
    private Vector2 spritePosition;
    private float rotationAngle;
    public float _playerSpeed = 3f;
    
    
    
    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        // TODO: Add your initialization logic here

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        // TODO: use this.Content to load your game content here
        _texture = new Texture2D(GraphicsDevice, 1, 1);
        _texture.SetData<Color>(new Color[] { Color.White });
        
        Viewport viewport = _graphics.GraphicsDevice.Viewport;
        
        spritePosition.X = viewport.Width / 2;
        spritePosition.Y = viewport.Height / 2;
        
        spritePosition = new Vector2(60, 60);
        
        
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();
        
       
        
        KeyboardState state = Keyboard.GetState();
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
        {
            Exit();
        }

        

        if (state.IsKeyDown(Keys.D))
        {
            rotationAngle += 0.06f;
            //_tankPosition.X += _playerSpeed;
        }

        if (state.IsKeyDown(Keys.A))
        {
            rotationAngle -= 0.06f;
            //_tankPosition.X -= _playerSpeed;
        }

        if (state.IsKeyDown(Keys.W))
        {
            spritePosition.X += (float)Math.Sin(rotationAngle) * _playerSpeed;
            spritePosition.Y -= (float)Math.Cos(rotationAngle) * _playerSpeed;
            //_tankPosition.Y -= _playerSpeed;
        }

        if (state.IsKeyDown(Keys.S))
        {
            //_tankPosition.Y += _playerSpeed;
            spritePosition.X -= (float)Math.Sin(rotationAngle) * _playerSpeed;
            spritePosition.Y += (float)Math.Cos(rotationAngle) * _playerSpeed;
        }
        
       

        


       
        // TODO: Add your update logic here

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        // TODO: Add your drawing code here
        _spriteBatch.Begin();

        var rectangle = new Rectangle(0, 0, 35, 60);

       
        _spriteBatch.Draw(_texture, spritePosition, rectangle, Color.White, rotationAngle, new Vector2(rectangle.Width / 2f, rectangle.Height / 2f), Vector2.One, SpriteEffects.None, 0);        _spriteBatch.End();
        base.Draw(gameTime);
    }
}

