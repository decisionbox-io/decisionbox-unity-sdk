#!/bin/bash

echo "=== DecisionBox Unity SDK Validation ==="
echo ""

# Check required files
echo "1. Checking required files..."
required_files=(
    "package.json"
    "README.md"
    "LICENSE.md"
    "CHANGELOG.md"
    "Runtime/DecisionBox.Runtime.asmdef"
    "Tests/Runtime/DecisionBox.Tests.asmdef"
    "Runtime/Core/DecisionBoxSDK.cs"
    "Runtime/Models/EventModels.cs"
    "Runtime/Models/Enums.cs"
)

all_good=true
for file in "${required_files[@]}"; do
    if [ -f "$file" ]; then
        echo "  ✓ $file"
    else
        echo "  ✗ $file - MISSING!"
        all_good=false
    fi
done

echo ""
echo "2. Checking C# syntax..."
# Basic syntax check - look for common issues
cs_files=$(find . -name "*.cs" -type f)
syntax_errors=0

for file in $cs_files; do
    # Check for basic syntax issues
    if grep -q "^\s*}\s*}\s*$" "$file"; then
        echo "  ⚠ Possible double closing brace in $file"
        syntax_errors=$((syntax_errors + 1))
    fi
    
    # Check for unmatched braces
    open_braces=$(grep -o "{" "$file" | wc -l)
    close_braces=$(grep -o "}" "$file" | wc -l)
    if [ "$open_braces" -ne "$close_braces" ]; then
        echo "  ⚠ Unmatched braces in $file (open: $open_braces, close: $close_braces)"
        syntax_errors=$((syntax_errors + 1))
    fi
done

if [ $syntax_errors -eq 0 ]; then
    echo "  ✓ No obvious syntax errors found"
else
    echo "  ✗ Found $syntax_errors potential syntax issues"
    all_good=false
fi

echo ""
echo "3. Checking assembly definitions..."
for asmdef in $(find . -name "*.asmdef"); do
    if jq . "$asmdef" > /dev/null 2>&1; then
        echo "  ✓ $asmdef - Valid JSON"
    else
        echo "  ✗ $asmdef - Invalid JSON!"
        all_good=false
    fi
done

echo ""
echo "4. Checking package.json..."
if jq .name package.json > /dev/null 2>&1; then
    name=$(jq -r .name package.json)
    version=$(jq -r .version package.json)
    echo "  ✓ Package: $name v$version"
else
    echo "  ✗ Invalid package.json!"
    all_good=false
fi

echo ""
echo "5. Checking for test files..."
test_count=$(find Tests -name "*Test*.cs" -type f | wc -l)
echo "  ✓ Found $test_count test files"

echo ""
echo "6. API Integration Test..."
if [ -f "./test-api.sh" ]; then
    echo "  ✓ API test script exists"
    # Run API test
    if ./test-api.sh | grep -q "Test Complete"; then
        echo "  ✓ API integration test passed"
    else
        echo "  ✗ API integration test failed"
        all_good=false
    fi
else
    echo "  ✗ API test script missing"
    all_good=false
fi

echo ""
if $all_good; then
    echo "=== ✓ SDK validation PASSED ==="
    exit 0
else
    echo "=== ✗ SDK validation FAILED ==="
    exit 1
fi