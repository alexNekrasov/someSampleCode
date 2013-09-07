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
    public class WritePixelData : AccessPixels
    {
        /// <summary>
        /// this property returns raster after values were written to it
        /// </summary>
        public IRaster2 GetWrittenRaster { get { return rasterToRead; } }

        /// <summary>
        /// constructor takes empty raster and arrays of values as input and sets respective vriables defined in the parent class
        /// </summary>
        /// <param name="dummy">empty raster</param>
        /// <param name="resultsToWrite">values which must be written to the raster</param>
        public WritePixelData(IRaster2 dummy, double[,] resultsToWrite)
        {
            this.blocksWithValues = resultsToWrite;
            this.rasterToRead = dummy;
            CreateRasterCursor();
            ProcessPixels();
        }
        /// <summary>
        ///  this method writes values to the current block of pixels of the raster
        /// </summary>
        /// <param name="pixelblock3">current block of pixels</param>
        protected override void DoOperationsWithPixels(IPixelBlock3 pixelblock3)
        {
            //Get the pixel array.
            pixels = (System.Array)pixelblock3.get_PixelData(0);
            blockwidth = pixelblock3.Width;
            blockheight = pixelblock3.Height;
            pixelblock3.Mask(255);
            object k = null;

            //iterate through each line in an image         
            for (int i = 0; i < blockwidth; i++)
            {
                //iterate through each pixel in the line
                for (int j = 0; j < blockheight; j++)
                {
                    k = Convert.ToSingle(blocksWithValues[numOfBlock, i + j * 64]);
                    pixels.SetValue(k, i, j);

                    
                }
            }
                
                //Set the pixel array to the pixel block.
                pixelblock3.set_PixelData(0, pixels);

            //Write back to the raster.
            tlc = rasterCursor.TopLeft;
            rasterEdit.Write(tlc, (IPixelBlock)pixelblock3);
        }
    }
}
