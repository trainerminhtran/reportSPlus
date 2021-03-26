using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ExcelDataReader;
using System.Data;
using System.IO;
using Splusreport.Models;
using Splusreport.Services;
namespace Splusreport.Controllers
{
    public class AdminController : Controller
    {
        private readonly SPlusReportEntities _db = new SPlusReportEntities();
        private static string path = "";

        // GET: DearlerFSM
        public ActionResult Index()
        {
            path = Server.MapPath("~/Uploads/Data.csv");
            if (!Directory.Exists(Server.MapPath("~/ Uploads")))
            {
                Directory.CreateDirectory(Server.MapPath("~/ Uploads"));
            }
            if (!System.IO.File.Exists(path))
            {

                string[] title = new string[] {
                     "SPlusCode",
                     "Fullname",
                     "Store",
                     "Region",
                     "ActivityCode",
                     "IsLearned",
                     "Score"
                };
                // Write sample data to CSV file
                using (CsvFileWriter writer = new CsvFileWriter(path))
                {
                    //Add data to row
                    CsvRow row = new CsvRow();
                    for (int j = 0; j < title.Length; j++)
                        row.Add(title[j]);

                    //add row to file
                    writer.WriteRow(row);
                }

                // Read sample data from CSV file
                using (CsvFileReader reade = new CsvFileReader(path))
                {
                    CsvRow row = new CsvRow();
                    while (reade.ReadRow(row))
                    {
                        for (int j = 1; j < row.Count; j++)
                        {

                        }
                    }
                }
            }



            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateSplus(FormCollection collection, HttpPostedFileBase file)
        {
            try
            {
                #region Variable Declaration  
                string message = "";
                DataSet dsexcelRecords = new DataSet();
                IExcelDataReader r = null;
                Stream FileStream = null;
                List<SplusActivityUpload> templist = new List<SplusActivityUpload>();
                #endregion

                #region Save Splus activation Detail From Excel  

                if (file.ContentLength > 0)
                {
                    FileStream = file.InputStream;

                    if (FileStream != null)
                    {
                        if (file.FileName.EndsWith(".xls"))
                            r = ExcelReaderFactory.CreateBinaryReader(FileStream);
                        else if (file.FileName.EndsWith(".xlsx"))
                            r = ExcelReaderFactory.CreateOpenXmlReader(FileStream);
                        else
                            message = "The file format is not supported.";

                        dsexcelRecords = r.AsDataSet();
                        r.Close();


                        //tạo biến chứa dữ liệu nhân viên cần thêm hoặc sửa vào data tìm kiếm
                        var changes = new List<SplusActivityUpload>();
                        //Nếu file có dữ liệu
                        if (dsexcelRecords != null && dsexcelRecords.Tables.Count > 0)
                        {
                            //đặt tên cho bảng dữ liệu
                            DataTable dtStudentRecords = dsexcelRecords.Tables[0];

                            //duyệt dữ liệu từ dòng thứ 2 
                            for (int i = 1; i < dtStudentRecords.Rows.Count; i++)
                            {
                                var date = Convert.ToDateTime(dtStudentRecords.Rows[i][9]);
                                //Nếu không phải tháng này thì thôi
                                if (date.Month != DateTime.Now.Month)
                                {
                                    continue;
                                }
                                //tạo biến chứ dữ liệu từng dòng của file acitivity
                                SplusActivityUpload objStudent = new SplusActivityUpload();
                                objStudent.AttempStartdate = date;
                                objStudent.LoginID = Convert.ToString(dtStudentRecords.Rows[i][1]);
                                objStudent.ActivityCode = Convert.ToString(dtStudentRecords.Rows[i][6]);
                                int score = 0;
                                Int32.TryParse(dtStudentRecords.Rows[i][12].ToString(), out score);
                                objStudent.Score = score;
                                objStudent.IsLearned = "Yes";
                                var b = changes.Where(x => x.LoginID == objStudent.LoginID).FirstOrDefault();
                                if (b != null)
                                {
                                    if (b.Score < objStudent.Score)
                                    {
                                        b.Score = objStudent.Score;
                                    }
                                }
                                else
                                {
                                    changes.Add(objStudent);
                                }
                            }
                            // Write sample data to CSV file
                            string[] title = new string[] {
                                 "SPlusCode",
                                 "Fullname",
                                 "Store",
                                 "Region",
                                 "ActivityCode",
                                 "IsLearned",
                                 "Score"
                            };
                            if (!System.IO.File.Exists(path))
                            {
                                using (CsvFileWriter writer = new CsvFileWriter(path))
                                {
                                    //Tạo tiêu đề
                                    CsvRow row = new CsvRow();
                                    for (int j = 0; j < title.Length; j++)
                                        row.Add(title[j]);
                                    writer.WriteRow(row);

                                    //Add data to row
                                    foreach (var c in changes)
                                    {
                                        row.Add(c.LoginID);
                                        row.Add(c.LoginID);
                                        row.Add(c.LoginID);
                                        row.Add(c.LoginID);
                                        row.Add(c.ActivityCode);
                                        row.Add(c.IsLearned);
                                        row.Add(c.Score.ToString());

                                        //add row to file
                                        writer.WriteRow(row);
                                        row = new CsvRow();
                                    }
                                }
                            }
                        }
                    }
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

        // POST: Admin/Create Splus
        /*
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

                  #region Save Splus activation Detail From Excel  

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
                          List<SplusActivityUpload> idtoRemoves = new List<SplusActivityUpload>();
                          List<SplusActivityUpload> idtoAdds = new List<SplusActivityUpload>();

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

                                  var fromDatabase = datas.Where(x => x.LoginID == objStudent.LoginID).FirstOrDefault();
                                  objStudent.AttempEnddate = Convert.ToDateTime(dtStudentRecords.Rows[i][9]);
                                  if (objStudent.ActivityCode.ToLower().Contains("test") && !string.IsNullOrEmpty(dtStudentRecords.Rows[i][10].ToString()))
                                  {
                                      if (!string.IsNullOrEmpty(dtStudentRecords.Rows[i][12].ToString()) && objStudent.AttempEnddate.Month == DateTime.Now.Month)
                                      {
                                          objStudent.Score = Convert.ToInt32(dtStudentRecords.Rows[i][12]);
                                          objStudent.IsLearned = "Yes";
                                          var b = templist.Where(x => x.LoginID == objStudent.LoginID).FirstOrDefault();
                                          if (b != null)
                                          {
                                              if (b.Score < objStudent.Score)
                                              {
                                                  templist.Remove(b);
                                                  //Add to database if not exist
                                                  if (fromDatabase == null)
                                                  {
                                                      templist.Add(objStudent);
                                                  }
                                                  else if (fromDatabase.Score < objStudent.Score)
                                                  {
                                                      idtoRemoves.Add(fromDatabase);
                                                      templist.Add(objStudent);
                                                  }
                                              }
                                              else
                                              {
                                                  templist.Remove(objStudent);
                                              }
                                          }
                                          else
                                          {
                                              //Add to database if not exist
                                              if (fromDatabase == null)
                                              {
                                                  templist.Add(objStudent);
                                              }
                                              else if (fromDatabase.Score < objStudent.Score)
                                              {
                                                  idtoRemoves.Add(fromDatabase);
                                                  templist.Add(objStudent);
                                              }
                                          }
                                      }
                                  }
                                  else if (fromDatabase == null && templist.Where(x => x.LoginID == objStudent.LoginID).FirstOrDefault() == null)
                                  {
                                      objStudent.IsLearned = "Yes";
                                      templist.Add(objStudent);
                                  }
                              }
                              if (idtoRemoves.Count > 0)
                              {
                                  _db.SplusActivityUploads.RemoveRange(idtoRemoves);
                              }
                              if (templist.Count > 0)
                              {
                                  _db.SplusActivityUploads.AddRange(templist);
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

          */


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
                                objStudent.MNV = Convert.ToString(dtStudentRecords.Rows[i][1]);
                                objStudent.SPlusCode = Convert.ToString(dtStudentRecords.Rows[i][2]);
                                objStudent.Fullname = Convert.ToString(dtStudentRecords.Rows[i][3]);


                                objStudent.Store = Convert.ToString(dtStudentRecords.Rows[i][4]);

                                objStudent.Region = Convert.ToString(dtStudentRecords.Rows[i][5]);
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
