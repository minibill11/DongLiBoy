//------------------------------------------------------------------------------
// <auto-generated>
//     此代码已从模板生成。
//
//     手动更改此文件可能导致应用程序出现意外的行为。
//     如果重新生成代码，将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace JianHeMES.Areas.KongYaHT.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class aircomp3
    {
        public int id { get; set; }
        public Nullable<int> status { get; set; }
        public Nullable<int> slight { get; set; }
        public Nullable<int> severity { get; set; }
        public Nullable<double> pressure { get; set; }
        public Nullable<double> temperature { get; set; }
        public Nullable<double> current_u { get; set; }
        public Nullable<double> current_v { get; set; }
        public Nullable<double> current_w { get; set; }
        public System.DateTime recordingTime { get; set; }
    }
}
