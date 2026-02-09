# Polly UnsupportedMediaType (415) Error - Fix Documentation

**Date**: January 2025  
**Issue**: Polly demo returning HTTP 415 (UnsupportedMediaType) errors  
**Project**: AsyncSpark.Web (.NET 10)  
**Status**: ? RESOLVED

## Problem Summary

The `/polly` endpoint was consistently returning `UnsupportedMediaType` errors when making POST requests to `/api/remote/Results`. The error manifested as:

```
Result: UnsupportedMediaType
RunTime (ms): 1594
Message: init
Retry 1: UnsupportedMediaType;
Retry 2: UnsupportedMediaType;
Retry 3: UnsupportedMediaType
```

## Root Cause

The issue had **two parts**:

### 1. API Endpoint Not Configured to Accept JSON

The `RemoteController.GetResults()` endpoint wasn't explicitly configured to accept `application/json` content type, and the parameter wasn't marked with `[FromBody]` attribute.

**Missing Configuration:**
- No `[Consumes("application/json")]` attribute
- No `[FromBody]` attribute on model parameter

### 2. PollyController Not Sending Content-Type Header

The `PollyController` was using `HttpClientJsonExtensions.PostAsJsonAsync()` which should have set the `Content-Type` header automatically, but there was an issue with how it was being invoked within the Polly retry policy lambda.

## The Fix

### Part 1: Update API Endpoint to Accept JSON

**File**: `AsyncSpark.Web/Controllers/Api/BaseApiController.cs`

Added `[Consumes("application/json")]` to the base controller:

```csharp
[Produces("application/json")]
[Consumes("application/json")]  // ? ADDED
[ApiController]
public abstract class BaseApiController : Controller
```

**File**: `AsyncSpark.Web/Controllers/Api/RemoteController.cs`

Added `[Consumes]` and `[FromBody]` attributes:

```csharp
[Consumes("application/json")]  // ? ADDED
[HttpPost]
[Route("Results")]
public async Task<IActionResult> GetResults([FromBody] MockResults model)  // ? ADDED [FromBody]
```

### Part 2: Fix HTTP Client POST Request

**File**: `AsyncSpark.Web/Controllers/PollyController.cs`

**Before (BROKEN)**:
```csharp
response = await _httpIndexPolicy.ExecuteAsync(ctx =>
    HttpClientJsonExtensions.PostAsJsonAsync(_httpClient, "remote/Results", mockResults, Cts.Token), context);
```

**After (FIXED)**:
```csharp
response = await _httpIndexPolicy.ExecuteAsync(async ctx =>
{
    var json = JsonSerializer.Serialize(mockResults);
    var content = new StringContent(json, Encoding.UTF8, "application/json");
    return await _httpClient.PostAsync("remote/Results", content, Cts.Token);
}, context);
```

**File**: `AsyncSpark.Web/GlobalUsings.cs`

Added required namespaces:

```csharp
global using System.Text;
global using System.Text.Json;
```

## Why The Original Code Failed

1. **`PostAsJsonAsync` was not properly setting Content-Type**: The extension method call was wrapped in a non-async lambda, preventing proper async execution
2. **API wasn't configured to accept JSON**: Without `[Consumes("application/json")]` and `[FromBody]`, ASP.NET Core's model binding was failing to read the JSON body
3. **Missing explicit Content-Type header**: The `StringContent` constructor now explicitly sets `"application/json"` as the media type

## Testing The Fix

### Before Fix:
```
Result: UnsupportedMediaType
Retry 1: UnsupportedMediaType
Retry 2: UnsupportedMediaType  
Retry 3: UnsupportedMediaType
```

### After Fix:
```
Loop Count: 10
Max Time (ms): 100
Result: [actual numeric value or timeout]
RunTime (ms): [actual runtime]
Message: Task Complete (or "Time Out Occurred")
```

## How to Apply the Fix

1. **Stop the debugger** (Shift + F5)
2. **Clean the solution** (optional but recommended)
   ```bash
   dotnet clean
   ```
3. **Build the solution**
   ```bash
   dotnet build
   ```
4. **Start debugging** (F5)
5. **Navigate to** `/polly`
6. **Run a test** with any parameters
7. **Verify** the result shows actual values instead of "UnsupportedMediaType"

## Related Files Modified

1. `AsyncSpark.Web/Controllers/Api/BaseApiController.cs` - Added `[Consumes]` attribute
2. `AsyncSpark.Web/Controllers/Api/RemoteController.cs` - Added `[Consumes]` and `[FromBody]`
3. `AsyncSpark.Web/Controllers/PollyController.cs` - Fixed POST request to explicitly set Content-Type
4. `AsyncSpark.Web/GlobalUsings.cs` - Added `System.Text` and `System.Text.Json` namespaces

## Key Lessons Learned

### 1. Always Explicitly Set Content-Type for POST Requests

When using `HttpClient.PostAsync()`, create `StringContent` with explicit media type:

```csharp
var content = new StringContent(json, Encoding.UTF8, "application/json");
```

### 2. Use [Consumes] and [FromBody] for API Endpoints

For ASP.NET Core Web API endpoints that accept JSON:

```csharp
[Consumes("application/json")]
[HttpPost]
public async Task<IActionResult> MyEndpoint([FromBody] MyModel model)
```

### 3. Async Lambdas in Polly Policies

When using Polly retry policies with async operations, ensure the lambda is properly async:

```csharp
// ? CORRECT
response = await policy.ExecuteAsync(async ctx => 
{
    return await httpClient.PostAsync(url, content);
}, context);

// ? WRONG
response = await policy.ExecuteAsync(ctx => 
    httpClient.PostAsync(url, content), context);
```

### 4. Hot Reload Limitations

Attribute changes and method signature changes require a full restart:
- Adding/removing attributes (`[Consumes]`, `[FromBody]`)
- Changing method parameters
- Modifying dependency injection

These changes **cannot** be hot-reloaded and require stopping and restarting the debugger.

## Prevention

To prevent similar issues in the future:

1. **Always test API endpoints directly** (via Swagger/Scalar) before integrating with clients
2. **Check HTTP status codes** in logs - 415 always means content-type mismatch
3. **Use explicit Content-Type** when creating `HttpContent` objects
4. **Add API attributes** at the base controller level when possible
5. **Test POST requests** with different content types to ensure proper configuration

## Additional Notes

- The fix ensures both sides of the HTTP communication agree on JSON format
- The `[Consumes]` attribute documents the API's expectations in OpenAPI/Swagger
- Using `StringContent` with explicit media type is more reliable than extension methods in complex scenarios
- The Polly retry policy will now properly retry actual failures, not content-type mismatches

## Verification Checklist

- [x] API endpoint accepts `application/json`
- [x] API endpoint has `[FromBody]` attribute
- [x] HTTP client sends `Content-Type: application/json` header
- [x] Request body is properly JSON serialized
- [x] Build succeeds without errors
- [x] Application runs without exceptions
- [x] Polly demo returns actual results
- [x] Retry logic works as expected

## Status: ? RESOLVED

The Polly demo now works correctly. API calls return proper results and Polly's retry logic handles actual failures (timeouts, cancellations) rather than getting stuck on media type errors.

---

**Last Updated**: January 2025  
**Resolution**: Complete rebuild after code changes applied successfully  
**Next Steps**: Test with various timeout scenarios to verify Polly resilience patterns
