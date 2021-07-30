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
        private static string path = "~/Uploads/Data.csv";
        private static string pathNK = "~/Uploads/DataNK.csv";
        private static string[] title = new string[] {
                     "SPlusCode",
                     "Fullname",
                     "Store",
                     "Region",
                     "ActivityCode",
                     "IsLearned",
                     "SecondTest",
                     "Score",
                     "SecondLearn",
                     "TimesLearn",
                     "Đạt"
                };

        // GET: DearlerFSM
        public ActionResult Index()
        {
            path = Server.MapPath("~/Uploads/Data.csv");
            pathNK = Server.MapPath("~/Uploads/DataNK.csv");
            if (!Directory.Exists(Server.MapPath("~/ Uploads")))
            {
                Directory.CreateDirectory(Server.MapPath("~/Uploads"));
            }
            if (!System.IO.File.Exists(path))
            {


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
            }
            if (!System.IO.File.Exists(pathNK))
            {

                string[] titleNK = new string[] {
                     "SPlusCode",
                     "Fullname",
                     "Store",
                     "Region",
                     "ActivityCode",
                     "IsLearned",
                     "SecondTest",
                     "Score"
                };
                // Write sample data to CSV file
                using (CsvFileWriter writer = new CsvFileWriter(pathNK))
                {
                    //Add data to row
                    CsvRow row = new CsvRow();
                    for (int j = 0; j < titleNK.Length; j++)
                        row.Add(titleNK[j]);

                    //add row to file
                    writer.WriteRow(row);
                }
            }
            return View();
        }
        public void ReaderCSV(string path, out int learned, out int tested)
        {
            // Read sample data from CSV file
            using (CsvFileReader reade = new CsvFileReader(path))
            {
                var l = -1;//trừ dòng đầu tiên
                var t = 0;
                CsvRow row = new CsvRow();
                while (reade.ReadRow(row))
                {
                    if (!string.IsNullOrEmpty(row[5]))
                    {
                        l++;
                    }
                    var score = 0;
                    Int32.TryParse(row[6], out score);
                    if (score > 0)
                    {
                        t++;
                    }
                }
                learned = l;
                tested = t;
            }

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
                var learnedOld = 0;
                var learnedNew = 0;
                var testedOld = 0;
                var testedNew = 0;

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
                        var searchResults = _db.DearlerFSMUploads.Select(x => new SearchResult
                        {
                            SPlusCode = x.SPlusCode,
                            Fullname = x.Fullname,
                            Region = x.Region,
                            Store = x.Store,
                            ActivityCode = "",
                            IsLearned = "",
                            SecondTest = 0,
                            Score = 0,
                            SecondLearn = 0,
                            TimesLearn = 0,
                            IsComplete = "No"
                        }).ToList();


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
                                //if (date.Month != DateTime.Now.Month)
                                //{
                                //    continue;
                                //}
                                //tạo biến chứ dữ liệu từng dòng của file acitivity
                                SplusActivityUpload objStudent = new SplusActivityUpload();
                                objStudent.AttempStartdate = date;
                                var loginId = Convert.ToString(dtStudentRecords.Rows[i][1]).ToUpper();
                                if (loginId == "CE.MM00323874")
                                {
                                    var m = loginId + "M";
                                }
                                if (loginId.Contains("TGDD"))
                                {
                                    loginId = loginId.Replace("TGDD", "DMX");
                                }
                                objStudent.LoginID = loginId.Contains("CE.") ? loginId : "CE." + loginId;
                                if (loginId.Contains("NK"))
                                {
                                    continue;
                                }
                                //if (objStudent.LoginID == "CE.DMX31228")
                                //{
                                //    objStudent.LoginID = "CE.DMX31228";
                                //}
                                objStudent.ActivityName = Convert.ToString(dtStudentRecords.Rows[i][5]);
                                objStudent.ActivityCode = Convert.ToString(dtStudentRecords.Rows[i][6]);
                                decimal score = 0;
                                decimal secondTest = 0;
                                var sc = dtStudentRecords.Rows[i][12].ToString();
                                Decimal.TryParse(dtStudentRecords.Rows[i][12].ToString(), out score);
                                Decimal.TryParse(dtStudentRecords.Rows[i][11].ToString(), out secondTest);

                                //Nếu mã bài học có chữ "TEST" thì mới tính điểm, ko thì gán điểm bằng 0
                                if (objStudent.ActivityCode.ToLower().Contains("test"))
                                {
                                    objStudent.Score = (int)score;
                                    objStudent.SecondTest = (int)secondTest;
                                }
                                else
                                {
                                    objStudent.SecondTest = 0;
                                }
                                //Đánh dấu đã học bài (luôn luôn đã học bài vì có thể học bài, hoặc làm bài test cũng tính học bài

                                string[] add3point = {
                                    "[Quan Trọng] - Video Cẩm Nang Bán Hàng - Máy Lọc Không Khí, Máy Hút Bụi, Lò Vi Sóng Samsung - Tháng 07/2021",
                                    "[Đề Xuất] - Bài Học Tham Khảo 1 - Tháng 07/2021",
                                };
                                string[] add1point = {
                                    "[Quan Trọng] - Video Cẩm Nang Bán Hàng - Lò Vi Sóng Samsung - Tháng 07/2021",
                                    "[Quan Trọng] - Video Cẩm Nang Bán Hàng - Máy Lọc Không Khí Samsung - Tháng 07/2021",
                                    "[Quan Trọng] - Video Cẩm Nang Bán Hàng - Máy Hút Bụi Samsung - Tháng 07/2021",
                                };
                                if (add3point.Contains(objStudent.ActivityName))
                                {
                                    objStudent.TimesLearn = 3;
                                    objStudent.SecondLearn = (int)secondTest;
                                }

                                if (add1point.Contains(objStudent.ActivityName))
                                {
                                    objStudent.TimesLearn = 1;
                                    objStudent.SecondLearn = (int)secondTest;
                                }
                                if (objStudent.TimesLearn >= 3 && (objStudent.SecondLearn / 60) >= 6)
                                {
                                    objStudent.IsLearned = "Yes";
                                }
                                else
                                {
                                    objStudent.IsLearned = "";
                                }
                                if (objStudent.IsLearned == "Yes" && objStudent.Score > 0)
                                {
                                    objStudent.IsComplete = "Yes";
                                }
                                else
                                {
                                    objStudent.IsComplete = "No";
                                }
                                var b = changes.Where(x => x.LoginID == objStudent.LoginID).FirstOrDefault();
                                if (b != null)
                                {
                                    // So sánh điểm test cũ nếu điểm bài test cũ ít điểm hơn sẽ thay thế bằng điểm và thời gian làm bài mới
                                    if (b.Score < objStudent.Score)
                                    {
                                        b.Score = objStudent.Score;
                                        b.SecondTest = objStudent.SecondTest;
                                    }

                                    //bài học
                                    b.TimesLearn += objStudent.TimesLearn;
                                    b.SecondLearn += objStudent.SecondLearn;
                                    if (b.TimesLearn >= 3 && (b.SecondLearn / 60) >= 6)
                                    {
                                        b.IsLearned = "Yes";
                                    }
                                    if (b.IsComplete == "Yes" || (b.IsLearned == "Yes" && b.Score > 0))
                                    {
                                        b.IsComplete = "Yes";
                                    }
                                }
                                else
                                {
                                    changes.Add(objStudent);
                                }
                            }
                            // Write sample data to CSV file

                            if (System.IO.File.Exists(path))
                            {

                                // Read sample data from CSV file
                                ReaderCSV(path, out learnedOld, out testedOld);


                                using (CsvFileWriter writer = new CsvFileWriter(path))
                                {
                                    //Tạo tiêu đề
                                    CsvRow row = new CsvRow();
                                    for (int j = 0; j < title.Length; j++)
                                        row.Add(title[j]);
                                    writer.WriteRow(row);

                                    //searchResults fillin
                                    for (int i = 0; i < searchResults.Count(); i++)
                                    {
                                        var s = searchResults[i];
                                        var spluscode = s.SPlusCode.ToUpper();

                                        var input = changes.FirstOrDefault(x => x.LoginID.ToUpper() == spluscode);

                                        if (input != null)
                                        {
                                            s.IsLearned = input.IsLearned;
                                            s.Score = input.Score;
                                            s.SecondTest = input.SecondTest;
                                            s.ActivityCode = input.ActivityCode;
                                            learnedNew++;
                                            if (input.Score > 0)
                                            {
                                                testedNew++;
                                            }
                                            s.SecondLearn = input.SecondLearn;
                                            s.TimesLearn = input.TimesLearn;
                                            s.IsComplete = input.IsComplete;
                                        }
                                        //Add data to row
                                        row = new CsvRow();
                                        row.Add(s.SPlusCode);
                                        row.Add(s.Fullname);
                                        row.Add(s.Store);
                                        row.Add(s.Region);
                                        row.Add(s.ActivityCode);
                                        row.Add(s.IsLearned);
                                        row.Add(s.SecondTest.ToString());
                                        row.Add(s.Score.ToString());
                                        row.Add(s.SecondLearn.ToString());
                                        row.Add(s.TimesLearn.ToString());
                                        if (s.IsComplete == null)
                                        {
                                            row.Add("");
                                        }
                                        else
                                        {
                                            row.Add(s.IsComplete.ToString());
                                        }
                                        //add row to file
                                        writer.WriteRow(row);
                                    }
                                }
                                message = "Successfully uploaded. Have " + (learnedNew - learnedOld) + " learned and " + (testedNew - testedOld) + " tested today";
                            }
                        }//end readfile activity
                    }
                }

                ModelState.AddModelError("message", message);
                _db.Dayupdates.Add(new Dayupdate { Dateupdate = DateTime.Now });
                _db.SaveChanges();
                TempData["SSuccess"] = message;
                return View("Index");
                #endregion
            }
            catch (Exception e)
            {
                var mes = e;
                throw;
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateSplusNK(FormCollection collection, HttpPostedFileBase file)
        {
            try
            {
                #region Variable Declaration  
                string message = "";
                DataSet dsexcelRecords = new DataSet();
                IExcelDataReader r = null;
                Stream FileStream = null;
                List<SplusActivityUpload> templist = new List<SplusActivityUpload>();
                var learnedOld = 0;
                var learnedNew = 0;
                var testedOld = 0;
                var testedNew = 0;

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
                        var searchResults = _db.DearlerFSMUploads.Where(x => x.Dealer == "NK").Select(x => new SearchResult
                        {
                            SPlusCode = x.SPlusCode,
                            Fullname = x.Fullname,
                            Region = x.Region,
                            Store = x.Store,
                            ActivityCode = "",
                            IsLearned = "",
                            SecondTest = 0,
                            Score = 0
                        }).ToList();


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
                                //if (date.Month != DateTime.Now.Month)
                                //{
                                //    continue;
                                //}
                                //tạo biến chứ dữ liệu từng dòng của file acitivity
                                SplusActivityUpload objStudent = new SplusActivityUpload();
                                objStudent.AttempStartdate = date;
                                var loginId = Convert.ToString(dtStudentRecords.Rows[i][1]).ToUpper();


                                if (!loginId.Contains("NK"))
                                {
                                    continue;
                                }
                                objStudent.LoginID = loginId;
                                objStudent.ActivityCode = Convert.ToString(dtStudentRecords.Rows[i][6]);
                                decimal score = 0;
                                decimal secondTest = 0;
                                var sc = dtStudentRecords.Rows[i][12].ToString();
                                Decimal.TryParse(dtStudentRecords.Rows[i][12].ToString(), out score);
                                Decimal.TryParse(dtStudentRecords.Rows[i][11].ToString(), out secondTest);

                                //Nếu mã bài học có chữ "TEST" thì mới tính điểm, ko thì gán điểm bằng 0
                                if (objStudent.ActivityCode.ToLower().Contains("test"))
                                {
                                    objStudent.Score = (int)score;
                                    objStudent.SecondTest = (int)secondTest;
                                }
                                else
                                {
                                    objStudent.Score = 0;
                                    objStudent.SecondTest = 0;
                                }
                                //Đánh dấu đã học bài (luôn luôn đã học bài vì có thể học bài, hoặc làm bài test cũng tính học bài
                                objStudent.IsLearned = "Yes";

                                var b = changes.Where(x => x.LoginID == objStudent.LoginID).FirstOrDefault();
                                if (b != null)
                                {
                                    // So sánh điểm test cũ nếu điểm bài test cũ ít điểm hơn sẽ thay thế bằng điểm và thời gian làm bài mới
                                    if (b.Score < objStudent.Score)
                                    {
                                        b.Score = objStudent.Score;
                                        b.SecondTest = objStudent.SecondTest;
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
                                 "SecondTest",
                                 "Score"
                            };
                            if (System.IO.File.Exists(pathNK))
                            {

                                // Read sample data from CSV file
                                ReaderCSV(pathNK, out learnedOld, out testedOld);


                                using (CsvFileWriter writer = new CsvFileWriter(pathNK))
                                {
                                    //Tạo tiêu đề
                                    CsvRow row = new CsvRow();
                                    for (int j = 0; j < title.Length; j++)
                                        row.Add(title[j]);
                                    writer.WriteRow(row);

                                    //searchResults fillin
                                    foreach (var s in searchResults)
                                    {
                                        var spluscode = s.SPlusCode.ToUpper();

                                        var input = changes.FirstOrDefault(x => x.LoginID.ToUpper() == spluscode);

                                        if (input != null)
                                        {
                                            s.IsLearned = input.IsLearned;
                                            s.Score = input.Score;
                                            s.SecondTest = input.SecondTest;
                                            s.ActivityCode = input.ActivityCode;
                                            learnedNew++;
                                            if (input.Score > 0)
                                            {
                                                testedNew++;
                                            }
                                        }
                                        //Add data to row
                                        row = new CsvRow();
                                        row.Add(s.SPlusCode);
                                        row.Add(s.Fullname);
                                        row.Add(s.Store);
                                        row.Add(s.Region);
                                        row.Add(s.ActivityCode);
                                        row.Add(s.IsLearned);
                                        row.Add(s.SecondTest.ToString());
                                        row.Add(s.Score.ToString());


                                        //add row to file
                                        writer.WriteRow(row);
                                    }
                                }
                                message = "Successfully uploaded. Have " + (learnedNew - learnedOld) + " learned and " + (testedNew - testedOld) + " tested today";
                            }
                        }//end readfile activity
                    }
                }

                ModelState.AddModelError("message", message);
                _db.Dayupdates.Add(new Dayupdate { Dateupdate = DateTime.Now });
                _db.SaveChanges();
                TempData["SSuccess"] = message;
                return View("Index");
                #endregion
            }
            catch (Exception e)
            {
                var mes = e;
                throw;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ResetData(string dealer)
        {
            string message;
            try
            {
                _db.ResetDealer(dealer);
                message = "Reset Data Thành công";
            }
            catch
            {
                message = "Reset Data Thất bại, thử lại.";
            }
            ModelState.AddModelError("message", message);
            TempData["DSuccess"] = message;

            return View("Index");

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

                        var datas = new List<DearlerFSMUpload>();
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
                                objStudent.Dealer = Convert.ToString(dtStudentRecords.Rows[i][6]);
                                datas.Add(objStudent);
                            }
                        }
                        _db.DearlerFSMUploads.AddRange(datas);
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
