using System.Collections.Generic;
using domi1819.UpServer.Web;

namespace domi1819.UpServer.Console.Commands
{
    internal class CommandReloadWebContent : BaseCommand
    {
        private readonly UpWebService webService;

        public CommandReloadWebContent(BaseCommand parent, UpWebService webService) : base(parent)
        {
            this.webService = webService;
        }
        
        protected override Result Run(IEnumerable<string> input)
        {
            this.webService.ReloadCachedFiles();

            Feedback.WriteLine("Cached Files were reloaded.");

            return Result.Default;
        }
    }
}
