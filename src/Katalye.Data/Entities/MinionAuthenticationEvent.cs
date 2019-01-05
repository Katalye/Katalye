using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Katalye.Data.Interfaces;

namespace Katalye.Data.Entities
{
    public class MinionAuthenticationEvent : IEntity, IAuditable
    {
        public Guid Id { get; set; }

        [Required]
        public Guid? MinionId { get; set; }

        [ForeignKey(nameof(MinionId))]
        public Minion Minion { get; set; }

        public DateTimeOffset Timestamp { get; set; }

        [Required]
        public string Action { get; set; }

        [Required]
        public string PublicKey { get; set; }

        [Required]
        public string PublicKeyHash { get; set; }

        public bool Success { get; set; }

        public DateTimeOffset CreatedOn { get; set; }

        public DateTimeOffset ModifiedOn { get; set; }
    }
}