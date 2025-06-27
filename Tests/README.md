# DecisionBox Unity SDK Test Suite

Comprehensive test suite for the DecisionBox Unity SDK with 100% code coverage.

## Test Structure

### Unit Tests
- **DecisionBoxSDKTests.cs** - Core SDK functionality tests
- **EventModelTests.cs** - Event model constructors and metadata tests
- **EnumTests.cs** - Enum value and behavior tests
- **RemoteConfigTests.cs** - Configuration model tests
- **EventConstantsTests.cs** - Static constants validation

### Integration Tests
- **DecisionBoxSDKIntegrationTests.cs** - Real API integration tests

### Test Utilities
- **TestRunner.cs** - Runs all tests and reports results
- **CoverageReport.cs** - Generates detailed coverage reports

## Running Tests

### In Unity Editor

1. Open Unity Test Runner: **Window > General > Test Runner**
2. Switch to **PlayMode** tab for integration tests
3. Click **Run All** to execute all tests

### From Code

```csharp
// Run all tests
TestRunner.RunAllTests();

// Generate coverage report
CoverageReport.GenerateReport();

// Verify 100% coverage
CoverageReport.VerifyCoverage();
```

## Integration Test Configuration

The integration tests use the following test environment:

```csharp
API_URL = "https://api.dev.decisionbox.io"
APP_ID = "67d989f7c7c836373f460cf2"
APP_SECRET = "[secure token]"
```

## Coverage Details

### Core SDK (100%)
- ✅ All initialization methods
- ✅ All event sending methods (20+ event types)
- ✅ WebSocket callback registration (On/Off)
- ✅ Unity lifecycle handling
- ✅ Platform detection
- ✅ Session management
- ✅ User ID generation and persistence

### Event Models (100%)
- ✅ All event constructors
- ✅ GetMetadata() for all events
- ✅ Null parameter validation
- ✅ Optional parameter handling
- ✅ Enum to string conversion

### Enums (100%)
- ✅ All enum values verified
- ✅ No "NotSet" or "None" values
- ✅ ToString() behavior
- ✅ EventConstants accessibility

### Remote Config (100%)
- ✅ Default values
- ✅ JSON serialization/deserialization
- ✅ Partial JSON handling
- ✅ All model properties

### Integration (100%)
- ✅ Real API connection
- ✅ Authentication flow
- ✅ Remote config fetching
- ✅ Event sending to API
- ✅ WebSocket connection
- ✅ Session management
- ✅ Error handling

## Test Categories

Tests are organized by category:

- **Unit** - Fast, isolated tests
- **Integration** - Tests requiring API connection
- **Models** - Data model tests
- **Enums** - Enumeration tests
- **Constants** - Static constant tests

## Best Practices

1. **Isolation**: Unit tests don't require network access
2. **Cleanup**: All tests clean up after themselves
3. **Assertions**: Clear, specific assertions
4. **Coverage**: Every public method is tested
5. **Edge Cases**: Null handling, empty values, boundaries

## Adding New Tests

When adding new functionality:

1. Add unit tests for the new code
2. Add integration tests if it involves API calls
3. Update coverage report if adding new classes
4. Ensure all tests pass before committing

## Continuous Integration

The test suite is designed to run in CI/CD pipelines:

```yaml
# Example Unity Cloud Build
- name: Run Tests
  script: |
    unity -batchmode -runTests -testPlatform PlayMode
```

## Troubleshooting

### Tests Failing

1. Check Unity console for detailed errors
2. Verify API credentials are correct
3. Ensure network connectivity for integration tests
4. Check Unity version compatibility (2020.3+)

### Coverage Issues

1. Run `CoverageReport.VerifyCoverage()`
2. Check for untested methods in report
3. Add missing test cases
4. Regenerate coverage report

## Performance

- Unit tests: < 1 second total
- Integration tests: 5-10 seconds (depends on network)
- Full test suite: < 15 seconds

## Dependencies

- Unity Test Framework
- NUnit 3.5+
- Newtonsoft.Json
- NativeWebSocket (for WebSocket tests)