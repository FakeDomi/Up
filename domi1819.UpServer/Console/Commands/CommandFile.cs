using System.Collections.Generic;

namespace domi1819.UpServer.Console.Commands
{
    internal class CommandFile : BaseCommand
    {
        public CommandFile(BaseCommand parent, FileManager files) : base(parent)
        {
            this.SubCommands.Add("drop", new CommandFileDrop(this, files));
        }

        private class CommandFileDrop : BaseCommand
        {
            private FileManager files;

            public CommandFileDrop(BaseCommand parent, FileManager files) : base(parent)
            {
                this.files = files;
            }

            protected override Result Run(List<string> input)
            {
                this.files.SetDownloadable(input[input.Count - 1], false);

                return Result.Default;
            }
        }
    }
}
