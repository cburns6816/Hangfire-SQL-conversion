namespace LOJIC.Orchestration.Data.Models
{
    public abstract class BaseServiceHeartbeat : BaseEntity
    {
        public bool ServiceIsAvailable { get; set; }
    }
}
