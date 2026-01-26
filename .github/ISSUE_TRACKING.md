# Issue Tracking and Status Management

This document explains how error reports and feature requests are tracked in the Extenject project.

## Overview

Extenject uses GitHub Issues with structured forms and labels to track bug reports, feature requests, and other issues. Each issue has status tracking capabilities to help users and maintainers understand the current state of reported problems.

## Issue Types

### Bug Reports

When reporting a bug, you'll use the Bug Report form which includes:

- **Status Field**: Tracks the current status of the bug
  - Needs Triage (default)
  - Under Investigation
  - Confirmed
  - In Progress
  - Fixed
  - Won't Fix
  - Duplicate

- **Priority Field**: Indicates the severity
  - Low - Minor issue with workaround
  - Medium - Moderate impact
  - High - Major functionality affected
  - Critical - Blocking or crashes

### Feature Requests

Feature requests use a similar tracking system:

- **Status Field**:
  - Needs Triage (default)
  - Under Consideration
  - Planned
  - In Progress
  - Completed
  - Won't Implement
  - Duplicate

## Labels

The project uses a comprehensive labeling system:

### Status Labels
- `status: needs-triage` - Issue needs initial assessment
- `status: under-investigation` - Being investigated
- `status: confirmed` - Bug confirmed and reproduced
- `status: in-progress` - Work is ongoing
- `status: fixed` / `status: completed` - Resolution complete
- `status: wont-fix` / `status: wont-implement` - Will not be addressed
- `status: duplicate` - Duplicate of another issue

### Priority Labels
- `priority: low` - Minor issues
- `priority: medium` - Moderate impact
- `priority: high` - Major functionality affected
- `priority: critical` - Blocking or crashes

### Type Labels
- `bug` - Something isn't working
- `enhancement` - New features
- `documentation` - Documentation improvements
- `question` - Questions or discussions
- `showcase` - Solutions built with Extenject

### Component Labels
- `component: core` - Core DI container
- `component: bindings` - Binding system
- `component: signals` - Signals system
- `component: memory-pools` - Memory pools
- `component: factories` - Factories
- `component: subcontainers` - Sub-containers

### Unity-Specific Labels
- `unity: mono` - Mono backend specific
- `unity: il2cpp` - IL2CPP backend specific

## Checking Issue Status

To check the status of an error report or feature request:

1. **Go to the Issues page**: https://github.com/lask3802/Extenject/issues
2. **Use filters**: Click on labels to filter by status, priority, or component
3. **Check individual issues**: Each issue will have status labels and may include status dropdown selections

### Filtering Examples

- To see all bugs that need triage: Use label filter `bug + status: needs-triage`
- To see all in-progress items: Use label filter `status: in-progress`
- To see high priority bugs: Use label filter `bug + priority: high`
- To see fixed issues: Use label filter `status: fixed`

## For Maintainers

### Updating Issue Status

When working on issues:

1. Add appropriate status labels as work progresses
2. Remove old status labels when updating to a new status
3. Add priority labels based on severity and impact
4. Add component labels to help categorize issues
5. Close issues when they are resolved or won't be fixed

### Best Practices

- Triage new issues within 1-2 days
- Keep status labels up to date
- Use milestones for planned work
- Link related issues and pull requests
- Update issue status when PR is merged

## For Contributors

When submitting an issue:

1. Choose the appropriate template (Bug Report, Feature Request, etc.)
2. Fill out all required fields
3. Set priority based on your assessment (maintainers will adjust if needed)
4. Provide as much detail as possible
5. Check for existing issues before creating a new one

## Automated Tracking

The issue forms automatically apply initial labels:
- Bug reports get `bug` and `status: needs-triage` labels
- Feature requests get `enhancement` and `status: needs-triage` labels
- Solutions showcase gets `documentation` and `showcase` labels

This helps ensure all issues are properly categorized from the start.
