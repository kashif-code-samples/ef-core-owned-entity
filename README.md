# Entity Framework Core Owned Entity

## Owned Entity Types
As defined on [Owned Entity Types](https://learn.microsoft.com/en-us/ef/core/modeling/owned-entities)
> EF Core allows you to model entity types that can only ever appear on navigation properties of other entity types. These are called owned entity types. The entity containing an owned entity type is its owner.

If we talk in terms Domain Driven Design, the entity containing owned entity equates to the aggregate root and owned entity would become an aggregate.

Also owned entity is a good candidate to model values objects in Domain Driven Design. Value objects don't have a surrogate identity instead all fields make up the object identity.

> An aggregate will have one of its component objects be the aggregate root. Any references from outside the aggregate should only go to the aggregate root. The root can thus ensure the integrity of the aggregate as a whole.

Based on the rule above EF Core has following limitations
* A `DbSet<T>` of owned entity cannot be created.
* You cannot call `Entity<T>()` with an owned type on `ModelBuilder`.

## Project Setup
Let's start by creating a new empty folder and create a new solution using following command.
```shell
dotnet new sln --name Customer
```

Let's add a webapi project and add it to solution.
```shell
dotnet new webapi --use-controllers -o Customer.Api
dotnet sln add Customer.Api/Customer.Api.csproj
```

## References
In no particular order
* [Owned Entity Types](https://learn.microsoft.com/en-us/ef/core/modeling/owned-entities)
* [DDD Aggregate](https://martinfowler.com/bliki/DDD_Aggregate.html)