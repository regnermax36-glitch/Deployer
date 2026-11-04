using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using Serilog;
using Zafiro.Core;
using Zafiro.Core.Mixins;
using Zafiro.Storage;

namespace Zafiro.System.Windows.Dism
{
    public abstract class ImageServiceBase : IWindowsImageService
    {
        private readonly IFileSystem fileSystem;

        protected ImageServiceBase(IFileSystem fileSystem)
        {
            this.fileSystem = fileSystem;
        }

        public abstract Task ApplyImage(IPartition target, string imagePath, int imageIndex = 1, bool useCompact = false,
            IOperationProgress progressObserver = null, CancellationToken token = default(CancellationToken));

        protected void EnsureValidParameters(string applyPath, string imagePath, int imageIndex)
        {
            if (applyPath == null)
            {
                throw new ArgumentNullException(nameof(applyPath));
            }

            if (applyPath == null)
            {
                throw new ArgumentException("The volume to apply the image is invalid");
            }

            if (imagePath == null)
            {
                throw new ArgumentNullException(nameof(imagePath));
            }

            EnsureValidImage(imagePath, imageIndex);
        }

        private void EnsureValidImage(string imagePath, int imageIndex)
        {
            Log.Verbose("Checking image at {Path}, with index {Index}", imagePath, imageIndex);

            if (!fileSystem.File.Exists(imagePath))
            {
                throw new FileNotFoundException($"Image not found: {imagePath}. Please, verify that the file exists and it's accessible.");
            }

            Log.Verbose("Image file at '{ImagePath}' exists", imagePath);                    
        }

        public async Task<IList<string>> InjectDrivers(string path, string windowsRootPath)
        {
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            var outputSubject = new Subject<string>();

            var subscription = outputSubject.Subscribe(Log.Verbose);

            var args = new[]
            {
                "/Add-Driver",
                $"/Image:{windowsRootPath}",
                $@"/Driver:""{path}""",
                IsUniqueFile(path) ? "" : "/Recurse",
            };

            var processResults = await Process.Run(ToolPaths.Dism, args.Join(" "), outputSubject, outputSubject);
            subscription.Dispose();
            
            if (processResults.ExitCode != 0)
            {
                throw new Exception(
                    $"There has been a problem during deployment: DISM exited with code {processResults.ExitCode}. Output: {processResults.StandardOutput.Join()}");
            }

            return Enumerable.ToList<string>(StringMixin.ExtractFileNames(string.Concat((IEnumerable<string>) processResults.StandardOutput)));
        }

        /// <summary>
        /// v4: Codes and injects a built-in CoreOS-style Shell into the offline Windows image for Lumia application.
        /// No external files required. Places interactive shell under Windows\System32\CoreOSShell.
        /// </summary>
        public async Task InjectCoreOSShell(string shellSourcePath, string windowsRootPath)
        {
            // shellSourcePath is ignored for built-in coded shell; kept for API compatibility
            if (string.IsNullOrWhiteSpace(windowsRootPath))
                throw new ArgumentNullException(nameof(windowsRootPath));

            var targetDir = fileSystem.Path.Combine(windowsRootPath, "Windows", "System32", "CoreOSShell");
            fileSystem.Directory.CreateDirectory(targetDir);

            Log.Information("Coding and injecting built-in CoreOS Shell into {Target}", targetDir);

            // Code the main interactive shell
            var shellCmd = @"@echo off
setlocal enabledelayedexpansion
title CoreOS Shell v4 - Deployer for Lumia
:menu
cls
echo =========================================
echo   CoreOS Shell - Windows on Lumia Edition
echo   Injected by Deployer v4
echo =========================================
echo.
echo 1. System Status
echo 2. Disk and Partition Info
echo 3. Network Configuration
echo 4. Service Management
echo 5. Process List
echo 6. Open Command Prompt
echo 7. Reboot System
echo 8. Exit Shell
echo.
set /p choice=Select option: 

if "%choice%"=="1" goto status
if "%choice%"=="2" goto disk
if "%choice%"=="3" goto network
if "%choice%"=="4" goto services
if "%choice%"=="5" goto processes
if "%choice%"=="6" goto cmd
if "%choice%"=="7" goto reboot
if "%choice%"=="8" exit
goto menu

:status
echo.
echo === System Status ===
systeminfo | findstr /B /C:"OS Name" /C:"OS Version" /C:"System Type" /C:"Total Physical Memory"
echo.
pause
goto menu

:disk
echo.
echo === Disk Info ===
wmic logicaldisk get name,size,freespace,description
echo.
pause
goto menu

:network
echo.
echo === Network ===
ipconfig /all
echo.
pause
goto menu

:services
echo.
echo === Services (running) ===
sc query type= service state= all | findstr "SERVICE_NAME STATE"
echo.
pause
goto menu

:processes
echo.
echo === Processes ===
tasklist
echo.
pause
goto menu

:cmd
cmd
goto menu

:reboot
shutdown /r /t 5
exit
";
            await fileSystem.File.WriteAllTextAsync(fileSystem.Path.Combine(targetDir, "CoreOSShell.cmd"), shellCmd);

            // Additional helper scripts
            var statusPs1 = @"# CoreOS Shell Status Module
Write-Host 'CoreOS Shell Status' -ForegroundColor Cyan
Get-ComputerInfo | Select-Object WindowsProductName, OsArchitecture, CsProcessors
";
            await fileSystem.File.WriteAllTextAsync(fileSystem.Path.Combine(targetDir, "status.ps1"), statusPs1);

            var readme = @"CoreOS Shell v4
Coded and injected by Deployer.
Designed for modified Windows applied to Lumia devices.
Launch with: CoreOSShell.cmd
";
            await fileSystem.File.WriteAllTextAsync(fileSystem.Path.Combine(targetDir, "README.txt"), readme);

            Log.Information("Built-in CoreOS Shell coded and injected successfully. Windows is ready for Lumia application.");
        }

        private void CopyDirectory(string source, string target)
        {
            fileSystem.Directory.CreateDirectory(target);
            foreach (var file in fileSystem.Directory.GetFiles(source))
            {
                var dest = fileSystem.Path.Combine(target, fileSystem.Path.GetFileName(file));
                fileSystem.File.Copy(file, dest, overwrite: true);
            }
            foreach (var dir in fileSystem.Directory.GetDirectories(source))
            {
                var destDir = fileSystem.Path.Combine(target, fileSystem.Path.GetFileName(dir));
                CopyDirectory(dir, destDir);
            }
        }

        private bool IsUniqueFile(string path)
        {
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            var hasInfExt = Path.GetExtension(path).Equals(".inf", StringComparison.InvariantCultureIgnoreCase);
            return hasInfExt && fileSystem.File.Exists(path);
        }

        public async Task RemoveDriver(string path, string windowsRootPath)
        {
            var outputSubject = new Subject<string>();
            var subscription = outputSubject.Subscribe(Log.Verbose);
            var processResults = await Process.Run(ToolPaths.Dism, $@"/Remove-Driver /Image:{windowsRootPath} /Driver:""{path}""", outputSubject,
                outputSubject);
            subscription.Dispose();
            
            if (processResults.ExitCode != 0)
            {
                throw new Exception(
                    $"There has been a problem during removal: DISM exited with code {processResults}.");
            }
        }

        public abstract Task CaptureImage(IPartition source, string destination,
            IOperationProgress progressObserver = null, CancellationToken cancellationToken = default(CancellationToken));

        public abstract Task ApplyImage(string targetDriveRoot, string imagePath, int imageIndex = 1, bool useCompact = false,
            IOperationProgress progressObserver = null, CancellationToken token = default(CancellationToken));
    }
}
