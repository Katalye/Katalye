using System;
using System.ComponentModel.DataAnnotations;
using Katalye.Data.Interfaces;

namespace Katalye.Data.Entities
{
    public class Minion : IEntity, IAuditable, IConcurrencyCheck
    {
        public Guid Id { get; set; }

        [Required]
        public string MinionSlug { get; set; }

        public DateTimeOffset? LastAuthentication { get; set; }

        public int Version { get; set; }

        public DateTimeOffset CreatedOn { get; set; }

        public DateTimeOffset ModifiedOn { get; set; }
    }
}