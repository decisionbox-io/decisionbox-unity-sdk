#!/bin/bash

# DecisionBox Unity SDK API Test Script
# Tests all API endpoints with the provided credentials

APP_ID="68077d9a65f9ed2ee2b45666"
CLIENT_SECRET="352bf0616107a3dc772ccd1f60574ebaf84e055a8d5fc78bf641d6ba7bc8ff28623e2a2cab32ef37ad35e76cc9f4a4ca"
API_BASE="https://eventapi.dev.decisionbox.io"

echo "=== DecisionBox API Integration Test ==="
echo "Testing with App ID: $APP_ID"
echo ""

# Test 1: Authentication
echo "1. Testing Authentication..."
AUTH_RESPONSE=$(curl -s --location "$API_BASE/oauth/token" \
  --header 'Content-Type: application/json' \
  --data "{
    \"app_id\": \"$APP_ID\",
    \"client_secret\": \"$CLIENT_SECRET\"
  }")

echo "Response: $AUTH_RESPONSE"

# Extract access token
ACCESS_TOKEN=$(echo $AUTH_RESPONSE | grep -o '"access_token":"[^"]*' | cut -d'"' -f4)

if [ -z "$ACCESS_TOKEN" ]; then
  echo "✗ Authentication failed"
  exit 1
fi

echo "✓ Authentication successful"
echo "Access Token: ${ACCESS_TOKEN:0:20}..."
echo ""

# Test 2: Config Fetch
echo "2. Testing Config Fetch..."
CONFIG_RESPONSE=$(curl -s --location --request POST "$API_BASE/apps/config?appid=$APP_ID" \
  --header "Authorization: Bearer $ACCESS_TOKEN" \
  --data '')

echo "Response: $CONFIG_RESPONSE"

if [[ $CONFIG_RESPONSE == *"error"* ]]; then
  echo "✗ Config fetch failed"
else
  echo "✓ Config fetch successful"
fi
echo ""

# Test 3: Send Event
echo "3. Testing Event Sending..."
USER_ID="test_user_$(uuidgen)"
SESSION_ID="$(uuidgen)"
TIMESTAMP=$(date +%s000)

EVENT_RESPONSE=$(curl -s --location "$API_BASE/events" \
  --header 'Content-Type: application/json' \
  --header "Authorization: Bearer $ACCESS_TOKEN" \
  --data "{
    \"user_id\": \"$USER_ID\",
    \"session_id\": \"$SESSION_ID\",
    \"app_id\": \"$APP_ID\",
    \"event_type\": \"LevelSuccess\",
    \"timestamp\": $TIMESTAMP,
    \"metadata\": {
      \"levelNumber\": 1
    }
  }")

echo "Response: $EVENT_RESPONSE"

if [[ $EVENT_RESPONSE == *"error"* ]]; then
  echo "✗ Event sending failed"
else
  echo "✓ Event sent successfully"
fi
echo ""

# Test 4: Token Refresh
echo "4. Testing Token Refresh..."
REFRESH_RESPONSE=$(curl -s --location "$API_BASE/oauth/token/refresh" \
  --header 'Content-Type: application/json' \
  --header "Authorization: Bearer $ACCESS_TOKEN" \
  --data "{
    \"access_token\": \"$ACCESS_TOKEN\"
  }")

echo "Response: $REFRESH_RESPONSE"

if [[ $REFRESH_RESPONSE == *"access_token"* ]]; then
  echo "✓ Token refresh successful"
else
  echo "✗ Token refresh failed"
fi
echo ""

echo "=== Test Complete ==="