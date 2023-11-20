# Dotnet specific information

# Code structure

* src
  * API - Code that is required during runtime
  * DataLayer Code that opens a window into our database for code usage
* tests
  * all the tests for API

# API

## Codestructure

1. **Route**
   * Uses [Carter](https://github.com/CarterCommunity/Carter) for a more fluent approach and to integrate well with OpenAPI.
   * Logic is mediated through [MediatR](https://github.com/jbogard/MediatR) (_More explanation later_)
2. **Request and Response**
   * [POCOs](https://learn.microsoft.com/en-us/dotnet/standard/glossary#poco).
   * To communicate the inputs and outputs of the routes.
   * Naming convention - should be prefixed with the use case so that OpenAPI definitions can be granular and searchable.
3. **Validator**
   * Uses [FluentValidation](https://docs.fluentvalidation.net) to write the validation for the request.
   * The validation done here is targetted solely to the values in the request. Think of this as the first-pass form validation. Any further validation that has dependencies is done in the `Domain`.
   * Validation happens through the MediatR Behaviours (`ValidationBehaviour.cs`)
     * this allows us to reuse validation if the request is from a message subscription.
4. **Handler**
   * Uses [MediatR](https://github.com/jbogard/MediatR). It may seem overkill but what we really leverage off is:
      1. The [Behaviours](https://github.com/jbogard/MediatR/wiki/Behaviors) allow us to handle common execution of logic (eg: validation)
      2. Reusable logic through different interfaces. (But don't mediate messages across use cases!!)
      3. Easily perform [branch by abstraction](https://www.martinfowler.com/bliki/BranchByAbstraction.html) from the `Route` if we need to feature flag or scientist logic.
5. **DbAccess & Domain**
   * **DbAccess**
      * Alias for `Data Access`.
      * `DbAccess` sends its models into the `Domain` as an "in-memory" state and returns the `Domain` for us to interact with
      * `DbAccess` is the connector/interactor with our repository/ORM, like doing the mapping, saving, retrieval, and deletion on our `Domain` updates
   * **Domain**
     * ATM we allow the access to the `IQueryable` interface.


### Extras

A root `Routes.cs` ties in any route names so that we can quickly see what routes are available and allow uses cases to share the routes.

For instance two different use cases, one for viewing recipes and the other creating recipes. Viewing would have a route of `GET /recipes` and creation would be `POST /recipes`.

Through `Routes.cs` we can share that routing name without fluffing around double-guessing ourselves if we got the right route name. _Also_ gives some cohesiveness to the whole API endpoint.

## Infrastructure

### Structure

1. **Behaviours**
   * The middleware for [MediatR](https://github.com/jbogard/MediatR/wiki/Behaviors)
   * Executed in the order it was registered in `program.cs`
   * Handles a start-stop logging, ability to rollback transactions, and execute the FluentValidator
   * This is to abstract common execution of task that needs to be performed on a request.
   * Maybe overkill but it allows us to separate some boilercode.
2. **Middleware**
   * AspNetCore middleware. We would love to just use this for middleware.
   * ....but it doesn't cover for when a request is an subscribed event.
   * But situations where we want to deal with the http request and response directly; such as auth or exceptions
   * Whenever we need to execute common logic on an incoming or outgoing data we should try to do it on this layer first!
3. **Swagger**
   * Any configuration code pertaining to OpenAPI(_Swagger_)
   * Should utilize its startup API first before having huge objects to work with.


## Common

> Any universal code that is shared among use cases.
>
> Common is anything that would be shared across more than 3 use cases would be considered common. They are pure functions as in they have an input and output with no side effects and dependencies.

# DataLayer

We use Entity Framework

### Principles

* There should not be any business logic here.
* What should be in here is code that describes the schema and structure of our database.
* It also provides the direct operations on our database such as saving, deleting, etc.
  * ie: the repository patterns or ORMs.
* A benefit having our database entities placed here is the psychological visual indicator that these
objects are bound to the database schema and should not be used as our DTO within our application.
It is a DTO but one that is to view into our database layer.
