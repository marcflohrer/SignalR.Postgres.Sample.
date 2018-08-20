using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace StockDatabase.Models.Core
{
    public abstract class BaseEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public override bool Equals(object obj)
        {
            var entity = obj as BaseEntity;
            return entity != null &&
                   Id == entity.Id;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}
