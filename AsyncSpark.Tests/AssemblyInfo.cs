using Microsoft.VisualStudio.TestTools.UnitTesting;

// Configure test parallelization - disable for async tests to avoid timing issues
[assembly: Parallelize(Workers = 0, Scope = ExecutionScope.MethodLevel)]
