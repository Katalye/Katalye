using System;

namespace Katalye.Data.Interfaces
{
    public interface IAuditable
    {
        DateTimeOffset CreatedOn { get; set; }

        DateTimeOffset ModifiedOn { get; set; }
    }
}