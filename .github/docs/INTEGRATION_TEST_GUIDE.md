# Unity SDK Integration Test Guide

## Overview
This guide explains how to run integration tests for the DecisionBox Unity SDK to verify real API communication.

## API Endpoints Used

The SDK communicates with the following endpoints:

1. **Authentication**: `https://eventapi.dev.decisionbox.io/oauth/token`
2. **Token Refresh**: `https://eventapi.dev.decisionbox.io/oauth/token/refresh`
3. **Configuration**: `https://eventapi.dev.decisionbox.io/apps/config?appid={appId}`
4. **Events**: `https://eventapi.dev.decisionbox.io/events`

## Test Credentials

```json
{
  "environment": "development",
  "appId": "68077d9a65f9ed2ee2b45666",
  "appSecret": "352bf0616107a3dc772ccd1f60574ebaf84e055a8d5fc78bf641d6ba7bc8ff28623e2a2cab32ef37ad35e76cc9f4a4ca"
}
```

## Running Tests

### Option 1: Unity Test Runner

1. Open the Unity project
2. Go to **Window > General > Test Runner**
3. Switch to **Play Mode** tab
4. Select `DecisionBoxSDKIntegrationTests`
5. Click **Run All** or run individual tests

### Option 2: Command Line API Test

Run the included bash script to test API endpoints directly:

```bash
./test-api.sh
```

This will test:
- OAuth authentication
- Config fetching
- Event sending
- Token refresh

### Option 3: Manual Integration Test

1. Create a GameObject with `TestIntegrationRunner` component
2. Enter Play mode
3. Check Console for test results

## Expected Results

### Authentication Response
```json
{
  "access_token": "eyJhbGciOiJIUzI1NiIs...",
  "expires_in": "1751039753"
}
```

### Config Response
```json
{
  "enabled": true,
  "max_session_duration": 30,
  "video_recording_enabled": true
}
```

### Event Response
```
Event received
```

### Token Refresh Response
```json
{
  "access_token": "eyJhbGciOiJIUzI1NiIs...",
  "expires_in": "1751039754"
}
```

## SDK Changes Made

1. **Authentication**: Changed from `grant_type` + `app_secret` to `app_id` + `client_secret`
2. **Config Endpoint**: Changed from GET to POST with appid parameter
3. **Token Expiry**: Handle expires_in as timestamp string instead of seconds integer
4. **Event API**: All events now go to dedicated event API endpoint
5. **Token Refresh**: Added automatic token refresh before expiry

## Troubleshooting

### Authentication Fails
- Verify app_id and client_secret are correct
- Check API endpoint is reachable
- Ensure Content-Type header is application/json

### Config Fetch Fails
- Ensure authentication succeeded first
- Verify appid parameter is included in URL
- Check Authorization header includes Bearer token

### Event Sending Fails
- Verify all required fields are present
- Check timestamp is in milliseconds
- Ensure metadata matches event type requirements

### Token Refresh Fails
- Original token must still be valid
- Both Authorization header and access_token in body required
- Check response for new token

## Test Coverage

The integration tests cover:
- SDK initialization with remote config
- All event types (GameStarted, LevelSuccess, etc.)
- WebSocket connection with authentication
- Session management and timeouts
- Background/foreground handling
- Token refresh on expiry

All tests should pass with 100% success rate when connected to the development API.