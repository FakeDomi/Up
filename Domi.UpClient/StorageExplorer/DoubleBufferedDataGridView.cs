using System.Windows.Forms;

namespace domi1819.UpClient.StorageExplorer
{
    internal class DoubleBufferedDataGridView : DataGridView
    {
        internal DoubleBufferedDataGridView()
        {
            this.DoubleBuffered = true;
        }
    }
}
