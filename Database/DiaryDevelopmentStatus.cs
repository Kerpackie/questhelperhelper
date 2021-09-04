using System;
using System.ComponentModel.DataAnnotations;

namespace questhelperhelper.Database
{
    public partial class DiaryDevelopmentStatus
    {
        [Key]
        public long DiaryID { get; set; }
        public string DiaryName { get; set; }
        public string DiaryStatus { get; set; }
    }
}