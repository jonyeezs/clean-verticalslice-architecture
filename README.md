Uses a combination of Vertical Slice and Clean architecture.

Vertical Slice - to structure all logic of a use case.
Clean - to compartment-ize common execution of code in a layered manner.

# Architecture

> Folders and namespaces is the first level of understanding how the application works
>
> We separate what's shared and what is commonly run amongst all our code
>
> All our business logics are grouped as features/use cases/activities.

* **src** - All the code to that makes our application
  1. **Api** - The layers (middlewares) of the application and the use cases of our business
  2. **DataLayer** -
  3. **Migrations** - Code that defines the on how to build your database.
* **tests**
  * Would love tests to be in the same folder as code but package dependencies that tests needs aren't needed for your app. So for that sake it is best to separate the files apart.
  * Keep the tests in the same folder structure as `src` to easily navigate
  * Intergration and unit test are placed together. They are both equally as cheap and should be run at all test. _Different story if they were e2e test_.
* **infrastructure** - Resource provisioning using Terraform(IaC) and docker related
  * The local `docker-compose.yml` is in root. This creates the images to run your local environment

# Api

> Code that is required during runtime

## Use cases

> All the work we do solves a use case that we have for our product.
> How consumers interact with our application, how we validate that interaction and how we handle that with business logic.

### Principles

* All work are co-located to ease navigation between code and understandable at a glance.
* All business logic (user interface, DTOs, validations, persistent interaction, business domain smarts, and responses) are all in the same namespace and folder.
* All code to be isolated to its own use case so that when it is refactored or tempered it will not affect another use case.
  * This may feel like a smell once you see duplicate code all over the place - that's fine; [We focus on the right abstraction rather than DRY-all-the-things](https://youtu.be/8bZh5LMaSmE?t=891)
* Each use case can work differently (ie one may be a http only endpoint another could be a messagint endpoint)

### Structure

1. **Route**
   * Uses [Carter](https://github.com/CarterCommunity/Carter) for a more fluent approach and to integrate well with OpenAPI.
   * API endpoint and swagger configuration. How is the endpoint setup (is it with auth? What kind of authorization?)
   * Should contain almost no actual business domain logic. Why? Say if this use case works for both REST API and a messaging subscription, we don't need to duplicate that logic. But most times it would be either or; as to keep to a convention it is easier to speed through than think which way should i got about.
   * Logic is mediated through [MediatR](https://github.com/jbogard/MediatR) (_More explanation later_)
2. **Request and Response**
   * [POCOs](https://learn.microsoft.com/en-us/dotnet/standard/glossary#poco).
   * To communicate the inputs and outputs of the routes.
   * Should not have any logic whatsoever.
   * Should be easily created and read.
   * This is not the same objects used as entities for the persistent layer.
   * Naming convention - should be prefixed with the use case so that OpenAPI definitions can be granular and searchable.
3. **Validator**
   * Uses [FluentValidation](https://docs.fluentvalidation.net) to write the validation for the request.
   * The validation done here is targetted solely to the values in the request. Think of this as the first-pass form validation. Any further validation that has dependencies is done in the `Domain`.
   * Validation happens through the MediatR Behaviours (`ValidationBehaviour.cs`)
     * this allows us to reuse validation if the request is from a message subscription.
4. **Handler**
   * The crux of the interaction between the request and the domain logic.
   * Most of your dependencies will sit in here
   * This is where we can reach out to our data layer access to allow our domain logic to start manipulating state.
     * Only rule is that the actual domain logic should be in the **Domain**
   * Uses [MediatR](https://github.com/jbogard/MediatR). It may seem overkill but what we really leverage off is:
      1. The [Behaviours](https://github.com/jbogard/MediatR/wiki/Behaviors) allow us to handle common execution of logic (eg: validation)
      2. Reusable logic through different interfaces. (But don't mediate messages across use cases!!)
      3. Easily perform [branch by abstraction](https://www.martinfowler.com/bliki/BranchByAbstraction.html) from the `Route` if we need to feature flag or scientist logic.
5. **DbAccess & Domain**
   * These two components plays apart in maintaining our state
   * This follows two ideas: [(1) Domain Driven Design](https://youtu.be/52qChRS4M0Y) and [(2) Seperation of concern of data access and logic](https://www.thereformedprogrammer.net/six-ways-to-build-better-entity-framework-core-and-ef6-applications/#5-pattern-building-business-logic-using-domain-driven-design).
   * Code only to the use case and nothing more
   * _Caveat:_ Not all situation will need a domain(business logic). This is more often, than not, in queries. IMO, the query logic can all be done within the DbAccess area.
   * You may ask _"Why do you still have data access code here? That's not clean"_. Answer:
     * We follow a more pragmatic approach; end of the day all this is our code,
     * more abstraction and layers adds more complexity,
     * and this follows close to the [vertical slice architecture](https://codeopinion.com/restructuring-to-a-vertical-slice-architecture/).
   * **DbAccess**
      * Every use case requires access to the data so ever differently
      * It is nothing more than a facade to our data access layer.
      * Here we can piece together the aggregates that we need for our domain logic.
        * Think about this as our Aggregate factory.
      * `DbAccess` sends its models into the `Domain` as an "in-memory" state and returns the `Domain` for us to interact with
      * `DbAccess` is the connector/interactor with our repository/ORM, like doing the mapping, saving, retrieval, and deletion on our `Domain` updates
   * **Domain**
     * `Domain` is our aggregate root for this use case
       * > An aggregate is a cluster of associated objects that we treat as a unit for the purpose of data changes.
     * This should be the only place we can change the state.
     * Our aggregates are the Data layer's model.
       * When we deal with plain objects, it helps us to test the domain logic easily for edge cases and multiple cases.
       * Hence, this should not have dependencies on any other services. Stay away from "_God objects_".
     * _"What if i need a function from say a time calculator?"_. Answer:
       * Use it as an interface into your domain method's argument.
       * Think of this as a pure function of inputs and outputs (albeit the output is the domain object itself).
### Extras

A root `Routes.cs` ties in any route names so that we can quickly see what routes are available and allow uses cases to share the routes.

For instance two different use cases, one for viewing recipes and the other creating recipes. Viewing would have a route of `GET /recipes` and creation would be `POST /recipes`.

Through `Routes.cs` we can share that routing name without fluffing around double-guessing ourselves if we got the right route name. _Also_ gives some cohesiveness to the whole API endpoint.

## Infrastructure

> These are the layers outside of our domain. Infrastructure that runs our domain logic through the layers.
> This follows loosely on the clean architecture. Where it diverge is that all logic pertaining to a business is bundled
> together under the same namespace and co-located folder space (`Api/UseCases`).

### Principles

Classes, structs, or helpers in its root namespace (`*.Infrastructure.*`) should never be imported into any other area (_well of course other than the `program.cs`_)

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

> Code that opens a window into our database for code usage

Contains the [Data Access Layer](https://en.wikipedia.org/wiki/Data_access_layer) and migration plan

### Principles

* There should not be any business logic here.
* What should be in here is code that describes the schema and structure of our database.
* It also provides the direct operations on our database such as saving, deleting, etc.
  * ie: the repository patterns or ORMs.
* A benefit having our database entities placed here is the psychological visual indicator that these
objects are bound to the database schema and should not be used as our DTO within our application.
It is a DTO but one that is to view into our database layer.


# CICD

Currently using github actions (`.github/`). The principles here are:

* split the pull request and deployment into different processes.
* allow CICD to operate using [ship-show-ask](https://martinfowler.com/articles/ship-show-ask.html)

# References

* Borrows ideas from https://github.com/threenine/api-template
* Understanding how to layout your folders from [Architecture the Lost Years by Robert Martin](https://youtu.be/WpkDN78P884)
* How to do vertical slice architecture by [Jimmy Bogard](https://youtu.be/SUiWfhAhgQw)
* Some principles on how to think about [domain/business logic from John P Smith](https://www.thereformedprogrammer.net/architecture-of-business-layer-working-with-entity-framework-core-and-v6-revisited/#2-the-bizlogic-layer)
