# Use ./mvnw wrapper for all Maven commands
MVNW = ./mvnw

.PHONY: clean restore build db test run

clean:
	$(MVNW) clean

restore:
	$(MVNW) dependency:resolve

build:
	$(MVNW) clean package

db:
	# Example: Flyway migration (adjust if using Liquibase or another tool)
	$(MVNW) flyway:migrate

test:
	$(MVNW) test

run:
	$(MVNW) spring-boot:run
