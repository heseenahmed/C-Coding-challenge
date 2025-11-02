using System.ComponentModel.DataAnnotations;

namespace InterviewTest.Entities
{
    public class User
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [MaxLength(120)]
        public string Name { get; set; } = null!;

        public int Age { get; set; }

        [MaxLength(200)]
        public string Email { get; set; } = null!;

        public DateTime TimeStamp { get; set; } = DateTime.UtcNow;
    }
}
