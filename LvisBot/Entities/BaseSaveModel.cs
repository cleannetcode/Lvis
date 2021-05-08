using System;
using System.Collections.Generic;

namespace LvisBot.DataAccess.Entities
{
    public class BaseSaveModel
    {
        public DateTime DateTime { get; set; }
        public long SecondFromStreamStart { get; set; }
        public string UserName { get; set; }
        public string Message { get; set; }
        public string TimeSpan { get; set; }

        public override bool Equals(object obj)
        {
            return obj is BaseSaveModel model &&
                   TimeSpan == model.TimeSpan;
        }

        public override int GetHashCode()
        {
            return 1985640572 + EqualityComparer<string>.Default.GetHashCode(TimeSpan);
        }
    }
}
