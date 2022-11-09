using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace todo_domain_entities
{
    /// <summary>
    /// ToDoList class that represents list of T0D0 entries
    /// </summary>
    public class ToDoList : IValidatableObject
    {
        public int Id { get; set; }

        [Required]
        public string MainTitle { get; set; }

        public virtual List<ToDoEntry> ToDoEntries { get; set; } = new List<ToDoEntry>();

        [NotMapped]
        public ToDoStatus Status
        {
            get
            {
                if (ToDoEntries != null && ToDoEntries.All(list => list.Status == ToDoStatus.Completed))
                {
                    return ToDoStatus.Completed;
                }
                else
                {
                    return ToDoStatus.NotStarted; 
                }
            }
        }

        public bool IsVisible { get; set; } = true;

        public override bool Equals(object obj)
        {
            return (obj as ToDoList)?.Id == this.Id;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id);
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var errors = new List<ValidationResult>();

            if(string.IsNullOrEmpty(MainTitle))
            {
                errors.Add(new ValidationResult("MainTitle in not set"));
            }

            for(var i = 0; i < ToDoEntries.Count-1; ++i)
            {
                if(ToDoEntries[i].OrdinalNumber == ToDoEntries[i+1].OrdinalNumber)
                {
                    errors.Add(new ValidationResult("There is a duplicate values of OrdinalNumber in ToDoEntries list"));
                    break;
                }
            }

            return errors;
        }

        public bool IsTheSame(ToDoList item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            if (!string.Equals(MainTitle, item.MainTitle))
                return false;

            if (!ToDoEntries.SequenceEqual(item.ToDoEntries))
                return false;

            return true;
        }
    }
}
