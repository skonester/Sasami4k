# Sasami4K

Sasami4K is a high-performance **WebView2-based media player** shell built with **F#** and **WPF**. It provides a minimalist, modern desktop container for web-based playback interfaces.

## Project Versions

### 1. Sasami4K (Main)
The production version of the player, optimized for Windows.
- **Framework**: .NET Framework 4.8 / WPF
- **Rendering Engine**: Microsoft Edge WebView2 (Chromium)
- **Features**: Native Mica/Acrylic effects, high-DPI support, and deep Windows integration.
- **Project**: `Sasami4k.WindowApi/Sasami4k.WindowApi.fsproj`

### 2. Sasami4K Photino (Prototype)
An experimental cross-platform shell.
- **Framework**: .NET 8.0 / Photino.NET
- **Status**: **Prototype** (Cross-platform support for Linux/macOS).
- **Project**: `Sasami4k.Photino/Sasami4k.Photino.fsproj`

## Technology Stack

- **Language**: F# 8.0+
- **UI Framework**: WPF (Windows) / Photino (Cross-platform)
- **Rendering Engine**: Chromium (via WebView2)
- **Aesthetics**: Glassmorphism, Modern Dark Mode, Dynamic Favicons.

## Getting Started

### Prerequisites
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download)
- [WebView2 Runtime](https://developer.microsoft.com/en-us/microsoft-edge/webview2/)

### Build and Run
To build the main Windows version:
```powershell
dotnet build Sasami4k.slnx
dotnet run --project Sasami4k.WindowApi/Sasami4k.WindowApi.fsproj
```

To build the Photino prototype:
```powershell
dotnet build Sasami4k.Photino.slnx
dotnet run --project Sasami4k.Photino/Sasami4k.Photino.fsproj
```

---
*Sasami4K: The modern way to play web media on your desktop.*
