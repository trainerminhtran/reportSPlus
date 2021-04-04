using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ExcelDataReader;
using System.Data;
using System.IO;
using System.Net;
using System.Net.Http;
using Splusreport.Models;

namespace Splusreport.Controllers
{
    public class AdminController : Controller
    {
        private readonly SPlusReportEntities _db = new SPlusReportEntities();
        // GET: DearlerFSM
        public ActionResult Index()
        {
            return View();
        }


        // POST: Admin/Create Splus
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateSplus(FormCollection collection, HttpPostedFileBase file)
        {
            try
            {
                #region Variable Declaration  
                string message = "";
                DataSet dsexcelRecords = new DataSet();
                IExcelDataReader reader = null;
                Stream FileStream = null;
                List<SplusActivityUpload> templist = new List<SplusActivityUpload>();
                #endregion

                #region Save DealerFSM Detail From Excel  

                if (file.ContentLength > 0)
                {
                    FileStream = file.InputStream;

                    if (FileStream != null)
                    {
                        if (file.FileName.EndsWith(".xls"))
                            reader = ExcelReaderFactory.CreateBinaryReader(FileStream);
                        else if (file.FileName.EndsWith(".xlsx"))
                            reader = ExcelReaderFactory.CreateOpenXmlReader(FileStream);
                        else
                            message = "The file format is not supported.";

                        dsexcelRecords = reader.AsDataSet();
                        reader.Close();
                        var datas = _db.SplusActivityUploads.Where(x => x.AttempEnddate.Month == DateTime.Now.Month);
                        if (dsexcelRecords != null && dsexcelRecords.Tables.Count > 0)
                        {
                            DataTable dtStudentRecords = dsexcelRecords.Tables[0];
                            for (int i = 1; i < dtStudentRecords.Rows.Count; i++)
                            {
                                SplusActivityUpload objStudent = new SplusActivityUpload();
                                objStudent.LoginID = Convert.ToString(dtStudentRecords.Rows[i][1]);
                                if (dtStudentRecords.Rows[i][3] != null)
                                {
                                    objStudent.Jobgroup = Convert.ToString(dtStudentRecords.Rows[i][3]);
                                }
                                objStudent.ActivityCode = Convert.ToString(dtStudentRecords.Rows[i][6]);

                                if (objStudent.ActivityCode.ToLower().Contains("test") && !string.IsNullOrEmpty(dtStudentRecords.Rows[i][10].ToString()))
                                {
                                    objStudent.AttempEnddate = Convert.ToDateTime(dtStudentRecords.Rows[i][10]);
                                    if (!string.IsNullOrEmpty(dtStudentRecords.Rows[i][12].ToString()) && objStudent.AttempEnddate.Month == 3)
                                    {
                                        objStudent.Score = Convert.ToInt32(dtStudentRecords.Rows[i][12]);
                                        var fromDatabase = datas.Where(x => x.LoginID == objStudent.LoginID).FirstOrDefault();

                                        var b = templist.Where(x => x.LoginID == objStudent.LoginID).FirstOrDefault();

                                        if (b == null || (b != null && b.Score < objStudent.Score))
                                        {
                                            //Add to database if not exist
                                            if (fromDatabase == null || fromDatabase.Score < objStudent.Score)
                                            {
                                                _db.SplusActivityUploads.Add(objStudent);
                                                templist.Add(objStudent);
                                            }
                                        }
                                    }
                                }
                            }

                            int output = _db.SaveChanges();
                            if (output > 0)
                                message = "Successfully uploaded. " + output + " user had done today";
                            else
                                message = "Have no anyone test today.";
                        }
                        else
                            message = "Selected file is empty.";
                    }
                    else
                        message = "Invalid File.";
                }
                ModelState.AddModelError("message", message);

                TempData["SSuccess"] = message;
                return View("Index");

                #endregion
            }
            catch (Exception)
            {
                throw;
            }
        }




        // POST: Admin/Create From dealer
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateDearler(FormCollection collection, HttpPostedFileBase file)
        {
            try
            {
                #region Variable Declaration  
                string message = "";
                DataSet dsexcelRecords = new DataSet();
                IExcelDataReader reader = null;
                Stream FileStream = null;
                #endregion

                #region Save DealerFSM Detail From Excel  

                if (file.ContentLength > 0)
                {
                    FileStream = file.InputStream;

                    if (FileStream != null)
                    {
                        if (file.FileName.EndsWith(".xls"))
                            reader = ExcelReaderFactory.CreateBinaryReader(FileStream);
                        else if (file.FileName.EndsWith(".xlsx"))
                            reader = ExcelReaderFactory.CreateOpenXmlReader(FileStream);
                        else
                            message = "The file format is not supported.";

                        dsexcelRecords = reader.AsDataSet();
                        reader.Close();

                        var datas = _db.DearlerFSMUploads;
                        if (dsexcelRecords != null && dsexcelRecords.Tables.Count > 0)
                        {
                            DataTable dtStudentRecords = dsexcelRecords.Tables[0];
                            for (int i = 1; i < dtStudentRecords.Rows.Count; i++)
                            {
                                DearlerFSMUpload objStudent = new DearlerFSMUpload();
                                objStudent.MNV = Convert.ToString(dtStudentRecords.Rows[i][0]);
                                objStudent.SPlusCode = Convert.ToString(dtStudentRecords.Rows[i][1]);
                                objStudent.Fullname = Convert.ToString(dtStudentRecords.Rows[i][2]);


                                objStudent.Store = Convert.ToString(dtStudentRecords.Rows[i][3]);

                                objStudent.Region = Convert.ToString(dtStudentRecords.Rows[i][4]);
                                var user = datas.Where(x => x.SPlusCode == objStudent.SPlusCode).FirstOrDefault();
                                if (user != null)
                                {
                                    _db.DearlerFSMUploads.Remove(user); 
                                }
                                _db.DearlerFSMUploads.Add(objStudent);
                            }
                        }

                       int output = _db.SaveChanges();
                        if (output > 0)
                            message = "Successfully uploaded.";
                        else
                            message = "Have no anyone uploaded.";
                    }
                    else
                        message = "Selected file is empty.";
                }
                else
                    message = "Invalid File.";
                ModelState.AddModelError("message", message);

                TempData["DSuccess"] = message;
                return View("Index");
                #endregion
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
