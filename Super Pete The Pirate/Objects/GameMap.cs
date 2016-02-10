﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Maps.Tiled;

namespace Super_Pete_The_Pirate
{
    public sealed class GameMap
    {
        //--------------------------------------------------
        // Singleton

        private static GameMap _instance = null;
        private static readonly object _padlock = new object();

        //--------------------------------------------------
        // Colliders

        private List<Rectangle> _tileColliderBoxes;
        private Texture2D _colliderTexture;

        //--------------------------------------------------
        // Encapsulaments

        public int MapWidth { get { return _tiledMap == null ? 0 : _tiledMap.WidthInPixels; } }
        public int MapHeight { get { return _tiledMap == null ? 0 : _tiledMap.HeightInPixels; } }

        //--------------------------------------------------
        // Tiles stuff

        private TiledMap _tiledMap;
        public Vector2 TileSize = new Vector2(32, 32);

        public enum TileCollision
        {
            Passable = 0,
            Block = 1,
            Platform = 2
        }

        //----------------------//------------------------//

        public static GameMap Instance
        {
            get
            {
                lock (_padlock)
                {
                    if (_instance == null)
                        _instance = new GameMap();
                    return _instance;
                }
            }
        }

        private GameMap()
        {
            _tileColliderBoxes = new List<Rectangle>();
            _colliderTexture = new Texture2D(SceneManager.Instance.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            _colliderTexture.SetData<Color>(new Color[] { Color.Red });
        }

        public void LoadMap(ContentManager contentManager, int id)
        {
            _tiledMap = contentManager.Load<TiledMap>("maps/map1");
            var blockedLayer = (TiledTileLayer)_tiledMap.GetLayer("Block");
            foreach (var tile in blockedLayer.Tiles)
            {
                if (tile.Id != 0)
                {
                    _tileColliderBoxes.Add(new Rectangle(tile.X * (int)TileSize.X, tile.Y * (int)TileSize.Y, (int)TileSize.X, (int)TileSize.Y));
                }
            }
        }

        public int GetTileByX(double x)
        {
            return (int)(x / TileSize.X);
        }

        public int GetTileByY(double y)
        {
            return (int)(y / TileSize.Y);
        }

        public TiledTileLayer GetBlockLayer()
        {
            return _tiledMap.GetLayer<TiledTileLayer>("Block");
        }

        public TiledTileLayer GetPlatformLayer()
        {
            return _tiledMap.GetLayer<TiledTileLayer>("Batata");
        }

        public bool IsTileBlocked(int x, int y)
        {
            var blockLayer = GetBlockLayer();
            return blockLayer.GetTile(x, y).Id != 0;
        }

        public bool IsTilePlatform(int x, int y)
        {
            var platformLayer = GetPlatformLayer();
            if (platformLayer == null)
                return false;
            return platformLayer.GetTile(x, y).Id != 0;
        }

        public TileCollision GetCollision(int x, int y)
        {
            if (x < 0 || x >= _tiledMap.Width || IsTileBlocked(x, y))
                return TileCollision.Block;
            if (IsTilePlatform(x, y))
                return TileCollision.Platform;
            return TileCollision.Passable;
        }

        public Rectangle GetTileBounds(int x, int y)
        {
            return new Rectangle(x * (int)TileSize.X, y * (int)TileSize.Y, (int)TileSize.X, (int)TileSize.Y);
        }

        private void DrawTileColliders(SpriteBatch spriteBatch)
        {
            foreach (var collider in _tileColliderBoxes)
            {
                spriteBatch.Draw(_colliderTexture, collider, Color.White * 0.1f);
            }
        }

        public void Draw(Camera2D camera, SpriteBatch spriteBatch)
        {
            _tiledMap.Draw(camera);
            spriteBatch.Begin(transformMatrix: camera.GetViewMatrix());
            DrawTileColliders(spriteBatch);
            spriteBatch.End();
        }
    }
}