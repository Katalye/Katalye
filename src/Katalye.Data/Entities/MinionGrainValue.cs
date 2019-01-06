using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Katalye.Data.Interfaces;

namespace Katalye.Data.Entities
{
    public class MinionGrainValue : IEntity
    {
        public Guid Id { get; set; }

        [Required]
        public Guid? MinionGrainId { get; set; }

        [ForeignKey(nameof(MinionGrainId))]
        public MinionGrain MinionGrain { get; set; }

        [Required]
        public string Value { get; set; }
    }
}