using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using GeometryManager;
using Geometry;
//Created Oleksandr Nekrasov (860725-2510) 2011-08-28
namespace DataStorageUtility
{
     public class DataBaseAccess
    {
        private SqlConnection connectObj = new SqlConnection();
        private ArrayList listOfParcels = new ArrayList();//this array list contains parcels which should be read from the file
        private ArrayList listOfPoints = new ArrayList();//this array list contains corner points which should be read from the file
        private ArrayList listOfSegments = new ArrayList();//this array list contains border segments which should be read from the file
        private DataSet dataSetObjParcels = new DataSet();
        private DataSet dataSetObjBorders = new DataSet();
        private DataSet dataSetObjCorners = new DataSet();
        private ParcelManager managerParcel;
        private SqlDataAdapter adapterParcels;
        private SqlDataAdapter adapterBorderSegments;
        private SqlDataAdapter adapterCornerPoints;
        /// <summary>
        /// this constructor initializes a connection string and invokes methods which set connection
        /// </summary>
        public DataBaseAccess(ParcelManager managerParcel)
        {
            connectObj.ConnectionString = @"Data Source=.\SQLEXPRESS;AttachDbFilename=D:\Studies\Malmo_advanced\Project\ProjectCS\DataStorageUtility\GeometryStorage.mdf;Integrated Security=True;Connect Timeout=30;User Instance=True";
            //connectObj.ConnectionString = Properties.Settings.Default.GeometryStorageConnectionString; //<--- This one doesn't work!!!!!!!!! Dear teacher, if in your computer it will work, please just use this line instead of the upper one. 
            //this is the bug that I cannot ever fix, since there is no sollution in internet to fix this problem!!!!
            this.managerParcel = managerParcel;
            GetConnection();
            FillDataSets();
            FillCorners();
            FillParcels();
            FillBorderSegments();
            FillManager();
        }
        /// <summary>
        /// this method establishes connection to the DB through initialization of two adapters
        /// </summary>
        private void GetConnection()
        {
            connectObj.Open();
            string sqlCommand = "Select * FROM Parcels";
            string sqlCommand2 = "Select * FROM BorderSegments";
            string sqlCommand3 = "Select * FROM CornerPoints";
            adapterParcels = new SqlDataAdapter(sqlCommand, connectObj);
            adapterBorderSegments = new SqlDataAdapter(sqlCommand2, connectObj);
            adapterCornerPoints = new SqlDataAdapter(sqlCommand3, connectObj);
            connectObj.Close();
        }
        /// <summary>
        /// this method fills data adapters with corresponding data
        /// </summary>
        private void FillDataSets()
        {
            adapterParcels.Fill(dataSetObjParcels, "Parcels");
            adapterBorderSegments.Fill(dataSetObjBorders, "BorderSegments");
            adapterCornerPoints.Fill(dataSetObjCorners, "CornerPoints");
        }
         /// <summary>
         /// this method fills data from the respective data adapter to the list of corner points
         /// </summary>
        private void FillCorners()
        {
            DataTable dt = dataSetObjCorners.Tables["CornerPoints"];
            foreach (DataRow r in dt.Rows)
            {
                CornerPoint newPoint = new CornerPoint();
                newPoint.cornerNum = Convert.ToUInt64(Convert.ToString(r[1]));
                newPoint.coordX = Convert.ToDouble(Convert.ToString(r[2]));
                newPoint.coordY = Convert.ToDouble(Convert.ToString(r[3]));
                newPoint.SRS = Convert.ToInt32(Convert.ToString(r[4]));
                listOfPoints.Add(newPoint);
            }
            
        }

        /// <summary>
        /// this method fills data from the respective data adapter to the list of border segments
        /// </summary>
        private void FillParcels()
        {
            DataTable dt = dataSetObjParcels.Tables["Parcels"];
            foreach (DataRow r in dt.Rows)
            {
                Parcel newParcel = new Parcel();
                newParcel.CadastralNum = (ulong)Convert.ToInt64(Convert.ToString(r[1]));
                listOfParcels.Add(newParcel);
            }

        }


        /// <summary>
        /// this method fills data from the respective data adapter to the list of border segments
        /// </summary>
        private void FillBorderSegments()
        {
            DataTable dt = dataSetObjBorders.Tables["BorderSegments"];
            foreach (DataRow r in dt.Rows)
            {
                BorderSegment newSegment = new BorderSegment();
                BorderSegment newSegment2 = new BorderSegment();
                newSegment.AddCorner(ReturnCorner(Convert.ToString(r[1])));
                newSegment.AddCorner(ReturnCorner(Convert.ToString(r[2])));
                newSegment2.AddCorner(ReturnCorner(Convert.ToString(r[1])));
                newSegment2.AddCorner(ReturnCorner(Convert.ToString(r[2])));

                //the banch of code bellow adds the new segment to the left and right parcels
                if (ReturnParcel(Convert.ToString(r[3])) != null)//if there is a parcel to the left of the segment
                {
                    BorderSegment veryNewSegment = newSegment;
                    veryNewSegment.LeftOrNot = true;
                    ReturnParcel(Convert.ToString(r[3])).BorderSegmentAdd(veryNewSegment);
                }
                else
                { 
                }
                if (ReturnParcel(Convert.ToString(r[4])) != null)//if there is a parcel to the right of the segment
                {
                    BorderSegment veryNewSegment2 = newSegment2;
                    veryNewSegment2.LeftOrNot = false;
                    ReturnParcel(Convert.ToString(r[4])).BorderSegmentAdd(veryNewSegment2);
                }
                else
                { 
                }
            }

        }
         /// <summary>
         /// this method fills parcel manager with parcels from the list
         /// </summary>
        private void FillManager()
        {
            foreach (Parcel p in listOfParcels)
            {
                managerParcel.AddParcel(p);
            }
        }

        /// <summary>
        /// this method is used to return specified corner point from an array
        /// </summary>
        /// <param name="pointStrID">registration number of the point that should be returned</param>
        /// <returns>corner point with the specified number</returns>
        private CornerPoint ReturnCorner(string pointStrID)
        {
            ulong pointID = Convert.ToUInt32(pointStrID);
            CornerPoint point = new CornerPoint();
            foreach (CornerPoint element1 in listOfPoints)
            {
                if (element1.cornerNum == pointID)
                {
                    point = element1;
                }
            }
            return point;

        }

        /// <summary>
        /// this method is used to return specified parcel from an array
        /// </summary>
        /// <param name="pointStrID">registration number of the point that should be returned</param>
        /// <returns>corner point with the specified number</returns>
        private Parcel ReturnParcel(string cadastrulNumberStrID)
        {
            try
            {
                ulong pointID = Convert.ToUInt32(cadastrulNumberStrID);
                Parcel parcel = null;
                foreach (Parcel element1 in listOfParcels)
                {
                    if (element1.CadastralNum == pointID)
                    {
                        parcel = element1;
                    }
                }
                return parcel;
            }
            catch (FormatException exc) { return null; }//if this exception is catched then the segment has not right or left parcel defined
            
           

        }
    }
}
