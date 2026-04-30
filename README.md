# Sasami4K

Sasami4K is a modern, high-performance Windows desktop shell built with **F#** and **WPF**. It leverages the **WebView2** runtime to host web-based user interfaces while providing deep integration with the Windows operating system.

## Key Features

- **Modern UI Shell**: A minimalist, borderless window designed to host web applications with a native feel.
- **Windows 11 Integration**: Full support for native Windows 11 aesthetics, including **Mica**, **Acrylic**, and **Tabbed** backdrop effects.
- **Custom Window API**: Exposes a powerful F# backend API to JavaScript, allowing web content to control window state (maximize, minimize, drag, opacity, and always-on-top) natively.
- **Dynamic Titlebar Icons**: Automatically updates the native Windows title bar icon based on the current website's favicon (supporting both high-resolution PNGs and ICO files).
- **Mixed-Language Architecture**: Originally built in C#, now fully migrated to idiomatic **F#** for enhanced safety and performance.

## Technology Stack

- **Language**: F# 8.0+
- **Framework**: .NET Framework 4.8 (Windows)
- **UI Framework**: WPF (Windows Presentation Foundation)
- **Rendering Engine**: Microsoft Edge WebView2 (Chromium)
- **OS**: Windows 10/11 (Mica/Acrylic effects require Windows 11)

## Getting Started

### Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download) or later.
- [WebView2 Runtime](https://developer.microsoft.com/en-us/microsoft-edge/webview2/) (installed by default on modern Windows).

### Build and Run

1. Clone the repository.
2. Open the solution in Visual Studio or your preferred IDE.
3. Restore dependencies and build:
   ```powershell
   dotnet build Sasami4k.slnx
   ```
4. Run the application:
   ```powershell
   dotnet run --project Sasami4k.WindowApi/Sasami4k.WindowApi.fsproj
   ```

## Development

The project is structured to be extremely lightweight:
- `App.fs`: Application entry point and resource management.
- `MainWindow.fs`: Main window logic and WebView2 orchestration.
- `WindowApi.fs`: The bridge between JavaScript and F#.
- `MicaHelper.fs`: Native Win32 interop for Windows 11 styling.
- `TitlebarIconHelper.fs`: Image processing and icon injection.

---
*Built with ❤️ using F# and WebView2.*
