using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace todo_domain_entities
{
    public enum ToDoStatus
    {
        [Display(Name = "Completed")]
        Completed,

        [Display(Name = "In progress")]
        InProgress,

        [Display(Name = "Not started")]
        NotStarted
    }

    /// <summary>
    /// ToDoEntry class that represents T0D0 entry of T0D0 list
    /// </summary>
    public class ToDoEntry : IValidatableObject
    {
        public int Id { get; set; }

        [Required]
        public int OrdinalNumber { get; set; }

        [Required]
        public string Title { get; set; }

        public string Description { get; set; } = "-";

        public DateTime? DueDate { get; set; } = DateTime.Now;

        public DateTime CreationDate { get; private set; } = DateTime.Now;

        [Required]
        public ToDoStatus Status { get; set; } = ToDoStatus.NotStarted;

        public string Notes { get; set; } = "-";

        [Required, JsonIgnore, IgnoreDataMember]
        public virtual ToDoList ToDoList { get; set; }

        public override bool Equals(object obj)
        {
            return (obj as ToDoEntry)?.Id == this.Id;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id);
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var errors = new List<ValidationResult>();

            if(OrdinalNumber == 0)
            {
                errors.Add(new ValidationResult("OrdinalNumber can't be 0"));
            }

            if(string.IsNullOrEmpty(Title))
            {
                errors.Add(new ValidationResult("Title is not set"));
            }

            return errors;
        }

        public bool IsTheSame(ToDoEntry item)
        {
            if(item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            if (!string.Equals(Title, item.Title))
                return false;

            if (!string.Equals(Title, item.Title))
                return false;

            if (DueDate != item.DueDate)
                return false;

            if (!ToDoList.IsTheSame(item.ToDoList))
                return false;

            return true;
        }
    }
}
