# JSONStorable Class

## Overview
The contained classes are considered 'Restorables', in the sense that they are a `Component` 

The `JSONStorable` class is designed to facilitate the storage and retrieval of objects in JSON format. This class provides methods to serialize and deserialize objects, making it easier to persist and restore their state.

## Features
- **Serialization**: Convert objects to JSON format.
- **Deserialization**: Restore objects from JSON format.
- **Error Handling**: Robust error handling during serialization and deserialization.

## Usage

### Initialization
To create an instance of `JSONStorable`, simply instantiate the class:
```csharp
var jsonStorable = new JSONStorable();
