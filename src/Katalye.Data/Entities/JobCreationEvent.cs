using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Katalye.Data.Interfaces;

namespace Katalye.Data.Entities
{
    public class JobCreationEvent : IEntity, IAuditable
    {
        public Guid Id { get; set; }

        [Required]
        public Guid? JobId { get; set; }

        [ForeignKey(nameof(JobId))]
        public Job Job { get; set; }

        public DateTimeOffset Timestamp { get; set; }

        [Required]
        public string User { get; set; }

        public List<string> Minions { get; set; }

        public List<string> MissingMinions { get; set; }

        public List<string> Targets { get; set; }

        [Required]
        public string TargetType { get; set; }

        public DateTimeOffset CreatedOn { get; set; }

        public DateTimeOffset ModifiedOn { get; set; }
    }
}