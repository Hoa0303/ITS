﻿namespace ITS_BE.Models
{
    public class Category : IBaseEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
    }
}
