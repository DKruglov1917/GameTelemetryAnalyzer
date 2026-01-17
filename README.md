## GameTelemetryAnalyzer

A telemetry analysis tool built for a game project. It processes runtime snapshots, evaluates them against external balancing models (CSV / Sheets), and produces structured reports with optional Discord delivery. Designed for automated execution in CI.

---

### Why I built this tool

This tool was built as part of the development workflow for my game project, *Yurko*.

During development, the game produces telemetry snapshots describing the state of the world (economy, points of interest, reachability, etc.).  
Manually inspecting this data quickly became impractical and error-prone.

GameTelemetryAnalyzer was created to:
- automatically validate game state against configurable rules
- detect critical issues early (economy imbalance, unreachable locations)
- provide fast feedback without manual inspection
- integrate analysis results into existing developer workflows

Although the tool was built for a specific game, the analysis pipeline is generic and can be extended to other telemetry-driven systems.

---

### Economic model and balancing

The CSV sources consumed by the analyzer represent a **balancing model**, not a set of static thresholds.

For example, the Economy sheet defines:
- recommended resource levels
- acceptable deviation ranges
- warning and critical thresholds

Together, these values describe the intended economic balance of the game.

By combining:
- telemetry snapshots produced by the game runtime
- external balancing data (CSV / Sheets)
- deterministic analysis rules

the tool validates whether the observed game state aligns with the intended design.

In practice, this analysis is used to:
- tune resource availability
- adjust object and enemy spawn rates
- validate reachability and progression pacing
- iteratively refine game configuration without manual inspection

The analyzer consumes the same economic model that is used to configure the game runtime.  
It does **not** modify the model and does **not** influence gameplay parameters directly.

Instead, it highlights deviations between the intended balance and the observed game state,
forming a controlled feedback loop between telemetry, balancing data, and configuration decisions.

---

## Overview

At a high level, the application performs the following steps:

1. Loads external configuration sources
2. Builds runtime analysis configuration
3. Loads a telemetry snapshot exported from the game
4. Executes a set of analyzers on the snapshot
5. Produces an analysis report
6. Optionally delivers the report to Discord

---

## Architecture

The project is structured into separated layers:

```txt
Domain
├─ Core models and invariants
├─ Telemetry input model
└─ Analysis output model

Analysis
├─ Analyzer abstractions
├─ Concrete analyzers (Economy, Reachability)
└─ Analyzer execution pipeline

Application
├─ CLI parsing
├─ Runtime configuration building
└─ Application orchestration

Infrastructure
├─ JSON loading
├─ CSV / Sheets ingestion
├─ Discord delivery
└─ Message formatting
```

---

## Telemetry input

The analyzer consumes a single telemetry snapshot represented as JSON.

### TelemetryRun structure

A telemetry run consists of:

- **RunInfo**
  - Run identifier
  - Source
  - UTC timestamp
- **Snapshot**
  - Points of interest (POI metrics)
  - Available resources
  - Electricity metrics
  - Enemy spawner metrics
  - Weather metrics
- **RuntimeEvents**
  - Time-based events grouped by category

All fields are expected to be present.  
Missing or unknown values are represented explicitly.

---

## Configuration

### Runtime configuration (Sheets / CSV)

Analysis rules are defined externally using CSV-based sources (for example, Google Sheets):

- **Economy**
  - Per-resource recommended values
  - Warning and critical thresholds
- **Reachability**
  - Distance thresholds
  - Access baselines
  - Danger level configuration

These sources are loaded at runtime and mapped into domain configuration objects.

---

### Delivery configuration

Delivery behavior is fully data-driven and configured via JSON:

- Enable or disable Discord delivery
- Webhook URL
- Message template
- Display rules (hide OK blocks, hide empty sections, etc.)

Presentation logic is not hardcoded in analyzers.

---

## Analysis model

Analysis is built around the concept of **analyzers**.

### Analyzer abstraction

Each analyzer:

- Has a readable identifier (`Name`)
- Processes a telemetry snapshot and runtime configuration
- Produces zero or more findings
- Assigns a severity level to each finding

Metric-based analyzers share a common base class.

---

### Analyzer execution

Analyzers are:

- Discovered automatically via reflection
- Filtered via CLI options (`--only`, `--exclude`)
- Executed sequentially
- Aggregated into a single analysis report

Analyzer execution is deterministic and order-independent.

---

## CLI usage
Supported command-line options:
```txt
--discord        Enable Discord delivery
--console        Print report to console
--only           Run only selected analyzers
--exclude        Exclude selected analyzers
```


Analyzer names are case-insensitive.

If the same analyzer is present in both `--only` and `--exclude`, exclusion takes precedence.

---

## Automation and CI

GameTelemetryAnalyzer is designed to run automatically as part of the project infrastructure.

In the current setup, the tool is executed on a server via Jenkins:
- telemetry snapshots are generated by the game runtime
- the analyzer is triggered automatically
- analysis results are sent to Discord for immediate visibility

This allows critical issues to be detected without manual intervention and provides continuous feedback during development.

---

## Output

The result of execution is a `TelemetryReport` containing:

- Executed analyzers
- Analysis results
- Findings grouped by analyzer and severity
- Execution timestamp

If Discord delivery is enabled, the report is formatted and sent via webhook.

---

## Example Discord output

```txt
Analysis completed
Run time: Wednesday, January 14, 2026 2:06 PM
Economy:
    Critical:
        bp_FuelRod: amount 9 deviates from recommended 120 by 111
        bp_Fuse: amount 3 deviates from recommended 40 by 37

Reachability:
    Warning:
        PoI_10: distance 67922,22 exceeds baseline 100 by 67822,22
        PoI_11: distance 52374,863 exceeds baseline 100 by 52274,863
        PoI_3: distance 57558,016 exceeds baseline 100 by 57458,016
        PoI_6: distance 61345,895 exceeds baseline 100 by 61245,895
        PoI_0: distance 55485,293 exceeds baseline 100 by 55385,293
        PoI_13: distance 68187,414 exceeds baseline 100 by 68087,414
        PoI_15: distance 66343,64 exceeds baseline 100 by 66243,64
        PoI_1: distance 50905,8 exceeds baseline 100 by 50805,8

    Critical:
        PoI_14: distance 104098,49 exceeds baseline 100 by 103998,49
        PoI_12: distance 101409,84 exceeds baseline 100 by 101309,84
        PoI_8: distance 71573,164 exceeds baseline 100 by 71473,164
```

---

## Extending the system

To add a new analyzer:

1. Create a new class inheriting from `Analyzer` or `BaselineAnalyzer<T>`
2. Implement metric selection and evaluation logic
3. Assign a unique analyzer name

The analyzer will be discovered automatically at runtime.

---

## Error handling philosophy

The system follows a **fail-fast** approach for:

- Invalid configuration
- Missing external data
- Malformed telemetry input

Running with silent misconfiguration is intentionally avoided.

---

## Testing

Testing strategy, coverage, and rationale are documented separately.  
See `README.tests.md`.

