using System;

namespace Common.Database.Model
{
    public class TestType : EntityBase
    {
        public Guid Uuid { get; set; }
        public string Name { get; set; }
    }
}
