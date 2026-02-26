<p align="center">
  <h1 align="center">🚀 Enumium</h1>
  <p align="center">
    <strong>Professional Windows System Optimizer</strong>
  </p>
  <p align="center">
    <img src="https://img.shields.io/badge/.NET-8.0-512BD4?style=for-the-badge&logo=dotnet" alt=".NET 8" />
    <img src="https://img.shields.io/badge/WPF-Desktop-0078D4?style=for-the-badge&logo=windows" alt="WPF" />
    <img src="https://img.shields.io/badge/C%23-12-239120?style=for-the-badge&logo=csharp" alt="C#" />
    <img src="https://img.shields.io/badge/License-MIT-yellow?style=for-the-badge" alt="MIT License" />
  </p>
</p>

---

**Enumium** is a free, open-source Windows system optimization tool built with WPF and .NET 8. It provides a modern, beautiful dashboard with real-time system monitoring and a full suite of optimization tools — all in one place.

## ✨ Features

| Module | Description |
|--------|-------------|
| 📊 **Dashboard** | Real-time CPU, RAM, disk, and GPU monitoring with animated gauges |
| 🧹 **Cleaner** | Scan and remove temp files, browser cache, recycle bin junk |
| 🚀 **Startup Manager** | Enable/disable startup programs to speed up boot times |
| 🔒 **Privacy** | Disable telemetry, clear activity history, manage privacy settings |
| 🌐 **Network** | DNS optimization, network diagnostics, adapter info |
| ⚡ **Performance** | System tweaks and performance optimizations |
| 🛠️ **Tools** | Quick-access system utilities and maintenance tools |
| 📋 **Reports** | Generate and export system optimization reports |

## 🌍 Languages

Enumium supports **English** and **Russian** out of the box, selectable from the splash screen.

## 🏗️ Building from Source

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) (or later)
- Windows 10/11

### Build & Run

```bash
git clone https://github.com/enum000/Enumium.git
cd Enumium
dotnet build
dotnet run --project Enumium
```

## 📁 Project Structure

```
Enumium/
├── Enumium.sln              # Solution file
└── Enumium/
    ├── App.xaml              # Application entry point
    ├── Enumium.csproj        # Project configuration
    ├── Helpers/              # Utility classes (RelayCommand, converters)
    ├── Models/               # Data models
    ├── Services/             # Core services (Cleaner, Network, Privacy, etc.)
    ├── Themes/               # WPF styles and themes
    ├── ViewModels/           # MVVM ViewModels
    └── Views/                # XAML views and windows
```

## 🤝 Contributing

Contributions are welcome! Feel free to:

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## 📄 License

This project is licensed under the **MIT License** — see the [LICENSE](LICENSE) file for details.

---

<p align="center">
  Made with ❤️ by <a href="https://github.com/enum000">enum000</a>
</p>
