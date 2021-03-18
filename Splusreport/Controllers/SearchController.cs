using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Splusreport.Models;
using Splusreport.ViewModel;

namespace Splusreport.Controllers
{
    public class SearchController : ApiController
    {
        private readonly SPlusReportEntities _db = new SPlusReportEntities();
        
        
        [HttpGet]
        public IHttpActionResult SearchDMX()
        {
            var datas = new SearchModel();

            var SelectScoreDMX_Results = _db.SelectScoreDMX();

            var groups = SelectScoreDMX_Results.GroupBy(x => x.Store);
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
                    var itemscore = item.Score.GetValueOrDefault();
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
            lsos = lsos.OrderByDescending(o => o.AverageScore).ThenByDescending(i=>i.TotalEmployee).ToList();
            for (int i = 0; i < lsos.Count; i++)
            {
                lsos[i].Order = i+1;
            }
            datas.Data = _db.SelectScoreDMX().ToList();
            datas.ListStoreOrder = lsos;
            return Ok(datas);
        }
        
        [HttpPost]
        public IHttpActionResult SearchRegion()
        {

            var SelectScoreDMX_Results = _db.SelectScoreDMX();

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
                    var itemscore = item.Score.GetValueOrDefault();
                    if (itemscore > 0)
                    {
                        lso.TotalScore += itemscore;
                        lso.TotalTested++;
                    }
                }
                lso.AverageScore = lso.TotalScore / lso.TotalEmployee;
                lsos.Add(lso);
            }
            lsos = lsos.OrderByDescending(o => o.AverageScore).ThenByDescending(i => i.TotalEmployee).ToList();
            for (int i = 0; i < lsos.Count; i++)
            {
                lsos[i].Order = i+1;
            }

            var ls = new StoreOrderView
            {
                StoreName = "Total",
                TotalEmployee = lsos.Sum(x=>x.TotalEmployee),
                AverageScore = lsos.Sum(x => x.TotalScore)/ lsos.Sum(x => x.TotalEmployee),
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

            var SelectScoreMM_Results = _db.SelectScoreMM();

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
                    var itemscore = item.Score.GetValueOrDefault();
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

            var SelectScoreNK_Results = _db.SelectScoreNK();

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
                    var itemscore = item.Score.GetValueOrDefault();
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

            var SelectScoreVHC_Results = _db.SelectScoreVHC();

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
                    var itemscore = item.Score.GetValueOrDefault();
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

            var SelectScorePICO_Results = _db.SelectScorePICO();

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
                    var itemscore = item.Score.GetValueOrDefault();
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
