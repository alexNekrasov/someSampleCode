using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.DataSourcesRaster;

namespace Calculations
{
    public abstract class AccessPixels
    {
        protected  IRaster2 rasterToRead;//the raster that must be processed
        protected  IRasterCursor rasterCursor;
        protected  IRasterEdit rasterEdit;
        protected IPixelBlock3 pixelblock3 = null;
        protected System.Array pixels;
        protected long blockwidth = 0;
        protected long blockheight = 0;
        protected object v;
        protected byte[,] values = new byte[64, 64];
        protected IPnt tlc = null;// current position of a "cursor" in an image specified as row/column
        protected int numOfBlock = 0;//a number of a current block of pixels, which may be useful for verification and bug fixation
        protected double[ , ] allBlocks = new double[91, 4096];//array which contain values of each pixel of the input image
        protected double[,] blocksWithValues = new double[91, 4096];//array which contain values to be written in new image

        //this property returns all values from raster dataset
        public double[,] GetAllValues { get { return allBlocks; } }
        /// <summary>
        /// create and initialyze raster cursor
        /// </summary>
        protected void CreateRasterCursor()
        {
            rasterCursor = rasterToRead.CreateCursorEx(null);
            rasterEdit = rasterToRead as IRasterEdit;

        }
        /// <summary>
        /// this method iterates through blocks of pixels of the image and initiate processing operation for each of them
        /// </summary>
        protected void ProcessPixels()
        {
            do
            {

                tlc = rasterCursor.TopLeft;
                //return pixel block from the cursor
                pixelblock3 = rasterCursor.PixelBlock as IPixelBlock3;

                DoOperationsWithPixels(pixelblock3);

                //increment the current number of pixel block
                numOfBlock = numOfBlock + 1;

            }
            while (rasterCursor.Next() == true);//while there is another block of pixels left
            
           // System.Runtime.InteropServices.Marshal.ReleaseComObject(rasterEdit);
            
        }
        /// <summary>
        /// this methof performs actual processing of pixels 
        /// </summary>
        /// <param name="pixelblock3">block to be processed</param>
        protected abstract void DoOperationsWithPixels(IPixelBlock3 pixelblock3);
        
    }
}
