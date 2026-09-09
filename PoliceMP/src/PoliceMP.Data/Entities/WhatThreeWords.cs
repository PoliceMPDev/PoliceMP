using System.ComponentModel.DataAnnotations;

namespace PoliceMP.Data.Entities
{
    public class WhatThreeWords
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public float PosX { get; set; }
        public float PosY { get; set; }
        public float PosZ { get; set; }
    }
}