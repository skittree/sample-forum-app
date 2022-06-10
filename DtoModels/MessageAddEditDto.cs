using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using System.Text.Json.Serialization;

namespace Task3.DtoModels
{
    public class MessageAddEditDto
    {
        [JsonPropertyName("text")]
        public string Text { get; set; }
    }
}
