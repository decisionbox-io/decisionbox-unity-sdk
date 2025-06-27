# Fix Unity Import Issues - Missing Meta Files

## Problem
Unity cannot recognize DecisionBoxSDK as a component because the package is missing `.meta` files. These files are required for Unity to:
- Recognize scripts as MonoBehaviour components
- Track asset GUIDs
- Maintain references between assets

## Quick Solution

### Option 1: Import and Re-export (Recommended)

1. **Create a new Unity project** (Unity 2020.3+)

2. **Import the package manually**:
   ```
   - Create folder: Assets/DecisionBoxSDK
   - Copy all files from this repository into that folder
   - Unity will generate all meta files automatically
   ```

3. **Wait for Unity to compile** (check the progress bar)

4. **Test that it works**:
   - Create empty GameObject
   - Click "Add Component" 
   - Search for "DecisionBox"
   - You should see "Decision Box SDK" component

5. **Export the package with meta files**:
   ```
   - Right-click Assets/DecisionBoxSDK folder
   - Select "Export Package..."
   - Ensure all items are checked (including .meta files)
   - Save as DecisionBoxSDK.unitypackage
   ```

### Option 2: Direct Git Import Fix

1. **Clone the repository locally**:
   ```bash
   git clone https://github.com/decisionbox-io/decisionbox-unity-sdk.git
   cd decisionbox-unity-sdk
   ```

2. **Open in Unity**:
   - Create new Unity project
   - Copy the entire repository content to Packages/com.decisionbox.sdk/
   - Unity generates meta files

3. **Commit meta files**:
   ```bash
   git add "*.meta"
   git commit -m "Add Unity meta files for component recognition"
   git push
   ```

### Option 3: Manual Package Installation

1. **In your Unity project**, create the structure:
   ```
   Packages/
   └── com.decisionbox.sdk/
       ├── package.json
       ├── Runtime/
       ├── Tests/
       └── ...
   ```

2. **Copy all files** from this repository

3. **Restart Unity** - it will generate meta files

4. **Verify** the component appears in Add Component menu

## Verification Steps

After importing with meta files:

1. ✅ Create GameObject → Add Component → Search "Decision"
2. ✅ DecisionBox SDK component should appear
3. ✅ No console errors about missing scripts
4. ✅ Assembly definitions recognized (no compilation errors)

## Why This Happens

- Git repositories don't include Unity meta files by default
- Meta files contain GUIDs that Unity generates
- Without meta files, Unity treats scripts as regular text files
- MonoBehaviour scripts need meta files to be recognized as components

## Permanent Fix

We need to:
1. Generate meta files in Unity
2. Commit them to the repository
3. Update .gitignore to include meta files

This is a one-time fix that will solve the issue permanently.