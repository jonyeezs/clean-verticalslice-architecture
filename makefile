CONFIGURATION=debug
TEST=all
PROJECTS := $(shell find . -name '*.csproj' -type f)

clean:
	dotnet clean DotNetArchitecture.sln

restore:
	dotnet restore

build:
    # There's an issue with rosalyn running in the background
    # the recommended workaround doesn't work for me. Unfortunately doesn't seem to be fixed yet
    # Details in https://github.com/dotnet/runtime/issues/77723
	dotnet build -c=$(CONFIGURATION) -p:UseSharedCompilation=false

test: build
	$(foreach project,$(PROJECTS),dotnet test $(project) --no-build --collect:"XPlat Code Coverage" -v n;)

# clean, restore and build
.PHONY: all
all: clean restore build
