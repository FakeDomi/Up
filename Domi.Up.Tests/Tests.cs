using Domi.UpCore.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Domi.Up.Tests
{
    [TestClass]
    public class Tests
    {
        [TestMethod]
        public void TestHttpGetRange()
        {
            Test("bytes=50-150", 50, 150, 10000);
            Test("bytes=-500", 9500, 9999, 10000);
            Test("bytes=-1", 9999, 9999, 10000);
            Test("bytes=0-", 0, 9999, 10000);
            Test("bytes=5000-15000", 5000, 9999, 10000);
            TestFail("bytes=-15000", 10000);
            TestFail("bytes=12000-", 10000);
            Test("bytes=13000-5000", 5000, 9999, 10000);

            void Test(string range, long startExpected, long endExpected, long fileSize)
            {
                Assert.IsTrue(Http.GetRange(range, out long start, out long end, fileSize));
                Assert.AreEqual(startExpected, start);
                Assert.AreEqual(endExpected, end);
            }

            void TestFail(string range, long fileSize)
            {
                Assert.IsFalse(Http.GetRange(range, out long start, out long end, fileSize));
            }
        }
    }
}
