using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using SudokuCollective.Core.Interfaces.Models.DomainObjects.Params;
using SudokuCollective.Core.Interfaces.Models.DomainObjects.Settings;
using SudokuCollective.Core.Interfaces.Services;
using SudokuCollective.Data.Models;
using SudokuCollective.Data.Models.Settings;
using SudokuCollective.Test.TestData;

namespace SudokuCollective.Test.Services
{
    public class MockedSettingsService
    {
        private MockedDifficultiesService MockedDifficultiesService { get; set; }
        private Settings _settings { get; set; }
        private List<EnumListItem> _releaseEnvironments { get; set; }
        private List<EnumListItem> _sortValues { get; set; }
        private List<EnumListItem> _timeFrames { get; set; }

        internal Mock<ISettingsService> Request { get; set; }

        public MockedSettingsService(DatabaseContext context)
        {
            Request = new Mock<ISettingsService>();

            _settings = TestObjects.GetSettings();
            _releaseEnvironments = _settings.ReleaseEnvironments;
            _sortValues = _settings.SortValues;
            _timeFrames = _settings.TimeFrames;

            Request.Setup(service =>
                service.GetAsync())
                .Returns(Task.FromResult(TestObjects.GetResult() as IResult));

            Request.Setup(service =>
                service.GetReleaseEnvironments())
                .Returns(_releaseEnvironments.ConvertAll(x => (IEnumListItem)x));

            Request.Setup(service =>
                service.GetSortValues())
                .Returns(_sortValues.ConvertAll(x => (IEnumListItem)x));

            Request.Setup(service =>
                service.GetTimeFrames())
                .Returns(_timeFrames.ConvertAll(x => (IEnumListItem)x));
        }
    }
}
