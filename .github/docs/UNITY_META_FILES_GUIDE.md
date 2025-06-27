# Unity Meta Files Generation Guide

## Problem
The Unity SDK package is missing `.meta` files which are required for Unity to properly recognize and import the scripts and assets. Without these files, Unity won't be able to use the SDK.

## Solution

### Option 1: Generate Meta Files in Unity (Recommended)

1. **Create a temporary Unity project**:
   ```bash
   # Using Unity Hub or Unity Editor
   # Create new 3D project named "DecisionBoxSDKMetaGen"
   ```

2. **Import the SDK into Unity**:
   - Copy the entire `decisionbox-unity-sdk` folder into the project's `Packages` folder
   - Or use Package Manager with local path

3. **Unity will automatically generate all .meta files**:
   - Unity creates a `.meta` file for every file and folder
   - These contain unique GUIDs that Unity uses for asset tracking

4. **Copy the generated .meta files back**:
   ```bash
   # From Unity project
   find Packages/decisionbox-unity-sdk -name "*.meta" -exec cp {} /path/to/sdk/{} \;
   ```

5. **Commit the .meta files to the repository**:
   ```bash
   git add "*.meta"
   git commit -m "Add Unity meta files for proper package importing"
   git push
   ```

### Option 2: Add .gitignore Exception

Update `.gitignore` to NOT ignore meta files:

```gitignore
# Unity meta files should be included for packages
!*.meta
```

### Required Meta Files

Every file and folder needs a corresponding .meta file:

```
Runtime.meta
Runtime/
├── Core.meta
├── Core/
│   ├── DecisionBoxSDK.cs.meta
│   ├── AuthManager.cs.meta
│   └── ...
├── Models.meta
├── Models/
│   ├── EventModels.cs.meta
│   ├── Enums.cs.meta
│   └── ...
├── DecisionBox.Runtime.asmdef.meta
└── ...

Tests.meta
Tests/
├── Runtime.meta
├── Runtime/
│   ├── DecisionBoxSDKTests.cs.meta
│   └── ...
└── ...

package.json.meta
README.md.meta
LICENSE.md.meta
CHANGELOG.md.meta
```

## Meta File Format

A typical Unity meta file looks like:

```yaml
fileFormatVersion: 2
guid: a1b2c3d4e5f6789012345678901234567
MonoImporter:
  externalObjects: {}
  serializedVersion: 2
  defaultReferences: []
  executionOrder: 0
  icon: {instanceID: 0}
  userData: 
  assetBundleName: 
  assetBundleVariant: 
```

## Important Notes

1. **GUIDs must be unique** - Don't copy meta files between different assets
2. **Meta files must match their assets** - DecisionBoxSDK.cs must have DecisionBoxSDK.cs.meta
3. **Include all meta files in version control** for Unity packages
4. **Assembly definition files** (.asmdef) also need meta files

## Verification

After adding meta files, test by:

1. Create a new Unity project
2. Import the package via Package Manager (git URL or local path)
3. Verify no import errors
4. Check that all scripts are recognized
5. Ensure assembly definitions are working

Without meta files, Unity will show errors like:
- "Script cannot be loaded"
- "Missing MonoBehaviour"
- "Type or namespace not found"

With proper meta files, the import should be seamless.