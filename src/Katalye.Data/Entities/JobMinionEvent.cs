using System;
using System.ComponentModel.DataAnnotations.Schema;

using Katalye.Data.Interfaces;

using Newtonsoft.Json.Linq;

namespace Katalye.Data.Entities
{
    public class JobMinionEvent : IEntity, IAuditable
    {
        public Guid Id { get; set; }

        public JobEventType Type { get; set; }

        [NotMapped]
        public JObject Data { get; set; }

        public DateTimeOffset CreatedOn { get; set; }

        public DateTimeOffset ModifiedOn { get; set; }
    }

    public enum JobEventType
    {
        Progress,
        Return
    }
}