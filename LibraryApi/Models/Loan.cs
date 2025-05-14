using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryApi.Models
{
    public class Loan
    {
        public int Id { get; set; }

        public int BookId { get; set; }

        [ForeignKey("BookId")]
        public Book? Book { get; set; }

        public int UserId { get; set; }

        public DateTime LoanDate { get; set; }
    }
}
