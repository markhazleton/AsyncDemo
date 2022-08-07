namespace AsyncDemo.Tests.Models
{
    [TestClass]
    public class MockResultsTests
    {
        [TestMethod]
        public void TestMethod1()
        {
            // Arrange
            var testResults = new MockResults();

            // Act
            testResults.LoopCount = 1;
            testResults.MaxTimeMS = 99;
            testResults.RunTimeMS = 99;
            testResults.Message = "Message";
            testResults.ResultValue = "ResultValue";

            // Assert
            Assert.IsNotNull(testResults);
        }
    }
}
