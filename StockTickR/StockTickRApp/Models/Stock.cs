using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using StockTickR.Repositories.Core;

namespace StockTickR.Models
{
    public class Stock : BaseEntity
    {
        [ForeignKey("SymbolId")]
        public Symbol Symbol { get; set; }

        public decimal DayOpen { get; set; }

        public decimal DayLow { get; set; }

        public decimal DayHigh { get; set; }

        public decimal LastChange { get; set; }

        public decimal Change
        {
            get
            {
                return Price - DayOpen;
            }
        }

        public double PercentChange
        {
            get
            {
                return (double)Math.Round(Change / Price, 4);
            }
        }
        decimal _price;

        public decimal Price
        {
            get
            {
                return _price;
            }
            set
            {
                if (_price == value)
                {
                    return;
                }

                LastChange = value - _price;
                _price = value;

                if (DayOpen == 0)
                {
                    DayOpen = _price;
                }
                if (_price < DayLow || DayLow == 0)
                {
                    DayLow = _price;
                }
                if (_price > DayHigh)
                {
                    DayHigh = _price;
                }
            }
        }
    }

    public class Symbol : ValueObject
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
