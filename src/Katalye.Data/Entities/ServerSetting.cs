using System;
using System.ComponentModel.DataAnnotations;
using Katalye.Data.Interfaces;

namespace Katalye.Data.Entities
{
    public class ServerSetting : IAuditable, IConcurrencyCheck
    {
        [Required, Key]
        public string Key { get; set; }

        [Required]
        public string Value { get; set; }

        [ConcurrencyCheck]
        public int Version { get; set; }

        public DateTimeOffset? LastUpdated { get; set; }

        public DateTimeOffset CreatedOn { get; set; }

        public DateTimeOffset ModifiedOn { get; set; }
    }
}