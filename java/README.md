# Java specific information

## Commands

`./mvnw clean install`

## Code structure
We use spring boot but we diverge from their folder/namespace structure. We move to a [package by use case structure](https://medium.com/sahibinden-technology/package-by-layer-vs-package-by-feature-7e89cde2ae3a)

We still initialize it using https://start.spring.io/

We will keep the code structure as close to common Java Spring Boot setups.

* src/main/java/demo/cleanslice
  * usecases - Code that is required during runtime
* src/test
  * all the tests for API

## usecases

Each use case will be in its own package/namespace.

Utilizes the default class access modifier - package-private to ensure use case specific logic is isolated.

### Code structure

1. **Controller**
   * We still want to separate the presentation layer from our application layer.
   * We keep this as bare minimum as possible.
   * Logic is mediated through a basic service
2. **Handler**
   * This is a cheap mediator class as as service.
     * _TODO: move to something like https://github.com/sizovs/PipelinR_
3. **DataAccess**
   * This is where we will interact with the DAOs (Data Access Objects) and any external IO.
   * We abstract any direct interactions from the domain. Think of it as our repository layer.
4. **XYZDomain**
   * Here we declare the root aggregate.
   * Due to Java's rule, where the class must be the same as the folder, we'll name the file as the root aggregate's name
     followed by `Domain` to indicate the domain logic layer.

## datalayer

For this demo we use ormlite as a simplified use case on connecting to a database.

The database related entities and its connections are managed in the `demo.cleanslice.datalayer` package.

### Variables

The database requires these environment variables to be set to connect to the database (_for application and migration_)

* DB_HOST
* DB_PORT
* DB_NAME
* DB_USER
* DB_PASSWORD

### Migration

We aren't going to be using a full-fledge migration process here. Just as an example sake, we'll just use ormlite's
method to create our database instance through running `MigrationApplication.java`.
