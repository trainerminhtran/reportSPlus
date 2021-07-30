﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Splusreport.Models;
using Splusreport.ViewModel;
using Splusreport.Services;
using System.IO;
using System.Data;
namespace Splusreport.Controllers
{
    public class SearchController : ApiController
    {
        private readonly SPlusReportEntities _db = new SPlusReportEntities();
        private string path = System.Web.HttpContext.Current.Request.MapPath("~/Uploads/Data.csv");
        private string pathNK = System.Web.HttpContext.Current.Request.MapPath("~/Uploads/DataNK.csv");
        public List<SearchResult> FindByAccount(string account)
        {
            var ls = new List<SearchResult>();

            using (CsvFileReader reade = new CsvFileReader(path))
            {

                CsvRow row = new CsvRow();
                while (reade.ReadRow(row))
                {
                    if (account == "DMX")
                    {
                        if (row[0].Contains(account.ToUpper()) || row[0].Contains("TGDD") || row[0].Contains("dmx"))
                        {
                            var sr = new SearchResult
                            {
                                SPlusCode = row[0],
                                Fullname = row[1],
                                Store = row[2],
                                Region = row[3],
                                ActivityCode = row[4],
                                IsLearned = row[5],
                                SecondTest = Int32.Parse(row[6]),
                                Score = Int32.Parse(row[7]),
                                SecondLearn = Int32.Parse(row[8]),
                                TimesLearn = Int32.Parse(row[9]),
                                IsComplete = row[10]
                            };
                            ls.Add(sr);
                        }
                    }
                    else
                    {

                        if (row[0].Contains(account))
                        {
                            var sr = new SearchResult
                            {
                                SPlusCode = row[0],
                                Fullname = row[1],
                                Store = row[2],
                                Region = row[3],
                                ActivityCode = row[4],
                                IsLearned = row[5],
                                SecondTest = Int32.Parse(row[6]),
                                Score = Int32.Parse(row[7]),
                                SecondLearn = Int32.Parse(row[8]),
                                TimesLearn = Int32.Parse(row[9]),
                                IsComplete = row[10]
                            };
                            ls.Add(sr);
                        }
                    }

                }
            }
            return ls;
        }
        public List<SearchResultNK> FindByAccountNK()
        {
            var ls = new List<SearchResultNK>();

            using (CsvFileReader reade = new CsvFileReader(pathNK))
            {
                var i = 1;
                CsvRow row = new CsvRow();
                while (reade.ReadRow(row))
                {
                    if (i == 1)
                    {
                        i++;
                        continue;
                    }
                    var sr = new SearchResultNK
                    {
                        SPlusCode = row[0],
                        Fullname = row[1],
                        Store = row[2],
                        Region = row[3],
                        ActivityCode = row[4],
                        IsLearned = row[5],
                        SecondTest = Int32.Parse(row[6]),
                        Score = Int32.Parse(row[7])
                    };
                    ls.Add(sr);
                }
            }
            return ls;
        }

        [HttpGet]
        public IHttpActionResult SearchDMX()
        {

            var datas = new SearchModel();

            var SelectScoreDMX_Results = FindByAccount("DMX");

            var groups = SelectScoreDMX_Results.GroupBy(x => x.Store);

            var lsos = new List<StoreOrderView>();
            foreach (var items in groups)
            {
                var lso = new StoreOrderView
                {
                    TotalEmployee = 0,
                    AverageScore = 0,
                    TotalScore = 0,
                    TotalTested = 0,
                    TotalLearned = 0,
                    RateLearned = 0,
                    RateTested = 0,
                    SecondTest = 0,
                    Gold = 0,
                    TotalComplete = 0
                };
                lso.TotalEmployee = items.Count();
                foreach (var item in items)
                {
                    lso.StoreName = item.Store;
                    var itemscore = item.Score;
                    var itemSecond = item.SecondTest;
                    if (itemscore > 0)
                    {
                        lso.TotalSecond += itemSecond;
                        lso.TotalScore += itemscore;
                        lso.TotalTested++;
                    }
                    if (item.IsLearned == "Yes")
                    {
                        lso.TotalLearned++;
                    }
                    if (item.IsComplete == "Yes")
                    {
                        lso.TotalComplete++;
                    }
                }
                lso.AverageScore = (lso.TotalScore / lso.TotalEmployee);
                lso.AverageSecond = (lso.TotalSecond / lso.TotalEmployee);
                lso.RateTested = decimal.Round(((decimal)lso.TotalTested / (decimal)lso.TotalEmployee) * 100, 2, MidpointRounding.AwayFromZero);
                lso.Gold = lso.AverageScore + lso.RateTested;
                lso.RateLearned = decimal.Round(((decimal)lso.TotalLearned / (decimal)lso.TotalEmployee) * 100, 2, MidpointRounding.AwayFromZero);
                if (lso.TotalEmployee >= 5)
                {
                    lsos.Add(lso);
                }
            }
            lsos = lsos.OrderByDescending(o => o.Gold).ThenBy(x => x.AverageSecond).ThenByDescending(i => i.TotalEmployee).ToList();
            for (int i = 0; i < lsos.Count; i++)
            {
                lsos[i].Order = i + 1;
            }
            datas.Data = FindByAccount("DMX");
            datas.ListStoreOrder = lsos;

            var lastupdate = _db.Dayupdates.OrderByDescending(obj => obj.ID).FirstOrDefault();
            if (lastupdate == null)
            {
                datas.Dayupdate = DateTime.Now;
            }
            else
            {
                datas.Dayupdate = lastupdate.Dateupdate;
            }
            return Ok(datas);
        }

        [HttpPost]
        public IHttpActionResult SearchRegion()
        {

            var SelectScoreDMX_Results = FindByAccount("DMX");

            var groups = SelectScoreDMX_Results.GroupBy(x => x.Region);
            var lsos = new List<StoreOrderView>();
            foreach (var items in groups)
            {
                var lso = new StoreOrderView
                {
                    TotalEmployee = 0,
                    AverageScore = 0,
                    TotalScore = 0,
                    TotalTested = 0,
                    TotalLearned = 0,
                    RateLearned = 0,
                    TotalComplete =0
                };
                lso.TotalEmployee = items.Count();
                foreach (var item in items)
                {
                    lso.StoreName = item.Region;
                    var itemscore = item.Score;
                    if (itemscore > 0)
                    {
                        lso.TotalScore += itemscore;
                        lso.TotalTested++;
                    }
                    if (item.IsLearned == "Yes")
                    {
                        lso.TotalLearned++;
                    }
                    if (item.IsComplete == "Yes")
                    {
                        lso.TotalComplete++;
                    }
                }
                lso.RateTested = decimal.Round(((decimal)lso.TotalTested / (decimal)lso.TotalEmployee) * 100, 2, MidpointRounding.AwayFromZero);
                lso.RateLearned = decimal.Round(((decimal)lso.TotalLearned / (decimal)lso.TotalEmployee) * 100, 2, MidpointRounding.AwayFromZero);
                lsos.Add(lso);
            }
            lsos = lsos.OrderByDescending(o => o.AverageScore).ThenByDescending(i => i.RateTested).ThenByDescending(i => i.RateLearned).ThenByDescending(i => i.TotalEmployee).ToList();
            for (int i = 0; i < lsos.Count; i++)
            {
                lsos[i].Order = i + 1;
            }


            var ls = new StoreOrderView
            {
                StoreName = "Tổng cộng",
                TotalEmployee = lsos.Sum(x => x.TotalEmployee),
                RateTested = decimal.Round(((decimal)lsos.Sum(x => x.TotalTested) / (decimal)lsos.Sum(x => x.TotalEmployee)) * 100, 2, MidpointRounding.AwayFromZero),
                TotalLearned = lsos.Sum(x => x.TotalLearned),
                TotalScore = lsos.Sum(x => x.TotalScore),
                TotalTested = lsos.Sum(x => x.TotalTested),
                TotalComplete = lsos.Sum(x=>x.TotalComplete)
            };
            lsos.Add(ls);
            return Ok(lsos);
        }

        [HttpGet]
        public IHttpActionResult SearchMM()
        {
            var datas = new SearchModelMM();

            var SelectScoreMM_Results = FindByAccount("MM");

            var groups = SelectScoreMM_Results.GroupBy(x => x.Store);
            var lsos = new List<StoreOrderView>();
            foreach (var items in groups)
            {
                var lso = new StoreOrderView
                {
                    TotalEmployee = 0,
                    AverageScore = 0,
                    TotalScore = 0,
                    TotalTested = 0,
                    TotalLearned = 0,
                    RateLearned = 0,
                    RateTested = 0,
                    SecondTest = 0,
                    TotalComplete = 0
                };
                lso.TotalEmployee = items.Count();
                foreach (var item in items)
                {
                    lso.StoreName = item.Store;
                    var itemscore = item.Score;
                    var itemSecond = item.SecondTest;
                    if (itemscore > 0)
                    {
                        lso.TotalSecond += itemSecond;
                        lso.TotalScore += itemscore;
                        lso.TotalTested++;
                    }
                    if (item.IsLearned == "Yes")
                    {
                        lso.TotalLearned++;
                    }
                    if (item.IsComplete == "Yes")
                    {
                        lso.TotalComplete++;
                    }
                }
                lso.AverageScore = (lso.TotalScore / lso.TotalEmployee);
                lso.AverageSecond = (lso.TotalSecond / lso.TotalEmployee);
                lso.RateTested = decimal.Round(((decimal)lso.TotalTested / (decimal)lso.TotalEmployee) * 100, 2, MidpointRounding.AwayFromZero);
                lso.Gold = lso.AverageScore + lso.RateTested;
                lso.RateLearned = decimal.Round(((decimal)lso.TotalLearned / (decimal)lso.TotalEmployee) * 100, 2, MidpointRounding.AwayFromZero);
                if (lso.TotalEmployee >= 5)
                {
                    lsos.Add(lso);
                }
            }
            lsos = lsos.OrderByDescending(o => o.AverageScore).ThenByDescending(i => i.RateTested).ThenByDescending(i => i.RateLearned).ThenByDescending(i => i.TotalEmployee).ToList();
            for (int i = 0; i < lsos.Count; i++)
            {
                lsos[i].Order = i + 1;
            }
            datas.Data = FindByAccount("MM");
            datas.ListStoreOrder = lsos;
            datas.Dayupdate = _db.Dayupdates.OrderByDescending(x => x.ID).FirstOrDefault().Dateupdate;

            return Ok(datas);
        }

        [HttpGet]
        public IHttpActionResult SearchNK()
        {
            var datas = new SearchModelNK();

            var SelectScoreNK_Results = FindByAccountNK();

            var groups = SelectScoreNK_Results.GroupBy(x => x.Store);
            var lsos = new List<StoreOrderView>();
            foreach (var items in groups)
            {
                var lso = new StoreOrderView
                {
                    TotalEmployee = 0,
                    AverageScore = 0,
                    TotalScore = 0,
                    TotalTested = 0,
                    RateTested = 0,
                    SecondTest = 0,
                    Gold = 0
                };
                lso.TotalEmployee = items.Count();
                foreach (var item in items)
                {
                    lso.StoreName = item.Store;
                    var itemscore = item.Score;
                    var itemSecond = item.SecondTest;
                    if (itemscore > 0)
                    {
                        lso.TotalSecond += itemSecond;
                        lso.TotalScore += itemscore;
                        lso.TotalTested++;
                    }
                }
                lso.AverageScore = (lso.TotalScore / lso.TotalEmployee);
                lso.AverageSecond = (lso.TotalSecond / lso.TotalEmployee);
                lso.RateTested = decimal.Round(((decimal)lso.TotalTested / (decimal)lso.TotalEmployee) * 100, 2, MidpointRounding.AwayFromZero);
                if (lso.TotalEmployee >= 10)
                {
                    lso.Gold = lso.AverageScore + lso.RateTested;

                }
                else
                {
                    lso.Gold = 0;
                }

                if (lso.TotalEmployee >= 0)
                {
                    lsos.Add(lso);
                }
            }
            lsos = lsos.OrderByDescending(o => o.Gold).ThenBy(x => x.AverageSecond).ThenByDescending(i => i.TotalEmployee).ToList();
            for (int i = 0; i < lsos.Count; i++)
            {
                lsos[i].Order = i + 1;
            }
            datas.Data = FindByAccountNK();
            datas.ListStoreOrder = lsos;
            datas.Dayupdate = _db.Dayupdates.OrderByDescending(x => x.ID).FirstOrDefault().Dateupdate;

            return Ok(datas);
        }

        [HttpGet]
        public IHttpActionResult SearchVHC()
        {
            var datas = new SearchModelVHC();

            var SelectScoreVHC_Results = FindByAccount("VHC");

            var groups = SelectScoreVHC_Results.GroupBy(x => x.Store);
            var lsos = new List<StoreOrderView>();
            foreach (var items in groups)
            {
                var lso = new StoreOrderView
                {
                    TotalEmployee = 0,
                    AverageScore = 0,
                    TotalScore = 0,
                    TotalTested = 0,
                    TotalLearned = 0,
                    RateLearned = 0,
                    RateTested = 0,
                    SecondTest = 0,
                    TotalComplete = 0
                };
                lso.TotalEmployee = items.Count();
                foreach (var item in items)
                {
                    lso.StoreName = item.Store;
                    var itemscore = item.Score;
                    var itemSecond = item.SecondTest;
                    if (itemscore > 0)
                    {
                        lso.TotalSecond += itemSecond;
                        lso.TotalScore += itemscore;
                        lso.TotalTested++;
                    }
                    if (item.IsLearned == "Yes")
                    {
                        lso.TotalLearned++;
                    }
                    if (item.IsComplete == "Yes")
                    {
                        lso.TotalComplete++;
                    }
                }
                lso.AverageScore = (lso.TotalScore / lso.TotalEmployee);
                lso.AverageSecond = (lso.TotalSecond / lso.TotalEmployee);
                lso.RateTested = decimal.Round(((decimal)lso.TotalTested / (decimal)lso.TotalEmployee) * 100, 2, MidpointRounding.AwayFromZero);
                lso.Gold = lso.AverageScore + lso.RateTested;
                lso.RateLearned = decimal.Round(((decimal)lso.TotalLearned / (decimal)lso.TotalEmployee) * 100, 2, MidpointRounding.AwayFromZero);
                if (lso.TotalEmployee >= 5)
                {
                    lsos.Add(lso);
                }
            }
            lsos = lsos.OrderByDescending(o => o.AverageScore).ThenByDescending(i => i.RateTested).ThenByDescending(i => i.RateLearned).ThenByDescending(i => i.TotalEmployee).ToList();
            for (int i = 0; i < lsos.Count; i++)
            {
                lsos[i].Order = i + 1;
            }
            datas.Data = FindByAccount("VHC");
            datas.ListStoreOrder = lsos;
            datas.Dayupdate = _db.Dayupdates.OrderByDescending(x => x.ID).FirstOrDefault().Dateupdate;

            return Ok(datas);
        }
        [HttpGet]
        public IHttpActionResult SearchPICO()
        {
            var datas = new SearchModelPICO();

            var SelectScorePICO_Results = FindByAccount("PICO");

            var groups = SelectScorePICO_Results.GroupBy(x => x.Store);
            var lsos = new List<StoreOrderView>();
            foreach (var items in groups)
            {
                var lso = new StoreOrderView
                {
                    TotalEmployee = 0,
                    AverageScore = 0,
                    TotalScore = 0,
                    TotalTested = 0,
                    TotalLearned = 0,
                    RateLearned = 0,
                    RateTested = 0,
                    SecondTest = 0,
                    TotalComplete = 0
                };
                lso.TotalEmployee = items.Count();
                foreach (var item in items)
                {
                    lso.StoreName = item.Store;
                    var itemscore = item.Score;
                    var itemSecond = item.SecondTest;
                    if (itemscore > 0)
                    {
                        lso.TotalSecond += itemSecond;
                        lso.TotalScore += itemscore;
                        lso.TotalTested++;
                    }
                    if (item.IsLearned == "Yes")
                    {
                        lso.TotalLearned++;
                    }
                    if (item.IsComplete == "Yes")
                    {
                        lso.TotalComplete++;
                    }
                }
                lso.AverageScore = (lso.TotalScore / lso.TotalEmployee);
                lso.AverageSecond = (lso.TotalSecond / lso.TotalEmployee);
                lso.RateTested = decimal.Round(((decimal)lso.TotalTested / (decimal)lso.TotalEmployee) * 100, 2, MidpointRounding.AwayFromZero);
                lso.Gold = lso.AverageScore + lso.RateTested;
                lso.RateLearned = decimal.Round(((decimal)lso.TotalLearned / (decimal)lso.TotalEmployee) * 100, 2, MidpointRounding.AwayFromZero);
                if (lso.TotalEmployee >= 5)
                {
                    lsos.Add(lso);
                }
            }
            lsos = lsos.OrderByDescending(o => o.AverageScore).ThenByDescending(i => i.RateTested).ThenByDescending(i => i.RateLearned).ThenByDescending(i => i.TotalEmployee).ToList();
            for (int i = 0; i < lsos.Count; i++)
            {
                lsos[i].Order = i + 1;
            }
            datas.Data = FindByAccount("PICO");
            datas.ListStoreOrder = lsos;
            datas.Dayupdate = _db.Dayupdates.OrderByDescending(x => x.ID).FirstOrDefault().Dateupdate;

            return Ok(datas);
        }
    }
}
