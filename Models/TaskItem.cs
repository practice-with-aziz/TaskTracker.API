using System.ComponentModel.DataAnnotations;

namespace TaskTracker.API.Models
{
    public class TaskItem
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(300)]
        public string Description { get; set; } = string.Empty;
        public DateOnly DueDate { get; set; }
        public bool IsCompleted { get; set; }
        public Priority Priority { get; set; }
    }
    public enum Priority
    {
        Low,
        Medium,
        High
    }
}
