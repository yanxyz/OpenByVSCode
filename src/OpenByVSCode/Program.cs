using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenByVSCode
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Utils.Start(args);
            }
            catch (Exception ex)
            {
                Utils.ShowError(ex.Message);
#if DEBUG       
                throw;
#endif
            }
        }
    }
}
