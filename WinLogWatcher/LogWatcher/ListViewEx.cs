using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LogWatcher
{
    class ListViewEx: System.Windows.Forms.ListView
    {

        public ListViewEx(): base()
        {
            this.DoubleBuffered = true;
        }
    }
}
