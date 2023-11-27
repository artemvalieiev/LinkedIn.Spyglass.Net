## [LinkedIn.Spyglass](https://github.com/linkedin/Spyglass) .NET-Android Binding Library 
[![NuGet version (LinkedIn.Hakawai)](https://img.shields.io/nuget/v/LinkedIn.Spyglass.svg?style=flat-square)](https://www.nuget.org/packages/LinkedIn.Spyglass/)
# Spyglass 

A powerful Android library that provides highly-customizable widgets (with smart defaults) to easily add social-media-esque mention (aka tag) support to your app

*For a broad overview, check out our [blog post](https://engineering.linkedin.com/android/open-sourcing-spyglass-flexible-library-implementing-mentions-android) at the LinkedIn engineering blog.*

## Features

- A subclass of `EditText` that contains enhanced functionality in order to tokenize user input and display mentions
- A custom view, similar to a `MultiAutoCompleteTextView`, that displays suggestions in an embedded `ListView` rather than a `Popup`
- Custom tokenizer interface, including a default implementation containing several options for customization
- Designed to handle suggestions dynamically as are retrieved from multiple data sources
- Supports both implicit and explicit (i.e. "@User Name") mentions


## Overview

Spyglass is divided into three, customizable layers: *tokenization*, *suggestions*, and *mentions*. Together, these layers form the update lifecycle used within Spyglass:

1. User types a character
2. Tokenize input
3. Generate and display suggestions
4. User taps a suggestion
5. Insert and display mention

For sample setup of custom view that shows mentions list and appears on "@" character type see [GridMentions.cs in Activity Sample project](https://github.com/artemvalieiev/LinkedIn.Spyglass.Net/blob/main/src/LinkedIn.Spyglass/LinkedIn.Spyglass.Sample/GridMentions.cs)

