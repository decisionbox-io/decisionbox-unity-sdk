# Unity SDK API Endpoint Fix Summary

## Issue Identified
The SDK was using the wrong API endpoint for sending events. It was using the general API endpoint (`https://api.dev.decisionbox.io`) instead of the dedicated event API endpoints.

## Changes Made

### 1. DecisionBoxSDK.cs
- Added `EventApiUrl` property that returns the correct event API endpoint based on environment:
  - Development: `https://eventapi.dev.decisionbox.io/events`
  - Production: `https://eventapi.decisionbox.io/events`
- Changed SDK to use `environment` field ("development" or "production") instead of hardcoded API URL
- Updated `SendEventAsync` method to use `EventApiUrl` instead of `ApiUrl`
- Changed all event methods to return `Task<bool>` instead of `Task` to properly indicate success/failure
- Updated session event methods to return `Task<bool>`

### 2. Integration Tests (DecisionBoxSDKIntegrationTests.cs)
- Updated `SetSDKConfiguration` to set `environment` field instead of `apiUrl`
- Changed all test assertions from checking `IsCompletedSuccessfully` to checking `task.Result`
- Updated `SendAllEventTypes_ToRealAPI` to use `List<Task<bool>>` and check actual results
- Fixed assertions to properly verify that events were sent successfully

## Key Code Changes

### Before:
```csharp
// SDK was using general API endpoint
private string apiUrl = "https://api.dev.decisionbox.io";

// In SendEventAsync:
using (var request = new UnityWebRequest($"{apiUrl}/events", "POST"))

// Event methods returned Task
public async Task SendGameStartedAsync(...)
```

### After:
```csharp
// SDK now uses dedicated event API endpoint
private string EventApiUrl => environment == "development" 
    ? "https://eventapi.dev.decisionbox.io/events" 
    : "https://eventapi.decisionbox.io/events";

// In SendEventAsync:
using (var request = new UnityWebRequest(EventApiUrl, "POST"))

// Event methods return Task<bool> to indicate success/failure
public async Task<bool> SendGameStartedAsync(...)
```

## Testing Impact
- Tests now properly verify that API calls succeed by checking the boolean return value
- Tests will fail if the API endpoint is incorrect or if events fail to send
- Integration tests now use the correct development event API endpoint

## Next Steps
To run the integration tests and verify the fix:
1. Open the Unity project
2. Run the test suite through Unity Test Runner
3. Verify all integration tests pass with the correct event API endpoints