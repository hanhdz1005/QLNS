using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLNS.Core.Dto
{
    public class FileUploadResult
    {
        public string FileName { get; set; }
        public string OriginalName { get; set; }
        public long Size { get; set; }
        public string Url { get; set; }
    }
}
