# Claude Code Guidelines

## Code Style

- All comments, tooltips, and documentation must be in **English**
- **Never use coroutines** - always use `Update()` with state machines and timers instead

## Project Structure

- Unity project for Mini-Jam 202
- Scripts located in `Assets/Scripts/`
  - `Combat/` - Combat system (CombatManager, CombatAnimator, CombatantStats)
  - `Player/` - Player management (PlayerController, PlayerState, Inventory)
  - `Core/` - Core systems (GameManager, TimeManager)
  - `UI/` - UI panels and HUD
  - `Data/` - Data structures and enums
  - `Shop/` - Shop system
