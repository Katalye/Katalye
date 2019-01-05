using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Katalye.Data.Interfaces;
using Newtonsoft.Json.Linq;

namespace Katalye.Data.Entities
{
    public class UnknownEvent: IEntity, IAuditable
    {
        public Guid Id { get; set; }

        [Required]
        public string Tag { get; set; }

        [NotMapped]
        public JToken Data { get; set; }

        public DateTimeOffset CreatedOn { get; set; }
        public DateTimeOffset ModifiedOn { get; set; }
    }
}