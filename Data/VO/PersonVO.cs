﻿using RestWithASPNET.Model.Base;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace RestWithASPNETUdemy.Data.VO
{

    public class PersonVO
    {
        //Serialização dos VO, isso vai mudar a forma que o JSON do objeto sera criado.
        //Esse codigo ficara comentado.

        //[JsonPropertyName("code")]
        public long Id { get; set; }

        //[JsonPropertyName("name")]
        public string FirstName { get; set; }

        //[JsonPropertyName("last_name")]
        public string LastName { get; set; }

        //[JsonIgnore]
        public string Address { get; set; }

        //[JsonPropertyName("sex")]
        public string Gender { get; set; }
    }
}
