IMAGE_NAME=clean-slice-image
CONFIGURATION=debug
TEST=all
PROJECTS := $(shell find . -name '*.csproj' -type f)

GREEN=\033[32m
BOLD=\033[1m
RESET=\033[0m

clean:
	rm -rf .publish
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

publish:
	@echo "Publish will only be done in Release. Rebuilding Api.csproj with Release"
	dotnet build ./src/Api/Api.csproj --no-restore -c Release -p:UseSharedCompilation=false
	dotnet publish ./src/Api/Api.csproj --no-build -c Release --output=.publish

image:
	docker build -t $(IMAGE_NAME) -f ./infrastructure/Dockerfile .
	@echo "Image built. You can test it with ${GREEN}${BOLD}docker run -p 8080:80 $(IMAGE_NAME)${RESET}."
# clean, restore and build
.PHONY: all
all: clean restore build
