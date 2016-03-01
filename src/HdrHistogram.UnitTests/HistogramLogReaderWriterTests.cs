using System;
using System.IO;
using System.Linq;
using HdrHistogram.Utilities;
using NUnit.Framework;

namespace HdrHistogram.UnitTests
{
    [TestFixture]
    public class HistogramLogReaderWriterTests
    {
        [Test]
        public void CanReadEmptyLog()
        {
            byte[] data;
            var startTimeWritten = DateTime.Now;
            var expectedStartTime = startTimeWritten.SecondsSinceUnixEpoch()
                .Round(3)
                .ToDateFromSecondsSinceEpoch();

            using (var writerStream = new MemoryStream())
            {
                var writer = new HistogramLogWriter(writerStream);
                writer.Write(startTimeWritten);
                data = writerStream.ToArray();
            }

            using (var readerStream = new MemoryStream(data))
            {
                var reader = new HistogramLogReader(readerStream);
                var histograms = reader.ReadHistograms();
                CollectionAssert.IsEmpty(histograms.ToList());
                var actual = reader.GetStartTime();
                Assert.AreEqual(expectedStartTime, actual);
            }
        }

        [TestCase("Resources\\jHiccup-2.0.7S.logV2.hlog")]
        [TestCase("Resources\\jHiccup-2.0.7S.logV2.hlog")]
        public void CanReadv2Logs(string logPath)
        {
            var readerStream = File.OpenRead(logPath);
            HistogramLogReader reader = new HistogramLogReader(readerStream);
            int histogramCount = 0;
            long totalCount = 0;
            HistogramBase encodeableHistogram = null;
            var accumulatedHistogram = new LongHistogram(85899345920838, 3);
            foreach (var histogram in reader.ReadHistograms())
            {
                histogramCount++;
                Assert.IsInstanceOf<HistogramBase>(histogram, "Expected integer value histograms in log file");

                totalCount += histogram.TotalCount;
                accumulatedHistogram.Add(histogram);
            }

            Assert.AreEqual(62, histogramCount);
            Assert.AreEqual(48761, totalCount);
            Assert.AreEqual(1745879039, accumulatedHistogram.GetValueAtPercentile(99.9));
            Assert.AreEqual(1796210687, accumulatedHistogram.GetMaxValue());
            Assert.AreEqual(1441812279.474, reader.GetStartTime().SecondsSinceUnixEpoch());
        }
    }
}