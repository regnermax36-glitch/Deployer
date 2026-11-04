using System.Threading.Tasks;
using Deployer.Functions.Core;
using Zafiro.System.Windows;

namespace Deployer.Functions
{
    /// <summary>
    /// v4 function: Injects a CoreOS Shell into an applied/offline Windows image.
    /// Use after ApplyImage and before finalizing for Lumia deployment.
    /// </summary>
    public class InjectCoreOSShell : DeployerFunction
    {
        private readonly IWindowsImageService imageService;

        public InjectCoreOSShell(IWindowsImageService imageService)
        {
            this.imageService = imageService;
        }

        public async Task Execute(string shellSourcePath, string windowsPath)
        {
            await imageService.InjectCoreOSShell(shellSourcePath, windowsPath);
        }
    }
}
