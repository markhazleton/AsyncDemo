namespace AsyncSpark.Tests.Models
{
    [TestClass]
    public class MockResultsTests
    {
        [TestMethod]
        public void TestMethod1()
        {
            // Arrange
            var testResults = new MockResults(1, 99)
            {
                // Act
                RunTimeMS = 99,
                Message = "Message",
                ResultValue = "ResultValue"
            };

            // Assert
            Assert.IsNotNull(testResults);
        }
    }
}
