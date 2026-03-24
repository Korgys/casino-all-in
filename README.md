# Casino All-In

![Build](https://github.com/korgys/casino-all-in/actions/workflows/buildAndTest.yml/badge.svg) ![Status](https://img.shields.io/badge/status-active-success) ![Last Commit](https://img.shields.io/github/last-commit/korgys/casino-all-in) ![Coverage](https://sonarcloud.io/api/project_badges/measure?project=Korgys_casino-all-in&metric=coverage) [![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=Korgys_casino-all-in&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=Korgys_casino-all-in) [![Lines of Code](https://sonarcloud.io/api/project_badges/measure?project=Korgys_casino-all-in&metric=ncloc)](https://sonarcloud.io/summary/new_code?id=Korgys_casino-all-in) [![Duplicated Lines (%)](https://sonarcloud.io/api/project_badges/measure?project=Korgys_casino-all-in&metric=duplicated_lines_density)](https://sonarcloud.io/summary/new_code?id=Korgys_casino-all-in) [![Bugs](https://sonarcloud.io/api/project_badges/measure?project=Korgys_casino-all-in&metric=bugs)](https://sonarcloud.io/summary/new_code?id=Korgys_casino-all-in) [![Vulnerabilities](https://sonarcloud.io/api/project_badges/measure?project=Korgys_casino-all-in&metric=vulnerabilities)](https://sonarcloud.io/summary/new_code?id=Korgys_casino-all-in) [![Reliability Rating](https://sonarcloud.io/api/project_badges/measure?project=Korgys_casino-all-in&metric=reliability_rating)](https://sonarcloud.io/summary/new_code?id=Korgys_casino-all-in) [![Security Rating](https://sonarcloud.io/api/project_badges/measure?project=Korgys_casino-all-in&metric=security_rating)](https://sonarcloud.io/summary/new_code?id=Korgys_casino-all-in) [![Maintainability Rating](https://sonarcloud.io/api/project_badges/measure?project=Korgys_casino-all-in&metric=sqale_rating)](https://sonarcloud.io/summary/new_code?id=Korgys_casino-all-in) ![.NET](https://img.shields.io/badge/.NET-10.0-blue) ![Platform](https://img.shields.io/badge/platform-Windows%20%7C%20Linux%20%7C%20MacOS-lightgrey) ![License](https://img.shields.io/github/license/korgys/casino-all-in)

## Overview

Casino All-In is a console-based casino project built with .NET. It is designed as a portfolio showcase to demonstrate:

- Clean architecture separation (`casino.core` / `casino.console`)
- Game logic modeling (deck, rounds, actions, scoring)
- Automated tests for core rules and console behavior

### Gameplay preview

![Gameplay preview](docs/images/poker_gameplay.png)

## Features

- **Texas Hold'em** game in the terminal with betting phases (Pre-Flop, Flop, Turn, River, Showdown).
- **Blackjack** game in the terminal (hit/stand loop, dealer draw rules, winner resolution).
- **Slot Machine** game in the terminal with animated reels, combo payouts, and jackpot spins.
- Computer players with multiple strategies (aggressive, conservative, opportunistic, random).
- Poker action flow support: fold, check, call, raise, bet, all-in.
- Poker hand evaluation and winner resolution.
- Console renderer for table state, actions, and game progression
- Unit tests for game engine and console layer.

## Installation

### Prerequisites

- [.NET SDK 10.0](https://dotnet.microsoft.com/) (target framework: `net10.0`)
- or Docker

### Option A — Run with .NET CLI

```bash
git clone https://github.com/<your-username>/casino-all-in.git
cd casino-all-in
dotnet restore
dotnet run --project casino.console
```

### Option B — Run with Docker

```bash
git clone https://github.com/<your-username>/casino-all-in.git
cd casino-all-in
docker build -t casino-all-in -f casino.console/Dockerfile .
docker run --rm -it casino-all-in
```

## Usage

1. Launch the app (CLI or Docker).
2. Choose a game (Poker, Blackjack, or Slot Machine).
3. Follow prompts in the terminal to choose actions (`check`, `call`, `raise`, `fold`, `all-in`, `hit`, `stand`, etc.).
4. Play rounds until the game ends, then choose whether to start a new game.

### Run tests

```bash
dotnet test
```

## Project Structure

```text
casino-all-in/
├── casino.core/           # Domain/game engine (rules, cards, scores, phases, players, slots)
├── casino.console/        # Console application (entrypoint, renderer, input handling)
├── casino.core.tests/     # Unit tests for core poker logic
├── casino.console.tests/  # Unit tests for console behaviors
├── casino-all-in.slnx
└── README.md
```

## Poker Rules

This project implements a simplified Texas Hold'em loop:

- Each player receives two private cards.
- Community cards are revealed in 3 stages: **Flop (3)**, **Turn (1)**, **River (1)**.
- Players can act each betting round depending on game state (check, bet, call, raise, fold, all-in).
- At showdown, the best 5-card hand (from 7 available cards) determines the winner.
- Standard ranking order is used (High Card → One Pair → Two Pair → Three of a Kind → Straight → Flush → Full House → Four of a Kind → Straight Flush).

## Roadmap

- [ ] Add additional casino games (Roulette, Craps, etc.).
- [ ] Improve UI/UX with richer console animations/colors.
- [ ] Add configurable game setup (number of bots, starting stacks, blinds).
- [ ] Add localization options (FR/EN prompts and messages).

## License

Distributed under the MIT License. See [`LICENSE`](LICENSE) for details.
