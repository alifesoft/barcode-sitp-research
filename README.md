# barcode-sitp-research

Research code and experiments for SITP-based barcode encoding, ESID design, and related academic papers.

## Overview

This repository contains research code, experimental materials, datasets, and supporting notes related to the development and evaluation of the Self-Identified Transport Protocol (SITP) for barcode payload encoding.

The repository is also intended to support the development of the Encoding Scheme Identifier (ESID) mechanism, which is used to explicitly identify nested payload encoding schemes inside SITP-compatible barcode data streams.

The materials stored here are primarily related to academic research papers on SITP-based barcode encoding and protocol identification. Final production implementations, standalone .NET libraries, and separate barcode symbology projects may be maintained in independent repositories.

## Related papers

Yurii Kulakov, Oleksandr Havryliuk.
**A Review of Data Encoding Problems in Barcodes and a Layered Encoding Protocol Based on the OSI Model.**

Additional SITP-related papers and experimental studies may be added as the research develops.

## Repository structure
papers/
    Academic paper materials, paper-specific datasets, and experimental results.

papers/layered-encoding-protocol/
    Materials related to the paper Yurii Kulakov, Oleksandr Havryliuk "A Review of Data Encoding Problems in Barcodes and a Layered Encoding Protocol Based on the OSI Model".

papers/stability-analysis/
    Materials related to the paper Yurii Kulakov, Oleksandr Havryliuk "Stability Analysis and Applicability Boundaries of the Self-Identified Transport Protocol in Stochastic Data Environments"

sitp-stability/
    Experimental C# software used to generate SITP stability data and research outputs.

sitp-stability/src/
    Source code of the experimental C# implementation.

sitp-stability/release/
    Release builds of the experimental software.

sitp-stability/debug/
    Debug builds of the experimental software.

sitp-stability/data/
    Input datasets used in stability experiments.

sitp-stability/output/
    Generated experimental results.

## Author

**Oleksandr Havryliuk**
Alternative transliteration: Alexander Gavriluk

Academic contact: [o.havryliuk@kpi.ua](mailto:o.havryliuk@kpi.ua)
General contact: [alifesoft@gmail.com](mailto:alifesoft@gmail.com)

## Licensing

Source code in this repository is licensed under the MIT License.

Research data, generated experimental results, tables, figures, and documentation are provided under the Creative Commons Attribution 4.0 International License (CC BY 4.0), unless otherwise stated.
