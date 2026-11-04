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
        /// v4: Injects CoreOS Shell files into the offline Windows image so the modified Windows can be applied to Lumia.
        /// Places shell content under Windows\System32\CoreOSShell and creates a simple launcher.
        /// </summary>
        public async Task InjectCoreOSShell(string shellSourcePath, string windowsRootPath)
        {
            if (string.IsNullOrWhiteSpace(shellSourcePath))
                throw new ArgumentNullException(nameof(shellSourcePath));
            if (string.IsNullOrWhiteSpace(windowsRootPath))
                throw new ArgumentNullException(nameof(windowsRootPath));

            if (!fileSystem.Directory.Exists(shellSourcePath) && !fileSystem.File.Exists(shellSourcePath))
                throw new DirectoryNotFoundException($"CoreOS Shell source not found: {shellSourcePath}");

            var targetDir = fileSystem.Path.Combine(windowsRootPath, "Windows", "System32", "CoreOSShell");
            fileSystem.Directory.CreateDirectory(targetDir);

            Log.Information("Injecting CoreOS Shell from {Source} into {Target}", shellSourcePath, targetDir);

            // Copy files (supports directory or single file)
            if (fileSystem.Directory.Exists(shellSourcePath))
            {
                CopyDirectory(shellSourcePath, targetDir);
            }
            else
            {
                var destFile = fileSystem.Path.Combine(targetDir, fileSystem.Path.GetFileName(shellSourcePath));
                fileSystem.File.Copy(shellSourcePath, destFile, overwrite: true);
            }

            // Create a simple launcher batch/cmd that can be called from Windows
            var launcherPath = fileSystem.Path.Combine(targetDir, "CoreOSShell.cmd");
            var launcherContent = "@echo off\r\necho CoreOS Shell injected by Deployer v4\r\necho Ready for Lumia deployment\r\ncd /d \"%~dp0\"\r\n";
            await fileSystem.File.WriteAllTextAsync(launcherPath, launcherContent);

            Log.Information("CoreOS Shell injection completed. Modified Windows image is ready for Lumia application.");
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
