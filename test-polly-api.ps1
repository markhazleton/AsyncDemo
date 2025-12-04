# Test Polly API Endpoint
# This script tests the /api/remote/Results endpoint directly

Write-Host "???????????????????????????????????????????????????????????" -ForegroundColor Cyan
Write-Host "  Testing Polly API Endpoint" -ForegroundColor Cyan
Write-Host "???????????????????????????????????????????????????????????" -ForegroundColor Cyan
Write-Host ""

$baseUrl = "https://localhost:7101"

# Test 0: Check if API is running
Write-Host "Test 0: Checking if API is running..." -ForegroundColor Yellow
try {
    $statusCheck = Invoke-RestMethod -Uri "$baseUrl/status" -SkipCertificateCheck -ErrorAction Stop
    Write-Host "? API is running!" -ForegroundColor Green
    Write-Host "   Region: $($statusCheck.region)" -ForegroundColor Gray
    Write-Host "   Status: $($statusCheck.status)" -ForegroundColor Gray
    Write-Host ""
} catch {
    Write-Host "? API is not running or not accessible!" -ForegroundColor Red
    Write-Host "   Make sure the application is started and running on port 7101" -ForegroundColor Red
    Write-Host ""
    exit 1
}

# Test 1: Basic success case
Write-Host "Test 1: Basic request (should succeed)..." -ForegroundColor Yellow
$body1 = @{
    LoopCount = 10
    MaxTimeMS = 1000
    RunTimeMS = 0
    Message = "init"
    ResultValue = "empty"
} | ConvertTo-Json

Write-Host "Request body:" -ForegroundColor Gray
Write-Host $body1 -ForegroundColor DarkGray
Write-Host ""

try {
    $result1 = Invoke-RestMethod -Method Post `
      -Uri "$baseUrl/api/remote/Results" `
      -ContentType "application/json" `
      -Headers @{"Accept"="application/json"} `
      -Body $body1 `
      -SkipCertificateCheck `
      -ErrorAction Stop
    
    Write-Host "? SUCCESS (200 OK):" -ForegroundColor Green
    Write-Host "   Loop Count: $($result1.loopCount)" -ForegroundColor Gray
    Write-Host "   Max Time: $($result1.maxTimeMS)ms" -ForegroundColor Gray
    Write-Host "   Run Time: $($result1.runTimeMS)ms" -ForegroundColor Gray
    Write-Host "   Result Value: $($result1.resultValue)" -ForegroundColor Gray
    Write-Host "   Message: $($result1.message)" -ForegroundColor Gray
    Write-Host ""
} catch {
    $statusCode = $_.Exception.Response.StatusCode.value__
    Write-Host "? FAILED (Status: $statusCode):" -ForegroundColor Red
    
    if ($statusCode -eq 415) {
        Write-Host "   ERROR: 415 Unsupported Media Type" -ForegroundColor Red
        Write-Host "   This means the API is not accepting application/json!" -ForegroundColor Red
        Write-Host ""
        Write-Host "   Possible causes:" -ForegroundColor Yellow
        Write-Host "   1. [Consumes(`"application/json`")] attribute missing" -ForegroundColor Yellow
        Write-Host "   2. [FromBody] attribute missing on parameter" -ForegroundColor Yellow
        Write-Host "   3. Code changes not applied (need rebuild)" -ForegroundColor Yellow
        Write-Host ""
    }
    
    Write-Host "   Full error: $($_.Exception.Message)" -ForegroundColor DarkGray
    Write-Host ""
}

# Test 2: Quick completion case
Write-Host "Test 2: Quick request (5 loops, 2 seconds timeout)..." -ForegroundColor Yellow
$body2 = @{
    LoopCount = 5
    MaxTimeMS = 2000
    RunTimeMS = 0
    Message = "init"
    ResultValue = "empty"
} | ConvertTo-Json

try {
    $result2 = Invoke-RestMethod -Method Post `
      -Uri "$baseUrl/api/remote/Results" `
      -ContentType "application/json" `
      -Headers @{"Accept"="application/json"} `
      -Body $body2 `
      -SkipCertificateCheck `
      -ErrorAction Stop
    
    Write-Host "? SUCCESS (200 OK):" -ForegroundColor Green
    Write-Host "   Loop Count: $($result2.loopCount)" -ForegroundColor Gray
    Write-Host "   Max Time: $($result2.maxTimeMS)ms" -ForegroundColor Gray
    Write-Host "   Run Time: $($result2.runTimeMS)ms" -ForegroundColor Gray
    Write-Host "   Result Value: $($result2.resultValue)" -ForegroundColor Gray
    Write-Host "   Message: $($result2.message)" -ForegroundColor Gray
    Write-Host ""
} catch {
    $statusCode = $_.Exception.Response.StatusCode.value__
    Write-Host "? FAILED (Status: $statusCode):" -ForegroundColor Red
    Write-Host "   Error: $($_.Exception.Message)" -ForegroundColor DarkGray
    Write-Host ""
}

# Test 3: Timeout case
Write-Host "Test 3: Timeout request (should return 408)..." -ForegroundColor Yellow
$body3 = @{
    LoopCount = 100
    MaxTimeMS = 50
    RunTimeMS = 0
    Message = "init"
    ResultValue = "empty"
} | ConvertTo-Json

try {
    $result3 = Invoke-RestMethod -Method Post `
      -Uri "$baseUrl/api/remote/Results" `
      -ContentType "application/json" `
      -Headers @{"Accept"="application/json"} `
      -Body $body3 `
      -SkipCertificateCheck `
      -ErrorAction Stop
    
    Write-Host "??  Request completed (expected timeout but got success):" -ForegroundColor Yellow
    Write-Host "   Result Value: $($result3.resultValue)" -ForegroundColor Gray
    Write-Host "   Message: $($result3.message)" -ForegroundColor Gray
    Write-Host ""
} catch {
    $statusCode = $_.Exception.Response.StatusCode.value__
    if ($statusCode -eq 408) {
        # Try to read the response body even on error
        $responseStream = $_.Exception.Response.GetResponseStream()
        $reader = New-Object System.IO.StreamReader($responseStream)
        $responseBody = $reader.ReadToEnd() | ConvertFrom-Json
        
        Write-Host "? TIMEOUT (Expected 408 Request Timeout):" -ForegroundColor Green
        Write-Host "   Status: 408 Request Timeout" -ForegroundColor Gray
        Write-Host "   Result Value: $($responseBody.resultValue)" -ForegroundColor Gray
        Write-Host "   Message: $($responseBody.message)" -ForegroundColor Gray
        Write-Host "   Run Time: $($responseBody.runTimeMS)ms" -ForegroundColor Gray
        Write-Host ""
    } else {
        Write-Host "? UNEXPECTED ERROR (Status: $statusCode):" -ForegroundColor Red
        Write-Host "   Error: $($_.Exception.Message)" -ForegroundColor DarkGray
        Write-Host ""
    }
}

Write-Host "???????????????????????????????????????????????????????????" -ForegroundColor Cyan
Write-Host "  Testing Complete!" -ForegroundColor Cyan
Write-Host "???????????????????????????????????????????????????????????" -ForegroundColor Cyan
Write-Host ""
Write-Host "Summary:" -ForegroundColor White
Write-Host "- If all tests show ?, the API is working correctly" -ForegroundColor White
Write-Host "- If you see 415 errors (?), the API needs configuration fixes" -ForegroundColor White
Write-Host "- If Test 3 shows ? (408), that's expected - timeout is working" -ForegroundColor White
Write-Host ""
