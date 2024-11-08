using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lab1.Models
{
    public class LevelResult
    {
        [Key]
        public int QuizResultId { get; set; }
        [ForeignKey("User")]
        public string UserId { get; set; }
        [ForeignKey("Level")]
        public int LevelId { get; set; }
        public int Score { get; set; }
        public DateOnly CompletionDate { get; set; }

        public static implicit operator LevelResult(LevelResult v)
        {
            throw new NotImplementedException();
        }
    }
}
