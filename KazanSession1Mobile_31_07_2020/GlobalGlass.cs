using System;
using System.Collections.Generic;
using System.Text;

namespace KazanSession1Mobile_31_07_2020
{
    public class GlobalGlass
    {
        public class GetCustomViews
        {
            public int AssetID { get; set; }
            public string AssetSN { get; set; }
            public string AssetName { get; set; }
            public string AssetGroup { get; set; }
            public DateTime? Warranty { get; set; }
            public string Department { get; set; }
        }

        public class Department
        {
            public long ID { get; set; }
            public string Name { get; set; }
        }

        public partial class AssetGroup
        {
            public long ID { get; set; }
            public string Name { get; set; }
        }
    }
}
