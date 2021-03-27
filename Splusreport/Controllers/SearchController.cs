using System;
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
        public List<SearchResult> FindByAccount(string account)
        {
            var ls = new List<SearchResult>();
            using (CsvFileReader reade = new CsvFileReader(path))
            {
                
                CsvRow row = new CsvRow();
                while (reade.ReadRow(row))
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
                            Score = Int32.Parse(row[6])
                        };
                        ls.Add(sr);
                    }
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
                    RateTested = 0,
                    Gold = 0
                };
                lso.TotalEmployee = items.Count();
                foreach (var item in items)
                {
                    lso.StoreName = item.Store;
                    var itemscore = item.Score;
                    if (itemscore > 0)
                    {
                        lso.TotalScore += itemscore;
                        lso.TotalTested++;
                    }
                }
                lso.AverageScore = (lso.TotalScore / lso.TotalEmployee);
                lso.RateTested = decimal.Round(((decimal)lso.TotalTested / (decimal)lso.TotalEmployee) * 100, 2, MidpointRounding.AwayFromZero);
                lso.Gold = lso.AverageScore + lso.RateTested;

                if (lso.TotalEmployee >= 5)
                {
                    lsos.Add(lso);
                }
            }
            lsos = lsos.OrderByDescending(o => o.Gold).ThenByDescending(i => i.TotalEmployee).ToList();
            for (int i = 0; i < lsos.Count; i++)
            {
                lsos[i].Order = i + 1;
            }
            datas.Data = _db.SelectScoreDMX().ToList();
            datas.ListStoreOrder = lsos;
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
                    TotalTested = 0
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
                }
                lso.RateTested = decimal.Round(((decimal)lso.TotalTested / (decimal)lso.TotalEmployee) * 100, 2, MidpointRounding.AwayFromZero);
                lsos.Add(lso);
            }
            lsos = lsos.OrderByDescending(o => o.AverageScore).ThenByDescending(i => i.TotalEmployee).ToList();
            for (int i = 0; i < lsos.Count; i++)
            {
                lsos[i].Order = i + 1;
            }


            var ls = new StoreOrderView
            {
                StoreName = "Tổng cộng",
                TotalEmployee = lsos.Sum(x => x.TotalEmployee),
                RateTested = decimal.Round(((decimal)lsos.Sum(x => x.TotalTested) / (decimal)lsos.Sum(x => x.TotalEmployee)) * 100, 2, MidpointRounding.AwayFromZero),
                TotalScore = lsos.Sum(x => x.TotalScore),
                TotalTested = lsos.Sum(x => x.TotalTested)
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
                    TotalTested = 0
                };
                lso.TotalEmployee = items.Count();
                foreach (var item in items)
                {
                    lso.StoreName = item.Store;
                    var itemscore = item.Score;
                    if (itemscore > 0)
                    {
                        lso.TotalScore += itemscore;
                        lso.TotalTested++;
                    }
                }
                lso.AverageScore = (lso.TotalScore / lso.TotalEmployee);
                if (lso.TotalEmployee >= 5)
                {
                    lsos.Add(lso);
                }
            }

            lsos = lsos.OrderByDescending(o => o.AverageScore).ThenByDescending(i => i.TotalEmployee).ToList();
            for (int i = 0; i < lsos.Count; i++)
            {
                lsos[i].Order = i + 1;
            }
            datas.Data = _db.SelectScoreMM().ToList();
            datas.ListStoreOrder = lsos;
            return Ok(datas);
        }

        [HttpGet]
        public IHttpActionResult SearchNK()
        {
            var datas = new SearchModelNK();

            var SelectScoreNK_Results = FindByAccount("NK");

            var groups = SelectScoreNK_Results.GroupBy(x => x.Store);
            var lsos = new List<StoreOrderView>();
            foreach (var items in groups)
            {
                var lso = new StoreOrderView
                {
                    TotalEmployee = 0,
                    AverageScore = 0,
                    TotalScore = 0,
                    TotalTested = 0
                };
                lso.TotalEmployee = items.Count();
                foreach (var item in items)
                {
                    lso.StoreName = item.Store;
                    var itemscore = item.Score;
                    if (itemscore > 0)
                    {
                        lso.TotalScore += itemscore;
                        lso.TotalTested++;
                    }
                }
                lso.AverageScore = (lso.TotalScore / lso.TotalEmployee);
                if (lso.TotalEmployee >= 5)
                {
                    lsos.Add(lso);
                }
            }
            lsos = lsos.OrderByDescending(o => o.AverageScore).ThenByDescending(i => i.TotalEmployee).ToList();
            for (int i = 0; i < lsos.Count; i++)
            {
                lsos[i].Order = i + 1;
            }
            datas.Data = _db.SelectScoreNK().ToList();
            datas.ListStoreOrder = lsos;
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
                    TotalTested = 0
                };
                lso.TotalEmployee = items.Count();
                foreach (var item in items)
                {
                    lso.StoreName = item.Store;
                    var itemscore = item.Score;
                    if (itemscore > 0)
                    {
                        lso.TotalScore += itemscore;
                        lso.TotalTested++;
                    }
                }
                lso.AverageScore = (lso.TotalScore / lso.TotalEmployee);
                if (lso.TotalEmployee >= 5)
                {
                    lsos.Add(lso);
                }
            }
            lsos = lsos.OrderByDescending(o => o.AverageScore).ThenByDescending(i => i.TotalEmployee).ToList();
            for (int i = 0; i < lsos.Count; i++)
            {
                lsos[i].Order = i + 1;
            }
            datas.Data = _db.SelectScoreVHC().ToList();
            datas.ListStoreOrder = lsos;
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
                    TotalTested = 0
                };
                lso.TotalEmployee = items.Count();
                foreach (var item in items)
                {
                    lso.StoreName = item.Store;
                    var itemscore = item.Score;
                    if (itemscore > 0)
                    {
                        lso.TotalScore += itemscore;
                        lso.TotalTested++;
                    }
                }
                lso.AverageScore = (lso.TotalScore / lso.TotalEmployee);
                if (lso.TotalEmployee >= 5)
                {
                    lsos.Add(lso);
                }
            }
            lsos = lsos.OrderByDescending(o => o.AverageScore).ThenByDescending(i => i.TotalEmployee).ToList();
            for (int i = 0; i < lsos.Count; i++)
            {
                lsos[i].Order = i + 1;
            }
            datas.Data = _db.SelectScorePICO().ToList();
            datas.ListStoreOrder = lsos;
            return Ok(datas);
        }
    }
}
