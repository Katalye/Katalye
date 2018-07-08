using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Katalye.Data.Interfaces;

namespace Katalye.Data.Entities
{
    public class JobMinion : IEntity, IAuditable
    {
        public Guid Id { get; set; }

        [Required]
        public Guid? JobId { get; set; }

        [ForeignKey(nameof(JobId))]
        public Job Job { get; set; }

        [Required]
        public string MinionId { get; set; }

        public DateTimeOffset CreatedOn { get; set; }

        public DateTimeOffset ModifiedOn { get; set; }
    }
}