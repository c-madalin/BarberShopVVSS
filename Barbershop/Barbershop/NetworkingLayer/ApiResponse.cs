using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Barbershop.NetworkingLayer
{
    internal sealed class ApiResponse
    {
        [JsonPropertyName("status")]
        public string Status { get; set; } = string.Empty;
    }
}
