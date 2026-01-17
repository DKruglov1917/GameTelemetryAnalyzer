# Testing Strategy

This document describes the testing approach used in **GameTelemetryAnalyzer**.

The goal of testing in this project is to validate:
- core analysis logic
- configuration-driven behavior
- CLI-to-domain mapping
- critical infrastructure boundaries

---

## Test structure

Tests are organized by responsibility and mirror the main project structure:

```txt
Tests
├─ Analysis
│ ├─ AnalyzerRunnerTests
│ ├─ EconomyAnalyzerTests
│ ├─ ReachabilityAnalyzerTests
│ └─ CliOptionsTests
│
├─ Domain
│ └─ AnalyzerSelectionTests
│
├─ Infrastructure
│ └─ JsonLoaderTests
│
└─ TestData
├─ TestTelemetryRunBuilder
├─ TestRunConfigBuilder
└─ TestSnapshotBuilder
```

---

## Analysis tests

### AnalyzerRunnerTests

These tests validate the analyzer execution pipeline:

- analyzer discovery
- `--only` selection behavior
- `--exclude` filtering behavior
- exclusion precedence over inclusion
- result aggregation into a single report
- correct behavior for empty snapshots

The goal is to ensure that the execution pipeline behaves correctly regardless of which analyzers are enabled.

---

### EconomyAnalyzerTests

Economy analysis is tested using **delta-based scenarios**:

- small deviation from recommended value → warning
- large deviation → critical
- insignificant deviation → no findings

---

### ReachabilityAnalyzerTests

Reachability analysis tests validate:

- distance vs access baseline comparison
- correct severity assignment
- no findings when metrics are within expected limits

---

## CLI tests

### CliOptionsTests

CLI parsing tests validate:

- mapping of CLI arguments to domain-level `AnalyzerSelection`
- case-insensitive analyzer names
- parsing of comma-separated values
- default behavior when no arguments are provided
- failure on unknown arguments

---

## Domain tests

### AnalyzerSelectionTests

Domain-level tests verify:

- correct behavior of `Only` and `Exclude` sets
- correctness of the `AnalyzerSelection.All` preset

---

## Infrastructure tests

### JsonLoaderTests

Infrastructure tests focus on contract validation, not implementation details.

The JSON loader tests verify that:

- valid telemetry JSON is correctly deserialized
- required fields are present
- enum values are parsed correctly

---

## Test data builders

The project uses dedicated builders to construct test data:

- `TestTelemetryRunBuilder`
- `TestRunConfigBuilder`
- `TestSnapshotBuilder`

This approach:
- avoids repetitive setup code
- makes test intent explicit
- keeps individual tests small and readable

---

## Test coverage scope and future work

The current test suite focuses on deterministic unit and contract-level tests that validate core analysis logic and configuration behavior.

Some infrastructure components, such as:
- HTTP delivery to Discord
- Google Sheets / CSV network loading

are currently not covered by tests.

These areas are considered integration boundaries and are planned to be covered by integration or end-to-end tests if required by production usage.


---

## Design philosophy

Tests are written to validate **behavior**, not implementation details.

Priority is given to:
- clarity of intent
- deterministic outcomes
- minimal test setup
- explicit failure conditions


