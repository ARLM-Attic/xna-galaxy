/**
 * Graphics.cs
 * 
 * 2D / 3D Graphics Implementation.
 * 
 * @file
 * @author
 *        2D Part: YongChul [Chris] Jin / id: anecia
 *                 [Please remove this line and put your name/id here instead]
 *        3D Part:
 *
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
    struct Image2D
    {
        public UInt32      id;
        public Vector2     position;
        public Texture2D   image;
        public bool        holdLife;
    };

    public class Graphics
    {
        private GraphicsDeviceManager grpDevMgr   = null;
        private SpriteBatch           sprBatch    = null;
        private LinkedList<Image2D>[] layers      = null;
        private uint                  maxLayerNum = 0;

        //
        // Summary:
        //     Gets or sets the current Framework.Graphics.GraphicsDevice.
        //
        // Returns:
        //     The current Framework.Graphics.GraphicsDevice.
        private GraphicsDevice GraphicsDevice { get; set; }

        public Graphics()
        {
            Debug.Fail("Please call Graphics(game, planeNum) constructor "
                         + "instead of this constructor"); 
        }
        
        /**
         * Graphics class constructor.
         * 
         * Initializes a new instance of this class, which provides
         * GraphicsDeviceManager and image layer.
         * 
         * @param game          Game object
         * @param layerNum      The number of image layers
         */
        public Graphics(Game game, uint layerNum)
        {
            Debug.Assert(game != null);

            grpDevMgr = new GraphicsDeviceManager(game);

            layers = new LinkedList<Image2D>[layerNum];
            Debug.Assert(layers != null);
            if ( layers != null )
            {
                maxLayerNum = layerNum;
            }
        }

        /**
         * Sets the GraphicsDevice.
         * 
         * @param grpDevice     Graphics Device
         */
        public void SetGraphicDevice(GraphicsDevice grpDevice)
        {
            GraphicsDevice = grpDevice;
            Debug.Assert(GraphicsDevice != null);
        }
        
        //===================================================================
        // 2D Graphic Implimentation
        //===================================================================
        #region 2DGraphic
        
        /**
         * Initializes 2D Graphic system.
         * 
         * It should be called once before using 2D Graphic methods.
         * 
         * @return  true if the initialization of 2D system is succeed,
         *          false otherwise.
         */
        public bool Init2D()
        {
            Debug.Assert(GraphicsDevice != null);

            sprBatch = new SpriteBatch(GraphicsDevice);
            Debug.Assert(sprBatch != null);
            return (sprBatch != null);
        }

        /**
         * Clears the screen with the given color.
         * 
         * @param color     Color to clear the screen
         */
        public void ClearScreen(Color color)
        {
            Debug.Assert(GraphicsDevice != null);
            GraphicsDevice.Clear(color);
        }

        /**
         * Puts the image to an image layer.
         * 
         * The image added to the image layer is not drawn immediately,
         * it will be drawn when Update() method is called.
         * 
         * @param layerNo       Layer number which will have the image
         * @param image         Image, 2D texture
         * @param position      The location, in screen coordinates,
         *                      where the image will be put.
         * 
         * @return  an unique id for the added image as Image2D to an
         *          image layer, or 0 otherwise.
         */
        UInt32 PutImage(uint layerNo, Texture2D image, Vector2 position)
        {
            Image2D myImage;

            Debug.Assert(layerNo < maxLayerNum);

            myImage = new Image2D();
            myImage.id       = 1;
            myImage.position = position;
            myImage.image    = image;
            myImage.holdLife = true;

            layers[layerNo - 1].AddLast(myImage);

            return myImage.id;
        }
        
        #endregion // 2DGraphic

        //===================================================================
        // 3D Graphic Implimentation
        //===================================================================
        #region 2DGraphic
    
        #endregion // 3DGraphic
    };
}
