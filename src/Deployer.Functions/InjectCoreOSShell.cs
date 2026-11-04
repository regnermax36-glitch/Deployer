using System.Threading.Tasks;
using Deployer.Functions.Core;
using Zafiro.System.Windows;

namespace Deployer.Functions
{
    /// <summary>
    /// v4 function: Codes and injects the built-in CoreOS Shell into an applied/offline Windows image.
    /// Use after ApplyImage. No external shell files needed.
    /// </summary>
    public class InjectCoreOSShell : DeployerFunction
    {
        private readonly IWindowsImageService imageService;

        public InjectCoreOSShell(IWindowsImageService imageService)
        {
            this.imageService = imageService;
        }

        public async Task Execute(string windowsPath)
        {
            // shellSourcePath not required for built-in coded shell
            await imageService.InjectCoreOSShell(null, windowsPath);
        }
    }
}
