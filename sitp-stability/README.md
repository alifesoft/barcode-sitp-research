# SITP Stability

This directory contains experimental C# software used to implement and evaluate SITP-v1 for related academic research.

The project includes:

- SITP message encoding and decoding;
- Mode11, Mode10, and Mode01 header processing;
- CRC8, CRC16, and CRC32 validation;
- LCG-based masking;
- generation and verification of reproducible random datasets;
- byte-wise false-positive detection experiments;
- SITP encoding and decoding performance measurements;
- XML storage of detected SITP messages and aggregated statistics.

The software is intended for research, reproducibility, and experimental data generation. It is not a production-ready SITP library.

## Utilities

The project contains two executable utilities:

- `sitp-stability` — runs dataset generation, decode-stability experiments, statistics generation, and performance measurements;
- `tryout` — provides a lightweight environment for exploratory tests and validation of individual implementation ideas.

## Directory structure

```text
sitp-stability/
├── src/
│   ├── modules/    SITP, storage, dataset, and utility classes.
│   ├── research/   Experimental workflows and statistics generation.
│   ├── tests/      Basic encode/decode verification.
│   ├── sitp-stability/
│   │               Main research utility.
│   └── tryout/     Exploratory test utility.
├── data/           Input datasets used by experiments.
├── output/         Locally generated experimental results.
├── release/        Release build used for stable experimental runs.
└── debug/          Debug build used during development.
```

## Experimental workflow

A typical experiment consists of:
1. generating or loading a reproducible random baseline dataset;
2. verifying its SHA-256 checksum;
3. attempting SITP decoding at every byte offset;
4. storing all detected messages in XML;
5. aggregating false-positive statistics by SITP mode;
6. measuring complete SITP encode and decode execution time.

Publication-oriented copies of datasets and generated results are stored in the corresponding directories under `papers/`.

## Implementation notes

C# was selected because it provides a practical balance between readability, portability, and execution performance. The implementation is intentionally straightforward so that the protocol logic and experimental methodology can be inspected and reproduced.

