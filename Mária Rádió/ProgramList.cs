using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Maria_Radio
{
    public class ProgramList<T>: List<T> where T : Data.Program
    {
        public Data.Program GetCurrentProgram()
        {
            foreach (T program in this)
            {
                if (program.Current)
                {
                    return program;
                }
            }

            return null;
        }

        public Data.Program GetNextProgram()
        {
            bool next = false;

            foreach (T program in this)
            {
                if (next)
                {
                    return program;
                }

                if (program.Current)
                {
                    next = true;
                }
            }

            return null;
        }
    }
}
