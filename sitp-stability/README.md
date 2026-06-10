# SITP Stability

This directory contains experimental C# software used to generate data and verify assumptions for SITP-related research.

The project is intended for research and experimental data generation only. It is not a production-ready SITP implementation or a standalone .NET library.

## Utilities

The project contains two utilities:

* `sitp-stability` — the main utility used to generate scientific results for SITP stability and protocol-identification experiments.
* `tryout` — an auxiliary utility used for quick tests, exploratory checks, and experimental validation of individual ideas.

## Directory structure

```text
sitp-stability/
├── src/       Source code of the experimental C# implementation.
├── release/   Release build used for stable experimental runs.
├── debug/     Debug build used during development and verification.
├── data/      Input datasets and experiment parameters.
└── output/    Generated experimental results.
```

## Implementation notes

C# is used for this experimental implementation because it provides a practical balance between readability and execution performance. The code is intended to be easy to inspect and reproduce, while remaining fast enough for large-scale payload generation, checksum evaluation, and stability experiments.

Generated results from this project may be used in paper-specific directories under `papers/`.

