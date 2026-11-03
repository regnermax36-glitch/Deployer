# Deployer v4
[![Build](https://github.com/regnermax36-glitch/Deployer/actions/workflows/build.yml/badge.svg)](https://github.com/regnermax36-glitch/Deployer/actions/workflows/build.yml)

![Xzibit](Docs/xzibit.png)

## OK, what's this?

"Deploy your deployment while you deploy"

> Deployer v4 is a modernized generic tool to automate complex deployments across architectures.

## v4 Highlights

- **Multi-Architecture Support**: Full support for AMD64 (x64) and ARM64 Shell and GUI modes.
- **CoreOS Architecture**: Modular core OS deployment architecture ready for CoreOS-style and custom system images (expandable).
- **Windows to Lumia Applying**: Enhanced tools for applying Windows 10/11 ARM64 (and AMD64) images to Lumia 950/950XL and similar devices, with improved partitioning, driver injection, and boot configuration.
- **Powerful Shell & Scripting**: Advanced console Shell with Iridio scripting, progress UI, and extensible functions for disk, filesystem, DISM imaging, and more.
- **Avalonia GUI**: Cross-platform ready desktop application.
- **CI Workflow**: Automated GitHub Actions builds producing ready-to-use packages.
- And so much more: better error handling, modern .NET, packaging for Windows, extensible device support (RPi, phones, SOCs).

## How is this useful?

This allows you to install **Windows on ARM** (or other OS) into esoteric devices like the Lumia 950 phones or SOCs like Raspberry Pi.

Deployer v4 builds on the reimagined Deployer 3 foundation to handle almost every kind of deployment you might think of, with a solid core architecture.

## **Super easy to use. No-hassle.**

Please keep reading carefully. All you need is here.

- One of the supported devices
- A Windows 10/11 ARM64 (or AMD64) Image (.wim). Please, check [this link](https://github.com/WOA-Project/guides/blob/master/GettingWOA.md) to get it.

### Additional requirements
- To run Deployer you need a recent version of Windows 10/11
- The .NET 7 runtime (or self-contained builds from CI)

# Download it!

Download the latest version from the [Releases](https://github.com/regnermax36-glitch/Deployer/releases) or grab artifacts from the [CI builds](https://github.com/regnermax36-glitch/Deployer/actions).

# Executing the tool
1. Extract the .zip
2. Find Deployer.Desktop.exe file
3. Run it (you will be prompted for admin rights)

Or use the Console Shell for scripted deployments.

# Show the love 🧡

If you feel this project has been useful for you, please, consider supporting the original authors and contributors.

# Need help?
For Lumia phones: don't hesitate to join our great [Telegram group 📱](https://t.me/joinchat/Ey6mehEPg0Fe4utQNZ9yjA)

# Credits and Acknowledgements
## Lumia

- [Ben Imbushuo](https://github.com/imbushuo) for his awesome work with UEFI and misc. stuff.
- [Gustave M.](https://twitter.com/gus33000) for his HUGE load of work on drivers, testing, fixing... For his support, suggestions, for testing and those neat pieces of code!
- René Lergner ([Heathcliff74XDA](http://www.twitter.com/Heathcliff74XDA)) for WPInternals and for the code to read info from the phone. You started everything 😉
- [Googulator](https://github.com/Googulator). For his work on the USB-C and for the great support. 
- Swift (AppleCyclone) for suggestions and his work with the rest of team.
- Abdel [ADeltaX](https://twitter.com/ADeltaXForce?s=17) for testing and for his work.

## Raspberry Pi Support

This WOA Deployer is possible because the great community behind it. I would like to thank the brilliant minds behind this technical wonder. If you think you should be listed, please, contact me using the e-mail address on my profile.

- [Andrei Warkentin](https://github.com/andreiw) for the **64-bit Pi UEFI**, UEFI Pi (HDMI, USB, SD/MMC) drivers, improved ATF and Windows boot/runtime support.
- [MCCI](https://mcci.com/) for their great contribution to the RaspberryPI WOA project:
  - for porting their **TrueTask USB stack** to Windows 10 ARM64, and allowing non-commercial use with this project ([see license](Docs/mcci_license.md))
  - for funding the site of the project http://pi64.win and the discourse site http://discourse.pi64.win
  - Special thanks to Terry Moore for all the great support and commitment, and for setting up the online presence for the project and its infrastructure.
- [Ard Bisheuvel](http://www.workofard.com/2017/02/uefi-on-the-pi/) for initial ATF and UEFI ports
- [Googulator](https://github.com/Googulator) for his method to install WOA in the Raspberry Pi
- Bas Timmer ([@NTauthority](https://twitter.com/ntauthority)) for leaving ample thick hints that led to the development of HypDXe and the first bootable WOA builds
- Microsoft for their original [32-bit UEFI for Pi](https://github.com/ms-iot/RPi-UEFI), [Windows BSP drivers](https://github.com/ms-iot/rpi-iotcore), and for removing the HAL check that required HypDxe in the first place, so now we can run any new build.
- Mario Bălănică for his [awesome tool](https://www.worproject.ml), and for tips and support :)
	- daveb77
    - thchi12
    - falkor2k15
    - driver1998
    - XperfectTR
    - woachk
    - novaspirit
    - zlockard 
     
    ...for everything from ACPI/driver work to installation procedures, testing and so on.
- Microsoft for the 32-bit IoT firmware.

In addition to:

- [Ian Johnson](https://github.com/ipjohnson) for his wonderful DI-IOC container: [Grace](https://github.com/ipjohnson/Grace)
- [Eric Zimmerman](https://github.com/EricZimmerman) for [The Registry Project](https://github.com/EricZimmerman/Registry)
- [Jan Karger](https://github.com/punker76) for his wonderful [MahApps.Metro](https://mahapps.com)
- [ReactiveUI](https://reactiveui.net)
- [Adam Hathcock](https://github.com/adamhathcock) for [SharpCompress](https://github.com/adamhathcock/sharpcompress)
- [Etcher](https://www.balena.io/etcher/), the perfect tool for flashing.

And our wonderful group at Telegram for their testing and support!
- [RaspberryPiWOA](https://t.me/raspberrypiwoa)

## Related projects
These are the related projects. The Core Packages comes from them. Big thanks!

- [RaspberryPiPkg](https://github.com/andreiw/RaspberryPiPkg)
- [Microsoft IoT-BSP](https://github.com/ms-iot/bsp)
- [Raspberry Pi ATF](https://github.com/andreiw/raspberry-pi3-atf)
- [WOR Project](https://www.worproject.ml) by [Mario Bălănică](https://github.com/mariobalanica)

In addition to:

- [Ian Johnson](https://github.com/ipjohnson) for his wonderful DI-IOC container: [Grace](https://github.com/ipjohnson/Grace)
- [Eric Zimmerman](https://github.com/EricZimmerman) for [The Registry Project](https://github.com/EricZimmerman/Registry)
- [Jan Karger](https://github.com/punker76) [MahApps.Metro](https://mahapps.com)
- [ReactiveUI](https://reactiveui.net)
- [Adam Hathcock](https://github.com/adamhathcock) for [SharpCompress](https://github.com/adamhathcock/sharpcompress)
- [Markdown XAML](https://github.com/theunrepentantgeek/Markdown.XAML)
- [Serilog](https://serilog.net/)
- [Thomas Galliker](https://www.linkedin.com/in/thomasgalliker/?originalSubdomain=ch) for his great [Value Converters](https://github.com/thomasgalliker/ValueConverters.NET)

This project is [Powered by AvaloniaUI](https://www.avaloniaui.net).

![image](https://github.com/SuperJMN/Deployer/assets/3109851/1752bd42-beb5-43f9-8c27-d0d9a3c79ac9)
