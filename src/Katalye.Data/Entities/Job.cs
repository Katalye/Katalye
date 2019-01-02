using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Katalye.Data.Interfaces;
using Newtonsoft.Json.Linq;

namespace Katalye.Data.Entities
{
    public class Job : IEntity, IAuditable
    {
        public Guid Id { get; set; }

        [StringLength(20, MinimumLength = 20), Required]
        public string Jid { get; set; }

        [Required]
        public string Function { get; set; }

        [Required]
        public string TargetType { get; set; }

        [Required]
        public List<string> Target { get; set; }

        [Required]
        public string User { get; set; }

        public JArray Arguments { get; set; }

        public List<string> MissingMinions { get; set; }

        public DateTimeOffset TimeStamp { get; set; }

        public DateTimeOffset CreatedOn { get; set; }

        public DateTimeOffset ModifiedOn { get; set; }
    }
}