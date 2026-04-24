# Architecture

This document summarizes how the `casino-all-in` solution is structured and why key design choices were made.

## Component boundaries

### `casino.core`

`casino.core` contains the game engine and domain logic shared by all front ends:

- Contracts such as `IGame` and `IGameFactory`
- Game implementations for Poker, Blackjack, Roulette, and Slots
- Common value objects, events, random abstraction, and game state models
- Core localization resources (`casino.core/Properties/Languages/*.resx`)

This project does **not** depend on console-specific rendering or input classes.

### `casino.console`

`casino.console` is the presentation and orchestration layer:

- Application entrypoint and game selection flow (`Program.cs`)
- Console-specific factory and adapters
- Input parsing and output rendering for each game
- Console localization resources (`casino.console/Localization/*.resx`)

It depends on `casino.core`, but `casino.core` remains UI-agnostic.

### Tests

- `casino.core.tests` validates pure rules/behavior in the core engine.
- `casino.console.tests` validates console-facing behavior (input parsing, frame rendering, UI text, and integration with core contracts).

This keeps domain correctness checks isolated from terminal I/O concerns.

## Key abstractions

### `IGame`

`IGame` is the central runtime contract for playable games in the core. It provides a stable surface for starting/advancing games without coupling callers to specific game implementations.

### `IGameFactory`

`IGameFactory` abstracts game creation, allowing callers to request a game without knowing constructor details. This is used to keep selection/orchestration logic separated from concrete game wiring.

### Poker strategy interfaces

Under `casino.core/Games/Poker/Players/Strategies`, strategy contracts (notably `IPlayerStrategy`) and concrete policies (aggressive, conservative, opportunistic, adaptive, random) define computer-player decision behavior.

This enables behavior swapping by composition rather than by branching inside player entities.

## Testing strategy split

### `casino.core.tests`

Focus:

- Deterministic verification of game rules and scoring
- Pot/round/phase transitions
- Winner evaluation and action validation
- Localization behavior in core resources where relevant

These tests avoid terminal dependencies and favor small, focused rule-level assertions.

### `casino.console.tests`

Focus:

- Input alias parsing and command translation
- Rendering and frame buffer layout behavior
- Console-specific localization text/access
- Factory/program flow behavior at the console boundary

These tests verify that console interaction maps correctly to core contracts.

## Localization approach

Localization is resource-driven with `.resx` files in both layers:

- Core strings live in `casino.core/Properties/Languages/*.resx`
- Console UI strings live in `casino.console/Localization/*.resx`

This separation lets domain text and presentation text evolve independently, while still supporting the same language set. Generated designer classes provide strongly typed accessors and culture-aware lookup.

## Design trade-offs

1. **Separation of core vs. console projects**  
   Chosen to maximize testability and future portability (e.g., another UI host). The trade-off is extra interfaces/factories and slightly more wiring.

2. **Interface-driven extension points (`IGame`, `IGameFactory`, `IPlayerStrategy`)**  
   Chosen to reduce coupling and simplify substitution in tests. The trade-off is additional abstraction that can feel heavier than direct concrete calls in a small codebase.

3. **Dedicated core and console test projects**  
   Chosen to isolate failures and keep test intent clear (domain vs. presentation). The trade-off is duplicated setup helpers and more project-level maintenance.

4. **Resource files split by layer (`core` vs. `console`)**  
   Chosen so domain terms and UI wording can be managed independently. The trade-off is the need to synchronize language coverage across two resource sets.

5. **Multiple concrete poker AI strategies rather than one monolithic heuristic**  
   Chosen for readability, tunability, and scenario testing across different AI personalities. The trade-off is more classes to maintain and calibrate.
