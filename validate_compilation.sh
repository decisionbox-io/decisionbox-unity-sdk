#!/bin/bash

echo "=== DecisionBox Unity SDK - UIKit Compilation Validation ==="
echo ""

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Counters
errors=0
warnings=0
files_checked=0

# Function to check file syntax
check_file() {
    local file=$1
    files_checked=$((files_checked + 1))
    
    echo -n "Checking $file... "
    
    # Basic syntax checks
    local issues=""
    
    # Check for unmatched braces
    local open_braces=$(grep -o '{' "$file" | wc -l | tr -d ' ')
    local close_braces=$(grep -o '}' "$file" | wc -l | tr -d ' ')
    if [ "$open_braces" -ne "$close_braces" ]; then
        issues="$issues\n  - Unmatched braces (open: $open_braces, close: $close_braces)"
        errors=$((errors + 1))
    fi
    
    # Check for unmatched parentheses
    local open_parens=$(grep -o '(' "$file" | wc -l | tr -d ' ')
    local close_parens=$(grep -o ')' "$file" | wc -l | tr -d ' ')
    if [ "$open_parens" -ne "$close_parens" ]; then
        issues="$issues\n  - Unmatched parentheses (open: $open_parens, close: $close_parens)"
        errors=$((errors + 1))
    fi
    
    # Check for basic syntax errors
    if grep -q ';;' "$file"; then
        issues="$issues\n  - Double semicolon found"
        warnings=$((warnings + 1))
    fi
    
    # Check namespace declarations
    if ! grep -q '^namespace' "$file"; then
        issues="$issues\n  - No namespace declaration found"
        warnings=$((warnings + 1))
    fi
    
    # Check for class/interface declaration
    if ! grep -q -E '^[[:space:]]*(public|internal|private|protected)?[[:space:]]*(abstract|sealed|static)?[[:space:]]*(class|interface|enum|struct)' "$file"; then
        issues="$issues\n  - No class/interface/enum declaration found"
        warnings=$((warnings + 1))
    fi
    
    if [ -z "$issues" ]; then
        echo -e "${GREEN}OK${NC}"
    else
        echo -e "${RED}Issues found:${NC}"
        echo -e "$issues"
    fi
}

# Check all C# files
echo "Checking all C# files in Runtime directory..."
echo ""

# Check Runtime/UIKit files
for file in Runtime/UIKit/*/*.cs; do
    if [ -f "$file" ]; then
        check_file "$file"
    fi
done

# Check Runtime/Core files
for file in Runtime/Core/*.cs; do
    if [ -f "$file" ]; then
        check_file "$file"
    fi
done

# Check Runtime/Models files
for file in Runtime/Models/*.cs; do
    if [ -f "$file" ]; then
        check_file "$file"
    fi
done

echo ""
echo "=== Dependency Check ==="

# Check for required dependencies
echo -n "Checking for TextMeshPro references... "
if grep -r "using TMPro" Runtime/UIKit --include="*.cs" > /dev/null; then
    echo -e "${GREEN}Found${NC}"
else
    echo -e "${RED}Not found${NC}"
    errors=$((errors + 1))
fi

echo -n "Checking for Newtonsoft.Json references... "
if grep -r "using Newtonsoft.Json" Runtime --include="*.cs" > /dev/null; then
    echo -e "${GREEN}Found${NC}"
else
    echo -e "${RED}Not found${NC}"
    errors=$((errors + 1))
fi

echo -n "Checking for UnityEngine references... "
if grep -r "using UnityEngine" Runtime/UIKit --include="*.cs" > /dev/null; then
    echo -e "${GREEN}Found${NC}"
else
    echo -e "${RED}Not found${NC}"
    errors=$((errors + 1))
fi

echo ""
echo "=== Namespace Consistency Check ==="

# Check namespace consistency
echo -n "Checking DecisionBox.UIKit.Core namespace... "
core_files=$(find Runtime/UIKit/Core -name "*.cs" | wc -l | tr -d ' ')
core_namespace=$(grep -l "namespace DecisionBox.UIKit.Core" Runtime/UIKit/Core/*.cs | wc -l | tr -d ' ')
if [ "$core_files" -eq "$core_namespace" ]; then
    echo -e "${GREEN}Consistent ($core_files files)${NC}"
else
    echo -e "${RED}Inconsistent (files: $core_files, namespace: $core_namespace)${NC}"
    errors=$((errors + 1))
fi

echo -n "Checking DecisionBox.UIKit.Components namespace... "
comp_files=$(find Runtime/UIKit/Components -name "*.cs" | wc -l | tr -d ' ')
comp_namespace=$(grep -l "namespace DecisionBox.UIKit.Components" Runtime/UIKit/Components/*.cs | wc -l | tr -d ' ')
if [ "$comp_files" -eq "$comp_namespace" ]; then
    echo -e "${GREEN}Consistent ($comp_files files)${NC}"
else
    echo -e "${RED}Inconsistent (files: $comp_files, namespace: $comp_namespace)${NC}"
    errors=$((errors + 1))
fi

echo ""
echo "=== Cross-Reference Check ==="

# Check if UIKitManager is referenced in DecisionBoxSDK
echo -n "Checking UIKit integration in DecisionBoxSDK... "
if grep -q "UIKit.Core.UIKitManager" Runtime/Core/DecisionBoxSDK.cs; then
    echo -e "${GREEN}Found${NC}"
else
    echo -e "${RED}Not found${NC}"
    errors=$((errors + 1))
fi

# Check for circular dependencies
echo -n "Checking for potential circular dependencies... "
if grep -r "using DecisionBox.Core" Runtime/UIKit --include="*.cs" | grep -v "DecisionBoxSDK" > /dev/null; then
    echo -e "${YELLOW}Warning: UIKit references Core${NC}"
    warnings=$((warnings + 1))
else
    echo -e "${GREEN}OK${NC}"
fi

echo ""
echo "=== Summary ==="
echo "Files checked: $files_checked"
echo -e "Errors: ${RED}$errors${NC}"
echo -e "Warnings: ${YELLOW}$warnings${NC}"

if [ $errors -eq 0 ]; then
    echo -e "\n${GREEN}✓ No critical errors found. The code structure appears valid.${NC}"
    echo "Note: This is a basic syntax check. Full compilation requires Unity Editor."
    exit 0
else
    echo -e "\n${RED}✗ Critical errors found. Please fix before attempting Unity compilation.${NC}"
    exit 1
fi