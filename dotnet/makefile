# Repository name should match the ecr repository name (infrastructure/main.tf:34)
IMAGE_NAME=clean-slice
IMAGE_TAG=local
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

run:
	docker-compose up -d
# yet to figure out how to use th certs
	dotnet dev-certs https --trust
	dotnet watch --project src/Api/Api.csproj -lp local

build:
    # There's an issue with rosalyn running in the background
    # the recommended workaround doesn't work for me. Unfortunately doesn't seem to be fixed yet
    # Details in https://github.com/dotnet/runtime/issues/77723
	dotnet build -c=$(CONFIGURATION) -p:UseSharedCompilation=false

# If the first argument is "migration"...
ifeq (ef,$(firstword $(MAKECMDGOALS)))
  # use the rest as arguments for "run"
  EF_ARGS := $(wordlist 2,$(words $(MAKECMDGOALS)),$(MAKECMDGOALS))
  # ...and turn them into do-nothing targets
  $(eval $(EF_ARGS):;@:)
endif
ef: build
	dotnet ef $(EF_ARGS) --startup-project=./src/Api/Api.csproj --project=./src/DataLayer/DataLayer.csproj --no-build

test: build
	$(foreach project,$(PROJECTS),dotnet test $(project) --no-build --collect:"XPlat Code Coverage" -v n;)

publish:
	@echo "Publish will only be done in Release. Rebuilding Api.csproj with Release"
	dotnet build ./src/Api/Api.csproj --no-restore -c Release -p:UseSharedCompilation=false
	dotnet publish ./src/Api/Api.csproj --no-build -c Release --output=.publish -p:GenerateFullPaths=true

image:
	docker build --pull -t $(IMAGE_NAME) -f ./infrastructure/Dockerfile .
	ifeq ($(CONFIGURATION),'debug')
	@echo "Image built. If you are in local, you can test it with ${GREEN}${BOLD}docker run -p 8080:80 $(IMAGE_NAME)${RESET}."
	endif

image-push:
	docker tag $(IMAGE_NAME) $(IMAGE_REPOSITORY)/$(IMAGE_NAME):$(IMAGE_TAG)
	docker push $(IMAGE_REPOSITORY)/$(IMAGE_NAME):$(IMAGE_TAG)

# clean, restore and build
.PHONY: all
all: clean restore build
