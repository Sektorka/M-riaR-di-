using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Maria_Radio.Data
{
    public class Program
    {
        private DateTime dateTime;
        private String title, description;
        private bool current;

        public Program(string title, string description, DateTime dateTime, bool current)
        {
            this.title = title;
            this.description = description;
            this.dateTime = dateTime;
            this.current = current;
        }

        public string Title
        {
            get { return title; }
        }

        public string Description
        {
            get { return description; }
        }

        public DateTime DateTime
        {
            get { return dateTime; }
        }

        public bool Current
        {
            get { return current; }
            set { current = value; }
        }
    }
}
