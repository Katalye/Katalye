using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Katalye.Data.Interfaces;
using Newtonsoft.Json.Linq;

namespace Katalye.Data.Entities
{
    public class MinionReturnEvent : IEntity, IAuditable
    {
        public Guid Id { get; set; }

        [Required]
        public Guid? MinionId { get; set; }

        [ForeignKey(nameof(MinionId))]
        public Minion Minion { get; set; }

        [Required]
        public Guid? JobId { get; set; }

        [ForeignKey(nameof(JobId))]
        public Job Job { get; set; }

        [NotMapped]
        public JObject ReturnData { get; set; }

        public bool Success { get; set; }

        public long ReturnCode { get; set; }

        public DateTimeOffset Timestamp { get; set; }

        public DateTimeOffset CreatedOn { get; set; }

        public DateTimeOffset ModifiedOn { get; set; }
    }
}