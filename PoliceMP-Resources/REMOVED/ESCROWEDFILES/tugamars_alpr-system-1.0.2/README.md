# ALPR System

This script allows police (or any other jobs you configure) access to a set of fixed ALPRs (Automatic License Plate Readers) spread across the state.
With the usage of a dedicated UI, players are gonna be able to check where a car has been or what cars passed by a location.

Enhance your RP.

## Features

- UI lore integrated
- 113 locations by default
- Configurable
- Easy to add new locations
- Standalone
- Framework integration (toggle on/off in config)
- AI vehicles are also logged
- Runs at 0.0 ms
- Map for visualization

## Requirements
- qb-core / esx / standalone (framework integration is exposed - you can add your own framework)
- PolyZone

## Events
Use the following client event to open the UI: ``tugamars:alpr:tablet:open``

### Command

``/alprtablet`` - Opens the tablet (if command is enabled)

``/alprcreate`` - Starts 3-step easy creation mode (if creation mode is enabled)
``/alprsave`` - Saves ALPRs created into a file (if creation mode is enabled)

