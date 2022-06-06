# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.8.1] - 2022-06-06

### Added

- Update whtml drivers 0.12.1 -> 0.12.4 https://github.com/wkhtmltopdf/wkhtmltopdf/releases/tag/0.12.4 (based on https://github.com/webgio/Rotativa/pull/115)
- Fix try to kill process when cancellation requested

## [1.8.0] - 2022-06-03

### Added

- Add timeout support https://github.com/webgio/Rotativa/issues/203

### Fixed

- fix when WkhtmlDriver exits with error code https://github.com/webgio/Rotativa/issues/189
- fix using HttpContext.Current instead passed ControllerContext https://github.com/webgio/Rotativa/pull/178

