using System;

namespace LOJIC.Orchestration.Data.Models
{
    public class BaseEntity
    {
        public int Id { get; set; }
        public DateTimeOffset Created { get; set; }
    }
}
