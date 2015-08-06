using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Maria_Radio
{
    public class MountPoint
    {
        public const int INDEX_TITLE = 0;
        public const int INDEX_DESCRIPTION = 1;
        public const int INDEX_CONTENT_TYPE = 2;
        public const int INDEX_UPTIME = 3;
        public const int INDEX_BITRATE = 4;
        public const int INDEX_CURRENTLISTENERS = 5;
        public const int INDEX_PEAKLISTENERS = 6;
        public const int INDEX_GENRE = 7;
        public const int INDEX_URL = 8;
        public const int INDEX_CURRENT_SONG = 9;

        private string title, description, contentType, genre, url, currentSong;
        private DateTime upTime;
        private int bitrate, currentListeners, peakListeners;

        public MountPoint(string title, string description, string contentType, DateTime upTime, int bitrate, int currentListeners, int peakListeners, string genre, string url, string currentSong)
        {
            this.title = title;
            this.description = description;
            this.contentType = contentType;
            this.upTime = upTime;
            this.bitrate = bitrate;
            this.currentListeners = currentListeners;
            this.peakListeners = peakListeners;
            this.genre = genre;
            this.url = url;
            this.currentSong = currentSong;
        }

        public string Title
        {
            get { return title; }
        }

        public string Description
        {
            get { return description; }
        }

        public string ContentType
        {
            get { return contentType; }
        }

        public string Genre
        {
            get { return genre; }
        }

        public string Url
        {
            get { return url; }
        }

        public string CurrentSong
        {
            get { return currentSong; }
        }

        public DateTime UpTime
        {
            get { return upTime; }
        }

        public int Bitrate
        {
            get { return bitrate; }
        }

        public int CurrentListeners
        {
            get { return currentListeners; }
        }

        public int PeakListeners
        {
            get { return peakListeners; }
        }

        public override string ToString()
        {
            return String.Format("{0}, {1} kbit/s", title, bitrate);
        }
    }
}
