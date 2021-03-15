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
                lsos.Add(lso);
            }
            lsos = lsos.OrderByDescending(o => o.AverageScore).ToList();
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
            lsos = lsos.OrderByDescending(o => o.AverageScore).ToList();
            for (int i = 0; i < lsos.Count; i++)
            {
                lsos[i].Order = i+1;
            }
            return Ok(lsos);
        }
    }
}
