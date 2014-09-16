using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace btPrint4wp
{
    public interface IDemoFile
    {
        string filename { get; set; }
        string filetype { get; set; }
        string filedescription { get; set; }
        string fileimage { get; set; }
        string filehelp { get; set; }
    }

    public interface IDemoFiles
    {
        List<DemoFile> demofiles { get; }
    }
}
