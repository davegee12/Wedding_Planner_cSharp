using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;


namespace WeddingPlanner.Models
{
    public class Wedding
    {
        [Key]
        public int WeddingId{get;set;}

        // Wedder1 Name
        [Required(ErrorMessage="Name of who is getting married is required")]
        public string Wedder1{get;set;}

        // Wedder2 Name
        [Required(ErrorMessage="Name of who is getting married is required")]
        public string Wedder2{get;set;}

        // Wedding Date
        [Required(ErrorMessage="Please enter the wedding date")]
        public DateTime WeddingDate{get;set;}

        // Wedding Address
        [Required(ErrorMessage="Please enter the wedding address")]
        public string Address{get;set;}

        public DateTime CreatedAt{get;set;} = DateTime.Now;
        public DateTime UpdatedAt{get;set;} = DateTime.Now;

        public List<RSVP> GuestList{get;set;}

        public int RegUserId {get;set;}
        public RegUser Creator {get;set;}
    }
}