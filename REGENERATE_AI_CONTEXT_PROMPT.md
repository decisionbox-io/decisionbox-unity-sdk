# Prompt to Regenerate AI_INTEGRATION_CONTEXT.md

Use this prompt to regenerate the AI_INTEGRATION_CONTEXT.md file when the SDK is updated.

## Instructions

Copy and paste the following prompt to an AI assistant to regenerate the AI_INTEGRATION_CONTEXT.md file:

---

**PROMPT:**

I need you to create a comprehensive AI integration context document for the DecisionBox Unity SDK. This document will be used as a system prompt for an AI agent in a VSCode extension to help developers integrate the SDK.

Please analyze the DecisionBox Unity SDK codebase and create a markdown document named `AI_INTEGRATION_CONTEXT.md` that includes:

1. **Overview Section**
   - Brief description of the SDK
   - Key features
   - Supported Unity versions
   - Installation instructions via Unity Package Manager

2. **Initial Setup Section**
   - Setup window method (DecisionBox > Setup SDK)
   - Manual setup method
   - Configuration options (App ID, App Secret, Environment, Logging)

3. **Complete API Reference**
   - All public methods in DecisionBoxSDK.cs with:
     - Method signature
     - Parameters with types and descriptions
     - Return type
     - Brief description of what it does
   - Group methods by category:
     - Configuration Methods
     - Game Event Methods
     - Level Event Methods
     - Currency & Economy Methods
     - Booster Event Methods
     - Gameplay Event Methods
     - Location Event Methods
     - Action Outcome Methods
     - Device Token Method
     - WebSocket Methods

4. **Enum Reference**
   - All enums from the Models directory
   - Include all enum values for each enum
   - Enums to include:
     - CurrencyType
     - CurrencyUpdateReason
     - BoosterType
     - OfferMethod
     - AcceptMethod
     - DeclineReason
     - FailureReason
     - MetricType
     - PlatformType

5. **Usage Examples**
   - Basic game flow example
   - Currency management example
   - Booster system example
   - Metrics tracking example
   - WebSocket events example
   - Push notifications example (SendUserDeviceTokenAsync)
   - Location tracking example
   - Action outcomes example

6. **Important Notes Section**
   - Singleton pattern usage (DecisionBoxSDK.Instance)
   - Initialization requirements
   - User ID generation
   - Session management
   - Background handling
   - Token management
   - Platform detection
   - Error handling
   - WebSocket details
   - Thread safety

7. **Configuration Options**
   - SDK configuration (Inspector/Configure method)
   - Remote configuration options

8. **Platform Support**
   - List all supported platforms

9. **Requirements**
   - Unity version
   - C# version
   - Dependencies

10. **Best Practices**
    - Top 10 best practices for using the SDK

11. **Troubleshooting**
    - Common issues and solutions

**Important Requirements:**
- Use the singleton pattern `DecisionBoxSDK.Instance` in all examples
- Include the `SendUserDeviceTokenAsync` method prominently
- Make sure all method signatures show nullable parameters with `?`
- Show default parameter values where applicable
- Use proper C# async/await patterns in examples
- Include proper using statements in examples
- Make the document self-contained (no external references needed)
- Use clean, well-formatted markdown
- Include code examples for every major feature
- Ensure the document can serve as a complete reference for AI-assisted integration

The document should be structured to provide maximum context to an AI agent helping developers integrate the SDK. Every section should be clear, comprehensive, and include practical examples.

---

## Additional Notes for Updating

When regenerating this document:

1. Check for new methods added to DecisionBoxSDK.cs
2. Look for new enums in the Models directory
3. Update any changed method signatures
4. Add examples for any new features
5. Update version requirements if changed
6. Ensure all examples use the latest patterns and best practices
7. Verify that the singleton pattern is used consistently
8. Update any deprecated features or methods
9. Add any new configuration options
10. Include any new platform-specific considerations

## File Locations to Check

- `/Runtime/Core/DecisionBoxSDK.cs` - Main SDK class
- `/Runtime/Models/` - All enum definitions
- `/Runtime/Models/Events/` - Event model definitions
- `/Editor/DecisionBoxSDKSetup.cs` - Editor setup functionality
- `/README.md` - For feature descriptions and examples
- `/package.json` - For version and dependency information