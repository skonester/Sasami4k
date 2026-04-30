# Sasami4K

Sasami4K is a high-performance **WebView2-based media player** shell built with **F#**. It provides a minimalist, modern desktop container for web-based playback interfaces, now upgraded to the latest .NET technologies.

## Project Versions

### 1. Sasami4K (WPF)
The original Windows-focused player, now modernized for the latest .NET runtime.
- **Framework**: .NET 10.0-windows / WPF
- **Rendering Engine**: Microsoft Edge WebView2 (Chromium)
- **Features**: Native Mica/Acrylic support, high-DPI awareness, and deep Windows integration.
- **Project**: `Sasami4k.WindowApi/Sasami4k.WindowApi.fsproj`

### 2. Sasami4K Photino (Cross-Platform)
A lightweight, cross-platform shell designed for performance and portability.
- **Framework**: .NET 10.0 / Photino.NET 2.6.0
- **Status**: Stable (Windows/Linux/macOS support).
- **Project**: `Sasami4k.Photino/Sasami4k.Photino.fsproj`

## Technology Stack

- **Language**: F# 9.0+
- **Framework**: .NET 10.0
- **UI Libraries**: WPF (Windows) / Photino.NET (Cross-platform)
- **Rendering Engine**: Chromium (via WebView2)
- **Aesthetics**: Glassmorphism, Modern Dark Mode, Dynamic Favicons.

## Getting Started

### Prerequisites
- [.NET 10.0 SDK](https://dotnet.microsoft.com/download)
- [WebView2 Runtime](https://developer.microsoft.com/en-us/microsoft-edge/webview2/) (Included in Windows 10/11)

### Build and Run
To build the solution:
```powershell
dotnet build Sasami4k.slnx
```

To run the WPF version (Windows only):
```powershell
dotnet run --project Sasami4k.WindowApi/Sasami4k.WindowApi.fsproj
```

To run the Photino version:
```powershell
dotnet run --project Sasami4k.Photino/Sasami4k.Photino.fsproj
```

---
*Sasami4K: The modern way to play web media on your desktop.*

**Specially Advanced Synchronized Accessible Media**
