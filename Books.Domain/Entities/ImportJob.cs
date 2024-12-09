using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Books.Domain.Entities
{
  public  class ImportJob
    {
        public int Id { get; set; }
        public Guid DomainId { get; set; }

        [Required(ErrorMessage ="{0} is required")]
        public string? FileName { get; set; }

        public JobStatus Status { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? StartedAt { get; set; }

        public DateTime? CompletedAt { get; set; }

        public DateTime? FailedAt { get; set; }

        public string? FailureReason { get; set; }


    }
    public enum JobStatus
    {
        Undefined=0,
        Enqueued=1,
        Running=2,
        Completed=3,
        Failed=4
    }
}
