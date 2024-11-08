using System.ComponentModel.DataAnnotations;

namespace Lab1.Models
{
    public class ResponseApi
    {
        public bool IsSuccess { get; set; } = true;
        public string? Notification { get; set; }
        public object? data { get; set; }
    }
}
