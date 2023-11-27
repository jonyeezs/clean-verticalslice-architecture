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

### Code structure

1. **Controller**
   * We still want to separate the presentation layer from our application layer.
   * We keep this as bare minimum as possible.
   * Logic is mediated through a basic service
2. **Service**
   * This is a cheap mediator class we can use.
   * TODO: move to something like https://github.com/sizovs/PipelinR
