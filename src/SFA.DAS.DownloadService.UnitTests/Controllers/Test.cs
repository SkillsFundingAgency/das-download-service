using NUnit.Framework;
using SFA.DAS.DownloadService.Api.Types.Roatp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SFA.DAS.DownloadService.UnitTests.Controllers
{
    [TestFixture]
    public class NewTest
    {
        [Test]
        public void Test()
        {
            // Your existing lists
            List<RoatpResult> roatpResults = GetRoatpResults();
            List<EPAOResult> epaoResults = GetEpaoResults();

            // Combine two lists into one and group by Ukprn
            var combinedResults = roatpResults.Select(r => new { Ukprn = r.Ukprn, Roatp = r, Epao = (EPAOResult)null })
                            .Concat(epaoResults.Select(e => new { Ukprn = e.Ukprn, Roatp = (RoatpResult)null, Epao = e }))
                            .GroupBy(x => x.Ukprn)
                            .Select(g => new CombinedResult
                            {
                                Roatp = g.FirstOrDefault(x => x.Roatp != null)?.Roatp,
                                Epao = g.FirstOrDefault(x => x.Epao != null)?.Epao
                            })
                            .ToList();

            Assert.That(true);

        }

        public List<RoatpResult> GetRoatpResults()
        {
            return new List<RoatpResult>
            {
                new RoatpResult { Ukprn = "12345678" },
                new RoatpResult { Ukprn = "11111111" }
            };
        }

        public List<EPAOResult> GetEpaoResults()
        {
            return new List<EPAOResult> 
            { 
                new EPAOResult { Ukprn = "12345678" },
                new EPAOResult { Ukprn = "22222222" }
            };
        }

        public class EPAOResult
        {
            public string Ukprn { get; set; }
        }

        public class CombinedResult
        { 
            public RoatpResult Roatp { get; set; }
            public EPAOResult Epao { get; set; }
        }
    }
}
