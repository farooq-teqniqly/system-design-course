#!/bin/sh
dotnet csharpier --check --loglevel Debug  .
dotnet test ".\distributed-log-processor\DistributedLogProcessor.sln"