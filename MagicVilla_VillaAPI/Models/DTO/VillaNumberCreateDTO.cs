﻿using System.ComponentModel.DataAnnotations;

namespace MagicVilla_VillaAPI.Models.DTO
{
    public class VillaNumberCreateDTO
    {
        [Required]
        public int VillaNO { get; set; }
        public string SpecialDetails { get; set; }
    }
}