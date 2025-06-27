# Generate Proper Unity Meta Files

Since you have Unity installed, let's generate the real meta files:

## Steps:

1. **Open Unity Hub** and create a new project:
   - Unity version: 2020.3 or later
   - Template: 3D (any template works)
   - Name: DecisionBoxMetaGenerator

2. **Import the SDK**:
   ```
   Option A (Recommended):
   - In Unity: Window > Package Manager
   - Click + > Add package from disk...
   - Navigate to this folder and select package.json
   
   Option B:
   - Copy this entire folder to: YourUnityProject/Packages/com.decisionbox.sdk/
   ```

3. **Unity will generate all meta files automatically**
   - Wait for Unity to compile (check progress bar)
   - You'll see .meta files appear next to every file

4. **Verify it works**:
   - Create empty GameObject
   - Add Component > search "Decision"
   - You should see "Decision Box SDK" component

5. **Copy meta files back** (from Terminal):
   ```bash
   # If you used Option A (Package Manager), the files are in:
   cd ~/Library/PackageCache/io.decisionbox.sdk@1.0.0/
   
   # If you used Option B (Packages folder):
   cd YourUnityProject/Packages/com.decisionbox.sdk/
   
   # Copy all meta files back to this repository
   find . -name "*.meta" -exec cp {} /Users/abacigil/Documents/decisionbox/sdk/sdk-generator/output/decisionbox-unity-sdk/{} \;
   ```

6. **Verify meta files were copied**:
   ```bash
   cd /Users/abacigil/Documents/decisionbox/sdk/sdk-generator/output/decisionbox-unity-sdk
   find . -name "*.meta" | wc -l
   # Should show 50+ files
   ```

## Quick Alternative:

If the SDK is already in a Unity project somewhere, just copy the meta files:

```bash
# From the Unity project where SDK is working
cd path/to/unity/project/Packages/decisionbox-sdk
cp -r . /Users/abacigil/Documents/decisionbox/sdk/sdk-generator/output/decisionbox-unity-sdk/
```

The key is that Unity must generate these files - they contain unique GUIDs that Unity uses for asset tracking.