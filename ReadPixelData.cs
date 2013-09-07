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
    public class ReadPixelData : AccessPixels
    {
        /// <summary>
        /// this constructor takes input raster and writes it to the respective variable of the parent class
        /// </summary> 
        /// <param name="rasterToRead"></param>
        public ReadPixelData(IRaster2 rasterToRead)
        {
            this.rasterToRead = rasterToRead;
            CreateRasterCursor();
            ProcessPixels();
        }
        /// <summary>
        /// this method reads values from the current block of pixels of the raster
        /// </summary>
        /// <param name="pixelblock3">current block of pixels</param>
        protected override void DoOperationsWithPixels(IPixelBlock3 pixelblock3)
        {


            blockwidth = pixelblock3.Width;
            blockheight = pixelblock3.Height;
            pixelblock3.Mask(255);

            //Get the pixel array.
            pixels = (System.Array)pixelblock3.get_PixelData(0);
            //iterate through each line in an image
            for (long i = 0; i < blockwidth; i++)
            {
                //iterate through each pixel in the line
                for (long j = 0; j < blockheight; j++)
                {
                    double val;
                    //Get the pixel value
                    v = pixels.GetValue(i, j);

                    
                    val = Convert.ToDouble(v.ToString());
                    if (val > 240) val = 0;//if value is higher than 240, then value is probaly incorrect
                    //write pixel value to the array
                    allBlocks[numOfBlock, i + j * 64] = val;

                }
            }

          
        }

   
    }
}
