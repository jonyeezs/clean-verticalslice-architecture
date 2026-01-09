# Use ./mvnw wrapper for all Maven commands
MVNW = ./mvnw

.PHONY: clean restore build db test run

clean:
	$(MVNW) clean

restore:
	$(MVNW) dependency:resolve

compile: clean
	$(MVNW) compile --fail-fast -T 1C

# This assumes make compile has been executed	
build:
	$(MVNW) package -DskipTests -Dmaven.test.skip=true

db:
	# Example: Flyway migration (adjust if using Liquibase or another tool)
	$(MVNW) flyway:migrate

# This assumes make compile has been executed	
test:
	$(MVNW) verify

run:
	$(MVNW) spring-boot:run
