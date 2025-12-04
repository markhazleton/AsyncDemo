# Testing Polly API with curl

## Direct API Test Commands

### 1. Test with curl (Windows PowerShell)

```powershell
# Basic test - should succeed
curl -X POST "https://localhost:7101/api/remote/Results" `
  -H "Content-Type: application/json" `
  -H "Accept: application/json" `
  -d '{"LoopCount":10,"MaxTimeMS":1000,"RunTimeMS":0,"Message":"init","ResultValue":"empty"}' `
  -k -v

# Timeout test - should return 408
curl -X POST "https://localhost:7101/api/remote/Results" `
  -H "Content-Type: application/json" `
  -H "Accept: application/json" `
  -d '{"LoopCount":100,"MaxTimeMS":100,"RunTimeMS":0,"Message":"init","ResultValue":"empty"}' `
  -k -v
```

### 2. Test with curl (Windows CMD)

```cmd
REM Basic test - should succeed
curl -X POST "https://localhost:7101/api/remote/Results" ^
  -H "Content-Type: application/json" ^
  -H "Accept: application/json" ^
  -d "{\"LoopCount\":10,\"MaxTimeMS\":1000,\"RunTimeMS\":0,\"Message\":\"init\",\"ResultValue\":\"empty\"}" ^
  -k -v

REM Timeout test - should return 408
curl -X POST "https://localhost:7101/api/remote/Results" ^
  -H "Content-Type: application/json" ^
  -H "Accept: application/json" ^
  -d "{\"LoopCount\":100,\"MaxTimeMS\":100,\"RunTimeMS\":0,\"Message\":\"init\",\"ResultValue\":\"empty\"}" ^
  -k -v
```

### 3. Test with PowerShell Invoke-RestMethod

```powershell
# Basic test - should succeed
$body = @{
    LoopCount = 10
    MaxTimeMS = 1000
    RunTimeMS = 0
    Message = "init"
    ResultValue = "empty"
} | ConvertTo-Json

Invoke-RestMethod -Method Post `
  -Uri "https://localhost:7101/api/remote/Results" `
  -ContentType "application/json" `
  -Body $body `
  -SkipCertificateCheck

# Timeout test - should return 408
$body2 = @{
    LoopCount = 100
    MaxTimeMS = 100
    RunTimeMS = 0
    Message = "init"
    ResultValue = "empty"
} | ConvertTo-Json

try {
    Invoke-RestMethod -Method Post `
      -Uri "https://localhost:7101/api/remote/Results" `
      -ContentType "application/json" `
      -Body $body2 `
      -SkipCertificateCheck
} catch {
    Write-Host "Expected error (408 Timeout):" $_.Exception.Message
}
```

### 4. Test with Postman/Insomnia

**URL**: `https://localhost:7101/api/remote/Results`

**Method**: `POST`

**Headers**:
```
Content-Type: application/json
Accept: application/json
```

**Body (JSON)**:
```json
{
  "LoopCount": 10,
  "MaxTimeMS": 1000,
  "RunTimeMS": 0,
  "Message": "init",
  "ResultValue": "empty"
}
```

## Expected Responses

### Success Response (200 OK)
```json
{
  "loopCount": 10,
  "maxTimeMS": 1000,
  "runTimeMS": 150,
  "message": "Task Complete",
  "resultValue": "45"
}
```

### Timeout Response (408 Request Timeout)
```json
{
  "loopCount": 100,
  "maxTimeMS": 100,
  "runTimeMS": 102,
  "message": "Time Out Occurred",
  "resultValue": "-1"
}
```

### Error Response (415 Unsupported Media Type) - SHOULD NOT HAPPEN
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.13",
  "title": "Unsupported Media Type",
  "status": 415
}
```

## Troubleshooting curl Output

### Look for these in the verbose output (-v flag):

**? GOOD - Request is correct:**
```
> POST /api/remote/Results HTTP/1.1
> Host: localhost:7101
> Content-Type: application/json
> Accept: application/json
> Content-Length: 79
>
< HTTP/1.1 200 OK
< Content-Type: application/json; charset=utf-8
```

**? BAD - Missing Content-Type:**
```
> POST /api/remote/Results HTTP/1.1
> Host: localhost:7101
> Accept: application/json
> Content-Length: 79
>
< HTTP/1.1 415 Unsupported Media Type
```

**? BAD - Wrong Content-Type:**
```
> POST /api/remote/Results HTTP/1.1
> Host: localhost:7101
> Content-Type: text/plain
> Content-Length: 79
>
< HTTP/1.1 415 Unsupported Media Type
```

## Debug Steps

### Step 1: Verify the API is running
```powershell
curl https://localhost:7101/status -k
```

Expected: Status information returned

### Step 2: Check OpenAPI/Swagger documentation
Navigate to: `https://localhost:7101/scalar/v1`

Look for the `/api/remote/Results` endpoint and verify:
- Method: POST
- Consumes: application/json
- Request body: MockResults model

### Step 3: Test with minimal curl (verbose)
```powershell
curl -X POST "https://localhost:7101/api/remote/Results" `
  -H "Content-Type: application/json" `
  -d '{"LoopCount":10,"MaxTimeMS":1000}' `
  -k -v
```

**What to check in output:**
1. Request headers include `Content-Type: application/json`
2. Response status code (should be 200 or 408, NOT 415)
3. Response body contains expected JSON

### Step 4: Compare curl vs PollyController request

If curl works but PollyController doesn't, the issue is in how PollyController sends the request.

## Quick Test Script

Save this as `test-polly-api.ps1`:

```powershell
# Test Polly API Endpoint
Write-Host "Testing Polly API Endpoint..." -ForegroundColor Cyan
Write-Host ""

# Test 1: Basic success case
Write-Host "Test 1: Basic request (should succeed)..." -ForegroundColor Yellow
$body1 = @{
    LoopCount = 10
    MaxTimeMS = 1000
} | ConvertTo-Json

try {
    $result1 = Invoke-RestMethod -Method Post `
      -Uri "https://localhost:7101/api/remote/Results" `
      -ContentType "application/json" `
      -Body $body1 `
      -SkipCertificateCheck
    
    Write-Host "? SUCCESS:" -ForegroundColor Green
    Write-Host "  Loop Count: $($result1.loopCount)"
    Write-Host "  Max Time: $($result1.maxTimeMS)ms"
    Write-Host "  Run Time: $($result1.runTimeMS)ms"
    Write-Host "  Result: $($result1.resultValue)"
    Write-Host "  Message: $($result1.message)"
} catch {
    Write-Host "? FAILED:" -ForegroundColor Red
    Write-Host "  Status: $($_.Exception.Response.StatusCode.value__)"
    Write-Host "  Error: $($_.Exception.Message)"
}

Write-Host ""

# Test 2: Timeout case
Write-Host "Test 2: Timeout request (should return 408)..." -ForegroundColor Yellow
$body2 = @{
    LoopCount = 100
    MaxTimeMS = 50
} | ConvertTo-Json

try {
    $result2 = Invoke-RestMethod -Method Post `
      -Uri "https://localhost:7101/api/remote/Results" `
      -ContentType "application/json" `
      -Body $body2 `
      -SkipCertificateCheck -ErrorAction Stop
    
    Write-Host "?? Completed (expected timeout):" -ForegroundColor Yellow
    Write-Host "  Result: $($result2.resultValue)"
    Write-Host "  Message: $($result2.message)"
} catch {
    $statusCode = $_.Exception.Response.StatusCode.value__
    if ($statusCode -eq 408) {
        Write-Host "? TIMEOUT (Expected 408):" -ForegroundColor Green
        Write-Host "  Status: 408 Request Timeout"
    } else {
        Write-Host "? UNEXPECTED ERROR:" -ForegroundColor Red
        Write-Host "  Status: $statusCode"
        Write-Host "  Error: $($_.Exception.Message)"
    }
}

Write-Host ""
Write-Host "Testing complete!" -ForegroundColor Cyan
```

Run it:
```powershell
.\test-polly-api.ps1
```

## Common Issues & Solutions

### Issue: "Connection refused"
**Cause**: Application not running or wrong port
**Solution**: 
1. Check app is running
2. Verify port in `launchSettings.json`
3. Try: `https://localhost:7101/status`

### Issue: "SSL certificate problem"
**Cause**: Self-signed certificate
**Solution**: Add `-k` flag to curl or `-SkipCertificateCheck` to PowerShell

### Issue: Still getting 415 after fixes
**Possible causes**:
1. Changes not applied (need rebuild)
2. Old instance still running
3. Browser/Postman cached the endpoint
4. Reverse proxy/load balancer stripping headers

**Solution**:
1. Stop all instances
2. Clean and rebuild
3. Start fresh
4. Test with curl first (no caching)

## Diagnostic Output

When you run curl with `-v`, save the output and check:

```
REQUEST HEADERS (What we send):
> POST /api/remote/Results HTTP/1.1
> Host: localhost:7101
> User-Agent: curl/7.68.0
> Accept: application/json          ? Should be present
> Content-Type: application/json    ? MUST be present
> Content-Length: 79
>
REQUEST BODY (What we send):
{"LoopCount":10,"MaxTimeMS":1000,"RunTimeMS":0,"Message":"init","ResultValue":"empty"}

RESPONSE HEADERS (What we get back):
< HTTP/1.1 200 OK                   ? Should be 200 or 408, NOT 415
< Content-Type: application/json; charset=utf-8
< Date: Thu, 23 Jan 2025 10:30:00 GMT
< Server: Kestrel
< Content-Length: 123
<
RESPONSE BODY (What we get back):
{"loopCount":10,"maxTimeMS":1000,"runTimeMS":150,"message":"Task Complete","resultValue":"45"}
```

## Next Steps

1. **Run the curl command** from PowerShell
2. **Check the output** - Does it return 200 or 415?
3. **If curl returns 200**: The API is fine, issue is in PollyController
4. **If curl returns 415**: The API endpoint still has issues
5. **Share the curl output** with me for further diagnosis

---

**Note**: Make sure your application is running on `https://localhost:7101` before testing. Check your `launchSettings.json` for the actual port.
