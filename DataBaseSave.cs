using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Collections.ObjectModel;
using System.ComponentModel;
using GeometryManager;
using Geometry;
//Created Oleksandr Nekrasov (860725-2510) 2011-08-28
namespace DataStorageUtility
{
   public class DataBaseSave
    {
        private ArrayList listOfParcels = new ArrayList();//this array list contains parcels which should be written to the file
        private ArrayList listOfPoints = new ArrayList();//this array list contains corner points which should be written to the file
        private ArrayList listOfSegments = new ArrayList();//this array list contains border segments which should be written to the file
        private SqlConnection connectObj = new SqlConnection();
        private ParcelManager managerParcel;
       /// <summary>
       /// this constructor sets the refference to parcel manager 
       /// </summary>
       public DataBaseSave(ParcelManager managerParcel)
       {
           connectObj.ConnectionString = @"Data Source=.\SQLEXPRESS;AttachDbFilename=D:\Studies\Malmo_advanced\Project\ProjectCS\DataStorageUtility\GeometryStorage.mdf;Integrated Security=True;Connect Timeout=30;User Instance=True";
           //connectObj.ConnectionString = Properties.Settings.Default.GeometryStorageConnectionString; //<--- This one doesn't work!!!!!!!!! 
           //this is the bug that I cannot ever fix, since there is no sollution in internet to fix this problem!!!!
           this.managerParcel = managerParcel;
           FillArrays();
           connectObj.Open();
           Delete();
           WritteCorners();
           WritteBorderSegments();
           WritteParcels();
           connectObj.Close();
       }

       /// <summary>
       /// this method clears all records from the database
       /// </summary>
       public void Delete()
       {


           SqlCommand commandCorners = new SqlCommand("DELETE FROM CornerPoints", connectObj);
           SqlCommand commandParcels = new SqlCommand("DELETE FROM Parcels", connectObj);
           SqlCommand commandBorders = new SqlCommand("DELETE FROM BorderSegments", connectObj);
           commandCorners.ExecuteNonQuery();
           commandParcels.ExecuteNonQuery();
           commandBorders.ExecuteNonQuery();
       }
       /// <summary>
       /// This method fills the array lists with appropriate objects: parcels, boundary segments, corner points
       /// this method also invokes the check whether current object was already added to array
       /// </summary>
       private void FillArrays()
       {
           for (int i = 0; i < managerParcel.NumberOfParcels; i++)
           {
               if (!CheckForDuplicatesParcels(managerParcel.GetParcel(i).ReturnNumberAsString()))
               { listOfParcels.Add(managerParcel.GetParcel(i)); }

               for (int j = 0; j < managerParcel.GetParcel(i).GetNumber; j++)
               {
                   if (!CheckForDuplicatesSegments(managerParcel.GetParcel(i).ReturnSegment(j)))
                   {
                       listOfSegments.Add(managerParcel.GetParcel(i).ReturnSegment(j));
                   }
                   
                   for (int k = 0; k <= 1; k++)
                   {
                       if (!CheckForDuplicatesPoints(managerParcel.GetParcel(i).ReturnSegment(j).CornerData(k).numberAsString()))
                       {
                           listOfPoints.Add(managerParcel.GetParcel(i).ReturnSegment(j).CornerData(k));
                       }
                   }
               }
           }
       }

       /// <summary>
       /// this method writtes all corners to the database
       /// </summary>
       private void WritteCorners()
       {
           for (int i = 0; i < listOfPoints.Count; i++)
           {
               
                   
                   SqlCommand command = new SqlCommand("INSERT INTO CornerPoints (num, CadastralNumber, CoordinateX, CoordinateY, CRS) " +
                           "VALUES (@num, @CadastralNumber, @CoordinateX, @CoordinateY, @CRS)", connectObj);
                   command.Parameters.Add("@num", SqlDbType.Int).Value = i;
                   command.Parameters.Add("@CadastralNumber", SqlDbType.NChar).Value = ((CornerPoint)listOfPoints[i]).cornerNum;
                   command.Parameters.Add("@CoordinateX", SqlDbType.Decimal).Value = ((CornerPoint)listOfPoints[i]).coordX;
                   command.Parameters.Add("@CoordinateY", SqlDbType.Decimal).Value = ((CornerPoint)listOfPoints[i]).coordY;
                   command.Parameters.Add("@CRS", SqlDbType.NChar).Value = ((CornerPoint)listOfPoints[i]).SRS;
                   command.ExecuteNonQuery();
                   
           }
       }
       
       /// <summary>
       /// this method writtes all parcels to the database
       /// </summary>
       private void WritteParcels()
       {
           for (int i = 0; i < listOfParcels.Count; i++)
           {
               
                   
                   SqlCommand command = new SqlCommand("INSERT INTO Parcels (num, CadastralNumber) " +
                           "VALUES (@num, @CadastralNumber)", connectObj);
                   command.Parameters.Add("@num", SqlDbType.Int).Value = i;
                   command.Parameters.Add("@CadastralNumber", SqlDbType.NChar).Value = ((Parcel)listOfParcels[i]).CadastralNum;
                   command.ExecuteNonQuery();
                   
               
           }
       }
       /// <summary>
       /// this method writtes all border segments to the database
       /// </summary>
       private void WritteBorderSegments()
       {

           for (int i = 0; i < listOfSegments.Count; i++)
           {
               
               if (listOfSegments[i] != null)
               {
                   SqlCommand command = new SqlCommand("INSERT INTO BorderSegments (num, FirstPoint, SecondPoint, LeftParcel, RightParcel) " +
                               "VALUES (@num, @FirstPoint, @SecondPoint,@LeftParcel, @RightParcel)", connectObj);
                   command.Parameters.Add("@num", SqlDbType.Int).Value = i;
                   command.Parameters.Add("@FirstPoint", SqlDbType.NChar).Value = ((BorderSegment)listOfSegments[i]).CornerData(0).cornerNum;
                   command.Parameters.Add("@SecondPoint", SqlDbType.NChar).Value = ((BorderSegment)listOfSegments[i]).CornerData(1).cornerNum;
                   command.Parameters.Add("@LeftParcel", SqlDbType.NChar).Value = ReturnLeftParcelNumber(((BorderSegment)listOfSegments[i]));
                   command.Parameters.Add("@RightParcel", SqlDbType.NChar).Value = ReturnRightParcelNumber(((BorderSegment)listOfSegments[i]));
                   command.ExecuteNonQuery();
               }
                   
            

           }
       }

       /// <summary>
       /// this method check the segments for been common for two parcels
       /// </summary>
       /// <param name="seg">segment to check</param>
       /// <param name="j">number of segment in array</param>
       private void DuplicatedSegment(BorderSegment seg, int i)
       {
           for (int j = 0; j < listOfSegments.Count; j++)
           {
               if ((j != i) && (((BorderSegment)listOfSegments[i]).Name() == seg.Name()))
                   listOfSegments[j] = null;
           }
       }

       /// <summary>
       /// This method performs the check about whether current parcel was already added to the array of parcels
       /// </summary>
       /// <param name="num"></param>
       /// <returns></returns>
       private bool CheckForDuplicatesParcels(string num)
       {
           bool index = false;
           foreach (Parcel item in listOfParcels)
           {
               if (num == ((Parcel)item).ReturnNumberAsString()) index = true;
           }
           return index;
       }
       /// <summary>
       /// This method performs the check about whether current border segment was already added to the array of border segments
       /// </summary>
       /// <param name="num">number of segment whitch should be checked</param>
       /// <returns></returns>
       private bool CheckForDuplicatesSegments(BorderSegment seg)
       {
           bool index = false;
           foreach (BorderSegment item in listOfSegments)
           {
               if ((seg.Name() == ((BorderSegment)item).Name())) index = true;
           }
           return index;
       }
       /// <summary>
       /// This method performs the check about whether current corner poin was already added to the array of points
       /// </summary>
       /// <param name="num"></param>
       /// <returns></returns>
       private bool CheckForDuplicatesPoints(string num)
       {
           bool index = false;
           foreach (CornerPoint item in listOfPoints)
           {
               if (num == ((CornerPoint)item).numberAsString()) index = true;
           }
           return index;
       }
       /// <summary>
       /// this method returns the cadastral number of the parcel which is left to the segment
       /// </summary>
       /// <param name="currentSegment">the current segment</param>
       /// <returns>cadastral number of a left parcel</returns>
       private string ReturnLeftParcelNumber(BorderSegment currentSegment)
       {
           for (int i = 0; i < listOfParcels.Count; i++)
           {
               for (int j = 0; j < ((Parcel)listOfParcels[i]).GetNumber; j++)
               {
                   if ((((Parcel)listOfParcels[i]).ReturnSegment(j).Name() == currentSegment.Name()) && (((Parcel)listOfParcels[i]).ReturnSegment(j).LeftOrNot == true))
                   {
                       return Convert.ToString(((Parcel)listOfParcels[i]).CadastralNum);
                   }
               }
           }
           return "";
       }

       /// <summary>
       /// this method returns the cadastral number of the parcel which is right to the segment
       /// </summary>
       /// <param name="currentSegment">the current segment</param>
       /// <returns>cadastral number of a right parcel</returns>
       private string ReturnRightParcelNumber(BorderSegment currentSegment)
       {
           for (int i = 0; i < listOfParcels.Count; i++)
           {
               for (int j = 0; j < ((Parcel)listOfParcels[i]).GetNumber; j++)
               {
                   if ((((Parcel)listOfParcels[i]).ReturnSegment(j).Name() == currentSegment.Name()) && (((Parcel)listOfParcels[i]).ReturnSegment(j).LeftOrNot == false))
                   {
                       return Convert.ToString(((Parcel)listOfParcels[i]).CadastralNum);
                   }
               }
           }
           return "";
       }

   }
}
