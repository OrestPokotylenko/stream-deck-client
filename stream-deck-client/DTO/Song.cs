﻿namespace stream_deck_client.DTO
{
    public class Song
    {
        public string _name;
        public string _duration;

        public Song(string name, string duration)
        {
            _name = name;
            _duration = duration;
        }
    }
}