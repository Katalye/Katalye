using System;
using System.ComponentModel.DataAnnotations;
using Katalye.Data.Interfaces;
using Newtonsoft.Json.Linq;

namespace Katalye.Data.Entities
{
    public class Job : IEntity, IAuditable, IConcurrencyCheck
    {
        public Guid Id { get; set; }

        [StringLength(20, MinimumLength = 20), Required]
        public string Jid { get; set; }

        [Required]
        public string Function { get; set; }

        public JArray Arguments { get; set; }

        [ConcurrencyCheck]
        public int Version { get; set; }

        public DateTimeOffset CreatedOn { get; set; }

        public DateTimeOffset ModifiedOn { get; set; }
    }
}