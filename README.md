# Distributed Log Processing System

This project is part of the "365-Day Distributed Log Processing System Implementation" series.

## Project Overview

We're building a distributed system for processing log data at scale. This repository contains all the code and configuration needed to run the system.

## Getting Started

### Prerequisites

- Docker and Docker Compose
- Git
- VS Code (recommended)

### Running the Application

1. Clone this repository
2. Navigate to the project directory
3. Run `docker-compose up`

## Project Structure

- `distributed-log-processor/`: Contains individual microservices
- `config/`: Configuration files
- `data/`: Data storage (gitignored)
- `docs/`: Documentation
- `tests/`: Test suites

## HTTP Endpoints

The LoggerService exposes the following HTTP endpoints:

### `GET /health`

- **Description:** Returns a simple health status and the current UTC time.
- **Response Example:**
```
{ "status": "healthy", "utc": "2025-05-20T12:34:56.789Z" }
```
---

### `GET /logs/tail?limit={limit}`

- **Description:** Returns the last N log entries from the most recent log file.
- **Query Parameters:**
  - `limit` (optional, integer, default: 100, min: 1, max: 100): The number of log entries to return.
- **Response Example:**
```
[ { "Timestamp": "2025-05-20T12:34:56.789Z", "Level": "Information", "Message": "Logger service running. This will be part of our distributed system!", "SourceContext": "LoggerService.Worker" }, ... ]
```
- **Notes:**
  - If no log file is found, an empty array is returned.
  - If `limit` is out of range, it defaults to 100.

---



## Day 1 Milestones

- Set up development environment
- Created project structure
- Implemented basic logger service
