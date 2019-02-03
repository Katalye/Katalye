using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Katalye.Data.Common;
using Katalye.Data.Interfaces;

namespace Katalye.Data.Entities
{
    public class AdHocTask : IEntity, IAuditable
    {
        public Guid Id { get; set; }

        [Required]
        public string Tag { get; set; }

        public BackgroundTaskStatus Status { get; set; }

        [NotMapped]
        public Dictionary<string, string> Metadata { get; set; }

        public DateTimeOffset StartedOn { get; set; }

        public DateTimeOffset CreatedOn { get; set; }

        public DateTimeOffset ModifiedOn { get; set; }
    }
}