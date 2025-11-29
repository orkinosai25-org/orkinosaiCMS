# OrkinosaiCMS Utility Scripts

This directory contains utility scripts for common development and troubleshooting tasks.

## Available Scripts

### fix-base-branch.sh

**Purpose:** Diagnose and fix the "base branch not found" error that commonly occurs when working with Copilot agent workflows in shallow git clones.

**Usage:**
```bash
# Use default settings (main branch, origin remote)
./scripts/fix-base-branch.sh

# Specify a different base branch
./scripts/fix-base-branch.sh develop

# Specify both base branch and remote
./scripts/fix-base-branch.sh main upstream
```

**What it does:**
1. Checks if the base branch is accessible locally
2. Fetches the base branch from remote if needed
3. Verifies you can compare your branch with the base branch
4. Shows commit count and changes
5. Performs repository health checks
6. Provides summary and next steps

**When to use:**
- After cloning with `--depth=1` or shallow clone
- When you get "ambiguous argument 'refs/heads/main'" errors
- Before creating a pull request to verify branch state
- When troubleshooting Copilot agent failures
- To check if your branch is up to date with the base branch

**Example output:**
```
=== Copilot Agent Base Branch Fix Tool ===

This script will:
  1. Check if the base branch (main) is accessible
  2. Fetch it if needed
  3. Verify the repository state

✓ In a git repository
Current branch: copilot/my-feature

Checking for base branch...
✓ Base branch origin/main is accessible
  SHA: db3f263b95a2c2d08d48e16ae6a5cfdbb5631785

Testing comparison with base branch...
✓ Can successfully compare with base branch
  Changes:  4 files changed, 716 insertions(+)

Commits ahead of base branch:
✓ 2 commit(s) ahead
5af1a24 Add comprehensive Copilot agent troubleshooting documentation
99f2ca4 Initial plan

Finding merge base...
✓ Merge base found: db3f263b
✓ Branch is up to date with origin/main

Repository health check...
✓ Working directory is clean
✓ Repository integrity check passed

=== Summary ===
Base branch: origin/main ✓
Current branch: copilot/my-feature
Commits ahead: 2

You can now:
  • Compare changes: git diff origin/main...HEAD
  • View commits: git log origin/main..HEAD
  • Create PR: Push your branch and open a pull request

All checks passed!
```

**Troubleshooting:**

If the script fails to fetch the base branch:
1. Check your internet connection
2. Verify the branch name is correct (default is 'main', some repos use 'master' or 'develop')
3. Ensure you have access to the repository
4. Try: `git remote -v` to see your remote configuration

If you see integrity warnings:
- Run `git fsck` for detailed diagnostics
- Consider re-cloning the repository if issues persist

## Adding New Scripts

When adding a new script to this directory:

1. **Make it executable:**
   ```bash
   chmod +x scripts/your-script.sh
   ```

2. **Add proper header:**
   ```bash
   #!/bin/bash
   # script-name.sh - Brief description
   # Detailed explanation of what the script does
   ```

3. **Include usage instructions:**
   - Add comments explaining parameters
   - Include example usage
   - Show expected output

4. **Update this README:**
   - Add your script to the "Available Scripts" section
   - Document purpose, usage, and when to use it
   - Include example output

5. **Use consistent style:**
   - Color output for better UX (see fix-base-branch.sh for example)
   - Include error handling with `set -e`
   - Provide helpful error messages
   - Show progress and status

6. **Test thoroughly:**
   - Test on a clean clone
   - Test error conditions
   - Verify output is helpful

## Script Development Best Practices

- **Idempotent:** Scripts should be safe to run multiple times
- **Defensive:** Check prerequisites before proceeding
- **Informative:** Explain what's happening and why
- **Helpful:** Provide next steps and troubleshooting tips
- **Safe:** Use `set -e` to exit on errors
- **Portable:** Use POSIX-compliant commands when possible

## Common Script Patterns

### Checking git repository
```bash
if ! git rev-parse --git-dir > /dev/null 2>&1; then
    echo "Error: Not in a git repository!"
    exit 1
fi
```

### Colored output
```bash
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

echo -e "${GREEN}Success!${NC}"
echo -e "${RED}Error!${NC}"
echo -e "${YELLOW}Warning!${NC}"
```

### Function for status messages
```bash
print_status() {
    echo -e "${GREEN}✓${NC} $1"
}

print_error() {
    echo -e "${RED}✗${NC} $1"
}
```

## Testing Scripts

Before committing scripts:

1. **Test in clean environment:**
   ```bash
   # In a temporary directory
   git clone --depth=1 <repo-url>
   cd <repo>
   ./scripts/your-script.sh
   ```

2. **Test error conditions:**
   - Test with wrong parameters
   - Test when git commands fail
   - Test when prerequisites are missing

3. **Verify output:**
   - Check that colors display correctly
   - Ensure error messages are helpful
   - Confirm success messages are clear

4. **Check portability:**
   - Test on different shells (bash, zsh)
   - Verify on different OS (if applicable)

## Support

If you encounter issues with any script:
1. Check the script's documentation above
2. Review the [QUICK_FIX_GUIDE.md](../docs/QUICK_FIX_GUIDE.md)
3. File an issue with:
   - Script name and version
   - Full error output
   - Steps to reproduce
   - Your environment (OS, Git version, etc.)

---

*Last updated: 2025-11-29*
