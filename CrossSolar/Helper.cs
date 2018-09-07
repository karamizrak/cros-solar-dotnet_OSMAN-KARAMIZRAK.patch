using CrossSolar.Domain;
using CrossSolar.Repository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CrossSolar
{
    public class Helper
    {
        private readonly IAnalyticsRepository _analyticsRepository;

        private readonly IPanelRepository _panelRepository;
        public Helper(IAnalyticsRepository analyticsRepository, IPanelRepository panelRepository)
        {
            _analyticsRepository = analyticsRepository;
            _panelRepository = panelRepository;
        }

        public async Task<Panel> GetPanelWithSerialNo(string serialNo)
        {
            return await _panelRepository.Query()
              .FirstOrDefaultAsync(x => x.Serial.Equals(serialNo, StringComparison.CurrentCultureIgnoreCase));

        }

        public async Task<Panel> GetPanelWithPanelId(int panelId)
        {
            return await _panelRepository.Query()
              .FirstOrDefaultAsync(x => x.Id == panelId);

        }

        public async Task<List<Domain.OneHourElectricity>> GetPanelElectricityInfoAsync(string serialNo, DateTime startDate, DateTime endDate)
        {
            var panel = await GetPanelWithSerialNo(serialNo);

            var liste = _analyticsRepository.Query().Where(x => x.PanelId == panel.Id && startDate <= x.DateTime && endDate.AddDays(1).AddMinutes(-1) > x.DateTime).ToList();

            return liste;

        }
    }
}
