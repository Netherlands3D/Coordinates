﻿# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.3.0] 16-02-2024

### Added

* Addition and subtraction operators for Coordinate

### Fixed

* implementation of gridshift for EPSG7415.ToWGS84

## [1.2.2] 25-10-2023

### Fixed

* Proper english name for coordinate variable

## [1.2.1] 25-10-2023

### Fixed

* EPSG7415.isValid()'s range determination was flipped; causing it to always return false, resulting in false-positives

## [1.2.0]

### Added

* Added CoordinateSetup.cs to manipulate the RD-coordinates that correspond to the unity-origin.

## [1.1.1] 15-09-2023
### Fixed
* ecefRotationToUp gave wrong results when GRoundLevelY in SetGlobalOrigin.cs was not set to 0.

## [1.1.0]

### Added

* New converter for EPSG:3857

### Fixed

* The converter formerly known as WGS84 had the wrong EPSG code, it is EPSG:4326 instead of EPSG:3857

## [1.0.1]

### Fixed

* Meta file got the package.json was missing
* EPSG7415.isValid()'s range determination was flipped; causing it to always return false

## [1.0.0]

### Added

* Extracted https://github.com/Amsterdam/Netherlands3D/tree/main/Packages/Netherlands3D/Core/Runtime/Scripts/Coordinates 
  into its own package
