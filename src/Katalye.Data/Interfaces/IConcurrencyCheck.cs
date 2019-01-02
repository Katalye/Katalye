using System.ComponentModel.DataAnnotations;

namespace Katalye.Data.Interfaces
{
    public interface IConcurrencyCheck
    {
        [ConcurrencyCheck]
        int Version { get; set; }
    }
}