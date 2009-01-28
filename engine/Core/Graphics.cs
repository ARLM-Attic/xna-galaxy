/**
 * Graphics.cs
 * 
 * 2D / 3D Graphics Implementation
 * 
 * @file
 * @author
 * Copyright (C) XNA Naver cafe, 2009. All rights reserved.
 * $Id: $
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using System.Diagnostics;

namespace Galaxy.Core
{
    public class Sprite
    {
    };

    public class Graphics
    {
        private GraphicsDeviceManager grpDevMgr   = null;
        private SpriteBatch           sprBatch    = null;
        private LinkedList<Sprite>[]  planes      = null;
        private uint                  maxPlaneNum = 0;

        //
        // Summary:
        //     Gets or sets the current Framework.Graphics.GraphicsDevice.
        //
        // Returns:
        //     The current Framework.Graphics.GraphicsDevice.
        private GraphicsDevice GraphicsDevice { get; set; }

        public Graphics()
        {
            Debug.Fail("Please call Graphics(game) constructor "
                         + "instead of this constructor"); 
        }
        public Graphics(Game game, uint planeNum)
        {
            Debug.Assert(game != null);

            grpDevMgr = new GraphicsDeviceManager(game);

            planes = new LinkedList<Sprite>[planeNum];
            Debug.Assert(planes != null);
            if ( planes != null )
            {
                maxPlaneNum = planeNum;
            }
        }

        public void SetGraphicDevice(GraphicsDevice grpDevice)
        {
            GraphicsDevice = grpDevice;
            Debug.Assert(GraphicsDevice != null);
        }

        public bool Init2D()
        {
            Debug.Assert(GraphicsDevice != null);

            sprBatch = new SpriteBatch(GraphicsDevice);
            Debug.Assert(sprBatch != null);
            return (sprBatch != null);
        }

        public void ClearScreen(Color color)
        {
            Debug.Assert(GraphicsDevice != null);
            GraphicsDevice.Clear(color);
        }

        /**
         * 2D Graphics
         */
        void DrawImage(uint planeNo, Texture2D image, Vector2 position)
        {
            Debug.Assert(planeNo < maxPlaneNum);
        }
    };
}
