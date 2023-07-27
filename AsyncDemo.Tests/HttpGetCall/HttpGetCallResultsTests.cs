using AsyncDemo.HttpGetCall;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AsyncDemo.Tests.HttpGetCall;

[TestClass]
public class HttpGetCallResultsTests
{
    [TestMethod]
    public void TestHttpGetCallResultsConstructorWithNoArgs()
    {
        // Arrange
        var httpGetCallResults = new HttpGetCallResults();

        // Act

        // Assert
        Assert.AreEqual(0, httpGetCallResults.Iteration);
        Assert.AreEqual(string.Empty, httpGetCallResults.StatusPath);
        Assert.IsNull(httpGetCallResults.CompletionDate);
        Assert.AreEqual(0, httpGetCallResults.ElapsedMilliseconds);
        Assert.IsNull(httpGetCallResults.StatusResults);
    }

    [TestMethod]
    public void TestHttpGetCallResultsConstructorWithHttpGetCallResultsArg()
    {
        // Arrange
        var statusCall = new HttpGetCallResults
        {
            Iteration = 42,
            StatusPath = "/example"
        };

        // Act
        var httpGetCallResults = new HttpGetCallResults(statusCall);

        // Assert
        Assert.AreEqual(statusCall.Iteration, httpGetCallResults.Iteration);
        Assert.AreEqual(statusCall.StatusPath, httpGetCallResults.StatusPath);
        Assert.IsNull(httpGetCallResults.CompletionDate);
        Assert.AreEqual(0, httpGetCallResults.ElapsedMilliseconds);
        Assert.IsNull(httpGetCallResults.StatusResults);
    }

    [TestMethod]
    public void TestHttpGetCallResultsConstructorWithArgs()
    {
        // Arrange
        int iteration = 5;
        string statusPath = "/test";

        // Act
        var httpGetCallResults = new HttpGetCallResults(iteration, statusPath);

        // Assert
        Assert.AreEqual(iteration, httpGetCallResults.Iteration);
        Assert.AreEqual(statusPath, httpGetCallResults.StatusPath);
        Assert.IsNull(httpGetCallResults.CompletionDate);
        Assert.AreEqual(0, httpGetCallResults.ElapsedMilliseconds);
        Assert.IsNull(httpGetCallResults.StatusResults);
    }


}
