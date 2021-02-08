using System;
using System.Collections.Generic;

namespace domi1819.UpServer.Console.Commands
{
    internal class CommandUserFind : BaseCommand
    {
        private const int BatchSize = 5;
        private readonly UserManager users;

        public CommandUserFind(BaseCommand parent, UserManager users) : base(parent)
        {
            this.users = users;
        }

        protected override Result Run(IEnumerable<string> input)
        {
            Feedback.ReadString("Search string?", null, null, out string userInput);
            Feedback.WriteLine($"-- Press ENTER or Down Arrow to view the next {BatchSize} results --");

            using (IEnumerator<string> results = this.users.Find(userInput).GetEnumerator())
            {
                ConsoleKey key;
                do
                {
                    int remainingBatchElements = BatchSize;
                    bool hasMoreResults = false;

                    while (remainingBatchElements > 0 && (hasMoreResults = results.MoveNext()))
                    {
                        Feedback.WriteLine(results.Current);
                        remainingBatchElements--;
                    }

                    if (!hasMoreResults)
                    {
                        Feedback.WriteLine("-- End of results --");
                        break;
                    }

                    key = System.Console.ReadKey().Key;
                } while (key == ConsoleKey.Enter || key == ConsoleKey.DownArrow);
            }

            return Result.Default;
        }
    }
}
