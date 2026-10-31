DOTNET ?= dotnet
CONFIGURATION ?= Release
SOLUTION := FlappyBird.sln
PROJECT := FlappyBird/FlappyBird.csproj

.DEFAULT_GOAL := help
.PHONY: help restore build run test check clean

help:
	@echo Available targets:
	@echo   restore - Restore locked NuGet dependencies
	@echo   build   - Restore and build the solution
	@echo   run     - Build and start the game
	@echo   test    - Build and run regression tests
	@echo   check   - Restore, build and test
	@echo   clean   - Remove build outputs for CONFIGURATION
	@echo Defaults: CONFIGURATION=Release DOTNET=dotnet

restore:
	"$(DOTNET)" restore "$(SOLUTION)" --locked-mode

build: restore
	"$(DOTNET)" build "$(SOLUTION)" --configuration "$(CONFIGURATION)" --no-restore

run: build
	"$(DOTNET)" run --project "$(PROJECT)" --configuration "$(CONFIGURATION)" --no-build --no-restore

test: build
	"$(DOTNET)" test --solution "$(SOLUTION)" --configuration "$(CONFIGURATION)" --no-build --no-restore --minimum-expected-tests 1

check: test

clean:
	"$(DOTNET)" clean "$(SOLUTION)" --configuration "$(CONFIGURATION)"
