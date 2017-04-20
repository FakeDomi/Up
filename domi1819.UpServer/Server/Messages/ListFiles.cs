using System;
using System.Collections.Generic;
using domi1819.NanoDB;
using domi1819.UpCore.Utilities;

namespace domi1819.UpServer.Server.Messages
{
    internal class ListFiles : IMessage
    {
        private readonly FileManager files;
        private readonly UserManager users;

        public ListFiles(FileManager files, UserManager users)
        {
            this.files = files;
            this.users = users;
        }

        public void OnMessage(MessageContext context, Connection connection)
        {
            string userId = connection.UserId;

            if (!this.users.HasUser(userId))
            {
                context.Disconnect = true;
                return;
            }

            int offset = context.ReadNextInt();

            List<NanoDBLine> fileList = this.files.GetFiles(userId);

            if (fileList == null)
            {
                context.Disconnect = true;
                return;
            }

            DateTime fromDate = context.ReadNextDateTime();
            DateTime toDate = context.ReadNextDateTime();
            long fromSize = context.ReadNextLong();
            long toSize = context.ReadNextLong();
            string filter = context.ReadNextString();
            int filterMatchMode = context.ReadNextInt();

            int currentFileIndex = offset, writtenFiles = 0;
            int startIndex = context.MessageWriter.Index;
            context.MessageWriter.Index += 4;

            while (writtenFiles < Constants.Network.MaxFilesPerPacket && currentFileIndex < fileList.Count)
            {
                if (this.files.SerializeFileInfo(fileList[currentFileIndex], context.MessageWriter, fromDate, toDate, fromSize, toSize, filter, filterMatchMode))
                {
                    writtenFiles++;
                }
                currentFileIndex++;
            }

            context.WriteNextInt(currentFileIndex == fileList.Count ? -1 : currentFileIndex);
            context.MessageWriter.InsertInt(writtenFiles, startIndex);
        }
    }
}
