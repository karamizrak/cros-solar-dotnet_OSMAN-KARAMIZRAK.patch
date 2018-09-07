using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CrossSolar.Domain;
using CrossSolar.Models;
using CrossSolar.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CrossSolar.Controllers
{
    public class AnalyticsController : Controller
    {
        private readonly IAnalyticsRepository _analyticsRepository;

        private readonly IPanelRepository _panelRepository;

        private Helper helper;
        public AnalyticsController(IAnalyticsRepository analyticsRepository, IPanelRepository panelRepository)
        {
            _analyticsRepository = analyticsRepository;
            _panelRepository = panelRepository;

            helper = new Helper(_analyticsRepository,_panelRepository);
        }

        // GET panel/XXXX1111YYYY2222/analytics
        [HttpGet("api/{serialNo}/[controller]")]
        public async Task<IActionResult> Get([FromRoute] string serialNo)
        {
            var panel=await helper.GetPanelWithSerialNo(serialNo);

            if (panel == null) return NotFound();

            
            var analytics = await _analyticsRepository.Query()
                .Where(x => x.PanelId== panel.Id).ToListAsync();

            var result = new OneHourElectricityListModel
            {
                OneHourElectricitys = analytics.Select(c => new OneHourElectricityModel
                {
                    Id = c.Id,
                    KiloWatt = c.KiloWatt,
                    DateTime = c.DateTime
                })
            };

            return Ok(result);
        }


        // GET panel/XXXX1111YYYY2222/analytics/startDay/endDay
        [HttpGet("api/panel/{serialNo}/[controller]/{startDay}/{endDay}")]
        public async Task<IActionResult> DayResults([FromRoute] string serialNo, [FromRoute] string startDay, [FromRoute] string endDay)
        {
            var result = new List<OneDayElectricityModel>();
            /*Check Date*/
            var startDayT = new DateTime();
            if (!DateTime.TryParse(startDay, out startDayT))
            {
                return NotFound("startDay is not date");
            }

            var endDayT = new DateTime();
            if (!DateTime.TryParse(endDay, out endDayT))
            {
                return NotFound("endDay is not date");
            }
            /*Check Date*/

            var listData = await helper.GetPanelElectricityInfoAsync(serialNo, startDayT, endDayT);
            
            var  gropedList= listData.GroupBy(x => x.DateTime);
            foreach (var grouping in gropedList)
            {
                var tmp = grouping.FirstOrDefault();
                result.Add(new OneDayElectricityModel()
                {
                    Sum = grouping.Sum(x => x.KiloWatt),
                    DateTime = tmp.DateTime.Date,
                    Average= grouping.Average(x => x.KiloWatt),
                    Maximum= grouping.Max(x => x.KiloWatt),
                    Minimum= grouping.Min(x => x.KiloWatt)
                });
            }
            return Ok(result);
        }

        // POST panel/XXXX1111YYYY2222/analytics
        [HttpPost("api/panel/{serialNo}/[controller]")]
        public async Task<IActionResult> Post([FromRoute] string serialNo, [FromBody] OneHourElectricityModel value)
        {
            var panel = await helper.GetPanelWithSerialNo(serialNo);

            if (!ModelState.IsValid) return BadRequest(ModelState);

            var oneHourElectricityContent = new OneHourElectricity
            {
                PanelId = panel.Id,
                KiloWatt = value.KiloWatt,
                DateTime = DateTime.UtcNow
            };

            await _analyticsRepository.InsertAsync(oneHourElectricityContent);

            var result = new OneHourElectricityModel
            {
                Id = oneHourElectricityContent.Id,
                KiloWatt = oneHourElectricityContent.KiloWatt,
                DateTime = oneHourElectricityContent.DateTime
            };

            return Created($"api/panel/{panel.Id}/analytics/{result.Id}", result);
        }
    }
}