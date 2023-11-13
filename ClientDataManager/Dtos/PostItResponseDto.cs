using Newtonsoft.Json;

namespace ClientDataManager.Dtos
{
        public class PostItResponseDto
        {
            public string? Status { get; set; }
            public bool Success { get; set; }
            public string? Message { get; set; }
            public int MessageCode { get; set; }
            public int Total { get; set; }
            public List<DataItemDto> Data { get; set; }
            public PageDto Page { get; set; }
        }

        public class DataItemDto
        {
            [JsonProperty("post_code")]
            public string? PostCode { get; set; }
            public string? Address { get; set; }
            public string? Street { get; set; }
            public string? Number { get; set; }

            [JsonProperty("only_number")]
            public string? OnlyNumber { get; set; }
            public string? Housing { get; set; }
            public string? City { get; set; }
            public string? Municipality { get; set; }
            public string? Post { get; set; }
            public string? Mailbox { get; set; }
            public string? Company { get; set; }
        }

        public class PageDto
        {
            public int Current { get; set; }
            public int Total { get; set; }
        }
    }
