﻿namespace ITS_BE.Response
{
    public class GHNData
    {
        public string Order_code { get; set; }
        public DateTime Expected_delivery_time { get; set; }
    }
    public class GHNResponse
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public GHNData? Data { get; set; }
    }
}
