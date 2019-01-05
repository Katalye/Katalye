using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Katalye.Data.Interfaces;

namespace Katalye.Data.Entities
{
    public class MinionGrain : IEntity, IAuditable
    {
        public Guid Id { get; set; }

        [Required]
        public Guid? MinionId { get; set; }

        [ForeignKey(nameof(MinionId))]
        public Minion Minion { get; set; }

        [Required]
        public string Path { get; set; }

        [Required]
        public List<string> Values { get; set; }

        [Required]
        public Guid? Generation { get; set; }

        public DateTimeOffset Timestamp { get; set; }

        public DateTimeOffset CreatedOn { get; set; }

        public DateTimeOffset ModifiedOn { get; set; }
    }
}