using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace btPrint4wp
{
    public class DemoFile:IDemoFile
    {
        public string filename { get; set; }
        public string filetype { get; set; }
        public string filedescription { get; set; }
        public string fileimage { get; set; }
        public string filehelp { get; set; }

        public DemoFile(string name, string description, string help)
        {
            filename = name;
            filedescription = description;
            filehelp = help;

            filetype = filename.Substring(0, 4);

            if (filetype.StartsWith("fp"))
                fileimage = "images/fp.gif";
            else if (filetype.StartsWith("csim"))
                fileimage = "images/csim.gif";
            else if (filetype.StartsWith("escp"))
                fileimage = "images/escp.gif";
            else if (filetype.StartsWith("ipl"))
                fileimage = "images/ipl.gif";
            else if (filetype.StartsWith("xsim"))
                fileimage = "images/xsim.gif";
            else
                fileimage = "images/pb42.gif";
        }

        public override string ToString()
        {
            return filename;
        }
    }

    public class DemoFiles : IDemoFiles
    {
        List<DemoFile> _demofiles = new List<DemoFile>();
        public List<DemoFile> demofiles
        {
            get { return _demofiles; }
            set { _demofiles = value; }
        }
    }
}
