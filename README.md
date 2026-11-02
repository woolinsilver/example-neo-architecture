# example-neo-architecture

This demonstrates a proposed architecture for game development.

There is a Framework project, similar to Neo.

There is an example Mermaid game, with Tickets and Profile projects.

In reality, each project would be in a separate solution.

---
## Features

### Reflection

At start time, the Game Engine would reflectively construct `MermaidEntryPoint`.

After that, there is no reflection!

The `IPluginLoader` supplied by the Game Engine is not needed.

Everything is done with normal project references and inheritance.

### Dependency Injection (DI)

The projects can be developed and tested independently, in separate solutions.

DI is used to bring everything together at runtime.

The Tickets and Profile projects each have an `IModule` which allows them to wire in anything they want.

### Generics

Generics are used extensively to make game development easier.

Notice that `MermaidTicketFactory` enjoys access to `IMermaidFeatures`, `MermaidTier` etc. without any casting.

---

## Framework

### `EarlyPay`

The class `EarlyPay` extends `GameLogicBase` (from SGStudios.LocalPrize.GameLogic) and simplifies it:

- to require only a single `ExecuteAsync` method
- to remove the concept of database state

### `EntryPoint`

The game must extend this to provide a concrete entry point for the Game Engine to reflectively construct.

The subtype must provide concrete implementations of `IProfile` and `ITicketFactory`.

This class handles all DI wiring.

This class receives requests from the Game Engine and passes them to services managed by the DI container.

### `GameLogicService`

`GameLogicService` is the heart of the framework.

It uses the injected `IProfile` and `ITicketFactory` to satisfy requests received from the Game Engine.
