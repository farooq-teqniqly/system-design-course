#!/bin/sh
dotnet csharpier --check --loglevel Debug  .
dotnet test ".\distributed-logging-service\DistributedLoggingService.sln"