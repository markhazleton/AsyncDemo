﻿
namespace AsyncDemo.Models;
/// <summary>
/// Mock Results
/// </summary>
public class MockResults
{
    /// <summary>
    /// Loop Count (number of iterations of work to perform)
    /// </summary>
    public int LoopCount { get; set; }

    /// <summary>
    /// Max Time for completing all iterations
    /// </summary>
    public int MaxTimeMS { get; set; }

    /// <summary>
    /// Actual Runtime to complete the requested loops (iterations)
    /// </summary>
    public long? RunTimeMS { get; set; }

    /// <summary>
    /// Return Message from calling for results
    /// </summary>
    public string? Message { get; set; }

    /// <summary>
    /// Return Value from calling for results
    /// </summary>
    public string? ResultValue { get; set; }
}
