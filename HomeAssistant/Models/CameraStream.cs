﻿using Microsoft.AspNetCore.SignalR;

namespace HomeAssistant.Models
{
    public class CameraStream
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Room Room { get; set; }
        public string Url { get; set; }


    }
}
